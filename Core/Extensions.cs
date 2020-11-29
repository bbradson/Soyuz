using System;
using System.Collections.Generic;
using RimWorld.Planet;
using RocketMan;
using UnityEngine;
using Verse;

namespace Soyuz
{
    public static class Extensions
    {
        private static Pawn _pawnTick;
        private static Pawn _pawnScreen;

        private static bool offScreen;
        private static bool shouldTick;

        private static int curDelta;

        private static readonly Dictionary<int, int> timers = new Dictionary<int, int>();

        private static int DilationRate
        {
            get
            {
                switch (Context.zoomRange)
                {
                    default:
                        return 1;
                    case CameraZoomRange.Closest:
                        return 60;
                    case CameraZoomRange.Close:
                        return 20;
                    case CameraZoomRange.Middle:
                        return 15;
                    case CameraZoomRange.Far:
                        return 15;
                    case CameraZoomRange.Furthest:
                        return 7;
                }
            }
        }

        public static bool OffScreen(this Pawn pawn)
        {
            if (Finder.alwaysDilating)
                return offScreen = true;
            if (_pawnScreen == pawn)
                return offScreen;
            _pawnScreen = pawn;
            if (Context.curViewRect.Contains(pawn.positionInt))
                return offScreen = false;
            return offScreen = true;
        }

        public static bool IsSkippingTicks(this Pawn pawn)
        {
            if (!Finder.timeDilation) return false;
            return _pawnTick == pawn && ((!pawn.Spawned && Finder.timeDilationWorldPawns) || pawn.OffScreen() ||
                                         Context.zoomRange == CameraZoomRange.Far ||
                                         Context.zoomRange == CameraZoomRange.Furthest);
        }

        public static bool ShouldTick(this Pawn pawn)
        {
            var tick = GenTicks.TicksGame;
            _pawnTick = pawn;
            shouldTick = ShouldTickInternal(pawn);
            if (timers.TryGetValue(pawn.thingIDNumber, out var val)) curDelta = tick - val;
            else curDelta = 1;
            if (shouldTick) timers[pawn.thingIDNumber] = tick;
            return shouldTick;
        }

        public static bool IsCustomTickInterval(this Thing thing, int interval)
        {
            if (!thing.Spawned && Finder.timeDilationWorldPawns)
            {
                int val = (int) Mathf.Max((float) interval / WorldPawnsTicker.BucketCount, 1);
                return WorldPawnsTicker.curCycle % val == 0;
            }
            else if(thing is Pawn pawn && pawn.IsSkippingTicks())
            {
                int val = (int)Mathf.Max(Mathf.RoundToInt(interval / 30) * 30, 30);
                return (thing.thingIDNumber + GenTicks.TicksGame) % val == 0;
            }
            return thing.IsHashIntervalTick(interval);
        }

        private static bool ShouldTickInternal(Pawn pawn)
        {
            if (!Finder.timeDilation) return true;
            if (!pawn.Spawned && Finder.timeDilationWorldPawns)
                return true;
            var tick = GenTicks.TicksGame;
            if (false
                || (pawn.thingIDNumber + tick) % 30 == 0
                || pawn.jobs?.curJob != null && pawn.jobs?.curJob?.expiryInterval > 0 &&
                (tick - pawn.jobs.curJob.startTick) % (pawn.jobs.curJob.expiryInterval * 2) == 0)
                return true;
            if (pawn.OffScreen())
                return (pawn.thingIDNumber + tick) % DilationRate == 0;
            if (Context.zoomRange == CameraZoomRange.Far || Context.zoomRange == CameraZoomRange.Furthest)
                return (pawn.thingIDNumber + tick) % 3 == 0;
            return true;
        }

        public static int GetDeltaT(this Thing thing)
        {
            if (_pawnTick != thing)
            {
                if (timers.TryGetValue(thing.thingIDNumber, out var val))
                    return GenTicks.TicksGame - val;
                throw new Exception();
            }
            return curDelta;
        }

        public static bool IsValidWildlifeOrWorldPawn(this Pawn pawn)
        {
            return _pawnTick == pawn && ((pawn.RaceProps.Animal && pawn.factionInt == null) || (!pawn.Spawned && Finder.timeDilationWorldPawns));
        }
    }
}