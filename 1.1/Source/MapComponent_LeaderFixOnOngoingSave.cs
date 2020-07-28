using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;

namespace VFE_Settlers
{
    class MapComponent_LeaderFixOnOngoingSave : MapComponent
    {
        public MapComponent_LeaderFixOnOngoingSave(Map map) : base(map)
        {
        }

        private bool relationFixed = false;

		public override void MapComponentTick()
		{
			if (!relationFixed)
			{
                List<Faction> facs = Find.FactionManager.AllFactionsVisible.Where((f) => f.def.defName == "SettlerCivil" || f.def.defName == "SettlerRough" || f.def.defName == "SettlerSavage").ToList();
                foreach (Faction fac in facs)
                {
                    if (fac != null && !fac.defeated && fac.leader == null)
                    {
                        fac.GenerateNewLeader();
                        Messages.Message("Fixed missing leader for " + fac.Name, MessageTypeDefOf.PositiveEvent);
                    }
                }
                relationFixed = true;
			}
		}

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.relationFixed, "relationFixed", false, false);
        }
    }
}
