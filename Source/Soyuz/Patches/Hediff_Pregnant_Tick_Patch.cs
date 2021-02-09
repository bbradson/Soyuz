using System;
using HarmonyLib;
using Verse;

namespace Soyuz.Patches
{
	// Token: 0x02000019 RID: 25
	[SoyuzPatch(typeof(Hediff_Pregnant), "Tick", MethodType.Normal, null, null)]
	public class Hediff_Pregnant_Tick_Patch
	{
		// Token: 0x06000052 RID: 82 RVA: 0x0000461C File Offset: 0x0000281C
		public static void Prefix(Hediff_Pregnant __instance)
		{
			Pawn pawn = __instance.pawn;
			if (pawn.IsSkippingTicks() && pawn.IsValidWildlifeOrWorldPawn())
			{
				int deltaT = pawn.GetDeltaT();
				__instance.ageTicks += deltaT - 1;
				__instance.GestationProgress += (float)deltaT / (pawn.RaceProps.gestationPeriodDays * 60000f);
			}
		}
	}
}
