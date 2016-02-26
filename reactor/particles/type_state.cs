using System;

namespace greactor
{
	/// <summary>
	/// class state type that stores state and type
	/// </summary>
	public struct type_state
	{
		public int state ;
		public int type ;

		public override string ToString ()
		{
			return ""+reactor.letters[type]+state+"";
		}

		public string color(){

			return reactor.colors [type];
		}

		public static bool operator == (type_state a, type_state b){

			return a.type == b.type && a.state == b.state;

		}

		public static bool operator != (type_state a, type_state b){

			return !(a == b);

		}


		public type_state(int t, int s){

			state = s;
			type = t;
		}
	}
}

