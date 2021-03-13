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
		public static partial Quadruple Ceiling(Quadruple x)
		{
			throw new NotImplementedException();
		}
		public static partial Quadruple Floor(Quadruple x)
		{
			throw new NotImplementedException();
		}

		public static partial Quadruple Round(Quadruple x)
		{
			throw new NotImplementedException();
		}
		public static partial Quadruple Round(Quadruple x, int digits)
		{
			throw new NotImplementedException();
		}
		public static partial Quadruple Round(Quadruple x, int digits, MidpointRounding mode)
		{
			throw new NotImplementedException();
		}
		public static partial Quadruple Round(Quadruple x, MidpointRounding mode)
		{
			throw new NotImplementedException();
		}

		public static unsafe partial Quadruple Truncate(Quadruple x)
		{ 
			if (!IsFinite(x))
				return x;
			int exp = x.Exp - Bias;
			if (exp > SignificandBits)
				return x;
			if (exp < 0)
				return Zero;
			UInt128 mask = BigIntHelpers.Int128MasksHighBits[SignificandBits - exp - 1];
			UInt128 s = ((QuadUnion*)&x)->i;
			s &= mask;
			return ((QuadUnion*)&s)->x;
		}
	}
}
