using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CustomPlayerEffects;
using Hints;
using InventorySystem;
using MEC;
using Mirror;
using Respawning;
using Scp914;
using UnityEngine;

using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;

using CommonPlugin.Extensions;

namespace CommonPlugin
{
    public class EventHandlers : IEventHandler079LevelUp, IEventHandler106CreatePortal, IEventHandlerCheckEscape, IEventHandlerRecallZombie, IEventHandlerCheckRoundEnd,
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

			Scp049id = 0;

			Scp079id = 0;

			Scp096id = 0;

			Scp106id = 0;

			Scp173id = 0;

			Scp181id = 0;

			Scp682id = 0;

			Scp703id = 0;

			Scp939id = 0;

			bScp035Detected = false;

			
		}

		public EventHandlers(CommonPlugin Plugin)
        {
			this.Plugin = Plugin;

			this.bRoundEnd = false;

			this.bWarhead = false;

			this.dWarheadRate = 0.0;

			this.Random = new System.Random();

			this.TrapItem = new List<int>();

			this.Coroutines = new List<CoroutineHandle>();

			this.PowerCutCooldown = 45;
		}

		// Public field
		public static int Scp035id;

		public static int Scp049id;

		public static int Scp079id;

		public static int Scp096id;

		public static int Scp106id;

		public static int Scp173id;

		public static int Scp181id;

		public static int Scp682id;

		public static int Scp703id;

		public static int Scp939id;


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

		private readonly List<int> TrapItem;

		private readonly List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();

		//private readonly MethodInfo SendSpawnMessage = typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static);

		// D级人员, 科学家, 九尾狐阵营, 混沌阵营
		private const int ClassdMaxHP = 100;

		private const int ScientistMaxHP = 100;

		private const int NtfMaxHP = 125;

		private const int NtfCaptainMaxHP = 150;

		private const int ChaosMaxHP = 125;

		private const int ChaosCommandHP = 150;

		// SCP-035
		private const float Scp035Hit = 4.0f;

		private const float Scp035Heal = 35.0f;

		// SCP-049 & SCP-049-2
		private const int Scp0492MaxHP = 500;

		private const int Scp049MaxHP = 2500;

		private const int Scp049MaxHP2 = 2800;

		private const float Scp049Heal = 2.0f;

		private const float Scp049Heal2 = 5.0f;

		// SCP-079
		private int PowerCutCooldown;

		private Scp079LevelType Scp079Level = Scp079LevelType.Level_1;

		// SCP-096
		public readonly int Scp096MaxHP = 1500;

		public static int Scp096AddShield = 70;

		public static float Scp096Shield = 350.0f;

		public static float Scp096MaxShield = 450.0f;

		private readonly float Scp096Heal = 2.0f;

		// SCP-106
		public const int Scp106MaxHP = 1000;

		private const float Scp106Heal = 20.0f;

		private static int Scp106Cooldown = 25;

		private static int Scp106LastTrap = 0;

		// SCP-173
		
		public const int Scp173MaxHP = 3200;

		private readonly float Heal173 = 7.5f;

		private readonly float Hurt173 = 0.67f;

		// SCP-682
		private const int Scp682MaxHP = 4000;

		private const float heal682 = 8.0f;

		private const float lifeSteal682 = 40.0f;

		// SCP-703
		public static bool bScp703Activating = false;

		// SCP-939
		public readonly int Scp939MaxHP = 3000;

		public readonly int Scp939MaxHP2 = 3300;

		private readonly float healScp939_1 = 1.0f;

		private readonly float healScp939_2 = 6.0f;

		private readonly float lifeStealScp939 = 25.0f;


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
					Scp079Level = Scp079LevelType.Level_2;
					PowerCutCooldown = 42;
					MeThodExtensions.FlickerLights(30.0f, 5);
					break;

				case 2:
					Scp079Level = Scp079LevelType.Level_3;
					PowerCutCooldown = 39;
					MeThodExtensions.FlickerLights(40.0f, 4);
					break;

				case 3:
					Scp079Level = Scp079LevelType.Level_4;
					PowerCutCooldown = 36;
					MeThodExtensions.FlickerLights(50.0f, 3);
					break;

				case 4:
					Scp079Level = Scp079LevelType.Level_5;
					PowerCutCooldown = 33;
					MeThodExtensions.FlickerLights(60.0f, 2);
					MeThodExtensions.SetScpBadge(ev.Player.GetHub().serverRoles, "SCP-079");
					break;
			}
		}

		public void On106CreatePortal(Player106CreatePortalEvent ev)
		{
			if (Plugin.Server.Map.WarheadDetonated)
				return;

			int duration = Plugin.Server.Round.Duration - Scp106LastTrap;
			if (duration > Scp106Cooldown)
			{
				Scp106LastTrap = Plugin.Server.Round.Duration;

				ItemType itemType = ItemType.None;
				TrapItemType trapItemType = (TrapItemType)Random.Next((int)TrapItemType.TrapItemCount);
				switch (trapItemType)
				{
					case TrapItemType.Adrenaline:
						itemType = ItemType.Adrenaline;
						break;

					case TrapItemType.GrenadeFlash:
						itemType = ItemType.GrenadeFlash;
						break;

					case TrapItemType.GrenadeFrag:
						itemType = ItemType.GrenadeHE;
						break;

					case TrapItemType.KeycardContainmentEngineer:
						itemType = ItemType.KeycardContainmentEngineer;
						break;

					case TrapItemType.KeycardFacilityManager:
						itemType = ItemType.KeycardFacilityManager;
						break;

					case TrapItemType.KeycardO5:
						itemType = ItemType.KeycardO5;
						break;

					case TrapItemType.Medkit:
						itemType = ItemType.Medkit;
						break;

					case TrapItemType.SCP018:
						itemType = ItemType.SCP018;
						break;

					case TrapItemType.SCP207:
						itemType = ItemType.SCP207;
						break;

					case TrapItemType.SCP500:
						itemType = ItemType.SCP500;
						break;
				}

				

				Inventory component = GameObject.Find("Host").GetComponent<Inventory>();
				Pickup pickup = component.SetPickup(itemType, 0.0f, new Vector3(ev.Position.x, ev.Position.y + 2.5f, ev.Position.z), Quaternion.Euler(Vector3.zero), 0, 0, 0);
				TrapItem.Add(pickup.gameObject.GetInstanceID());
				ev.Player.GetHub().hints.Show(new TextHint(
					"<b><color=#FF0000>诱捕陷阱</color>放置成功!</b>",
					new HintParameter[] { new StringHintParameter("") }, new HintEffect[] { HintEffectPresets.FadeOut() }, 5.0f));
			}
			else
				ev.Player.ShowHint($"<b>还需要<color=#FF0000>{Scp106Cooldown - duration}</color>秒才能放置诱捕陷阱</b>", 3.0f);
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
				MeThodExtensions.ClearScpBadge(ev.Player.GetHub().serverRoles);
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
			ev.Player.GetHub().GetHealthStat().
		}

		public void OnDecontaminate() => bScp703Activating = false;

		public void OnDetonate()
		{
			TrapItem.Clear();
			Plugin.Server.Map.OverchargeLights(900.0f, false);
		}

		public void OnMedicalUse(PlayerMedicalUseEvent ev)
		{
			ItemType itemType = (ItemType)ev.MedicalItem;
			ev.AmountArtificial = 0;
			ev.AmountHealth = 0;

			switch (itemType)
			{
				case ItemType.Medkit:
					Timing.RunCoroutine(Timing_OnMedkitEffect(ev.Player));
					break;

				case ItemType.SCP500:
					Timing.RunCoroutine(Timing_OnScp500Effect(ev.Player));
					break;

				case ItemType.Adrenaline:
					if (ev.Player.PlayerId == Scp035id)
						ev.AmountArtificial = 20;
					else
						ev.AmountArtificial = 40;
					Timing.RunCoroutine(Timing_OnAdrenalineEffect(ev.Player));
					break;

				case ItemType.Painkillers:
					ev.AmountArtificial = 5;
					Timing.RunCoroutine(Timing_OnPainkillersEffect(ev.Player));
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
			if (ev.DamageTypeVar == DamageType.NONE)
				return;

			double dDeath;
			while ((dDeath = Random.NextDouble()) > 0.432) { }
			dWarheadRate += dDeath + 0.345;

			ReferenceHub killer = ev.Killer.GetHub();
			ReferenceHub player = ev.Player.GetHub();
			
			string announceMessage;
			switch (player.characterClassManager.NetworkCurClass)
			{
				case RoleType.Scp049:
					announceMessage = ScpDeathInfo("SCP-049", ev.Killer.Name);
					Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0, announceMessage, 10));
					return;

				case RoleType.Scp079:
					announceMessage = ScpDeathInfo("SCP-079", ev.Killer.Name);
					Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0, announceMessage, 10));
					ClearScpBadge(player.serverRoles);
					return;

				case RoleType.Scp096:
					Scp096id = 0;
					announceMessage = ScpDeathInfo("SCP-096", ev.Killer.Name);
					Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0, announceMessage, 10));
					return;

				case RoleType.Scp106:
					announceMessage = ScpDeathInfo("SCP-106", ev.Killer.Name);
					Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0, announceMessage, 10));
					ClearScpBadge(player.serverRoles);
					return;

				case RoleType.Scp173:
					announceMessage = ScpDeathInfo("SCP-173", ev.Killer.Name);
					Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0, announceMessage, 10));
					return;

				case RoleType.Scp93953:
					if (ev.Player.PlayerId == Scp682id)
					{
						Scp682id = 0;
						announceMessage = ScpDeathInfo("SCP-682", ev.Killer.Name);
					}
					else
						announceMessage = ScpDeathInfo("SCP-939-53", ev.Killer.Name);
					player.playerStats.artificialHpDecay = 0.75f;
					Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0, announceMessage, 10));
					return;

				case RoleType.Scp93989:
					if (ev.Player.PlayerId == Scp682id)
					{
						Scp682id = 0;
						announceMessage = ScpDeathInfo("SCP-682", ev.Killer.Name);
					}
					else
						announceMessage = ScpDeathInfo("SCP-939-89", ev.Killer.Name);
					player.playerStats.artificialHpDecay = 0.75f;
					Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0, announceMessage, 10));
					return;

				default:
					if (player.playerId == Scp035id)
					{
						Scp035id = 0;
						player.playerStats.NetworkmaxArtificialHealth = 75;
						ClearScpBadge(player.serverRoles);
						announceMessage = ScpDeathInfo("SCP-035", ev.Killer.Name);
						Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0, announceMessage, 10));
						RespawnEffectsController.PlayCassieAnnouncement("SCP 0 3 5 CONTAINS SUCCESSFULLY", false, true);
						return;
					}
					else if (player.playerId == Scp181id)
					{
						Scp181id = 0;
						ClearScpBadge(player.serverRoles);
						announceMessage = ScpDeathInfo("SCP-181", ev.Killer.Name);
						Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0, announceMessage, 10));
						RespawnEffectsController.PlayCassieAnnouncement("SCP 1 8 1 CONTAINS SUCCESSFULLY", false, true);
						return;
					}
					break;
			}

			if (ev.Killer.PlayerId != ev.Player.PlayerId)
			{
				if (ev.Killer.PlayerId == Scp035id)
				{
					RoundSummary.kills_by_scp++;
					if (!bScp035Detected)
					{
						bScp035Detected = true;
						SetScpBadge(killer.serverRoles, "SCP-035");
						Timing.RunCoroutine(Timing_SendMessage(MessageType.All, 0, "警告: <color=#FF0000>SCP-035</color>已出现", 10));
						RespawnEffectsController.PlayCassieAnnouncement("WARNING SCP 0 3 5 CONTAINMENT BREACH", false, true);
					}
					return;
				}

				if (killer.characterClassManager.NetworkCurClass == RoleType.Scp106)
					if (killer.playerStats.Health + Scp106Heal < killer.playerStats.maxHP)
						killer.playerStats.Health += Scp106Heal;
					else
						killer.playerStats.Health = killer.playerStats.maxHP;
			}

			if (ev.DamageTypeVar == DamageType.POCKET_DECAY)
				Timing.RunCoroutine(Timing_OnPocketDimensionDie());
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (ev.Attacker.PlayerID != ev.Player.PlayerID)
			{
				ReferenceHub attackerhub = GetReferenceHub(ev.Attacker);
				ReferenceHub playerhub = GetReferenceHub(ev.Player);

				if (attackerhub.playerId == Scp035id)
				{
					switch (playerhub.characterClassManager.NetworkCurClass)
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

				if (playerhub.characterClassManager.NetworkCurClass == RoleType.Scp173)
				{
					ev.Damage *= Hurt173;
					return;
				}

				if (playerhub.playerId == Scp035id)
				{
					switch (ev.DamageType)
					{
						case DamageType.COM15:
						case DamageType.P90:
						case DamageType.E11_STANDARD_RIFLE:
						case DamageType.MP7:
						case DamageType.LOGICIER:
						case DamageType.USP:
							ev.Damage = (ev.Damage > 40.0f) ? Scp035Hit * 4 : Scp035Hit;
							return;
					}

					switch (attackerhub.characterClassManager.NetworkCurClass)
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
				else if (playerhub.playerId == Scp181id)
				{
					switch (attackerhub.characterClassManager.NetworkCurClass)
					{
						case RoleType.Scp049:
						case RoleType.Scp0492:
						case RoleType.Scp096:
						case RoleType.Scp106:
						case RoleType.Scp173:
						case RoleType.Scp93953:
						case RoleType.Scp93989:
							if (playerhub.inventory.items.Count > 0)
							{
								ev.Damage = 0.0f;
								playerhub.inventory.items.RemoveAt(Random.Next(playerhub.inventory.items.Count));
								playerhub.hints.Show(new TextHint("<b>你使用一件物品抵挡了一次来自<color=#FF0000>SCP</color>的伤害</b>",
									new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));
								attackerhub.hints.Show(new TextHint("<b><color=#FF0000>SCP-181</color>使用一件物品抵挡了你的一次伤害</b>",
									new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));
								return;
							}
							break;
					}
				}

				switch (attackerhub.characterClassManager.NetworkCurClass)
				{
					case RoleType.Scp93953:
					case RoleType.Scp93989:
						if (playerhub.playerId != Scp181id)
							playerhub.GetComponent<PlayerEffectsController>().EnableEffect<Blinded>(5.0f, true);

						if (attackerhub.playerId == Scp682id)
						{
							ev.Damage = 120.0f;
							int damage = playerhub.playerStats.maxHP - (int)(playerhub.playerStats.maxHP * 0.65);
							playerhub.playerStats.maxHP -= damage;
							playerhub.hints.Show(new TextHint($"<b><color=#FF0000>SCP-682</color> 给你造成了重伤效果 ( 减少 <color=#FF0000>{damage}</color> 点最大生命值 )</b>",
									new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));

							if (attackerhub.playerStats.Health + lifeSteal682 < attackerhub.playerStats.maxHP)
								attackerhub.playerStats.Health += lifeSteal682;
							else
								attackerhub.playerStats.Health = attackerhub.playerStats.maxHP;
						}
						else
						{
							ev.Damage = Random.Next(70, 100);
							playerhub.playerStats.maxHP -= (int)lifeStealScp939;
							playerhub.hints.Show(new TextHint($"<b><color=#FF0000>SCP-939</color> 给你造成了裂伤效果 ( 减少 <color=#FF0000>{lifeStealScp939}</color> 点最大生命值 )</b>",
									new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));

							if (attackerhub.playerStats.Health + lifeStealScp939 < attackerhub.playerStats.maxHP)
								attackerhub.playerStats.Health += lifeStealScp939;
							else
								attackerhub.playerStats.Health = attackerhub.playerStats.maxHP;
						}
						break;
				}
			}
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (MessageQueue.Messages.ContainsKey(ev.Player.PlayerId))
				return;

			PersonMessage personMessage = new PersonMessage(ev.Player.PlayerId, GetReferenceHub(ev.Player));
			MessageQueue.Messages.Add(ev.Player.PlayerId, personMessage);
			Coroutines.Add(Timing.RunCoroutine(Timing_PersonMessage(personMessage, ev.Player), "M" + ev.Player.PlayerId));
		}

		public void OnPlayerLeave(PlayerLeaveEvent ev)
		{
			GetReferenceHub(ev.Player).characterClassManager.NetworkCurClass = RoleType.Spectator;
			Timing.KillCoroutines("M" + ev.Player.PlayerId);

			lock (MessageQueue.Messages)
				MessageQueue.Messages.Remove(ev.Player.PlayerId);
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
				case ItemType.SCP018:
				case ItemType.Adrenaline:
					if (TrapItem.Contains(ev.Item.UniqueIdentifier))
					{
						ReferenceHub hub = GetReferenceHub(ev.Player);
						if (hub.scp106PlayerScript.goingViaThePortal) {
							ev.ChangeTo = Smod2.API.ItemType.NONE;
							return;
						}
						else if (hub.playerId == Scp035id) {
							ev.ChangeTo = Smod2.API.ItemType.NONE;
							hub.hints.Show(new TextHint("<b>该物品为<color=#FF0000>SCP-106</color>的诱捕陷阱</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));
							return;
						}

						TrapItem.Remove(ev.Item.UniqueIdentifier);
						ev.ChangeTo = Smod2.API.ItemType.NONE;
						ev.Item.Remove();
						Timing.RunCoroutine(Timing_OnTriggerTrapItem(ev.Player));
					}
					break;
            }
		}

		public void OnPlayerTriggerTesla(PlayerTriggerTeslaEvent ev)
		{
			if (ev.Player.PlayerId == Scp181id)
				ev.Triggerable = false;
		}

		public void OnPocketDimensionDie(PlayerPocketDimensionDieEvent ev)
		{
			if (!Plugin.Server.Map.WarheadDetonated && ev.Player.PlayerID == Scp181id)
			{
				ev.Die = false;

				ReferenceHub hub = ev.Player.GetHub();

				HCZRoomType hczRoomType = (HCZRoomType)Random.Next((int)HCZRoomType.HczRoomCount);

				switch (hczRoomType)
				{
					case HCZRoomType.Scp049Room:
						hub.playerMovementSync.OverridePosition(MapManager.Scp049Room.Position);
						break;

					case HCZRoomType.Scp079Room:
						hub.playerMovementSync.OverridePosition(MapManager.Scp079Room.Position);
						break;

					case HCZRoomType.Scp096Room:
						hub.playerMovementSync.OverridePosition(MapManager.Scp096Room.Position);
						break;

					case HCZRoomType.Scp106Room:
						hub.playerMovementSync.OverridePosition(MapManager.Scp106Room.Position);
						break;

					case HCZRoomType.Scp939Room:
						hub.playerMovementSync.OverridePosition(MapManager.Scp939Room.Position);
						break;

					case HCZRoomType.MircoHIDRoom:
						hub.playerMovementSync.OverridePosition(MapManager.MircohidRoom.Position);
						break;

					case HCZRoomType.ServersRoom:
						hub.playerMovementSync.OverridePosition(MapManager.ServersRoom.Position);
						break;
				}
			}
		}

		public void OnPocketDimensionEnter(PlayerPocketDimensionEnterEvent ev)
		{
			if (ev.Player.PlayerID == Scp181id && Plugin.Server.Map.WarheadDetonated)
				ev.Player.Kill(DamageType.WARHEAD);
		}

		public void OnPocketDimensionExit(PlayerPocketDimensionExitEvent ev)
		{
			HCZRoomType roomType = (HCZRoomType)Random.Next((int)HCZRoomType.HczRoomCount);
			switch (roomType)
			{
				case HCZRoomType.Scp049Room:
					ev.ExitPosition = MapManager.Scp049Room.Position2;
					break;

				case HCZRoomType.Scp079Room:
					ev.ExitPosition = MapManager.Scp079Room.Position2;
					break;

				case HCZRoomType.Scp096Room:
					ev.ExitPosition = MapManager.Scp096Room.Position2;
					break;

				case HCZRoomType.Scp106Room:
					ev.ExitPosition = MapManager.Scp106Room.Position2;
					break;

				case HCZRoomType.Scp939Room:
					ev.ExitPosition = MapManager.Scp939Room.Position2;
					break;

				case HCZRoomType.MircoHIDRoom:
					ev.ExitPosition = MapManager.MircohidRoom.Position2;
					break;

				case HCZRoomType.ServersRoom:
					ev.ExitPosition = MapManager.ServersRoom.Position2;
					break;
			}
		}

		public void OnRecallZombie(PlayerRecallZombieEvent ev) => Timing.RunCoroutine(Timing_OnRecallZombie(ev.Target));

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (Scp035id != 0)
				if (ev.Round.Stats.ClassDEscaped == 0 && ev.Round.Stats.ScientistsEscaped == 0)
					ev.LeadingTeam = LeadingTeam.ANOMALIES;
				else
					ev.LeadingTeam = LeadingTeam.DRAW;

			bRoundEnd = true;
		}

		public void OnRoundStart(RoundStartEvent ev) => Timing.RunCoroutine(Timing_OnRoundStart());

		public void OnScp096AddTarget(Scp096AddTargetEvent ev)
		{
			if (ev.Target.PlayerId == Scp035id)
				ev.Allow = false;
		}

		public void OnSCP914Activate(SCP914ActivateEvent ev)
		{
			List<Smod2.API.Item> items = new List<Smod2.API.Item>();
			foreach (Smod2.API.Item item in ev.ItemInputs)
				if (TrapItem.Contains(item.UniqueIdentifier))
					items.Add(item);
			foreach (Smod2.API.Item item in items)
				ev.ItemInputs.Remove(item);

			Timing.RunCoroutine(Timing_OnScp914Activate(ev.PlayerInputs));
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			ReferenceHub hub = ev.Player.GetHub();
			ev.UsingDefaultItem = false;
			ev.Items.Clear();
			ev.Player.GetHub().inventory.drop

			switch (hub.characterClassManager.NetworkCurClass)
			{
				case RoleType.ClassD:
					Timing.RunCoroutine(Timing_OnSetRole(hub, ClassdMaxHP));
					break;

				case RoleType.Scientist:
					Timing.RunCoroutine(Timing_OnSetRole(hub, ScientistMaxHP));
					break;

				case RoleType.FacilityGuard:
				case RoleType.NtfPrivate:
				case RoleType.NtfSergeant:
				case RoleType.NtfSpecialist:
					Timing.RunCoroutine(Timing_OnSetRole(hub, NtfMaxHP));
					break;

				case RoleType.NtfCaptain:
					Timing.RunCoroutine(Timing_OnSetRole(hub, NtfCaptainMaxHP));
					break;

				case RoleType.ChaosInsurgency:
					Timing.RunCoroutine(Timing_OnSetRole(hub, ChaosInsurgencyHP));
					break;

				case RoleType.Scp049:
					Timing.RunCoroutine(Timing_OnSetRole(hub, Scp049MaxHP));
					break;

				case RoleType.Scp0492:
					Timing.RunCoroutine(Timing_OnSetRole(hub, Scp0492MaxHP));
					break;

				case RoleType.Scp079:
					Timing.RunCoroutine(Timing_OnScp079SwitchTime(hub));
					break;

				case RoleType.Scp096:
					Timing.RunCoroutine(Timing_OnSetRole(hub, Scp096MaxHP));
					break;

				case RoleType.Scp106:
					Timing.RunCoroutine(Timing_OnSetRole(hub, Scp106MaxHP));
					break;

				case RoleType.Scp173:
					Timing.RunCoroutine(Timing_OnSetRole(hub, Scp173MaxHP));
					break;

				case RoleType.Scp93953:
				case RoleType.Scp93989:
					if (hub.playerId == Scp682id)
						Timing.RunCoroutine(Timing_OnSetRole(hub, Scp682MaxHP));
					else
						Timing.RunCoroutine(Timing_OnSetRole(hub, Scp939MaxHP));
					break;

				case RoleType.Spectator:
					if (hub.playerId == Scp035id)
					{
						Scp035id = 0;
						hub.playerStats.NetworkmaxArtificialHealth = 75;
						ClearScpBadge(hub.serverRoles);
					}
					else if (hub.playerId == Scp181id)
					{
						Scp181id = 0;
						ClearScpBadge(hub.serverRoles);
					}
					else if (hub.playerId == Scp682id)
					{
						Scp682id = 0;
						ClearScpBadge(hub.serverRoles);
					}
					break;
			}
		}

		public void OnStopCountdown(WarheadStopEvent ev) => ev.Cancel = bWarhead == true;

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			dWarheadRate += ((maxPlayer - PlayerManager.players.Count + 10) * 0.622);

			foreach (Ragdoll doll in Object.FindObjectsOfType<Ragdoll>())
				NetworkServer.Destroy(doll.gameObject);

			if (dWarheadRate > 25.0)
				foreach (Pickup item in Object.FindObjectsOfType<Pickup>())
				{
					switch (item.ItemId)
					{
						case ItemType.KeycardJanitor:
						case ItemType.KeycardScientist:
						case ItemType.KeycardScientistMajor:
						case ItemType.KeycardZoneManager:
						case ItemType.KeycardGuard:
						case ItemType.KeycardSeniorGuard:
						case ItemType.KeycardNTFLieutenant:
						case ItemType.KeycardNTFCommander:
						case ItemType.KeycardChaosInsurgency:
						case ItemType.Radio:
						case ItemType.WeaponManagerTablet:
						case ItemType.Ammo556:
						case ItemType.Disarmer:
						case ItemType.Ammo762:
						case ItemType.Ammo9mm:
						case ItemType.Painkillers:
						case ItemType.Coin:
							item.Delete();
							break;

						case ItemType.Flashlight:
							if (Scp703id != item.gameObject.GetInstanceID())
								item.Delete();
							break;

						case ItemType.GunCOM15:
						case ItemType.Medkit:
						case ItemType.SCP500:
						case ItemType.SCP207:
						case ItemType.GunE11SR:
						case ItemType.GunProject90:
						case ItemType.GunMP7:
						case ItemType.GunLogicer:
						case ItemType.GrenadeFrag:
						case ItemType.GrenadeFlash:
						case ItemType.GunUSP:
						case ItemType.SCP018:
						case ItemType.Adrenaline:
							if (Random.Next(2) == 0)
							{
								if (TrapItem.Contains(item.gameObject.GetInstanceID()))
									TrapItem.Remove(item.gameObject.GetInstanceID());
								item.Delete();
							}
							break;
					}
				}

			// Automatic Warhead
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
			TrapItem.Clear();
			MapManager.GetRooms();
			Patches.ServerRolesPatch.SetStartScreen();

			Scp035id = 0;
			Scp079id = 0;
			Scp181id = 0;
			Scp682id = 0;

			bScp035Detected = false;
			Scp079Level = Scp079LevelType.Level_1;
			PowerCutCooldown = 50;
			Scp106LastTrap = 0;
			Scp703id = 0;

			bWarhead = false;
			bRoundEnd = false;
			bScp703Activating = false;
			dWarheadRate = 0.0;
		}

		private IEnumerator<float> Timing_SelfHealth(ReferenceHub hub)
        {
			int n = 0;
			Vector3 v3 = Vector3.zero; 
			Transform transform = hub.transform;
			switch (hub.characterClassManager.NetworkCurClass)
			{
				case RoleType.Scp049:
					while (!bRoundEnd && hub.characterClassManager.NetworkCurClass == RoleType.Scp049)
					{
						if (transform.position == v3)
                        {
							if(++n > selfHealCooldown)
                            {
								if (hub.playerStats.maxHP == Scp049MaxHP2)
								{
									if (hub.playerStats.Health + healScp049_2 < Scp049MaxHP2)
										hub.playerStats.Health += healScp049_2;
									else
										hub.playerStats.Health = hub.playerStats.maxHP;
								}
								else
								{
									if (hub.playerStats.Health + Scp049Heal2 < hub.playerStats.maxHP)
										hub.playerStats.Health += Scp049Heal2;
									else
										hub.playerStats.Health = hub.playerStats.maxHP;
								}

								yield return Timing.WaitForSeconds(1.0f);
								continue;
							}
						}
						else
                        {
							n = 0;
							v3 = transform.position;
						}

						if (hub.playerStats.Health + Scp049Heal < hub.playerStats.maxHP)
							hub.playerStats.Health += Scp049Heal;
						else
							hub.playerStats.Health = hub.playerStats.maxHP;

						yield return Timing.WaitForSeconds(1.0f);
					}
					break;

				case RoleType.Scp079:
					while (!bRoundEnd && hub.characterClassManager.NetworkCurClass == RoleType.Scp079)
						yield return Timing.WaitForSeconds(2.0f);
					Scp079id = 0;
					break;

				case RoleType.Scp096:
					while (!bRoundEnd && hub.characterClassManager.NetworkCurClass == RoleType.Scp096)
					{
						if (transform.position == v3)
                        {
							if (++n > selfHealCooldown)
                            {
								if (hub.playerStats.Health + Scp096Heal < hub.playerStats.maxHP)
									hub.playerStats.Health += Scp096Heal;
								else
									hub.playerStats.Health = hub.playerStats.maxHP;

								yield return Timing.WaitForSeconds(1.0f);
								continue;
							}
						}
						else
                        {
							n = 0;
							v3 = transform.position;
						}

						yield return Timing.WaitForSeconds(1.0f);
					}
					Scp096id = 0;
					break;

				case RoleType.Scp173:
					while (!bRoundEnd && hub.characterClassManager.NetworkCurClass == RoleType.Scp173)
					{
						if (transform.position == v3)
                        {
							if (++n > selfHealCooldown)
                            {
								if (hub.playerStats.Health + Heal173 < hub.playerStats.maxHP)
									hub.playerStats.Health += Heal173;
								else
									hub.playerStats.Health = hub.playerStats.maxHP;

								yield return Timing.WaitForSeconds(1.0f);
								continue;
							}
						}
						else
                        {
							n = 0;
							v3 = transform.position;
						}

						yield return Timing.WaitForSeconds(1.0f);
					}
					break;

				case RoleType.Scp93953:
				case RoleType.Scp93989:
					while (!bRoundEnd && (hub.characterClassManager.NetworkCurClass == RoleType.Scp93953 || hub.characterClassManager.NetworkCurClass == RoleType.Scp93989))
					{
						if (hub.playerId == Scp682id)
						{
							if (hub.playerStats.Health + heal682 < hub.playerStats.maxHP)
								hub.playerStats.Health += heal682;
							else
								hub.playerStats.Health = hub.playerStats.maxHP;

							yield return Timing.WaitForSeconds(1.0f);
						}
						else
						{
							if (transform.position == v3)
							{
								if (++n > selfHealCooldown)
								{
									if (hub.playerStats.Health + healScp939_2 < hub.playerStats.maxHP)
										hub.playerStats.Health += healScp939_2;
									else
										hub.playerStats.Health = hub.playerStats.maxHP;

									yield return Timing.WaitForSeconds(1.0f);
									continue;
								}
							}
							else
							{
								n = 0;
								v3 = transform.position;
							}

							if (hub.playerStats.maxHP == Scp939MaxHP2)
							{
								if (hub.playerStats.Health + healScp939_1 < hub.playerStats.maxHP)
									hub.playerStats.Health += healScp939_1;
								else
									hub.playerStats.Health = hub.playerStats.maxHP;
							}
							else
							{
								if (hub.playerStats.Health + healScp939_1 < hub.playerStats.maxHP)
									hub.playerStats.Health += healScp939_1;
								else
									hub.playerStats.Health = hub.playerStats.maxHP;
							}

							yield return Timing.WaitForSeconds(1.0f);
						}
					}
					hub.playerStats.artificialHpDecay = 0.75f;
					break;
			}
			ClearScpBadge(hub.serverRoles);

			yield break;
		}

		private IEnumerator<float> Timing_OnAdrenalineEffect(Player player)
		{
			ReferenceHub hub = GetReferenceHub(player);
			float hp = player.PlayerId == Scp035id ? 0.2f : 0.5f;

			for (int i = 0; i < 80; i++)
			{
				if (hub.playerStats.Health + hp > hub.playerStats.maxHP)
					hub.playerStats.Health = hub.playerStats.maxHP;
				else
					hub.playerStats.Health += hp;

				yield return Timing.WaitForSeconds(0.2f);
				if (hub.characterClassManager.NetworkCurClass == RoleType.Spectator)
					yield break;
			}

			yield break;
		}

		private IEnumerator<float> Timing_OnFlickerLights()
		{
			yield return Timing.WaitForSeconds(30.0f);

			while (!bRoundEnd && Scp079id != 0)
			{
				switch (Scp079Level)
				{
					case Scp079LevelType.Level_1:
						MeThodExtensions.FlickerLights(33.0f);
						break;

					case Scp079LevelType.Level_2:
						MeThodExtensions.FlickerLights(36.0f);
						break;

					case Scp079LevelType.Level_3:
						MeThodExtensions.FlickerLights(39.0f);
						break;

					case Scp079LevelType.Level_4:
						MeThodExtensions.FlickerLights(42.0f);
						break;

					case Scp079LevelType.Level_5:
						MeThodExtensions.FlickerLights(45.0f);
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

		private IEnumerator<float> Timing_OnMedkitEffect(Player player)
		{
			ReferenceHub hub = GetReferenceHub(player);
			if (hub.playerId == Scp035id)
			{
				if (hub.playerStats.Health + Scp035Heal > hub.playerStats.maxHP)
					hub.playerStats.Health = hub.playerStats.maxHP;
				else
					hub.playerStats.Health += Scp035Heal;
			}
			else
			{
				if (hub.playerStats.Health + MedkitHeal > hub.playerStats.maxHP)
					hub.playerStats.Health = hub.playerStats.maxHP;
				else
					hub.playerStats.Health += MedkitHeal;
			}

			yield break;
		}

		private IEnumerator<float> Timing_OnRecallZombie(Player player)
		{
			ReferenceHub hub = GetReferenceHub(player);
			hub.playerStats.maxHP = Scp0492MaxHP;
			hub.playerStats.Health = Scp0492MaxHP;

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
			int randomGen = 0;
			List<Player> Players = Plugin.Server.GetPlayers(Smod2.API.RoleType.CLASSD);

			// 创建SCP-181
			if (Players.Count > 3)
			{
				randomGen = Random.Next(Players.Count);
				hub = GetReferenceHub(Players[randomGen]);
				SetScp181(hub);
				Players.RemoveAt(randomGen);
			}

			// 在基础上增加科学家和设施警卫
			Players.AddRange(Plugin.Server.GetPlayers(Smod2.API.RoleType.SCIENTIST));
			Players.AddRange(Plugin.Server.GetPlayers(Smod2.API.RoleType.FACILITY_GUARD));

			// 创建SCP-035
			if (PlayerManager.players.Count > 10)
			{
				randomGen = Random.Next(Players.Count);
				hub = GetReferenceHub(Players[randomGen]);
				SetScp035(hub);
				Players.RemoveAt(randomGen);
			}

			// 创建SCP-682或本局未生成的SCP
			if (PlayerManager.players.Count > 15)
			{
				randomGen = Random.Next(Players.Count);
				hub = GetReferenceHub(Players[randomGen]);
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

		private IEnumerator<float> Timing_OnScp500Effect(Player player)
		{
			ReferenceHub hub = GetReferenceHub(player);
			switch (hub.characterClassManager.NetworkCurClass)
			{
				case RoleType.ClassD:
					hub.playerStats.maxHP = ClassdMaxHP;
					break;

				case RoleType.Scientist:
					hub.playerStats.maxHP = ScientistMaxHP;
					break;

				case RoleType.FacilityGuard:
					hub.playerStats.maxHP = FacilityGuardHP;
					break;

				case RoleType.NtfCadet:
					hub.playerStats.maxHP = NtfCadetHP;
					break;

				case RoleType.NtfLieutenant:
					hub.playerStats.maxHP = NtfLieutenantHP;
					break;

				case RoleType.NtfScientist:
					hub.playerStats.maxHP = NtfScientistHP;
					break;

				case RoleType.NtfCommander:
					hub.playerStats.maxHP = NtfCommanderHP;
					break;

				case RoleType.ChaosInsurgency:
					hub.playerStats.maxHP = ChaosInsurgencyHP;
					break;

				case RoleType.Tutorial:
					hub.playerStats.maxHP = ClassdMaxHP;
					break;
			}
			hub.playerStats.Health = hub.playerStats.maxHP;

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
				ReferenceHub hub = GetReferenceHub(ply);
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
							case ItemType.KeycardScientistMajor:
							case ItemType.KeycardZoneManager:
							case ItemType.KeycardGuard:
							case ItemType.KeycardSeniorGuard:
							case ItemType.KeycardContainmentEngineer:
							case ItemType.KeycardNTFLieutenant:
							case ItemType.KeycardNTFCommander:
							case ItemType.KeycardFacilityManager:
							case ItemType.KeycardChaosInsurgency:
							case ItemType.KeycardO5:
								hub.inventory.items.Remove(hub.inventory.GetItemInHand());
								hub.inventory.AddNewItem(ItemType.KeycardJanitor);
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

		private IEnumerator<float> Timing_OnSetRole(ReferenceHub hub, int hp)
		{
			yield return Timing.WaitForOneFrame;

			hub.playerStats.maxHP = hp;
			hub.playerStats.Health = hp;
			switch (hub.characterClassManager.NetworkCurClass)
			{
				case RoleType.ClassD:
					if (Random.Next(5) == 0)
						hub.inventory.AddNewItem(ItemType.KeycardScientist);
					else if (Random.Next(4) == 0)
						hub.inventory.AddNewItem(ItemType.KeycardJanitor);

					if (Random.Next(4) == 0)
						hub.inventory.AddNewItem(ItemType.Medkit);
					else if (Random.Next(3) == 0)
						hub.inventory.AddNewItem(ItemType.Painkillers);
					hub.inventory.AddNewItem(ItemType.Flashlight);

					if (Random.Next(10) == 0)
						hub.inventory.AddNewItem(ItemType.SCP207);
					else if (Random.Next(12) == 0)
						hub.inventory.AddNewItem(ItemType.SCP268);
					else if (Random.Next(8) == 0)
						hub.inventory.AddNewItem(ItemType.SCP018);

					int item1 = Random.Next(3);
					ItemType bulletType1 = (item1 == 0) ? ItemType.Ammo556 : (item1 == 1) ? ItemType.Ammo762 : ItemType.Ammo9mm;
					hub.inventory.AddNewItem(bulletType1, 10);
					break;

				case RoleType.Scientist:
					hub.inventory.AddNewItem(ItemType.KeycardScientist);
					hub.inventory.AddNewItem(ItemType.Medkit);
					hub.inventory.AddNewItem(ItemType.Flashlight);

					if (Random.Next(5) == 0)
						hub.inventory.AddNewItem(ItemType.SCP500);
					else if (Random.Next(4) == 0)
						hub.inventory.AddNewItem(ItemType.Adrenaline);

					if (Random.Next(5) == 0)
						hub.inventory.AddNewItem(ItemType.SCP268);
					else if (Random.Next(5) == 0)
						hub.inventory.AddNewItem(ItemType.SCP018);
					else if (Random.Next(100) == 0)
						hub.inventory.AddNewItem(ItemType.MicroHID, 0);

					int item2 = Random.Next(3);
					ItemType bulletType2 = (item2 == 0) ? ItemType.Ammo556 : (item2 == 1) ? ItemType.Ammo762 : ItemType.Ammo9mm;
					hub.inventory.AddNewItem(bulletType2, 10);
					break;

				case RoleType.FacilityGuard:
					hub.inventory.AddNewItem(ItemType.KeycardGuard);
					hub.inventory.AddNewItem(ItemType.GunMP7);
					hub.inventory.AddNewItem(ItemType.GunUSP);
					hub.inventory.AddNewItem(ItemType.Medkit);
					hub.inventory.AddNewItem(ItemType.GrenadeFrag);
					hub.inventory.AddNewItem(ItemType.GrenadeFlash);
					hub.inventory.AddNewItem(ItemType.Flashlight);
					hub.inventory.AddNewItem(ItemType.Disarmer);
					break;

				case RoleType.NtfCadet:
					hub.inventory.AddNewItem(ItemType.KeycardSeniorGuard);
					hub.inventory.AddNewItem(ItemType.GunProject90);
					hub.inventory.AddNewItem(ItemType.Medkit);

					if (Random.Next(3) == 0)
						hub.inventory.AddNewItem(ItemType.GrenadeFrag);
					else if (Random.Next(3) == 0)
						hub.inventory.AddNewItem(ItemType.GrenadeFlash);

					hub.inventory.AddNewItem(ItemType.Flashlight);
					hub.inventory.AddNewItem(ItemType.WeaponManagerTablet);
					hub.inventory.AddNewItem(ItemType.Disarmer);
					break;

				case RoleType.NtfLieutenant:
					hub.inventory.AddNewItem(ItemType.KeycardNTFLieutenant);
					hub.inventory.AddNewItem(ItemType.GunE11SR);
					hub.inventory.AddNewItem(ItemType.GunUSP);
					hub.inventory.AddNewItem(ItemType.Medkit);
					hub.inventory.AddNewItem(ItemType.GrenadeFrag);
					hub.inventory.AddNewItem(ItemType.Flashlight);
					hub.inventory.AddNewItem(ItemType.WeaponManagerTablet);
					hub.inventory.AddNewItem(ItemType.Disarmer);
					break;

				case RoleType.NtfScientist:
					hub.inventory.AddNewItem(ItemType.KeycardNTFLieutenant);
					hub.inventory.AddNewItem(ItemType.MicroHID);
					hub.inventory.AddNewItem(ItemType.GunE11SR);
					hub.inventory.AddNewItem(ItemType.SCP500);
					hub.inventory.AddNewItem(ItemType.Flashlight);
					hub.inventory.AddNewItem(ItemType.Disarmer);
					break;

				case RoleType.NtfCommander:
					hub.inventory.AddNewItem(ItemType.KeycardNTFLieutenant);
					hub.inventory.AddNewItem(ItemType.GunE11SR);
					hub.inventory.AddNewItem(ItemType.GunUSP);
					hub.inventory.AddNewItem(ItemType.Adrenaline);
					hub.inventory.AddNewItem(ItemType.GrenadeFrag);
					hub.inventory.AddNewItem(ItemType.Flashlight);
					hub.inventory.AddNewItem(ItemType.WeaponManagerTablet);
					hub.inventory.AddNewItem(ItemType.Disarmer);
					break;

				case RoleType.ChaosInsurgency:
					hub.inventory.AddNewItem(ItemType.KeycardChaosInsurgency);
					hub.inventory.AddNewItem(ItemType.GunLogicer);
					hub.inventory.AddNewItem(ItemType.Medkit);
					hub.inventory.AddNewItem(ItemType.Painkillers);
					hub.inventory.AddNewItem(ItemType.Flashlight);
					hub.inventory.AddNewItem(ItemType.Disarmer);
					break;

				case RoleType.Scp106:
					hub.hints.Show(new TextHint($"<voffset=27em><size=120><b> </b></size>\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n<size=35><b>你放置传送门的时候可以在地上随机创建一个物品(<color=#FF0000>诱捕陷阱</color>)</b></size>\n<size=35><b>尝试捡起该物品的人会被传送至口袋空间, 并且该物品会消失</b></size></voffset>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 15.0f));
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

		private IEnumerator<float> Timing_OnTriggerTrapItem(Player player)
        {
			ReferenceHub victim = GetReferenceHub(player);
			victim.scp106PlayerScript.RpcTeleportAnimation();
			victim.scp106PlayerScript.goingViaThePortal = true;
			yield return Timing.WaitForSeconds(3.5f);
			victim.playerMovementSync.OverridePosition(Vector3.down * 1997.0f, 0.0f, true);
			victim.scp106PlayerScript.goingViaThePortal = false;
			victim.hints.Show(new TextHint("<b>你触发了<color=#FF0000>SCP-106</color>的诱捕陷阱</b>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 5.0f));
			victim.playerEffectsController.GetEffect<Corroding>().IsInPd = true;
			victim.playerEffectsController.EnableEffect<Corroding>(0.0f, false);

			foreach (GameObject gameObject in PlayerManager.players)
			{
				ReferenceHub hub = GetReferenceHub(gameObject);
				if (hub.characterClassManager.NetworkCurClass == RoleType.Scp106)
					Timing.RunCoroutine(Timing_SendMessage(MessageType.Person, hub.playerId, $"<color=#FFFF00>{victim.nicknameSync.MyNick}</color>触发了你的<color=#FF0000>诱捕陷阱</color>", 5));
			}

			yield break;
		}

		private IEnumerator<float> Timing_OnPainkillersEffect(Player player)
        {
			ReferenceHub hub = GetReferenceHub(player);
			for (int i = 0; i < 40; i++)
			{
				if (hub.playerStats.Health + 1.0f < hub.playerStats.maxHP)
					hub.playerStats.Health += 1.0f;
				else
					hub.playerStats.Health = hub.playerStats.maxHP;

				yield return Timing.WaitForSeconds(1.0f);
				if (hub.characterClassManager.NetworkCurClass == RoleType.Spectator)
					yield break;
			}

			yield break;
		}

		private IEnumerator<float> Timing_OnPocketDimensionDie()
		{
			foreach (GameObject gameObject in PlayerManager.players)
            {
				ReferenceHub hub = GetReferenceHub(gameObject);
				if (hub.characterClassManager.NetworkCurClass == RoleType.Scp106){
					if (hub.playerStats.Health + Scp106Heal < hub.playerStats.maxHP)
						hub.playerStats.Health += Scp106Heal;
					else
						hub.playerStats.Health = hub.playerStats.maxHP;
				}
			}

			yield break;
		}

		private IEnumerator<float> Timing_PersonMessage(PersonMessage personMessage, Player player)
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
							case RoleType.NtfCadet:
							case RoleType.NtfLieutenant:
							case RoleType.NtfScientist:
							case RoleType.NtfCommander:
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
							case RoleType.ChaosInsurgency:
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

		private IEnumerator<float> Timing_PowerCutTimer()
		{


			yield break;
		}
	}
}