using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InventorySystem;
using PlayerStatsSystem;
using UnityEngine;
using Smod2.API;

using CommonPlugin.Components;

namespace CommonPlugin.Extensions
{
    public static class MethodExtensions
    {
        private static FieldInfo fieldInfo = typeof(AhpStat).GetField("_activeProcesses", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void RemoveAllItems(this Inventory inventory)
        {
            while (inventory.UserInventory.Items.Count > 0)
            {
                inventory.ServerRemoveItem(inventory.UserInventory.Items.ElementAt(0).Key, null);
            }
            inventory.UserInventory.ReserveAmmo.Clear();
            inventory.SendAmmoNextFrame = true;
        }

        public static GameObject GameObject(this Player player) => player.GetGameObject() as GameObject;

        public static ReferenceHub GetHub(this Player player) => ReferenceHub.GetHub(player.GetGameObject() as GameObject);

        public static AhpStat.AhpProcess GetAhpProcess(this ReferenceHub hub)
        {
            AhpStat.AhpProcess ahpProcess = (fieldInfo.GetValue(hub.playerStats.StatModules[1] as AhpStat) as IEnumerable<AhpStat.AhpProcess>).FirstOrDefault();

            return ahpProcess is not null ? ahpProcess : (hub.playerStats.StatModules[1] as AhpStat).ServerAddProcess(0.0f, 75.0f, 0.75f, 0.7f, 0, true);
        }

        public static HealthController GetHealthControler(this ReferenceHub hub) => hub.gameObject.GetComponent<HealthController>();

        public static HealthStat GetHealthStat(this ReferenceHub hub) => hub.playerStats.StatModules[0] as HealthStat;

        public static void TeleportAnimation(this Scp106PlayerScript scp106PlayerScript)
        {
            MethodInfo methodInfo = typeof(global::Scp106PlayerScript).GetMethod("RpcTeleportAnimation", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(scp106PlayerScript, null);
        }
    }
}