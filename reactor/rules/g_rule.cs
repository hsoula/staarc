using System;
using System.Collections.Generic;

namespace greactor
{
	public class g_rule : rule
	{

		int function;
		int target_type;
		int p1,p2;

		public g_rule (int f, int t, int p1, int p2, double l, int idx)
		{
			function = f;
			target_type = t;
			this.p1 = p1;
			this.p2 = p2;			
			rate = l;
			this.idx = idx;
			Console.WriteLine("rule f:" + f + "-" + idx + "-"+this);

		}

		public override string ToString ()
		{
			string s = "X";
			switch (function) {
			case 0:
			default:
				s += "null";
				break;
			case 1:
				s += "link";
				break;
			case 2:
				s += "split";
				break;
			case 3:
				s += "add";
				break;
			case 4:
				s += "swap";
				break;				
			}

			s += ":" + target_type + "-" + p1 + " " + p2;
			return s;
		}

		#region Propensity 

		public override double propensity(Dictionary<type_state,List<int>> stDict, Dictionary<pair_st,List<pair_part>> pDict){

			type_state a = new type_state(target_type,p1);
			type_state b = new type_state(target_type,p2);
			pair_st st = new pair_st(a,b);


			switch (function) {

			case 0:
			default:
				return 0.0;


			case 1: // link 
				int na = stDict.ContainsKey (a) ? stDict [a].Count : 0;
				int nb = stDict.ContainsKey (b) ? stDict [b].Count : 0;
				int nc = pDict.ContainsKey (st) ? pDict [st].Count : 0;
				nc += pDict.ContainsKey (st.swap ()) ? pDict [st.swap ()].Count : 0;
				if (a.state == b.state && a.type == b.type)
					nb += -1;

				Console.WriteLine (na + " " + nb + " " + nc +  " " + rate * (na * nb - nc));
				if (nc > na * nb)
					return 0.0;
				return rate * (na * nb - nc);
				
			case 2: // split
			case 3: // add
			case 4: // swap 				
				int count = 0;
				if (pDict.ContainsKey (st))
					count += pDict [st].Count;
				pair_st st_swap = st.swap ();
				if (pDict.ContainsKey (st_swap))
					count += pDict [st_swap].Count;
				Console.WriteLine (count);
				return rate * count;
				
			}
			
		}
		#endregion

		#region Reaction routines

		public pair_part pick_reactants(Dictionary<type_state,List<int>> stDict, Dictionary<pair_st,List<pair_part>> pDict,Random rd){

			type_state a = new type_state(target_type,p1);
			type_state b = new type_state(target_type,p2);
			pair_st st = new pair_st(a,b);

			switch (function) {
			case 0:
			default:
				return new pair_part (-1, -1);				
				
			case 1: // link 
				var la = stDict [a];
				var lb = stDict [b];
				bool done = false;
				int tmx = 0;
				while (!done) {
					tmx++;
					int na = (int)(rd.NextDouble () * la.Count);
					int nb = (int)(rd.NextDouble () * lb.Count);
					int pa = la [na];
					int pb = lb [nb];
					if (pa !=  pb){

						if (!pDict.ContainsKey (st)) {
							var ret = new pair_part (pa, pb);					
							return ret;
						} else {
							pair_part pp = new pair_part (pa, pb);
							if (!pDict [st].Contains (pp) && !pDict [st].Contains (pp.swap ())) {
								var ret = new pair_part (pa, pb);					
								return ret;
							}
						}
					}
					if (tmx > 100)
						done = true;
				}

				for (int xa = 0; xa < la.Count; xa++)
					for (int xb = 0; xb < lb.Count; xb++) {
						int pa = la [xa];
						int pb = lb [xb];
						pair_part pp = new pair_part (pa, pb);
						//Console.WriteLine (pp.id1+"-"+pp.id2 + " " + xa + " " + xb + " " + pDict [st].Contains (pp) + pDict [st].Contains (pp.swap ()));
						if (!pDict [st].Contains (pp) && !pDict [st].Contains (pp.swap ())) {
							var ret = new pair_part (pa, pb);					
							return ret;
						}
					}
				int ra = stDict.ContainsKey (a) ? stDict [a].Count : 0;
				int rb = stDict.ContainsKey (b) ? stDict [b].Count : 0;
				int nc = pDict.ContainsKey (st) ? pDict [st].Count : 0;
				nc +=  pDict.ContainsKey (st.swap()) ? pDict [st.swap()].Count : 0;
				if (a.state == b.state && a.type ==b.type )
					rb += -1;
				//	Console.WriteLine ("early stop :"+a +" "+b+" " + ra + " " + rb + " " + nc + " " + (ra*rb -nc) );											
				return new pair_part (-1, -1);
			
					
			case 2: // split
			case 3: // add
			case 4: // swap 	
				
				double ur = rd.NextDouble ();
				int n = 0, u = 0, v = 0;
				if (pDict.ContainsKey (st)) {
					u = pDict [st].Count;
					n += u;
				}
				if (pDict.ContainsKey (st.swap ())) {
					v = pDict [st.swap ()].Count;
					n += v;
				}
				if (ur * n < u)
					return pDict [st] [(int)(ur * n)];
				else {
					var m = pDict [st.swap ()];

					return m [(int)(ur * n - u)].swap ();
				}
					

			}			
		}


		public bool apply(reactor rec,pair_part ret){


			particle a = rec.get_particle_id(ret.id1);
			particle b = rec.get_particle_id(ret.id2);
			type_state old_a = a.st;
			type_state old_b = b.st;

			switch (function) {

			case 0:
			default: 
				return false;
			case 1: //link
				rec.link (a, b);
				break;
			case 2: // split 
				rec.unlink (a, b);
				break;
			case 3: // add 
				type_state new_a = new type_state(a.st.type,a.st.state+1);
				type_state new_b = new type_state(b.st.type,b.st.state+1);
				rec.update_new_state (a, old_a,new_a);
				rec.update_new_state (b, old_b,new_b);			
				break;
			case 4: // swap 
				rec.update_new_state (a, old_a,old_b);
				rec.update_new_state (b, old_b,old_a);	
				break;
			}

			return true;
		}


		public override bool apply_rule(reactor rec, Random rd){

			var ret = pick_reactants (rec.stDict, rec.pDict, rd);
			if (ret.id1 == -1)
				return false;
			return apply (rec, ret);
		}

		#endregion

	}		

}

