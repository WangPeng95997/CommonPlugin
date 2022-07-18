using PlayerStatsSystem;
using Smod2.API;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Ragdoll), nameof(Ragdoll.ServerSpawnRagdoll), typeof(ReferenceHub), typeof(DamageHandlerBase))]
    internal static class RagdollPatch
    {
        private static bool Prefix(Ragdoll __instance, ReferenceHub hub, DamageHandlerBase handler)
        {
            return GameCore.RoundStart.singleton.NetworkTimer == -1 && handler.smDamageType != DamageType.POCKET_DECAY;
        }
    }
}