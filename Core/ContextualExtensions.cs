using System;
using System.Collections.Generic;
using RimWorld.Planet;
using RocketMan;
using UnityEngine;
using Verse;

namespace Soyuz
{
    [StaticConstructorOnStartup]
    public static class ContextualExtensions
    {
        private static Pawn _pawnTick;
        private static Pawn _pawnScreen; 

        private static bool offScreen;
        private static bool shouldTick;

        private static int curDelta;

        private const int TransformationCacheSize = 2500;

        private static readonly int[] _transformationCache = new int[TransformationCacheSize];
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

        static ContextualExtensions()
        {
            for (int i = 0; i < _transformationCache.Length; i++)
                _transformationCache[i] = (int) Mathf.Max(Mathf.RoundToInt(i / 30) * 30, 30);
        }

        private static int RoundTransform(int interval)
        {
            if(interval >= TransformationCacheSize)
                return (int)Mathf.Max(Mathf.RoundToInt(interval / 30) * 30, 30);
            return _transformationCache[interval];
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

        private static bool _isSkippingPawn = false;
        private static Pawn _skippingPawn = null;
        public static bool IsSkippingTicks(this Pawn pawn)
        {
            if (!Finder.timeDilation) 
                return false;
            if (pawn == _skippingPawn) 
                return _isSkippingPawn;
            bool spawned = pawn.Spawned;
            _skippingPawn = pawn;
            _isSkippingPawn = _pawnTick == pawn && (
                        (spawned == false && WorldPawnsTicker.isActive && Finder.timeDilationWorldPawns) ||
                                         (spawned == true && (pawn.OffScreen() || 
                                                              Context.zoomRange == CameraZoomRange.Far ||
                                                              Context.zoomRange == CameraZoomRange.Furthest)));
            return _isSkippingPawn;
        }

        public static void BeginTick(this Pawn pawn)
        {
            _pawnTick = pawn;
            if (!Finder.enabled || !Finder.timeDilation || !pawn.IsValidWildlifeOrWorldPawn())
            {
                _isValidPawn = false;
                _isSkippingPawn = false;
                _skippingPawn = pawn;
            }
        }

        public static void EndTick(this Pawn pawn)
        {
            _pawnTick = null;
            _validPawn = null;
            _pawnScreen = null;
            _skippingPawn = null;
        }

        public static bool ShouldTick(this Pawn pawn)
        {
            var tick = GenTicks.TicksGame;
            shouldTick = ShouldTickInternal(pawn);
            if (timers.TryGetValue(pawn.thingIDNumber, out var val)) curDelta = tick - val;
            else curDelta = 1;
            if (shouldTick) timers[pawn.thingIDNumber] = tick;
            return shouldTick;
        }
        
        public static bool IsCustomTickInterval(this Thing thing, int interval)
        {
            if (_pawnTick == thing && Finder.timeDilation && Finder.enabled)
            {
                if (WorldPawnsTicker.isActive)
                {
                    return WorldPawnsTicker.curCycle % WorldPawnsTicker.Transform(interval) == 0;
                }
                else if (((Pawn) thing).IsSkippingTicks())
                {
                    return (thing.thingIDNumber + GenTicks.TicksGame) % RoundTransform(interval) == 0;
                }
            }
            return thing.IsHashIntervalTick(interval);
        }

        private static bool ShouldTickInternal(Pawn pawn)
        {
            if (!Finder.timeDilation || !Finder.enabled) 
                return true;
            if (WorldPawnsTicker.isActive && Finder.timeDilationWorldPawns)
                return true;
            var tick = GenTicks.TicksGame;
            if (false
                || (pawn.thingIDNumber + tick) % 30 == 0
                || (tick % 250 == 0)
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
            if (thing == _pawnTick) 
                return curDelta;
            if (timers.TryGetValue(thing.thingIDNumber, out var val))
                return GenTicks.TicksGame - val;
            throw new Exception();
        }

        private static bool _isValidPawn = false;
        private static Pawn _validPawn = null;
        public static bool IsValidWildlifeOrWorldPawn(this Pawn pawn)
        {
            if (_validPawn == pawn) 
                return _isValidPawn;
            _validPawn = pawn;
            _isValidPawn = (_pawnTick == pawn && !pawn.IsColonist && (
                (Context.dilatedRaces[pawn.def.index] && ((pawn.factionInt == null && !Context.ignoreFaction[pawn.def.index]) || Context.ignoreFaction[pawn.def.index])) || 
                (WorldPawnsTicker.isActive && Finder.timeDilationWorldPawns)));
            return _isValidPawn;
        }
    }
}