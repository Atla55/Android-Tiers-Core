using System;
using RimWorld;
using Verse.AI;

namespace MOARANDROIDS
{
    // Token: 0x02000017 RID: 23
    public class MentalState_ManhunterNotColony : MentalState
    {
        // Token: 0x06000037 RID: 55 RVA: 0x00002D08 File Offset: 0x00000F08
        public override RandomSocialMode SocialModeMax()
        {
            return RandomSocialMode.Off;
        }
    }
}
