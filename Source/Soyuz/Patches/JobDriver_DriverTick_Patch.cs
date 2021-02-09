using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace Soyuz.Patches
{
	// Token: 0x0200001B RID: 27
	[SoyuzPatch(typeof(JobDriver), "DriverTick", MethodType.Normal, null, null)]
	public class JobDriver_DriverTick_Patch
	{
		// Token: 0x06000056 RID: 86 RVA: 0x00004696 File Offset: 0x00002896
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			List<CodeInstruction> codes = instructions.MethodReplacer(AccessTools.Method(typeof(Gen), "IsHashIntervalTick", new Type[]
			{
				typeof(Thing),
				typeof(int)
			}, null), AccessTools.Method(typeof(ContextualExtensions), "IsCustomTickInterval", null, null)).ToList<CodeInstruction>();
			Label l = generator.DefineLabel();
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(JobDriver), "pawn"));
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ContextualExtensions), "IsValidWildlifeOrWorldPawn", null, null));
			yield return new CodeInstruction(OpCodes.Brfalse_S, l);
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(JobDriver), "pawn"));
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ContextualExtensions), "IsSkippingTicks", null, null));
			yield return new CodeInstruction(OpCodes.Brfalse_S, l);
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(JobDriver), "ticksLeftThisToil"));
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(JobDriver), "pawn"));
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ContextualExtensions), "GetDeltaT", null, null));
			yield return new CodeInstruction(OpCodes.Ldc_I4, 1);
			yield return new CodeInstruction(OpCodes.Sub, null);
			yield return new CodeInstruction(OpCodes.Sub, null);
			yield return new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(JobDriver), "ticksLeftThisToil"));
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
