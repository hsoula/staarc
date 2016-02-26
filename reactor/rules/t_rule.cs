using System;
using System.Collections.Generic;


namespace greactor
{
	/// <summary>
	/// Tri molecular reactions 
	///  (t1|s1)(.|s)(t2|s2) + (t3|s3) -> (t4|s4)(.|+)(t5|s5)(.|+)(t6|s6)
	/// </summary>
	public class t_rule : rule
	{

		public pair_st reactants;
		public type_state trireac; 
		public pair_st products1;
		public pair_st products2;
		public pair_st products3;

		public bool reactionContact;
		public bool productContact;
		public bool enzymeContact1;
		public bool enzymeContact2;

		#region Constructor and helpers

		public t_rule (bool a, int rt1, int rs1,int rt2, int rs2, int enz_t1, int enz_s1, bool b, int pt1, int ps1,int pt2, int ps2, int enz_t2, int enz_s2, bool c, bool d, int idx, double rate)
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

			trireac.type = enz_t1;
			trireac.state = enz_s1;

			type_state e2;
			e2.type = enz_t2;
			e2.state = enz_s2;


			reactionContact = a;
			productContact = b;
			enzymeContact1 = c;
			enzymeContact2 = c;

			reactants = new pair_st (r1, r2);
			products1 = new pair_st (p1, p2);				
			products2 = new pair_st (p1, e2);
			products3 = new pair_st (p2, e2);

			this.idx = idx;
			this.rate = rate;							
		}
		#endregion

		#region Propensity 

		public override double propensity(reactor rec){
			var pDict = rec.pDict;
			var stDict = rec.stDict;

			pair_st st = reactants;

			if (reactionContact == true) {

				// looking for linked reactants as in pair 

				int count = 0;
				if (pDict.ContainsKey (st))
					count += pDict [st].Count;
				pair_st st_swap = st.swap ();
				if (pDict.ContainsKey (st_swap))
					count += pDict [st_swap].Count;

				// counting the number of reactants 
				int na = stDict.ContainsKey (trireac) ? stDict [trireac].Count : 0;

				// we need to remove already linked 
				pair_st a1 = new pair_st(reactants.a, trireac);

				int rem = 0;
				if (pDict.ContainsKey (a1)) {
					foreach (pair_part p in pDict[a1]) {
						particle p1 = rec.get_particle_id(p.id1);
						particle p2 = rec.get_particle_id(p.id2);
						if (p1.st == trireac) {
							foreach (int id in p1.linked) {
								particle o = rec.get_particle_id (id);
								if (o.st == reactants.b)
									rem += 1;
							}							
						} else {
							foreach (int id in p2.linked) {
								particle o = rec.get_particle_id (id);
								if (o.st == reactants.b)
									rem += 1;
								;
							}
						}
					}
				}

				
				return rate * (count * na - rem); 

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
								

				return rate * ntot;
			}
		}
		#endregion

	}
}

