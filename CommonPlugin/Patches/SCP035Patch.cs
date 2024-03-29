﻿using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Footprinting;
using InventorySystem.Items.ThrowableProjectiles;
using PlayerStatsSystem;
using HarmonyLib;
using NorthwoodLib.Pools;
using static HarmonyLib.AccessTools;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(HitboxIdentity), nameof(HitboxIdentity.CheckFriendlyFire), typeof(ReferenceHub), typeof(ReferenceHub), typeof(bool))]
    internal static class CheckFriendlyFirePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLable = generator.DefineLabel();

            int index = 0;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldc_I4_1),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Call, PropertyGetter(typeof(EventHandlers), nameof(EventHandlers.Scp035id))),
                new(OpCodes.Beq_S, returnLable),

                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Call, PropertyGetter(typeof(EventHandlers), nameof(EventHandlers.Scp035id))),
                new(OpCodes.Beq_S, returnLable),

                new(OpCodes.Pop),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLable);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    [HarmonyPatch(typeof(ExplosionGrenade), "ExplodeDestructible")]
    internal static class ExplodeDestructiblePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLable = generator.DefineLabel();

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call &&
            (MethodInfo)i.operand == Method(typeof(ReferenceHub), nameof(ReferenceHub.TryGetHubNetID))) + 2;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),
                new(OpCodes.Ldloc_3),
                new(OpCodes.Call, Method(typeof(ExplodeDestructiblePatch), nameof(ExplodeDestructiblePatch.AllowDamage))),
                new(OpCodes.Brfalse_S, returnLable),
            });

            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldc_I4_0);

            newInstructions[index].WithLabels(returnLable);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool AllowDamage(ReferenceHub attacker, ReferenceHub player) =>
            attacker == null || player == null || attacker.playerId == EventHandlers.Scp035id || attacker.playerId != player.playerId;
    }

    [HarmonyPatch(typeof(LocalCurrentRoomEffects), "FixedUpdate")]
    internal static class FixedUpdatePatch2
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueLable = generator.DefineLabel();

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call
            && (MethodInfo)i.operand == PropertySetter(typeof(LocalCurrentRoomEffects), nameof(LocalCurrentRoomEffects.NetworksyncFlicker)));

            newInstructions[index].WithLabels(continueLable);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Dup),
                new(OpCodes.Brfalse_S, continueLable),
                new(OpCodes.Pop),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(LocalCurrentRoomEffects), "_hub")),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Call, PropertyGetter(typeof(EventHandlers), nameof(EventHandlers.Scp035id))),
                new(OpCodes.Ceq),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ceq),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    [HarmonyPatch(typeof(AttackerDamageHandler), "ProcessDamage", typeof(ReferenceHub))]
    internal static class ProcessDamagePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder Damage = generator.DeclareLocal(typeof(float));
            Label continueLable = generator.DefineLabel();
            Label friendlyfireLable = generator.DefineLabel();

            int index = 0;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Damage))),
                new(OpCodes.Stloc, Damage.LocalIndex),
            });

            index = newInstructions.Count - 4;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // if (this.Attacker.Hub.playerId == EventHandlers.Scp035id)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Attacker))),
                new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Call, PropertyGetter(typeof(EventHandlers), nameof(EventHandlers.Scp035id))),
                new(OpCodes.Ceq),
                new(OpCodes.Brtrue_S, friendlyfireLable),

                // if (ply.playerId == EventHandlers.Scp035id)
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Call, PropertyGetter(typeof(EventHandlers), nameof(EventHandlers.Scp035id))),
                new(OpCodes.Ceq),
                new(OpCodes.Brfalse_S, continueLable),
                
                // this.Damage = Damage;
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(friendlyfireLable),
                new(OpCodes.Ldloc, Damage.LocalIndex),
                new(OpCodes.Callvirt, PropertySetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Damage))),
            });

            index = newInstructions.Count - 4;

            newInstructions[index].WithLabels(continueLable);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}