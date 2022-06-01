using Smod2;
using Smod2.Attributes;
using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace CommonPlugin
{
	[PluginDetails(
		author = "Lunaird",
		name = "萌新天堂服务器",
		description = "萌新天堂服务器核心插件",
		id = "Lunaird.Common",
		configPrefix = "CM",
		langFile = "CommonPlugin",
		version = "A",
		SmodMajor = 3,
		SmodMinor = 9,
		SmodRevision = 10
		)]

	public class CommonPlugin : Plugin
	{
		public static Harmony Harmony { set; get; }

		public override void OnEnable()
		{
			Harmony = new Harmony($"Lunaird.HarmonyPatch");
			Harmony.PatchAll();
			this.Info(Details.name + " Has Loaded Successfully!");
		}

		public override void OnDisable()
		{
			this.Info(Details.name + " Has Been Disabled :(");
		}

		public override void Register()
		{
			AddEventHandlers(new EventHandlers(this));
		}
	}
}