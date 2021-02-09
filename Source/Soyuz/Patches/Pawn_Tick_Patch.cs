using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RocketMan;
using Verse;

namespace Soyuz.Patches
{
	// Token: 0x02000025 RID: 37
	[SoyuzPatch(typeof(Pawn), "Tick", MethodType.Normal, null, null)]
	public class Pawn_Tick_Patch
	{
		// Token: 0x0600006A RID: 106 RVA: 0x000048FE File Offset: 0x00002AFE
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			List<CodeInstruction> codes = instructions.ToList<CodeInstruction>();
			bool finished = false;
			LocalBuilder localSkipper = generator.DeclareLocal(typeof(bool));
			Label l = generator.DefineLabel();
			yield return new CodeInstruction(OpCodes.Ldarg_0, null);
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ContextualExtensions), "BeginTick", null, null));
			int num;
			for (int i = 0; i < codes.Count; i = num + 1)
			{
				if (!finished && codes[i].OperandIs(Pawn_Tick_Patch.mSuspended))
				{
					finished = true;
					yield return codes[i];
					yield return new CodeInstruction(OpCodes.Dup, null);
					yield return new CodeInstruction(OpCodes.Brtrue_S, l);
					yield return new CodeInstruction(OpCodes.Ldarg_0, null);
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ContextualExtensions), "IsValidWildlifeOrWorldPawn", null, null));
					yield return new CodeInstruction(OpCodes.Brfalse_S, l);
					yield return new CodeInstruction(OpCodes.Ldarg_0, null);
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ContextualExtensions), "ShouldTick", null, null));
					yield return new CodeInstruction(OpCodes.Brtrue_S, l);
					yield return new CodeInstruction(OpCodes.Pop, null);
					yield return new CodeInstruction(OpCodes.Ldarg_0, null);
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn_Tick_Patch), "TickExtras", null, null));
					yield return new CodeInstruction(OpCodes.Ldc_I4_1, null);
					codes[i + 1].labels.Add(l);
				}
				else
				{
					if (codes[i].opcode == OpCodes.Ret)
					{
						yield return new CodeInstruction(OpCodes.Ldarg_0, null)
						{
							labels = codes[i].labels
						};
						yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ContextualExtensions), "EndTick", null, null));
						codes[i].labels = new List<Label>();
					}
					yield return codes[i];
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004918 File Offset: 0x00002B18
		private static void TickExtras(Pawn pawn)
		{
			if (pawn.Spawned)
			{
				Pawn_StanceTracker stances = pawn.stances;
				if (stances != null)
				{
					stances.StanceTrackerTick();
				}
				if (!pawn.OffScreen())
				{
					Pawn_DrawTracker drawer = pawn.drawer;
					if (drawer != null)
					{
						drawer.DrawTrackerTick();
					}
					Pawn_RotationTracker rotationTracker = pawn.rotationTracker;
					if (rotationTracker != null)
					{
						rotationTracker.RotationTrackerTick();
					}
				}
			}
			if (Finder.flashDilatedPawns && pawn.Spawned)
			{
				pawn.Map.debugDrawer.FlashCell(pawn.positionInt, 0.05f, string.Format("{0}", pawn.OffScreen()), 100);
			}
		}

		// Token: 0x0400004D RID: 77
		private static MethodInfo mSuspended = AccessTools.PropertyGetter(typeof(Thing), "Suspended");
	}
}
