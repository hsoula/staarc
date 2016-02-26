using System;

namespace greactor
{
	public class ensemble
	{
		public int size;
		public type_state[] parts;
		public int[,] links;

		public ensemble (int s, type_state[] p, int[,] l)
		{
			size = s;
			parts = new type_state[s];
			for (int i = 0; i < s; i++) {
				parts [i] = p [i];
			}
			links = new int[s, s];
			for (int i = 0; i < s; i++) {
				for (int j = 0; j < s; j++) {
					links [i, j] = l [i, j];
				}
			}
		}
		public override string ToString ()
		{
			return size + " " + parts [0];
		}

	}
}

