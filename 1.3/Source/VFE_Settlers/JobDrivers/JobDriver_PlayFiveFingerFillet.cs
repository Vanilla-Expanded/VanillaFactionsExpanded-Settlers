using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;
using UnityEngine;

namespace VFE_Settlers.JobGivers
{
    class JobDriver_PlayFiveFingerFillet : JobDriver_WatchBuilding
	{
		protected override void WatchTickAction()
		{
			if (this.pawn.IsHashIntervalTick(1000))
			{
				int rand = UnityEngine.Random.Range(0, 100);
				if (rand > 80)
				{
					BodyPartRecord bodyPartRecord = (from x in pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside, BodyPartTagDefOf.ManipulationLimbDigit, null)
													 where !x.def.conceptual
													 select x).RandomElement<BodyPartRecord>();
					this.pawn.TakeDamage(new DamageInfo(DamageDefOf.Cut, 1, 0, -1, pawn, bodyPartRecord));
				}
				this.pawn.skills.Learn(SkillDefOf.Melee, 100);
			}
			base.WatchTickAction();
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.EndOnDespawnedOrNull(TargetIndex.A, JobCondition.Incompletable);
			Toil watch;
			if (base.TargetC.HasThing && base.TargetC.Thing is Building_Bed)
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
				this.WatchTickAction();
			});
			watch.AddFinishAction(delegate
			{
				JoyUtility.TryGainRecRoomThought(this.pawn);
			});
			watch.defaultCompleteMode = ToilCompleteMode.Delay;
			watch.defaultDuration = this.job.def.joyDuration;
			watch.handlingFacing = true;
			watch.WithEffect(() => Defs.SettlerEffecterDefOf.Joy_HoldKnife, () => base.TargetA.Thing.OccupiedRect().ClosestCellTo(this.pawn.Position));
			yield return watch;
			yield break;
		}
	}
}
