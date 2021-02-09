using System;
using Verse;

namespace Soyuz
{
	// Token: 0x02000008 RID: 8
	public class RaceSettings : IExposable
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002DF4 File Offset: 0x00000FF4
		public int DilationInt
		{
			get
			{
				int val = 0;
				if (this.dilated)
				{
					val |= 1;
				}
				if (this.ignoreFactions)
				{
					val |= 2;
				}
				if (this.ignorePlayerFaction)
				{
					val |= 4;
				}
				return val;
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002DCA File Offset: 0x00000FCA
		public RaceSettings()
		{
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002E28 File Offset: 0x00001028
		public RaceSettings(string pawnDefName)
		{
			this.pawnDefName = pawnDefName;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002E38 File Offset: 0x00001038
		public void ExposeData()
		{
			Scribe_Values.Look<string>(ref this.pawnDefName, "pawnDefName", null, false);
			Scribe_Values.Look<bool>(ref this.dilated, "dilated", false, false);
			Scribe_Values.Look<bool>(ref this.ignoreFactions, "ignoreFactions", false, false);
			Scribe_Values.Look<bool>(ref this.ignorePlayerFaction, "ignorePlayerFaction", false, false);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002E90 File Offset: 0x00001090
		public void ResolveContent()
		{
			ThingDef def;
			if (DefDatabase<ThingDef>.defsByName.TryGetValue(this.pawnDefName, out def))
			{
				this.pawnDef = def;
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002EB8 File Offset: 0x000010B8
		public void Cache()
		{
			Context.dilationByDef[this.pawnDef] = this;
			Context.dilationInts[(int)this.pawnDef.index] = this.DilationInt;
		}

		// Token: 0x0400002A RID: 42
		public ThingDef pawnDef;

		// Token: 0x0400002B RID: 43
		public string pawnDefName;

		// Token: 0x0400002C RID: 44
		public bool dilated;

		// Token: 0x0400002D RID: 45
		public bool ignoreFactions;

		// Token: 0x0400002E RID: 46
		public bool ignorePlayerFaction;
	}
}
