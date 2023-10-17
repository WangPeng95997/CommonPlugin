using System.Collections.Generic;
using System.Reflection.Emit;
using CustomPlayerEffects;
using HarmonyLib;
using NorthwoodLib.Pools;
using static HarmonyLib.AccessTools;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Scp939_VisionController), nameof(Scp939_VisionController.CanSee))]
    internal static class CanSeePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueLabel = generator.DefineLabel();

            int index = 0;

            newInstructions[index].WithLabels(continueLabel);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(PlayerEffect), nameof(PlayerEffect.Hub))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Call, PropertyGetter(typeof(EventHandlers), nameof(EventHandlers.Scp682id))),
                new(OpCodes.Bne_Un_S, continueLabel),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}