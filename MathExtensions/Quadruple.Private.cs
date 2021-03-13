using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace MathExtensions
{
	public unsafe partial struct Quadruple
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Equals(Quadruple left, Quadruple right) => NeitherNaN(left, right) && ((long*)left._b)[0] == ((long*)right._b)[0] && ((long*)left._b)[1] == ((long*)right._b)[1];
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetHashCode(Quadruple quad) => HashCode.Combine(((long*)quad._b)[0], ((long*)quad._b)[1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static short GetExp(Quadruple quad) => (short)(((short*)quad._b)[7] & 0x7fff);

		private static readonly UInt256 _stickyMulMask = ((UInt256)1 << 110) - 1;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Quadruple Multiply(Quadruple left, Quadruple right)
		{
			if (EitherNaN(left, right) || (IsInfinity(left) && right == Zero) || (left == Zero && IsInfinity(right)))
				return NaN;
			int sign = left.Sign ^ right.Sign;
			int lExp = left.Exp == 0 ? 1 : left.Exp;
			int rExp = right.Exp == 0 ? 1 : right.Exp;
			long exponent = lExp + rExp - 2 * Bias;
			UInt128 lMantissa = GetSignificand(left);
			UInt128 rMantissa = GetSignificand(right);
			UInt256 mul = (UInt256)lMantissa * rMantissa;
			byte grs = (byte)(((mul._u[3] & 0xC000U) >> 29) | (((mul & _stickyMulMask) != 0) ? 1U : 0U));
			UInt128 m = RoundToEven((UInt128)(mul >> 112), grs);
			int shift = UInt128.GetHighestBit(m) - 112;
			exponent += shift;
			m >>= shift;
			if (exponent < -0x4000)
				return Zero;
			if (exponent < MinExponent)
				exponent = MinExponent;
			if (exponent > MaxExponent)
			{
				m = UInt128.Zero;
				exponent = InfinityExponent;
			}
			m &= FractionMask;
			m |= (UInt128)((exponent + Bias) | (uint)sign << 15) << 112;
			return FromUInt128(m);
		}

		private static readonly UInt256 _stickyDivMask = ((UInt256)1 << 14) - 1;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Quadruple Divide(Quadruple left, Quadruple right)
		{
			if (EitherNaN(left, right) || (IsInfinity(left) && IsInfinity(right)))
				return NaN;
			if (right == Zero)
			{
				if (left == Zero)
					return NaN;
#pragma warning disable CS8509 // The switch statement will always return a value. (Sign can only be 0 or 1)
				return left.Sign switch
				{
					0 => PositiveInfinity,
					1 => NegativeInfinity
				};
#pragma warning restore CS8509
			}
			int sign = left.Sign ^ right.Sign;
			int lExp = left.Exp == 0 ? 1 : left.Exp;
			int rExp = right.Exp == 0 ? 1 : right.Exp;
			long exponent = lExp - rExp;
			UInt256 lMantissa = new UInt256(UInt128.Zero, GetSignificand(left));
			UInt128 rMantissa = GetSignificand(right);
			UInt256 div = lMantissa / rMantissa;
			//TODO: Add proper rounding.
			//bool roundingBit = div[15];
			byte grs = (byte)(((((ushort*)div._u)[0] & 0xC000U) >> 13) | (((div & _stickyDivMask) != 0) ? 1U : 0U));
			UInt128 d = RoundToEven((UInt128)(div >> 16), grs);
			int shift = UInt128.GetHighestBit(d) - 112;
			exponent += shift;
			d >>= shift;
			if (exponent < -0x4000)
				return Zero;
			if (exponent < MinExponent)
				exponent = MinExponent;
			if (exponent > MaxExponent)
			{
				d = UInt128.Zero;
				exponent = InfinityExponent;
			}
			d &= FractionMask;
			d |= (UInt128)((exponent + Bias) | (uint)sign << 15) << 112;
			return FromUInt128(d);
		}
		private static UInt256 _stickyAddSubMask = UInt128.MaxValue >> 2;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Quadruple Add(Quadruple left, Quadruple right)
		{
			if (EitherNaN(left, right))
				return NaN;
			int lExp = left.Exp == 0 ? 1 : left.Exp;
			int rExp = right.Exp == 0 ? 1 : right.Exp;
			int exponent = Math.Max(lExp, rExp) - Bias;
			UInt256 l = new UInt256(UInt128.Zero, GetSignificand(left));
			UInt256 r = new UInt256(UInt128.Zero, GetSignificand(right));
			int diff = Math.Abs(lExp - rExp);
			if (lExp > rExp)
				r >>= diff;
			else if (rExp > lExp)
				l >>= diff;
			if ((left.Sign ^ right.Sign) != 0)
			{
				if (left.Sign == 1)
					l = UInt256.TwosComplement(l);
				else
					r = UInt256.TwosComplement(r);
			}
			UInt256 sum = l + r;
			int sign = sum[255] ? 1 : 0;
			if (sign == 1)
				sum = UInt256.TwosComplement(sum);
			byte grs = (byte)(((sum._u[3] & 0xC000_0000U) >> 29) | ((sum & _stickyAddSubMask) != 0 ? 1U : 0U));
			UInt128 s = RoundToEven((UInt128)(sum >> 128), grs);
			int highBit = UInt128.GetHighestBit(s);
			if (highBit == 0)
				return Zero;
			int shift = highBit - SignificandBits;
			exponent += shift;
			s >>= shift;
			if (exponent < -0x4000)
				return Zero;
			if (exponent < MinExponent)
				exponent = MinExponent;
			if (exponent > MaxExponent)
			{
				s = UInt128.Zero;
				exponent = InfinityExponent;
			}
			s &= FractionMask;
			s |= (UInt128)((exponent + Bias) | sign << 15) << SignificandBits;
			return FromUInt128(s);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Quadruple Subtract(Quadruple left, Quadruple right)
		{
			if (EitherNaN(left, right))
				return NaN;
			int lExp = left.Exp == 0 ? 1 : left.Exp;
			int rExp = right.Exp == 0 ? 1 : right.Exp;
			int exponent = Math.Max(lExp, rExp) - Bias;
			UInt256 l = new UInt256(UInt128.Zero, GetSignificand(left));
			UInt256 r = new UInt256(UInt128.Zero, GetSignificand(right));
			int diff = Math.Abs(lExp - rExp);
			if (lExp > rExp)
				r >>= diff;
			else if (rExp > lExp)
				l >>= diff;
			if ((left.Sign ^ right.Sign) != 0)
			{
				if (left.Sign == 1)
					l = UInt256.TwosComplement(l);
				else
					r = UInt256.TwosComplement(r);
			}
			UInt256 difference = l - r;
			int sign = difference[255] ? 1 : 0;
			if (sign == 1)
				difference = UInt256.TwosComplement(difference);
			byte grs = (byte)(((difference._u[3] & 0xC000_0000U) >> 29) | ((difference & _stickyAddSubMask) != 0 ? 1U : 0U));
			UInt128 d = RoundToEven((UInt128)(difference >> 128), grs);
			int highBit = UInt128.GetHighestBit(d);
			if (highBit == 0)
				return Zero;
			int shift = highBit - SignificandBits;
			exponent += shift;
			d >>= shift;
			if (exponent < -0x4000)
				return Zero;
			if (exponent < MinExponent)
				exponent = MinExponent;
			if (exponent > MaxExponent)
			{
				d = UInt128.Zero;
				exponent = InfinityExponent;
			}
			d &= FractionMask;
			d |= (UInt128)((exponent + Bias) | sign << 15) << SignificandBits;
			return FromUInt128(d);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static UInt128 RoundToEven(UInt128 value, byte grs)
		{
			if ((grs & 0b100) == 0)
				return value;
			if (grs == 0b100)
			{
				if (value[0])
					return value + 1;
				else
					return value;
			}
			else
				return value + 1;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static UInt128 GetSignificand(Quadruple quad)
		{
			const long mask = (1L << 48) - 1;
			const long normalBit = 1L << 48;
			ulong* p = (ulong*)quad._b;
			ulong lo = p[0];
			ulong hi = p[1] & mask;
			if (!IsSubnormal(quad))
				hi |= normalBit;
			return new UInt128(lo, hi);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static UInt128 AsUInt128(Quadruple quad) => *(UInt128*)&quad;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Quadruple FromUInt128(UInt128 u) => *(Quadruple*)&u;


		internal static string FormatQuadruple(Quadruple value, string? format, NumberFormatInfo info)
		{
			ValueStringBuilder sb = new ValueStringBuilder(stackalloc char[32]);
			return QuadrupleParsing.FormatQuadruple(ref sb, value, format, info) ?? sb.ToString();
		}

		private static int ConvertToInt32(Quadruple q)
		{
			const int significandBits = 31;
			int sign = q.Sign;
			int exponent = q.Exp - Bias;
			if (exponent < 0 || exponent > significandBits)
				return 0;
			UInt128 s = GetSignificand(q) >> (SignificandBits - exponent);
			if (sign == 0)
				return (int)(uint)s;
			else
				return ~(int)(uint)s + 1;
		}
		private static uint ConvertToUInt32(Quadruple q)
		{
			const int significandBits = 32;
			int sign = q.Sign;
			int exponent = q.Exp - Bias;
			if (exponent < 0 || exponent > significandBits || sign == 1)
				return 0;
			UInt128 s = GetSignificand(q) >> (SignificandBits - exponent);
			return (uint)s;
		}
		private static long ConvertToInt64(Quadruple q)
		{
			const int significandBits = 63;
			int sign = q.Sign;
			int exponent = q.Exp - Bias;
			if (exponent < 0 || exponent > significandBits)
				return 0;
			UInt128 s = GetSignificand(q) >> (SignificandBits - exponent);
			if (sign == 0)
				return (long)(ulong)s;
			else
				return ~(long)(ulong)s + 1;
		}

		private static ulong ConvertToUInt64(Quadruple q)
		{
			const int significandBits = 64;
			int sign = q.Sign;
			int exponent = q.Exp - Bias;
			if (exponent < 0 || exponent > significandBits || sign == 1)
				return 0;
			UInt128 s = GetSignificand(q) >> (SignificandBits - exponent);
			return (ulong)s;
		}

		private static UInt128 ConvertToUInt128(Quadruple q)
		{
			const int significandBits = 128;
			int sign = q.Sign;
			int exponent = q.Exp - Bias;
			if (exponent < 0 || exponent > significandBits || sign == 1)
				return UInt128.Zero;
			UInt128 s = GetSignificand(q) >> (SignificandBits - exponent);
			return s;
		}

		private static UInt256 ConvertToUInt256(Quadruple q)
		{
			const int significandBits = 256;
			int sign = q.Sign;
			int exponent = q.Exp - Bias;
			if (exponent < 0 || exponent > significandBits || sign == 1)
				return UInt256.Zero;
			UInt256 s = ((UInt256)GetSignificand(q)) >> (SignificandBits - exponent);
			return s;
		}

		public static float ConvertToSingle(Quadruple q)
		{
			const int bias = 127;
			const int significandBits = 23;
			if (IsNaN(q))
				return float.NaN;
			int sign = q.Sign;
			int e = q.Exp;
			int exp = e != 0 ? e - Bias : 1 - Bias;
			if (exp < 1 - bias - significandBits)
			{
				if (sign == 1)
					return -0f;
				else
					return 0f;
			}
			else if (exp > bias)
			{
				if (sign == 1)
					return float.NegativeInfinity;
				else
					return float.PositiveInfinity;
			}
			else
			{
				UInt128 s = GetSignificand(q);
				int sbits;
				if (exp < 1 - bias)
				{
					int diff = -exp - bias;
					s >>= NormalMantissaBits - significandBits + diff;
					sbits = (int)s._u[0] & 0x007F_FFFF;
					return BitConverter.Int32BitsToSingle((sign << 31) | sbits);
				}
				s >>= NormalMantissaBits - significandBits - 1;
				sbits = (int)s._u[0] & 0x007F_FFFF;
				return BitConverter.Int32BitsToSingle((sign << 31) | ((exp + bias) << significandBits) | sbits);
			}
		}

		public static double ConvertToDouble(Quadruple q)
		{
			const int bias = 1023;
			const int significandBits = 52;
			if (IsNaN(q))
				return double.NaN;
			long sign = q.Sign;
			int e = q.Exp;
			int exp = e != 0 ? e - Bias : 1 - Bias;
			if (exp < 1 - bias - significandBits)
			{
				if (sign == 1)
					return -0d;
				else
					return 0d;
			}
			else if (exp > bias)
			{
				if (sign == 1)
					return double.NegativeInfinity;
				else
					return double.PositiveInfinity;
			}
			else
			{
				UInt128 s = GetSignificand(q);
				long sbits;
				if (exp < 1 - bias)
				{
					int diff = -exp - bias;
					s >>= NormalMantissaBits - significandBits + diff;
					sbits = (long)((ulong*)s._u)[0] & 0x000F_FFFF_FFFF_FFFF;
					return BitConverter.Int64BitsToDouble((sign << 63) | sbits);
				}
				s >>= NormalMantissaBits - significandBits - 1;
				sbits = (long)((ulong*)s._u)[0] & 0x000F_FFFF_FFFF_FFFF;
				return BitConverter.Int64BitsToDouble((sign << 63) | ((long)(exp + bias) << significandBits) | sbits);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int CompareTo(Quadruple left, Quadruple right)
		{
			bool lNaN = IsNaN(left);
			bool rNaN = IsNaN(right);
			if (left == right || (lNaN && rNaN))
				return 0;
			if (lNaN)
				return -1;
			if (rNaN)
				return 1;
			if (IsZero(left) && IsZero(right))
				return 0;
			int lSign = left.Sign;
			int rSign = right.Sign;
			if ((lSign ^ rSign) != 0)
				return rSign - lSign;
			int expDiff = left.Exp - right.Exp;
			int compare;
			if (expDiff == 0)
				compare = CompareSignificandsUnsafe((ulong*)left._b, (ulong*)right._b);
			else
				compare = expDiff;
			return lSign == 0 ? compare : -compare;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int CompareToInternal(Quadruple left, Quadruple right)
		{
			if (left == right)
				return 0;
			if (IsZero(left) && IsZero(right))
				return 0;
			int lSign = left.Sign;
			int rSign = right.Sign;
			if ((lSign ^ rSign) != 0)
				return rSign - lSign;
			int expDiff = left.Exp - right.Exp;
			int compare;
			if (expDiff == 0)
				compare = CompareSignificandsUnsafe((ulong*)left._b, (ulong*)right._b);
			else
				compare = expDiff;
			return lSign == 0 ? compare : -compare;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int CompareSignificandsUnsafe(ulong* sl, ulong* sr)
		{
			const ulong highMask = 0xFFFF_FFFF_FFFF;
			long highDiff = unchecked((long)(sl[1] & highMask) - (long)(sr[1] & highMask));
			if (highDiff != 0)
				return highDiff > 0 ? 1 : -1;
			long lowDiff = unchecked((long)sl[0] - (long)sr[0]);
			if (lowDiff != 0)
				return lowDiff > 0 ? 1 : -1;
			return 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool EitherNaN(Quadruple left, Quadruple right) => IsNaN(left) || IsNaN(right);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool NeitherNaN(Quadruple left, Quadruple right) => !(IsNaN(left) || IsNaN(right));
	}
}