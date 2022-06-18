using System.Collections.Generic;
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
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(ServerAttackPatch), nameof(ServerAttackPatch.AllowAttack))),
                new(OpCodes.Brfalse_S, retrunLabel),
            });

            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldc_I4_0);
            newInstructions[index].WithLabels(retrunLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool AllowAttack(GameObject gameObject)
        {
            if (gameObject.GetComponent<BreakableWindow>() == null && ReferenceHub.GetHub(gameObject).playerId == EventHandlers.Scp035id)
                return false;

            return true;
        }
    }
}