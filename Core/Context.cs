using System;
using System.Collections.Generic;
using RocketMan;
using Soyuz.Profiling;
using Verse;

namespace Soyuz
{
    public static class Context
    {
        public static CameraZoomRange zoomRange;
        public static CellRect curViewRect;
        public static SoyuzSettings settings;
        public static SoyuzMod soyuz;

        public static readonly int[] dilationInts = new int[ushort.MaxValue];
        public static readonly Dictionary<ThingDef, RaceSettings> dilationByDef =
            new Dictionary<ThingDef, RaceSettings>();
    }
}