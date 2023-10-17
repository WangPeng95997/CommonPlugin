using System.Linq;
using System.Text;
using Hints;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.MicroHID;
using MEC;
using Mirror;
using PlayerStatsSystem;
using Respawning;
using UnityEngine;

namespace CommonPlugin.Extensions
{
    public class PluginEx
    {
        private static System.Random Random = new System.Random();

        public static void CassieMessage(string message, string translation)
        {
            StringBuilder annoucement = new StringBuilder();

            string[] cassies = message.Split('\n');
            string[] translations = translation.Split('\n');
            for (int i = 0; i < cassies.Count(); i++)
                annoucement.Append($"{translations[i].Replace(' ', ' ')}<alpha=#00> {cassies[i]} </alpha><split>");

            RespawnEffectsController.PlayCassieAnnouncement(annoucement.ToString(), false, true, true);
        }

        public static void ClearServerBadge(ServerRoles serverRoles, string badgeColor = "default")
        {
            if (string.IsNullOrEmpty(serverRoles.NetworkGlobalBadge))
            {
                serverRoles.Network_myText = null;
                serverRoles.Network_myColor = badgeColor;
                serverRoles.RpcResetFixed();
            }
        }

        public static void FlickerLights(int minValue, int maxValue)
        {
            foreach (FlickerableLightController fc in Object.FindObjectsOfType<FlickerableLightController>())
            {
                if (Random.Next(5) == 0)
                    fc.ServerFlickerLights(Random.Next(minValue, maxValue));
            }
        }

        public static RoleType GetRandomScp()
        {
            bool bRandomEnd = false, bScp049 = false, bScp096 = false, bScp106 = false, bScp173 = false;
            RoleType roleType = RoleType.None;

            foreach (GameObject gameObject in PlayerManager.players)
            {
                switch (ReferenceHub.GetHub(gameObject).characterClassManager.NetworkCurClass)
                {
                    case RoleType.Scp049:
                        bScp049 = true;
                        break;

                    case RoleType.Scp096:
                        bScp096 = true;
                        break;

                    case RoleType.Scp106:
                        bScp106 = true;
                        break;

                    case RoleType.Scp173:
                        bScp173 = true;
                        break;
                }
            }

            while (!bRandomEnd)
            {
                RandomScp randomScp = (RandomScp)Random.Next((int)RandomScp.RandomScpCount);
                switch (randomScp)
                {
                    case RandomScp.Scp049:
                        if (!bScp049)
                        {
                            bRandomEnd = true;
                            roleType = RoleType.Scp049;
                        }
                        break;

                    case RandomScp.Scp096:
                        if (!bScp096)
                        {
                            bRandomEnd = true;
                            roleType = RoleType.Scp096;
                        }
                        break;

                    case RandomScp.Scp106:
                        if (!bScp106)
                        {
                            bRandomEnd = true;
                            roleType = RoleType.Scp106;
                        }
                        break;

                    case RandomScp.Scp173:
                        if (!bScp173)
                        {
                            bRandomEnd = true;
                            roleType = RoleType.Scp173;
                        }
                        break;

                    case RandomScp.Scp939:
                        bRandomEnd = true;

                        if (Random.Next(2) == 0)
                            roleType = RoleType.Scp93953;
                        else
                            roleType = RoleType.Scp93989;
                        break;
                }
            }

            return roleType;
        }

        public static ReferenceHub GetHub(int playerId)
        {
            foreach (GameObject gameObject in PlayerManager.players)
                if (ReferenceHub.GetHub(gameObject).playerId == playerId)
                    return ReferenceHub.GetHub(gameObject);

            return null;
        }

        public static void PlaceTrapItem(Vector3 position)
        {
            TrapItem trapItem = (TrapItem)Random.Next((int)TrapItem.TrapItemCount);
            ItemType itemType = ItemType.None;

            switch (trapItem)
            {
                case TrapItem.Adrenaline:
                    itemType = ItemType.Adrenaline;
                    break;

                case TrapItem.GrenadeFlash:
                    itemType = ItemType.GrenadeFlash;
                    break;

                case TrapItem.GrenadeFrag:
                    itemType = ItemType.GrenadeHE;
                    break;

                case TrapItem.KeycardContainmentEngineer:
                    itemType = ItemType.KeycardContainmentEngineer;
                    break;

                case TrapItem.KeycardFacilityManager:
                    itemType = ItemType.KeycardFacilityManager;
                    break;

                case TrapItem.KeycardO5:
                    itemType = ItemType.KeycardO5;
                    break;

                case TrapItem.Medkit:
                    itemType = ItemType.Medkit;
                    break;

                case TrapItem.SCP207:
                    itemType = ItemType.SCP207;
                    break;

                case TrapItem.SCP268:
                    itemType = ItemType.SCP268;
                    break;

                case TrapItem.SCP500:
                    itemType = ItemType.SCP500;
                    break;

                case TrapItem.SCP1853:
                    itemType = ItemType.SCP1853;
                    break;
            }

            ItemPickupBase itemPickupBase = SpawnItem(itemType, new Vector3(position.x, position.y, position.z) + Vector3.up, Quaternion.Euler(Vector3.zero));
            EventHandlers.TrapItems.Add(itemPickupBase.NetworkInfo.Serial);
        }

