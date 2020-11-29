using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RocketMan;
using Verse;

namespace Soyuz.Patches
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Tick))]
    public class Pawn_Tick_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.MethodReplacer(
                AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Suspended)),
                AccessTools.Method(typeof(Pawn_Tick_Patch), nameof(Suspended))).ToList();
            foreach (var code in codes)
                yield return code;
        }

        public static bool Suspended(Thing thing)
        {
            if (thing.Suspended)
                return true;
            if (!(thing is Pawn pawn))
                return thing.Suspended;
            var shouldTick = pawn.ShouldTick();
            if ( !pawn.IsValidWildlifeOrWorldPawn())
                return thing.Suspended;           
            if (!shouldTick)
            {
                pawn.ageTracker?.AgeTick();
                if (pawn.Spawned) pawn.stances?.StanceTrackerTick();
                if (pawn.Spawned && !pawn.OffScreen())
                {
                    pawn.drawer?.DrawTrackerTick();
                    pawn.rotationTracker?.RotationTrackerTick();
                }
                if (Finder.flashDilatedPawns && pawn.Spawned)
                    pawn.Map.debugDrawer.FlashCell(pawn.positionInt, 0.05f, $"{pawn.OffScreen()}", 100);
                return true;
            }
            return false;
        }
    }
}