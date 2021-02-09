using System;
using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using RocketMan;
using Soyuz.Profiling;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Soyuz
{
	// Token: 0x02000003 RID: 3
	public static class ContextualExtensions
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002064 File Offset: 0x00000264
		private static int DilationRate
		{
			get
			{
				switch (Context.zoomRange)
				{
				case CameraZoomRange.Closest:
					return 60;
				case CameraZoomRange.Close:
					return 20;
				case CameraZoomRange.Middle:
					return 15;
				case CameraZoomRange.Far:
					return 15;
				case CameraZoomRange.Furthest:
					return 7;
				default:
					return 1;
				}
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020A0 File Offset: 0x000002A0
		[Main.OnInitialization]
		public static void Initialize()
		{
			for (int i = 0; i < ContextualExtensions._transformationCache.Length; i++)
			{
				ContextualExtensions._transformationCache[i] = Mathf.Max(Mathf.RoundToInt((float)(i / 30)) * 30, 30);
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020DA File Offset: 0x000002DA
		private static int RoundTransform(int interval)
		{
			if (interval >= 2500)
			{
				return Mathf.Max(Mathf.RoundToInt((float)(interval / 30)) * 30, 30);
			}
			return ContextualExtensions._transformationCache[interval];
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002100 File Offset: 0x00000300
		public static bool OffScreen(this Pawn pawn)
		{
			if (Finder.alwaysDilating)
			{
				return ContextualExtensions.offScreen = true;
			}
			if (ContextualExtensions._pawnScreen == pawn)
			{
				return ContextualExtensions.offScreen;
			}
			ContextualExtensions._pawnScreen = pawn;
			if (Context.curViewRect.Contains(pawn.positionInt))
			{
				return ContextualExtensions.offScreen = false;
			}
			return ContextualExtensions.offScreen = true;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002154 File Offset: 0x00000354
		public static bool IsSkippingTicks(this Pawn pawn)
		{
			if (!Finder.timeDilation)
			{
				return false;
			}
			if (pawn == ContextualExtensions._skippingPawn)
			{
				return ContextualExtensions._isSkippingPawn;
			}
			bool spawned = pawn.Spawned;
			ContextualExtensions._skippingPawn = pawn;
			ContextualExtensions._isSkippingPawn = (ContextualExtensions._pawnTick == pawn && ((!spawned && WorldPawnsTicker.isActive && Finder.timeDilationWorldPawns) || (spawned && (pawn.OffScreen() || Context.zoomRange == CameraZoomRange.Far || Context.zoomRange == CameraZoomRange.Furthest))));
			return ContextualExtensions._isSkippingPawn;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021D0 File Offset: 0x000003D0
		public static void BeginTick(this Pawn pawn)
		{
			ContextualExtensions._pawnTick = pawn;
			if (!Finder.enabled || !Finder.timeDilation || !pawn.IsValidWildlifeOrWorldPawn())
			{
				ContextualExtensions.Skip(pawn);
				return;
			}
			if (Finder.logData && Time.frameCount - Finder.lastFrame < 60)
			{
				ContextualExtensions._stopwatch.Reset();
				ContextualExtensions._stopwatch.Start();
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000222A File Offset: 0x0000042A
		private static void Skip(Pawn pawn)
		{
			ContextualExtensions._isValidPawn = false;
			ContextualExtensions._isSkippingPawn = false;
			ContextualExtensions._skippingPawn = pawn;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002240 File Offset: 0x00000440
		public static void EndTick(this Pawn pawn)
		{
			if (Finder.logData && Time.frameCount - Finder.lastFrame < 60)
			{
				ContextualExtensions._stopwatch.Stop();
				PawnPerformanceModel performanceModel = pawn.GetPerformanceModel();
				performanceModel.AddResult(ContextualExtensions._stopwatch.ElapsedTicks);
				if (GenTicks.TicksGame % 150 == 0)
				{
					Dictionary<Type, PawnNeedModel> needsModel = pawn.GetNeedModels();
					Pawn_NeedsTracker needs = pawn.needs;
					if (((needs != null) ? needs.needs : null) != null)
					{
						Pawn_NeedsTracker needs2 = pawn.needs;
						foreach (Need need in ((needs2 != null) ? needs2.needs : null))
						{
							Type type = need.GetType();
							PawnNeedModel model;
							if (needsModel.TryGetValue(type, out model))
							{
								model.AddResult(need.CurLevelPercentage);
							}
							else
							{
								needsModel[type] = new PawnNeedModel();
							}
						}
					}
					Dictionary<Hediff, PawnHediffModel> hediffModel = pawn.GetHediffModels();
					foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
					{
						PawnHediffModel model2;
						if (hediffModel.TryGetValue(hediff, out model2))
						{
							model2.AddResult(hediff.Severity);
						}
						else
						{
							hediffModel[hediff] = new PawnHediffModel();
						}
					}
				}
			}
			ContextualExtensions._pawnScreen = null;
			ContextualExtensions._pawnTick = null;
			ContextualExtensions._skippingPawn = null;
			ContextualExtensions._validPawn = null;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000023C4 File Offset: 0x000005C4
		public static PawnPerformanceModel GetPerformanceModel(this Pawn pawn)
		{
			PawnPerformanceModel model;
			if (ContextualExtensions.pawnPerformanceModels.TryGetValue(pawn, out model))
			{
				return model;
			}
			return ContextualExtensions.pawnPerformanceModels[pawn] = new PawnPerformanceModel();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000023F8 File Offset: 0x000005F8
		public static Dictionary<Type, PawnNeedModel> GetNeedModels(this Pawn pawn)
		{
			Dictionary<Type, PawnNeedModel> model;
			if (ContextualExtensions.pawnNeedModels.TryGetValue(pawn, out model))
			{
				return model;
			}
			return ContextualExtensions.pawnNeedModels[pawn] = new Dictionary<Type, PawnNeedModel>();
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000242C File Offset: 0x0000062C
		public static Dictionary<Hediff, PawnHediffModel> GetHediffModels(this Pawn pawn)
		{
			Dictionary<Hediff, PawnHediffModel> model;
			if (ContextualExtensions.pawnHediffsModels.TryGetValue(pawn, out model))
			{
				return model;
			}
			return ContextualExtensions.pawnHediffsModels[pawn] = new Dictionary<Hediff, PawnHediffModel>();
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002460 File Offset: 0x00000660
		public static bool ShouldTick(this Pawn pawn)
		{
			int tick = GenTicks.TicksGame;
			ContextualExtensions.shouldTick = ContextualExtensions.ShouldTickInternal(pawn);
			int val;
			if (ContextualExtensions.timers.TryGetValue(pawn.thingIDNumber, out val))
			{
				ContextualExtensions.curDelta = tick - val;
			}
			else
			{
				ContextualExtensions.curDelta = 1;
			}
			if (ContextualExtensions.shouldTick)
			{
				ContextualExtensions.timers[pawn.thingIDNumber] = tick;
			}
			return ContextualExtensions.shouldTick;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000024C0 File Offset: 0x000006C0
		public static bool IsCustomTickInterval(this Thing thing, int interval)
		{
			if (ContextualExtensions._pawnTick == thing && Finder.timeDilation && Finder.enabled)
			{
				if (WorldPawnsTicker.isActive)
				{
					return WorldPawnsTicker.curCycle % WorldPawnsTicker.Transform(interval) == 0;
				}
				if (((Pawn)thing).IsSkippingTicks())
				{
					return (thing.thingIDNumber + GenTicks.TicksGame) % ContextualExtensions.RoundTransform(interval) == 0;
				}
			}
			return thing.IsHashIntervalTick(interval);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002528 File Offset: 0x00000728
		private static bool ShouldTickInternal(Pawn pawn)
		{
			if (!Finder.timeDilation || !Finder.enabled)
			{
				return true;
			}
			if (WorldPawnsTicker.isActive && Finder.timeDilationWorldPawns)
			{
				return true;
			}
			int tick = GenTicks.TicksGame;
			if ((pawn.thingIDNumber + tick) % 30 != 0 && tick % 250 != 0)
			{
				Pawn_JobTracker jobs = pawn.jobs;
				if (((jobs != null) ? jobs.curJob : null) != null)
				{
					Pawn_JobTracker jobs2 = pawn.jobs;
					bool flag;
					if (jobs2 == null)
					{
						flag = false;
					}
					else
					{
						Job curJob = jobs2.curJob;
						int? num = (curJob != null) ? new int?(curJob.expiryInterval) : null;
						int num2 = 0;
						flag = (num.GetValueOrDefault() > num2 & num != null);
					}
					if (flag && (tick - pawn.jobs.curJob.startTick) % (pawn.jobs.curJob.expiryInterval * 2) == 0)
					{
						return true;
					}
				}
				if (pawn.OffScreen())
				{
					return (pawn.thingIDNumber + tick) % ContextualExtensions.DilationRate == 0;
				}
				return (Context.zoomRange != CameraZoomRange.Far && Context.zoomRange != CameraZoomRange.Furthest) || (pawn.thingIDNumber + tick) % 3 == 0;
			}
			return true;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000262C File Offset: 0x0000082C
		public static int GetDeltaT(this Thing thing)
		{
			if (thing == ContextualExtensions._pawnTick)
			{
				return ContextualExtensions.curDelta;
			}
			int val;
			if (ContextualExtensions.timers.TryGetValue(thing.thingIDNumber, out val))
			{
				return GenTicks.TicksGame - val;
			}
			throw new Exception();
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002668 File Offset: 0x00000868
		public static bool IsValidWildlifeOrWorldPawn(this Pawn pawn)
		{
			if (ContextualExtensions._validPawn == pawn)
			{
				return ContextualExtensions._isValidPawn;
			}
			ContextualExtensions._validPawn = pawn;
			int pawnInt = pawn.AsInt();
			return ContextualExtensions._isValidPawn = (!pawn.IsBleeding() && (ContextualExtensions._pawnTick == pawn && !pawn.IsColonist) && (pawnInt == (pawnInt & Context.dilationInts[(int)pawn.def.index]) || (WorldPawnsTicker.isActive && Finder.timeDilationWorldPawns)));
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000026DC File Offset: 0x000008DC
		public static bool IsBleeding(this Pawn pawn)
		{
			bool isBleeding = false;
			Pair<bool, int> store;
			if (ContextualExtensions.bleeding.TryGetValue(pawn.thingIDNumber, out store) && ((GenTicks.TicksGame - store.second < 900 && !store.first) || (GenTicks.TicksGame - store.second < 2500 && store.first)))
			{
				isBleeding = store.first;
			}
			else if (pawn.RaceProps.IsFlesh)
			{
				ContextualExtensions.bleeding[pawn.thingIDNumber] = new Pair<bool, int>(isBleeding = (pawn.health.hediffSet.CalculateBleedRate() > 0f), GenTicks.TicksGame);
			}
			return isBleeding;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002780 File Offset: 0x00000980
		public static int AsInt(this Pawn pawn)
		{
			int val = 1;
			if (pawn.factionInt != null)
			{
				if (pawn.factionInt != Faction.OfPlayerSilentFail)
				{
					val |= 2;
				}
				else
				{
					val |= 4;
				}
			}
			return val;
		}

		// Token: 0x04000006 RID: 6
		private static Pawn _pawnTick;

		// Token: 0x04000007 RID: 7
		private static Pawn _pawnScreen;

		// Token: 0x04000008 RID: 8
		private static bool offScreen;

		// Token: 0x04000009 RID: 9
		private static bool shouldTick;

		// Token: 0x0400000A RID: 10
		private static int curDelta;

		// Token: 0x0400000B RID: 11
		private const int TransformationCacheSize = 2500;

		// Token: 0x0400000C RID: 12
		private static readonly int[] _transformationCache = new int[2500];

		// Token: 0x0400000D RID: 13
		private static readonly Dictionary<int, int> timers = new Dictionary<int, int>();

		// Token: 0x0400000E RID: 14
		private static readonly Dictionary<int, Pair<bool, int>> bleeding = new Dictionary<int, Pair<bool, int>>();

		// Token: 0x0400000F RID: 15
		private static readonly Dictionary<Pawn, PawnPerformanceModel> pawnPerformanceModels = new Dictionary<Pawn, PawnPerformanceModel>();

		// Token: 0x04000010 RID: 16
		private static readonly Dictionary<Pawn, Dictionary<Type, PawnNeedModel>> pawnNeedModels = new Dictionary<Pawn, Dictionary<Type, PawnNeedModel>>();

		// Token: 0x04000011 RID: 17
		private static readonly Dictionary<Pawn, Dictionary<Hediff, PawnHediffModel>> pawnHediffsModels = new Dictionary<Pawn, Dictionary<Hediff, PawnHediffModel>>();

		// Token: 0x04000012 RID: 18
		private static bool _isSkippingPawn = false;

		// Token: 0x04000013 RID: 19
		private static Pawn _skippingPawn = null;

		// Token: 0x04000014 RID: 20
		private static Stopwatch _stopwatch = new Stopwatch();

		// Token: 0x04000015 RID: 21
		private static bool _isValidPawn = false;

		// Token: 0x04000016 RID: 22
		private static Pawn _validPawn = null;
	}
}
