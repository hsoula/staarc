using NUnit.Framework;
using System;
using greactor;

namespace greactor_test
{
	[TestFixture ()]
	public class Test
	{

		[Test ()]
		public void TestPairST ()
		{
			int t = 0, s = 1;

			particle p = new particle (t, s);
			particle q = new particle (t, s);

			pair_st pst = new pair_st (p, q);
			pair_st pstn = new pair_st (p.st, q.st);

			Assert.AreEqual (pst.a, pstn.a);
			Assert.AreEqual (pst.b, pstn.b);

		}
		[Test ()]
		public void TestReactor ()
		{
			
			reactor rec = new reactor ();

			/// new configuration tests
			Assert.AreEqual (0, rec.particles.Count);
			Assert.AreEqual (0, rec.rules.Count);

			int t = 0, s = 1;
			type_state key = new type_state(t,s);
			particle p = new particle (t, s);
			rec.add_particle (p);

			// test add particles 
			Assert.AreEqual (1, rec.particles.Count);
			Assert.IsTrue(rec.stDict.ContainsKey(key));
			Assert.AreEqual (1, rec.stDict [key].Count);

			// check link -> no links 
			Assert.AreEqual (0, rec.pDict.Count);

			// double add 
			rec.add_particle (p);
			Assert.AreEqual (1, rec.particles.Count);


			// two particles 
			particle q = new particle (t, s);


			// test add 
			rec.add_particle (q);
			Assert.AreEqual (2, rec.particles.Count);

			// test dictionnaries 
			Assert.IsTrue(rec.stDict.ContainsKey(key));
			Assert.AreEqual (2, rec.stDict [key].Count);

			// test remove 

			rec.remove_particle (q);

			Assert.AreEqual (1, rec.particles.Count);
			Assert.IsTrue(rec.stDict.ContainsKey(key));
			Assert.AreEqual (1, rec.stDict [key].Count);


			rec.remove_particle (p);

			Assert.AreEqual (0, rec.particles.Count);
			Assert.IsTrue(rec.stDict.ContainsKey(key));
			Assert.AreEqual (0, rec.stDict [key].Count);


			// test link 
			rec.add_particle (q);
			rec.add_particle (p);

			Assert.AreEqual (2, rec.particles.Count);
			Assert.IsTrue(rec.stDict.ContainsKey(key));
			Assert.AreEqual (2, rec.stDict [key].Count);

			// create pair part 

			pair_st pst = new pair_st (p, q);
			pair_st pstn = new pair_st (p.st, q.st);

			Assert.AreEqual (pst.a, pstn.a);
			Assert.AreEqual (pst.b, pstn.b);


			rec.link (p, q);

			Assert.AreEqual (2, rec.particles.Count);
			Assert.IsTrue(rec.stDict.ContainsKey(key));
			Assert.AreEqual (2, rec.stDict [key].Count);

			Assert.IsTrue (rec.pDict.ContainsKey (pst));
			Assert.AreEqual (1, rec.pDict[pst].Count);

		}
	}
}

