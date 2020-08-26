using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace VanillaFactionsExpandedSettlers
{
    public class CompProperties_WarmUpReduction : CompProperties
    {
        public CompProperties_WarmUpReduction()
        {
            this.compClass = typeof(CompWarmUpReduction);
        }
        public float WarmUpReductionPerShot = 0.1f;
        public ThingDef OverheatMoteThrown = null;
        public float OverheatMoteSize = 0.5f;
        public string UiIconPath = string.Empty;
    }

    // Token: 0x02000002 RID: 2
    public class CompWarmUpReduction : ThingComp
    {
        public CompProperties_WarmUpReduction Props => (CompProperties_WarmUpReduction)this.props;
        public int shotstack = 0;
        public bool hotshot;
        public bool initalized;
        protected virtual bool IsManned => (GetGunner != null);
        protected virtual Pawn GetGunner
        {
            get
            {
                Building_TurretGun turret = parent as Building_TurretGun;
                if (turret != null)
                {
                    CompMannable mannable = turret.TryGetComp<CompMannable>();
                    if (mannable.MannedNow)
                    {
                        return mannable.ManningPawn;
                    }
                }
                return null;
            }
        }

        private Texture2D CommandTex
        {
            get
            {
                return Props.UiIconPath.NullOrEmpty() ? this.parent.def.uiIcon : ContentFinder<Texture2D>.Get(Props.UiIconPath);
            }
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {

            ThingWithComps owner = IsManned ? GetGunner : parent;
            bool flag = Find.Selector.SingleSelectedThing == GetGunner;
            if (flag && GetGunner.Drafted && GetGunner.IsColonist)
            {

                int num = 700000101;
                yield return new Command_Toggle
                {
                    icon = this.CommandTex,
                    defaultLabel = "VWEL_ToggleHotshotLabel".Translate(),
                    defaultDesc = "VWEL_ToggleHotshotDesc".Translate(),
                    isActive = (() => hotshot),
                    toggleAction = delegate ()
                    {
                        hotshot = !hotshot;
                    },
                    activateSound = SoundDef.Named("Click"),
                    groupKey = num,
                    hotKey = KeyBindingDefOf.Misc2
                };
            }
            yield break;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!IsManned)
            {
                shotstack = 0;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.shotstack, "shotstack", 0);
            Scribe_Values.Look(ref this.hotshot, "hotshot", false);
            Scribe_Values.Look(ref this.initalized, "initalized", false);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
            }
        }

    }
}
