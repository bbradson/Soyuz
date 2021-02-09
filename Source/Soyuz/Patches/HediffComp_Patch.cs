using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace Soyuz.Patches
{
	// Token: 0x02000016 RID: 22
	public class HediffComp_Patch
	{
		// Token: 0x0200002E RID: 46
		[SoyuzPatch]
		public static class HediffComp_GenHashInterval_Replacement
		{
			// Token: 0x06000081 RID: 129 RVA: 0x00004A90 File Offset: 0x00002C90
			public static IEnumerable<MethodBase> TargetMethods()
			{
				yield return AccessTools.Method(typeof(Hediff), "Tick", null, null);
				foreach (Type type in typeof(Hediff).AllSubclassesNonAbstract())
				{
					MethodBase method = type.GetMethod("Tick");
					if (method != null && method.HasMethodBody())
					{
						yield return method;
					}
				}
				IEnumerator<Type> enumerator = null;
				yield return AccessTools.Method(typeof(HediffComp), "CompPostTick", null, null);
				foreach (Type type2 in typeof(HediffComp).AllSubclassesNonAbstract())
				{
					MethodBase method = type2.GetMethod("CompPostTick");
					if (method != null && method.HasMethodBody())
					{
						yield return method;
					}
				}
				enumerator = null;
				yield break;
				yield break;
			}

			// Token: 0x06000082 RID: 130 RVA: 0x00004A9C File Offset: 0x00002C9C
			public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				return instructions.MethodReplacer(AccessTools.Method(typeof(Gen), "IsHashIntervalTick", new Type[]
				{
					typeof(Thing),
					typeof(int)
				}, null), AccessTools.Method(typeof(ContextualExtensions), "IsCustomTickInterval", null, null));
			}
		}

		// Token: 0x0200002F RID: 47
		[SoyuzPatch(typeof(HediffComp_ChanceToRemove), "CompPostTick", MethodType.Normal, null, null)]
		public static class HediffComp_ChanceToRemove_Patch
		{
			// Token: 0x06000083 RID: 131 RVA: 0x00004AFC File Offset: 0x00002CFC
			public static void Prefix(HediffComp_ChanceToRemove __instance)
			{
				if (__instance.parent.pawn.IsSkippingTicks() && __instance.parent.pawn.IsValidWildlifeOrWorldPawn())
				{
					__instance.currentInterval -= __instance.parent.pawn.GetDeltaT();
				}
			}
		}

		// Token: 0x02000030 RID: 48
		[SoyuzPatch(typeof(HediffComp_ChangeNeed), "CompPostTick", MethodType.Normal, null, null)]
		public static class HediffComp_ChangeNeed_Patch
		{
			// Token: 0x06000084 RID: 132 RVA: 0x00004B4C File Offset: 0x00002D4C
			public static void Prefix(HediffComp_ChangeNeed __instance)
			{
				if (__instance.parent.pawn.IsSkippingTicks() && __instance.parent.pawn.IsValidWildlifeOrWorldPawn() && __instance.Need != null)
				{
					__instance.Need.CurLevelPercentage += __instance.Props.percentPerDay / 60000f * (float)(__instance.parent.pawn.GetDeltaT() - 1);
				}
			}
		}

		// Token: 0x02000031 RID: 49
		[SoyuzPatch(typeof(HediffComp_Disappears), "CompPostTick", MethodType.Normal, null, null)]
		public static class HediffComp_Disappears_Patch
		{
			// Token: 0x06000085 RID: 133 RVA: 0x00004BBC File Offset: 0x00002DBC
			public static void Prefix(HediffComp_Disappears __instance)
			{
				if (__instance.parent.pawn.IsSkippingTicks() && __instance.parent.pawn.IsValidWildlifeOrWorldPawn())
				{
					__instance.ticksToDisappear -= __instance.parent.pawn.GetDeltaT() - 1;
				}
			}
		}

		// Token: 0x02000032 RID: 50
		[SoyuzPatch(typeof(HediffComp_Discoverable), "CompPostTick", MethodType.Normal, null, null)]
		public static class HediffComp_Discoverable_Patch
		{
			// Token: 0x06000086 RID: 134 RVA: 0x00004C0C File Offset: 0x00002E0C
			public static bool Prefix(HediffComp_Discoverable __instance)
			{
				if (__instance.parent.pawn.IsSkippingTicks() && __instance.parent.pawn.IsValidWildlifeOrWorldPawn() && (Find.TickManager.TicksGame + __instance.parent.pawn.thingIDNumber) % 90 == 0)
				{
					__instance.CheckDiscovered();
				}
				return false;
			}
		}

		// Token: 0x02000033 RID: 51
		[SoyuzPatch(typeof(HediffComp_HealPermanentWounds), "CompPostTick", MethodType.Normal, null, null)]
		public static class HediffComp_HealPermanentWounds_Patch
		{
			// Token: 0x06000087 RID: 135 RVA: 0x00004C64 File Offset: 0x00002E64
			public static void Prefix(HediffComp_HealPermanentWounds __instance)
			{
				if (__instance.parent.pawn.IsSkippingTicks() && __instance.parent.pawn.IsValidWildlifeOrWorldPawn())
				{
					__instance.ticksToHeal -= __instance.parent.pawn.GetDeltaT() - 1;
				}
			}
		}

		// Token: 0x02000034 RID: 52
		[SoyuzPatch(typeof(HediffComp_Infecter), "CompPostTick", MethodType.Normal, null, null)]
		public static class HediffComp_Infecter_Patch
		{
			// Token: 0x06000088 RID: 136 RVA: 0x00004CB4 File Offset: 0x00002EB4
			public static void Prefix(HediffComp_Infecter __instance)
			{
				if (__instance.parent.pawn.IsSkippingTicks() && __instance.parent.pawn.IsValidWildlifeOrWorldPawn())
				{
					__instance.ticksUntilInfect -= __instance.parent.pawn.GetDeltaT() - 1;
				}
			}
		}

		// Token: 0x02000035 RID: 53
		[SoyuzPatch(typeof(HediffComp_SelfHeal), "CompPostTick", MethodType.Normal, null, null)]
		public static class HediffComp_SelfHeal_Patch
		{
			// Token: 0x06000089 RID: 137 RVA: 0x00004D04 File Offset: 0x00002F04
			public static void Prefix(HediffComp_SelfHeal __instance)
			{
				if (__instance.parent.pawn.IsSkippingTicks() && __instance.parent.pawn.IsValidWildlifeOrWorldPawn())
				{
					__instance.ticksSinceHeal += __instance.parent.pawn.GetDeltaT() - 1;
				}
			}
		}

		// Token: 0x02000036 RID: 54
		[SoyuzPatch(typeof(HediffComp_TendDuration), "CompPostTick", MethodType.Normal, null, null)]
		public static class HediffComp_TendDuration_Patch
		{
			// Token: 0x0600008A RID: 138 RVA: 0x00004D54 File Offset: 0x00002F54
			public static void Prefix(HediffComp_TendDuration __instance)
			{
				if (__instance.parent.pawn.IsSkippingTicks() && __instance.parent.pawn.IsValidWildlifeOrWorldPawn() && !__instance.TProps.TendIsPermanent)
				{
					__instance.tendTicksLeft -= __instance.parent.pawn.GetDeltaT() - 1;
				}
			}
		}
	}
}
