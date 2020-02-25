using System;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Androids
{
	// Token: 0x02000031 RID: 49
	public class SpawnerProjectileProperties : DefModExtension
	{
		// Token: 0x060000CF RID: 207 RVA: 0x00007C58 File Offset: 0x00005E58
		public LordJob CreateJobForLord(IntVec3 point)
		{
			bool flag = this.lordJob.GetConstructors().Any(delegate(ConstructorInfo constructor)
			{
				ParameterInfo[] parameters;
				return (parameters = constructor.GetParameters()) != null && parameters.Count<ParameterInfo>() > 0 && parameters[0].ParameterType == typeof(IntVec3);
			});
			LordJob result;
			if (flag)
			{
				result = (LordJob)Activator.CreateInstance(this.lordJob, new object[]
				{
					point
				});
			}
			else
			{
				result = (LordJob)Activator.CreateInstance(this.lordJob);
			}
			return result;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00007CD8 File Offset: 0x00005ED8
		public Faction GetFaction(Thing launcher)
		{
			bool flag = !this.usePlayerFaction;
			Faction result;
			if (flag)
			{
				bool flag2 = this.forcedFaction == null;
				if (flag2)
				{
					result = launcher.Faction;
				}
				else
				{
					result = FactionUtility.DefaultFactionFrom(this.forcedFaction);
				}
			}
			else
			{
				result = Faction.OfPlayer;
			}
			return result;
		}

		// Token: 0x04000070 RID: 112
		public PawnKindDef pawnKind;

		// Token: 0x04000071 RID: 113
		public ThingDef pawnThingDef;

		// Token: 0x04000072 RID: 114
		public int amount = 1;

		// Token: 0x04000073 RID: 115
		public FactionDef forcedFaction;

		// Token: 0x04000074 RID: 116
		public bool usePlayerFaction = true;

		// Token: 0x04000075 RID: 117
		public bool forceAgeToZero = false;

		// Token: 0x04000076 RID: 118
		public MentalStateDef mentalStateUponSpawn;

		// Token: 0x04000077 RID: 119
		public bool joinLordOnSpawn;

		// Token: 0x04000078 RID: 120
		public Type lordJob = typeof(LordJob_DefendPoint);

		// Token: 0x04000079 RID: 121
		public float lordJoinRadius = 99999f;

		// Token: 0x0400007A RID: 122
		public bool joinSameLordFromProjectile = true;
	}
}
