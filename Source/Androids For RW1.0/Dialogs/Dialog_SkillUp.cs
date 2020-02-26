using System;
using Verse;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;

namespace MOARANDROIDS
{
    public class Dialog_SkillUp : Window
    {
        Pawn android;
        List<string> libs;
        List<SkillDef> sd;
        List<int> points;
        List<int> passionsState;

        int curSumPassions = 0;
        bool isMind = false;

        int pointsNeededPerSkill = 0;
        int pointsNeededToIncreasePassion = 0;

        public static Vector2 scrollPosition = Vector2.zero;

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(580f, 725f);
            }
        }

        public Dialog_SkillUp(Pawn android, bool isMind=false)
        {
            this.android = android;
            this.forcePause = true;
            this.doCloseX = true;
            this.absorbInputAroundWindow = true;
            this.closeOnAccept = false;
            this.closeOnClickedOutside = true;

            points = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            passionsState = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            libs = new List<string> { "Shooting".Translate(), "Melee".Translate(), "Construction".Translate(), "Mining".Translate(), "Cooking".Translate(), "Plants".Translate(), "Animals".Translate(), "Crafting".Translate(), "Artistic".Translate(), "Medicine".Translate(), "Social".Translate(), "Intellectual".Translate()};
            sd = new List<SkillDef> { SkillDefOf.Shooting, SkillDefOf.Melee, SkillDefOf.Construction, SkillDefOf.Mining, SkillDefOf.Cooking, SkillDefOf.Plants, SkillDefOf.Animals, SkillDefOf.Crafting, SkillDefOf.Artistic, SkillDefOf.Medicine, SkillDefOf.Social, SkillDefOf.Intellectual };

            int i = 0;
            foreach(var csd in sd)
            {
                SkillRecord sr = android.skills.GetSkill(csd);
                if (sr != null && !sr.TotallyDisabled)
                {
                    passionsState[i] = (int)sr.passion;
                    curSumPassions += curSumPassions;
                }
                else
                {
                    passionsState[i] = -1;
                }
                i++;
            }

            pointsNeededPerSkill = Utils.getNbSkillPointsPerSkill(android, isMind);
            pointsNeededToIncreasePassion = Utils.getNbSkillPointsToIncreasePassion(android, isMind);
            this.isMind = isMind;
        }

        public override void DoWindowContents(Rect inRect)
        {
            inRect.yMin += 15f;
            inRect.yMax -= 15f;
            var defaultColumnWidth = (inRect.width - 50);
            Listing_Standard list = new Listing_Standard() { ColumnWidth = defaultColumnWidth - 20 };

            //Image logo
            Widgets.ButtonImage(new Rect(0,0, inRect.width, 80), Tex.TexUISkillLogo, Color.white, Color.white);

            var outRect = new Rect(inRect.x, inRect.y + 100, inRect.width, inRect.height - 245);
            var scrollRect = new Rect(0f, 100f, inRect.width - 16f, inRect.height * 2.5f + 50);

            Widgets.BeginScrollView(outRect, ref scrollPosition, scrollRect, true);

            list.Begin(scrollRect);

            GUI.color = Color.yellow;
            list.Label("=> "+android.LabelShort);
            GUI.color = Color.white;
            list.Gap();

            SkillRecord sr;
            int availableSkillPoints = Utils.GCATPP.getNbSkillPoints();
            int nbPointsBuyable = 0;

            if(pointsNeededPerSkill != 0)
                nbPointsBuyable = (int)Math.Floor((double)(availableSkillPoints / pointsNeededPerSkill));

            //Controles de selection des points a acheter
            for (int i = 0; i != sd.Count; i++)
            {
                int p = points[i];
                string comp = "";
                sr = android.skills.GetSkill(sd[i]);
                if ( sr != null && !sr.TotallyDisabled)
                {
                    if (points[i] != 0)
                        comp = "+" + points[i];

                    if (comp != "")
                        GUI.color = Color.green;
                    list.Label(libs[i] +" "+sr.levelInt+"/20 "+ comp);
                    if (comp != null)
                        GUI.color = Color.white;

                    p = (int)list.Slider(p, 0, 20);

                    //Check possibilité action
                    if(p+sr.levelInt < 20 &&  (p+ getNbPointsWantedToBuy(i)) <= nbPointsBuyable )
                    {
                        //On peut les acheter 
                        points[i] = p;
                    }

                    //Partie gestion des passions
                    if(passionsState[i] == -1)
                    {
                        list.ButtonImage(Tex.PassionDisabled, 24, 24);
                    }
                    else
                    {
                        Texture2D tex = Tex.NoPassion;
                        bool max = false;

                        if (passionsState[i] == (int)Passion.Minor)
                            tex = Tex.MinorPassion;
                        else if (passionsState[i] == (int)Passion.Major) {
                            tex = Tex.MajorPassion;
                            max = true;
                        }

                        if(list.ButtonImage(tex, 24, 24))
                        {
                            if (!max)
                            {
                                //Check player a les moyens 
                                int locNbWantedPoints = getNbPointsWantedToBuy();
                                int locAvailablePoints = nbPointsBuyable - locNbWantedPoints;

                                if (locAvailablePoints - pointsNeededToIncreasePassion >= 0)
                                {
                                    passionsState[i]++;
                                }
                                else
                                {
                                    passionsState[i] = (int)sr.passion;
                                    Messages.Message("ATPP_NotEnoughtSkillPoints".Translate(pointsNeededToIncreasePassion), MessageTypeDefOf.NegativeEvent);
                                }
                            }
                            else
                            {
                                passionsState[i] = (int)sr.passion;
                            }
                        }
                    }

                    list.GapLine();
                }
            }


            int nbWantedPoints = getNbPointsWantedToBuy();
            int availablePoints = nbPointsBuyable - nbWantedPoints;

            list.End();
            Widgets.EndScrollView();

            //Affichage nb points 
            if (availablePoints > 0)
                GUI.color = Color.green;
            Widgets.Label(new Rect(0, inRect.height - 115f, inRect.width - 30f, 35f),"ATPP_SkillsWorkshopAvailablePoints".Translate(availablePoints));

            GUI.color = Color.cyan;
            Widgets.Label(new Rect(0, inRect.height - 95f, inRect.width - 30f, 45f), "ATPP_SkillsWorkshopAvailablePointsNote".Translate(pointsNeededPerSkill));
            GUI.color = Color.white;

            //Validation
            if (nbWantedPoints != 0)
                GUI.color = Color.green;
            else
                GUI.color = Color.gray;

            if (Widgets.ButtonText(new Rect(0, inRect.height - 45f, inRect.width, 35f), "OK".Translate(), true, false, true))
            {
                if (nbWantedPoints == 0)
                    return;

                GUI.color = Color.white;

                //Decrementation des points de skills
                Utils.GCATPP.decSkillPoints(nbWantedPoints * pointsNeededPerSkill);

                //Incrementation effective des points
                for (int i = 0; i != sd.Count; i++)
                {
                    sr = android.skills.GetSkill(sd[i]);
                    if (sr != null)
                    {
                        sr.levelInt += points[i];

                        int diff = passionsState[i] - (int)sr.passion;
                        if(diff > 0)
                        {
                            sr.passion = (Passion)passionsState[i];
                        }
                    }
                }

                Messages.Message("ATPP_SkillsWorkshopPointsConverted".Translate(nbWantedPoints, android.LabelShortCap), MessageTypeDefOf.PositiveEvent);

                Find.WindowStack.TryRemove(this, true);
            }
            GUI.color = Color.white;
            //Annulation
            GUI.color = Color.red;
            if (Widgets.ButtonText(new Rect(0, inRect.height - 10f, inRect.width, 35f), "Back".Translate(), true, false, true))
            {
                GUI.color = Color.white;
                Find.WindowStack.TryRemove(this, true);
            }
            GUI.color = Color.white;

        }

        private int getNbPointsWantedToBuy(int indexToAvoid=-1)
        {
            int ret = 0;
            for(int i = 0; i != points.Count; i++)
            {
                if (indexToAvoid == i)
                    continue;
                ret += points[i];
            }

            for (int i = 0; i != passionsState.Count; i++)
            {
                SkillRecord sr = android.skills.GetSkill(sd[i]);
                if (sr != null && !sr.TotallyDisabled)
                {
                    if (passionsState[i] != (int)sr.passion)
                        ret += (passionsState[i] - (int)sr.passion) * pointsNeededToIncreasePassion;
                }
            }

            return ret;
        }
    }
}