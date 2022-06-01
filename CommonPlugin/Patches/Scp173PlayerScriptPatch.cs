﻿using System;
using HarmonyLib;
using UnityEngine;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Scp173PlayerScript), "CallCmdHurtPlayer", new Type[] { typeof(GameObject) })]
    internal static class Scp173PlayerScriptPatch
    {
        private static bool Prefix(GameObject target)
        {
            return ReferenceHub.GetHub(target).playerId != EventHandlers.Scp035id;
        }
    }
}