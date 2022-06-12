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
    /*
    [HarmonyPatch(typeof(GameObject), "CallCmdHurtPlayer", typeof(GameObject))]
    internal static class Scp173Patch
    {
        private static bool Prefix(GameObject target)
        {
            return ReferenceHub.GetHub(target).playerId != EventHandlers.Scp035id;
        }
    }
    */

    [HarmonyPatch(typeof(Scp173), "UpdateObservers")]
    internal static class FixedUpdatePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label addVision = generator.DefineLabel();

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_S) + 1;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_3),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Ldsfld, Field(typeof(EventHandlers), nameof(EventHandlers.Scp035id))),
                new(OpCodes.Beq_S, addVision),
            });

            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Brfalse_S) + 1;

            newInstructions[index].WithLabels(addVision);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}