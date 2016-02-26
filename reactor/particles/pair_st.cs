using System;

namespace greactor
{
	public struct pair_st
	{
		public type_state a;
		public type_state b;

		public pair_st (type_state a,type_state b)
		{
		/*	if (a.type <= b.type) {
				if (a.state <= b.state) {*/
					this.a = a;
					this.b = b;
			/*
				} else {
					this.a = b;
					this.b = a;
				}
			} else {
				this.a = b;
				this.b = a;

			}*/
		}
		public pair_st(particle a, particle b){

			this.a = a.st;
			this.b = b.st;
		}

		public pair_st swap(){

			return new pair_st (this.b, this.a);			
		}

		public override string ToString ()
		{
			return a + "-" + b;
		}
	}
}

