using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RocketMan;
using Verse;

namespace Soyuz
{
	// Token: 0x02000007 RID: 7
	public class SoyuzPatcher
	{
		// Token: 0x0600001B RID: 27 RVA: 0x00002C98 File Offset: 0x00000E98
		[Main.OnDefsLoaded]
		public static void PatchAll()
		{
			foreach (SoyuzPatchInfo patch in SoyuzPatcher.patches)
			{
				patch.Patch(SoyuzPatcher.harmony);
			}
			Finder.soyuzLoaded = true;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002CDC File Offset: 0x00000EDC
		[Main.OnInitialization]
		public static void Intialize()
		{
			IEnumerable<Type> flaggedTypes = SoyuzPatcher.GetSoyuzPatches();
			List<SoyuzPatchInfo> patchList = new List<SoyuzPatchInfo>();
			foreach (Type type in flaggedTypes)
			{
				SoyuzPatchInfo patch = new SoyuzPatchInfo(type);
				patchList.Add(patch);
			}
			SoyuzPatcher.patches = (from p in patchList
			where p.IsValid
			select p).ToArray<SoyuzPatchInfo>();
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002D90 File Offset: 0x00000F90
		private static IEnumerable<Type> GetSoyuzPatches()
		{
			return from t in typeof(SoyuzPatcher).Assembly.GetTypes()
			where t.HasAttribute<SoyuzPatch>()
			select t;
		}

		// Token: 0x04000028 RID: 40
		public static SoyuzPatchInfo[] patches = null;

		// Token: 0x04000029 RID: 41
		private static readonly Harmony harmony = new Harmony(Finder.HarmonyID + ".Soyuz");
	}
}
