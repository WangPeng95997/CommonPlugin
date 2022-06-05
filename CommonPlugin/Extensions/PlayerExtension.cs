using UnityEngine;
using Smod2.API;

namespace CommonPlugin.Extensions
{
    public static class PlayerExtension
    {
        public static GameObject GameObject(this Player player) => player.GetGameObject() as GameObject;

        public static global::ReferenceHub GetHub(this Player player) => global::ReferenceHub.GetHub(player.GetGameObject() as GameObject);
    }
}