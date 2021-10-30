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
		public static partial Quadruple Exp(Quadruple x)
		{
			if (IsNaN(x))
				return x;
			int sign = x.Sign;
			if (!IsFinite(x))
			{
				if (sign == 0)
					return x;
				else
					return Zero;
			}
			throw new NotImplementedException();
		}
	}
}
