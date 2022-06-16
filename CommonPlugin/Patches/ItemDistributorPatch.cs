using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Pickups;
using MapGeneration.Distributors;
using UnityEngine;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(ItemDistributor), "SpawnPickup", typeof(ItemPickupBase))]
    internal static class SpawnPickupPatch
    {
        private static void Prefix(ItemDistributor __instance, ref ItemPickupBase ipb)
        {
            switch (ipb.NetworkInfo.ItemId)
            {
                // WeaponType
                case ItemType.GunCOM15:
                case ItemType.GunE11SR:
                case ItemType.GunCrossvec:
                case ItemType.GunFSP9:
                case ItemType.GunLogicer:
                case ItemType.GunCOM18:
                case ItemType.GunRevolver:
                case ItemType.GunAK:
                case ItemType.GunShotgun:
                case ItemType.ParticleDisruptor:
                    ItemBase itemBase;
                    InventoryItemLoader.AvailableItems.TryGetValue(ipb.NetworkInfo.ItemId, out itemBase);
                    ItemPickupBase itemPickupBase = Object.Instantiate(itemBase.PickupDropModel, ipb.transform.position, ipb.transform.rotation);
                    FirearmPickup firearmPickup = itemPickupBase as FirearmPickup;

                    (ipb as FirearmPickup).NetworkStatus = new FirearmStatus(0, firearmPickup.NetworkStatus.Flags, firearmPickup.NetworkStatus.Attachments);
                    break;
            }
        }
    }
}