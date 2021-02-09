using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Soyuz.Patches
{
	// Token: 0x02000024 RID: 36
	[SoyuzPatch(typeof(Pawn_NeedsTracker), "NeedsTrackerTick", MethodType.Normal, null, null)]
	public class Pawn_NeedsTracker_NeedTrackerTick_Patch
	{
		// Token: 0x06000068 RID: 104 RVA: 0x000048A0 File Offset: 0x00002AA0
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
