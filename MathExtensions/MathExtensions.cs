using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public static class MathExtensions
	{
		public static int DivideRoundAway(int a, int b)
		{
			int p = b > 0 ? b - 1 : b + 1;
			return (a + (a > 0 ? p : -p)) / b;
		}

		public static int DivideRoundNearest(int a, int b)
		{
			int p, q;
			if (b < 0)
			{
				p = 1 - b;
				q = 1;
			}
			else
			{
				p = 1 + b;
				q = -1;
			}
			if (a < 0)
				return (a + p) / b + q;
			else
				return (a - p) / b + q;
		}
	}
}
