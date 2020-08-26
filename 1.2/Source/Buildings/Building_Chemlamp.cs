using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace VFE_Settlers.Buildings
{
    public class Building_Chemlamp : Building
	{
		private Graphic cachedGraphicFull;
		public override Graphic Graphic
		{
			get
			{
				if (GetComp<CompFlickable>().SwitchIsOn)
				{
					if (def.building.fullGraveGraphicData == null)
					{
						return base.Graphic;
					}
					if (GetComp<CompFlickable>().SwitchIsOn && GetComp<CompRefuelable>().HasFuel)
					{
						cachedGraphicFull = def.building.fullGraveGraphicData.GraphicColoredFor(this);
					}
					return cachedGraphicFull;
				}
				return base.Graphic;
			}
		}
	}
}
