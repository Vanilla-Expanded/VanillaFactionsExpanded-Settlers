using System;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;

namespace VanillaFactionsExpandedSettlers.HarmonyPatches
{
    
    [HarmonyPatch(typeof(Verb), "TryStartCastOn", new Type[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(bool), typeof(bool) })]
    public static class VFES_Verb_Shoot_TryStartCastOn_RapidFire_Patch
    {
        [HarmonyPrefix, HarmonyPriority(100)]
        public static void TryStartCastOn_RapidFire_Prefix(ref Verb __instance, LocalTargetInfo castTarg, float __state, ref bool __result)
        {
            if (__instance.GetType() == typeof(Verb_Shoot))
            {
                Building_TurretGun turret = __instance.caster as Building_TurretGun;
                if (turret != null)
                {
                    if (castTarg != null && turret!=null && __instance.CanHitTarget(castTarg))
                    {
                        if (turret.TryGetComp<CompWarmUpReduction>() != null)
                        {
                            if (turret.TryGetComp<CompWarmUpReduction>() is CompWarmUpReduction GunExt)
                            {
                                if (turret.IsMannable)
                                {
                                    CompMannable mannable = turret.TryGetComp<CompMannable>();
                                    if (mannable == null)
                                    {
                                        return;
                                    }
                                    if (mannable.MannedNow)
                                    {
                                        GunExt.shotstack++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
