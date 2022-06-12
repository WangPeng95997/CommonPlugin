using System.Collections.Generic;
using System.Reflection.Emit;
using InventorySystem.Configs;
using InventorySystem.Items.Armor;
using HarmonyLib;
using NorthwoodLib.Pools;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(InventoryLimits), "GetAmmoLimit", typeof(ItemType), typeof(ReferenceHub))]
    internal static class GetAmmoLimitPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.Clear();

            int index = 0;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldc_I4, 300),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    [HarmonyPatch(typeof(InventoryLimits), "GetAmmoLimit", typeof(BodyArmor), typeof(ItemType))]
    internal static class GetAmmoLimitPatch2
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.Clear();

            int index = 0;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldc_I4, 300),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}