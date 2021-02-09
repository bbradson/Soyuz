using System;
using HarmonyLib;
using Verse;

namespace Soyuz.Patches
{
	// Token: 0x02000017 RID: 23
	[SoyuzPatch(typeof(Hediff), "BleedRate", MethodType.Getter, null, null)]
	public static class Hediff_BleedRate_Patch
	{
		// Token: 0x06000050 RID: 80 RVA: 0x000045D7 File Offset: 0x000027D7
		public static void Postfix(Hediff __instance, ref float __result)
		{
			if (__instance.pawn.IsValidWildlifeOrWorldPawn() && __instance.pawn.IsSkippingTicks())
			{
				__result *= (float)__instance.pawn.GetDeltaT();
			}
		}
	}
}
