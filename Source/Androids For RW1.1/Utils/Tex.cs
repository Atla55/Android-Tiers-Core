using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace MOARANDROIDS
{
    [StaticConstructorOnStartup]
    static class Tex
    {
        static Tex()
        {
        }
        public static readonly Texture2D Battery = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_Battery", true);


        public static readonly Texture2D TexUISkillLogo = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_SkillUILogo", true);
        public static readonly Texture2D NoCare = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_NoCare", true);
        public static readonly Texture2D NoMed = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_OnlyDocVisit", true);
        public static readonly Texture2D NanoKitBasic = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_NanoKitBasic/ATPP_NanoKitBasic_a", true);
        public static readonly Texture2D NanoKitIntermediate = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_NanoKitIntermediate/ATPP_NanoKitIntermediate_a", true);
        public static readonly Texture2D NanoKitAdvanced = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_NanoKitAdvanced/ATPP_NanoKitAdvanced_a", true);

        public static readonly Texture2D UploadConsciousness = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_UploadConsciousness", true);
        public static readonly Texture2D Permute = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_Permute", true);
        public static readonly Texture2D PermuteDisabled = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_PermuteDisabled", true);
        public static readonly Texture2D Duplicate = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_Duplicate", true);
        public static readonly Texture2D DuplicateDisabled = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_DuplicateDisabled", true);
        public static readonly Texture2D UploadConsciousnessDisabled = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_UploadConsciousnessDisabled", true);
        public static readonly Texture2D UploadToSkyCloud = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_UploadToSkyCloud", true);
        public static readonly Texture2D UploadToSkyCloudDisabled = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_UploadToSkyCloudDisabled", true);
        public static readonly Texture2D DownloadFromSkyCloud = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_DownloadFromSkyCloud", true);
        public static readonly Texture2D DownloadFromSkyCloudDisabled = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_DownloadFromSkyCloudDisabled", true);

        public static readonly Texture2D MindAbsorption = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_MindAbsorption", true);
        public static readonly Texture2D MindAbsorptionDisabled = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_MindAbsorptionDisabled", true);


        public static readonly Texture2D RepairAndroid = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_RepairAndroids", true);

        
        public static readonly Texture2D PassionDisabled = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_PassionMinorGrayDisabled", true);
        public static readonly Texture2D NoPassion = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_PassionMinorGray", true);
        public static readonly Texture2D MinorPassion = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_PassionMinor", true);
        public static readonly Texture2D MajorPassion = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_PassionMajor", true);


        public static readonly Texture2D texAutoDoorClose = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_CloseDoor", true);
        public static readonly Texture2D texAutoDoorOpen = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_OpenDoor", true);

        public static readonly Texture2D processInfo = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_ProcessInfo", true);
        public static readonly Texture2D processRemove = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_ProcessRemove", true);
        public static readonly Texture2D processDuplicate = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_ProcessDuplicate", true);
        public static readonly Texture2D processAssist = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_ProcessAssist", true);
        public static readonly Texture2D processMigrate = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_ProcessMigrate", true);
        public static readonly Texture2D processSkillUp = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_ProcessSkillUp", true);


        public static readonly Texture2D SurrogateMode = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_SurrogateMode", true);
        public static readonly Texture2D SkyMindConn = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_SkyMind", true);
        public static readonly Texture2D SkyMindAutoConn = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_SkyMindAutoReconnect", true);
        public static readonly Texture2D SkillUp = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_SkillUp", true);

        public static readonly Texture2D ForceAndroidToExplode = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_OverloadAndroid", true);
        public static readonly Texture2D ForceAndroidToExplodeDisabled = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_OverloadAndroidDisabled", true);


        public static readonly Texture2D AndroidToControlTarget = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_AndroidToControlTarget", true);
        public static readonly Texture2D AndroidToControlTargetRecovery = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_AndroidToControlTargetRecovery", true);
        public static readonly Texture2D AndroidToControlTargetDisconnect = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_AndroidToControlTargetDisconnect", true);

        public static readonly Texture2D AndroidSurrogateReconnectToLastController = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_SurrogateReconnectLastUser", true);


        public static readonly Texture2D PlayerExplosiveVirus = ContentFinder<Texture2D>.Get("Things/Misc/Hacking/ATPP_ExplosiveVirus", true);
        public static readonly Texture2D PlayerExplosiveVirusDisabled = ContentFinder<Texture2D>.Get("Things/Misc/Hacking/ATPP_ExplosiveVirusDisabled", true);
        public static readonly Texture2D PlayerVirus = ContentFinder<Texture2D>.Get("Things/Misc/Hacking/ATPP_Virus", true);
        public static readonly Texture2D PlayerVirusDisabled = ContentFinder<Texture2D>.Get("Things/Misc/Hacking/ATPP_VirusDisabled", true);
        public static readonly Texture2D PlayerHacking = ContentFinder<Texture2D>.Get("Things/Misc/Hacking/ATPP_Hacking", true);
        public static readonly Texture2D PlayerHackingDisabled = ContentFinder<Texture2D>.Get("Things/Misc/Hacking/ATPP_HackingDisabled", true);
        public static readonly Texture2D PlayerHackingTemp = ContentFinder<Texture2D>.Get("Things/Misc/Hacking/ATPP_HackingTemp", true);
        public static readonly Texture2D PlayerHackingTempDisabled = ContentFinder<Texture2D>.Get("Things/Misc/Hacking/ATPP_HackingTempDisabled", true);

        public static readonly Texture2D StopVirused = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_StopVirused", true);

        public static readonly Texture2D ColorPicker = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_ColorPicker", true);


        public static readonly Texture2D SettingsHeader = ContentFinder<Texture2D>.Get("Things/Misc/SettingsHeader", true);

        public static readonly Texture2D LWPNConnected = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_LWPNConnected", true);
        public static readonly Texture2D LWPNNotConnected = ContentFinder<Texture2D>.Get("Things/Misc/ATPP_LWPNNotConnected", true);

        public static readonly Material UploadInProgress = MaterialPool.MatFrom("Things/Misc/ATPP_Sync", ShaderDatabase.MetaOverlay);
        public static readonly Material SelectableSX = MaterialPool.MatFrom("Things/Misc/ATPP_SelectableSX", ShaderDatabase.MetaOverlay);
        
        public static readonly Material SelectableSXToHack = MaterialPool.MatFrom("Things/Misc/ATPP_SelectableSXToHack", ShaderDatabase.MetaOverlay);
        public static readonly Material ConnectedUser = MaterialPool.MatFrom("Things/Misc/ATPP_SkyMind", ShaderDatabase.MetaOverlay);

        public static readonly Material matHotLevel1 =  MaterialPool.MatFrom("Things/Building/ATPP_Servers/Temperature/ATPP_Hot1", ShaderDatabase.MetaOverlay);
        public static readonly Material matHotLevel2 = MaterialPool.MatFrom("Things/Building/ATPP_Servers/Temperature/ATPP_Hot2", ShaderDatabase.MetaOverlay);
        public static readonly Material matHotLevel3 = MaterialPool.MatFrom("Things/Building/ATPP_Servers/Temperature/ATPP_Hot3", ShaderDatabase.MetaOverlay);

        public static readonly Material explosiveVirus = MaterialPool.MatFrom("Things/Misc/Virus/ATPP_ExplosiveVirus", ShaderDatabase.MetaOverlay);
        public static readonly Material virus = MaterialPool.MatFrom("Things/Misc/Virus/ATPP_Virus", ShaderDatabase.MetaOverlay);
        public static readonly Material cryptolocker = MaterialPool.MatFrom("Things/Misc/Virus/ATPP_Cryptolocker", ShaderDatabase.MetaOverlay);
        public static readonly Material virusLite = MaterialPool.MatFrom("Things/Misc/Virus/ATPP_VirusLite", ShaderDatabase.MetaOverlay);


        public static readonly Material Virused = MaterialPool.MatFrom("Things/Misc/Hacking/ATPP_Virused", ShaderDatabase.MetaOverlay);
        public static readonly Material ExplosiveVirused = MaterialPool.MatFrom("Things/Misc/Hacking/ATPP_ExplosiveVirused", ShaderDatabase.MetaOverlay);
        public static readonly Material HackedTemp = MaterialPool.MatFrom("Things/Misc/Hacking/ATPP_HackedTemp", ShaderDatabase.MetaOverlay);

        public static readonly Material RemotelyControlledNode = MaterialPool.MatFrom("Things/Misc/ATPP_RemoteControlled", ShaderDatabase.MetaOverlay);


        public static Dictionary<Pair<string, Color>, Graphic> eyeGlowEffectCache = new Dictionary<Pair<string, Color>, Graphic>();

        public static Graphic getEyeGlowEffect(Color color, string gender, int type, int front)
        {
            Pair<string, Color> key = new Pair<string, Color>(type.ToString()+ gender + front.ToString(), color);
            string path;

            Graphic res;
            if (eyeGlowEffectCache.ContainsKey(key))
            {
                res = eyeGlowEffectCache[key];
            }
            else
            {
                if (front==1)
                {
                    if(gender == "M")
                        path = "Things/Misc/Androids/Effects/Front";
                    else
                        path = "Things/Misc/Androids/Effects/FFront";

                    eyeGlowEffectCache[key] = GraphicDatabase.Get<Graphic_Single>(path+type, ShaderDatabase.MoteGlow, Vector2.one, color);
                }
                else
                {
                    if (gender == "M")
                        path = "Things/Misc/Androids/Effects/Side";
                    else
                        path = "Things/Misc/Androids/Effects/FSide";

                    eyeGlowEffectCache[key] = GraphicDatabase.Get<Graphic_Single>(path+type, ShaderDatabase.MoteGlow, Vector2.one, color);
                }
                res = eyeGlowEffectCache[key];
            }
            return res;
        }
    }
}