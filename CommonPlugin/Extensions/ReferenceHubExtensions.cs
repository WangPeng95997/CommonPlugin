using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;
using PlayerStatsSystem;

namespace CommonPlugin.Extensions
{
    public static class ReferenceHubExtensions
    {
        public static AhpStat.AhpProcess GetAhpProcess(this ReferenceHub hub)
        {
            Type type = typeof(AhpStat);
            FieldInfo fieldInfo = type.GetField("_activeProcesses", BindingFlags.Instance | BindingFlags.NonPublic);

            return (fieldInfo.GetValue(hub.playerStats.StatModules[1] as AhpStat) as IEnumerable<AhpStat.AhpProcess>).FirstOrDefault();
        }

        public static HealthStat GetHealthStat(this ReferenceHub hub)
        {
            return hub.playerStats.StatModules[0] as HealthStat;
        }
    }
}