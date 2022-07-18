using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using HarmonyLib;
using NorthwoodLib.Pools;
using static HarmonyLib.AccessTools;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.IsAnyScp))]
    internal static class IsAnyScpPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueLable = generator.DefineLabel();

            int index = 0;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CharacterClassManager), "_hub")),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                new(OpCodes.Call, PropertyGetter(typeof(EventHandlers), nameof(EventHandlers.Scp035id))),
                new(OpCodes.Ceq),
                new(OpCodes.Brfalse_S, continueLable),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ret),
            });

            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + 1;

            newInstructions[index].WithLabels(continueLable);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.RpcPlaceBlood), typeof(Vector3), typeof(int), typeof(float))]
    internal static class PlaceBloodPatch
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

    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.UserCode_CmdRequestHideTag))]
    internal static class CmdRequestHideTagPatch
    {
        private static bool Prefix(CharacterClassManager __instance)
        {
            ReferenceHub hub = ReferenceHub.GetHub(__instance.gameObject);
            bool bGlobalBadge = string.IsNullOrEmpty(hub.serverRoles.NetworkGlobalBadge);
            if (bGlobalBadge)
                hub.queryProcessor.GCT.SendToClient(__instance.connectionToClient, "你没有该权限!", "red");

            return !bGlobalBadge;
        }
    }
}