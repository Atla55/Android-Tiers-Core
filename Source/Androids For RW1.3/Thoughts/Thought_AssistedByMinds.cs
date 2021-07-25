using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class Thought_AssistedByMinds : Thought_Situational
    {
        public override string LabelCap
        {
            get
            {
                return base.CurStage.label;
            }
        }

        protected override float BaseMoodOffset
        {
            get
            {
                int points = Utils.GCATPP.getNbAssistingMinds() * Settings.nbMoodPerAssistingMinds;
                //Capping mood bonus at 10 (from where start extra-consciousness bonus)
                if (points > 10)
                    points = 10;
                return points;
            }
        }
    }
}