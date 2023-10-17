using Smod2;
using Smod2.Attributes;
using HarmonyLib;

namespace CommonPlugin
{
    [PluginDetails(
        author = "l4kkS41",
        name = "萌新天堂服务器",
        description = "萌新天堂服务器核心插件V2.0",
        id = "l4kkS41.CommonPlugin",
        configPrefix = "CM",
        langFile = "CommonPlugin",
        version = "A",
        SmodMajor = 3,
        SmodMinor = 10,
        SmodRevision = 6
        )]
    public class CommonPlugin : Plugin
    {
        public static Harmony Harmony { set; get; }

        public override void OnEnable()
        {
            Harmony = new Harmony("l4kkS41.HarmonyPatch");
            Harmony.PatchAll();
            this.Info(Details.name + " Has Loaded Successfully!");
        }

        public override void OnDisable()
        {
            this.Info(Details.name + " Has Been Disabled :(");
        }

        public override void Register()
        {
            this.AddEventHandlers(new EventHandlers(this));
        }
    }
}