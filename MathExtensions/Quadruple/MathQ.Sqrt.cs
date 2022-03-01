using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MathExtensions.Quadruple;

namespace MathExtensions
{
	static unsafe partial class MathQ
	{
		public static partial Quadruple Sqrt(Quadruple x)
		{
			if (IsNaN(x))
				return x;
			if (IsNegative(x))
				return NaN;
			if (IsPositiveInfinity(x))
				return x;
			int exponent = x.Exp != 0 ? x.Exp - Bias : 1 - Bias;
			bool isOdd = (exponent & 1) == 1;
			int newExp = exponent / 2;
			UInt128 s = GetSignificand(x);
			UInt256 iSqrt = ISqrt((UInt256)s << 112);
			s = (UInt128)iSqrt;
			int shift = UInt128.Log2(s) - 112;
			newExp -= shift;
			if (newExp <= -0x3FFF)
				return Zero;
			s <<= shift;
			s &= FractionMask;
			s |= (UInt128)(newExp + Bias) << 112;
			Quadruple q = FromUInt128(s);
			return isOdd ? SQRT2 * q : q;
		}

		static readonly UInt128 _pow4_128 = (UInt128)1 << 126;
		public static partial UInt128 ISqrt(UInt128 x)
		{
			UInt128 res, bit;
			if (x < UInt128.Zero)
				throw new ArithmeticException("Value cannot be negative");
			res = UInt128.Zero;
			bit = _pow4_128;
			while (bit > x)
				bit >>= 2;
			while (bit != UInt128.Zero)
			{
				if (x >= res + bit)
				{
					x -= res + bit;
					res = (res >> 1) + bit;
				}
				else
					res >>= 1;
				bit >>= 2;
			}
			return res;
		}

		static readonly UInt256 _pow4_256 = (UInt256)1 << 254;
		public static partial UInt256 ISqrt(UInt256 x)
		{
			UInt256 res, bit;
			if (x < UInt256.Zero)
				throw new ArithmeticException("Value cannot be negative");
			res = UInt256.Zero;
			bit = _pow4_256;
			while (bit > x)
				bit >>= 2;
			while (bit != UInt256.Zero)
			{
				if (x >= res + bit)
				{
					x -= res + bit;
					res = (res << 1) + bit;
				}
				else
					res >>= 1;
				bit >>= 2;
			}
			return res;
		}
	}
}
