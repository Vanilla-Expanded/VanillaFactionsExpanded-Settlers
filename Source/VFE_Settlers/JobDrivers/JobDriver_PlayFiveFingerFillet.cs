using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace VFE_Settlers.JobGivers
{
    internal class JobDriver_PlayFiveFingerFillet : JobDriver_WatchBuilding
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.EndOnDespawnedOrNull(TargetIndex.A, JobCondition.Incompletable);
            Toil watch;
            if (TargetC.HasThing && TargetC.Thing is Building_Bed)
            {
                this.KeepLyingDown(TargetIndex.C);
                yield return Toils_Bed.ClaimBedIfNonMedical(TargetIndex.C, TargetIndex.None);
                yield return Toils_Bed.GotoBed(TargetIndex.C);
                watch = Toils_LayDown.LayDown(TargetIndex.C, true, false, true, true);
                watch.AddFailCondition(() => !watch.actor.Awake());
            }
            else
            {
                yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
                watch = new Toil();
            }
            watch.AddPreTickAction(delegate
            {
                WatchTickAction();
            });
            watch.AddFinishAction(delegate
            {
                JoyUtility.TryGainRecRoomThought(pawn);
            });
            watch.defaultCompleteMode = ToilCompleteMode.Delay;
            watch.defaultDuration = job.def.joyDuration;
            watch.handlingFacing = true;
            watch.WithEffect(() => Defs.SettlerEffecterDefOf.Joy_HoldKnife, () => TargetA.Thing.OccupiedRect().ClosestCellTo(pawn.Position));
            yield return watch;
            yield break;
        }

        protected override void WatchTickAction()
        {
            if (pawn.IsHashIntervalTick(1000))
            {
                if (pawn.Faction == Faction.OfPlayer && UnityEngine.Random.Range(0, 100) > 80)
                {
                    BodyPartRecord bodyPartRecord = (from x in pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside, BodyPartTagDefOf.ManipulationLimbDigit, null)
                                                     where !x.def.conceptual
                                                     select x).RandomElement();
                    pawn.TakeDamage(new DamageInfo(DamageDefOf.Cut, 1, 0, -1, null, bodyPartRecord));
                }
                pawn.skills.Learn(SkillDefOf.Melee, 50);
            }
            base.WatchTickAction();
        }
    }
}