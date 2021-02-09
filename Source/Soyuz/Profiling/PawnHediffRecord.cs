using System;
using RocketMan;

namespace Soyuz.Profiling
{
	// Token: 0x0200000F RID: 15
	public struct PawnHediffRecord
	{
		// Token: 0x06000041 RID: 65 RVA: 0x00003CDF File Offset: 0x00001EDF
		public PawnHediffRecord(float value)
		{
			this.value = value;
			this.dilationEnabled = (Finder.timeDilation && Finder.enabled);
			this.statCachingEnabled = Finder.enabled;
		}

		// Token: 0x04000041 RID: 65
		public float value;

		// Token: 0x04000042 RID: 66
		public bool dilationEnabled;

		// Token: 0x04000043 RID: 67
		public bool statCachingEnabled;
	}
}
