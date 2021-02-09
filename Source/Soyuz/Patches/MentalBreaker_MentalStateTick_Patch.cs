using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace Soyuz.Patches
{
	// Token: 0x0200001C RID: 28
	[SoyuzPatch(typeof(MentalState), "MentalStateTick", MethodType.Normal, null, null)]
	public class MentalBreaker_MentalStateTick_Patch
	{
		// Token: 0x06000058 RID: 88 RVA: 0x000046B0 File Offset: 0x000028B0
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
