using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CustomPlayerEffects;
using Hints;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Pickups;
using MEC;
using Mirror;
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

namespace CommonPlugin
{
    public class EventHandlers : IEventHandler079LevelUp, IEventHandlerCheckEscape, IEventHandlerConsumableUse, IEventHandlerCheckRoundEnd,
		IEventHandlerContain106, IEventHandlerLCZDecontaminate, IEventHandlerPlayerSCP207Use, IEventHandlerPlayerDie,
		IEventHandlerPlayerHurt, IEventHandlerPlayerJoin, IEventHandlerPlayerLeave, IEventHandlerPlayerPickupItem, IEventHandlerPlayerTriggerTesla, IEventHandlerPocketDimensionDie,
		IEventHandlerPocketDimensionEnter, IEventHandlerPocketDimensionExit, IEventHandlerRoundEnd, IEventHandlerRoundStart, IEventHandlerScp096AddTarget, IEventHandlerSCP914Activate,
		IEventHandlerSetRole, IEventHandlerTeamRespawn, IEventHandlerWaitingForPlayers, IEventHandlerWarheadChangeLever, IEventHandlerWarheadStopCountdown, IEventHandlerWarheadDetonate
	{
		public CommonPlugin Plugin { get; set; }

		public EventHandlers() { }

		static EventHandlers()
		{
			Scp035id = 0;

			Scp079id = 0;

			Scp181id = 0;

			Scp682id = 0;

			Scp703id = 0;

			bScp035Detected = false;

            TrapItems = new List<ushort>();

		}

		public EventHandlers(CommonPlugin Plugin)
		{
			this.Plugin = Plugin;

			this.bRoundEnd = false;

			this.bWarhead = false;

			this.dWarheadRate = 0.0;

			this.Random = new System.Random();

			

			this.Coroutines = new List<CoroutineHandle>();
		}


		// 角色生命值
		public const float ClassdMaxHP = 100.0f;

		public const float ScientistMaxHP = 100.0f;

		public const float MtfMaxHP = 125.0f;

		public const float MtfCaptainMaxHP = 150.0f;

		public const float ChaosMaxHP = 125.0f;

		public const float ChaosRepressorMaxHP = 150.0f;

		public const float Scp0492MaxHP = 500.0f;

		public const float Scp049MaxHP = 2500.0f;

		public const float Scp079MaxHP = 100000.0f;

		public const float Scp096MaxHP = 1500.0f;
		// 
		public static int Scp035id;

		//public static int Scp049id;

		public static int Scp079id;

		//public static int Scp096id;

		//public static int Scp106id;

		//public static int Scp173id;

		public static int Scp181id;

		public static int Scp682id;

		public static int Scp703id;

		//public static int Scp939id;

		public const int lateJoinTime = 90;

		public static bool bScp035Detected;

		public const int Scp079SwitchTime = 30;

		private const float MedkitHeal = 70.0f;

		// private field
		private bool bRoundEnd;

		private bool bWarhead;

		private double dWarheadRate;

		private const int selfHealCooldown = 3;

		private const int maxPlayer = 33;

		private readonly System.Random Random;

		public static readonly List<ushort> TrapItems;

		private readonly List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();

		//private readonly MethodInfo SendSpawnMessage = typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static);


		// SCP-035
		private const float Scp035Hit = 4.0f;

		private const float Scp035Heal = 35.0f;

		// SCP-049 & SCP-049-2
		

		private const float Scp049Heal = 2.0f;

		private const float Scp049Heal2 = 5.0f;

		// SCP-079
		private const int PowerCutCooldown = 30;

		private Scp079Level Scp079Lv = Scp079Level.Level_1;

		// SCP-096
		

		public static int Scp096AddShield = 70;

		public static float Scp096Shield = 350.0f;

		public static float Scp096MaxShield = 450.0f;

		private readonly float Scp096Heal = 2.0f;

		// SCP-106
		public const int Scp106MaxHP = 1000;

		private const float Scp106Heal = 20.0f;

		public const int Scp106Cooldown = 30;

		public static int Scp106LastPlace = 0;

		// SCP-173
		
		public const int Scp173MaxHP = 3200;

		private readonly float Heal173 = 7.5f;

		private readonly float Scp173Hit = 0.67f;

		// SCP-682
		public const int Scp682MaxHP = 4000;

		private const float heal682 = 8.0f;

		private const float lifeSteal682 = 40.0f;

		// SCP-703
		public static bool bScp703Activating = false;

		// SCP-939
		public const int Scp939MaxHP = 3000;

		private readonly float healScp939_1 = 1.0f;

		private readonly float healScp939_2 = 6.0f;

		private readonly float lifeStealScp939 = 25.0f;

		private const float Scp939Hit = 25.0f;


		// Private Custom Method
		private string ScpDeathInfo(string scp, string killer)
		{
			return $"[<color=#FF0000>{scp}</color>]收容成功 收容者: {killer}";
		}

		// Smod2插件接口
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
			// 测试
			if (ev.Player.PlayerID == Scp035id)
				ev.ActivateContainment = false;
		}

		public void OnDecontaminate() => bScp703Activating = false;

