using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using QuickGraph;
using QuickGraph.Algorithms;
using greactor;

namespace replicator
{
	class MainClass
	{


		public static void graph(reactor rec, int seed, double lambda, int nb){
			rec.export_graphviz ("out_" + seed + "_" + lambda + "_" + nb + ".dot");
			// rec.export_particles_list ("part.rec");

			var g = rec.create_graph ();

			IDictionary<int,int> components = new Dictionary<int,int> ();
			int count = g.WeaklyConnectedComponents (components);

			Console.Write (count + " " + rec.npart + " " + ((double)count) / (rec.npart));
			List<particle>[] u = new List<particle>[count];
			for (int i = 0; i < count; i++)
				u [i] = new List<particle> ();

			foreach (KeyValuePair<int,int> kv in components) {			
				u [kv.Value].Add (rec.get_particle_id (kv.Key));				
			}

			int tk = 0;
			for (int i = 0; i < count; i++)
				tk += u [i].Count;
			Console.WriteLine (" " + ((float)tk) / count);
			for (int i = 0; i < count; i++) {
				List<particle> l = u [i];
				particle p = l.Find (x => x.st.type == 5);
				if (p != null) {

					particle current = p;

					bool done = true;
					while (done) {
						Console.Write (current.st);
						l.Remove (current);
						if (current.linked.Count > 0) {
							int nc = current.linked.Count;
							int z = 0;
							while (z < nc && !l.Contains (rec.get_particle_id (current.linked [z]))) {
								z++;
							}
							if (z == nc)
								done = false;
							else {
								current = rec.get_particle_id (current.linked [z]);							
							}
						}				
					}
					Console.WriteLine ("");
				} else {
					foreach (particle r in l) {
						Console.Write (r.st);
					}
					Console.WriteLine ("");
				}
			}


		}


		public static void Main (string[] args)
		{


			int seed = Int32.Parse (args [0]);
			double lambda = double.Parse (args [1]);
			int nb = Int32.Parse (args [2]);
			//double kappa = double.Parse (args [3]);

			string js_name = args [3];
			string json;
			using (System.IO.StreamReader r = new System.IO.StreamReader (js_name)) {
				json = r.ReadToEnd ();
			}
			
			ensemble e = JsonConvert.DeserializeObject<ensemble> (json);


			reactor rec = new reactor ();
			for(int x = 0; x < nb; x++)
				rec.insert_ensemble (e);


			var lr0 = b_rule.generate_rules (7, false, 5, 8, 5, 0, true, 4, 3,1,lambda);
			var lr1 = b_rule.generate_rules (7, true, -1, 4, -2, 1, true, 2, 5,2,lambda);
			var lr2 = b_rule.generate_rules (7, false, -1, 5, -1, 0, true, 7, 6,3,lambda);
			var lr3 = b_rule.generate_rules (7, false, -1, 3, -2, 6, true, 2, 13, 4,lambda);
			var lr4 = b_rule.generate_rules (7, true, -1, 7, -2, 13, true, 4, 3,5,lambda);
			var lr5 = b_rule.generate_rules (7, true, 6, 4, 6, 3, false, 8, 8,6,lambda);
			var lr6 = b_rule.generate_rules (7, true, -1, 2, -2, 8, true, 9, 1,7,lambda);
			var lr7 = b_rule.generate_rules (7, true, -1, 9, -2, 9, false, 8, 8, 8,lambda);

			foreach(b_rule r in lr0) rec.add_rule(r);
			foreach(b_rule r in lr1) rec.add_rule(r);		
			foreach(b_rule r in lr2) rec.add_rule(r);
			foreach(b_rule r in lr3) rec.add_rule(r);
			foreach(b_rule r in lr4) rec.add_rule(r);
			foreach(b_rule r in lr5) rec.add_rule(r);
			foreach(b_rule r in lr6) rec.add_rule(r);
			foreach(b_rule r in lr7) rec.add_rule(r);

			double lambda_p = 1e-6;
			for (int i = 1; i < 7; i++) {
				var er0 = new o_rule (i, 0, lambda_p); 
				rec.add_rule (er0);
			}

			double lambda_d = 1e-7;
			for (int i = 1; i < 7; i++) {
				for (int j = 0; j < 1; j++) {
					
					var er0 = new d_rule (i, j, lambda_d, 0);
					rec.add_rule (er0);
				}
			}
		

			for (int i = 0; i < 10; i++) {

				rec.add_particle (new particle (1, 0));
				rec.add_particle (new particle (2, 0));
				rec.add_particle (new particle (3, 0));
				rec.add_particle (new particle (4, 0)); 
				rec.add_particle (new particle (5, 0));
				rec.add_particle (new particle (6, 0));
			}

			double tau;
			double total_time = 0.0;
			Random rd = new Random (seed);
			int step = 0;
			int idx;
			int ndiv = 0;

			while (rec.gillespie_step (rd,out tau, out idx)) {

				total_time +=tau;
				step++;
				int npart = rec.particles.Count;
				if (npart>100000)
					break;

				Console.WriteLine (step+ " " + npart + " " + ndiv + " " + total_time + " " + idx + " " + rec.get_rule(idx).idx + " " +  rec.get_rule(idx));
				if (idx == 245)
					ndiv ++;

				if (step % 1000 == 0) {
					var g = rec.create_graph ();
					IDictionary<int,int> components = new Dictionary<int,int> ();
					int count = g.WeaklyConnectedComponents (components);
					List<particle>[] u = new List<particle>[count];
					for (int i = 0; i < count; i++)
						u [i] = new List<particle> ();

					foreach (KeyValuePair<int,int> kv in components) {			
						u [kv.Value].Add (rec.get_particle_id (kv.Key));				
					}

					using(System.IO.StreamWriter str = new System.IO.StreamWriter("out"+step+".out")){				
						for (int i = 0; i < count; i++)					
							str.Write(u [i].Count + " ");
					}
					rec.export_graphviz ("out_" + seed + "_" + lambda + "_" + nb + "_" + step + ".dot");
				}

				
				
			}						
		/*	var g = rec.create_graph ();

			IDictionary<int,int> components = new Dictionary<int,int> ();
			int count = g.WeaklyConnectedComponents (components);
			List<particle>[] u = new List<particle>[count];
			for (int i = 0; i < count; i++)
				u [i] = new List<particle> ();

			foreach (KeyValuePair<int,int> kv in components) {			
				u [kv.Value].Add (rec.get_particle_id (kv.Key));				
			}

			using(System.IO.StreamWriter str = new System.IO.StreamWriter("out.out")){				
				for (int i = 0; i < count; i++)					
					str.Write(u [i].Count + " ");
			}

			int tk = 0;
			for (int i = 0; i < count; i++)
				tk += u [i].Count;
				*/

/*			Console.Write(lambda + " " + nb + " " + kappa + " " + seed + " " + step+" " + total_time+ " " + count + " " + rec.npart + " " + ((double)count) / (rec.npart));
			Console.Write (" " + ndiv);
			Console.WriteLine (" " + ((float)tk) / count);
*/
			rec.export_graphviz ("out_" + seed + "_" + lambda + "_" + nb + ".dot");
		}
	}
}
