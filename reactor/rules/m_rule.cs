using System;
using System.Collections.Generic;

namespace greactor
{
	/// <summary>
	///  Mono molecular rule m_rule 
	///  (t1|s1) -> (t2|s2) : rate 
	/// 
	/// </summary>

	public class m_rule : rule
	{
		public type_state start;
		public type_state end;
		public bool unlink;
		
		public m_rule (int sT, int sS, int eT, int eS, bool u, double rate, int idx)
		{
			start = new type_state (sT, sS);
			end = new type_state (eT, eS);
			unlink = u;		
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

			type_state old_a = a.st;
			type_state new_a = end;
			if (old_a != new_a)
				rec.update_new_state (a, old_a, new_a);
		

			if (unlink)
				rec.all_unlink (a.id);

			return true;
		}

		public override bool apply_rule(reactor rec, Random rd){

			int pa = pick_reactants (rec.stDict, rec.pDict, rd);
			return apply (rec, pa);

		}
		public override string ToString ()
		{
			return string.Format (reactor.letters [start.type] + "" + start.state + "->" + reactor.letters [end.type] + "" + end.state);
		}
	}
}
