using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Soyuz.Profiling
{
	// Token: 0x02000012 RID: 18
	public class PawnNeedModel
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00003FF1 File Offset: 0x000021F1
		public void AddResult(float value)
		{
			this.records.Insert(0, new PawnNeedRecord(value));
			if (this.records.Count > 2000)
			{
				this.records.Pop<PawnNeedRecord>();
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004024 File Offset: 0x00002224
		public void DrawGraph(Rect rect, int historyLength = 500, string unit = "%")
		{
			Widgets.DrawBoxSolid(rect, Color.white);
			rect = rect.ContractedBy(1f);
			Widgets.DrawBoxSolid(rect, Color.black);
			rect = rect.ContractedBy(5f);
			Rect numbersRect = rect.LeftPartPixels(50f);
			rect.xMin += 25f;
			Widgets.DrawBox(rect, 1);
			int oldHistoyLenght = historyLength;
			historyLength = Mathf.Min(historyLength, this.records.Count);
			if (historyLength <= 1)
			{
				return;
			}
			PawnNeedRecord[] curRecords = this.records.GetRange(0, historyLength).ToArray();
			float maxY = -100000f;
			float minY = (float)((int)curRecords.First<PawnNeedRecord>().value);
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
			Widgets.Label(numbersRect.TopPartPixels(25f), string.Format("{0}{1}", maxY * 100f, unit));
			Text.Anchor = TextAnchor.LowerLeft;
			Widgets.Label(numbersRect.BottomPartPixels(25f), string.Format("{0}{1}", minY * 100f, unit));
			Text.Font = font;
			Text.Anchor = anchor;
			float stepX = rect.width / (float)oldHistoyLenght;
			Vector2 curA = rect.position + new Vector2(0f, rect.height);
			Vector2 curB = rect.position + new Vector2(stepX, rect.height);
			for (int j = 0; j < historyLength - 1; j++)
			{
				PawnNeedRecord a = curRecords[j];
				PawnNeedRecord b = curRecords[j + 1];
				curA.y = (1f - (a.value - minY) / maxY) * rect.height + rect.y;
				curB.y = (1f - (b.value - minY) / maxY) * rect.height + rect.y;
				curA.x += stepX;
				curB.x += stepX;
				Widgets.DrawLine(curA, curB, a.dilationEnabled ? Color.green : Color.yellow, 1f);
			}
		}

		// Token: 0x04000048 RID: 72
		public List<PawnNeedRecord> records = new List<PawnNeedRecord>();
	}
}
