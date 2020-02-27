using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Androids
{
	// Token: 0x0200001C RID: 28
	public class ThingOrderProcessor
	{
		// Token: 0x0600006D RID: 109 RVA: 0x00004D01 File Offset: 0x00002F01
		public ThingOrderProcessor()
		{
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004D16 File Offset: 0x00002F16
		public ThingOrderProcessor(ThingOwner thingHolder, StorageSettings storageSettings)
		{
			this.thingHolder = thingHolder;
			this.storageSettings = storageSettings;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004D39 File Offset: 0x00002F39
		public IEnumerable<ThingOrderRequest> PendingRequests()
		{
			foreach (ThingOrderRequest idealRequest in this.requestedItems)
			{
				bool nutrition = idealRequest.nutrition;
				if (nutrition)
				{
					float totalNutrition = this.CountNutrition();
					bool flag = totalNutrition < idealRequest.amount;
					if (flag)
					{
						ThingOrderRequest request = new ThingOrderRequest();
						request.nutrition = true;
						request.amount = idealRequest.amount - totalNutrition;
						request.thingFilter = this.storageSettings.filter;
						yield return request;
						request = null;
					}
				}
				else
				{
					float totalItemCount = (float)this.thingHolder.TotalStackCountOfDef(idealRequest.thingDef);
					bool flag2 = totalItemCount < idealRequest.amount;
					if (flag2)
					{
						ThingOrderRequest request2 = new ThingOrderRequest();
						request2.thingDef = idealRequest.thingDef;
						request2.amount = idealRequest.amount - totalItemCount;
						yield return request2;
						request2 = null;
					}
				}
				idealRequest = null;
			}
			List<ThingOrderRequest>.Enumerator enumerator = default(List<ThingOrderRequest>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00004D4C File Offset: 0x00002F4C
		public float CountNutrition()
		{
			float num = 0f;
			foreach (Thing thing in ((IEnumerable<Thing>)this.thingHolder))
			{
				Corpse corpse = thing as Corpse;
				bool flag = corpse != null;
				if (flag)
				{
					num += FoodUtility.GetBodyPartNutrition(corpse.InnerPawn, corpse.InnerPawn.RaceProps.body.corePart);
				}
				else
				{
					bool isIngestible = thing.def.IsIngestible;
					if (isIngestible)
					{
						float num2 = num;
						ThingDef def = thing.def;
						num = num2 + ((def != null) ? def.ingestible.nutrition : 0.05f) * (float)thing.stackCount;
					}
				}
			}
			return num;
		}

		// Token: 0x04000025 RID: 37
		public ThingOwner thingHolder;

		// Token: 0x04000026 RID: 38
		public StorageSettings storageSettings;

		// Token: 0x04000027 RID: 39
		public List<ThingOrderRequest> requestedItems = new List<ThingOrderRequest>();
	}
}
