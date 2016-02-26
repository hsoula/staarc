using System;
using System.Collections.Generic;
using QuickGraph;
using QuickGraph.Algorithms;

namespace greactor
{
	/// <summary>
	/// Reactor. class than contains reactors
	/// </summary>
	public class reactor
	{

		double EPSILON = 1e-10;
		static public char[] letters = new char[8]{ 'z','a', 'b', 'c', 'd', 'e', 'f', 'g' };
		static public string[] colors = new string[8]{ "black","red", "blue", "green", "gray", "yellow", "cyan", "white" };
		public List<particle> particles;
		public Dictionary<int, particle> ids;

		public List<rule> rules;
		double collisionRate;
		double conformationRate;

		public int npart { get{ return particles.Count; } }
		public int nrule { get{ return rules.Count; } }

		public Dictionary<type_state,List<int>> stDict;
		public Dictionary<pair_st,List<pair_part>> pDict;

		#region Constructor 
		public reactor ()
		{
			particles = new List<particle> ();
			ids = new Dictionary<int, particle> ();
			rules = new List<rule> ();
			stDict = new Dictionary<type_state,List<int>>();
			pDict = new Dictionary<pair_st,List<pair_part>>();
		}

		#endregion
	
		#region Get Utilities
		public particle get_particle_id(int id){

			if (ids.ContainsKey (id))
				return ids [id];
			return null;
		}

		public particle get_particle(int n){

		/*	if (n >= particles.Count)
				return null;*/
			return get_particle_id (n);
			//return particles [n];
		}

		public rule get_rule(int n){

			if (n >= rules.Count)
				return null;
			return rules [n];
		}

		#endregion

		#region Propensities and Gillespie Code 

		public double [] get_propensities(out double a0){

			var prop = new double[rules.Count];
			a0 = 0.0;
			
			for (int i = 0; i < rules.Count; i++) {
				rule r = rules [i];						
				prop [i] = r.propensity(this);		
				a0 += prop [i];	
			}
			return prop;
		}


		public double chose_rule(Random rd, out rule r){


			double a0;
			double[] a = get_propensities(out a0);
			if (a0 < EPSILON) {
				r = new b_rule();
				return 0.0;
			}


			double tau = -Math.Log (rd.NextDouble ()) / a0;
			int n = -1, nr = rules.Count;
			double s = 0, rx = rd.NextDouble ();
			while (n < nr-1 && s < rx) {
				n++;
				s += a [n] / a0;		
			}
			r = rules[n];
			return tau;
		}


		public bool gillespie_step(Random rd, out double tau, out int rule_idx){
					
			rule r;
			rule_idx = -1;
			tau = chose_rule (rd, out r);		
			if (tau  != 0.0) {	
	
				rule_idx = r.id;
				bool  o = r.apply_rule (this, rd);
				return o;
			}
			return false;
		}
		#endregion


		#region Reactor Manipulations




		public void update_new_state(particle a,type_state old_state, type_state new_state){

			foreach (int pid in a.linked) {
				
				particle p = get_particle_id (pid);
				pair_st st1 = new pair_st (old_state, p.st);
				List<pair_part> l;
	
				if (pDict.ContainsKey (st1)) {
					l = pDict [st1];
			//		Console.WriteLine (st1);
					l.RemoveAll (x => x.id1 == a.id && x.id2 == p.id);
					l.RemoveAll (x => x.id2 == a.id && x.id1 == p.id);

				} 			
				pair_st st2 = new pair_st (p.st,old_state);
				if (pDict.ContainsKey(st2))
					{
					l = pDict [st2];
			//		Console.WriteLine ("swap:"+st2);
					l.RemoveAll (x => x.id1 == a.id && x.id2 == p.id);
					l.RemoveAll (x => x.id2 == a.id && x.id1 == p.id);
				}
			//	Console.WriteLine ("update :" + a + "O:" + old_state + "N:" + new_state + " " + p); 
			
				pair_st pst = new pair_st (new_state, p.st);
				if(!pDict.ContainsKey(pst)) pDict.Add(pst,new List<pair_part>());
				var ol = new pair_part (a.id, p.id);
				pDict [pst].Add (ol);
				//if (pDict.ContainsKey(ol.swap())
					
			}
		//	Console.WriteLine ("#-" + a.id + " " + stDict [old_state].Count+ " " + old_state);
			stDict [old_state].Remove (a.id);
		//	Console.WriteLine ("#-" + a.id + " " + stDict [old_state].Count+ " " + old_state);

			a.st = new_state;
			if (!stDict.ContainsKey (new_state))
				stDict [new_state] = new List<int> ();
			stDict [new_state].Add (a.id);


		}

	

