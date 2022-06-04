using System.Reflection;
using Assets._Scripts.Dissonance;
using HarmonyLib;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(Radio), "UserCode_CmdSyncTransmissionStatus", typeof(bool))]
    internal static class RadioPatch
    {
        private static FieldInfo fieldInfo = typeof(Radio).GetField("_dissonanceSetup", BindingFlags.Instance | BindingFlags.NonPublic);

        private static void Prefix(Radio __instance, bool b)
        {
            RoleType roleType = __instance.gameObject.GetComponent<CharacterClassManager>().NetworkCurClass;

            switch (roleType)
            {
                case RoleType.Scp049:
                case RoleType.Scp0492:
                case RoleType.Scp096:
                case RoleType.Scp106:
                case RoleType.Scp173:
                case RoleType.Scp93953:
                case RoleType.Scp93989:
                    (fieldInfo.GetValue(__instance) as DissonanceUserSetup).MimicAs939 = b;
                    break;
            }
        }
    }
}