using System;
using System.Collections.Generic;

namespace greactor
{
	/// <summary>
	///  Main Particle class
	/// </summary>
	public class particle
	{

		static int gid = 0;
		public int id;
		public type_state st;
		public List<int> linked;

		public particle (int t,int s)
		{
			id = gid;
			gid++;

			st.state = s;
			st.type = t;
			linked = new List<int> ();
		}
		public bool islink(particle o){

			return linked.Contains (o.id);
		}
		public void link(particle o){

			if (!linked.Contains (o.id)) {
				linked.Add (o.id);
				if (!o.linked.Contains(this.id))
					o.linked.Add (this.id);
			}
		}

		public void unlink(particle o){

			if (linked.Contains (o.id)) {
				linked.Remove(o.id);
				if (o.linked.Contains(this.id))
					o.linked.Remove (this.id);
			}
		}

		public override string ToString ()
		{
			return st+"_"+id;
		}
	}
}

