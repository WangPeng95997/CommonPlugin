using System.Collections.Generic;
using System.Reflection.Emit;

using Hints;
using InventorySystem.Items.Usables;
using Mirror;
using UnityEngine;

using HarmonyLib;
using NorthwoodLib.Pools;

using static HarmonyLib.AccessTools;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(UsableItemsController), "ServerReceivedStatus", typeof(NetworkConnection), typeof(StatusMessage))]
    internal static class ServerReceivedStatusPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_2) + 1;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.gameObject))),
                new(OpCodes.Call, Method(typeof(ReferenceHub), nameof(ReferenceHub.GetHub), new[] { typeof(GameObject) })),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Call, Method(typeof(ServerReceivedStatusPatch), nameof(ServerReceivedStatusPatch.AllowUsingItem))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool AllowUsingItem(ReferenceHub hub, UsableItem usableItem)
        {
            if (usableItem.ItemTypeId == ItemType.SCP207 && hub.playerEffectsController.GetEffect<CustomPlayerEffects.Scp207>().Intensity > 0)
            {
                hub.hints.Show(
                    new TextHint("<b><color=#FF0000>SCP-207</color>的效果不能进行叠加</b>",
                    new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));

                return false;
            }

            if (hub.playerId == EventHandlers.Scp035id)
            {
                switch (usableItem.ItemTypeId)
                {
                    case ItemType.SCP207:
                    case ItemType.SCP268:
                    case ItemType.SCP1853:
                        hub.hints.Show(
                            new TextHint("<b><color=#FF0000>SCP-035</color>不能使用该物品</b>",
                            new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));

                        return false;
                }
            }

            return true;
        }
    }
}