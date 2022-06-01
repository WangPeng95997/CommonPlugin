﻿using UnityEngine;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Scp106PlayerScript), "UserCode_CmdMovePlayer", typeof(GameObject), typeof(int))]
    internal static class Scp106PlayerScriptPatch
    {
        private static bool Prefix(GameObject ply, int t)
        {
            ReferenceHub hub = ReferenceHub.GetHub(ply);

            return hub.playerId != EventHandlers.Scp035id && !hub.scp106PlayerScript.goingViaThePortal;
        }
    }
}