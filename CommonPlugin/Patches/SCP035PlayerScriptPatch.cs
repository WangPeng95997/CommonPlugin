using UnityEngine;
using InventorySystem.Items.ThrowableProjectiles;
using PlayerStatsSystem;
using CommonPlugin.Extensions;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(HitboxIdentity), "CheckFriendlyFire", typeof(ReferenceHub), typeof(ReferenceHub), typeof(bool))]
    internal static class CheckFriendlyFirePatch
    {
        private static void Postfix(ref bool __result, HitboxIdentity __instance, ReferenceHub attacker, ReferenceHub victim, bool ignoreConfig)
        {
            if (!__result) {
                if (victim.playerId == EventHandlers.Scp035id) {
                    __result = true;
                    return;
                }

                if (attacker.playerId == EventHandlers.Scp035id)
                    __result = true;
            } 
        }
    }

    [HarmonyPatch(typeof(LocalCurrentRoomEffects), "IsInDarkenedRoom", typeof(Vector3))]
    internal static class IsInDarkenedRoomPatch
    {
        private static bool Prefix(LocalCurrentRoomEffects __instance, Vector3 PositionToCheck)
        {
            return ReferenceHub.GetHub(__instance.gameObject).playerId != EventHandlers.Scp035id;
        }
    }

    [HarmonyPatch(typeof(FlashbangGrenade), "ProcessPlayer", typeof(ReferenceHub))]
    internal static class ProcessPlayerPatch
    {
        private static bool Prefix(FlashbangGrenade __instance, ReferenceHub hub)
        {
            AhpStat.AhpProcess ahpProcess = ReferenceHub.GetHub(__instance.gameObject).GetAhpProcess();

            if (ahpProcess is not null && ahpProcess.Limit == 35.0f)
            {
                switch(hub.characterClassManager.NetworkCurClass)
                {
                    case RoleType.Scp049:
                    case RoleType.Scp0492:
                    case RoleType.Scp079:
                    case RoleType.Scp096:
                    case RoleType.Scp106:
                    case RoleType.Scp173:
                    case RoleType.Scp93953:
                    case RoleType.Scp93989:
                        return false;
                }
            }

            return true;
        }
    }
}