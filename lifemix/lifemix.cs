using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using QuickGraph;
using QuickGraph.Algorithms;
using greactor;


namespace lifemix
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			int Seed = Int32.Parse (args [0]);
			double rho = double.Parse(args[1]);

			int Seed2 = Int32.Parse (args [2]);

			int tmax = 3, smax = 5;

			double max_time = double.Parse (args [3]);

			int nparticles = Int32.Parse (args [4]);
			double pl = double.Parse (args [5]);
			double rate = double.Parse (args [6]);
			var lr = new lifereactor (tmax, smax, rho, Seed, rate);
			double tau;
			double total_time = 0.0;

			Random rd = new Random (Seed2)	;
			int step = 0;
			int idx;

			for (int i = 0; i <nparticles; i++) {
				int ns = (int)(rd.NextDouble () * smax);

				int nt = (int)(rd.NextDouble () * tmax + 1);

				lr.rec.add_particle (new particle (nt, ns));
			}

			
			foreach (particle p1 in lr.rec.particles) {
				foreach (particle p2 in lr.rec.particles) {
					if (p1 != p2) {

						double o = rd.NextDouble ();
						if (o < pl) {
							lr.rec.link (p1, p2);
						}							
					}

				}
			}

			var g0 = lr.rec.create_graph ();
			IDictionary<int,int> components0 = new Dictionary<int,int> ();
			int count0 = g0.WeaklyConnectedComponents (components0);
			List<particle>[] u0 = new List<particle>[count0];
			for (int i = 0; i < count0; i++)
				u0 [i] = new List<particle> ();

			foreach (KeyValuePair<int,int> kv in components0) {			
				u0 [kv.Value].Add (lr.rec.get_particle_id (kv.Key));				
			}
			int tk0 = 0;
			for (int i = 0; i < count0; i++)
				tk0 += u0 [i].Count;
			
				
		/*	foreach (rule r in lr.rec.rules) {
				Console.WriteLine (r);
			} */

			while (lr.rec.gillespie_step (rd, out tau, out idx)) {

				total_time += tau;
				step++;
				if (step > max_time)
					break;
				//Console.WriteLine (step+ " " + total_time + " " + idx + " " + lr.rec.get_rule(idx).idx + " " +  lr.rec.get_rule(idx));
			}

			var g = lr.rec.create_graph ();
			IDictionary<int,int> components = new Dictionary<int,int> ();
			int count = g.WeaklyConnectedComponents (components);
			List<particle>[] u = new List<particle>[count];
			for (int i = 0; i < count; i++)
				u [i] = new List<particle> ();

			foreach (KeyValuePair<int,int> kv in components) {			
				u [kv.Value].Add (lr.rec.get_particle_id (kv.Key));				
			}
			int tk = 0;
			for (int i = 0; i < count; i++)
				tk += u [i].Count;
			
			Console.WriteLine (Seed + " " + Seed2 + " " + nparticles + " " + rho + " "+ rate +" " + pl + " " + total_time + " " + step + " " + count + " " + tk/(float)count + " " + count0 + " " + tk0/(float)count0);

			//lr.rec.export_graphviz ("out"+ Seed + "_" + Seed2 +"_"+nparticles+ "_" + rho + "_"+ pl+"_" + rate + ".dot");
		}
	}
}