        public static void SetScp035(ReferenceHub hub)
        {
            EventHandlers.Scp035id = hub.playerId;
            EventHandlers.bScp035Detected = false;

            hub.hints.Show(
                new TextHint($"<voffset=10em><size=220><color=#FF0000><b>SCP-035</b></color></size></voffset>\n\n\n\n" +
                $"<size=30><b>你拥有极高的子弹抗性, 与其他<color=#FF0000>SCP</color>合作, 消灭所有人类</b></size>",
                new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 15.0f));
        }

        public static void SetScp181(ReferenceHub hub)
        {
            EventHandlers.Scp181id = hub.playerId;

            hub.inventory.RemoveAllItems();
            hub.inventory.ServerAddItem(ItemType.Flashlight);
            hub.inventory.ServerAddItem(ItemType.SCP500);
            hub.inventory.ServerAddItem(ItemType.SCP268);
            hub.inventory.ServerAddItem(ItemType.SCP207);
            hub.inventory.ServerAddItem(ItemType.SCP207);
            hub.inventory.ServerAddItem(ItemType.SCP207);
            hub.inventory.ServerAddItem(ItemType.SCP207);
            hub.inventory.ServerAddItem(ItemType.Coin);

            SetServerBadge(hub.serverRoles, "SCP-181");
            hub.hints.Show(
                new TextHint($"<voffset=10em><size=220><color=#FF0000><b>SCP-181</b></color></size></voffset>\n\n\n\n" +
                $"<size=30><b>你有几率打开任何门, 背包里的每件物品都能为你抵挡一次来自<color=#FF0000>SCP</color>的伤害</b></size>",
                new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 15.0f));
        }

        public static void SetScp682(ReferenceHub hub)
        {
            EventHandlers.Scp682id = hub.playerId;
            hub.characterClassManager.SetClassIDAdv(Random.Next(2) == 0 ? RoleType.Scp93953 : RoleType.Scp93989, false, CharacterClassManager.SpawnReason.ForceClass);

            SetServerBadge(hub.serverRoles, "SCP-682");
            hub.hints.Show(
                new TextHint($"<voffset=10em><size=220><color=#FF0000><b>SCP-682</b></color></size></voffset>\n\n\n" +
                $"<size=30><b>你拥有正常视野, 极高的伤害, 生命值和生命恢复速度\n" +
                $"按住V键时可以直接摧毁门或检查点</b></size>",
                new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 15.0f));

            Timing.CallDelayed(0.25f, () => {
                AhpStat.AhpProcess ahpProcess = hub.GetAhpProcess();
                ahpProcess.Limit = 0;
                ahpProcess.CurrentAmount = 0;
                hub.playerEffectsController.GetEffect<CustomPlayerEffects.Visuals939>().Intensity = 0;
            });
        }

        public static void SetServerBadge(ServerRoles serverRoles, string badgeName, string badgeColor = "red")
        {
            if (string.IsNullOrEmpty(serverRoles.NetworkGlobalBadge))
            {
                serverRoles.Network_myText = badgeName;
                serverRoles.Network_myColor = badgeColor;
            }
        }

        public static ItemPickupBase SpawnItem(ItemType itemType, Vector3 position, Quaternion rotation, int ammo = 0)
        {
            ItemBase itemBase;
            InventoryItemLoader.AvailableItems.TryGetValue(itemType, out itemBase);

            ItemPickupBase itemPickupBase = Object.Instantiate(itemBase.PickupDropModel, position, rotation);

            switch (itemType)
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
                    FirearmPickup firearmPickup = itemPickupBase as FirearmPickup;
                    firearmPickup.NetworkStatus = new FirearmStatus((byte)ammo, firearmPickup.NetworkStatus.Flags, firearmPickup.NetworkStatus.Attachments);
                    break;

                case ItemType.MicroHID:
                    (itemPickupBase as MicroHIDPickup).NetworkEnergy = ammo;
                    break;

                // AmmoType
                case ItemType.Ammo12gauge:
                case ItemType.Ammo556x45:
                case ItemType.Ammo44cal:
                case ItemType.Ammo762x39:
                case ItemType.Ammo9x19:
                    (itemPickupBase as AmmoPickup).NetworkSavedAmmo = (ushort)ammo;
                    break;
            }

            NetworkServer.Spawn(itemPickupBase.gameObject);

            itemPickupBase.NetworkInfo = new PickupSyncInfo
            {
                ItemId = itemType,
                Serial = ItemSerialGenerator.GenerateNext(),
                Weight = itemBase.Weight,
                Position = position,
                Rotation = new LowPrecisionQuaternion(rotation)
            };

            itemPickupBase.gameObject.GetComponent<Rigidbody>().mass = Mathf.Max(0.3f, itemBase.Weight);

            return itemPickupBase;
        }
    }
}