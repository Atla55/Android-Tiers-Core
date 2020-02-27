using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Androids
{
	// Token: 0x0200000F RID: 15
	public static class DroidUtility
	{
		// Token: 0x06000037 RID: 55 RVA: 0x00003458 File Offset: 0x00001658
		public static Pawn MakeDroidTemplate(ThingDef raceDef, PawnKindDef pawnKindDef, Faction faction, Map map, List<SkillRequirement> skills = null, int defaultSkillLevel = 6)
		{
			Pawn pawn2 = (Pawn)ThingMaker.MakeThing(raceDef, null);
			bool flag = pawn2 == null;
			Pawn result;
			if (flag)
			{
				result = null;
			}
			else
			{
				pawn2.kindDef = pawnKindDef;
				pawn2.SetFactionDirect(faction);
				PawnComponentsUtility.CreateInitialComponents(pawn2);
				pawn2.gender = Gender.Male;
				pawn2.needs.SetInitialLevels();
				bool humanlike = pawn2.RaceProps.Humanlike;
				if (humanlike)
				{
					pawn2.story.melanin = 1f;
					pawn2.story.crownType = CrownType.Average;
					pawn2.story.hairColor = new Color(1f, 1f, 1f, 1f);
					pawn2.story.hairDef = DefDatabase<HairDef>.GetNamed("Shaved", true);
					pawn2.story.bodyType = BodyType.Thin;
					PortraitsCache.SetDirty(pawn2);
					Backstory childhood = null;
					BackstoryDatabase.TryGetWithIdentifier("ChJAndroid_Droid", out childhood);
					pawn2.story.childhood = childhood;
					bool flag2 = skills == null || skills.Count <= 0;
					if (flag2)
					{
						List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
						for (int i = 0; i < allDefsListForReading.Count; i++)
						{
							SkillDef skillDef2 = allDefsListForReading[i];
							SkillRecord skill = pawn2.skills.GetSkill(skillDef2);
							bool flag3 = skillDef2 == SkillDefOf.Shooting || skillDef2 == SkillDefOf.Melee || skillDef2 == SkillDefOf.Mining || skillDef2 == SkillDefOf.Growing;
							if (flag3)
							{
								skill.Level = 8;
							}
							else
							{
								bool flag4 = skillDef2 == SkillDefOf.Medicine || skillDef2 == SkillDefOf.Crafting || skillDef2 == SkillDefOf.Cooking;
								if (flag4)
								{
									skill.Level = 4;
								}
								else
								{
									skill.Level = 6;
								}
							}
							skill.passion = Passion.None;
						}
					}
					else
					{
						List<SkillDef> allDefsListForReading2 = DefDatabase<SkillDef>.AllDefsListForReading;
						for (int j = 0; j < allDefsListForReading2.Count; j++)
						{
							SkillDef skillDef = allDefsListForReading2[j];
							SkillRecord skill2 = pawn2.skills.GetSkill(skillDef);
							SkillRequirement skillRequirement = skills.First((SkillRequirement sr) => sr.skill == skillDef);
							bool flag5 = skillRequirement != null;
							if (flag5)
							{
								skill2.Level = skillRequirement.minLevel;
							}
							else
							{
								skill2.Level = defaultSkillLevel;
							}
							skill2.passion = Passion.None;
						}
					}
				}
				bool flag6 = pawn2.workSettings != null;
				if (flag6)
				{
					pawn2.workSettings.EnableAndInitialize();
				}
				bool flag7 = map != null;
				if (flag7)
				{
					IEnumerable<Name> enumerable = from pawn in map.mapPawns.FreeColonists
					select pawn.Name;
					bool flag8 = enumerable != null;
					if (flag8)
					{
						int num = enumerable.Count((Name name) => name.ToStringShort.ToLower().StartsWith("droid"));
						string nickName = "Droid " + num;
						pawn2.Name = DroidUtility.MakeDroidName(nickName);
					}
					else
					{
						pawn2.Name = DroidUtility.MakeDroidName(null);
					}
				}
				else
				{
					pawn2.Name = DroidUtility.MakeDroidName(null);
				}
				result = pawn2;
			}
			return result;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000037A0 File Offset: 0x000019A0
		public static Pawn MakeCustomDroid(PawnKindDef pawnKind, Faction faction)
		{
			return PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKind, faction, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, new float?(0f), new float?(0f), null, null, null));
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003808 File Offset: 0x00001A08
		public static NameTriple MakeDroidName(string nickName)
		{
			string text = string.Format("D-{0:X}-{1:X}", Rand.Range(0, 256), Rand.Range(0, 256));
			bool flag = nickName == null;
			NameTriple result;
			if (flag)
			{
				result = new NameTriple(text, text, "");
			}
			else
			{
				result = new NameTriple(text, nickName, "");
			}
			return result;
		}
	}
}
