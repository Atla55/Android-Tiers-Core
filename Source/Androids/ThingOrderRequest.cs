using System;
using System.Xml;
using RimWorld;
using Verse;

namespace Androids
{
	// Token: 0x02000029 RID: 41
	public class ThingOrderRequest
	{
		// Token: 0x06000091 RID: 145 RVA: 0x00005838 File Offset: 0x00003A38
		public ThingRequest Request()
		{
			bool flag = this.thingDef != null;
			ThingRequest result;
			if (flag)
			{
				result = ThingRequest.ForDef(this.thingDef);
			}
			else
			{
				bool flag2 = this.nutrition;
				if (flag2)
				{
					result = ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree);
				}
				else
				{
					result = ThingRequest.ForUndefined();
				}
			}
			return result;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005880 File Offset: 0x00003A80
		public Predicate<Thing> ExtraPredicate()
		{
			bool flag = this.nutrition;
			Predicate<Thing> result;
			if (flag)
			{
				bool flag2 = this.thingFilter == null;
				if (flag2)
				{
					result = delegate(Thing thing)
					{
						ThingDef def = thing.def;
						return def != null && !def.ingestible.IsMeal && thing.def.IsNutritionGivingIngestible;
					};
				}
				else
				{
					result = delegate(Thing thing)
					{
						bool flag3 = this.thingFilter.Allows(thing) && thing.def.IsNutritionGivingIngestible;
						bool result2;
						if (flag3)
						{
							Corpse corpse = thing as Corpse;
							bool flag4 = corpse != null && corpse.IsDessicated();
							result2 = !flag4;
						}
						else
						{
							result2 = false;
						}
						return result2;
					};
				}
			}
			else
			{
				result = ((Thing thing) => true);
			}
			return result;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000058FC File Offset: 0x00003AFC
		public void LoadDataFromXmlCustom(XmlNode xmlRoot)
		{
			bool flag = xmlRoot.ChildNodes.Count != 1;
			if (flag)
			{
				Log.Error("Misconfigured ThingOrderRequest: " + xmlRoot.OuterXml);
			}
			else
			{
				bool flag2 = xmlRoot.Name.ToLower() == "nutrition";
				if (flag2)
				{
					this.nutrition = true;
				}
				else
				{
					DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "thingDef", xmlRoot.Name);
				}
				this.amount = (float)ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(float));
			}
		}

		// Token: 0x0400003E RID: 62
		public ThingDef thingDef;

		// Token: 0x0400003F RID: 63
		public bool nutrition = false;

		// Token: 0x04000040 RID: 64
		public ThingFilter thingFilter = null;

		// Token: 0x04000041 RID: 65
		public float amount;
	}
}
