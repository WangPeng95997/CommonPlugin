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
    [HarmonyPatch(typeof(DoorVariant), "ServerInteract", typeof(ReferenceHub), typeof(byte))]
    internal static class ServerInteractPatch
    {
        private static System.Random Random = new System.Random();

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label Scp181Label = generator.DefineLabel();

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldc_I4_7) - 2;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Pop),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(ServerInteractPatch), nameof(ServerInteractPatch.AllowInteract))),
                new(OpCodes.Brfalse_S, Scp181Label),
                new(OpCodes.Ldarg_1),
            });

            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == PropertySetter(typeof(DoorVariant), nameof(DoorVariant.NetworkTargetState))) - 5;
            newInstructions[index].WithLabels(Scp181Label);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool AllowInteract(DoorVariant __instance, ReferenceHub hub)
        {
            if (hub.playerId == EventHandlers.Scp181id && Random.NextDouble() < 0.0777)
                return false;
            else if (hub.playerId == EventHandlers.Scp682id && hub.radio.Network_syncAltVoicechatButton)
            {
                BreakableDoor breakableDoor = __instance.gameObject.GetComponent<BreakableDoor>();
                CheckpointDoor checkpointDoor = __instance.gameObject.GetComponent<CheckpointDoor>();

                if (breakableDoor != null)
                    breakableDoor.Network_destroyed = true;
                if (checkpointDoor != null)
                    checkpointDoor.ServerDamage(65535.0f, DoorDamageType.ServerCommand);
            }

            return true;
        }
    }
}