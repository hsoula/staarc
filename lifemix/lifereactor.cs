using System;
using System.Collections.Generic;
using greactor;

namespace lifemix
{
	public class lifereactor
	{
		/// <summary>
		/// max number of type conversion 
		/// </summary>
		public int tmax; 
		/// <summary>
		/// max number of available state 
		/// </summary>
		public int smax; 
		/// <summary>
		/// fraction of random kept rules
		/// </summary>
		public double fraction;
		/// <summary>
		/// seed for random generation 
		/// </summary>
		public int seed;
		/// <summary>
		/// main reactor 
		/// </summary>
		public reactor rec;
		/// <summary>
		/// main random generator
		/// </summary>
		Random rd;
		/// <summary>
		/// main init <see cref="lifemix.lifereactor"/> class using seed+fraction to eliminate 
		/// rules and add them to the reactor.
		/// </summary>
		/// <param name="tm"> max type </param>
		/// <param name="sm"> max state </param>
		/// <param name="f"> fraction (0 < x < 1.0) </param>
		/// <param name="s"> seed for replay </param>
		public lifereactor (int tm, int sm, double f, int s, double r)
		{
			/// init members
			tmax = tm;
			smax = sm;
			fraction = f;
			seed = s;

			/// then init random generator 
			rd = new Random (seed);

			/// create reactor
			rec = new reactor ();

			int idx = 0;
			

			/// prepare the loop for all possible reaction 
			/// and thin it according to fraction 
			for (int t1 = 0; t1 < tmax; t1++) {
				for (int s1 = 0; s1 < smax; s1++) {
					for (int t2 = 0; t2 < tmax; t2++) {
						for (int s2 = 0; s2 < smax; s2++) {
							for (int s3 = 0; s3 < smax; s3++) {
								for (int s4 = 0; s4 < smax; s4++) {
									for (int c = 0; c < 2; c++) {
										for (int p = 0; p < 2; p++) {
											if ((s1 != s3) || (s2 != s4) || (c != p)) {
												double o = rd.NextDouble ();
												if (o < fraction) {												
													var ru = new b_rule (c == 1, t1 + 1, s1, t2 + 1, s2, p == 1, t1 + 1, s3, t2 + 1, s4, idx, r);
													idx++;
													rec.add_rule (ru);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}

		/*	for (int t1 = 0; t1 < tmax; t1++) {
				for (int s1 = 0; s1 < smax; s1++) {
					for (int t2 = 0; t2 < tmax; t2++) {
						for (int s2 = 0; s2 < smax; s2++) {
							double o = rd.NextDouble ();
							if (o < fraction) {
								var ru = new m_rule (t1, s1, t2, s2, false, r, ++idx);
								rec.add_rule (ru);
							}
						}
					}
				}
			}
			*/	
		}
	}
}

