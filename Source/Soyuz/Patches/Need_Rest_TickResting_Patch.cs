using System;
using HarmonyLib;
using RimWorld;

namespace Soyuz.Patches
{
	// Token: 0x0200001E RID: 30
	[SoyuzPatch(typeof(Need_Rest), "TickResting", MethodType.Normal, null, null)]
	public class Need_Rest_TickResting_Patch
	{
		// Token: 0x0600005C RID: 92 RVA: 0x0000476E File Offset: 0x0000296E
		public static void Postfix(Need_Rest __instance)
		{
			if (__instance.pawn.IsValidWildlifeOrWorldPawn() && __instance.pawn.IsSkippingTicks())
			{
				__instance.lastRestTick += __instance.pawn.GetDeltaT();
			}
		}
	}
}
