using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Androids.Integration;
using Harmony;
using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Androids
{
	// Token: 0x02000012 RID: 18
	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
		// Token: 0x0600003E RID: 62 RVA: 0x000039C4 File Offset: 0x00001BC4
		static HarmonyPatches()
		{
			HarmonyInstance harmonyInstance = HarmonyInstance.Create("chjees.androids");
			Type typeFromHandle = typeof(Pawn_NeedsTracker);
			HarmonyPatches.int_Pawn_NeedsTracker_GetPawn = typeFromHandle.GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
			harmonyInstance.Patch(typeFromHandle.GetMethod("ShouldHaveNeed", BindingFlags.Instance | BindingFlags.NonPublic), null, new HarmonyMethod(typeof(HarmonyPatches).GetMethod("Patch_Pawn_NeedsTracker_ShouldHaveNeed")), null);
			Type typeFromHandle2 = typeof(PawnRenderer);
			HarmonyPatches.int_PawnRenderer_GetPawn = typeFromHandle2.GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
			harmonyInstance.Patch(typeFromHandle2.GetMethod("RenderPawnInternal", BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, CallingConventions.Any, new Type[]
			{
				typeof(Vector3),
				typeof(Quaternion),
				typeof(bool),
				typeof(Rot4),
				typeof(Rot4),
				typeof(RotDrawMode),
				typeof(bool),
				typeof(bool)
			}, null), null, new HarmonyMethod(typeof(HarmonyPatches).GetMethod("Patch_PawnRenderer_RenderPawnInternal")), null);
			Type typeFromHandle3 = typeof(Need_Food);
			HarmonyPatches.int_Need_Food_Starving_GetPawn = typeFromHandle3.GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
			PropertyInfo propertyInfo = AccessTools.Property(typeFromHandle3, "Starving");
			MethodInfo original = (propertyInfo != null) ? propertyInfo.GetGetMethod() : null;
			harmonyInstance.Patch(original, null, new HarmonyMethod(typeof(HarmonyPatches).GetMethod("Patch_Need_Food_Starving_Get")), null);
			Type typeFromHandle4 = typeof(HealthUtility);
			harmonyInstance.Patch(typeFromHandle4.GetMethod("AdjustSeverity"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("Patch_HealthUtility_AdjustSeverity")), null, null);
			Type typeFromHandle5 = typeof(ThinkNode_ConditionalNeedPercentageAbove);
			HarmonyPatches.int_ConditionalPercentageNeed_need = typeFromHandle5.GetField("need", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
			harmonyInstance.Patch(typeFromHandle5.GetMethod("Satisfied", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("Patch_ThinkNode_ConditionalNeedPercentageAbove_Satisfied")), null, null);
			Type typeFromHandle6 = typeof(FoodUtility);
			harmonyInstance.Patch(typeFromHandle6.GetMethod("WillIngestStackCountOf"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_WillIngestStackCountOf")), null, null);
			Type typeFromHandle7 = typeof(RecordWorker_TimeInBedForMedicalReasons);
			harmonyInstance.Patch(typeFromHandle7.GetMethod("ShouldMeasureTimeNow"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_ShouldMeasureTimeNow")), null, null);
			Type typeFromHandle8 = typeof(InteractionUtility);
			harmonyInstance.Patch(typeFromHandle8.GetMethod("CanInitiateInteraction"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_CanInitiateInteraction")), null, null);
			Type typeFromHandle9 = typeof(Pawn_HealthTracker);
			HarmonyPatches.int_Pawn_HealthTracker_GetPawn = typeFromHandle9.GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
			harmonyInstance.Patch(typeFromHandle9.GetMethod("ShouldBeDeadFromRequiredCapacity"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_ShouldBeDeadFromRequiredCapacity")), null, null);
			Type typeFromHandle10 = typeof(HediffSet);
			harmonyInstance.Patch(typeFromHandle10.GetMethod("CalculatePain", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_CalculatePain")), null, null);
			Type typeFromHandle11 = typeof(RestUtility);
			harmonyInstance.Patch(typeFromHandle11.GetMethod("TimetablePreventsLayDown"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_TimetablePreventsLayDown")), null, null);
			Type typeFromHandle12 = typeof(GatheringsUtility);
			harmonyInstance.Patch(typeFromHandle12.GetMethod("ShouldGuestKeepAttendingGathering"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_ShouldGuestKeepAttendingGathering")), null, null);
			Type typeFromHandle13 = typeof(JobGiver_EatInPartyArea);
			harmonyInstance.Patch(typeFromHandle13.GetMethod("TryGiveJob", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_EatInPartyAreaTryGiveJob")), null, null);
			Type typeFromHandle14 = typeof(JobGiver_GetJoy);
			harmonyInstance.Patch(typeFromHandle14.GetMethod("TryGiveJob", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_GetJoyTryGiveJob")), null, null);
			Type typeFromHandle15 = typeof(Pawn_InteractionsTracker);
			HarmonyPatches.int_Pawn_InteractionsTracker_GetPawn = typeFromHandle15.GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
			harmonyInstance.Patch(typeFromHandle15.GetMethod("SocialFightChance"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_SocialFightChance")), null, null);
			harmonyInstance.Patch(typeFromHandle15.GetMethod("InteractionsTrackerTick"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_InteractionsTrackerTick")), null, null);
			harmonyInstance.Patch(typeFromHandle15.GetMethod("CanInteractNowWith"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_CanInteractNowWith")), null, null);
			Type typeFromHandle16 = typeof(InteractionUtility);
			harmonyInstance.Patch(typeFromHandle16.GetMethod("CanInitiateInteraction"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_CanDoInteraction")), null, null);
			harmonyInstance.Patch(typeFromHandle16.GetMethod("CanReceiveInteraction"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_CanDoInteraction")), null, null);
			Type typeFromHandle17 = typeof(PawnDiedOrDownedThoughtsUtility);
			harmonyInstance.Patch(typeFromHandle17.GetMethod("AppendThoughts_ForHumanlike", BindingFlags.Static | BindingFlags.NonPublic), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_AppendThoughts_ForHumanlike")), null, null);
			Type typeFromHandle18 = typeof(InspirationHandler);
			harmonyInstance.Patch(typeFromHandle18.GetMethod("InspirationHandlerTick"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_InspirationHandlerTick")), null, null);
			Type typeFromHandle19 = typeof(JobDriver_Vomit);
			harmonyInstance.Patch(typeFromHandle19.GetMethod("MakeNewToils", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompatPatch_VomitJob")), null, null);
			harmonyInstance.Patch(typeof(SymbolResolver_RandomMechanoidGroup).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).First((MethodInfo mi) => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.ReturnType == typeof(bool) && mi.GetParameters().Count<ParameterInfo>() == 1 && mi.GetParameters()[0].ParameterType == typeof(PawnKindDef)), null, new HarmonyMethod(typeof(HarmonyPatches), "MechanoidsFixerAncient", null), null);
			harmonyInstance.Patch(typeof(CompSpawnerMechanoidsOnDamaged).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First((MethodInfo mi) => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.ReturnType == typeof(bool) && mi.GetParameters().Count<ParameterInfo>() == 1 && mi.GetParameters()[0].ParameterType == typeof(PawnKindDef)), null, new HarmonyMethod(typeof(HarmonyPatches), "MechanoidsFixer", null), null);
			harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000040B0 File Offset: 0x000022B0
		public static bool CompatPatch_VomitJob(ref JobDriver_Vomit __instance, ref IEnumerable<Toil> __result)
		{
			Pawn pawn = __instance.pawn;
			bool flag = pawn.def.HasModExtension<MechanicalPawnProperties>();
			bool result;
			if (flag)
			{
				JobDriver_Vomit instance = __instance;
				__result = new List<Toil>
				{
					new Toil
					{
						initAction = delegate()
						{
							instance.pawn.jobs.StopAll(false);
						}
					}
				};
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00004118 File Offset: 0x00002318
		public static bool CompatPatch_CanDoInteraction(ref bool __result, ref Pawn pawn)
		{
			bool flag = pawn.def.HasModExtension<MechanicalPawnProperties>();
			bool result;
			if (flag)
			{
				__result = false;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004144 File Offset: 0x00002344
		public static bool CompatPatch_InspirationHandlerTick(ref InspirationHandler __instance)
		{
			bool flag = __instance.pawn.def.HasModExtension<MechanicalPawnProperties>();
			return !flag;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00004174 File Offset: 0x00002374
		public static bool CompatPatch_AppendThoughts_ForHumanlike(ref Pawn victim)
		{
			bool flag = victim.def.HasModExtension<MechanicalPawnProperties>();
			return !flag;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x0000419C File Offset: 0x0000239C
		public static bool CompatPatch_InteractionsTrackerTick(ref Pawn_InteractionsTracker __instance)
		{
			Pawn pawn = HarmonyPatches.Pawn_InteractionsTracker_GetPawn(__instance);
			bool flag = pawn.def.HasModExtension<MechanicalPawnProperties>();
			return !flag;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000041CC File Offset: 0x000023CC
		public static bool CompatPatch_CanInteractNowWith(ref Pawn_InteractionsTracker __instance, ref bool __result, ref Pawn recipient)
		{
			bool flag = recipient.def.HasModExtension<MechanicalPawnProperties>();
			bool result;
			if (flag)
			{
				__result = false;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000041F8 File Offset: 0x000023F8
		public static bool CompatPatch_SocialFightChance(ref Pawn_InteractionsTracker __instance, ref float __result, ref InteractionDef interaction, ref Pawn initiator)
		{
			Pawn pawn = HarmonyPatches.Pawn_InteractionsTracker_GetPawn(__instance);
			bool flag = pawn.def.HasModExtension<MechanicalPawnProperties>() || initiator.def.HasModExtension<MechanicalPawnProperties>();
			bool result;
			if (flag)
			{
				__result = 0f;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00004240 File Offset: 0x00002440
		public static void MechanoidsFixerAncient(ref bool __result, PawnKindDef kind)
		{
			bool flag = kind.race.HasModExtension<MechanicalPawnProperties>();
			if (flag)
			{
				__result = false;
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004260 File Offset: 0x00002460
		public static void MechanoidsFixer(ref bool __result, PawnKindDef def)
		{
			bool flag = def.race.HasModExtension<MechanicalPawnProperties>();
			if (flag)
			{
				__result = false;
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004280 File Offset: 0x00002480
		public static bool CompatPatch_GetJoyTryGiveJob(ref JobGiver_EatInPartyArea __instance, ref Job __result, ref Pawn pawn)
		{
			bool flag = pawn.def.HasModExtension<MechanicalPawnProperties>();
			bool result;
			if (flag)
			{
				__result = null;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000042AC File Offset: 0x000024AC
		public static bool CompatPatch_EatInPartyAreaTryGiveJob(ref JobGiver_EatInPartyArea __instance, ref Job __result, ref Pawn pawn)
		{
			bool flag = pawn.def.HasModExtension<MechanicalPawnProperties>();
			bool result;
			if (flag)
			{
				__result = null;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000042D8 File Offset: 0x000024D8
		public static bool CompatPatch_ShouldGuestKeepAttendingGathering(ref bool __result, ref Pawn p)
		{
			bool flag = p.def.HasModExtension<MechanicalPawnProperties>();
			bool result;
			if (flag)
			{
				__result = (!p.Downed && p.health.hediffSet.BleedRateTotal <= 0f && !p.health.hediffSet.HasTendableNonInjuryNonMissingPartHediff(false) && p.Awake() && !p.InAggroMentalState && !p.IsPrisoner);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004358 File Offset: 0x00002558
		public static bool CompatPatch_TimetablePreventsLayDown(ref bool __result, ref Pawn pawn)
		{
			bool flag = pawn.def.HasModExtension<MechanicalPawnProperties>();
			bool result;
			if (flag)
			{
				__result = (pawn.timetable != null && !pawn.timetable.CurrentAssignment.allowRest);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000043A4 File Offset: 0x000025A4
		public static bool CompatPatch_CalculatePain(ref HediffSet __instance, ref float __result)
		{
			bool flag = __instance.pawn.def.HasModExtension<MechanicalPawnProperties>();
			bool result;
			if (flag)
			{
				__result = 0f;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000043D8 File Offset: 0x000025D8
		public static bool CompatPatch_ShouldBeDeadFromRequiredCapacity(ref Pawn_HealthTracker __instance, ref PawnCapacityDef __result)
		{
			Pawn pawn = HarmonyPatches.Pawn_HealthTracker_GetPawn(__instance);
			bool flag = pawn.def.HasModExtension<MechanicalPawnProperties>();
			bool result;
			if (flag)
			{
				List<PawnCapacityDef> allDefsListForReading = DefDatabase<PawnCapacityDef>.AllDefsListForReading;
				for (int i = 0; i < allDefsListForReading.Count; i++)
				{
					PawnCapacityDef pawnCapacityDef = allDefsListForReading[i];
					bool flag2 = allDefsListForReading[i] == PawnCapacityDefOf.Consciousness && !__instance.capacities.CapableOf(pawnCapacityDef);
					if (flag2)
					{
						__result = pawnCapacityDef;
						return false;
					}
				}
				__result = null;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004468 File Offset: 0x00002668
		public static bool CompatPatch_WillIngestStackCountOf(int __result, ref Pawn ingester, ref ThingDef def)
		{
			bool flag = ingester == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				Pawn pawn = ingester;
				bool flag2 = ((pawn != null) ? pawn.needs.TryGetNeed(NeedDefOf.Food) : null) != null;
				bool flag3 = !flag2;
				result = !flag3;
			}
			return result;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000044B4 File Offset: 0x000026B4
		public static bool CompatPatch_ShouldMeasureTimeNow(bool __result, ref Pawn pawn)
		{
			bool flag = pawn == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				Pawn pawn2 = pawn;
				bool flag2 = ((pawn2 != null) ? pawn2.needs.TryGetNeed(NeedDefOf.Rest) : null) != null;
				bool flag3 = !flag2;
				if (flag3)
				{
					if (pawn.InBed())
					{
						if (!HealthAIUtility.ShouldSeekMedicalRestUrgent(pawn))
						{
							bool flag4 = HealthAIUtility.ShouldSeekMedicalRest(pawn) && pawn.CurJob.restUntilHealed;
						}
					}
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004530 File Offset: 0x00002730
		public static bool CompatPatch_CanInitiateInteraction(bool __result, ref Pawn pawn)
		{
			bool flag = pawn.def.HasModExtension<MechanicalPawnProperties>();
			return !flag;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x0000455C File Offset: 0x0000275C
		public static Pawn Pawn_HealthTracker_GetPawn(Pawn_HealthTracker instance)
		{
			return (Pawn)HarmonyPatches.int_Pawn_HealthTracker_GetPawn.GetValue(instance);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00004580 File Offset: 0x00002780
		public static Pawn Pawn_InteractionsTracker_GetPawn(Pawn_InteractionsTracker instance)
		{
			return (Pawn)HarmonyPatches.int_Pawn_InteractionsTracker_GetPawn.GetValue(instance);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000045A4 File Offset: 0x000027A4
		public static Pawn Pawn_NeedsTracker_GetPawn(Pawn_NeedsTracker instance)
		{
			return (Pawn)HarmonyPatches.int_Pawn_NeedsTracker_GetPawn.GetValue(instance);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000045C8 File Offset: 0x000027C8
		public static Pawn Need_Food_Starving_GetPawn(Need_Food instance)
		{
			return (Pawn)HarmonyPatches.int_Need_Food_Starving_GetPawn.GetValue(instance);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000045EC File Offset: 0x000027EC
		public static Pawn PawnRenderer_GetPawn_GetPawn(PawnRenderer instance)
		{
			return (Pawn)HarmonyPatches.int_PawnRenderer_GetPawn.GetValue(instance);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00004610 File Offset: 0x00002810
		public static NeedDef ThinkNode_ConditionalNeedPercentageAbove_GetNeed(ThinkNode_ConditionalNeedPercentageAbove instance)
		{
			return (NeedDef)HarmonyPatches.int_ConditionalPercentageNeed_need.GetValue(instance);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00004634 File Offset: 0x00002834
		public static bool Patch_ThinkNode_ConditionalNeedPercentageAbove_Satisfied(ref ThinkNode_ConditionalNeedPercentageAbove __instance, ref bool __result, ref Pawn pawn)
		{
			NeedDef def = HarmonyPatches.ThinkNode_ConditionalNeedPercentageAbove_GetNeed(__instance);
			bool flag = pawn.needs.TryGetNeed(def) != null;
			bool flag2 = !flag;
			bool result;
			if (flag2)
			{
				__result = false;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004670 File Offset: 0x00002870
		public static bool Patch_HealthUtility_AdjustSeverity(Pawn pawn, HediffDef hdDef, float sevOffset)
		{
			bool flag = pawn.def == ThingDefOf.ChjAndroid && hdDef == HediffDefOf.Malnutrition;
			return !flag;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000046A8 File Offset: 0x000028A8
		public static void Patch_Pawn_NeedsTracker_ShouldHaveNeed(ref Pawn_NeedsTracker __instance, ref bool __result, ref NeedDef nd)
		{
			Pawn pawn = HarmonyPatches.Pawn_NeedsTracker_GetPawn(__instance);
			bool flag = NeedsDefOf.ChJEnergy != null;
			if (flag)
			{
				bool flag2 = nd == NeedsDefOf.ChJEnergy;
				if (flag2)
				{
					bool flag3 = pawn.def == ThingDefOf.ChjAndroid || pawn.def.HasModExtension<MechanicalPawnProperties>();
					if (flag3)
					{
						__result = true;
					}
					else
					{
						__result = false;
					}
				}
			}
			bool flag4 = !AndroidsModSettings.Instance.droidCompatibilityMode;
			if (flag4)
			{
				bool flag5 = nd == NeedDefOf.Food || nd == NeedDefOf.Rest || nd == NeedDefOf.Joy || nd == NeedsDefOf.Beauty || nd == NeedsDefOf.Comfort || nd == NeedsDefOf.Space || (HarmonyPatches.Need_Bladder != null && nd == HarmonyPatches.Need_Bladder) || (HarmonyPatches.Need_Hygiene != null && nd == HarmonyPatches.Need_Hygiene);
				if (flag5)
				{
					bool flag6 = pawn.def.HasModExtension<MechanicalPawnProperties>();
					if (flag6)
					{
						__result = false;
					}
				}
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004798 File Offset: 0x00002998
		public static void Patch_PawnRenderer_RenderPawnInternal(ref PawnRenderer __instance, Vector3 rootLoc, Quaternion quat, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump)
		{
			bool flag = __instance != null && AndroidsModSettings.Instance.androidEyeGlow;
			if (flag)
			{
				Pawn pawn = HarmonyPatches.PawnRenderer_GetPawn_GetPawn(__instance);
				bool flag4;
				if (pawn != null && pawn.def == ThingDefOf.ChjAndroid && !pawn.Dead && !headStump)
				{
					bool flag3;
					if (!portrait)
					{
						bool flag2;
						if (pawn == null)
						{
							flag2 = (null != null);
						}
						else
						{
							Pawn_JobTracker jobs = pawn.jobs;
							flag2 = (((jobs != null) ? jobs.curDriver : null) != null);
						}
						if (flag2)
						{
							flag3 = !pawn.jobs.curDriver.asleep;
							goto IL_73;
						}
					}
					flag3 = portrait;
					IL_73:
					flag4 = (flag3 || portrait);
				}
				else
				{
					flag4 = false;
				}
				bool flag5 = flag4;
				if (flag5)
				{
					Vector3 a = rootLoc;
					bool flag6 = bodyFacing != Rot4.North;
					if (flag6)
					{
						a.y += 0.0281250011f;
						rootLoc.y += 0.0234375f;
					}
					else
					{
						a.y += 0.0234375f;
						rootLoc.y += 0.0281250011f;
					}
					Vector3 b = quat * __instance.BaseHeadOffsetAt(headFacing);
					Vector3 loc = a + b + new Vector3(0f, 0.01f, 0f);
					bool flag7 = headFacing != Rot4.North;
					if (flag7)
					{
						Mesh mesh = MeshPool.humanlikeHeadSet.MeshAt(headFacing);
						bool isHorizontal = headFacing.IsHorizontal;
						if (isHorizontal)
						{
							GenDraw.DrawMeshNowOrLater(mesh, loc, quat, EffectTextures.GetEyeGraphic(false, pawn.story.hairColor.SaturationChanged(0.6f)).MatSingle, portrait);
						}
						else
						{
							GenDraw.DrawMeshNowOrLater(mesh, loc, quat, EffectTextures.GetEyeGraphic(true, pawn.story.hairColor.SaturationChanged(0.6f)).MatSingle, portrait);
						}
					}
				}
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004948 File Offset: 0x00002B48
		public static void Patch_Need_Food_Starving_Get(ref Need_Food __instance, ref bool __result)
		{
			Pawn pawn = HarmonyPatches.Need_Food_Starving_GetPawn(__instance);
			bool flag = pawn != null && pawn.def == ThingDefOf.ChjAndroid;
			if (flag)
			{
				__result = false;
			}
		}

		// Token: 0x04000012 RID: 18
		public static FieldInfo int_Pawn_NeedsTracker_GetPawn;

		// Token: 0x04000013 RID: 19
		public static FieldInfo int_PawnRenderer_GetPawn;

		// Token: 0x04000014 RID: 20
		public static FieldInfo int_Need_Food_Starving_GetPawn;

		// Token: 0x04000015 RID: 21
		public static FieldInfo int_ConditionalPercentageNeed_need;

		// Token: 0x04000016 RID: 22
		public static FieldInfo int_Pawn_HealthTracker_GetPawn;

		// Token: 0x04000017 RID: 23
		public static FieldInfo int_Pawn_InteractionsTracker_GetPawn;

		// Token: 0x04000018 RID: 24
		public static NeedDef Need_Bladder = DefDatabase<NeedDef>.GetNamedSilentFail("Bladder");

		// Token: 0x04000019 RID: 25
		public static NeedDef Need_Hygiene = DefDatabase<NeedDef>.GetNamedSilentFail("Hygiene");
	}
}
