using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace Soyuz.Patches
{
	// Token: 0x02000022 RID: 34
	[SoyuzPatch(typeof(Pawn_JobTracker), "JobTrackerTick", MethodType.Normal, null, null)]
	public class Pawn_JobTracker_JobTrackerTick_Patch
	{
		// Token: 0x06000064 RID: 100 RVA: 0x00004830 File Offset: 0x00002A30
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			return instructions.MethodReplacer(AccessTools.Method(typeof(Gen), "IsHashIntervalTick", new Type[]
			{
				typeof(Thing),
				typeof(int)
			}, null), AccessTools.Method(typeof(ContextualExtensions), "IsCustomTickInterval", null, null));
		}
	}
}
