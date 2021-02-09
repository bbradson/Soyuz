using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Soyuz.Patches
{
	// Token: 0x02000026 RID: 38
	[SoyuzPatch(typeof(Pawn_RecordsTracker), "RecordsTick", MethodType.Normal, null, null)]
	public class Pawn_RecordsTracker_Patch
	{
		// Token: 0x0600006E RID: 110 RVA: 0x000049C4 File Offset: 0x00002BC4
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			List<CodeInstruction> codes = instructions.MethodReplacer(AccessTools.Method(typeof(Gen), "IsHashIntervalTick", new Type[]
			{
				typeof(Thing),
				typeof(int)
			}, null), AccessTools.Method(typeof(ContextualExtensions), "IsCustomTickInterval", null, null)).ToList<CodeInstruction>();
			foreach (CodeInstruction code in codes)
			{
				if (code.OperandIs(80))
				{
					code.operand = 90;
				}
				yield return code;
			}
			List<CodeInstruction>.Enumerator enumerator = default(List<CodeInstruction>.Enumerator);
			yield break;
			yield break;
		}
	}
}
