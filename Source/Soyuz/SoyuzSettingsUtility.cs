using System;
using System.Collections.Generic;
using System.Linq;
using RocketMan;
using Verse;

namespace Soyuz
{
	// Token: 0x0200000D RID: 13
	public static class SoyuzSettingsUtility
	{
		// Token: 0x06000039 RID: 57 RVA: 0x00003893 File Offset: 0x00001A93
		[Main.OnDefsLoaded]
		public static void LoadSettings()
		{
			SoyuzSettingsUtility.pawnDefs = (from def in DefDatabase<ThingDef>.AllDefs
			where def.race != null
			select def).ToList<ThingDef>();
			SoyuzSettingsUtility.CacheSettings();
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000038CD File Offset: 0x00001ACD
		[Main.OnScribe]
		public static void PostScribe()
		{
			Scribe_Deep.Look<SoyuzSettings>(ref Context.settings, "soyuzSettings", Array.Empty<object>());
			Finder.soyuzLoaded = true;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000038EC File Offset: 0x00001AEC
		public static void CacheSettings()
		{
			if (Context.settings == null)
			{
				Context.settings = new SoyuzSettings();
			}
			if (Context.settings.raceSettings.Count == 0)
			{
				SoyuzSettingsUtility.CreateSettings();
			}
			foreach (RaceSettings element in Context.settings.raceSettings)
			{
				if (element.pawnDef == null)
				{
					element.ResolveContent();
					if (element.pawnDef == null)
					{
						continue;
					}
				}
				element.Cache();
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003980 File Offset: 0x00001B80
		public static void CreateSettings()
		{
			Context.settings.raceSettings.Clear();
			foreach (ThingDef def in SoyuzSettingsUtility.pawnDefs)
			{
				Context.settings.raceSettings.Add(new RaceSettings
				{
					pawnDef = def,
					pawnDefName = def.defName,
					dilated = (def.race.Animal && !def.race.Humanlike && !def.race.IsMechanoid),
					ignoreFactions = false
				});
			}
			Finder.rocketMod.WriteSettings();
		}

		// Token: 0x0400003E RID: 62
		private static List<ThingDef> pawnDefs;
	}
}
