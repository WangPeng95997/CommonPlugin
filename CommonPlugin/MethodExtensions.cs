using UnityEngine;
using Hints;

namespace CommonPlugin.Extensions
{
    public class MeThodExtensions
	{
		private static System.Random Random = new System.Random();

		public static void ClearScpBadge(ServerRoles serverRoles, string badgeColor = "default")
		{
			if (string.IsNullOrEmpty(serverRoles.NetworkGlobalBadge))
			{
				serverRoles.HiddenBadge = null;
				serverRoles.NetworkMyText = null;
				serverRoles.NetworkMyColor = badgeColor;
				serverRoles.RpcResetFixed();
			}
		}

		public static RoleType GetRandomScp()
        {
			bool bRandomEnd = false, bScp049 = false, bScp096 = false, bScp106 = false, bScp173 = false;
			RoleType roleType = RoleType.None;

			foreach (GameObject gameObject in PlayerManager.players)
				switch (ReferenceHub.GetHub(gameObject).characterClassManager.NetworkCurClass)
				{
					case RoleType.Scp049:
						bScp049 = true;
						break;

					case RoleType.Scp096:
						bScp096 = true;
						break;

					case RoleType.Scp106:
						bScp106 = true;
						break;

					case RoleType.Scp173:
						bScp173 = true;
						break;
				}

			while (!bRandomEnd)
			{
				RandomScpType randomScp = (RandomScpType)Random.Next((int)RandomScpType.RandomScpCount);
				switch (randomScp)
				{
					case RandomScpType.Scp049:
						if (!bScp049)
						{
							bRandomEnd = true;
							roleType = RoleType.Scp049;
						}
						break;

					case RandomScpType.Scp096:
						if (!bScp096)
						{
							bRandomEnd = true;
							roleType = RoleType.Scp096;
						}
						break;

					case RandomScpType.Scp106:
						if (!bScp106)
						{
							bRandomEnd = true;
							roleType = RoleType.Scp106;
						}
						break;

					case RandomScpType.Scp173:
						if (!bScp173)
						{
							bRandomEnd = true;
							roleType = RoleType.Scp173;
						}
						break;

					case RandomScpType.Scp939:
						if (new System.Random().Next(2) == 0)
							roleType = RoleType.Scp93953;
						else
							roleType = RoleType.Scp93989;
						break;
				}
			}

			return roleType;
		}

		public static void SetScpBadge(ServerRoles serverRoles, string badgeName, string badgeColor = "red")
		{
			if (string.IsNullOrEmpty(serverRoles.NetworkGlobalBadge))
			{
				serverRoles.NetworkMyText = badgeName;
				serverRoles.NetworkMyColor = badgeColor;
			}
		}

		public static void FlickerLights(float duration, int random = 5)
		{
			foreach (FlickerableLightController fc in Object.FindObjectsOfType<FlickerableLightController>())
				if (Random.Next(random) == 0)
					fc.ServerFlickerLights(duration);
		}

		

		public static void SetScp035(ReferenceHub hub)
		{
			IsScp035Hidden = false;
			Scp035id = hub.playerId;
			hub.playerStats.NetworkmaxArtificialHealth = 35;
			hub.hints.Show(new TextHint($"<voffset=27em><size=120><color=#FF0000><b>SCP-035</b></color></size>\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n<size=35><b></b></size>\n<size=35><b>你拥有极高的子弹抗性, 与其他<color=#FF0000>SCP</color>合作, 消灭所有人类</b></size></voffset>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 15.0f));
		}

		public static void SetScp181(ReferenceHub hub)
		{
			Scp181id = hub.playerId;
			SetScpBadge(hub.serverRoles, "SCP-181");
			hub.inventory.ServerDropAll();
			hub.inventory.AddNewItem(ItemType.Flashlight);
			hub.inventory.AddNewItem(ItemType.SCP268);
			hub.inventory.AddNewItem(ItemType.SCP500);
			hub.inventory.AddNewItem(ItemType.Medkit);
			hub.inventory.AddNewItem(ItemType.SCP207);
			hub.inventory.AddNewItem(ItemType.SCP207);
			hub.inventory.AddNewItem(ItemType.SCP207);
			hub.inventory.AddNewItem(ItemType.Coin);
			hub.hints.Show(new TextHint($"<voffset=27em><size=120><color=#FF0000><b>SCP-181</b></color></size>\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n<size=35><b>你有几率打开任何门</b></size>\n<size=35><b>背包里的每件物品都能为你抵挡一次来自<color=#FF0000>SCP</color>的伤害</b></size></voffset>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 15.0f));
		}

		public static void SetScp682(ReferenceHub hub)
		{
			Scp682id = hub.playerId;
			SetScpBadge(hub.serverRoles, "SCP-682");
			if (new System.Random().Next(2) == 0)
				hub.characterClassManager.SetPlayersClass(RoleType.Scp93953, hub.gameObject, CharacterClassManager.SpawnReason.ForceClass);
			else
				hub.characterClassManager.SetPlayersClass(RoleType.Scp93989, hub.gameObject, CharacterClassManager.SpawnReason.ForceClass);
			hub.hints.Show(new TextHint($"<voffset=27em><size=120><color=#FF0000><b>SCP-682</b></color></size>\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n<size=35><b>你拥有正常视野, 极高的伤害和生命恢复速度</b></size>\n<size=35><b>并且按住V键时可以将门或检查点直接摧毁</b></size></voffset>", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 15.0f));
			Timing.CallDelayed(0.1f, () => { hub.playerEffectsController.GetEffect<Visuals939>().ServerDisable(); });
			Timing.CallDelayed(1.0f, () => { hub.playerEffectsController.GetEffect<Visuals939>().ServerDisable(); });
			Timing.CallDelayed(2.0f, () => { hub.playerEffectsController.GetEffect<Visuals939>().ServerDisable(); });
		}

		public static ReferenceHub GetHub(int PlayerId)
		{
			foreach (GameObject gameObject in PlayerManager.players)
				if (ReferenceHub.GetHub(gameObject).playerId == PlayerId)
					return ReferenceHub.GetHub(gameObject);

			return null;
		}
	}
}
