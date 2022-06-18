﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CustomPlayerEffects;
using Hints;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.MicroHID;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Mirror;
using PlayableScps;
using PlayerStatsSystem;
using Respawning;
using Scp914;
using UnityEngine;

using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;

using CommonPlugin.Components;
using CommonPlugin.Extensions;
using CommonPlugin.Patches;

namespace CommonPlugin
{
    public class EventHandlers : IEventHandler079LevelUp, IEventHandlerCheckEscape, IEventHandlerConsumableUse, IEventHandlerCheckRoundEnd, IEventHandlerContain106, IEventHandlerScpDeathAnnouncement,
		IEventHandlerLCZDecontaminate, IEventHandlerPlayerDie, IEventHandlerPlayerHurt, IEventHandlerPlayerJoin, IEventHandlerPlayerLeave,
		IEventHandlerPlayerPickupItem, IEventHandlerPlayerTriggerTesla, IEventHandlerPocketDimensionDie, IEventHandlerPocketDimensionEnter, IEventHandlerPocketDimensionExit,
		IEventHandlerRoundEnd, IEventHandlerRoundStart, IEventHandlerScp096AddTarget, IEventHandlerSCP914Activate, IEventHandlerSetRole, IEventHandlerTeamRespawn,
		IEventHandlerWaitingForPlayers, IEventHandlerWarheadChangeLever, IEventHandlerWarheadStopCountdown, IEventHandlerWarheadDetonate, IEventHandlerSetInventory, IEventHandlerCassieTeamAnnouncement
	{
		public CommonPlugin Plugin { get; set; }

		public EventHandlers() { }

		public EventHandlers(CommonPlugin Plugin)
		{
			this.Plugin = Plugin;

			Random = new System.Random();
			Coroutines = new List<CoroutineHandle>();
		}

		// 生命值配置
		public const float ClassdMaxHP = 100.0f;
		public const float ScientistMaxHP = 100.0f;
		public const float MtfMaxHP = 125.0f;
		public const float MtfCaptainMaxHP = 150.0f;
		public const float ChaosMaxHP = 125.0f;
		public const float ChaosMarauderMaxHP = 150.0f;

		public const float Scp0492MaxHP = 500.0f;
		public const float Scp049MaxHP = 2500.0f;
		public const float Scp079MaxHP = 100000.0f;
		public const float Scp096MaxHP = 2000.0f;
		public const float Scp106MaxHP = 1200.0f;
		public const float Scp173MaxHP = 3000.0f;
		public const float Scp682MaxHP = 3600.0f;
		public const float Scp939MaxHP = 2200.0f;

		public const int lateJoinTime = 90;
		private const int maxPlayer = 33;
		private const float medkitHeal = 70.0f;
		private const float medkitHeal2 = 10.0f;
		private const int selfHealCooldown = 3;
		private const float traumaDamage = 0.25f;

		private const float Scp035Hit = 4.0f;
		private const float Scp035Heal = 35.0f;
		private const float Scp049Heal = 5.0f;
		private const float Scp049Heal2 = 2.0f;
		private const int Scp079ChangeTime = 30;
		private const int PowerCutCooldown = 30;
		public const float Scp096MaxShield = 1000.0f;
		public const float Scp096MaxShield2 = 1250.0f;
		private const float Scp096Heal = 2.0f;
		public const int Scp106Cooldown = 30;
		private const float Scp106Kill = 20.0f;
		private const float Scp173Heal = 7.5f;
		private const float Scp682Heal = 8.0f;
		private const float Scp682Kill = 40.0f;
		private const float Scp939Hit = 25.0f;

		private bool bRoundEnd;
		private bool bWarhead;
		private double dWarheadRate;
		private readonly System.Random Random;
		private readonly List<CoroutineHandle> Coroutines;
		public static readonly List<ushort> TrapItems = new List<ushort>();
		private Broadcast PersonalBC;

		public static int Scp035id = 0;
		public static int Scp079id = 0;
		public static int Scp181id = 0;
		public static int Scp682id = 0;
		public static int Scp703id = 0;

		public static ushort Scp035ItemId = 0;
		private static Scp079Level Scp079Lv = Scp079Level.Level_1;
		public static int Scp106LastPlace = 0;

		public static bool bScp035Detected = false;
		public static bool bScp703Working = false;

		// Smod2 Interface
		public void On079LevelUp(Player079LevelUpEvent ev)
		{
			switch (ev.Player.SCP079Data.Level)
			{
				case 1:
					Scp079Lv = Scp079Level.Level_2;
					PluginEx.FlickerLights(20, 30);
					break;

				case 2:
					Scp079Lv = Scp079Level.Level_3;
					PluginEx.FlickerLights(30, 40);
					break;

				case 3:
					Scp079Lv = Scp079Level.Level_4;
					PluginEx.FlickerLights(40, 50);
					break;

				case 4:
					Scp079Lv = Scp079Level.Level_5;
					PluginEx.FlickerLights(50, 60);
					PluginEx.SetServerBadge(ev.Player.GetHub().serverRoles, "SCP-079");
					break;
			}
		}

		public void OnCassieTeamAnnouncement(CassieTeamAnnouncementEvent ev)
		{
			ev.SCPsLeft += Scp035id == 0 ? 0 : 1;
		}

		public void OnScpDeathAnnouncement(ScpDeathAnnouncementEvent ev)
        {
			if (ev.DeadPlayer.PlayerID== Scp682id)
            {
				PluginEx.CassieMessage("SCP 6 8 2 CONTAINS SUCCESSFULLY", $"<color=#FF0000>SCP-682</color> 收容成功!");
				ev.ShouldPlay = false;
			}
        }

		public void OnChangeLever(WarheadChangeLeverEvent ev)
		{
			if (ev.Player.GetHub().characterClassManager.NetworkCurClass == RoleType.Scp0492 && !AlphaWarheadOutsitePanel.nukeside.enabled)
				ev.Allow = false;
		}

		public void OnCheckEscape(PlayerCheckEscapeEvent ev)
		{
			if (ev.Player.PlayerID == Scp035id)
				ev.AllowEscape = false;
			else if (ev.Player.PlayerID == Scp181id)
			{
				Scp181id = 0;
				PluginEx.ClearServerBadge(ev.Player.GetHub().serverRoles);
			}
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (Scp035id != 0)
			{
				ReferenceHub hub;

				bool bEndRound = true;

				foreach (GameObject gameObject in PlayerManager.players)
				{
					hub = ReferenceHub.GetHub(gameObject);

					if (!bEndRound)
						break;

					if (hub.playerId == Scp035id)
						continue;

					switch (hub.characterClassManager.NetworkCurClass) {
						case RoleType.ClassD:
						case RoleType.Scientist:
						case RoleType.FacilityGuard:
						case RoleType.NtfPrivate:
						case RoleType.NtfSergeant:
						case RoleType.NtfSpecialist:
						case RoleType.NtfCaptain:
							bEndRound = false;
							break;
					}
				}

				if (!bEndRound)
					ev.Status = RoundEndStatus.ON_GOING;
				else
					ev.Status = RoundEndStatus.NO_VICTORY;
			}
		}

		public void OnContain106(PlayerContain106Event ev)
		{
			if (ev.Player.PlayerID == Scp035id)
				ev.ActivateContainment = false;
		}

		public void OnDecontaminate() => bScp703Working = false;

		public void OnDetonate()
        {
			Timing.RunCoroutine(Timing_OnDetonate());
        }

		public void OnConsumableUse(PlayerConsumableUseEvent ev)
		{
			ReferenceHub hub = ev.Player.GetHub();
			HealthStat healthStat = hub.GetHealthStat();
			HealthController healthControler = hub.GetHealthControler();

			ItemType itemType = (ItemType)ev.ConsumableItem;
			switch (itemType)
			{
				case ItemType.Medkit:
					if (hub.playerId != Scp035id)
						healthControler.MaxHealth = healthControler.MaxHealth + medkitHeal2 > healthControler.MaxHealth2 ? healthControler.MaxHealth2 : healthControler.MaxHealth + medkitHeal2;

					healthStat.ServerHeal(hub.playerId == Scp035id ? Scp035Heal : medkitHeal);
					hub.playerEffectsController.UseMedicalItem(itemType);
					break;

				case ItemType.SCP500:
					if (hub.playerId != Scp035id)
						healthControler.MaxHealth = healthControler.MaxHealth2;

					healthStat.ServerHeal(healthControler.MaxHealth);
					hub.playerEffectsController.UseMedicalItem(itemType);
					break;

				case ItemType.SCP207:
					if (hub.playerId != Scp035id)
						hub.playerEffectsController.GetEffect<Scp207>().Intensity = 1;
					break;

				case ItemType.Adrenaline:
				case ItemType.Painkillers:
					Timing.RunCoroutine(Timing_OnConsumableUse(hub, healthControler, itemType, hub.characterClassManager.NetworkCurClass));
					break;

				case ItemType.SCP1853:
					hub.playerEffectsController.GetEffect<Scp1853>().Intensity += 1;
					break;
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (GameCore.RoundStart.singleton.NetworkTimer != -1 || ev.DamageTypeVar == DamageType.NONE)
				return;

			dWarheadRate += Random.NextDouble() + 0.345;

			ReferenceHub killer = ev.Killer.GetHub();
			ReferenceHub player = ev.Player.GetHub();
			
			switch (player.characterClassManager.NetworkCurClass)
			{
				case RoleType.Scp049:
				case RoleType.Scp079:
				case RoleType.Scp096:
				case RoleType.Scp106:
				case RoleType.Scp173:
				case RoleType.Scp93953:
				case RoleType.Scp93989:
					PluginEx.ClearServerBadge(player.serverRoles);
					return;

				default:
					if (player.playerId == Scp035id)
					{
						Scp035id = 0;
						PluginEx.ClearServerBadge(player.serverRoles);
						PluginEx.CassieMessage("SCP 0 3 5 CONTAINS SUCCESSFULLY", $"<color=#FF0000>SCP-035</color> 收容成功, 收容者: {killer.nicknameSync.DisplayName}");
						return;
					}
					else if (player.playerId == Scp181id)
					{
						Scp181id = 0;
						PluginEx.ClearServerBadge(player.serverRoles);
						PluginEx.CassieMessage("SCP 1 8 1 CONTAINS SUCCESSFULLY", $"<color=#FF0000>SCP-181</color> 收容成功, 收容者: {killer.nicknameSync.DisplayName}");
						return;
					}
					break;
			}

			if (ev.Killer.PlayerID != ev.Player.PlayerID)
			{
				if (killer.playerId == Scp035id)
				{
					RoundSummary.KilledBySCPs++;
					if (!bScp035Detected)
					{
						bScp035Detected = true;

						PluginEx.SetServerBadge(killer.serverRoles, "SCP-035");
						PluginEx.CassieMessage("WARNING SCP 0 3 5 CONTAINMENT BREACH DETECTED", "警告, 检测到<color=#FF0000>SCP-035</color>突破收容");
					}
					return;
				}
				else if (killer.playerId == Scp682id)
                {
					HealthController healthController = killer.GetHealthControler();
					HealthStat healthStat = killer.GetHealthStat();

					healthController.MaxHealth += Scp682Kill;
					healthController.Heal = healthController.Heal2 += 0.25f;
					healthStat.ServerHeal(Scp682Kill);

					if (player.playerEffectsController.GetEffect<Scp207>().Intensity > 0)
						killer.playerEffectsController.GetEffect<Scp207>().Intensity = 1;
                }
			}

			if (ev.DamageTypeVar == DamageType.POCKET_DECAY)
				Timing.RunCoroutine(Timing_OnPocketDimensionDie());
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (ev.Attacker.PlayerID != ev.Player.PlayerID)
			{
				ReferenceHub attacker = ev.Attacker.GetHub();
				ReferenceHub player = ev.Player.GetHub();
				HealthController healthController = player.GetHealthControler();

				if (attacker.playerId == Scp035id)
				{
					switch (player.characterClassManager.NetworkCurClass)
					{
						case RoleType.Scp049:
						case RoleType.Scp0492:
						case RoleType.Scp096:
						case RoleType.Scp106:
						case RoleType.Scp173:
						case RoleType.Scp93953:
						case RoleType.Scp93989:
							ev.Damage = 0.0f;
							return;
					}
				}

				if (player.playerId == Scp035id)
				{
					switch (ev.DamageType)
					{
						case DamageType.COM15:
						case DamageType.E11_SR:
						case DamageType.CROSSVEC:
						case DamageType.FSP9:
						case DamageType.LOGICER:
						case DamageType.COM18:
						case DamageType.REVOLVER:
						case DamageType.AK:
						case DamageType.SHOTGUN:
							ev.Damage = (ev.Damage > 40.0f) ? Scp035Hit * 4 : Scp035Hit;
							healthController.MaxHealth -= ev.Damage > 40.0f ? 4.0f : 1.0f;
							return;
					}

					switch (attacker.characterClassManager.NetworkCurClass)
					{
						case RoleType.Scp049:
						case RoleType.Scp0492:
						case RoleType.Scp096:
						case RoleType.Scp106:
						case RoleType.Scp173:
						case RoleType.Scp93953:
						case RoleType.Scp93989:
							ev.Damage = 0.0f;
							return;
					}
				}
				else if (player.playerId == Scp181id)
				{
					switch (attacker.characterClassManager.NetworkCurClass)
					{
						case RoleType.Scp049:
						case RoleType.Scp0492:
						case RoleType.Scp096:
						case RoleType.Scp106:
						case RoleType.Scp173:
						case RoleType.Scp93953:
						case RoleType.Scp93989:
							int itemCount = player.inventory.UserInventory.Items.Count;

							if (itemCount > 0 && !BlastDoor.OneDoor.isClosed)
							{
								ev.Damage = 0.0f;

								ushort itemSerial = player.inventory.UserInventory.Items.ElementAt(Random.Next(itemCount)).Key;
								player.inventory.ServerRemoveItem(itemSerial, null);

								player.hints.Show(
									new TextHint("<size=30><b>你使用一件物品抵挡了一次来自<color=#FF0000>SCP</color>的伤害</b></size>",
									new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));
								attacker.hints.Show(
									new TextHint("<size=30><b><color=#FF0000>SCP-181</color>使用一件物品抵挡了你的一次伤害</b></size>",
									new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));
								return;
							}
							break;
					}
				}

				switch (attacker.characterClassManager.NetworkCurClass)
				{
					case RoleType.Scp93953:
					case RoleType.Scp93989:
						if (attacker.playerId == Scp682id)
						{
							ev.Damage = 100.0f;
							healthController.MaxHealth -= healthController.MaxHealth2 * 0.3f;
						}
						else
						{
							ev.Damage = Random.Next(70, 100);
							healthController.MaxHealth -= Scp939Hit;
							player.playerEffectsController.EnableEffect<Amnesia>(3.0f, true);
						}
						return;
				}

				switch (player.characterClassManager.NetworkCurClass)
				{
					case RoleType.ClassD:
					case RoleType.Scientist:
					case RoleType.FacilityGuard:
					case RoleType.NtfPrivate:
					case RoleType.NtfSergeant:
					case RoleType.NtfSpecialist:
					case RoleType.NtfCaptain:
					case RoleType.ChaosConscript:
					case RoleType.ChaosRifleman:
					case RoleType.ChaosRepressor:
					case RoleType.ChaosMarauder:
						healthController.MaxHealth -= ev.Damage * traumaDamage;
						break;
				}
			}
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (MessageQueue.Messages.ContainsKey(ev.Player.PlayerID))
				return;

			ReferenceHub hub = ev.Player.GetHub();
			PersonMessage personMessage = new PersonMessage(hub.playerId, hub);
			MessageQueue.Messages.Add(hub.playerId, personMessage);
			Coroutines.Add(Timing.RunCoroutine(Timing_PersonMessage(personMessage, hub.nicknameSync.connectionToClient), "Mq" + hub.playerId));
		}

		public void OnPlayerLeave(PlayerLeaveEvent ev)
		{
			ev.Player.GetHub().characterClassManager.NetworkCurClass = RoleType.Spectator;
			Timing.KillCoroutines("Mq" + ev.Player.PlayerID);

			lock (MessageQueue.Messages)
				MessageQueue.Messages.Remove(ev.Player.PlayerID);
		}

		public void OnPlayerPickupItem(PlayerPickupItemEvent ev)
		{
			ItemType itemType = (ItemType)ev.Item.ItemType;

			switch (itemType)
			{
				case ItemType.KeycardContainmentEngineer:
				case ItemType.KeycardFacilityManager:
				case ItemType.KeycardO5:
				case ItemType.Medkit:
				case ItemType.SCP500:
				case ItemType.SCP207:
				case ItemType.GrenadeHE:
				case ItemType.GrenadeFlash:
				case ItemType.SCP268:
				case ItemType.Adrenaline:
				case ItemType.SCP1853:
					if (TrapItems.Contains(ev.Item.SerialNumber))
					{
						ReferenceHub hub = ev.Player.GetHub();

						if (hub.scp106PlayerScript.goingViaThePortal)
						{
							ev.ChangeTo = Smod2.API.ItemType.NONE;

							return;
						}
						else if (hub.playerId == Scp035id)
						{
							ev.ChangeTo = Smod2.API.ItemType.NONE;
							hub.hints.Show(
								new TextHint("<size=30><b>该物品为<color=#FF0000>SCP-106</color>的诱捕陷阱</b></size>",
								new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));

							return;
						}

						ev.ChangeTo = Smod2.API.ItemType.NONE;
						ev.Item.Remove();
						TrapItems.Remove(ev.Item.SerialNumber);

						if (BlastDoor.OneDoor.isClosed)
							hub.playerStats.DealDamage(new UniversalDamageHandler(-1.0f, DeathTranslations.PocketDecay, null));
						else
							Timing.RunCoroutine(Timing_EnteringPocketDimension(hub, null, true));
					}
					break;

				case ItemType.Coin:
					if (ev.Item.SerialNumber == Scp035ItemId)
                    {
						ev.ChangeTo = Smod2.API.ItemType.NONE;
						ev.Item.Remove();
						PluginEx.SetScp035(ev.Player.GetHub());
					}
					break;
			}
		}

		public void OnPlayerTriggerTesla(PlayerTriggerTeslaEvent ev)
		{
			if (ev.Player.PlayerID == Scp181id)
				ev.Triggerable = false;
		}

		public void OnPocketDimensionDie(PlayerPocketDimensionDieEvent ev)
		{
			if (ev.Player.PlayerID == Scp181id && !BlastDoor.OneDoor.isClosed)
			{
				ev.Die = false;

				ReferenceHub hub = ev.Player.GetHub();
				HCZRoom hczRoomType = (HCZRoom)Random.Next((int)HCZRoom.HczRoomCount);

				switch (hczRoomType)
				{
					case HCZRoom.Scp049Room:
						hub.playerMovementSync.OverridePosition(MapManager.Scp049Room.Position);
						break;

					case HCZRoom.Scp079Room:
						hub.playerMovementSync.OverridePosition(MapManager.Scp079Room.Position);
						break;

					case HCZRoom.Scp096Room:
						hub.playerMovementSync.OverridePosition(MapManager.Scp096Room.Position);
						break;

					case HCZRoom.Scp106Room:
						hub.playerMovementSync.OverridePosition(MapManager.Scp106Room.Position);
						break;

					case HCZRoom.Scp939Room:
						hub.playerMovementSync.OverridePosition(MapManager.Scp939Room.Position);
						break;

					case HCZRoom.MircoHIDRoom:
						hub.playerMovementSync.OverridePosition(MapManager.MircohidRoom.Position);
						break;

					case HCZRoom.ServersRoom:
						hub.playerMovementSync.OverridePosition(MapManager.ServersRoom.Position);
						break;
				}
			}
		}

		public void OnPocketDimensionEnter(PlayerPocketDimensionEnterEvent ev)
		{
			ev.Allow = false;

			Timing.RunCoroutine(Timing_EnteringPocketDimension(ev.Player.GetHub(), ev.Attacker.GetHub(), false));
		}

		public void OnPocketDimensionExit(PlayerPocketDimensionExitEvent ev)
		{
			HCZRoom roomType = (HCZRoom)Random.Next((int)HCZRoom.HczRoomCount);
			switch (roomType)
			{
				case HCZRoom.Scp049Room:
					ev.ExitPosition = MapManager.Scp049Room.Position2;
					break;

				case HCZRoom.Scp079Room:
					ev.ExitPosition = MapManager.Scp079Room.Position2;
					break;

				case HCZRoom.Scp096Room:
					ev.ExitPosition = MapManager.Scp096Room.Position2;
					break;

				case HCZRoom.Scp106Room:
					ev.ExitPosition = MapManager.Scp106Room.Position2;
					break;

				case HCZRoom.Scp939Room:
					ev.ExitPosition = MapManager.Scp939Room.Position2;
					break;

				case HCZRoom.MircoHIDRoom:
					ev.ExitPosition = MapManager.MircohidRoom.Position2;
					break;

				case HCZRoom.ServersRoom:
					ev.ExitPosition = MapManager.ServersRoom.Position2;
					break;
			}
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (Scp035id != 0)
				if (ev.Round.Stats.DClassEscaped == 0 && ev.Round.Stats.ScientistsEscaped == 0)
					ev.LeadingTeam = LeadingTeam.ANOMALIES;
				else
					ev.LeadingTeam = LeadingTeam.DRAW;

			bRoundEnd = true;
		}

		public void OnRoundStart(RoundStartEvent ev) => Timing.RunCoroutine(Timing_OnRoundStart());

		public void OnScp096AddTarget(Scp096AddTargetEvent ev)
		{
			if (ev.Target.PlayerID == Scp035id)
				ev.Allow = false;
		}

		public void OnSCP914Activate(SCP914ActivateEvent ev)
		{
			List<Item> items = new List<Item>();

			foreach (Item item in ev.ItemInputs)
				if (TrapItems.Contains(item.SerialNumber))
					items.Add(item);
			foreach (Item item in items)
				ev.ItemInputs.Remove(item);

			Timing.RunCoroutine(Timing_OnScp914Activate(ev.PlayerInputs, (Scp914KnobSetting)ev.KnobSetting));
		}

		public void OnSetInventory(PlayerSetInventoryEvent ev)
		{
			switch((RoleType)ev.PreviousRole.RoleID)
            {
				case RoleType.FacilityGuard:
					ev.Ammo.Add(AmmoType.AMMO_556_X45, 40);
					ev.Ammo.Add(AmmoType.AMMO_9_X19, 200);

					ev.Items.Add(Smod2.API.ItemType.KEYCARD_GUARD);
					ev.Items.Add(Smod2.API.ItemType.GUN_FSP9);
					ev.Items.Add(Smod2.API.ItemType.ADRENALINE);
					ev.Items.Add(Smod2.API.ItemType.MEDKIT);
					ev.Items.Add(Smod2.API.ItemType.GRENADE_HE);
					ev.Items.Add(Smod2.API.ItemType.GRENADE_FLASH);
					ev.Items.Add(Smod2.API.ItemType.ARMOR_LIGHT);
					ev.Items.Add(Smod2.API.ItemType.RADIO);
					break;

				case RoleType.NtfPrivate:
					ev.Ammo.Add(AmmoType.AMMO_556_X45, 80);
					ev.Ammo.Add(AmmoType.AMMO_9_X19, 200);

					ev.Items.Add(Smod2.API.ItemType.KEYCARD_NTF_OFFICER);
					ev.Items.Add(Smod2.API.ItemType.GUN_CROSSVEC);
					ev.Items.Add(Smod2.API.ItemType.MEDKIT);

					if (Random.Next(3) == 0)
						ev.Items.Add(Smod2.API.ItemType.GRENADE_HE);
					else if (Random.Next(3) == 0)
						ev.Items.Add(Smod2.API.ItemType.GRENADE_FLASH);

					ev.Items.Add(Smod2.API.ItemType.ARMOR_COMBAT);
					ev.Items.Add(Smod2.API.ItemType.RADIO);
					break;

				case RoleType.NtfSergeant:
					ev.Ammo.Add(AmmoType.AMMO_556_X45, 200);
					ev.Ammo.Add(AmmoType.AMMO_44_CAL, 60);
					ev.Ammo.Add(AmmoType.AMMO_9_X19, 120);

					ev.Items.Add(Smod2.API.ItemType.KEYCARD_NTF_LIEUTENANT);
					ev.Items.Add(Smod2.API.ItemType.GUN_E11_SR);
					ev.Items.Add(Smod2.API.ItemType.GUN_REVOLVER);
					ev.Items.Add(Smod2.API.ItemType.MEDKIT);
					ev.Items.Add(Smod2.API.ItemType.GRENADE_HE);
					ev.Items.Add(Smod2.API.ItemType.ARMOR_COMBAT);
					ev.Items.Add(Smod2.API.ItemType.RADIO);
					break;

				case RoleType.NtfSpecialist:
					ev.Ammo.Add(AmmoType.AMMO_556_X45, 200);
					ev.Ammo.Add(AmmoType.AMMO_9_X19, 120);

					ev.Items.Add(Smod2.API.ItemType.KEYCARD_NTF_LIEUTENANT);
					ev.Items.Add(Smod2.API.ItemType.GUN_E11_SR);
					ev.Items.Add(Smod2.API.ItemType.MICRO_HID);
					ev.Items.Add(Smod2.API.ItemType.SCP_500);
					ev.Items.Add(Smod2.API.ItemType.SCP_1853);
					ev.Items.Add(Smod2.API.ItemType.GRENADE_HE);
					ev.Items.Add(Smod2.API.ItemType.ARMOR_COMBAT);
					ev.Items.Add(Smod2.API.ItemType.RADIO);
					break;

				case RoleType.NtfCaptain:
					ev.Ammo.Add(AmmoType.AMMO_12_GAUGE, 70);
					ev.Ammo.Add(AmmoType.AMMO_556_X45, 200);

					ev.Items.Add(Smod2.API.ItemType.KEYCARD_NTF_COMMANDER);
					ev.Items.Add(Smod2.API.ItemType.GUN_E11_SR);
					ev.Items.Add(Smod2.API.ItemType.GUN_SHOTGUN);
					ev.Items.Add(Smod2.API.ItemType.ADRENALINE);
					ev.Items.Add(Smod2.API.ItemType.MEDKIT);
					ev.Items.Add(Smod2.API.ItemType.GRENADE_HE);
					ev.Items.Add(Smod2.API.ItemType.ARMOR_HEAVY);
					ev.Items.Add(Smod2.API.ItemType.RADIO);
					break;

				case RoleType.ChaosConscript:
					ev.Ammo.Add(AmmoType.AMMO_762_X39, 160);

					ev.Items.Add(Smod2.API.ItemType.KEYCARD_CHAOS_INSURGENCY);
					ev.Items.Add(Smod2.API.ItemType.GUN_AK);
					ev.Items.Add(Smod2.API.ItemType.MEDKIT);
					ev.Items.Add(Smod2.API.ItemType.PAINKILLERS);
					ev.Items.Add(Smod2.API.ItemType.ARMOR_COMBAT);
					break;

				case RoleType.ChaosRifleman:
					ev.Ammo.Add(AmmoType.AMMO_12_GAUGE, 30);
					ev.Ammo.Add(AmmoType.AMMO_762_X39, 200);

					ev.Items.Add(Smod2.API.ItemType.KEYCARD_CHAOS_INSURGENCY);
					ev.Items.Add(Smod2.API.ItemType.GUN_AK);
					ev.Items.Add(Smod2.API.ItemType.MEDKIT);
					ev.Items.Add(Smod2.API.ItemType.PAINKILLERS);
					ev.Items.Add(Smod2.API.ItemType.ARMOR_COMBAT);
					break;

				case RoleType.ChaosRepressor:
					ev.Ammo.Add(AmmoType.AMMO_12_GAUGE, 100);
					ev.Ammo.Add(AmmoType.AMMO_44_CAL, 60);
					ev.Ammo.Add(AmmoType.AMMO_762_X39, 60);

					ev.Items.Add(Smod2.API.ItemType.KEYCARD_CHAOS_INSURGENCY);
					ev.Items.Add(Smod2.API.ItemType.GUN_SHOTGUN);
					ev.Items.Add(Smod2.API.ItemType.GUN_REVOLVER);
					ev.Items.Add(Smod2.API.ItemType.MEDKIT);
					ev.Items.Add(Smod2.API.ItemType.PAINKILLERS);
					ev.Items.Add(Smod2.API.ItemType.ARMOR_COMBAT);
					break;

				case RoleType.ChaosMarauder:
					ev.Ammo.Add(AmmoType.AMMO_12_GAUGE, 70);
					ev.Ammo.Add(AmmoType.AMMO_762_X39, 300);

					ev.Items.Add(Smod2.API.ItemType.KEYCARD_CHAOS_INSURGENCY);
					ev.Items.Add(Smod2.API.ItemType.GUN_LOGICER);
					ev.Items.Add(Smod2.API.ItemType.GUN_SHOTGUN);
					ev.Items.Add(Smod2.API.ItemType.ADRENALINE);
					ev.Items.Add(Smod2.API.ItemType.MEDKIT);
					ev.Items.Add(Smod2.API.ItemType.ARMOR_COMBAT);
					break;
			}
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			RoleType roleType = (RoleType)ev.RoleType;
			ReferenceHub hub = ev.Player.GetHub();

			if (roleType == RoleType.Spectator)
			{
				if (hub.playerId == Scp035id)
				{
					Scp035id = 0;
					PluginEx.ClearServerBadge(hub.serverRoles);
				}
				else if (hub.playerId == Scp079id)
                {
					Scp079id = 0;
					PluginEx.ClearServerBadge(hub.serverRoles);
				}
				else if (hub.playerId == Scp181id)
				{
					Scp181id = 0;
					PluginEx.ClearServerBadge(hub.serverRoles);
				}
				else if (hub.playerId == Scp682id)
				{
					Scp682id = 0;
					PluginEx.ClearServerBadge(hub.serverRoles);
				}
			}
			else
				Timing.RunCoroutine(Timing_OnSetRole(hub));
		}

		public void OnStopCountdown(WarheadStopEvent ev) => ev.Cancel = bWarhead == true;

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			dWarheadRate += ((maxPlayer - PlayerManager.players.Count + 10) * 0.6);

			foreach (Ragdoll doll in Object.FindObjectsOfType<Ragdoll>())
				NetworkServer.Destroy(doll.gameObject);

			if (dWarheadRate > 25.0)
				foreach (ItemPickupBase itemPickupBase in Object.FindObjectsOfType<ItemPickupBase>())
				{
					switch (itemPickupBase.NetworkInfo.ItemId)
					{
						case ItemType.KeycardJanitor:
						case ItemType.KeycardScientist:
						case ItemType.KeycardResearchCoordinator:
						case ItemType.KeycardZoneManager:
						case ItemType.KeycardGuard:
						case ItemType.KeycardNTFOfficer:
						case ItemType.KeycardNTFLieutenant:
						case ItemType.KeycardNTFCommander:
						case ItemType.KeycardChaosInsurgency:
						case ItemType.Radio:
						case ItemType.Ammo12gauge:
						case ItemType.Ammo556x45:
						case ItemType.Ammo44cal:
						case ItemType.Ammo762x39:
						case ItemType.Ammo9x19:
						case ItemType.Painkillers:
						case ItemType.Coin:
						case ItemType.ArmorLight:
						case ItemType.ArmorCombat:
						case ItemType.ArmorHeavy:
							itemPickupBase.DestroySelf();
							break;

						case ItemType.Flashlight:
							if (Scp703id != itemPickupBase.NetworkInfo.Serial)
								itemPickupBase.DestroySelf();
							break;

						case ItemType.GunCOM15:
						case ItemType.Medkit:
						case ItemType.SCP500:
						case ItemType.SCP207:
						case ItemType.GunE11SR:
						case ItemType.GunCrossvec:
						case ItemType.GunFSP9:
						case ItemType.GunLogicer:
						case ItemType.GrenadeHE:
						case ItemType.GrenadeFlash:
						case ItemType.GunCOM18:
						case ItemType.SCP018:
						case ItemType.Adrenaline:
						case ItemType.GunRevolver:
						case ItemType.GunAK:
						case ItemType.GunShotgun:
						case ItemType.SCP330:
						case ItemType.SCP1853:
							if (Random.Next(3) == 0)
							{
								if (TrapItems.Contains(itemPickupBase.NetworkInfo.Serial))
									TrapItems.Remove(itemPickupBase.NetworkInfo.Serial);
								itemPickupBase.DestroySelf();
							}
							break;
					}
				}

			if (!AlphaWarheadController.Host.detonated)
			{
				if (dWarheadRate >= 100.0)
				{
					bWarhead = true;
					if (!AlphaWarheadController.Host.inProgress)
					{
						AlphaWarheadController.Host.InstantPrepare();
						Plugin.Server.Map.StartWarhead();
						Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0,
							"<color=#FFFF00>☢</color> 系统核弹已启动 <color=#FFFF00>☢</color>", 15));
					}
					else
						Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0,
							"<color=#FFFF00>☢</color>  核弹已无法关闭!（系统核弹）<color=#FFFF00>☢</color> ", 15));
				}
				else
				{
					string strChances = (dWarheadRate).ToString("0.00");
					Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0,
						$"<color=#FFFF00>☢</color>  系统核弹当前进度: <color=#FF0000>{strChances}%</color> <color=#FFFF00>☢</color> ", 15));
				}
			}
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			MapManager.GetRooms();
			ServerRolesPatch.SetStartScreen();

			bRoundEnd = false;
			bWarhead = false;
			dWarheadRate = 0.0;

			Timing.KillCoroutines(Coroutines.ToArray());
			Coroutines.Clear();
			TrapItems.Clear();
			PersonalBC = GameObject.Find("Host").GetComponent<Broadcast>();

			Scp035id = 0;
			Scp079id = 0;
			Scp181id = 0;
			Scp682id = 0;
			Scp703id = 0;

			Scp035ItemId = 0;
			Scp079Lv = Scp079Level.Level_1;
			Scp106LastPlace = 0;

			bScp035Detected = false;
			bScp703Working = false;
		}

		private IEnumerator<float> Timing_SelfHealth(ReferenceHub hub, HealthController healthController, RoleType roleType)
		{
			int n = 0;
			Vector3 position = Vector3.zero; 
			Transform transform = hub.transform;

			while (!bRoundEnd && hub.characterClassManager.NetworkCurClass == roleType)
			{
				if (transform.position == position)
				{
					if (++n > selfHealCooldown)
					{
						if (healthController.Health + healthController.Heal > healthController.MaxHealth)
							healthController.Health = healthController.MaxHealth;
						else
							healthController.Health += healthController.Heal;

						yield return Timing.WaitForSeconds(1.0f);
						continue;
					}
				}
				else
				{
					n = 0;
					position = transform.position;
				}

				if (healthController.Health + healthController.Heal2 > healthController.MaxHealth)
					healthController.Health = healthController.MaxHealth;
				else
					healthController.Health += healthController.Heal2;

				yield return Timing.WaitForSeconds(1.0f);
			}

			yield break;
		}

		private IEnumerator<float> Timing_OnConsumableUse(ReferenceHub hub, HealthController healthController, ItemType itemType, RoleType roleType)
		{
			AhpStat.AhpProcess ahpProcess = hub.GetAhpProcess();

			switch(itemType)
			{
				case ItemType.Adrenaline:
					{
						hub.playerEffectsController.EnableEffect<Invigorated>(8.0f, true);
						Scp330Bag.AddSimpleRegeneration(hub, 2.5f, 8.0f);

						if (hub.playerId == Scp035id)
							yield break;

						ahpProcess.SustainTime = 8.0f;
						for (int i = 0; i < 40; i++)
						{
							if (ahpProcess.CurrentAmount + 1.0f > ahpProcess.Limit)
								ahpProcess.CurrentAmount = ahpProcess.Limit;
							else
								ahpProcess.CurrentAmount += 1.0f;

							yield return Timing.WaitForSeconds(0.2f);
							if (hub.characterClassManager.NetworkCurClass != roleType)
								yield break;
						}
					}
					break;

				case ItemType.Painkillers:
				{
						if (hub.playerId == Scp035id)
							yield break;

						if (ahpProcess.CurrentAmount + 5.0f > ahpProcess.Limit)
							ahpProcess.CurrentAmount = ahpProcess.Limit;
						else
							ahpProcess.CurrentAmount += 5.0f;

						Scp330Bag.AddSimpleRegeneration(hub, 1.6f, 25.0f);
					}
					break;
            }
		}

		private IEnumerator<float> Timing_OnDetonate()
        {
			bScp703Working = false;
			Plugin.Server.Map.OverchargeLights(1800.0f, false);

			foreach (ItemPickupBase itemPickupBase in Object.FindObjectsOfType<ItemPickupBase>())
				if (TrapItems.Contains(itemPickupBase.NetworkInfo.Serial))
					itemPickupBase.DestroySelf();
			TrapItems.Clear();

			yield break;
		}

		private IEnumerator<float> Timing_Scp079FlickerLights()
		{
			yield return Timing.WaitForSeconds(30.0f);

			while (!bRoundEnd && Scp079id != 0)
			{
				switch (Scp079Lv)
				{
					case Scp079Level.Level_1:
						PluginEx.FlickerLights(12, 24);
						break;

					case Scp079Level.Level_2:
						PluginEx.FlickerLights(16, 30);
						break;

					case Scp079Level.Level_3:
						PluginEx.FlickerLights(20, 36);
						break;

					case Scp079Level.Level_4:
						PluginEx.FlickerLights(24, 42);
						break;

					case Scp079Level.Level_5:
						PluginEx.FlickerLights(28, 48);
						break;
				}

				for (int i = 0; i < PowerCutCooldown; i++)
				{
					if (bRoundEnd || Scp079id == 0)
						yield break;

					yield return Timing.WaitForSeconds(1.0f);
				}
			}

			yield break;
		}

		private IEnumerator<float> Timing_OnRoundStart()
		{
			yield return Timing.WaitForOneFrame;

			// SCP-173收容室彩蛋
			PluginEx.SpawnItem(ItemType.GunRevolver, MapManager.Scp173Room.Position, Quaternion.Euler(Vector3.zero));

			// SCP-330收容室彩蛋
			PluginEx.SpawnItem(ItemType.KeycardScientist, MapManager.Scp330Room.Position, Quaternion.Euler(Vector3.zero));
			PluginEx.SpawnItem(Random.Next(2) == 0 ? ItemType.GunCOM15 : ItemType.GunCOM18, MapManager.Scp330Room.Position, Quaternion.Euler(Vector3.zero));

			// 轻收容区域刷新物品
			foreach (Smod2Room smod2Room in MapManager.Rooms)
			{
				switch (smod2Room.Roomname)
				{
					case "LCZ_372":
					case "LCZ_914":
					case "LCZ_ClassDSpawn":
					case "LCZ_Plants":
					case "LCZ_Toilets":
						ItemType itemType = Random.Next(10) == 0 ? ItemType.KeycardZoneManager : Random.Next(2) == 0 ? ItemType.KeycardScientist : ItemType.KeycardJanitor;
						PluginEx.SpawnItem(itemType, smod2Room.Position + Vector3.up, Quaternion.Euler(Vector3.zero));
						break;
				}
			}

			// 创建SCP-703
			ItemPickupBase itemPickupBase = PluginEx.SpawnItem(ItemType.Flashlight, Vector3.zero, Quaternion.Euler(Vector3.zero));
			Vector3 position = MapManager.Scp703Room.Position;

			Rigidbody rigidbody = itemPickupBase.gameObject.GetComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;

			PickupSyncInfo pickupSyncInfo = itemPickupBase.NetworkInfo;
			pickupSyncInfo.Locked = true;
			itemPickupBase.NetworkInfo = pickupSyncInfo;

			switch (MapManager.Scp703Room.Transform.rotation.eulerAngles.y)
			{
				case 0.0f:
					rigidbody.position = new Vector3(position.x + 8.0f, -3.0f, position.z);
					rigidbody.rotation = Quaternion.Euler(90.0f, 90.0f, 0.0f);
					position = new Vector3(rigidbody.position.x, 2.0f, rigidbody.position.z);
					break;

				case 90.0f:
					rigidbody.position = new Vector3(position.x, -3.0f, position.z - 8.0f);
					rigidbody.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
					position = new Vector3(rigidbody.position.x, 2.0f, rigidbody.position.z);
					break;

				case 180.0f:
					rigidbody.position = new Vector3(position.x - 8.0f, -3.0f, position.z);
					rigidbody.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
					position = new Vector3(rigidbody.position.x, 2.0f, rigidbody.position.z);
					break;

				case 270.0f:
					rigidbody.position = new Vector3(position.x, -3.0f, position.z + 8.0f);
					rigidbody.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
					position = new Vector3(rigidbody.position.x, 2.0f, rigidbody.position.z);
					break;
			}

			itemPickupBase.gameObject.transform.localScale = Vector3.one * 15.0f;
			NetworkServer.UnSpawn(itemPickupBase.gameObject);
			NetworkServer.Spawn(itemPickupBase.gameObject);

			bScp703Working = true;
			Scp703id = itemPickupBase.NetworkInfo.Serial;
			Timing.RunCoroutine(Timing_Scp703Working(position));
			
			// 选取所有的D级人员
			int index = 0;
			ReferenceHub hub;
            List<Player> Players = Plugin.Server.GetPlayers(Smod2.API.RoleType.D_CLASS);

			// 创建SCP-181
			if (Players.Count > 3)
			{
				index = Random.Next(Players.Count);
				hub = Players[index].GetHub();
				Players.RemoveAt(index);
				PluginEx.SetScp181(hub);
			}

			// 在基础上增加科学家和设施警卫
			Players.AddRange(Plugin.Server.GetPlayers(Smod2.API.RoleType.SCIENTIST));
			Players.AddRange(Plugin.Server.GetPlayers(Smod2.API.RoleType.FACILITY_GUARD));

			// 创建SCP-035
			if (PlayerManager.players.Count > 10)
			{
				index = Random.Next(Players.Count);
				hub = Players[index].GetHub();
				Players.RemoveAt(index);
				PluginEx.SetScp035(hub);
			}
			else
            {
				itemPickupBase = PluginEx.SpawnItem(ItemType.Flashlight, Vector3.zero, Quaternion.Euler(Vector3.zero));

				switch ((HCZRoom)Random.Next((int)HCZRoom.HczRoomCount))
				{
					case HCZRoom.Scp049Room:
						position = MapManager.Scp049Room.Position;
						break;

					case HCZRoom.Scp079Room:
						position = MapManager.Scp079Room.Position;
						break;

					case HCZRoom.Scp096Room:
						position = MapManager.Scp096Room.Position;
						break;

					case HCZRoom.Scp106Room:
						position = MapManager.Scp106Room.Position;
						break;

					case HCZRoom.Scp939Room:
						position = MapManager.Scp939Room.Position;
						break;

					case HCZRoom.MircoHIDRoom:
						position = MapManager.MircohidRoom.Position;
						break;

					case HCZRoom.ServersRoom:
						position = MapManager.ServersRoom.Position;
						break;
				}

				rigidbody = itemPickupBase.gameObject.GetComponent<Rigidbody>();
				rigidbody.isKinematic = true;
				rigidbody.useGravity = false;

				rigidbody.position = position;
				rigidbody.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);

				itemPickupBase.gameObject.transform.localScale = Vector3.one * 3.5f;
				NetworkServer.UnSpawn(itemPickupBase.gameObject);
				NetworkServer.Spawn(itemPickupBase.gameObject);

				Scp035ItemId = itemPickupBase.NetworkInfo.Serial;
			}

			// 创建SCP-682或本局未生成的SCP
			if (PlayerManager.players.Count > 15)
			{
				index = Random.Next(Players.Count);
				hub = Players[index].GetHub();

				if (Scp682id == 0 && PlayerManager.players.Count * 2 > Random.Next(100))
					PluginEx.SetScp682(hub);
				else
					hub.characterClassManager.SetClassIDAdv(PluginEx.GetRandomScp(), false, CharacterClassManager.SpawnReason.ForceClass);
			}
			
		}

		private IEnumerator<float> Timing_Scp079ChangeTime(ReferenceHub hub)
		{
			for (int i = Scp079ChangeTime; i > 0 && hub.characterClassManager.NetworkCurClass == RoleType.Scp079; i--)
			{
				hub.hints.Show(
					new TextHint($"<b>你还有<color=#FF0000>{i}</color>秒可以在控制台输入<color=#FF0000>.scp</color>指令随机成为其他SCP</b>",
					new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 1.0f));

				yield return Timing.WaitForSeconds(1.0f);
			}

			yield return Timing.WaitForSeconds(1.0f);

			if (hub.characterClassManager.NetworkCurClass == RoleType.Scp079)
            {
				Scp079id = hub.playerId;
				Timing.RunCoroutine(Timing_Scp079FlickerLights());
			}
		}

		private IEnumerator<float> Timing_Scp703Working(Vector3 position)
        {
			while (!bRoundEnd && bScp703Working)
			{
				ItemType itemType = (ItemType)Random.Next((int)ItemType.Coin);

				switch (itemType)
				{
					case ItemType.Ammo12gauge:
					case ItemType.Ammo556x45:
					case ItemType.Ammo44cal:
					case ItemType.Ammo762x39:
					case ItemType.Ammo9x19:
						PluginEx.SpawnItem(itemType, position, Quaternion.Euler(Vector3.zero), 30);
						break;

					default:
						PluginEx.SpawnItem(itemType, position, Quaternion.Euler(Vector3.zero));
						break;
				}

				yield return Timing.WaitForSeconds(Random.Next(25, 40));
			}

			Scp703id = 0;

			yield break;
		}

		private IEnumerator<float> Timing_OnScp914Activate(List<Player> players, Scp914KnobSetting scp914KnobSetting)
		{
			foreach (Player ply in players)
			{
				ReferenceHub hub = ply.GetHub();
				HealthController healthController = hub.GetHealthControler();

				switch (scp914KnobSetting)
				{
					case Scp914KnobSetting.Rough:
						switch (hub.inventory.NetworkCurItem.TypeId)
						{
							case ItemType.KeycardJanitor:
								// 希望卡没事[保佑][保佑][保佑]
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								break;

							case ItemType.KeycardScientist:
							case ItemType.KeycardResearchCoordinator:
							case ItemType.KeycardZoneManager:
							case ItemType.KeycardGuard:
							case ItemType.KeycardNTFOfficer:
							case ItemType.KeycardContainmentEngineer:
							case ItemType.KeycardNTFLieutenant:
							case ItemType.KeycardNTFCommander:
							case ItemType.KeycardFacilityManager:
							case ItemType.KeycardChaosInsurgency:
							case ItemType.KeycardO5:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardJanitor);
								break;
						}
						break;

					case Scp914KnobSetting.Coarse:
						switch (hub.inventory.NetworkCurItem.TypeId)
						{
							case ItemType.KeycardJanitor:
								// 希望卡没事[保佑][保佑][保佑]
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								break;

							case ItemType.KeycardScientist:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardJanitor);
								break;

							case ItemType.KeycardResearchCoordinator:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardScientist);
								break;

							case ItemType.KeycardZoneManager:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardScientist);
								break;

							case ItemType.KeycardGuard:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardZoneManager);
								break;

							case ItemType.KeycardNTFOfficer:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardGuard);
								break;

							case ItemType.KeycardContainmentEngineer:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardZoneManager);
								break;

							case ItemType.KeycardNTFLieutenant:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardNTFOfficer);
								break;

							case ItemType.KeycardNTFCommander:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardNTFLieutenant);
								break;

							case ItemType.KeycardFacilityManager:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardContainmentEngineer);
								break;

							case ItemType.KeycardChaosInsurgency:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardNTFOfficer);
								break;

							case ItemType.KeycardO5:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardFacilityManager);
								break;
						}
						break;

					case Scp914KnobSetting.OneToOne:
						switch (hub.inventory.NetworkCurItem.TypeId)
						{
							case ItemType.KeycardJanitor:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardZoneManager);
								break;

							case ItemType.KeycardScientist:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardZoneManager);
								break;

							case ItemType.KeycardResearchCoordinator:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardGuard);
								break;

							case ItemType.KeycardZoneManager:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardGuard);
								break;

							case ItemType.KeycardGuard:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardResearchCoordinator);
								break;

							case ItemType.KeycardNTFOfficer:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardContainmentEngineer);
								break;

							case ItemType.KeycardContainmentEngineer:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardFacilityManager);
								break;

							case ItemType.KeycardNTFLieutenant:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardContainmentEngineer);
								break;

							case ItemType.KeycardNTFCommander:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardChaosInsurgency);
								break;

							case ItemType.KeycardFacilityManager:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardContainmentEngineer);
								break;

							case ItemType.KeycardChaosInsurgency:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardNTFCommander);
								break;

							case ItemType.KeycardO5:
								// 希望卡没事[保佑][保佑][保佑]
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								break;
						}
						break;

					case Scp914KnobSetting.Fine:
						switch (hub.inventory.NetworkCurItem.TypeId)
						{
							case ItemType.KeycardJanitor:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardScientist);
								break;

							case ItemType.KeycardScientist:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardResearchCoordinator);
								break;

							case ItemType.KeycardResearchCoordinator:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardContainmentEngineer);
								break;

							case ItemType.KeycardZoneManager:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardFacilityManager);
								break;

							case ItemType.KeycardGuard:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardNTFOfficer);
								break;

							case ItemType.KeycardNTFOfficer:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardNTFLieutenant);
								break;

							case ItemType.KeycardContainmentEngineer:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardNTFLieutenant:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardNTFCommander);
								break;

							case ItemType.KeycardNTFCommander:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardFacilityManager:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardChaosInsurgency:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardO5:
								// 希望卡没事[保佑][保佑][保佑]
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								break;
						}
						break;

					case Scp914KnobSetting.VeryFine:
						switch (hub.inventory.NetworkCurItem.TypeId)
						{
							case ItemType.KeycardJanitor:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardScientist);
								break;

							case ItemType.KeycardScientist:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardResearchCoordinator);
								break;

							case ItemType.KeycardResearchCoordinator:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardContainmentEngineer);
								break;

							case ItemType.KeycardZoneManager:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardFacilityManager);
								break;

							case ItemType.KeycardGuard:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardNTFLieutenant);
								break;

							case ItemType.KeycardNTFOfficer:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardNTFCommander);
								break;

							case ItemType.KeycardContainmentEngineer:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardNTFLieutenant:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardNTFCommander:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardFacilityManager:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardChaosInsurgency:
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								hub.inventory.UserInventory.Items.Remove(hub.inventory.CurInstance.ItemSerial);
								hub.inventory.ServerAddItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardO5:
								// 希望卡没事[保佑][保佑][保佑]
								hub.inventory.NetworkCurItem = ItemIdentifier.None;
								break;
						}

						switch (hub.characterClassManager.NetworkCurClass)
						{
							case RoleType.ClassD:
							case RoleType.Scientist:
							case RoleType.FacilityGuard:
							case RoleType.NtfPrivate:
							case RoleType.NtfSergeant:
							case RoleType.NtfSpecialist:
							case RoleType.NtfCaptain:
							case RoleType.ChaosConscript:
							case RoleType.ChaosRifleman:
							case RoleType.ChaosMarauder:
							case RoleType.ChaosRepressor:
								if (hub.playerId != Scp035id)
									healthController.Health = healthController.MaxHealth = healthController.MaxHealth2;
								break;

							case RoleType.Scp049:
								if (!healthController.Evolved)
								{
									healthController.Evolved = true;
									healthController.MaxHealth = healthController.MaxHealth + 300.0f;
									hub.GetHealthStat().ServerHeal(300.0f);

									AhpStat.AhpProcess ahpProcess = hub.GetAhpProcess();
									ahpProcess.Limit = 200.0f;
									ahpProcess.DecayRate = -1.2f;

									PluginEx.SetServerBadge(hub.serverRoles, "SCP-049");
									hub.hints.Show(
										new TextHint("<b>强化成功!</b>",
										new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
								}
								else
									hub.hints.Show(
										new TextHint("<b>强化失败!</b>",
										new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));
								break;

							case RoleType.Scp0492:
								hub.hints.Show(
									new TextHint("<b>强化失败!</b>",
									new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));
								break;

							case RoleType.Scp096:
								if (!healthController.Evolved)
								{
									healthController.Evolved = true;
									healthController.MaxHealth = healthController.MaxHealth + 250.0f;
									hub.GetHealthStat().ServerHeal(250.0f);

									(hub.gameObject.GetComponent<PlayableScpsController>().CurrentScp as Scp096).CurMaxShield += 250.0f;

									PluginEx.SetServerBadge(hub.serverRoles, "SCP-096");
									hub.hints.Show(
										new TextHint("<b>强化成功!</b>\n<b>AHP: 1250(+250)\n<b>HP: 2250(+250)</b></b>",
										new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
								}
								else
									hub.hints.Show(
										new TextHint("<b>强化失败!</b>",
										new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));
								break;

							case RoleType.Scp106:
								if (!healthController.Evolved)
								{
									healthController.Evolved = true;
									healthController.MaxHealth = healthController.MaxHealth + 300.0f;
									hub.GetHealthStat().ServerHeal(300.0f);

									PluginEx.SetServerBadge(hub.serverRoles, "SCP-106");
									hub.hints.Show(
										new TextHint("<b>强化成功!</b>",
										new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
								}
								else
									hub.hints.Show(
										new TextHint("<b>强化失败!</b>",
										new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));
								break;

							case RoleType.Scp173:
								if (!healthController.Evolved)
								{
									healthController.Evolved = true;
									hub.playerEffectsController.GetEffect<Scp207>().Intensity = 4;
									
									PluginEx.SetServerBadge(hub.serverRoles, "SCP-173");
									hub.hints.Show(
										new TextHint("<b>强化成功!</b>\n<b><color=#FF0000>SCP-207</color>效果X4</b>",
										new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
								}
								else
									hub.hints.Show(
										new TextHint("<b>强化失败!</b>",
										new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));
								break;

							case RoleType.Scp93953:
							case RoleType.Scp93989:
								if (!healthController.Evolved && hub.playerId != Scp682id)
								{
									healthController.Evolved = true;
									healthController.MaxHealth = healthController.MaxHealth + 200.0f;
									hub.GetHealthStat().ServerHeal(200.0f);

									AhpStat.AhpProcess ahpProcess = hub.GetAhpProcess();
									ahpProcess.Limit += 250.0f;

									PluginEx.SetServerBadge(hub.serverRoles, hub.characterClassManager.NetworkCurClass == RoleType.Scp93953 ? "SCP-939-53" : "SCP-939-89");
									hub.hints.Show(
										new TextHint("<b>强化成功!</b>",
										new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
								}
								else
									hub.hints.Show(
										new TextHint("<b>强化失败!</b>",
										new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));
								break;
						}
						break;
				}
			}

			yield break;
		}

		private IEnumerator<float> Timing_OnSetRole(ReferenceHub hub)
		{
			yield return Timing.WaitForOneFrame;

			HealthController healthControler;
			if (!hub.gameObject.TryGetComponent(out healthControler))
				healthControler = hub.gameObject.AddComponent<HealthController>();
			healthControler.Start();

			switch (hub.characterClassManager.NetworkCurClass)
			{
				case RoleType.ClassD:
					if (Random.Next(5) == 0)
						hub.inventory.ServerAddItem(ItemType.KeycardScientist);
					else if (Random.Next(4) == 0)
						hub.inventory.ServerAddItem(ItemType.KeycardJanitor);

					hub.inventory.ServerAddItem(ItemType.Flashlight);

					if (Random.Next(4) == 0)
						hub.inventory.ServerAddItem(ItemType.Medkit);
					else if (Random.Next(3) == 0)
						hub.inventory.ServerAddItem(ItemType.Painkillers);

					if (Random.Next(20) == 0)
						hub.inventory.ServerAddItem(ItemType.SCP268);
					else if (Random.Next(10) == 0)
						hub.inventory.ServerAddItem(ItemType.SCP207);
					else if (Random.Next(10) == 0)
						hub.inventory.ServerAddItem(ItemType.SCP2176);

					hub.inventory.UserInventory.ReserveAmmo[Random.Next(2) == 0 ? ItemType.Ammo556x45 : Random.Next(2) == 0 ? ItemType.Ammo762x39 : ItemType.Ammo9x19] = 20;
					hub.inventory.SendAmmoNextFrame = true;
					break;

				case RoleType.Scientist:
					hub.inventory.ServerAddItem(ItemType.KeycardScientist);
					hub.inventory.ServerAddItem(ItemType.Flashlight);
					hub.inventory.ServerAddItem(ItemType.Medkit);

					if (Random.Next(5) == 0)
						hub.inventory.ServerAddItem(ItemType.SCP500);
					else if (Random.Next(4) == 0)
						hub.inventory.ServerAddItem(ItemType.Adrenaline);

					if (Random.Next(10) == 0)
						hub.inventory.ServerAddItem(ItemType.SCP268);
					else if (Random.Next(5) == 0)
						hub.inventory.ServerAddItem(ItemType.SCP1853);
					else if (Random.Next(100) == 0)
						(hub.inventory.ServerAddItem(ItemType.MicroHID).PickupDropModel as FirearmPickup).NetworkStatus = new FirearmStatus(0, FirearmStatusFlags.None, 0);

					hub.inventory.UserInventory.ReserveAmmo[Random.Next(2) == 0 ? ItemType.Ammo556x45 : Random.Next(2) == 0 ? ItemType.Ammo762x39 : ItemType.Ammo9x19] = 20;
					hub.inventory.SendAmmoNextFrame = true;
					break;

				case RoleType.ChaosMarauder:
					(hub.inventory.ServerAddItem(ItemType.ParticleDisruptor) as Firearm).Status = new FirearmStatus(1, FirearmStatusFlags.Cocked | FirearmStatusFlags.MagazineInserted, 1);
					break;
					
				case RoleType.Scp049:
					healthControler.Heal = Scp049Heal;
					healthControler.Heal2 = Scp049Heal2;
					Timing.RunCoroutine(Timing_SelfHealth(hub, healthControler, hub.characterClassManager.NetworkCurClass));
					break;

				case RoleType.Scp079:
					Timing.RunCoroutine(Timing_Scp079ChangeTime(hub));
					break;

				case RoleType.Scp096:
					healthControler.Heal = Scp096Heal;
					Timing.RunCoroutine(Timing_SelfHealth(hub, healthControler, hub.characterClassManager.NetworkCurClass));
					break;

				case RoleType.Scp106:
					hub.inventory.ServerAddItem(ItemType.KeycardJanitor);
					hub.hints.Show(
						new TextHint($"<voffset=27em><size=120><b> </b></size>\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" +
						$"<size=35><b>按下Ctrl键可以在地上创建一个<color=#FF0000>诱捕陷阱</color></b></size>\n<size=35><b>触发陷阱的人会被传送至口袋空间</b></size></voffset>",
						new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 15.0f));
					break;

				case RoleType.Scp173:
					healthControler.Heal = Scp173Heal;
					Timing.RunCoroutine(Timing_SelfHealth(hub, healthControler, hub.characterClassManager.NetworkCurClass));
					break;

				case RoleType.Scp93953:
				case RoleType.Scp93989:
					if (hub.playerId == Scp682id)
					{
						healthControler.Heal = Scp682Heal;
						healthControler.Heal2 = Scp682Heal;
						Timing.RunCoroutine(Timing_SelfHealth(hub, healthControler, hub.characterClassManager.NetworkCurClass));
						Timing.CallDelayed(0.25f, () => {
							AhpStat.AhpProcess ahpProcess = hub.GetAhpProcess();
							ahpProcess.Limit = 0;
							ahpProcess.CurrentAmount = 0;
							hub.playerEffectsController.GetEffect<CustomPlayerEffects.Visuals939>().Intensity = 0;
						});
					}
					break;
			}

			yield break;
		}

		private IEnumerator<float> Timing_EnteringPocketDimension(ReferenceHub player, ReferenceHub attacker, bool IsTrapped)
		{
			player.inventory.NetworkCurItem = ItemIdentifier.None;
			player.scp106PlayerScript.goingViaThePortal = true;
			player.scp106PlayerScript.TeleportAnimation();

			if (IsTrapped)
            {
				player.hints.Show(
					new TextHint("<size=30><b>你触发了<color=#FF0000>SCP-106</color>的诱捕陷阱</b></size>",
					new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));

				foreach (GameObject gameObject in PlayerManager.players)
				{
					ReferenceHub hub = ReferenceHub.GetHub(gameObject);
					if (hub.characterClassManager.NetworkCurClass == RoleType.Scp106)
						Timing.RunCoroutine(Timing_SendMessage(MessageType.Person, hub.playerId,
							$"<color=#FFFF00>{player.nicknameSync.MyNick}</color>触发了你的<color=#FF0000>诱捕陷阱</color>", 5));
				}
			}
			else if (attacker != null && attacker.GetHealthControler().Evolved)
            {
				int itemCount = player.inventory.UserInventory.Items.Count;

				if (itemCount > 0)
					player.inventory.ServerRemoveItem(player.inventory.UserInventory.Items.ElementAt(Random.Next(itemCount)).Key, null);

				player.hints.Show(
					new TextHint("<size=30><b>你在口袋空间中丢失了一件物品...</b></size>",
					new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
			}

			foreach (Scp079PlayerScript scp079PlayerScript in Scp079PlayerScript.instances)
				scp079PlayerScript.ServerProcessKillAssist(player, ExpGainType.PocketAssist);

			yield return Timing.WaitForSeconds(3.0f);

			player.playerEffectsController.EnableEffect<Corroding>(0.0f, false);
			player.scp106PlayerScript.goingViaThePortal = false;
		}

		private IEnumerator<float> Timing_OnPocketDimensionDie()
		{
			foreach (GameObject gameObject in PlayerManager.players)
			{
				ReferenceHub hub = ReferenceHub.GetHub(gameObject);

				if (hub.characterClassManager.NetworkCurClass == RoleType.Scp106)
				{
					HealthController healthController = hub.GetHealthControler();

					if (healthController.Health + Scp106Kill > healthController.MaxHealth)
						healthController.Health = healthController.MaxHealth;
					else
						healthController.Health += Scp106Kill;
				}
			}

			yield break;
		}

		private IEnumerator<float> Timing_PersonMessage(PersonMessage personMessage, NetworkConnection networkConnection)
		{
			string strText;
			personMessage.TextDisplay.Add(
				new Message("欢迎来到萤火服务器(°∀°)ﾉ", 15));

			for (; ; )
			{
				if (personMessage.TextDisplay.Count > 0)
				{
					strText = null;
					switch (personMessage.TextDisplay.Count)
					{
						case 1:
							strText = $"<size=30>[{personMessage.TextDisplay[0].Duration}]" + personMessage.TextDisplay[0].Text + "</size>\n";

							if (--personMessage.TextDisplay[0].Duration == 0)
								personMessage.TextDisplay.RemoveAt(0);
							break;

						case 2:
							strText = $"<size=30>[{personMessage.TextDisplay[0].Duration}]" + personMessage.TextDisplay[0].Text + "</size>\n";
							strText += $"<size=30>[{personMessage.TextDisplay[1].Duration}]" + personMessage.TextDisplay[1].Text + "</size>\n";

							if (--personMessage.TextDisplay[0].Duration == 0)
							{
								personMessage.TextDisplay.RemoveAt(0);

								if (--personMessage.TextDisplay[0].Duration == 0)
									personMessage.TextDisplay.RemoveAt(0);

								break;
							}

							if (--personMessage.TextDisplay[1].Duration == 0)
								personMessage.TextDisplay.RemoveAt(1);
							break;

						default:
							strText = $"<size=30>[{personMessage.TextDisplay[0].Duration}]" + personMessage.TextDisplay[0].Text + "</size>\n";
							strText += $"<size=30>[{personMessage.TextDisplay[1].Duration}]" + personMessage.TextDisplay[1].Text + "</size>\n";
							strText += $"<size=30>[{personMessage.TextDisplay[2].Duration}]" + personMessage.TextDisplay[2].Text + "</size>\n";

							if (--personMessage.TextDisplay[0].Duration == 0)
							{
								personMessage.TextDisplay.RemoveAt(0);

								if (--personMessage.TextDisplay[0].Duration == 0)
								{
									personMessage.TextDisplay.RemoveAt(0);

									if (--personMessage.TextDisplay[0].Duration == 0)
										personMessage.TextDisplay.RemoveAt(0);

									break;
								}

								if (--personMessage.TextDisplay[1].Duration == 0)
									personMessage.TextDisplay.RemoveAt(1);
								break;
							}

							if (--personMessage.TextDisplay[1].Duration == 0)
							{
								personMessage.TextDisplay.RemoveAt(1);

								if (--personMessage.TextDisplay[1].Duration == 0)
									personMessage.TextDisplay.RemoveAt(1);

								break;
							}

							if (--personMessage.TextDisplay[2].Duration == 0)
								personMessage.TextDisplay.RemoveAt(2);
							break;
					}

					PersonalBC.TargetAddElement(networkConnection, strText, 1, Broadcast.BroadcastFlags.Monospaced);
					yield return Timing.WaitForSeconds(0.975f);
				}
				else
					yield return Timing.WaitForSeconds(1.0f);
			}
		}

		private IEnumerator<float> Timing_SendMessage(MessageType messageType, int PlayerId, string text, int duration = 5)
		{
			switch (messageType)
			{
				case MessageType.All:
					foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
						personMessage.TextDisplay.Add(new Message(text, duration));
					break;

				case MessageType.Person:
					foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
						if (personMessage.PlayerId == PlayerId)
							personMessage.TextDisplay.Add(new Message(text, duration));
					break;

				case MessageType.TeamMtf:
					foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
					{
						switch (personMessage.Hub.characterClassManager.NetworkCurClass)
						{
							case RoleType.Scientist:
							case RoleType.FacilityGuard:
							case RoleType.NtfPrivate:
							case RoleType.NtfSergeant:
							case RoleType.NtfSpecialist:
							case RoleType.NtfCaptain:
								if (personMessage.PlayerId != Scp035id)
									personMessage.TextDisplay.Add(new Message(text, duration));
								break;
						}
					}
					break;

				case MessageType.TeamChaos:
					foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
					{
						switch (personMessage.Hub.characterClassManager.NetworkCurClass)
						{
							case RoleType.ClassD:
							case RoleType.ChaosConscript:
							case RoleType.ChaosRifleman:
							case RoleType.ChaosMarauder:
							case RoleType.ChaosRepressor:
								if (personMessage.PlayerId != Scp035id)
									personMessage.TextDisplay.Add(new Message(text, duration));
								break;
						}
					}
					break;

				case MessageType.TeamScp:
					foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
					{
						switch (personMessage.Hub.characterClassManager.NetworkCurClass)
						{
							case RoleType.Scp049:
							case RoleType.Scp0492:
							case RoleType.Scp079:
							case RoleType.Scp096:
							case RoleType.Scp106:
							case RoleType.Scp173:
							case RoleType.Scp93953:
							case RoleType.Scp93989:
								personMessage.TextDisplay.Add(new Message(text, duration));
								break;

							default:
								if (personMessage.PlayerId == Scp035id)
									personMessage.TextDisplay.Add(new Message(text, duration));
								break;
						}
					}
					break;

				case MessageType.TeamSpectator:
					foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
						if (personMessage.Hub.characterClassManager.NetworkCurClass == RoleType.Spectator)
							personMessage.TextDisplay.Add(new Message(text, duration));
					break;

				case MessageType.AdminChat:
					foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
						personMessage.TextDisplay.Add(new Message(text, duration));
					break;
			}

			yield break;
		}
    }
}