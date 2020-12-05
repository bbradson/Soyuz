using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using HarmonyLib;
using RimWorld;
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
        public bool ignoreFactions;
        public bool ignorePlayerFaction;
        
        public int DilationInt
        {
            get
            {
                int val = 0;
                if (dilated) val = val | 1;
                if (ignoreFactions) val = val | 2;
                if (ignorePlayerFaction) val = val | 4;
                return val;
            }
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref pawnDef, "pawnDef");
            Scribe_Values.Look(ref dilated, "dilated");
            Scribe_Values.Look(ref ignoreFactions, "ignoreFactions");
            Scribe_Values.Look(ref ignorePlayerFaction, "ignorePlayerFaction");
        }

        public void Cache()
        {
            Context.dilationByDef[pawnDef] = this;
            Context.dilationInts[pawnDef.index] = DilationInt;
        }
    }
    
    public class SoyuzSettings : ModSettings
    {
        public List<RaceSettings> raceSettings = new List<RaceSettings>();
        
        public override void ExposeData()
        {
            Scribe_Collections.Look(ref raceSettings, "raceSettings", LookMode.Deep);
            if (raceSettings == null) raceSettings = new List<RaceSettings>();
        }
    }

    [SoyuzPatch(typeof(TabContent_Debug), nameof(TabContent_Debug.DoExtras))]
    public static class SoyuzDebuggingGUIUtility
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void Postfix(Rect rect)
        {
            var anchor = Text.Anchor;
            var font = Text.Font;
            var style = Text.CurFontStyle.fontStyle;
            Widgets.DrawMenuSection(rect.ContractedBy(1));
            if (Find.Selector.selected.Count == 0 || !(Find.Selector.selected.First() is Pawn pawn))
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect, "Please select a pawn");
            }
            else DoContent(rect.ContractedBy(3));
            Text.CurFontStyle.fontStyle = style;
            Text.Font = font;
            Text.Anchor = anchor;
        }

        private static void DoContent(Rect rect)
        {
            var pawn = Find.Selector.selected.First() as Pawn;
            var needs = pawn.needs.needs;
            var hediffs = pawn.health.hediffSet.hediffs;
            var curRect = rect;
            curRect.ContractedBy(5);
            curRect.yMin += 5;            
            curRect.width -= 5;
            Widgets.BeginScrollView(curRect, ref scrollPosition, new Rect(Vector2.zero, new Vector2(rect.width - 15, (120 + 20) * (hediffs.Count + needs.Count()))));
            var elementRect = new Rect(5, 5, rect.width - 25, 87);
            var needsModel = pawn.GetNeedModels();
            foreach (var need in needs)
            {
                if (needsModel.TryGetValue(need.GetType(), out var model))
                {
                    model.DrawGraph(elementRect.BottomPartPixels(70));
                    Text.Font = GameFont.Tiny;
                    Text.CurFontStyle.fontStyle = FontStyle.Bold;
                    Widgets.Label(elementRect.TopPartPixels(14), GenText.CapitalizeFirst(need.def.label));
                    elementRect.y += elementRect.height + 20;
                }
            }
            var hediffsModel = pawn.GetHediffModels();
            foreach (var hediff in hediffs)
            {
                if (hediffsModel.TryGetValue(hediff, out var model))
                {
                    model.DrawGraph(elementRect.BottomPartPixels(70));
                    Text.Font = GameFont.Tiny;
                    Text.CurFontStyle.fontStyle = FontStyle.Bold;
                    
                    Widgets.Label(elementRect.TopPartPixels(14), GenText.CapitalizeFirst(hediff.def.label));
                    elementRect.y += elementRect.height;
                }
            }
            Widgets.EndScrollView();
        }
    }
    
    [SoyuzPatch(typeof(TabContent_Soyuz), nameof(TabContent_Soyuz.DoExtras))]
    public static class SoyuzSettingsGUIUtility
    {
        private static Vector2 scrollPosition = Vector2.zero;
        private static Rect viewRect = Rect.zero;
        private static Listing_Standard standard = new Listing_Standard();
        private static string searchString;
        private static RaceSettings curSelection;
        
        public static void Postfix(Rect rect)
        {
            Text.CurFontStyle.fontStyle = FontStyle.Bold;
            Widgets.Label(rect.TopPartPixels(25), "Dilated races");
            Text.CurFontStyle.fontStyle = FontStyle.Normal;
            var font = Text.Font;
            var anchor = Text.Anchor;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleLeft;
            rect.yMin += 25;
            var searchRect = rect.TopPartPixels(25);
            searchString = Widgets.TextField(searchRect, searchString).ToLower().Trim();
            rect.yMin += 30;
            if (curSelection != null)
            {
                var height = 128;
                var selectionRect = rect.TopPartPixels(height);
                Widgets.DrawMenuSection(selectionRect);
                Text.Font = GameFont.Tiny;
                Widgets.DefLabelWithIcon(selectionRect.TopPartPixels(54), curSelection.pawnDef);
                if (Widgets.ButtonImage(selectionRect.RightPartPixels(30).TopPartPixels(30).ContractedBy(5),
                    TexButton.CloseXSmall))
                {
                    curSelection = null;
                    return;
                }
                selectionRect.yMin += 54;
                standard.Begin(selectionRect.ContractedBy(3));
                Text.Font = GameFont.Tiny;
                standard.CheckboxLabeled($"Enable dilation for {curSelection.pawnDef.label}", ref curSelection.dilated);
                standard.CheckboxLabeled($"Enable dilation for all factions except Player", ref curSelection.ignoreFactions);
                standard.CheckboxLabeled($"Enable dilation for Player faction", ref curSelection.ignorePlayerFaction);
                standard.End();
                rect.yMin += height + 8;
            }
            else if(Find.Selector.selected.Count == 1 && Find.Selector.selected.First() is Pawn pawn)
            {
                var height = 128;
                var selectionRect = rect.TopPartPixels(height);
                var model = pawn.GetPerformanceModel();
                if(model != null){
                    model.DrawGraph(selectionRect, 2000);
                    rect.yMin += height + 8;
                }
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
            foreach (var element in Context.settings.raceSettings)
            {
                if(!element.pawnDef.label.ToLower().Contains(searchString))
                    continue;
                counter++;
                if(counter% 2 == 0)
                    Widgets.DrawBoxSolid(curRect, new Color(0.1f,0.1f,0.1f, 0.2f));
                Widgets.DrawHighlightIfMouseover(curRect);
                Widgets.DefLabelWithIcon(curRect.ContractedBy(3), element.pawnDef);
                if (Widgets.ButtonInvisible(curRect))
                {
                    curSelection = element;
                    break;
                }
                curRect.y += 58;
            }

            Text.Font = font;
            Text.Anchor = anchor;
            Widgets.EndScrollView();
            SoyuzSettingsUtility.CacheSettings();
        }
    }
    
    [StaticConstructorOnStartup]
    public static class SoyuzSettingsUtility
    {
        private static List<ThingDef> pawnDefs;
        
        [Main.OnDefsLoaded]
        public static void LoadSettings()
        {
            pawnDefs = DefDatabase<ThingDef>.AllDefs.Where(def => def.race != null).ToList();
            Context.soyuz = LoadedModManager.GetMod<SoyuzMod>();
            Context.settings = Context.soyuz.GetSettings<SoyuzSettings>();
            CacheSettings();
            Finder.soyuzLoaded = true;
        }

        public static void CacheSettings()
        {
            if (Context.settings == null)
                Context.settings = new SoyuzSettings();
            if (Context.settings.raceSettings.Count == 0)
                CreateSettings();
            foreach (var element in Context.settings.raceSettings)
            {
                if (element.pawnDef == null)
                    continue;
                element.Cache();
            }
        }

        public static void CreateSettings()
        {
            Context.settings.raceSettings.Clear();
            foreach (var def in pawnDefs)
            {
                Context.settings.raceSettings.Add(new RaceSettings()
                {
                    pawnDef = def, 
                    dilated = def.race.Animal && !def.race.Humanlike && !def.race.IsMechanoid,
                    ignoreFactions = false
                });
            }
        }
    
        public static void PostScribe()
        {
            Scribe_Deep.Look(ref Context.settings, "soyuzSettings");
        }
    }
}