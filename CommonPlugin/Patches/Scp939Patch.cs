using UnityEngine;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    /*
    [HarmonyPatch(typeof(GameObject), "CallCmdShoot", typeof(GameObject))]
    internal static class CallCmdShootPatch
    {
        private static bool Prefix(GameObject target)
        {
            return ReferenceHub.GetHub(target).playerId != EventHandlers.Scp035id;
        }
    }
    */
}