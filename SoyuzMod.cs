using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using HugsLib;
using RocketMan;
using Verse;

namespace Soyuz
{
    [StaticConstructorOnStartup]
    public class SoyuzMod : Mod
    {
        static SoyuzMod()
        { 
        }

        public SoyuzMod(ModContentPack content) : base(content)
        {
        }
    }
}