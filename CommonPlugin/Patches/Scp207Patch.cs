using System.Collections.Generic;
using System.Reflection.Emit;
using CustomPlayerEffects;
using HarmonyLib;
using NorthwoodLib.Pools;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Scp207), "OnUpdate")]
    internal static class OnUpdatePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.Clear();
            newInstructions.Add(new(OpCodes.Ret));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    [HarmonyPatch(typeof(Scp207), nameof(Scp207.IsHealable), typeof(ItemType))]
    internal static class IsHealablePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = 0;

            newInstructions.Clear();
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}