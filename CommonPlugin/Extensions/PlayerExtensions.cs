using UnityEngine;
using Smod2.API;

namespace CommonPlugin.Extensions
{
    public static class PlayerExtensions
    {
        public static ReferenceHub GetHub(this Player player) => ReferenceHub.GetHub(player.GetGameObject() as GameObject);
    }
}