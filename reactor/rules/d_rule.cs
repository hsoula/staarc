using System;
using System.Collections.Generic;

namespace greactor
{
	public class d_rule : rule
	{
		public type_state start;	

		public d_rule (int sT, int sS, double rate, int idx)
		{
			start = new type_state (sT, sS);		
			this.rate = rate;
			this.idx = idx;
		}


		public override double propensity(Dictionary<type_state,List<int>> stDict, Dictionary<pair_st,List<pair_part>> pDict){

			int na = stDict.ContainsKey (start) ? stDict [start].Count : 0;
			return rate * na;

		}

		public override double propensity(reactor rec){

			return propensity (rec.stDict, rec.pDict);
		}

		public int pick_reactants(Dictionary<type_state,List<int>> stDict, Dictionary<pair_st,List<pair_part>> pDict,Random rd){


			var la = stDict [start];
			int na = (int)(rd.NextDouble () * la.Count);
			return la[na];		
		}

		public  bool apply(reactor rec,int pa){


			particle a = rec.get_particle_id(pa);
			//Console.WriteLine("a: " + a);
			rec.remove_particle (a);
			return true;
		}

		public override bool apply_rule(reactor rec, Random rd){

			int pa = pick_reactants (rec.stDict, rec.pDict, rd);
			return apply (rec, pa);
		}

		public override string ToString ()
		{
			return   start+"->0";
		}
	}
}

