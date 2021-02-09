using System;
using RocketMan;

namespace Soyuz.Profiling
{
	// Token: 0x02000011 RID: 17
	public struct PawnNeedRecord
	{
		// Token: 0x06000045 RID: 69 RVA: 0x00003FC7 File Offset: 0x000021C7
		public PawnNeedRecord(float value)
		{
			this.value = value;
			this.dilationEnabled = (Finder.timeDilation && Finder.enabled);
			this.statCachingEnabled = Finder.enabled;
		}

		// Token: 0x04000045 RID: 69
		public float value;

		// Token: 0x04000046 RID: 70
		public bool dilationEnabled;

		// Token: 0x04000047 RID: 71
		public bool statCachingEnabled;
	}
}
