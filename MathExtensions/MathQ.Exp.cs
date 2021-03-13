using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MathExtensions.Quadruple;

namespace MathExtensions
{
	partial class MathQ
	{
		public static partial Quadruple Exp(Quadruple q)
		{
			if (IsNaN(q))
				return q;
			int sign = q.Sign;
			if (!IsInfinity(q))
			{
				if (sign == 0)
					return q;
				else
					return Zero;
			}
			return default;
		}
	}
}
