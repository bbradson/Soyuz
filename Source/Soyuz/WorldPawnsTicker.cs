using System;
using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Soyuz
{
	// Token: 0x0200000C RID: 12
	public class WorldPawnsTicker : GameComponent
	{
		// Token: 0x0600002D RID: 45 RVA: 0x00003614 File Offset: 0x00001814
		public WorldPawnsTicker(Game game)
		{
			WorldPawnsTicker.TryInitialize();
			for (int i = 0; i < WorldPawnsTicker._transformationCache.Length; i++)
			{
				WorldPawnsTicker._transformationCache[i] = (int)Mathf.Max((float)i / 30f, 1f);
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003658 File Offset: 0x00001858
		public override void StartedNewGame()
		{
			base.StartedNewGame();
			WorldPawnsTicker.TryInitialize();
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003665 File Offset: 0x00001865
		public override void LoadedGame()
		{
			base.LoadedGame();
			WorldPawnsTicker.TryInitialize();
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003672 File Offset: 0x00001872
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref WorldPawnsTicker.curIndex, "curIndex", 0, false);
			if (WorldPawnsTicker.curIndex >= 30)
			{
				WorldPawnsTicker.curIndex = 0;
				WorldPawnsTicker.curCycle = 0;
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000036A0 File Offset: 0x000018A0
		public static int Transform(int interval)
		{
			if (interval >= 2500)
			{
				return (int)Mathf.Max((float)interval / 30f, 1f);
			}
			return WorldPawnsTicker._transformationCache[interval];
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000036C8 File Offset: 0x000018C8
		public static void TryInitialize()
		{
			if (WorldPawnsTicker.game != Current.Game || WorldPawnsTicker.buckets == null)
			{
				WorldPawnsTicker.curIndex = (WorldPawnsTicker.curCycle = 0);
				WorldPawnsTicker.game = Current.Game;
				WorldPawnsTicker.buckets = new HashSet<Pawn>[30];
				for (int i = 0; i < 30; i++)
				{
					WorldPawnsTicker.buckets[i] = new HashSet<Pawn>();
				}
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003724 File Offset: 0x00001924
		public static void Rebuild(WorldPawns instance)
		{
			WorldPawnsTicker.curCycle = 0;
			WorldPawnsTicker.curIndex = 0;
			for (int i = 0; i < 30; i++)
			{
				WorldPawnsTicker.buckets[i].Clear();
			}
			foreach (Pawn pawn in instance.pawnsAlive)
			{
				WorldPawnsTicker.Register(pawn);
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000379C File Offset: 0x0000199C
		public static void Register(Pawn pawn)
		{
			int index = WorldPawnsTicker.GetBucket(pawn);
			if (WorldPawnsTicker.buckets == null)
			{
				WorldPawnsTicker.TryInitialize();
			}
			if (WorldPawnsTicker.buckets[index] == null)
			{
				WorldPawnsTicker.buckets[index] = new HashSet<Pawn>();
			}
			WorldPawnsTicker.buckets[index].Add(pawn);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000037E0 File Offset: 0x000019E0
		public static void Deregister(Pawn pawn)
		{
			int index = WorldPawnsTicker.GetBucket(pawn);
			if (WorldPawnsTicker.buckets[index] == null)
			{
				return;
			}
			WorldPawnsTicker.buckets[index].Remove(pawn);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000380C File Offset: 0x00001A0C
		public static HashSet<Pawn> GetPawns()
		{
			HashSet<Pawn> result = WorldPawnsTicker.buckets[WorldPawnsTicker.curIndex];
			WorldPawnsTicker.curIndex++;
			if (WorldPawnsTicker.curIndex >= 30)
			{
				WorldPawnsTicker.curIndex = 0;
				WorldPawnsTicker.curCycle++;
			}
			return result;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003850 File Offset: 0x00001A50
		private static int GetBucket(Pawn pawn)
		{
			int hash = pawn.GetHashCode();
			if (hash < 0)
			{
				hash *= -1;
			}
			return hash % 30;
		}

		// Token: 0x04000036 RID: 54
		public const int BucketCount = 30;

		// Token: 0x04000037 RID: 55
		private const int TransformationCacheSize = 2500;

		// Token: 0x04000038 RID: 56
		private static int[] _transformationCache = new int[2500];

		// Token: 0x04000039 RID: 57
		private static HashSet<Pawn>[] buckets;

		// Token: 0x0400003A RID: 58
		private static Game game;

		// Token: 0x0400003B RID: 59
		public static int curIndex = 0;

		// Token: 0x0400003C RID: 60
		public static int curCycle = 0;

		// Token: 0x0400003D RID: 61
		public static bool isActive = false;
	}
}
