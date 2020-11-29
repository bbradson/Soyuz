using System.Collections.Generic;
using RimWorld.Planet;
using Verse;

namespace Soyuz
{
    public class WorldPawnsTicker : GameComponent
    {
        public const int BucketCount = 60;
        
        private static HashSet<Pawn>[] buckets;
        private static Game game;
        
        public static int curIndex = 0;
        public static int curCycle = 0;        

        public WorldPawnsTicker(Game game)
        {
            TryInitialize();
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            TryInitialize();
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            TryInitialize();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref curIndex, "curIndex", 0);
            if (curIndex >= BucketCount)
            {
                curIndex = 0;
                curCycle = 0;
            }
        }

        public static void TryInitialize()
        {
            if (game != Current.Game)
            {
                curIndex = curCycle = 0;
                game = Current.Game;
                buckets = new HashSet<Pawn>[BucketCount];
                for (int i = 0; i < BucketCount; i++)
                    buckets[i] = new HashSet<Pawn>();
            }
        }

        public static void Rebuild(WorldPawns instance)
        {
            for (int i = 0; i < BucketCount; i++) buckets[i].Clear();
            foreach (Pawn pawn in instance.pawnsAlive) Register(pawn);
        }

        public static void Register(Pawn pawn)
        {
            var index = GetBucket(pawn);
            buckets[index].Add(pawn);
        }

        public static void Deregister(Pawn pawn)
        {
            var index = GetBucket(pawn);
            buckets[index].Remove(pawn);
        }

        public static HashSet<Pawn> GetPawns()
        {
            var result = buckets[curIndex];
            curIndex = curIndex + 1;
            if (curIndex >= BucketCount)
            {
                curIndex = 0;
                curCycle += 1;
            }
            return result;
        }

        private static int GetBucket(Pawn pawn)
        {
            int hash;
            unchecked
            {
                hash = pawn.GetHashCode() + pawn.thingIDNumber;
                if (hash < 0) hash *= -1;
            }
            return hash % BucketCount;
        }
    }
}