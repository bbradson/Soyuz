using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace Soyuz.Patches
{
	// Token: 0x02000023 RID: 35
	[SoyuzPatch(typeof(Pawn_MindState), "MindStateTick", MethodType.Normal, null, null)]
	public class Pawn_MindState_MindStateTick_Patch
	{
		// Token: 0x06000066 RID: 102 RVA: 0x0000488E File Offset: 0x00002A8E
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			List<CodeInstruction> codes = instructions.MethodReplacer(AccessTools.Method(typeof(Gen), "IsHashIntervalTick", new Type[]
			{
				typeof(Thing),
				typeof(int)
			}, null), AccessTools.Method(typeof(ContextualExtensions), "IsCustomTickInterval", null, null)).ToList<CodeInstruction>();
			foreach (CodeInstruction code in codes)
			{
				if (code.OperandIs(123))
				{
					code.operand = 120;
				}
				yield return code;
			}
			List<CodeInstruction>.Enumerator enumerator = default(List<CodeInstruction>.Enumerator);
			yield break;
			yield break;
		}
	}
}
