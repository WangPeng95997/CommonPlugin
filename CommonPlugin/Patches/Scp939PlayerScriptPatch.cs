using System;
using HarmonyLib;
using UnityEngine;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Scp939PlayerScript), "CallCmdShoot", new Type[] { typeof(GameObject) })]
    internal static class Scp939PlayerScriptPatch
    {
        private static bool Prefix(GameObject target)
        {
            return ReferenceHub.GetHub(target).playerId != EventHandlers.Scp035id;
        }
    }
}