using UnityEngine;
using Mirror;
using PlayableScps;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Scp096), "MaxShield", MethodType.Getter)]
    internal static class MaxShieldPatch
    {
        private static void Postfix(Scp096 __instance, ref float __result)
        {
            __result = EventHandlers.Scp096id == 0 ? EventHandlers.Scp096Shield : EventHandlers.Scp096MaxShield;
        }
    }

    [HarmonyPatch(typeof(Scp096), "ResetShield")]
    internal static class ResetShieldPatch
    {
        private static bool Prefix(Scp096 __instance)
        {
            __instance.Hub.serverRoles.Network_myColor
            __instance.CurMaxShield

            return false;
        }
    }
}