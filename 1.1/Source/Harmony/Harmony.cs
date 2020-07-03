using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using VFE_Settlers.Hediffs;

namespace VFE_Settlers.Utilities {
    [StaticConstructorOnStartup]
    internal static class Harmony
    {
        static Harmony()
        {
            var harmony = new HarmonyLib.Harmony("limetreesnake.settlers");
            #region Ticks
            harmony.Patch(AccessTools.Method(typeof(Pawn), "TickRare"), null, new HarmonyMethod(typeof(Harmony).GetMethod("TickRare_PostFix")));
            #endregion Ticks
            #region Functionality
            harmony.Patch(AccessTools.Method(typeof(GenStep_Settlement), "ScatterAt"), new HarmonyMethod(typeof(Harmony).GetMethod("ScatterAt_PreFix")), null);
            harmony.Patch(AccessTools.Method(typeof(Pawn), "Kill"), new HarmonyMethod(typeof(Harmony).GetMethod("Kill_PreFix")), null);
            #endregion Functionality
            #region GUI
            harmony.Patch(AccessTools.Method(typeof(Settlement), "GetCaravanGizmos"), null, new HarmonyMethod(typeof(Harmony).GetMethod("GetCaravanGizmos_PostFix")));
            #endregion GUI
        }
        #region Ticks
        public static void TickRare_PostFix(Pawn __instance) {
            if (!__instance.Dead && __instance.Spawned && __instance.kindDef == PawnKindDefOf.Muffalo) {
                bool validator(Thing t) {
                    if (__instance.IsFighting() || __instance.IsCaravanMember() || __instance.IsFormingCaravan()) {
                        return false;
                    }
                    return true;
                }
                Thing thing = GenClosest.ClosestThing_Global_Reachable(__instance.Position, __instance.Map, __instance.Map.listerThings.ThingsOfDef(Defs.ThingDefOf.Chemshine), PathEndMode.OnCell, TraverseParms.For(__instance), 20f, validator);
                if (thing != null) {
                    Job job = new Job(JobDefOf.Ingest, thing, __instance.Position);
                    __instance.jobs.TryTakeOrderedJob(job);
                }
            }
        }
        #endregion Ticks
        #region Functionality
        [HarmonyPriority(0)]
        public static bool ScatterAt_PreFix(IntVec3 c, Map map, int stackCount = 1) {
            try {
                if (map.ParentFaction != null && (map.ParentFaction.def.defName == "SettlerCivil" || map.ParentFaction.def.defName == "SettlerBandits")) {
                    IntRange range = new IntRange(36, 44);
                    int x = range.RandomInRange;
                    int z = range.RandomInRange;
                    return !UtilityBase.GenerateTown(map, Rand.Bool, map.ParentFaction, c, x, z, 6, 8, 3);
                }
                return true;
            }
            catch (Exception ex) {
                Log.Message(ex.Message);
                return true;
            }
        }
        public static void Kill_PreFix(Pawn __instance) {
            Hediff hediff = __instance.health.hediffSet.GetFirstHediffOfDef(Defs.HediffDefOf.Chemshined, false);
            if (hediff != null) {
                float radius;
                if (hediff.Severity <0.1)
                {
                    return;
                }
                else if(hediff.Severity < 0.25f) {
                    radius = 1.9f;
                }
                else if (hediff.Severity < 0.50f) {
                    radius = 2.9f;
                }
                else {
                    radius = 4.9f;
                }
                GenExplosion.DoExplosion(__instance.Position, __instance.Map, radius, DamageDefOf.Flame, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
            }
        }
        #endregion Functionality
        #region GUI
        public static void GetCaravanGizmos_PostFix(ref Caravan caravan, ref IEnumerable<Gizmo> __result) {
            try {
                Settlement settlement = CaravanVisitUtility.SettlementVisitedNow(caravan);
                if (settlement != null) {
                    foreach (Pawn criminal in caravan.PawnsListForReading) {
                        if (UtilityPawn.GetWantedComponent(criminal, out HediffComp_Wanted wanted) && settlement.Faction == wanted.WantedBy) {
                            List<Gizmo> gizmos = __result.ToList();
                            gizmos.Add(UtilityEvent.CommandTurnInWanted(caravan, criminal, wanted.Reward * 2));
                            __result = gizmos.AsEnumerable();
                        }
                    }
                    foreach (Corpse corpse in caravan.AllThings.Where(x=> x.def.IsCorpse)) {
                        if (UtilityPawn.GetWantedComponent(corpse, out HediffComp_Wanted wanted) && settlement.Faction == wanted.WantedBy) {
                            List<Gizmo> gizmos = __result.ToList();
                            gizmos.Add(UtilityEvent.CommandTurnInWanted(caravan, corpse.InnerPawn, wanted.Reward));
                            __result = gizmos.AsEnumerable();
                        }
                    }
                }
            }
            catch (Exception ex) {
                Log.Message(ex.Message);
            }
        }
        #endregion GUI
    }
}
