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

		private const int Cooldown = EventHandlers.Scp106Cooldown;

		private static bool Prefix(InventorySystem.Inventory __instance, ActionName hotkeyButtonPressed, ushort clientsideDesiredItem)
        {
			ReferenceHub hub = ReferenceHub.GetHub(__instance.gameObject);

			if (hotkeyButtonPressed == ActionName.HotkeyKeycard && hub.characterClassManager.NetworkCurClass == RoleType.Scp106 && !Plugin.Server.Map.WarheadDetonated)
            {
				int duration = Plugin.Server.Round.Duration - EventHandlers.Scp106LastPlace;

				if (duration > Cooldown)
				{
					PluginEx.PlaceTrapItem(hub.transform.position);

					EventHandlers.Scp106LastPlace = Plugin.Server.Round.Duration;
					hub.hints.Show(
						new TextHint("<b><color=#FF0000>诱捕陷阱</color>放置成功!</b>",
						new HintParameter[] { new StringHintParameter("") }, new HintEffect[] { HintEffectPresets.FadeOut() }, 5.0f));
				}
				else
					hub.hints.Show(
						new TextHint($"<b>还需要<color=#FF0000>{Cooldown - duration}</color>秒才能放置诱捕陷阱</b>",
						new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));

				return false;
			}

			return true;
        }
    }
}