		public void link(int k, int r){

			particle a = get_particle (k), b = get_particle (r);
			link ( a,b );
		}

		public void unlink(int k, int r){

			particle a = get_particle (k), b = get_particle (r);
			unlink (a, b);
		}

		public void all_unlink(int k){

			particle a = get_particle (k);
			
			foreach (int r in a.linked) {				
				particle b = get_particle (r);						
				unlink (a, b);
				b.linked.Remove (k);
			}
			a.linked.Clear();		
		}
	


		public void link(particle a, particle b){
			if (a == null)
				return;
			if (b == null)
				return;

			a.link (b);
			pair_st s = new pair_st (a, b);
			if (!pDict.ContainsKey (s)) {
				pDict.Add (s, new List<pair_part> ());
			}
			var par = new pair_part(a.id,b.id);
			pDict [s].Add (par);
		}

		public void unlink(particle a, particle b){
			if (a == null)
				return;
			if (b == null)
				return;			
			
			pair_st s = new pair_st (a, b);
			if (pDict.ContainsKey (s)) {

				pair_part n = new pair_part (-1, -1);

				foreach (pair_part l in pDict[s]) {

					if ((l.id1 == a.id) && (l.id2 == b.id)) {
						n = l;
						break;
					}
					if ((l.id2 == a.id) && (l.id1 == b.id)) {
						n = l;
						break;
					}
				}
				if (n.id1 != -1)
					pDict [s].Remove(n);
			}
		}

		#endregion
		#region Display Function 
		public void displayParticlesLinks(){

			foreach (pair_st s in pDict.Keys) {
				Console.WriteLine (s + "-" + pDict [s].Count);
			}

		}

		public void displayParticlesType(){

			foreach (type_state s in stDict.Keys) {
				if (stDict [s].Count>0)
					Console.WriteLine (s + "-" + stDict [s].Count);
			}

		}

		#endregion


		public int check_consistency(){

			foreach (particle o in particles) {
				foreach (int r in o.linked) {
					particle b = get_particle (r);
					if (b == null)
						return o.id;
				}
			}
			return -1;
		}
		#region Remove Utilities
		public void remove_particle(particle o){

		//	Console.WriteLine ("delete: " + o+  " " +  o.linked.Count);
			if (particles.Contains (o)) {

					if (o.linked.Count > 0) {
					
					foreach (int r in o.linked) {
						particle b = get_particle (r);
				//		Console.Write ("d: " + b);
						unlink (o, b);					
						b.linked.Remove (o.id);					
					}
			//		Console.WriteLine ();
				}
				o.linked.Clear ();
			// 	Console.WriteLine ("delete: " + o+  " " +  o.linked.Count);
				if (stDict.ContainsKey (o.st))					
					stDict [o.st].Remove (o.id);
				ids.Remove(o.id);
				particles.Remove (o);			
			}
		}

		#endregion


		#region Add Utilities
		public void add_particle(particle o){
			if (!particles.Contains (o)) {
				particles.Add (o);
				ids.Add (o.id, o);

				if (!stDict.ContainsKey (o.st))
					stDict.Add (o.st, new List<int> ());
				stDict [o.st].Add (o.id);

				foreach (int pid in o.linked) {
					particle p = get_particle_id (pid);
					pair_st s = new pair_st (o, p);
					if (!pDict.ContainsKey (s)) {
						pDict.Add (s, new List<pair_part> ());
					}
					var par = new pair_part (o.id, p.id);					
					pDict [s].Add (par);			
				}

			}
		}
		public void insert_ensemble(ensemble e){
			int s = e.size;
			particle[] p = new particle[s];

			for (int i = 0; i < s; i++) {
				int ty = e.parts [i].type;
				int st = e.parts [i].state;
				p [i] = new particle (ty, st);
				add_particle (p [i]);
			}


			for (int i = 0; i < s; i++) {
				for (int j = i+1; j < s; j++) {
					if (e.links [i, j] == 1) {
						link (p [i], p [j]);
					}
				}
			}
		}

