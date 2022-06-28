using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Footprinting;
using InventorySystem.Items.ThrowableProjectiles;
using PlayerStatsSystem;
using UnityEngine;
using HarmonyLib;
using NorthwoodLib.Pools;
using static HarmonyLib.AccessTools;

namespace CommonPlugin.Patches
{
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
                new(OpCodes.Ldc_I4_1),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Ldsfld, Field(typeof(EventHandlers), nameof(EventHandlers.Scp035id))),
                new(OpCodes.Beq_S, returnLable),

                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Ldsfld, Field(typeof(EventHandlers), nameof(EventHandlers.Scp035id))),
                new(OpCodes.Beq_S, returnLable),

                new(OpCodes.Pop),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLable);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
    /*
    [HarmonyPatch(typeof(HitboxIdentity), "Damage", typeof(float), typeof(DamageHandlerBase), typeof(Vector3))]
    internal static class DamagePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLable = generator.DefineLabel();

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_2);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, PropertyGetter(typeof(HitboxIdentity), nameof(HitboxIdentity.TargetHub))),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Call, Method(typeof(DamagePatch), nameof(DamagePatch.AllowDamage))),
                new(OpCodes.Brfalse_S, returnLable),
            });

            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldc_I4_0);

            newInstructions[index].WithLabels(returnLable);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool AllowDamage(ReferenceHub TargetHub, DamageHandlerBase handler)
        {
            ExplosionDamageHandler explosionDamageHandler;

            if ((explosionDamageHandler = (handler as ExplosionDamageHandler)) != null)
            {
                // TODO
                Smod2.PluginManager.Manager.Plugins[0].Info($"attacker: {explosionDamageHandler.Attacker.PlayerId}, target: {TargetHub.playerId}");

                if (explosionDamageHandler.Attacker.PlayerId == TargetHub.playerId && TargetHub.playerId != EventHandlers.Scp035id)
                    return false;
            }

            return true;
        }
    }
    */
    
    [HarmonyPatch(typeof(ExplosionGrenade), "ExplodeDestructible", typeof(IDestructible), typeof(Footprint), typeof(Vector3), typeof(ExplosionGrenade))]
    internal static class ExplodeDestructiblePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLable = generator.DefineLabel();

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldloc_S);

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

        private static bool AllowDamage(ReferenceHub attacker, ReferenceHub player)
        {
            if (attacker.playerId == EventHandlers.Scp035id)
            {
                switch (player.characterClassManager.NetworkCurClass)
                {
                    case RoleType.Scp049:
                    case RoleType.Scp0492:
                    case RoleType.Scp096:
                    case RoleType.Scp106:
                    case RoleType.Scp173:
                    case RoleType.Scp93953:
                    case RoleType.Scp93989:
                        return false;
                }
            }
            else if (attacker.playerId == player.playerId)
                return false;

            return true;
        }
    }

    /*
    [HarmonyPatch(typeof(LocalCurrentRoomEffects), "FixedUpdate")]
    internal static class NightVisionPatch
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
    */
}