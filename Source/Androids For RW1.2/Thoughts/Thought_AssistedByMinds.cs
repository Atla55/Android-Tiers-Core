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
                return Utils.GCATPP.getNbAssistingMinds()*Settings.nbMoodPerAssistingMinds;
            }
        }
    }
}