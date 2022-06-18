using Hints;
using InventorySystem;
using Smod2;
using HarmonyLib;
using CommonPlugin.Extensions;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Inventory), "UserCode_CmdProcessHotkey", typeof(ActionName), typeof(ushort))]
    internal static class CmdProcessHotkeyPatch
    {
		private static Plugin Plugin = PluginManager.Manager.Plugins[0];

		private const int Scp106Cooldown = EventHandlers.Scp106Cooldown;

		private static bool Prefix(Inventory __instance, ActionName hotkeyButtonPressed, ushort clientsideDesiredItem)
        {
			ReferenceHub hub = ReferenceHub.GetHub(__instance.gameObject);

			if (hub.scp106PlayerScript.goingViaThePortal)
				return false;

			if (hotkeyButtonPressed == ActionName.HotkeyKeycard && hub.characterClassManager.NetworkCurClass == RoleType.Scp106 && !BlastDoor.OneDoor.isClosed)
            {
				int duration = Plugin.Server.Round.Duration - EventHandlers.Scp106LastPlace;

				if (duration > Scp106Cooldown)
				{
					PluginEx.PlaceTrapItem(hub.transform.position);

					EventHandlers.Scp106LastPlace = Plugin.Server.Round.Duration;
					hub.hints.Show(
						new TextHint("<size=30><b><color=#FF0000>诱捕陷阱</color>放置成功!</b></size>",
						new HintParameter[] { new StringHintParameter("") }, new HintEffect[] { HintEffectPresets.FadeOut() }, 5.0f));
				}
				else
					hub.hints.Show(
						new TextHint($"<size=30><b>还需要<color=#FF0000>{Scp106Cooldown - duration}</color>秒才能放置诱捕陷阱</b></size>",
						new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));

				return false;
			}

			return true;
        }
    }
}