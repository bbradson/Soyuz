using System;
using System.Diagnostics;
using RocketMan;

namespace Soyuz.Profiling
{
	// Token: 0x02000013 RID: 19
	public struct PawnPerformanceRecord
	{
		// Token: 0x06000049 RID: 73 RVA: 0x000042AF File Offset: 0x000024AF
		public PawnPerformanceRecord(float value)
		{
			this.value = value / (float)Stopwatch.Frequency;
			this.dilationEnabled = (Finder.timeDilation && Finder.enabled);
			this.statCachingEnabled = Finder.enabled;
		}

		// Token: 0x04000049 RID: 73
		public float value;

		// Token: 0x0400004A RID: 74
		public bool dilationEnabled;

		// Token: 0x0400004B RID: 75
		public bool statCachingEnabled;
	}
}
