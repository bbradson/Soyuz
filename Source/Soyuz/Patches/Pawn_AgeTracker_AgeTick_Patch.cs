using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Soyuz.Patches
{
	// Token: 0x0200001F RID: 31
	[SoyuzPatch(typeof(Pawn_AgeTracker), "AgeTick", MethodType.Normal, null, null)]
	public class Pawn_AgeTracker_AgeTick_Patch
	{
		// Token: 0x0600005E RID: 94 RVA: 0x000047A2 File Offset: 0x000029A2
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			List<CodeInstruction> codes = instructions.ToList<CodeInstruction>();
			Label l = generator.DefineLabel();
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn_AgeTracker), "pawn"));
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ContextualExtensions), "IsValidWildlifeOrWorldPawn", null, null));
			yield return new CodeInstruction(OpCodes.Brfalse_S, l);
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn_AgeTracker), "pawn"));
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ContextualExtensions), "IsSkippingTicks", null, null));
			yield return new CodeInstruction(OpCodes.Brfalse_S, l);
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn_AgeTracker), "ageBiologicalTicksInt"));
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn_AgeTracker), "pawn"));
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ContextualExtensions), "GetDeltaT", null, null));
			yield return new CodeInstruction(OpCodes.Ldc_I4, 1);
			yield return new CodeInstruction(OpCodes.Sub, null);
			yield return new CodeInstruction(OpCodes.Conv_I8, null);
			yield return new CodeInstruction(OpCodes.Add, null);
			yield return new CodeInstruction(OpCodes.Conv_I8, null);
			yield return new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(Pawn_AgeTracker), "ageBiologicalTicksInt"));
			codes[0].labels.Add(l);
			foreach (CodeInstruction code in codes)
			{
				yield return code;
			}
			List<CodeInstruction>.Enumerator enumerator = default(List<CodeInstruction>.Enumerator);
			yield break;
			yield break;
		}
	}
}
