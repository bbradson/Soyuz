using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Soyuz.Profiling
{
	// Token: 0x02000014 RID: 20
	public class PawnPerformanceModel
	{
		// Token: 0x0600004A RID: 74 RVA: 0x000042E0 File Offset: 0x000024E0
		public void AddResult(long ticks)
		{
			this.records.Insert(0, new PawnPerformanceRecord((float)ticks));
			if (this.records.Count > 2000)
			{
				this.records.Pop<PawnPerformanceRecord>();
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004314 File Offset: 0x00002514
		public void DrawGraph(Rect rect, int historyLength = 60, string unit = "ms")
		{
			Widgets.DrawBoxSolid(rect, Color.white);
			rect = rect.ContractedBy(1f);
			Widgets.DrawBoxSolid(rect, Color.black);
			rect = rect.ContractedBy(5f);
			Rect numbersRect = rect.LeftPartPixels(50f);
			rect.xMin += 25f;
			Widgets.DrawBox(rect, 1);
			historyLength = Mathf.Min(historyLength, this.records.Count);
			if (historyLength <= 1)
			{
				return;
			}
			PawnPerformanceRecord[] curRecords = this.records.GetRange(0, historyLength).ToArray();
			float maxY = -100000f;
			float minY = (float)((int)curRecords.First<PawnPerformanceRecord>().value);
			for (int i = 0; i < historyLength - 1; i++)
			{
				if (curRecords[i].value > maxY)
				{
					maxY = curRecords[i].value;
				}
				if (curRecords[i].value < minY)
				{
					minY = curRecords[i].value;
				}
			}
			GameFont font = Text.Font;
			TextAnchor anchor = Text.Anchor;
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.Label(numbersRect.TopPartPixels(25f), string.Format("{0}{1}", maxY * 1000f, unit));
			Text.Anchor = TextAnchor.LowerLeft;
			Widgets.Label(numbersRect.BottomPartPixels(25f), string.Format("{0}{1}", minY * 1000f, unit));
			Text.Font = font;
			Text.Anchor = anchor;
			float stepX = rect.width / (float)historyLength;
			Vector2 curA = rect.position + new Vector2(0f, rect.height);
			Vector2 curB = rect.position + new Vector2(stepX, rect.height);
			for (int j = 0; j < historyLength - 1; j++)
			{
				PawnPerformanceRecord a = curRecords[j];
				PawnPerformanceRecord b = curRecords[j + 1];
				curA.y = (1f - (a.value - minY) / maxY) * rect.height + rect.y;
				curB.y = (1f - (b.value - minY) / maxY) * rect.height + rect.y;
				curA.x += stepX;
				curB.x += stepX;
				Widgets.DrawLine(curA, curB, a.dilationEnabled ? Color.green : Color.yellow, 1f);
			}
		}

		// Token: 0x0400004C RID: 76
		public List<PawnPerformanceRecord> records = new List<PawnPerformanceRecord>();
	}
}
