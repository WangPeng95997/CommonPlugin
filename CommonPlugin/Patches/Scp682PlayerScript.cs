using System.Collections.Generic;
using System.Reflection.Emit;

using CustomPlayerEffects;

using HarmonyLib;
using NorthwoodLib.Pools;

using static HarmonyLib.AccessTools;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Scp939_VisionController), "FixedUpdate")]
    internal static class FixedUpdatePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label addVision = generator.DefineLabel();

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Beq_S) + 1;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_1),
                new(OpCodes.Ldfld, Field(typeof(PlayerEffect), nameof(PlayerEffect.Hub))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Ldsfld, Field(typeof(EventHandlers), nameof(EventHandlers.Scp682id))),
                new(OpCodes.Beq_S, addVision),
            });

            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Bge_Un_S) + 1;

            newInstructions[index].WithLabels(addVision);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}