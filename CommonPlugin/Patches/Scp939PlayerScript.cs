using System;
using HarmonyLib;
using UnityEngine;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(global::Scp939PlayerScript), "CallCmdShoot", new Type[] { typeof(GameObject) })]
    internal static class Scp939PlayerScript
    {
        private static bool Prefix(GameObject target)
        {
            return ReferenceHub.GetHub(target).playerId != EventHandlers.Scp035id;
        }
    }
}