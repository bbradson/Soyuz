using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld.Planet;
using RocketMan;
using Verse;

namespace Soyuz.Patches
{
	// Token: 0x02000028 RID: 40
	public class WorldPawns_Patch
	{
		// Token: 0x0200003E RID: 62
		[SoyuzPatch(typeof(WorldPawns), "ExposeData", MethodType.Normal, null, null)]
		public static class WorldPawns_ExposeData_Patch
		{
			// Token: 0x060000C9 RID: 201 RVA: 0x00006DB3 File Offset: 0x00004FB3
			public static void Postfix(WorldPawns __instance)
			{
				WorldPawnsTicker.Rebuild(__instance);
			}
		}

		// Token: 0x0200003F RID: 63
		[SoyuzPatch(typeof(WorldPawns), "DoMothballProcessing", MethodType.Normal, null, null)]
		public static class WorldPawns_DoMothballProcessing_Patch
		{
			// Token: 0x060000CA RID: 202 RVA: 0x00006DB3 File Offset: 0x00004FB3
			public static void Postfix(WorldPawns __instance)
			{
				WorldPawnsTicker.Rebuild(__instance);
			}
		}

		// Token: 0x02000040 RID: 64
		[SoyuzPatch(typeof(WorldPawns), "AddPawn", MethodType.Normal, null, null)]
		public static class WorldPawns_AddPawn_Patch
		{
			// Token: 0x060000CB RID: 203 RVA: 0x00006DBB File Offset: 0x00004FBB
			public static void Prefix(Pawn p)
			{
				WorldPawnsTicker.Register(p);
			}
		}

		// Token: 0x02000041 RID: 65
		[SoyuzPatch(typeof(WorldPawns), "Notify_PawnDestroyed", MethodType.Normal, null, null)]
		public static class WorldPawns_Notify_PawnDestroyed_Patch
		{
			// Token: 0x060000CC RID: 204 RVA: 0x00006DC3 File Offset: 0x00004FC3
			public static void Prefix(Pawn p)
			{
				WorldPawnsTicker.Deregister(p);
			}
		}

		// Token: 0x02000042 RID: 66
		[SoyuzPatch(typeof(WorldPawns), "RemovePawn", MethodType.Normal, null, null)]
		public static class WorldPawns_RemovePawn_Patch
		{
			// Token: 0x060000CD RID: 205 RVA: 0x00006DC3 File Offset: 0x00004FC3
			public static void Prefix(Pawn p)
			{
				WorldPawnsTicker.Deregister(p);
			}
		}

		// Token: 0x02000043 RID: 67
		[SoyuzPatch(typeof(WorldPawns), "WorldPawnsTick", MethodType.Normal, null, null)]
		public static class WorldPawns_WorldPawnsTick_Patch
		{
			// Token: 0x060000CE RID: 206 RVA: 0x00006DCB File Offset: 0x00004FCB
			public static void Prefix()
			{
				WorldPawnsTicker.isActive = true;
			}

			// Token: 0x060000CF RID: 207 RVA: 0x00006DD3 File Offset: 0x00004FD3
			public static void Postfix()
			{
				WorldPawnsTicker.isActive = false;
			}

			// Token: 0x060000D0 RID: 208 RVA: 0x00006DDB File Offset: 0x00004FDB
			public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				List<CodeInstruction> codes = instructions.ToList<CodeInstruction>();
				bool finished = false;
				int num;
				for (int i = 0; i < codes.Count; i = num + 1)
				{
					if (!finished && codes[i].OperandIs(WorldPawns_Patch.WorldPawns_WorldPawnsTick_Patch.fAlivePawns))
					{
						finished = true;
						yield return codes[i];
						yield return new CodeInstruction(OpCodes.Ldarg_0, null);
						yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WorldPawns_Patch.WorldPawns_WorldPawnsTick_Patch), "GetAlivePawns", null, null));
					}
					else
					{
						yield return codes[i];
					}
					num = i;
				}
				yield break;
			}

			// Token: 0x060000D1 RID: 209 RVA: 0x00006DEC File Offset: 0x00004FEC
			private static HashSet<Pawn> GetAlivePawns(HashSet<Pawn> pawns, WorldPawns instance)
			{
				if (!Finder.timeDilation || !Finder.timeDilationWorldPawns || !Finder.enabled)
				{
					return pawns;
				}
				HashSet<Pawn> result = WorldPawnsTicker.GetPawns();
				if (Finder.debug && Finder.flashDilatedPawns)
				{
					Log.Message(string.Format("ROCKETMAN: ticker bucket of {0} from {1} and index is {2}", result.Count, pawns.Count, WorldPawnsTicker.curIndex), false);
				}
				return result;
			}

			// Token: 0x04000098 RID: 152
			private static readonly FieldInfo fAlivePawns = AccessTools.Field(typeof(WorldPawns), "pawnsAlive");
		}
	}
}
