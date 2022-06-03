using PlayerStatsSystem;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(HealthStat), "MaxValue", MethodType.Getter)]
    internal static class MaxValuePatch
    {
        private static float ClassD = EventHandlers.ClassdMaxHP;

        private static float Scientist = EventHandlers.ScientistMaxHP;

        private static float Ntf = EventHandlers.NtfMaxHP;

        private static float NtfCaptain = EventHandlers.NtfCaptainMaxHP;

        private static float Chaos = EventHandlers.ChaosMaxHP;

        private static float ChaosRepressor = EventHandlers.ChaosRepressorMaxHP;

        private static bool Prefix(HealthStat __instance, ref float __result)
        {
            RoleType roleType = __instance.Hub.characterClassManager.NetworkCurClass;

            switch (roleType)
            {
                case RoleType.ClassD:
                    __result = ClassD;
                    break;

                case RoleType.Scientist:
                    __result = Scientist;
                    break;

                case RoleType.FacilityGuard:
                case RoleType.NtfPrivate:
                case RoleType.NtfSergeant:
                case RoleType.NtfSpecialist:
                    __result = Ntf;
                    break;

                case RoleType.NtfCaptain:
                    __result = NtfCaptain;
                    break;

                case RoleType.ChaosConscript:
                case RoleType.ChaosRifleman:
                case RoleType.ChaosMarauder:
                    __result = Chaos;
                    break;

                case RoleType.ChaosRepressor:
                    __result = ChaosRepressor;
                    break;

                case RoleType.Scp079:

                    break;

                default:
                    __result = 100.0f;
                    break;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(HealthStat), "ServerHeal", typeof(float))]
    internal static class ServerHealPatch
    {
        private static bool Prefix(HealthStat __instance, float healAmount)
        {
            if (__instance.CurValue + healAmount > __instance.MaxValue)
                __instance.CurValue = __instance.MaxValue;
            else
                __instance.CurValue += healAmount;

            return false;
        }
    }
}