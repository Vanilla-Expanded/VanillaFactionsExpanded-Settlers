using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using HarmonyLib;

namespace VanillaFactionsExpandedSettlers
{
    [HarmonyPatch(typeof(Building_TurretGun), "BurstCooldownTime")]
    public static class VFES_Building_TurretGun_BurstCooldownTime_RapidFire_Patch
    {
        [HarmonyPostfix]
        public static void BurstCooldownTime_RapidFire_Postfix(ref Building_TurretGun __instance, ref float __result)
        {
            Building_TurretGun turret = __instance as Building_TurretGun;
            if (turret != null)
            {
                CompMannable mannable = turret.TryGetComp<CompMannable>();
                if (mannable != null && mannable.MannedNow)
                {
                    CompWarmUpReduction GunExt = turret.TryGetComp<CompWarmUpReduction>();
                    if (GunExt != null)
                    {
                        __result = Math.Max(__result - (GunExt.Props.WarmUpReductionPerShot * GunExt.shotstack), 0);
                    }
                }
            }
        }
    }
}