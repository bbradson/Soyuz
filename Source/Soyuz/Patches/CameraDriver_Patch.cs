using System;
using HarmonyLib;
using RocketMan;
using Verse;

namespace Soyuz.Patches
{
	// Token: 0x02000015 RID: 21
	[SoyuzPatch(typeof(CameraDriver), "Update", MethodType.Normal, null, null)]
	public class CameraDriver_Patch
	{
		// Token: 0x0600004D RID: 77 RVA: 0x00004597 File Offset: 0x00002797
		public static void Postfix(CameraDriver __instance)
		{
			Context.zoomRange = __instance.CurrentZoom;
			Context.curViewRect = __instance.CurrentViewRect;
			if (Finder.debug && Finder.statLogging)
			{
				Log.Message(string.Format("SOYUZ: Zoom range is {0}", Context.zoomRange), false);
			}
		}
	}
}
