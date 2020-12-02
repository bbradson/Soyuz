using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using RocketMan;
using RocketMan.Tabs;
using UnityEngine;
using Verse;

namespace Soyuz
{
    public class RaceSettings : IExposable
    {
        public ThingDef pawnDef;
        
        public bool dilated;
        public bool ignoreFaction;
        
        public void ExposeData()
        { 
            Scribe_Defs.Look(ref pawnDef,"pawnDef");
            Scribe_Values.Look(ref dilated,"dilated");
            Scribe_Values.Look(ref ignoreFaction,"ignoreFaction");
        }
    }
    
    public class SoyuzSettings : IExposable
    {
        public List<RaceSettings> raceSettings = new List<RaceSettings>();
        
        public void ExposeData()
        {
            Scribe_Collections.Look(ref raceSettings, "raceSettings", LookMode.Deep);
            if (raceSettings == null) raceSettings = new List<RaceSettings>();
        }
    }

    [SoyuzPatch(typeof(TabContent_Soyuz), nameof(TabContent_Soyuz.DoExtras))]
    public class SoyuzSettingsGUIUtility
    {
        private static Vector2 scrollPosition = Vector2.zero;
        private static Rect viewRect = Rect.zero;
        private static Listing_Standard standard = new Listing_Standard();

        private static RaceSettings curSelection;
        
        public static void Postfix(Rect rect)
        {
            Text.CurFontStyle.fontStyle = FontStyle.Bold;
            Widgets.Label(rect.TopPartPixels(25), "Dilated races");
            Text.CurFontStyle.fontStyle = FontStyle.Normal;
            var font = Text.Font;
            Text.Font = GameFont.Tiny;
            rect.yMin += 25;
            if (curSelection != null)
            {
                var height = 108;
                var selectionRect = rect.TopPartPixels(height);
                Widgets.DrawMenuSection(selectionRect);
                Text.Font = GameFont.Tiny;
                Widgets.DefLabelWithIcon(selectionRect.TopPartPixels(54), curSelection.pawnDef);
                selectionRect.yMin += 54;
                standard.Begin(selectionRect.ContractedBy(3));
                Text.Font = GameFont.Tiny;
                standard.CheckboxLabeled($"Enable dilation for {curSelection.pawnDef.label}", ref curSelection.dilated);
                standard.CheckboxLabeled($"Enable dilation regardless of faction", ref curSelection.ignoreFaction);
                standard.End();
                rect.yMin += height + 8;
            }
            Widgets.DrawMenuSection(rect);
            rect = rect.ContractedBy(2);
            viewRect.size = rect.size;
            viewRect.height = 60 * Context.settings.raceSettings.Count;
            viewRect.width -= 15;
            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect.AtZero());
            Rect curRect = viewRect.TopPartPixels(54);
            curRect.width -= 15;
            var counter = 0;
            foreach (var setting in Context.settings.raceSettings)
            {
                counter++;
                if(counter% 2 == 0)
                    Widgets.DrawBoxSolid(curRect, new Color(0.1f,0.1f,0.1f, 0.2f));
                Widgets.DrawHighlightIfMouseover(curRect);
                Widgets.DefLabelWithIcon(curRect.ContractedBy(3), setting.pawnDef);
                if (Widgets.ButtonInvisible(curRect))
                {
                    curSelection = setting;
                    break;
                }
                curRect.y += 58;
            }

            Text.Font = font;
            Widgets.EndScrollView();
            SoyuzSettingsUtility.ParseSettings();
            Context.rocketMod.WriteSettings();
        }
    }

    [SoyuzPatch(typeof(RocketMod.RocketModSettings), nameof(RocketMod.RocketModSettings.ExposeData))]
    public class SoyuzSettingsScribingUtility
    {
        public static void Postfix()
        {
            Scribe_Deep.Look(ref Context.settings, "SoyuzSettings");
            if (Context.settings == null)
            {
                Context.settings = new SoyuzSettings();
                SoyuzSettingsUtility.CreateSettings();
            }
            SoyuzSettingsUtility.ParseSettings();
        }
    }

    public class SoyuzSettingsUtility
    {
        private static List<ThingDef> pawnDefs;
        
        [RocketMan.Main.OnDefsLoaded]
        public static void LoadSettings()
        {
            pawnDefs = DefDatabase<ThingDef>.AllDefs.Where(def => def.race != null).ToList();
            Context.rocketMod = LoadedModManager.GetMod<RocketMod>();
            Context.rocketMod.GetSettings<RocketMod.RocketModSettings>();
            ParseSettings();
        }

        public static void ParseSettings()
        {
            if (Context.settings == null)
                Context.settings = new SoyuzSettings();
            if (Context.settings.raceSettings.Count == 0)
                CreateSettings();
            Context.ignoreFaction = new bool[ushort.MaxValue];
            Context.dilatedRaces = new bool[ushort.MaxValue];
            foreach (var setting in Context.settings.raceSettings)
            {
                if (setting.pawnDef == null)
                    continue;
                Context.ignoreFaction[setting.pawnDef.index] = setting.ignoreFaction;
                Context.dilatedRaces[setting.pawnDef.index] = setting.dilated;
            }
        }

        public static void CreateSettings()
        {
            Context.settings.raceSettings.Clear();
            foreach (var def in pawnDefs)
            {
                if(def.race.Animal && !def.race.Humanlike && !def.race.IsMechanoid)
                    Context.settings.raceSettings.Add(new RaceSettings()
                    {
                        pawnDef = def, 
                        dilated = true,
                        ignoreFaction = false
                    });
                else 
                    Context.settings.raceSettings.Add(new RaceSettings()
                    {
                        pawnDef = def, 
                        dilated = false,
                        ignoreFaction = false
                    });
            }
        }
    }
}