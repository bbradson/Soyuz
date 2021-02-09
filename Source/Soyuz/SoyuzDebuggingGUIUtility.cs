using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RocketMan.Tabs;
using Soyuz.Profiling;
using UnityEngine;
using Verse;

namespace Soyuz
{
	// Token: 0x0200000A RID: 10
	[SoyuzPatch(typeof(TabContent_Debug), "DoExtras", MethodType.Normal, null, null)]
	public static class SoyuzDebuggingGUIUtility
	{
		// Token: 0x06000028 RID: 40 RVA: 0x00002F20 File Offset: 0x00001120
		public static void Postfix(Rect rect)
		{
			TextAnchor anchor = Text.Anchor;
			GameFont font = Text.Font;
			FontStyle style = Text.CurFontStyle.fontStyle;
			Widgets.DrawMenuSection(rect.ContractedBy(1f));
			if (Find.Selector.selected.Count == 0 || !(Find.Selector.selected.First<object>() is Pawn))
			{
				Text.Font = GameFont.Medium;
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(rect, "Please select a pawn");
			}
			else
			{
				SoyuzDebuggingGUIUtility.DoContent(rect.ContractedBy(3f));
			}
			Text.CurFontStyle.fontStyle = style;
			Text.Font = font;
			Text.Anchor = anchor;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002FC0 File Offset: 0x000011C0
		private static void DoContent(Rect rect)
		{
			Pawn pawn = Find.Selector.selected.First<object>() as Pawn;
			List<Need> needs = pawn.needs.needs;
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			Rect curRect = rect;
			curRect.ContractedBy(5f);
			curRect.yMin += 5f;
			curRect.width -= 5f;
			Widgets.BeginScrollView(curRect, ref SoyuzDebuggingGUIUtility.scrollPosition, new Rect(Vector2.zero, new Vector2(rect.width - 15f, (float)(140 * (hediffs.Count + needs.Count<Need>())))), true);
			Rect elementRect = new Rect(5f, 5f, rect.width - 25f, 87f);
			Dictionary<Type, PawnNeedModel> needsModel = pawn.GetNeedModels();
			foreach (Need need in needs)
			{
				PawnNeedModel model;
				if (needsModel.TryGetValue(need.GetType(), out model))
				{
					model.DrawGraph(elementRect.BottomPartPixels(70f), 500, "%");
					Text.Font = GameFont.Tiny;
					Text.CurFontStyle.fontStyle = FontStyle.Bold;
					Widgets.Label(elementRect.TopPartPixels(14f), need.def.label.CapitalizeFirst());
					elementRect.y += elementRect.height + 20f;
				}
			}
			Dictionary<Hediff, PawnHediffModel> hediffsModel = pawn.GetHediffModels();
			foreach (Hediff hediff in hediffs)
			{
				PawnHediffModel model2;
				if (hediffsModel.TryGetValue(hediff, out model2))
				{
					model2.DrawGraph(elementRect.BottomPartPixels(70f), 500, "%");
					Text.Font = GameFont.Tiny;
					Text.CurFontStyle.fontStyle = FontStyle.Bold;
					Widgets.Label(elementRect.TopPartPixels(14f), hediff.def.label.CapitalizeFirst());
					elementRect.y += elementRect.height;
				}
			}
			Widgets.EndScrollView();
		}

		// Token: 0x04000030 RID: 48
		private static Vector2 scrollPosition = Vector2.zero;
	}
}
