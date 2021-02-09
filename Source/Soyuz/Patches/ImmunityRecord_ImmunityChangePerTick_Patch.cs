using System;
using HarmonyLib;
using Verse;

namespace Soyuz.Patches
{
	// Token: 0x0200001A RID: 26
	[SoyuzPatch(typeof(ImmunityRecord), "ImmunityChangePerTick", MethodType.Normal, null, null)]
	public class ImmunityRecord_ImmunityChangePerTick_Patch
	{
		// Token: 0x06000054 RID: 84 RVA: 0x00004678 File Offset: 0x00002878
		public static void Postfix(ImmunityRecord __instance, ref float __result, Pawn pawn)
		{
			if (pawn.IsValidWildlifeOrWorldPawn() && pawn.IsSkippingTicks())
			{
				__result *= (float)pawn.GetDeltaT();
			}
		}
	}
}
