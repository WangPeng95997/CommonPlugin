using System.Reflection;

namespace CommonPlugin.Extensions
{
    public static class Scp106PlayerScriptExtension
    {
        public static void TeleportAnimation(this Scp106PlayerScript scp106PlayerScript)
        {
            MethodInfo methodInfo = typeof(global::Scp106PlayerScript).GetMethod("RpcTeleportAnimation", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(scp106PlayerScript, null);
        }
    }
}