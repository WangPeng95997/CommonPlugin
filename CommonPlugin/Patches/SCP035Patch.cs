using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using InventorySystem.Items.ThrowableProjectiles;
using PlayerStatsSystem;
using UnityEngine;
using HarmonyLib;
using NorthwoodLib.Pools;
using static HarmonyLib.AccessTools;

namespace CommonPlugin.Patches
{
    /*
    [HarmonyPatch(typeof(HitboxIdentity), "CheckFriendlyFire", typeof(ReferenceHub), typeof(ReferenceHub), typeof(bool))]
    internal static class CheckFriendlyFirePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLable = generator.DefineLabel();

            int index = 0;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Ldsfld, Field(typeof(EventHandlers), nameof(EventHandlers.Scp035id))),
                new(OpCodes.Brtrue_S, returnLable),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Ldsfld, Field(typeof(EventHandlers), nameof(EventHandlers.Scp035id))),
                new(OpCodes.Brtrue_S, returnLable),
            });

            newInstructions.AddRange(new CodeInstruction[]
            {
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ret),
            });

            index = newInstructions.Count - 2;

            newInstructions[index].WithLabels(returnLable);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
    */

    [HarmonyPatch(typeof(LocalCurrentRoomEffects), "FixedUpdate")]
    internal static class NightVisionPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label jccLable = generator.DefineLabel();

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call
            && (MethodInfo)i.operand == PropertySetter(typeof(LocalCurrentRoomEffects), nameof(LocalCurrentRoomEffects.NetworksyncFlicker)));

            newInstructions[newInstructions.Count - 2].WithLabels(jccLable);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Dup),
                new(OpCodes.Brfalse_S, jccLable),
                new(OpCodes.Pop),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(LocalCurrentRoomEffects), "_hub")),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Ldsfld, Field(typeof(EventHandlers), nameof(EventHandlers.Scp035id))),
                new(OpCodes.Ceq),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ceq),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    [HarmonyPatch(typeof(FlashbangGrenade), "ProcessPlayer", typeof(ReferenceHub))]
    internal static class ProcessPlayerPatch
    {
        private static bool Prefix(FlashbangGrenade __instance, ReferenceHub hub)
        {
            ReferenceHub owner = ReferenceHub.GetHub(__instance.gameObject);

            if (owner.playerId == EventHandlers.Scp035id)
            {
                switch(hub.characterClassManager.NetworkCurClass)
                {
                    case RoleType.Scp049:
                    case RoleType.Scp0492:
                    case RoleType.Scp079:
                    case RoleType.Scp096:
                    case RoleType.Scp106:
                    case RoleType.Scp173:
                    case RoleType.Scp93953:
                    case RoleType.Scp93989:
                        return false;
                }
            }

            return true;
        }
    }
}