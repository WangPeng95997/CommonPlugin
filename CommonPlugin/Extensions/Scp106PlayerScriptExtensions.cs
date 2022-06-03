using System.Reflection;

namespace CommonPlugin.Extensions
{
    public static class Scp106PlayerScriptExtensions
    {
        public static void TeleportAnimation(this Scp106PlayerScript scp106PlayerScript)
        {
            MethodInfo methodInfo = typeof(Scp106PlayerScript).GetMethod("RpcTeleportAnimation", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(scp106PlayerScript, null);
        }
    }
}