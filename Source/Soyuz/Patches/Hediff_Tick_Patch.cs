using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Soyuz.Patches
{
	// Token: 0x02000018 RID: 24
	[SoyuzPatch(typeof(Hediff), "Tick", MethodType.Normal, null, null)]
	public static class Hediff_Tick_Patch
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00004604 File Offset: 0x00002804
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			List<CodeInstruction> codes = instructions.MethodReplacer(AccessTools.Method(typeof(Gen), "IsHashIntervalTick", new Type[]
			{
				typeof(Thing),
				typeof(int)
			}, null), AccessTools.Method(typeof(ContextualExtensions), "IsCustomTickInterval", null, null)).ToList<CodeInstruction>();
			Label l = generator.DefineLabel();
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Hediff), "pawn"));
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ContextualExtensions), "IsValidWildlifeOrWorldPawn", null, null));
			yield return new CodeInstruction(OpCodes.Brfalse_S, l);
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Hediff), "pawn"));
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ContextualExtensions), "IsSkippingTicks", null, null));
			yield return new CodeInstruction(OpCodes.Brfalse_S, l);
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Hediff), "ageTicks"));
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Hediff), "pawn"));
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ContextualExtensions), "GetDeltaT", null, null));
			yield return new CodeInstruction(OpCodes.Ldc_I4, 1);
			yield return new CodeInstruction(OpCodes.Sub, null);
			yield return new CodeInstruction(OpCodes.Add, null);
			yield return new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(Hediff), "ageTicks"));
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