		public void OnDetonate()
		{
			TrapItems.Clear();
			Plugin.Server.Map.OverchargeLights(1800.0f, false);
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
					healthStat.ServerHeal(hub.playerId == Scp035id ? Scp035Heal : MedkitHeal);
					hub.playerEffectsController.UseMedicalItem(itemType);
					break;

				case ItemType.SCP500:
					if (hub.playerId != Scp035id)
						healthControler.MaxHealth = healthControler.MaxHealth2;

					healthStat.ServerHeal(healthControler.MaxHealth);
					hub.playerEffectsController.UseMedicalItem(itemType);
					break;

				case ItemType.SCP207:
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

		public void OnPlayerSCP207Use(PlayerSCP207UseEvent ev)
		{
			if (ev.Player.GetPlayerEffect(StatusEffect.SCP207).Intensity > 0)
				ev.Allow = false;
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (GameCore.RoundStart.singleton.NetworkTimer != -1 || ev.DamageTypeVar == DamageType.NONE)
				return;

			dWarheadRate += Random.NextDouble() + 0.345;

			ReferenceHub killer = ev.Killer.GetHub();
			ReferenceHub player = ev.Player.GetHub();
			
			string announceMessage;
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
						// TODO
						Scp035id = 0;
						PluginEx.ClearServerBadge(player.serverRoles);

						announceMessage = ScpDeathInfo("SCP-035", ev.Killer.Name);
						Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0, announceMessage, 10));
						RespawnEffectsController.PlayCassieAnnouncement("SCP 0 3 5 CONTAINS SUCCESSFULLY", false, true);
						return;
					}
					else if (player.playerId == Scp181id)
					{
						// TODO
						Scp181id = 0;
						PluginEx.ClearServerBadge(player.serverRoles);

						announceMessage = ScpDeathInfo("SCP-181", ev.Killer.Name);
						Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0, announceMessage, 10));
						RespawnEffectsController.PlayCassieAnnouncement("SCP 1 8 1 CONTAINS SUCCESSFULLY", false, true);
						return;
					}
					break;
			}

			if (ev.Killer.PlayerID != ev.Player.PlayerID)
			{
				if (ev.Killer.PlayerID == Scp035id)
				{
					RoundSummary.KilledBySCPs++;
					if (!bScp035Detected)
					{
						bScp035Detected = true;
						PluginEx.SetServerBadge(killer.serverRoles, "SCP-035");

						// TODO
						Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0, "警告: <color=#FF0000>SCP-035</color>已出现", 10));
						RespawnEffectsController.PlayCassieAnnouncement("WARNING SCP 0 3 5 CONTAINMENT BREACH", false, true);
					}
					return;
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

				if (player.characterClassManager.NetworkCurClass == RoleType.Scp173)
				{
					ev.Damage *= Scp173Hit;
					return;
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
							// TODO
							healthController.MaxHealth -= 1.0f;
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

							if (itemCount > 0)
							{
								ev.Damage = 0.0f;

								ushort itemSerial = player.inventory.UserInventory.Items.ElementAt(Random.Next(itemCount)).Key;
								player.inventory.ServerRemoveItem(itemSerial, null);

								player.hints.Show(
									new TextHint("<b>你使用一件物品抵挡了一次来自<color=#FF0000>SCP</color>的伤害</b>",
									new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));
								attacker.hints.Show(
									new TextHint("<b><color=#FF0000>SCP-181</color>使用一件物品抵挡了你的一次伤害</b>",
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
						}
						break;
				}
			}
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (MessageQueue.Messages.ContainsKey(ev.Player.PlayerID))
				return;

			PersonMessage personMessage = new PersonMessage(ev.Player.PlayerID, ev.Player.GetHub());
			MessageQueue.Messages.Add(ev.Player.PlayerID, personMessage);
			Coroutines.Add(Timing.RunCoroutine(Timing_PersonMessage(personMessage, ev.Player), "Mq" + ev.Player.PlayerID));
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
				case ItemType.Adrenaline:
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
								new TextHint("<b>该物品为<color=#FF0000>SCP-106</color>的诱捕陷阱</b>",
								new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));

							return;
						}

						ev.ChangeTo = Smod2.API.ItemType.NONE;
						ev.Item.Remove();
						TrapItems.Remove(ev.Item.SerialNumber);

						Timing.RunCoroutine(Timing_OnTriggerTrapItem(hub));
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
			if (!Plugin.Server.Map.WarheadDetonated && ev.Player.PlayerID == Scp181id)
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
			if (ev.Player.PlayerID == Scp181id && Plugin.Server.Map.WarheadDetonated)
				ev.Player.Kill("SCP-106");
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

			Timing.RunCoroutine(Timing_OnScp914Activate(ev.PlayerInputs));
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			ReferenceHub hub = ev.Player.GetHub();

			if (hub.characterClassManager.NetworkCurClass == RoleType.Spectator)
			{
				if (hub.playerId == Scp035id)
				{
					Scp035id = 0;
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
							if (Scp703id != itemPickupBase.gameObject.GetInstanceID())
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
			Timing.KillCoroutines(Coroutines.ToArray());
			Coroutines.Clear();
			TrapItems.Clear();
			MapManager.GetRooms();
			Patches.ServerRolesPatch.SetStartScreen();

			Scp035id = 0;
			Scp079id = 0;
			Scp181id = 0;
			Scp682id = 0;

			bScp035Detected = false;
			Scp079Lv = Scp079Level.Level_1;
			Scp106LastPlace = 0;
			Scp703id = 0;

			bWarhead = false;
			bRoundEnd = false;
			bScp703Activating = false;
			dWarheadRate = 0.0;
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
						float hp = hub.playerId == Scp035id ? 0.2f : 0.25f;
						float ahp = hub.playerId == Scp035id ? 0 : 1.0f;

						hub.playerEffectsController.EnableEffect<Invigorated>(8.0f, true);
						ahpProcess.SustainTime = 8.0f;

						for (int i = 0; i < 40; i++)
						{
							if (healthController.Health + hp > healthController.MaxHealth)
								healthController.Health = healthController.MaxHealth;
							else
								healthController.Health += hp;

							if (ahpProcess.CurrentAmount + ahp > ahpProcess.Limit)
								ahpProcess.CurrentAmount = ahpProcess.Limit;
							else
								ahpProcess.CurrentAmount += ahp;

							yield return Timing.WaitForSeconds(0.2f);
							if (hub.characterClassManager.NetworkCurClass != roleType)
								yield break;
						}
					}
					break;

				case ItemType.Painkillers:
				{
						float hp = hub.playerId == Scp035id ? 0.1f : 0.5f;
						float ahp = hub.playerId == Scp035id ? 0 : 5.0f;

						if (ahpProcess.CurrentAmount + ahp > ahpProcess.Limit)
							ahpProcess.CurrentAmount = ahpProcess.Limit;
						else
							ahpProcess.CurrentAmount += ahp;

						for (int i = 0; i < 60; i++)
						{
							if (healthController.Health + hp > healthController.MaxHealth)
								healthController.Health = healthController.MaxHealth;
							else
								healthController.Health += hp;

							yield return Timing.WaitForSeconds(0.2f);
							if (hub.characterClassManager.NetworkCurClass != roleType)
								yield break;
						}
					}
					break;
            }
		}

		private IEnumerator<float> Timing_OnFlickerLights()
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
            Inventory itemSpawner = GameObject.Find("Host").GetComponent<Inventory>();
			GameObject ragdoll = PlayerManager.localPlayer;
			ReferenceHub hub = ReferenceHub.GetHub(ragdoll);
			Role role = hub.characterClassManager.Classes.SafeGet((int)RoleType.ClassD);
			PlayerStats.HitInfo ragdollInfo = new PlayerStats.HitInfo(10000.0f, "SCP-173", DamageTypes.Scp173, hub.playerId);
			Quaternion ratation = Quaternion.Euler(new Vector3(ragdoll.transform.rotation.x, ragdoll.transform.rotation.y, ragdoll.transform.rotation.z));

			// SCP-173收容室彩蛋
			ragdoll = Object.Instantiate(role.model_ragdoll, MapManager.Scp173Room.Position + role.ragdoll_offset.position, Quaternion.Euler(ratation.eulerAngles + role.ragdoll_offset.rotation));
			NetworkServer.Spawn(ragdoll);
			Ragdoll component = ragdoll.GetComponent<Ragdoll>();
			component.Networkowner = new Ragdoll.Info("萌新天堂服主", "萌新天堂服主", ragdollInfo, role, hub.playerId);
			component.NetworkallowRecall = false;
			component.NetworkPlayerVelo = Vector3.zero;

			// SCP-173收容室刷新物品
			itemSpawner.SetPickup(ItemType.GunUSP, 0.0f, MapManager.Scp173Room.Position, Quaternion.Euler(Vector3.zero), 0, 0, 0);

			// SCP-012收容室彩蛋
			role = hub.characterClassManager.Classes.SafeGet((int)RoleType.Scientist);
			ragdoll = Object.Instantiate(role.model_ragdoll, MapManager.Scp012Room.Position + role.ragdoll_offset.position, Quaternion.Euler(ratation.eulerAngles + role.ragdoll_offset.rotation));
			NetworkServer.Spawn(ragdoll);
			ragdollInfo = new PlayerStats.HitInfo(10000.0f, "SCP-012", DamageTypes.Bleeding, hub.playerId);
			component = ragdoll.GetComponent<Ragdoll>();
			component.Networkowner = new Ragdoll.Info("亮亮博士", "亮亮博士", ragdollInfo, role, hub.playerId);
			component.NetworkallowRecall = false;
			component.NetworkPlayerVelo = Vector3.zero;

			// SCP-012收容室刷新物品
			Vector3 postion = MapManager.Scp012Room.Position;
			itemSpawner.SetPickup(ItemType.KeycardScientist, 0.0f, postion, Quaternion.Euler(Vector3.zero), 0, 0, 0);
			itemSpawner.SetPickup(ItemType.GunUSP, 0.0f, postion, Quaternion.Euler(Vector3.zero), 0, 0, 0);
			itemSpawner.SetPickup(ItemType.Ammo9mm, 15.0f, postion, Quaternion.Euler(Vector3.zero), 0, 0, 0);
			itemSpawner.SetPickup(ItemType.Flashlight, 0.0f, postion, Quaternion.Euler(Vector3.zero), 0, 0, 0);
			itemSpawner.SetPickup(ItemType.GrenadeFrag, 0.0f, postion, Quaternion.Euler(Vector3.zero), 0, 0, 0);
			itemSpawner.SetPickup(ItemType.SCP500, 0.0f, postion, Quaternion.Euler(Vector3.zero), 0, 0, 0);

			// 轻收容区域刷新物品
			foreach (Smod2Room smod2Room in MapManager.Rooms)
			{
				switch (smod2Room.Roomname)
				{
					case "LCZ_372":
					case "LCZ_914":
					case "LCZ_Cafe":
					case "LCZ_ClassDSpawn":
					case "LCZ_Plants":
					case "LCZ_Toilets":
						ItemType itemType = Random.Next(10) == 0 ? ItemType.KeycardZoneManager : Random.Next(2) == 0 ? ItemType.KeycardScientist : ItemType.KeycardJanitor;
						itemSpawner.SetPickup(itemType, 0.0f, new Vector3(smod2Room.Position.x, smod2Room.Position.y + 0.25f, smod2Room.Position.z), Quaternion.Euler(Vector3.zero), 0, 0, 0);
						break;
				}
			}

			// 创建SCP-703
			bScp703Activating = true;
			Vector3 vector3 = new Vector3(0.0f, 0.0f, 0.0f);
			Pickup pickup = itemSpawner.SetPickup(ItemType.Flashlight, 0.0f, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.Euler(Vector3.zero), 0, 0, 0);
			Rigidbody rigidbody = pickup.GetComponent<Rigidbody>();
			pickup.transform.localScale = Vector3.one * 8.5f;
			NetworkServer.UnSpawn(pickup.gameObject);
			NetworkServer.Spawn(pickup.gameObject);
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
			pickup.Locked = true;
			Scp703id = pickup.gameObject.GetInstanceID();

			postion = MapManager.Scp703Room.Position;
			switch (MapManager.Scp703Room.Transform.rotation.eulerAngles.y)
			{
				case 0.0f:
					pickup.transform.position = new Vector3(postion.x + 13.4f, postion.y - 1.0f, postion.z);
					pickup.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 270.0f);
					vector3 = new Vector3(pickup.transform.position.x - 0.8f, 2.0f, pickup.transform.position.z);
					break;

				case 90.0f:
					pickup.transform.position = new Vector3(postion.x, postion.y - 1.0f, postion.z - 13.4f);
					pickup.transform.rotation = Quaternion.Euler(180.0f, 90.0f, 90.0f);
					vector3 = new Vector3(pickup.transform.position.x, 2.0f, pickup.transform.position.z + 0.8f);
					break;

				case 180.0f:
					pickup.transform.position = new Vector3(postion.x - 13.4f, postion.y - 1.0f, postion.z);
					pickup.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
					vector3 = new Vector3(pickup.transform.position.x + 0.8f, 2.0f, pickup.transform.position.z);
					break;

				case 270.0f:
					pickup.transform.position = new Vector3(postion.x, postion.y - 1.0f, postion.z + 13.4f);
					pickup.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 270.0f);
					vector3 = new Vector3(pickup.transform.position.x, 2.0f, pickup.transform.position.z - 0.8f);
					break;
			}
			Timing.RunCoroutine(Timing_OnScp703Activate(vector3));

			// 选取所有的D级人员
			int index = 0;
            List<Player> Players = Plugin.Server.GetPlayers(Smod2.API.RoleType.D_CLASS);

			// 创建SCP-181
			if (Players.Count > 3)
			{
				index = Random.Next(Players.Count);
				hub = Players[index].GetHub();
				SetScp181(hub);
				Players.RemoveAt(index);
			}

			// 在基础上增加科学家和设施警卫
			Players.AddRange(Plugin.Server.GetPlayers(Smod2.API.RoleType.SCIENTIST));
			Players.AddRange(Plugin.Server.GetPlayers(Smod2.API.RoleType.FACILITY_GUARD));

			// 创建SCP-035
			if (PlayerManager.players.Count > 10)
			{
				index = Random.Next(Players.Count);
				hub = Players[index].GetHub();
				SetScp035(hub);
				Players.RemoveAt(index);
			}

			// 创建SCP-682或本局未生成的SCP
			if (PlayerManager.players.Count > 15)
			{
				index = Random.Next(Players.Count);
				hub = Players[index].GetHub();
				Timing.RunCoroutine(Timing_SetRandomScp(hub));
			}

			yield break;
		}

		private IEnumerator<float> Timing_OnScp079SwitchTime(ReferenceHub hub)
		{
			for (int i = Scp079SwitchTime; i > 0 && hub.characterClassManager.NetworkCurClass == RoleType.Scp079; i--)
			{
				hub.hints.Show(new TextHint($"<b>你还有<color=#FF0000>{i}</color>秒可以在控制台输入<color=#FF0000>.scp</color>指令随机成为其他SCP</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 1.0f));
				yield return Timing.WaitForSeconds(1.0f);
			}

			yield return Timing.WaitForSeconds(1.0f);
			if (hub.characterClassManager.NetworkCurClass == RoleType.Scp079)
            {
				Scp079id = hub.playerId;
				Timing.RunCoroutine(Timing_SelfHealth(hub));
				Timing.RunCoroutine(Timing_OnFlickerLights());
			}

			yield break;
		}

		private IEnumerator<float> Timing_OnScp703Activate(Vector3 vector3)
        {
            Inventory itemSpawner = GameObject.Find("Host").GetComponent<Inventory>();

			while (!bRoundEnd && bScp703Activating)
			{
				ItemType itemType = (ItemType)Random.Next((int)ItemType.Coin);

				switch (itemType)
				{
					case ItemType.Ammo556:
					case ItemType.Ammo762:
					case ItemType.Ammo9mm:
						itemSpawner.SetPickup(itemType, 30.0f, vector3, Quaternion.Euler(Vector3.zero), 0, 0, 0);
						break;

					default:
						itemSpawner.SetPickup(itemType, 0.0f, vector3, Quaternion.Euler(Vector3.zero), 0, 0, 0);
						break;
				}

				yield return Timing.WaitForSeconds(Random.Next(20, 30));
			}
			Scp703id = 0;

			yield break;
		}

		private IEnumerator<float> Timing_OnScp914Activate(List<Player> players)
		{
			foreach (Player ply in players)
			{
				ReferenceHub hub = ply.GetHub();


				switch (Scp914Machine.singleton.knobState)
				{
					case Scp914Knob.Rough:
						switch ((ItemType)ply.GetCurrentItem().ItemType)
						{
							case ItemType.KeycardJanitor:
								// 希望卡没事[保佑][保佑][保佑]
								//player.GetCurrentItem().Remove();
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
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.ServerAddItem(ItemType.KeycardJanitor);
								break;
						}
						break;

					case Scp914Knob.Coarse:
						switch ((ItemType)ply.GetCurrentItem().ItemType)
						{
							case ItemType.KeycardJanitor:
								// 希望卡没事[保佑][保佑][保佑]
								// hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								break;

							case ItemType.KeycardScientist:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardJanitor);
								break;

							case ItemType.KeycardScientistMajor:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardScientist);
								break;

							case ItemType.KeycardZoneManager:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardScientist);
								break;

							case ItemType.KeycardGuard:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardZoneManager);
								break;

							case ItemType.KeycardSeniorGuard:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardGuard);
								break;

							case ItemType.KeycardContainmentEngineer:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardZoneManager);
								break;

							case ItemType.KeycardNTFLieutenant:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardSeniorGuard);
								break;

							case ItemType.KeycardNTFCommander:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardNTFLieutenant);
								break;

							case ItemType.KeycardFacilityManager:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardContainmentEngineer);
								break;

							case ItemType.KeycardChaosInsurgency:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardSeniorGuard);
								break;

							case ItemType.KeycardO5:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardFacilityManager);
								break;
						}
						break;

					case Scp914Knob.OneToOne:
						switch ((ItemType)ply.GetCurrentItem().ItemType)
						{
							case ItemType.KeycardJanitor:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardZoneManager);
								break;

							case ItemType.KeycardScientist:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardZoneManager);
								break;

							case ItemType.KeycardScientistMajor:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardGuard);
								break;

							case ItemType.KeycardZoneManager:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardGuard);
								break;

							case ItemType.KeycardGuard:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardScientistMajor);
								break;

							case ItemType.KeycardSeniorGuard:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardContainmentEngineer);
								break;

							case ItemType.KeycardContainmentEngineer:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardFacilityManager);
								break;

							case ItemType.KeycardNTFLieutenant:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardContainmentEngineer);
								break;

							case ItemType.KeycardNTFCommander:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardChaosInsurgency);
								break;

							case ItemType.KeycardFacilityManager:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardContainmentEngineer);
								break;

							case ItemType.KeycardChaosInsurgency:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardNTFCommander);
								break;

							case ItemType.KeycardO5:
								// 希望卡没事[保佑][保佑][保佑]
								// hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								break;
						}
						break;

					case Scp914Knob.Fine:
						switch ((ItemType)ply.GetCurrentItem().ItemType)
						{
							case ItemType.KeycardJanitor:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardScientist);
								break;

							case ItemType.KeycardScientist:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardScientistMajor);
								break;

							case ItemType.KeycardScientistMajor:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardContainmentEngineer);
								break;

							case ItemType.KeycardZoneManager:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardFacilityManager);
								break;

							case ItemType.KeycardGuard:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardSeniorGuard);
								break;

							case ItemType.KeycardSeniorGuard:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardNTFLieutenant);
								break;

							case ItemType.KeycardContainmentEngineer:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardNTFLieutenant:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardNTFCommander);
								break;

							case ItemType.KeycardNTFCommander:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardFacilityManager:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardChaosInsurgency:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardO5:
								// 希望卡没事[保佑][保佑][保佑]
								// hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								break;
						}
						break;

					case Scp914Knob.VeryFine:
						switch ((ItemType)ply.GetCurrentItem().ItemType)
						{
							case ItemType.KeycardJanitor:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardScientist);
								break;

							case ItemType.KeycardScientist:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardScientistMajor);
								break;

							case ItemType.KeycardScientistMajor:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardContainmentEngineer);
								break;

							case ItemType.KeycardZoneManager:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardFacilityManager);
								break;

							case ItemType.KeycardGuard:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardNTFLieutenant);
								break;

							case ItemType.KeycardSeniorGuard:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardNTFCommander);
								break;

							case ItemType.KeycardContainmentEngineer:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardNTFLieutenant:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardNTFCommander:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardFacilityManager:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardChaosInsurgency:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardO5);
								break;

							case ItemType.KeycardO5:
								// 希望卡没事[保佑][保佑][保佑]
								// hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								break;
						}

						if (hub.playerId != Scp035id)
							switch (hub.characterClassManager.NetworkCurClass)
							{
								case RoleType.ClassD:
									hub.playerStats.maxHP = ClassdMaxHP;
									hub.playerStats.Health = hub.playerStats.maxHP;
									break;

								case RoleType.Scientist:
									hub.playerStats.maxHP = ScientistMaxHP;
									hub.playerStats.Health = hub.playerStats.maxHP;
									break;

								case RoleType.FacilityGuard:
									hub.playerStats.maxHP = FacilityGuardHP;
									hub.playerStats.Health = hub.playerStats.maxHP;
									break;

								case RoleType.NtfCadet:
									hub.playerStats.maxHP = NtfCadetHP;
									hub.playerStats.Health = hub.playerStats.maxHP;
									break;

								case RoleType.NtfLieutenant:
									hub.playerStats.maxHP = NtfLieutenantHP;
									hub.playerStats.Health = hub.playerStats.maxHP;
									break;

								case RoleType.NtfScientist:
									hub.playerStats.maxHP = NtfScientistHP;
									hub.playerStats.Health = hub.playerStats.maxHP;
									break;

								case RoleType.NtfCommander:
									hub.playerStats.maxHP = NtfCommanderHP;
									hub.playerStats.Health = hub.playerStats.maxHP;
									break;

								case RoleType.ChaosInsurgency:
									hub.playerStats.maxHP = ChaosInsurgencyHP;
									hub.playerStats.Health = hub.playerStats.maxHP;
									break;

								case RoleType.Scp049:
									if (hub.playerStats.maxHP != Scp049MaxHP2)
									{
										hub.playerStats.maxHP = Scp049MaxHP2;
										SetScpBadge(hub.serverRoles, "SCP-049");
										hub.hints.Show(new TextHint("<b>强化成功!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									}
									else
										hub.hints.Show(new TextHint("<b>强化失败!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									break;

								case RoleType.Scp0492:
									hub.hints.Show(new TextHint("<b>强化失败, <color=#FF0000>SCP-049-2</color>不能进行强化!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									break;

								case RoleType.Scp096:
									if (Scp096id == 0)
									{
										Scp096id = hub.playerId;
										SetScpBadge(hub.serverRoles, "SCP-096");
										hub.playerStats.NetworkmaxArtificialHealth = (int)Scp096MaxShield;
										hub.hints.Show(new TextHint("<b>强化成功!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									}
									else
										hub.hints.Show(new TextHint("<b>强化失败!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									break;

								case RoleType.Scp106:
									if (hub.serverRoles.NetworkMyText != "SCP-106")
									{
										SetScpBadge(hub.serverRoles, "SCP-106");
										hub.hints.Show(new TextHint("<b>强化成功!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									}
									else
										hub.hints.Show(new TextHint("<b>强化失败!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									break;

								case RoleType.Scp173:
									if (hub.serverRoles.NetworkMyText != "SCP-173")
									{
										SetScpBadge(hub.serverRoles, "SCP-173");
										hub.playerEffectsController.EnableEffect<Scp207>();
										hub.playerEffectsController.EnableEffect<Scp207>();
										hub.playerEffectsController.EnableEffect<Scp207>();
										hub.playerEffectsController.EnableEffect<Scp207>();
										hub.hints.Show(new TextHint("<b>强化成功!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									}
									else
										hub.hints.Show(new TextHint("<b>强化失败!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									break;

								case RoleType.Scp93953:
									if (hub.playerId == Scp682id)
										hub.hints.Show(new TextHint("<b>强化失败, <color=#FF0000>SCP-682</color>不能进行强化!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									else if (hub.playerStats.maxHP != Scp939MaxHP2)
									{
										hub.playerStats.maxHP = Scp939MaxHP2;
										SetScpBadge(hub.serverRoles, "SCP-939-53");
										hub.playerStats.artificialHpDecay = -1.25f;
										hub.hints.Show(new TextHint("<b>强化成功!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									}
									else
										hub.hints.Show(new TextHint("<b>强化失败!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									break;

								case RoleType.Scp93989:
									if (hub.playerId == Scp682id)
										hub.hints.Show(new TextHint("<b>强化失败, <color=#FF0000>SCP-682</color>不能进行强化!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									else if (hub.playerStats.maxHP != Scp939MaxHP2)
									{
										hub.playerStats.maxHP = Scp939MaxHP2;
										SetScpBadge(hub.serverRoles, "SCP-939-89");
										hub.playerStats.artificialHpDecay = -1.25f;
										hub.hints.Show(new TextHint("<b>强化成功!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
									}
									else
										hub.hints.Show(new TextHint("<b>强化失败!</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
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

					if (Random.Next(4) == 0)
						hub.inventory.ServerAddItem(ItemType.Medkit);
					else if (Random.Next(3) == 0)
						hub.inventory.ServerAddItem(ItemType.Painkillers);
					hub.inventory.ServerAddItem(ItemType.Flashlight);

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
					hub.inventory.ServerAddItem(ItemType.Medkit);
					hub.inventory.ServerAddItem(ItemType.Flashlight);

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

				case RoleType.FacilityGuard:
					hub.inventory.ServerAddItem(ItemType.KeycardGuard);
					hub.inventory.ServerAddItem(ItemType.GunFSP9);
					hub.inventory.ServerAddItem(ItemType.Adrenaline);
					hub.inventory.ServerAddItem(ItemType.Medkit);
					hub.inventory.ServerAddItem(ItemType.GrenadeHE);
					hub.inventory.ServerAddItem(ItemType.GrenadeFlash);
					hub.inventory.ServerAddItem(ItemType.ArmorLight);
					hub.inventory.ServerAddItem(ItemType.Radio);

					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo556x45] = 40;
					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo9x19] = 200;
					hub.inventory.SendAmmoNextFrame = true;
					break;

				case RoleType.NtfPrivate:
					hub.inventory.ServerAddItem(ItemType.KeycardNTFOfficer);
					hub.inventory.ServerAddItem(ItemType.GunCrossvec);
					hub.inventory.ServerAddItem(ItemType.Medkit);

					if (Random.Next(3) == 0)
						hub.inventory.ServerAddItem(ItemType.GrenadeHE);
					else if (Random.Next(3) == 0)
						hub.inventory.ServerAddItem(ItemType.GrenadeFlash);

					hub.inventory.ServerAddItem(ItemType.ArmorCombat);
					hub.inventory.ServerAddItem(ItemType.Radio);

					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo556x45] = 80;
					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo9x19] = 200;
					hub.inventory.SendAmmoNextFrame = true;
					break;

				case RoleType.NtfSergeant:
					hub.inventory.ServerAddItem(ItemType.KeycardNTFLieutenant);
					hub.inventory.ServerAddItem(ItemType.GunE11SR);
					hub.inventory.ServerAddItem(ItemType.GunRevolver);
					hub.inventory.ServerAddItem(ItemType.Medkit);
					hub.inventory.ServerAddItem(ItemType.GrenadeHE);
					hub.inventory.ServerAddItem(ItemType.ArmorCombat);
					hub.inventory.ServerAddItem(ItemType.Radio);

					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo556x45] = 200;
					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo44cal] = 60;
					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo9x19] = 120;
					hub.inventory.SendAmmoNextFrame = true;
					break;

				case RoleType.NtfSpecialist:
					hub.inventory.ServerAddItem(ItemType.KeycardNTFLieutenant);
					hub.inventory.ServerAddItem(ItemType.GunE11SR);
					hub.inventory.ServerAddItem(ItemType.MicroHID);
					hub.inventory.ServerAddItem(ItemType.SCP500);
					hub.inventory.ServerAddItem(ItemType.SCP1853);
					hub.inventory.ServerAddItem(ItemType.GrenadeHE);
					hub.inventory.ServerAddItem(ItemType.ArmorCombat);
					hub.inventory.ServerAddItem(ItemType.Radio);

					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo556x45] = 200;
					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo9x19] = 120;
					hub.inventory.SendAmmoNextFrame = true;
					break;

				case RoleType.NtfCaptain:
					hub.inventory.ServerAddItem(ItemType.KeycardNTFCommander);
					hub.inventory.ServerAddItem(ItemType.GunE11SR);
					hub.inventory.ServerAddItem(ItemType.GunShotgun);
					hub.inventory.ServerAddItem(ItemType.Adrenaline);
					hub.inventory.ServerAddItem(ItemType.Medkit);
					hub.inventory.ServerAddItem(ItemType.GrenadeHE);
					hub.inventory.ServerAddItem(ItemType.ArmorHeavy);
					hub.inventory.ServerAddItem(ItemType.Radio);

					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo12gauge] = 70;
					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo556x45] = 200;
					hub.inventory.SendAmmoNextFrame = true;
					break;

				case RoleType.ChaosConscript:
					hub.inventory.ServerAddItem(ItemType.KeycardChaosInsurgency);
					hub.inventory.ServerAddItem(ItemType.GunAK);
					hub.inventory.ServerAddItem(ItemType.Medkit);
					hub.inventory.ServerAddItem(ItemType.Painkillers);
					hub.inventory.ServerAddItem(ItemType.ArmorCombat);

					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo762x39] = 160;
					hub.inventory.SendAmmoNextFrame = true;
					break;

				case RoleType.ChaosRifleman:
					hub.inventory.ServerAddItem(ItemType.KeycardChaosInsurgency);
					hub.inventory.ServerAddItem(ItemType.GunAK);
					hub.inventory.ServerAddItem(ItemType.Medkit);
					hub.inventory.ServerAddItem(ItemType.Painkillers);
					hub.inventory.ServerAddItem(ItemType.ArmorCombat);

					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo12gauge] = 30;
					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo762x39] = 200;
					hub.inventory.SendAmmoNextFrame = true;
					break;

				case RoleType.ChaosRepressor:
					hub.inventory.ServerAddItem(ItemType.KeycardChaosInsurgency);
					hub.inventory.ServerAddItem(ItemType.GunShotgun);
					hub.inventory.ServerAddItem(ItemType.Medkit);
					hub.inventory.ServerAddItem(ItemType.Painkillers);
					hub.inventory.ServerAddItem(ItemType.ArmorCombat);

					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo12gauge] = 100;
					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo44cal] = 60;
					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo762x39] = 120;
					hub.inventory.SendAmmoNextFrame = true;
					break;

				case RoleType.ChaosMarauder:
					hub.inventory.ServerAddItem(ItemType.KeycardChaosInsurgency);
					hub.inventory.ServerAddItem(ItemType.GunLogicer);
					hub.inventory.ServerAddItem(ItemType.GunShotgun);
					(hub.inventory.ServerAddItem(ItemType.ParticleDisruptor).PickupDropModel as FirearmPickup).NetworkStatus = new FirearmStatus(1, FirearmStatusFlags.None, 0);
					hub.inventory.ServerAddItem(ItemType.Adrenaline);
					hub.inventory.ServerAddItem(ItemType.Medkit);
					hub.inventory.ServerAddItem(ItemType.SCP207);
					hub.inventory.ServerAddItem(ItemType.ArmorHeavy);

					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo12gauge] = 70;
					hub.inventory.UserInventory.ReserveAmmo[ItemType.Ammo762x39] = 300;
					hub.inventory.SendAmmoNextFrame = true;
					break;

				case RoleType.Scp106:
					hub.inventory.ServerAddItem(ItemType.KeycardJanitor);
					hub.hints.Show(
						new TextHint($"<voffset=27em><size=120><b> </b></size>\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" +
						$"<size=35><b>按下Ctrl键可以在地上创建一个<color=#FF0000>诱捕陷阱</color></b></size>\n<size=35><b>触发陷阱的人会被传送至口袋空间</b></size></voffset>",
						new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 15.0f));
					break;

				case RoleType.Scp049:
				case RoleType.Scp096:
				case RoleType.Scp173:
				case RoleType.Scp93953:
				case RoleType.Scp93989:
					Timing.RunCoroutine(Timing_SelfHealth(hub));
					break;
			}

			yield break;
		}

		private IEnumerator<float> Timing_OnTriggerTrapItem(ReferenceHub victim)
		{
			victim.scp106PlayerScript.TeleportAnimation();
			victim.scp106PlayerScript.goingViaThePortal = true;

			yield return Timing.WaitForSeconds(3.0f);

			victim.playerEffectsController.EnableEffect<Corroding>(0.0f, false);
			victim.scp106PlayerScript.goingViaThePortal = false;
			victim.hints.Show(
				new TextHint("<b>你触发了<color=#FF0000>SCP-106</color>的诱捕陷阱</b>",
				new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));

			/*
			foreach (GameObject gameObject in PlayerManager.players)
			{
				ReferenceHub hub = GetReferenceHub(gameObject);
				if (hub.characterClassManager.NetworkCurClass == RoleType.Scp106)
					Timing.RunCoroutine(Timing_SendMessage(MessageType.Person, hub.playerId, $"<color=#FFFF00>{victim.nicknameSync.MyNick}</color>触发了你的<color=#FF0000>诱捕陷阱</color>", 5));
			}
			*/

			yield break;
		}

		private IEnumerator<float> Timing_OnPocketDimensionDie()
		{
			foreach (GameObject gameObject in PlayerManager.players)
			{
				ReferenceHub hub = ReferenceHub.GetHub(gameObject);

				if (hub.characterClassManager.NetworkCurClass == RoleType.Scp106)
				{
					HealthController healthController = hub.GetHealthControler();

					if (healthController.Health + Scp106Heal > healthController.MaxHealth)
						healthController.Health = healthController.MaxHealth;
					else
						healthController.Health += Scp106Heal;
				}
			}

			yield break;
		}

		private IEnumerator<float> Timing_PersonMessage(PersonMessage personMessage, Smod2.API.Player player)
		{
			string strText;
			personMessage.TextDisplay.Add(new Message(
				"<color=#FF0000>欢</color><color=#FF4800>迎</color><color=#FF5D00>来</color><color=#FF7200>到</color>" +
				"<color=#FF8A00>萌</color><color=#FFAF00>新</color><color=#FFD600>天</color><color=#FFF500>堂</color>" +
				"<color=#E8FF00>服</color><color=#9DFF00>务</color><color=#48FF00>器</color>" +
				"<color=#07FF00>(</color><color=#00FF2A>°</color><color=#00FF77>∀</color><color=#00FFC8>°</color><color=#00FFFF>)ﾉ</color>", 15));
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

					player.PersonalBroadcast(1, strText, true);
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