using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Soyuz.Patches
{
	// Token: 0x02000021 RID: 33
	[SoyuzPatch(typeof(Pawn_HealthTracker), "HealthTick", MethodType.Normal, null, null)]
	public class Pawn_HealthTracker_Tick_Patch
	{
		// Token: 0x06000062 RID: 98 RVA: 0x000047D0 File Offset: 0x000029D0
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
