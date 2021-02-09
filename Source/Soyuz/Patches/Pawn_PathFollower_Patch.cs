using System;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace Soyuz.Patches
{
	// Token: 0x02000029 RID: 41
	public static class Pawn_PathFollower_Patch
	{
		// Token: 0x0400004E RID: 78
		private static float remaining;

		// Token: 0x0400004F RID: 79
		private static Pawn curPawn;

		// Token: 0x02000044 RID: 68
		[SoyuzPatch(typeof(Pawn_PathFollower), "CostToPayThisTick", MethodType.Normal, null, null)]
		public class Pawn_PathFollower_CostToPayThisTick_Patch
		{
			// Token: 0x060000D3 RID: 211 RVA: 0x00006E70 File Offset: 0x00005070
			public static void Postfix(Pawn_PathFollower __instance, ref float __result)
			{
				Pawn_PathFollower_Patch.remaining = 0f;
				if (__instance.pawn.IsValidWildlifeOrWorldPawn() && __instance.pawn.IsSkippingTicks())
				{
					Pawn_PathFollower_Patch.curPawn = __instance.pawn;
					float modified = __result * (float)__instance.pawn.GetDeltaT();
					float cost = __instance.nextCellCostLeft;
					if (modified > cost)
					{
						Pawn_PathFollower_Patch.remaining = modified - cost;
						__result = cost;
						return;
					}
					__result = modified;
				}
			}
		}

		// Token: 0x02000045 RID: 69
		[SoyuzPatch(typeof(Pawn_PathFollower), "SetupMoveIntoNextCell", MethodType.Normal, null, null)]
		public class Pawn_PathFollower_SetupMoveIntoNextCell_Patch
		{
			// Token: 0x060000D5 RID: 213 RVA: 0x00006ED8 File Offset: 0x000050D8
			public static void Postfix(Pawn_PathFollower __instance)
			{
				Pawn pawn = __instance.pawn;
				if (pawn == Pawn_PathFollower_Patch.curPawn)
				{
					__instance.nextCellCostLeft -= Pawn_PathFollower_Patch.remaining;
					__instance.nextCellCostTotal -= Pawn_PathFollower_Patch.remaining;
					Pawn_PathFollower_Patch.curPawn = null;
				}
			}
		}
	}
}
