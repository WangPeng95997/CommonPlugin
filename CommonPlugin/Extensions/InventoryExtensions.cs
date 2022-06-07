using System.Linq;
using InventorySystem;

namespace CommonPlugin.Extensions
{
    public static class InventoryExtensions
    {
        public static void RemoveAllItems(this Inventory inventory)
        {
            while (inventory.UserInventory.Items.Count > 0)
            {
                inventory.ServerRemoveItem(inventory.UserInventory.Items.ElementAt(0).Key, null);
            }
            inventory.UserInventory.ReserveAmmo.Clear();
            inventory.SendAmmoNextFrame = true;
        }
    }
}