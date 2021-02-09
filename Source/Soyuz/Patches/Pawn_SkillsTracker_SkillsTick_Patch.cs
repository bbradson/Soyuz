using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Soyuz.Patches
{
	// Token: 0x02000027 RID: 39
	[SoyuzPatch(typeof(Pawn_SkillTracker), "SkillsTick", MethodType.Normal, null, null)]
	public class Pawn_SkillsTracker_SkillsTick_Patch
	{
		// Token: 0x06000070 RID: 112 RVA: 0x000049D4 File Offset: 0x00002BD4
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
