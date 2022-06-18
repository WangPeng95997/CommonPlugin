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
    
    [HarmonyPatch(typeof(LocalCurrentRoomEffects), "IsInDarkenedRoom", typeof(Vector3))]
    internal static class IsInDarkenedRoomPatch
    {
        private static bool Prefix(LocalCurrentRoomEffects __instance, Vector3 PositionToCheck)
        {
            return ReferenceHub.GetHub(__instance.gameObject).playerId != EventHandlers.Scp035id;
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