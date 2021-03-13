using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MathExtensions.Quadruple;

namespace MathExtensions
{
	partial class MathQ
	{
		public static partial Quadruple FusedMultiplyAdd(Quadruple x, Quadruple y, Quadruple z)
		{
			if (EitherNaN(x, y) || IsNaN(z) || (IsInfinity(x) && y == Zero) || (x == Zero && IsInfinity(y)))
				return NaN;
			int sign = x.Sign ^ y.Sign;
			int xExp = x.Exp == 0 ? 1 : x.Exp;
			int yExp = y.Exp == 0 ? 1 : y.Exp;
			long exponent = xExp + yExp - 2 * Bias;
			UInt128 xMantissa = GetSignificand(x);
			UInt128 yMantissa = GetSignificand(y);
			UInt256 mul = (UInt256)xMantissa * yMantissa;
			throw new NotImplementedException(mul.ToString());
			
		}
	}
}
