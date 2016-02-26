using System;
using System.Collections.Generic;
namespace greactor
{
	public class rule
	{
		public static int gid;
		public int id { get ; protected set; }
		public int idx { get; protected set; }


		public double rate;


		public rule ()
		{
			id = gid;
			gid++;
			rate = 0;
		}


		public virtual double propensity(Dictionary<type_state,List<int>> stDict, Dictionary<pair_st,List<pair_part>> pDict){

			//Console.WriteLine ("error");
			return 0.0;
		}			

		public virtual double propensity(reactor rec){

			//Console.WriteLine ("error");
			return 0.0;
		}			

		public virtual bool apply_rule(reactor rec, Random rd){

			return false;
		}
	}
}

