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
		private static Quadruple Multiply(Quadruple left, Quadruple right)
		{
			// Quick exit
			bool leftInfinity = IsInfinity(left);
			bool rightInfinity = IsInfinity(right);
			if (EitherNaN(left, right) || (leftInfinity && right == Zero) || (left == Zero && rightInfinity))
				return NaN;

			int leftSign = left.Sign;
			int rightSign = right.Sign;
			int sign = leftSign ^ rightSign;

			if (leftInfinity || rightInfinity)
				return sign == 1 ? NegativeInfinity : PositiveInfinity;

			int lExp = left.Exp;
			int rExp = right.Exp;
			int exponent = lExp + rExp - Bias;
			if (exponent < 0)
				return sign == 1 ? NegativeZero : Zero;

			UInt128 lMantissa = GetSignificand(left);
			UInt128 rMantissa = GetSignificand(right);

			UInt256 mul = (UInt256)lMantissa * rMantissa;
			int shift = UInt256.HighestBit(mul) - (2 * SignificandBits);

			exponent += shift;
			if (shift >= 0)
				mul >>= shift;
			else
				mul <<= -shift;

			// Handle special cases
			if (exponent > 0x7FFE)
				return sign == 1 ? NegativeInfinity : PositiveInfinity;
			if (exponent < 0x0001)
				return sign == 1 ? NegativeZero : Zero;

			UInt128 m = (UInt128)(mul >> 112);
			m &= FractionMask;
			return new Quadruple(BitHelper.GetHigh(&m._u1) | ((uint)exponent << 16) | ((uint)sign << 31), BitHelper.GetLow(&m._u1), BitHelper.GetHigh(&m._u0), BitHelper.GetLow(&m._u0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Quadruple Divide(Quadruple left, Quadruple right)
		{
			// Quick exits
			bool leftInfinity = IsInfinity(left);
			bool rightInfinity = IsInfinity(right);
			if (EitherNaN(left, right) || (leftInfinity & rightInfinity))
				return NaN;

			int leftSign = left.Sign;
			int rightSign = right.Sign;
			if (right == Zero)
			{
				if (left == Zero)
					return NaN;
				return leftSign == 1 ? NegativeInfinity : PositiveInfinity;
			}

			int sign = leftSign ^ rightSign;
			if (leftInfinity || rightInfinity)
				return sign == 1 ? NegativeInfinity : PositiveInfinity;

			int lExp = left.Exp;
			int rExp = right.Exp;
			int exponent = lExp - rExp + Bias;
			if (exponent < 0x0001)
				return sign == 1 ? NegativeZero : Zero;

			UInt256 lMantissa = new UInt256(UInt128.Zero, GetSignificand(left));
			UInt128 rMantissa = GetSignificand(right);

			UInt256 div = lMantissa / rMantissa;

			int shift = UInt256.HighestBit(div) - 128;
			exponent += shift;
			if (shift >= 0)
				div >>= shift;
			else
				div <<= -shift;

			// Handle special cases
			if (exponent > 0x7FFE)
				return sign == 1 ? NegativeInfinity : PositiveInfinity;
			if (exponent < 0x0001)
				return sign == 1 ? NegativeZero : Zero;

			UInt128 d = (UInt128)(div >> 16);
			d &= FractionMask;
			return new Quadruple(BitHelper.GetHigh(&d._u1) | ((uint)exponent << 16) | ((uint)sign << 31), BitHelper.GetLow(&d._u1), BitHelper.GetHigh(&d._u1), BitHelper.GetLow(&d._u0));
		}

		private static readonly UInt256 _stickyAddSubMask = UInt128.MaxValue >> 2;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Quadruple Add(Quadruple left, Quadruple right)
		{
			// Quick exits
			if (EitherNaN(left, right))
				return NaN;
			int lExp = left.Exp;
			if (lExp == 0)
				return right;
			int rExp = right.Exp;
			if (rExp == 0)
				return left;
			int exponent = Math.Max(lExp, rExp);

			UInt256 l = new UInt256(UInt128.Zero, GetSignificand(left));
			UInt256 r = new UInt256(UInt128.Zero, GetSignificand(right));

			int diff = lExp - rExp;
			if (diff > 0)
				r >>= diff;
			else if (diff < 0)
				l >>= -diff;

			int lSign = left.Sign;
			int rSign = right.Sign;
			if ((lSign ^ rSign) != 0)
			{
				if (lSign == 1)
					l = UInt256.TwosComplement(l);
				else
					r = UInt256.TwosComplement(r);
			}

			UInt256 sum = l + r;
			int sign = UInt256.HighestBit(sum) >> 7;
			if (sign == 1)
				sum = UInt256.TwosComplement(sum);

			int highBit = UInt256.HighestBit(sum) - 128;
			if (highBit == 0)
				return Zero;
			int shift = highBit - SignificandBits;
			exponent += shift;

			if (shift > 0)
				sum >>= shift;
			else
				sum <<= -shift;

			// Handle special cases
			if (exponent < 0x0001)
				return sign == 1 ? NegativeZero : Zero;
			if (exponent > 0x7FFE)
				return sign == 1 ? PositiveInfinity : NegativeInfinity;

			UInt128 s = (UInt128)(sum >> 128);
			s &= FractionMask;
			return new Quadruple(BitHelper.GetHigh(&s._u1) | ((uint)exponent << 16) | ((uint)(sign ^ lSign) << 31), BitHelper.GetLow(&s._u1), BitHelper.GetHigh(&s._u0), BitHelper.GetLow(&s._u0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Quadruple Subtract(Quadruple left, Quadruple right)
		{
			// Quick exits
			if (EitherNaN(left, right))
				return NaN;
			int lExp = left.Exp;
			if (lExp == 0)
				return right;
			int rExp = right.Exp;
			if (rExp == 0)
				return left;

			int exponent = Math.Max(lExp, rExp);
			UInt256 l = new UInt256(UInt128.Zero, GetSignificand(left));
			UInt256 r = new UInt256(UInt128.Zero, GetSignificand(right));

			int diff = lExp - rExp;
			if (diff > 0)
				r >>= diff;
			if (diff < 0)
				l >>= -diff;

			int lSign = left.Sign;
			int rSign = right.Sign;
			if ((lSign ^ rSign) != 0)
			{
				if (lSign == 1)
					l = UInt256.TwosComplement(l);
				else
					r = UInt256.TwosComplement(r);
			}

			UInt256 difference = l - r;
			int sign = UInt256.HighestBit(difference) >> 7;
			if (sign == 1)
				difference = UInt256.TwosComplement(difference);

			int highBit = UInt256.HighestBit(difference) - 128;

			int shift = highBit - SignificandBits;
			exponent += shift;

			if (shift > 0)
				difference >>= shift;
			else
				difference <<= -shift;

			// Handle special cases
			if (exponent < 0x0001)
				return sign == 1 ? NegativeZero : Zero;
			if (exponent > 0x7FFE)
				return sign == 1 ? PositiveInfinity : NegativeInfinity;

			UInt128 d = (UInt128)(difference >> 128);
			d &= FractionMask;
			return new Quadruple(BitHelper.GetHigh(&d._u1) | ((uint)exponent << 16) | ((uint)(sign ^ lSign) << 31), BitHelper.GetLow(&d._u1), BitHelper.GetHigh(&d._u0), BitHelper.GetLow(&d._u0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static UInt128 GetSignificand(Quadruple quad)
		{
			const int mask = (1 << 16) - 1;
			const int normalBit = 1 << 16;
			if (IsSubnormal(quad))
				return UInt128.Zero;
			return new UInt128(((((ulong)quad._b3 & mask) | normalBit) << 32) | quad._b2, (ulong)quad._b1 << 32 | quad._b0);
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
					sbits = (int)s._u0 & 0x007F_FFFF;
					return BitConverter.Int32BitsToSingle((sign << 31) | sbits);
				}
				s >>= NormalMantissaBits - significandBits - 1;
				sbits = (int)s._u0 & 0x007F_FFFF;
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
				// TODO: Redo arithmetic involving casting UInt128* to ulong*
				UInt128 s = GetSignificand(q);
				long sbits;
				if (exp < 1 - bias)
				{
					int diff = -exp - bias;
					s >>= NormalMantissaBits - significandBits + diff;
					sbits = (long)((ulong*)&s)[0] & 0x000F_FFFF_FFFF_FFFF;
					return BitConverter.Int64BitsToDouble((sign << 63) | sbits);
				}
				s >>= NormalMantissaBits - significandBits - 1;
				sbits = (long)((ulong*)&s)[0] & 0x000F_FFFF_FFFF_FFFF;
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
		internal static bool EitherNaN(in Quadruple left, in Quadruple right) => IsNaN(left) || IsNaN(right);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool NeitherNaN(in Quadruple left, in Quadruple right) => !(IsNaN(left) || IsNaN(right));
	}
}