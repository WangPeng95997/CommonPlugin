﻿using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CustomPlayerEffects;
using HarmonyLib;
using NorthwoodLib.Pools;
using static HarmonyLib.AccessTools;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(FirstPersonController), "ModifyStamina", typeof(float))]
    internal static class ModifyStaminaPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.Clear();

            int index = 0;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(FirstPersonController), "staminaController")),
                new(OpCodes.Ldc_R4, 200.0f),
                new(OpCodes.Stfld, Field(typeof(Stamina ), nameof(Stamina.RemainingStamina))),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    [HarmonyPatch(typeof(FirstPersonController), "ResetStamina")]
    internal static class ResetStaminaPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.Clear();

            int index = 0;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(FirstPersonController), "staminaController")),
                new(OpCodes.Ldc_R4, 200.0f),
                new(OpCodes.Stfld, Field(typeof(Stamina ), nameof(Stamina.RemainingStamina))),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    [HarmonyPatch(typeof(FirstPersonController), "Update")]
    internal static class UpdatePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.Clear();

            int index = 0;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_R4, 200.0f),
                new(OpCodes.Call, PropertySetter(typeof(FirstPersonController),nameof(FirstPersonController.Network_syncStamina))),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}