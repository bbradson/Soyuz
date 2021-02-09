using System;
using System.Linq;
using HarmonyLib;
using RocketMan;
using RocketMan.Tabs;
using Soyuz.Profiling;
using UnityEngine;
using Verse;

namespace Soyuz
{
	// Token: 0x0200000B RID: 11
	[SoyuzPatch(typeof(TabContent_Soyuz), "DoExtras", MethodType.Normal, null, null)]
	public static class SoyuzSettingsGUIUtility
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00003220 File Offset: 0x00001420
		public static void Postfix(Rect rect)
		{
			Text.CurFontStyle.fontStyle = FontStyle.Bold;
			Widgets.Label(rect.TopPartPixels(25f), "Dilated races");
			Text.CurFontStyle.fontStyle = FontStyle.Normal;
			GameFont font = Text.Font;
			TextAnchor anchor = Text.Anchor;
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.MiddleLeft;
			rect.yMin += 25f;
			Rect searchRect = rect.TopPartPixels(25f);
			SoyuzSettingsGUIUtility.searchString = Widgets.TextField(searchRect, SoyuzSettingsGUIUtility.searchString).ToLower().Trim();
			rect.yMin += 30f;
			if (SoyuzSettingsGUIUtility.curSelection != null)
			{
				int height = 128;
				Rect selectionRect = rect.TopPartPixels((float)height);
				Widgets.DrawMenuSection(selectionRect);
				Text.Font = GameFont.Tiny;
				Widgets.DefLabelWithIcon(selectionRect.TopPartPixels(54f), SoyuzSettingsGUIUtility.curSelection.pawnDef, 2f, 6f);
				if (Widgets.ButtonImage(selectionRect.RightPartPixels(30f).TopPartPixels(30f).ContractedBy(5f), TexButton.CloseXSmall, true))
				{
					SoyuzSettingsGUIUtility.curSelection = null;
					return;
				}
				selectionRect.yMin += 54f;
				SoyuzSettingsGUIUtility.standard.Begin(selectionRect.ContractedBy(3f));
				Text.Font = GameFont.Tiny;
				SoyuzSettingsGUIUtility.standard.CheckboxLabeled("Enable dilation for " + SoyuzSettingsGUIUtility.curSelection.pawnDef.label, ref SoyuzSettingsGUIUtility.curSelection.dilated, null);
				SoyuzSettingsGUIUtility.standard.CheckboxLabeled("Enable dilation for all factions except Player", ref SoyuzSettingsGUIUtility.curSelection.ignoreFactions, null);
				SoyuzSettingsGUIUtility.standard.CheckboxLabeled("Enable dilation for Player faction", ref SoyuzSettingsGUIUtility.curSelection.ignorePlayerFaction, null);
				SoyuzSettingsGUIUtility.standard.End();
				rect.yMin += (float)(height + 8);
			}
			else if (Find.Selector.selected.Count == 1)
			{
				Pawn pawn = Find.Selector.selected.First<object>() as Pawn;
				if (pawn != null)
				{
					int height2 = 128;
					Rect selectionRect2 = rect.TopPartPixels((float)height2);
					PawnPerformanceModel model = pawn.GetPerformanceModel();
					if (model != null)
					{
						model.DrawGraph(selectionRect2, 2000, "ms");
						rect.yMin += (float)(height2 + 8);
					}
				}
			}
			Widgets.DrawMenuSection(rect);
			rect = rect.ContractedBy(2f);
			SoyuzSettingsGUIUtility.viewRect.size = rect.size;
			SoyuzSettingsGUIUtility.viewRect.height = (float)(60 * Context.settings.raceSettings.Count);
			SoyuzSettingsGUIUtility.viewRect.width = SoyuzSettingsGUIUtility.viewRect.width - 15f;
			Widgets.BeginScrollView(rect, ref SoyuzSettingsGUIUtility.scrollPosition, SoyuzSettingsGUIUtility.viewRect.AtZero(), true);
			Rect curRect = SoyuzSettingsGUIUtility.viewRect.TopPartPixels(54f);
			curRect.width -= 15f;
			int counter = 0;
			foreach (RaceSettings element in Context.settings.raceSettings)
			{
				if (element.pawnDef.label.ToLower().Contains(SoyuzSettingsGUIUtility.searchString))
				{
					counter++;
					if (counter % 2 == 0)
					{
						Widgets.DrawBoxSolid(curRect, new Color(0.1f, 0.1f, 0.1f, 0.2f));
					}
					Widgets.DrawHighlightIfMouseover(curRect);
					Widgets.DefLabelWithIcon(curRect.ContractedBy(3f), element.pawnDef, 2f, 6f);
					if (Widgets.ButtonInvisible(curRect, true))
					{
						SoyuzSettingsGUIUtility.curSelection = element;
						break;
					}
					curRect.y += 58f;
				}
			}
			Text.Font = font;
			Text.Anchor = anchor;
			Widgets.EndScrollView();
			Finder.rocketMod.WriteSettings();
			SoyuzSettingsUtility.CacheSettings();
		}

		// Token: 0x04000031 RID: 49
		private static Vector2 scrollPosition = Vector2.zero;

		// Token: 0x04000032 RID: 50
		private static Rect viewRect = Rect.zero;

		// Token: 0x04000033 RID: 51
		private static Listing_Standard standard = new Listing_Standard();

		// Token: 0x04000034 RID: 52
		private static string searchString;

		// Token: 0x04000035 RID: 53
		private static RaceSettings curSelection;
	}
}
