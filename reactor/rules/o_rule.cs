using System;
using System.Collections.Generic;

namespace greactor
{
	public class o_rule : rule
	{
		
		public int target_type;
		public int target_state;

		public o_rule (int rt1, int rs1, double rate)
		{
			this.rate = rate;
			this.target_type = rt1;
			this.target_state = rs1;
		}

		public  override double propensity(Dictionary<type_state,List<int>> stDict, Dictionary<pair_st,List<pair_part>> pDict){


			return rate;
		}			

		public override double propensity(reactor rec){

			return propensity (rec.stDict, rec.pDict);
		}

		public  override bool apply_rule(reactor rec, Random rd){

			particle o = new particle (target_type, target_state);
			rec.add_particle(o);
			return true;
		}

		public override string ToString ()
		{
			return "0->" + reactor.letters [target_type] + ""+ target_state;
		}
	}
}

