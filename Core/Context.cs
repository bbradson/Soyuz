using System.Collections.Generic;
using Verse;

namespace Soyuz
{
    public static class Context
    {
        public static CameraZoomRange zoomRange;
        public static CellRect curViewRect;
        public static SoyuzSettings settings;

        public static bool[] dilatedRaces = new bool[ushort.MaxValue];
        public static bool[] ignoreFaction = new bool[ushort.MaxValue];

        public static RocketMan.RocketMod rocketMod;
    }
}