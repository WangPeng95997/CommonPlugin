using PlayerStatsSystem;
using HarmonyLib;
using CommonPlugin.Components;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(HealthStat), "MaxValue", MethodType.Getter)]
    internal static class MaxValuePatch
    {
        private const float ClassD = EventHandlers.ClassdMaxHP;

        private const float Scientist = EventHandlers.ScientistMaxHP;

        private const float Ntf = EventHandlers.MtfMaxHP;

        private const float NtfCaptain = EventHandlers.MtfCaptainMaxHP;

        private const float Chaos = EventHandlers.ChaosMaxHP;

        private const float ChaosRepressor = EventHandlers.ChaosRepressorMaxHP;

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
            HealthController healthControler = __instance.Hub.gameObject.GetComponent<HealthController>();

            if (healthControler.Health + healAmount > healthControler.MaxHealth)
                healthControler.Health = healthControler.MaxHealth;
            else
                healthControler.Health += healAmount;

            return false;
        }
    }
}