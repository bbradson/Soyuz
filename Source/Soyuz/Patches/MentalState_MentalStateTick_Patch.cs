﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace Soyuz.Patches
{
	// Token: 0x0200001D RID: 29
	[SoyuzPatch(typeof(MentalState), "MentalStateTick", MethodType.Normal, null, null)]
	public class MentalState_MentalStateTick_Patch
	{
		// Token: 0x0600005A RID: 90 RVA: 0x00004710 File Offset: 0x00002910
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
