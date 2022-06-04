using UnityEngine;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Scp049_2PlayerScript), "UserCode_CmdHurtPlayer", typeof(GameObject))]
    internal static class Scp049PlayerScript
    {
        private static bool Prefix(GameObject plyObj)
        {
            return ReferenceHub.GetHub(plyObj).playerId != EventHandlers.Scp035id;
        }
    }
}