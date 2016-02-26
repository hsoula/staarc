using System;

namespace greactor
{
	public struct pair_part
	{
		public int id1,id2;
		public pair_part (int id1,int id2)
		{

			this.id1 = id1;
			this.id2 = id2;
			/*
			this.id1 = id1>=id2 ? id1 : id2;
			this.id2 = id2<=id1 ? id2 : id1;
*/
		}

		public pair_part swap(){

			return new pair_part (this.id2, this.id1);			
		}
	}
}

