using HarmonyLib;
using RocketMan;
using Verse;

namespace Soyuz
{
    [StaticConstructorOnStartup]
    public class SoyuzMod : Mod
    {
        private static readonly Harmony harmony = new Harmony(Finder.HarmonyID + ".Soyuz");

        static SoyuzMod()
        {
            harmony.PatchAll();
        }

        public SoyuzMod(ModContentPack content) : base(content)
        {
        }

        public override string SettingsCategory()
        {
            return "Soyuz settings";
        }
    }
}