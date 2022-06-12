using Mirror;
using UnityEngine;
using Smod2;
using Smod2.API;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(CharacterClassManager), "IsAnyScp")]
    internal static class IsAnyScpPatch
    {
        private static void Postfix(CharacterClassManager __instance, ref bool __result)
        {
            if (!__result)
                __result = ReferenceHub.GetHub(__instance.gameObject).playerId != EventHandlers.Scp035id;
        }
    }

    [HarmonyPatch(typeof(CharacterClassManager), "IsTargetForSCPs")]
    internal static class IsTargetForSCPsPatch
    {
        private static void Postfix(CharacterClassManager __instance, ref bool __result)
        {
            if (__result)
                __result = ReferenceHub.GetHub(__instance.gameObject).playerId != EventHandlers.Scp035id;
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

    [HarmonyPatch(typeof(CharacterClassManager), "TargetRawDeathScreen", typeof(NetworkConnection))]
    internal static class TargetRawDeathScreenPatch
    {
        private static Round Round = PluginManager.Manager.Server.Round;

        private const int lateJoinTime = EventHandlers.lateJoinTime;

        private static bool Prefix(CharacterClassManager __instance, NetworkConnection conn)
        {
            bool bSpawnClass = Round.Duration < lateJoinTime;
            if (bSpawnClass)
                __instance.SetClassIDAdv(RoleType.ClassD, false, CharacterClassManager.SpawnReason.LateJoin);

            return !bSpawnClass;
        }
    }

    [HarmonyPatch(typeof(CharacterClassManager), "UserCode_CmdRequestHideTag")]
    internal static class CmdRequestHideTagPatch
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
}