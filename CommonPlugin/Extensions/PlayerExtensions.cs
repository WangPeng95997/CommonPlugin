using UnityEngine;
using Smod2.API;

namespace CommonPlugin.Extensions
{
    public static class PlayerExtensions
    {
        public static GameObject GameObject(this Player player) => player.GetGameObject() as GameObject;

        public static ReferenceHub GetHub(this Player player) => ReferenceHub.GetHub(player.GetGameObject() as GameObject);
    }
}