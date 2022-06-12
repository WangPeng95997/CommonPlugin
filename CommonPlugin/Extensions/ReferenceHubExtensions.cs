using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PlayerStatsSystem;
using CommonPlugin.Components;

namespace CommonPlugin.Extensions
{
    public static class ReferenceHubExtensions
    {
        private static FieldInfo fieldInfo = typeof(AhpStat).GetField("_activeProcesses", BindingFlags.Instance | BindingFlags.NonPublic);

        public static AhpStat.AhpProcess GetAhpProcess(this ReferenceHub hub)
        {
            AhpStat.AhpProcess ahpProcess = (fieldInfo.GetValue(hub.playerStats.StatModules[1] as AhpStat) as IEnumerable<AhpStat.AhpProcess>).FirstOrDefault();

            return ahpProcess is not null ? ahpProcess : (hub.playerStats.StatModules[1] as AhpStat).ServerAddProcess(0.0f, 75.0f, 0.75f, 0.7f, 0, true);
        }

        public static HealthController GetHealthControler(this ReferenceHub hub) => hub.gameObject.GetComponent<HealthController>();

        public static HealthStat GetHealthStat(this ReferenceHub hub) => hub.playerStats.StatModules[0] as HealthStat;
    }
}