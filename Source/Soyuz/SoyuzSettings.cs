using System;
using System.Collections.Generic;
using Verse;

namespace Soyuz
{
	// Token: 0x02000009 RID: 9
	public class SoyuzSettings : IExposable
	{
		// Token: 0x06000026 RID: 38 RVA: 0x00002EE2 File Offset: 0x000010E2
		public void ExposeData()
		{
			Scribe_Collections.Look<RaceSettings>(ref this.raceSettings, "raceSettings", LookMode.Deep, Array.Empty<object>());
			if (this.raceSettings == null)
			{
				this.raceSettings = new List<RaceSettings>();
			}
		}

		// Token: 0x0400002F RID: 47
		public List<RaceSettings> raceSettings = new List<RaceSettings>();
	}
}
