using UnityEngine;

namespace CommonPlugin.Extensions
{
    public static class Player
    {
        public static GameObject GameObject(this Smod2.API.Player player) => player.GetGameObject() as GameObject;

        public static global::ReferenceHub GetHub(this Smod2.API.Player player) => global::ReferenceHub.GetHub(player.GetGameObject() as GameObject);
    }
}