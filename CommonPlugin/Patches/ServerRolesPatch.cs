using System;
using MEC;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;
using HarmonyLib;
using CommonPlugin.Extensions;

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
            if (GameCore.RoundStart.singleton.NetworkTimer == -2)
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

        public static void SetStartScreen()
        {
            transform = GameObject.Find("StartRound").transform;
            transform.localScale = new Vector3(0.4f, 0.4f, 1.0f);

            WaitingPosition waitingPosition = (WaitingPosition)(new System.Random().Next((int)WaitingPosition.PositionCount));
            switch (waitingPosition)
            {
                case WaitingPosition.defaultPosition:
                    startPostion = defaultPostion;
                    break;

                case WaitingPosition.Scp173Position:
                    startPostion = GetScp173Position();
                    break;

                case WaitingPosition.Scp330Position:
                    startPostion = GetWarheadPostion();
                    break;

                case WaitingPosition.WarheadPostion:
                    startPostion = MapManager.Scp330Room.Position;
                    break;
            }
        }

        public static Vector3 GetScp173Position()
        {
            Transform transform = MapManager.Scp173Room.Transform;
            Vector3 postion = transform.position;

            switch (Convert.ToInt32(transform.rotation.eulerAngles.y))
            {
                case 75:
                    return new Vector3(postion.x + 24.5f, 21.0f, postion.z - 0.8f);

                case 255:
                    return new Vector3(postion.x - 24.5f, 21.0f, postion.z + 0.8f);

                default:
                    return defaultPostion;
            }
        }

        public static Vector3 GetWarheadPostion()
        {
            Transform transform = AlphaWarheadOutsitePanel.nukeside.transform;
            Vector3 postion = transform.position;

            switch (Convert.ToInt32(transform.rotation.eulerAngles.y))
            {
                case 180:
                    return new Vector3(postion.x + 9.4f, -596.8f, postion.z - 10.6f);

                case 270:
                    return new Vector3(postion.x - 10.55f, -596.8f, postion.z - 9.35f);

                default:
                    return defaultPostion;
            }
        }
    }
}