using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Hints;
using InventorySystem.Items;
using InventorySystem.Items.Usables;

using HarmonyLib;
using static HarmonyLib.AccessTools;

using NorthwoodLib.Pools;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(UsableItemsController), "ServerReceivedStatus")]
    internal static class UsableItemsControllerPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            int index = 20;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_1),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(UsableItemsControllerPatch), nameof(UsableItemsControllerPatch.OnUsingItem))),
                new(OpCodes.Brtrue_S,returnLabel),
                new(OpCodes.Ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool OnUsingItem(ReferenceHub hub, UsableItem usableItem)
        {
            ItemType itemType = usableItem.gameObject.GetComponent<ItemBase>().ItemTypeId;

            if (itemType == ItemType.SCP207 && hub.playerEffectsController.GetEffect<CustomPlayerEffects.Scp207>().Intensity>0)
            {
                hub.hints.Show(
                    new TextHint("<b><color=#FF0000>SCP-207</color>的效果不能进行叠加</b>",
                    new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));

                return false;
            }

            if (hub.playerId == EventHandlers.Scp035id)
            {
                switch (itemType)
                {
                    case ItemType.SCP207:
                    case ItemType.SCP268:
                    case ItemType.Painkillers:
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