		public void add_rule(rule o){
			if (!rules.Contains (o))
				rules.Add (o);			
		}

		#endregion

		#region Export Methods

		public void export_adjacency_matrix(string fname){
			using(System.IO.StreamWriter str = new System.IO.StreamWriter(fname)){

				foreach(particle p in particles){
					foreach(particle q in particles){
						if (p.islink(q)	)						
							str.Write ("1 ");
						else
							str.Write ("0 ");
					}
					str.Write("\n");
				}
			}	

		}

		public void export_particles_list(string fname){
			using(System.IO.StreamWriter str = new System.IO.StreamWriter(fname)){

				foreach (particle p in particles) {
					str.WriteLine (p.st);
				}
			}	


		}

		public void export_graphviz(string fname){
			using(System.IO.StreamWriter str = new System.IO.StreamWriter(fname)){
				int np, nq;
				str.WriteLine ("graph G{ \n margin = 0.0;bgcolor=transparent; node [shape=circle];");
				for (np = 0; np < particles.Count; np++) {
					particle p = particles [np];
					if (p.linked.Count!=0)
						str.WriteLine (p + "[ style=filled,color=" + p.st.color () + "];");
				}
				for (np = 0; np< particles.Count; np++){
					for (nq = np + 1; nq < particles.Count; nq++) {

						particle p = particles [np];
						if (p.linked.Count != 0) {
							particle q = particles [nq];
							if (p.islink (q))
								str.WriteLine (p + "--" + q + ";");
						}
					}
				}
				str.WriteLine("}");
			}
		}	


		#endregion


		#region Graph Routines 

		public AdjacencyGraph<int,Edge<int>> create_graph(){

			var g = new AdjacencyGraph<int, Edge<int>> ();
			foreach (particle p in particles) {
				g.AddVertex (p.id);
			}
			foreach (particle p in particles) {
				foreach (int x in p.linked) {
					var ed = new Edge<int> (p.id, x);
					g.AddEdge (ed);
				}
			}

			return g;
		}


		public AdjacencyGraph<string, Edge<string>> create_part_cycle_graph(){

			var g = new  AdjacencyGraph<string, Edge<string>> ();

			foreach (rule r in rules) {

				b_rule b = r as b_rule;
				if (b != null) {

					string a1 = b.reactants.a.ToString ();
					string a2 = b.reactants.b.ToString ();

					string b1 = b.products.a.ToString ();
					string b2 = b.products.b.ToString ();
					if (!g.ContainsVertex (a1)) {
						g.AddVertex (a1);
				//		Console.WriteLine (a1);
					}
					if (!g.ContainsVertex (a2)) {
						g.AddVertex (a2);
				//		Console.WriteLine (a2);
					}
					if (!g.ContainsVertex (b1)) {
						g.AddVertex (b1);
				//		Console.WriteLine (b1);
					}
					if (!g.ContainsVertex (b2)) {
						g.AddVertex (b2);
				//		Console.WriteLine (b2);
					}
					if (!g.ContainsEdge (a1, b1)) {
						Edge<string> e = new Edge<string> (a1, b1);	
						g.AddEdge (e);
				//		Console.WriteLine(a1+"->"+b1);
					}
				

					if (!g.ContainsEdge (a2, b2)) {
						Edge<string> f = new Edge<string> (a2, b2);	
						g.AddEdge (f);
				//		Console.WriteLine(a2+"->"+b2);
					}
				

				}

			}
						
			return g;

		}

		public AdjacencyGraph<string, Edge<string>> create_part_interaction_graph(){

			var g = new  AdjacencyGraph<string, Edge<string>> ();


			return g;

		}


		#endregion
	}
}

