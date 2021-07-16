using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using System;

namespace MOARANDROIDS
{
        public class StockGenerator_SlaveAndroids : StockGenerator
        {
            public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
            {
                if (this.respectPopulationIntent && Rand.Value > StorytellerUtilityPopulation.PopulationIntent)
                {
                    yield break;
                }
                int count = this.countRange.RandomInRange;
                for (int i = 0; i < count; i++)
                {
                    Faction androidFaction;
                    if (!(from fac in Find.FactionManager.AllFactionsVisible
                          where fac != Faction.OfPlayer && fac.def.humanlikeFaction
                          select fac).TryRandomElement(out androidFaction))
                    {
                        yield break;
                    }

                    Random rnd = new Random();
                    PawnKindDef android;
                    if (randomisePawnKind == true)
                    {
                        switch (rnd.Next(1, 4))
                        {
                            case 1:
                                 android = PawnKindDefOf.AndroidT1ColonistGeneral;
                                 break;
                            case 2:
                                 android = PawnKindDefOf.AndroidT2ColonistGeneral;
                                 break;
                            case 3:
                                android = PawnKindDefOf.AndroidT3ColonistGeneral;
                                break;
                            case 4:
                                android = PawnKindDefOf.AndroidT4ColonistGeneral;
                                break;
                            default:
                                android = PawnKindDefOf.AndroidT1ColonistGeneral;
                                break;
                        }
                    } else
                    {
                        android = pawnKind;
                    }
                    
                    Faction fac1 = androidFaction;
                PawnGenerationRequest request = new PawnGenerationRequest(android, fac1, PawnGenerationContext.NonPlayer, forTile, false, false, false, false, true, false, 1f, !this.trader.orbital, true, true, false, false, false, false);
                    yield return PawnGenerator.GeneratePawn(request);
                }
                yield break;
            }

            public override bool HandlesThingDef(ThingDef thingDef)
            {
                return thingDef.category == ThingCategory.Pawn && thingDef.race.Humanlike && thingDef.tradeability != Tradeability.None;
            }

            private bool respectPopulationIntent = false;
            private PawnKindDef pawnKind;
            private bool randomisePawnKind;
    }
}