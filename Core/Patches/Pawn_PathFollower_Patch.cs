using HarmonyLib;
using UnityEngine;
using Verse.AI;

namespace Soyuz.Patches
{
    [HarmonyPatch(typeof(Pawn_PathFollower), nameof(Pawn_PathFollower.CostToPayThisTick))]
    public class Pawn_PathFollower_CostToPayThisTick_Patch
    {
        public static void Postfix(Pawn_PathFollower __instance, ref float __result)
        {
            if (true
                && __instance.pawn.IsValidWildlifeOrWorldPawn()
                && __instance.pawn.IsSkippingTicks())
                __result = Mathf.Min(__result * __instance.pawn.GetDeltaT(), __instance.nextCellCostLeft);
        }
    }
}