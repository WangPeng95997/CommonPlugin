using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using PlayableScps;
using UnityEngine;
using HarmonyLib;
using NorthwoodLib.Pools;
using static HarmonyLib.AccessTools;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Scp939), "ServerAttack", typeof(GameObject))]
    internal static class ServerAttackPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label retrunLabel = generator.DefineLabel();

            int index = 0;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(ReferenceHub), nameof(ReferenceHub.GetHub), new[] { typeof(GameObject) })),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Ldsfld, Field(typeof(EventHandlers), nameof(EventHandlers.Scp035id))),
                new(OpCodes.Beq_S, retrunLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retrunLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}