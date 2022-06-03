using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(DoorVariant), "ServerInteract", typeof(ReferenceHub), typeof(byte))]
    internal static class ServerInteractPatch
    {
        private static void Prefix(DoorVariant __instance, ReferenceHub ply, byte colliderId)
        {
            if (ply.playerId == EventHandlers.Scp682id && ply.radio.Network_syncAltVoicechatButton)
            {
                BreakableDoor breakableDoor = __instance.gameObject.GetComponent<BreakableDoor>();
                CheckpointDoor checkpointDoor = __instance.gameObject.GetComponent<CheckpointDoor>();

                if (breakableDoor != null)
                    breakableDoor.Network_destroyed = true;
                if (checkpointDoor != null)
                    checkpointDoor.ServerDamage(65535.0f, DoorDamageType.ServerCommand);
            }
        }
    }
}