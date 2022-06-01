using UnityEngine;
using Smod2;
using Smod2.API;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(CharacterClassManager), "UserCode_CmdRequestHideTag")]
    internal static class CallCmdRequestHideTagPatch
    {
        private static bool Prefix(CharacterClassManager __instance)
        {
            ReferenceHub hub = ReferenceHub.GetHub(__instance.gameObject);
            bool bGlobalBadge = string.IsNullOrEmpty(hub.serverRoles.NetworkGlobalBadge);
            if (bGlobalBadge)
                hub.queryProcessor.GCT.SendToClient(__instance.connectionToClient, "你没有该权限!", "red");

            return !bGlobalBadge;
        }
    }

    [HarmonyPatch(typeof(CharacterClassManager), "TargetRawDeathScreen")]
    internal static class TargetRawDeathScreenPatch
    {
        private static Round Round = PluginManager.Manager.Server.Round;

        private static bool Prefix(CharacterClassManager __instance)
        {
            bool bSpawnClass = Round.Duration < 90;
            if (bSpawnClass)
                __instance.SetPlayersClass(RoleType.ClassD, __instance.gameObject, CharacterClassManager.SpawnReason.LateJoin);

            return !bSpawnClass;
        }
    }

    [HarmonyPatch(typeof(CharacterClassManager), "RpcPlaceBlood", typeof(Vector3), typeof(int), typeof(float))]
    internal static class PlaceBloodPatch
    {
        private static bool Prefix(CharacterClassManager __instance, Vector3 pos, int type, float f)
        {
            return false;
        }
    }
}