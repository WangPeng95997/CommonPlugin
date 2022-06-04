using System.Reflection;

namespace CommonPlugin.Extensions
{
    public static class Scp106PlayerScript
    {
        public static void TeleportAnimation(this global::Scp106PlayerScript scp106PlayerScript)
        {
            MethodInfo methodInfo = typeof(global::Scp106PlayerScript).GetMethod("RpcTeleportAnimation", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(scp106PlayerScript, null);
        }
    }
}