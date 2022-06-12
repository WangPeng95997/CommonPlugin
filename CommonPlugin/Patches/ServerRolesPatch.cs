using System;
using MEC;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(ServerRoles), "UserCode_CmdServerSignatureComplete", typeof(string), typeof(string), typeof(string), typeof(bool))]
    internal static class ServerRolesPatch
    {
        public static Vector3 startPostion = Vector3.zero;

        private static Vector3 defaultPostion = new Vector3(0.111f, 1010.06f, -10.30f);

        private static Transform transform;

        private static void Postfix(ServerRoles __instance, string challenge, string response, string publickey, bool hide)
        {
            if (GameCore.RoundStart.singleton.NetworkTimer > 1 || GameCore.RoundStart.singleton.NetworkTimer == -2)
                Timing.CallDelayed(0.1f, () =>
                {
                    transform.SetPositionAndRotation(new Vector3(330.2f, 573.8f, 0.0f), transform.rotation);
                    NetworkServer.UnSpawn(transform.gameObject);
                    NetworkServer.Spawn(transform.gameObject);

                    ReferenceHub hub = ReferenceHub.GetHub(__instance.gameObject);
                    hub.characterClassManager.NetworkCurClass = RoleType.Tutorial;
                    hub.playerStats.GetModule<HealthStat>().CurValue = 100.0f;
                    hub.playerMovementSync.OverridePosition(startPostion);
                });
        }

        public static Vector3 GetWarheadPostion()
        {
            Transform tf = AlphaWarheadOutsitePanel.nukeside.transform;
            Vector3 postion = tf.position;

            switch (Convert.ToInt32(tf.rotation.eulerAngles.y))
            {
                case 180:
                    return new Vector3(postion.x + 9.4f, -597.3f, postion.z - 10.6f);

                case 270:
                    return new Vector3(postion.x - 10.55f, -597.3f, postion.z - 9.35f);

                default:
                    return defaultPostion;
            }
        }

        public static void SetStartScreen()
        {
            transform = GameObject.Find("StartRound").transform;
            transform.localScale = new Vector3(0.4f, 0.4f, 1.0f);
            startPostion = new System.Random().Next(2) == 0 ? defaultPostion : GetWarheadPostion();
        }
    }
}