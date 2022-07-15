using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using HarmonyLib;
using NorthwoodLib.Pools;
using static HarmonyLib.AccessTools;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), typeof(ReferenceHub), typeof(byte))]
    internal static class ServerInteractPatch
    {
        private static readonly System.Random Random = new();

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label interactLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_0);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // CheckDestroyDoor(__instance, ply);
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(ServerInteractPatch), nameof(ServerInteractPatch.CheckDestroyDoor))),
            });

            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldc_I4_7) - 3;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // if (AllowInteract(__instance, ply));
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(ServerInteractPatch), nameof(ServerInteractPatch.AllowInteract))),
                new(OpCodes.Brtrue_S, interactLabel),
            });

            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call
            && (MethodInfo)i.operand == PropertySetter(typeof(DoorVariant), nameof(DoorVariant.NetworkTargetState))) - 5;

            newInstructions[index].WithLabels(interactLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void CheckDestroyDoor(DoorVariant __instance, ReferenceHub hub)
        {
            if (hub.playerId == EventHandlers.Scp682id && hub.radio.Network_syncAltVoicechatButton)
            {
                BreakableDoor breakableDoor = __instance.gameObject.GetComponent<BreakableDoor>();
                CheckpointDoor checkpointDoor = __instance.gameObject.GetComponent<CheckpointDoor>();

                if (breakableDoor != null)
                    breakableDoor.Network_destroyed = true;
                if (checkpointDoor != null)
                    checkpointDoor.ServerDamage(65535.0f, DoorDamageType.ServerCommand);
            }
        }

        private static bool AllowInteract(DoorVariant __instance, ReferenceHub hub)
        {
            DoorNametagExtension doorNametagExtension;
            if (!__instance.gameObject.TryGetComponent(out doorNametagExtension))
                return false;

            string doorName = doorNametagExtension.GetName;
            if (doorName == "HID" || doorName == "INTERCOM")
            {
                switch (hub.inventory.CurInstance.ItemTypeId)
                {
                    case ItemType.KeycardNTFOfficer:
                    case ItemType.KeycardNTFLieutenant:
                        return true;
                }
            }

            if (hub.playerId == EventHandlers.Scp181id && Random.NextDouble() < 0.0777)
                return true;

            return false;
        }
    }
}