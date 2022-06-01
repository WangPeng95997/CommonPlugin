using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(FallDamage), "OnTouchdown")]
    internal static class FallDamagePatch
    {
        private static bool Prefix(FallDamage __instance)
        {
            return GameCore.RoundStart.singleton.NetworkTimer == -1;
        }
    }
}