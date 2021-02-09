using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RocketMan;
using UnityEngine;
using Verse;

namespace Soyuz.Profiling
{
	// Token: 0x0200000E RID: 14
	public class ITab_History : ITab
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00003A44 File Offset: 0x00001C44
		public override bool IsVisible
		{
			get
			{
				return Finder.logData && Finder.debug && Prefs.LogVerbose && Prefs.DevMode;
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003A62 File Offset: 0x00001C62
		public override void UpdateSize()
		{
			base.UpdateSize();
			this.size = new Vector2(450f, 350f);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003A80 File Offset: 0x00001C80
		public override void FillTab()
		{
			Pawn pawn = base.SelPawn;
			IEnumerable<Type> needs = from n in pawn.needs.needs
			select n.GetType();
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			Rect curRect = new Rect(Vector2.zero, this.size);
			curRect.ContractedBy(5f);
			curRect.yMin += 20f;
			curRect = curRect.AtZero();
			curRect.yMin += 20f;
			curRect.width -= 5f;
			Widgets.BeginScrollView(curRect, ref this.scrollView, new Rect(Vector2.zero, new Vector2(430f, (float)(140 * (hediffs.Count + needs.Count<Type>())))), true);
			Rect elementRect = new Rect(0f, 0f, 425f, 95f);
			Dictionary<Type, PawnNeedModel> needsModel = pawn.GetNeedModels();
			foreach (Type type in needs)
			{
				PawnNeedModel model;
				if (needsModel.TryGetValue(type, out model))
				{
					model.DrawGraph(elementRect.BottomPartPixels(75f), 500, "%");
					Widgets.Label(elementRect.TopPartPixels(20f), type.Name);
					elementRect.y += elementRect.height + 20f;
				}
			}
			Dictionary<Hediff, PawnHediffModel> hediffsModel = pawn.GetHediffModels();
			foreach (Hediff hediff in hediffs)
			{
				PawnHediffModel model2;
				if (hediffsModel.TryGetValue(hediff, out model2))
				{
					model2.DrawGraph(elementRect.BottomPartPixels(75f), 500, "%");
					Widgets.Label(elementRect.TopPartPixels(20f), hediff.def.label);
					elementRect.y += elementRect.height + 20f;
				}
			}
			Widgets.EndScrollView();
		}

		// Token: 0x0400003F RID: 63
		private const int graphHeight = 95;

		// Token: 0x04000040 RID: 64
		private Vector2 scrollView = Vector2.zero;
	}
}
