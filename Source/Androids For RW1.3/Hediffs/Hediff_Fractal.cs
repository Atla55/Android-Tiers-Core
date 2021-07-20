﻿using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class Hediff_Fractal : HediffWithComps
    { 
            public override string LabelInBrackets
            {
                get
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(base.LabelInBrackets);
                    if (this.comps != null)
                    {
                        for (int i = 0; i < this.comps.Count; i++)
                        {
                            string compLabelInBracketsExtra = this.comps[i].CompLabelInBracketsExtra;
                            if (!compLabelInBracketsExtra.NullOrEmpty())
                            {
                                if (stringBuilder.Length != 0)
                                {
                                    stringBuilder.Append(", ");
                                }
                                stringBuilder.Append(compLabelInBracketsExtra);
                            }
                        }
                    }
                    return stringBuilder.ToString();
                }
            }

            public override bool ShouldRemove
            {
                get
                {
                    if (this.comps != null)
                    {
                        for (int i = 0; i < this.comps.Count; i++)
                        {
                            if (this.comps[i].CompShouldRemove)
                            {
                                return true;
                            }
                        }
                    }
                    return base.ShouldRemove;
                }
            }

            public override bool Visible
            {
                get
                {
                    if (this.comps != null)
                    {
                        for (int i = 0; i < this.comps.Count; i++)
                        {
                            if (this.comps[i].CompDisallowVisible())
                            {
                                return false;
                            }
                        }
                    }
                    return base.Visible;
                }
            }

            public override string TipStringExtra
            {
                get
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(base.TipStringExtra);
                    if (this.comps != null)
                    {
                        for (int i = 0; i < this.comps.Count; i++)
                        {
                            string compTipStringExtra = this.comps[i].CompTipStringExtra;
                            if (!compTipStringExtra.NullOrEmpty())
                            {
                                stringBuilder.AppendLine(compTipStringExtra);
                            }
                        }
                    }
                    return stringBuilder.ToString();
                }
            }

            public override TextureAndColor StateIcon
            {
                get
                {
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        TextureAndColor compStateIcon = this.comps[i].CompStateIcon;
                        if (compStateIcon.HasValue)
                        {
                            return compStateIcon;
                        }
                    }
                    return TextureAndColor.None;
                }
            }

            public override void PostAdd(DamageInfo? dinfo)
            {
                if (this.comps != null)
                {
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        this.comps[i].CompPostPostAdd(dinfo);
                    }
                }
            }

            public override void PostRemoved()
            {
                base.PostRemoved();
                if (this.comps != null)
                {
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        this.comps[i].CompPostPostRemoved();
                    }
                }
            }

            public override void PostTick()
            {
                base.PostTick();
                if (this.comps != null)
                {
                    float num = 0f;
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        this.comps[i].CompPostTick(ref num);
                    }
                    if (num != 0f)
                    {
                        this.Severity += num;
                    }
                }
            }

            public override void ExposeData()
            {
                base.ExposeData();
                if (Scribe.mode == LoadSaveMode.LoadingVars)
                {
                    this.InitializeComps();
                }
                if (this.comps != null)
                {
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        this.comps[i].CompExposeData();
                    }
                }
            }

            public override void Tended(float quality, float maxQuality, int batchPosition = 0)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].CompTended(quality, batchPosition);
                }
            }

            public override bool TryMergeWith(Hediff other)
            {
                if (base.TryMergeWith(other))
                {
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        this.comps[i].CompPostMerged(other);
                    }
                    return true;
                }
                return false;
            }

            public override void Notify_PawnDied()
            {
                base.Notify_PawnDied();
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].Notify_PawnDied();
                }
            }

            public override void ModifyChemicalEffect(ChemicalDef chem, ref float effect)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].CompModifyChemicalEffect(chem, ref effect);
                }
            }

            public override void PostMake()
            {
                base.PostMake();
                this.InitializeComps();
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].CompPostMake();
                }
            }

            private void InitializeComps()
            {
                if (this.def.comps != null)
                {
                    this.comps = new List<HediffComp>();
                    for (int i = 0; i < this.def.comps.Count; i++)
                    {
                        HediffComp hediffComp = (HediffComp)Activator.CreateInstance(this.def.comps[i].compClass);
                        hediffComp.props = this.def.comps[i];
                        hediffComp.parent = this;
                        this.comps.Add(hediffComp);
                    }
                }
            }

            public override string DebugString()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(base.DebugString());
                if (this.comps != null)
                {
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        string str;
                        if (this.comps[i].ToString().Contains("_"))
                        {
                            str = this.comps[i].ToString().Split(new char[]
                            {
                            '_'
                            })[1];
                        }
                        else
                        {
                            str = this.comps[i].ToString();
                        }
                        stringBuilder.AppendLine("--" + str);
                        string text = this.comps[i].CompDebugString();
                        if (!text.NullOrEmpty())
                        {
                            stringBuilder.AppendLine(text.TrimEnd(new char[0]).Indented());
                        }
                    }
                }
                return stringBuilder.ToString();
            }

    private bool IsSeverelyWounded
        {
            get
            {
                float num = 0f;
                List<Hediff> hediffs = this.pawn.health.hediffSet.hediffs;
                for (int i = 0; i < hediffs.Count; i++)
                {
                    if (hediffs[i] is Hediff_Injury && !hediffs[i].IsPermanent())
                    {
                        num += hediffs[i].Severity;
                    }
                }
                List<Hediff_MissingPart> missingPartsCommonAncestors = this.pawn.health.hediffSet.GetMissingPartsCommonAncestors();
                for (int j = 0; j < missingPartsCommonAncestors.Count; j++)
                {
                    if (missingPartsCommonAncestors[j].IsFreshNonSolidExtremity)
                    {
                        num += missingPartsCommonAncestors[j].Part.def.GetMaxHealth(this.pawn);
                    }
                }
                return num > 38f * this.pawn.RaceProps.baseHealthScale;
            }
        }

        public override void Tick()
        {
            this.ageTicks++;
            if (this.Severity >= 1f)
            {
                Hediff_Fractal.DoMutation(this.pawn);
                this.pawn.Destroy();
            }
        }

        public static void DoMutation(Pawn premutant)
        {
            string text = "Atlas_Mutation".Translate(premutant.Name.ToStringShort);
                text = text.AdjustedFor(premutant);
                string label = "LetterLabelAtlas_Mutation".Translate();
                Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NegativeEvent, premutant, null);

            PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOf.AbominationAtlas, Faction.OfMechanoids, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false, false);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            FilthMaker.TryMakeFilth(premutant.Position, premutant.Map, RimWorld.ThingDefOf.Filth_AmnioticFluid, premutant.LabelIndefinite(), 10);
            FilthMaker.TryMakeFilth(premutant.Position, premutant.Map, RimWorld.ThingDefOf.Filth_Blood, premutant.LabelIndefinite(), 10);

            GenSpawn.Spawn(pawn, premutant.Position, premutant.Map);
            pawn.mindState.mentalStateHandler.TryStartMentalState(RimWorld.MentalStateDefOf.ManhunterPermanent, null, true, false, null);
        }
    }
}
