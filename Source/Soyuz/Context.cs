using System;
using System.Collections.Generic;
using Verse;

namespace Soyuz
{
	// Token: 0x02000002 RID: 2
	public static class Context
	{
		// Token: 0x04000001 RID: 1
		public static CameraZoomRange zoomRange;

		// Token: 0x04000002 RID: 2
		public static CellRect curViewRect;

		// Token: 0x04000003 RID: 3
		public static SoyuzSettings settings;

		// Token: 0x04000004 RID: 4
		public static readonly int[] dilationInts = new int[65535];

		// Token: 0x04000005 RID: 5
		public static readonly Dictionary<ThingDef, RaceSettings> dilationByDef = new Dictionary<ThingDef, RaceSettings>();
	}
}
