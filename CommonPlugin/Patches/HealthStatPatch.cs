using PlayerStatsSystem;
using HarmonyLib;
using CommonPlugin.Components;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(HealthStat), nameof(HealthStat.MaxValue), MethodType.Getter)]
    internal static class MaxValuePatch
    {
        const float ClassD = EventHandlers.ClassdMaxHP;
        const float Scientist = EventHandlers.ScientistMaxHP;
        const float Mtf = EventHandlers.MtfMaxHP;
        const float MtfCaptain = EventHandlers.MtfCaptainMaxHP;
        const float Chaos = EventHandlers.ChaosMaxHP;
        const float ChaosMarauder = EventHandlers.ChaosMarauderMaxHP;
        const float Scp049 = EventHandlers.Scp049MaxHP;
        const float Scp0492 = EventHandlers.Scp0492MaxHP;
        const float Scp079 = EventHandlers.Scp079MaxHP;
        const float Scp096 = EventHandlers.Scp096MaxHP;
        const float Scp106 = EventHandlers.Scp106MaxHP;
        const float Scp173 = EventHandlers.Scp173MaxHP;
        const float Scp682 = EventHandlers.Scp682MaxHP;
        const float Scp939 = EventHandlers.Scp939MaxHP;

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
                    __result = Mtf;
                    break;

                case RoleType.NtfCaptain:
                    __result = MtfCaptain;
                    break;

                case RoleType.ChaosConscript:
                case RoleType.ChaosRifleman:
                case RoleType.ChaosRepressor:
                    __result = Chaos;
                    break;

                case RoleType.ChaosMarauder:
                    __result = ChaosMarauder;
                    break;

                case RoleType.Scp049:
                    __result = Scp049;
                    break;

                case RoleType.Scp0492:
                    __result = Scp0492;
                    break;

                case RoleType.Scp079:
                    __result = Scp079;
                    break;

                case RoleType.Scp096:
                    __result = Scp096;
                    break;

                case RoleType.Scp106:
                    __result = Scp106;
                    break;

                case RoleType.Scp173:
                    __result = Scp173;
                    break;

                case RoleType.Scp93953:
                case RoleType.Scp93989:
                    __result = __instance.Hub.playerId == EventHandlers.Scp682id ? Scp682 : Scp939;
                    break;

                default:
                    __result = 100.0f;
                    break;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(HealthStat), nameof(HealthStat.ServerHeal), typeof(float))]
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