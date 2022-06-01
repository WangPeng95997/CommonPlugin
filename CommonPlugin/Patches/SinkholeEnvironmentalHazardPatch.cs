using UnityEngine;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(SinkholeEnvironmentalHazard), "DistanceChanged", typeof(GameObject))]
    internal static class SinkholeEnvironmentalHazardPatch
    {
        private static bool Prefix(GameObject player)
        {
            return ReferenceHub.GetHub(player).playerId != EventHandlers.Scp035id;
        }
    }
}