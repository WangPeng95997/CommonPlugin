using CustomPlayerEffects;
using Hints;
using Interactables.Interobjects;
using InventorySystem.Items.Usables.Scp330;
using PlayerStatsSystem;
using HarmonyLib;
using CommonPlugin.Extensions;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Scp330Interobject), "ServerInteract", typeof(ReferenceHub), typeof(byte))]
    internal static class ServerInteractPatch2
    {
        private static bool Prefix(Ragdoll __instance, ReferenceHub ply, byte colliderId)
        {
            if (GameCore.RoundStart.singleton.NetworkTimer != -1)
            {
                ply.hints.Show(
                    new TextHint("<size=30><b>游戏还未开始, 请勿触碰!</b></size>",
                    new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), 3.0f));

                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CandyBlue), "ServerApplyEffects", typeof(ReferenceHub))]
    internal static class CandyBluePatch
    {
        private const float candyBlueAhp = 40.0f;

        private static bool Prefix(Ragdoll __instance, ReferenceHub hub)
        {
            if (hub.playerId != EventHandlers.Scp035id)
            {
                AhpStat.AhpProcess ahpProcess = hub.GetAhpProcess();

                if (ahpProcess.CurrentAmount + candyBlueAhp > ahpProcess.Limit)
                    ahpProcess.CurrentAmount = ahpProcess.Limit;
                else
                    ahpProcess.CurrentAmount += candyBlueAhp;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CandyGreen), "ServerApplyEffects", typeof(ReferenceHub))]
    internal static class CandyGreenPatch
    {
        private static bool Prefix(Ragdoll __instance, ReferenceHub hub)
        {
            if (hub.playerId != EventHandlers.Scp035id)
            {
                Scp330Bag.AddSimpleRegeneration(hub, 1.5f, 80f);
                hub.playerEffectsController.EnableEffect<Vitality>(30f, true);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CandyPurple), "ServerApplyEffects", typeof(ReferenceHub))]
    internal static class CandyPurplePatch
    {
        private static bool Prefix(Ragdoll __instance, ReferenceHub hub)
        {
            if (hub.playerId != EventHandlers.Scp035id)
            {
                Scp330Bag.AddSimpleRegeneration(hub, 2.5f, 10f);

                DamageReduction effect = hub.playerEffectsController.GetEffect<DamageReduction>();
                effect.Intensity = 25;
                effect.ServerChangeDuration(15f);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CandyRainbow), "ServerApplyEffects", typeof(ReferenceHub))]
    internal static class CandyRainbowPatch
    {
        private const float candyRainbowAdd = 20.0f;

        private static bool Prefix(Ragdoll __instance, ReferenceHub hub)
        {
            if (hub.playerId != EventHandlers.Scp035id)
            {
                AhpStat.AhpProcess ahpProcess = hub.GetAhpProcess();

                if (ahpProcess.CurrentAmount + candyRainbowAdd > ahpProcess.Limit)
                    ahpProcess.CurrentAmount = ahpProcess.Limit;
                else
                    ahpProcess.CurrentAmount += candyRainbowAdd;

                hub.playerStats.GetModule<HealthStat>().ServerHeal(candyRainbowAdd);
                hub.playerEffectsController.EnableEffect<Invigorated>(10.0f, true);
                hub.playerEffectsController.EnableEffect<RainbowTaste>(10.0f, true);
                hub.playerEffectsController.GetEffect<BodyshotReduction>().Intensity += 1;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CandyRed), "ServerApplyEffects", typeof(ReferenceHub))]
    internal static class CandyRedPatch
    {
        private static bool Prefix(Ragdoll __instance, ReferenceHub hub)
        {
            if (hub.playerId != EventHandlers.Scp035id)
            {
                Scp330Bag.AddSimpleRegeneration(hub, 10.0f, 5.0f);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CandyYellow), "ServerApplyEffects", typeof(ReferenceHub))]
    internal static class CandyYellowPatch
    {
        private static bool Prefix(Ragdoll __instance, ReferenceHub hub)
        {
            if (hub.playerId != EventHandlers.Scp035id)
            {
                hub.playerEffectsController.EnableEffect<Invigorated>(1.0f, true);
                hub.playerEffectsController.GetEffect<MovementBoost>().Intensity = 4;
                hub.playerEffectsController.EnableEffect<MovementBoost>(0.0f, false);
            }

            return false;
        }
    }
}