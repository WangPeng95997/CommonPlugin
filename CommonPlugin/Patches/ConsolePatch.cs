using System.Collections.Generic;
using System.Reflection.Emit;
using GameCore;
using HarmonyLib;
using NorthwoodLib.Pools;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Console), "AddDebugLog")]
    internal static class AddDebugLogPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.Clear();

            int index = 0;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}