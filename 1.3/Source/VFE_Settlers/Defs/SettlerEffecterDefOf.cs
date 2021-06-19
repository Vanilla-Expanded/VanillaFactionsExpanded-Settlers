using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;

namespace VFE_Settlers.Defs
{
	[DefOf]
	public static class SettlerEffecterDefOf
	{
		static SettlerEffecterDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(EffecterDefOf));
		}

		public static EffecterDef Joy_HoldKnife;
	}
}
