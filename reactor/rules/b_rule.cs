using System;
using System.Collections.Generic;

namespace greactor
{
	
	public class b_rule : rule
	{
		/// <summary>
		///  bi molecular rule b_rule 
		///  (t1|s1) (+.)(t2|s2)  -> (t3|s3) (+.)(t4|s4) : rate 
		/// </summary>
	
		static public double kappa = -1.0;
		public pair_st reactants;
		public pair_st products;
		public bool reactionContact;
		public bool productContact;

		public static  List<b_rule> generate_rules(int ntmax,bool a, int rt1, int rs1,int  rt2,int rs2,bool b, int ps1, int ps2, int idx,double lambda){

			var lr = new List<b_rule> ();
			double rate = a == true ? 1.0 : lambda;

			if ((rt1 >= 0) && (rt2 >= 0)) {

				lr.Add (new b_rule (a, rt1, rs1, rt2, rs2, b, rt1, ps1, rt2, ps2,idx,rate));
				return lr;
			} else {

				if (rt1 == rt2) {
					for (int t1 = 0; t1 < ntmax; t1++) {

						lr.Add (new b_rule (a, t1, rs1, t1, rs2, b, t1, ps1, t1, ps2,idx,rate));
					}
					return lr;
				} else {
					
					for (int t1 = 0; t1 < ntmax; t1++) {
						for (int t2 = 0; t2< ntmax; t2++) {
						lr.Add (new b_rule (a, t1, rs1, t2, rs2, b, t1, ps1, t2, ps2,idx,rate));
						}
					}

					return lr;
				}
			}
		}


		public b_rule (bool a, int rt1, int rs1,int rt2, int rs2,bool b, int pt1, int ps1,int pt2, int ps2, int idx, double rate)
		{
			type_state r1; 
			r1.state = rs1;
			r1.type = rt1;

			type_state r2; 
			r2.state = rs2;
			r2.type = rt2;
					
			type_state p1; 
			p1.state = ps1;
			p1.type = pt1;

			type_state p2; 
			p2.state = ps2;
			p2.type = pt2;

			reactionContact = a;
			productContact = b;
			reactants = new pair_st (r1, r2);
			products = new pair_st (p1, p2);				

			this.idx = idx;
			this.rate = rate;

		//	Console.WriteLine (this+" "+this.idx + " " + idx);
		}

		public b_rule(){
			
		}
			
		public override string ToString ()
		{ 
			string c = reactionContact ? "." : "+";
			string d = productContact ? "." : "+";
			string s=  reactor.letters[reactants.a.type]+""+reactants.a.state + c + reactor.letters[reactants.b.type]+""+reactants.b.state + "->"+reactor.letters[products.a.type]+""+products.a.state + d + reactor.letters[products.b.type]+""+products.b.state;
			return s;
		}

		#region Propensity 

		public override double propensity(Dictionary<type_state,List<int>> stDict, Dictionary<pair_st,List<pair_part>> pDict){

			pair_st st = reactants;
			if (reactionContact == true) {
				int count = 0;
				if (pDict.ContainsKey (st))
					count += pDict [st].Count;
				pair_st st_swap = st.swap ();
				if (pDict.ContainsKey (st_swap))
					count += pDict [st_swap].Count;
			
				return rate * count;

			}
			else {
				type_state a = st.a;
				type_state b = st.b;
				int na = stDict.ContainsKey (a) ? stDict [a].Count : 0;
				int nb = stDict.ContainsKey (b) ? stDict [b].Count : 0;
				int nc = pDict.ContainsKey (st) ? pDict [st].Count : 0;
				nc = pDict.ContainsKey (st.swap()) ? pDict [st.swap()].Count : 0;
				if (a.state == b.state && a.type ==b.type )
					nb += -1;
				int ntot = na * nb - nc;

				if (ntot < 0)
					return 0.0;
				
				if (kappa > 0.0) {

					return kappa * rate * ntot / (kappa + ntot);
				}
				
				return rate * ntot;
			}
		}


		public override double propensity(reactor rec){

			return propensity (rec.stDict, rec.pDict);
		}
		#endregion

		#region Reaction routines

		public pair_part pick_reactants(Dictionary<type_state,List<int>> stDict, Dictionary<pair_st,List<pair_part>> pDict,Random rd){

			pair_st st = reactants;

			if (reactionContact == true) {

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

			} else {

				type_state a = st.a;
				type_state b = st.b;

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
			}			
		}


		public bool apply(reactor rec,pair_part ret){


			particle a = rec.get_particle_id(ret.id1);
			particle b = rec.get_particle_id(ret.id2);
			type_state old_a = a.st;
			type_state old_b = b.st;
			//	Console.WriteLine ("A:" + (old_a.state == r.reactants.b.state));
			pair_st st = products;
			type_state new_a = st.a;
			type_state new_b = st.b;
			if (old_a != new_a)
				rec.update_new_state (a, old_a, new_a);
			if (old_b!=new_b)
				rec.update_new_state (b , old_b, new_b);				

			if (productContact)
				rec.link (a, b);
			else {
				a.unlink (b);
				rec.unlink (a, b);

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


		#region Creation routines

		public static  List<rule> create_all_rules(int ntmax,int nsmax, double p, double lambda, Random rd){

			var lr = new List<rule> ();
			int idx = 0;
			for (int contact = 0; contact < 2; contact++) {
				for (int link = 0; link < 2; link++) {
					for (int t1 = 0; t1 < ntmax; t1++) {
						for (int s1 = 0; s1 < nsmax; s1++) {
							for (int t2 = t1; t2 < ntmax; t2++) {
								for (int s2 = 0; s2 < nsmax; s2++) {
									for (int ps1 = 0; ps1 < nsmax; ps1++) {
										for (int ps2 = 0; ps2 < nsmax; ps2++) {
											if (rd.NextDouble() < p) {
												bool a = contact == 0;
												bool b = link == 0;
												double rate = a == true ? 1.0 : lambda;
												lr.Add (new b_rule (a, t1, s1, t2, s2, b, t1, ps1, t2, ps2, idx, rate));
										//		Console.WriteLine (idx + " " +p);
												idx++;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return lr;
		}	





		#endregion
	}


}



