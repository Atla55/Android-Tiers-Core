using System;
using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace Androids
{
	// Token: 0x0200002E RID: 46
	public class Projectile_Spawner : Projectile
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00007804 File Offset: 0x00005A04
		public SpawnerProjectileProperties SpawnerProps
		{
			get
			{
				return this.def.GetModExtension<SpawnerProjectileProperties>();
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00007824 File Offset: 0x00005A24
		public virtual void DoSpawn(Thing hitThing)
		{
			Pawn pawn = null;
			bool flag = this.SpawnerProps.pawnKind != null;
			if (flag)
			{
				pawn = PawnGenerator.GeneratePawn(this.SpawnerProps.pawnKind, null);
			}
			bool flag2 = this.SpawnerProps.pawnThingDef != null;
			if (flag2)
			{
				pawn = (Pawn)ThingMaker.MakeThing(this.SpawnerProps.pawnThingDef, null);
			}
			bool flag3 = pawn != null;
			if (flag3)
			{
				pawn.SetFaction(this.SpawnerProps.GetFaction(this.launcher), null);
				bool forceAgeToZero = this.SpawnerProps.forceAgeToZero;
				if (forceAgeToZero)
				{
					pawn.ageTracker.AgeBiologicalTicks = 0L;
					pawn.ageTracker.AgeChronologicalTicks = 0L;
				}
				GenPlace.TryPlaceThing(pawn, base.Position, base.Map, ThingPlaceMode.Near, null);
				bool flag4 = this.SpawnerProps.mentalStateUponSpawn != null;
				if (flag4)
				{
					pawn.mindState.mentalStateHandler.TryStartMentalState(this.SpawnerProps.mentalStateUponSpawn, null, true, false, null);
				}
				bool joinLordOnSpawn = this.SpawnerProps.joinLordOnSpawn;
				if (joinLordOnSpawn)
				{
					bool flag5 = this.lord == null && !this.SpawnerProps.joinSameLordFromProjectile;
					if (flag5)
					{
						this.lord = this.GetLord(pawn);
					}
					this.lord.AddPawn(pawn);
				}
				MoteMaker.ThrowSmoke(pawn.Position.ToVector3(), base.Map, Rand.Range(0.5f, 1.5f));
				MoteMaker.ThrowSmoke(pawn.Position.ToVector3(), base.Map, Rand.Range(1f, 3f));
				MoteMaker.ThrowAirPuffUp(pawn.Position.ToVector3(), base.Map);
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000079E0 File Offset: 0x00005BE0
		public Lord GetLord(Pawn forPawn)
		{
			Lord lord = null;
			Faction faction = forPawn.Faction;
			bool flag = forPawn.Map.mapPawns.SpawnedPawnsInFaction(faction).Any((Pawn p) => p != forPawn);
			if (flag)
			{
				Pawn p2 = (Pawn)GenClosest.ClosestThing_Global(forPawn.Position, forPawn.Map.mapPawns.SpawnedPawnsInFaction(faction), this.SpawnerProps.lordJoinRadius, (Thing p) => p != forPawn && ((Pawn)p).GetLord() != null, null);
				lord = p2.GetLord();
			}
			bool flag2 = lord == null;
			if (flag2)
			{
				LordJob lordJob = this.SpawnerProps.CreateJobForLord(forPawn.Position);
				lord = LordMaker.MakeNewLord(faction, lordJob, Find.VisibleMap, null);
			}
			return lord;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00007AC0 File Offset: 0x00005CC0
		protected override void Impact(Thing hitThing)
		{
			SoundDef soundExplode = this.def.projectile.soundExplode;
			bool flag = soundExplode != null;
			if (flag)
			{
				soundExplode.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
			}
			bool joinSameLordFromProjectile = this.SpawnerProps.joinSameLordFromProjectile;
			if (joinSameLordFromProjectile)
			{
				LordJob lordJob = this.SpawnerProps.CreateJobForLord(base.Position);
				this.lord = LordMaker.MakeNewLord(base.Faction, lordJob, Find.VisibleMap, null);
			}
			for (int i = 0; i < this.SpawnerProps.amount; i++)
			{
				this.DoSpawn(hitThing);
			}
			this.Destroy(DestroyMode.Vanish);
		}

		// Token: 0x0400005E RID: 94
		private Lord lord = null;
	}
}
