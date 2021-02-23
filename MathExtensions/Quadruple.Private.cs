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
		private static bool Equals(Quadruple left, Quadruple right) => !(IsNaN(left) || IsNaN(right)) && ((long*)left._b)[0] == ((long*)right._b)[0] && ((long*)left._b)[1] == ((long*)right._b)[1];
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetHashCode(Quadruple quad) => HashCode.Combine(((long*)quad._b)[0], ((long*)quad._b)[1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static short GetExp(Quadruple quad) => (short)(((short*)quad._b)[7] & 0x7fff);

		private static readonly UInt256 _stickyMulMask = ((UInt256)1 << 110) - 1;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Quadruple Multiply(Quadruple left, Quadruple right)
		{
			if (IsNaN(left) || IsNaN(right) || (IsInfinity(left) && right == Zero) || (left == Zero && IsInfinity(right)))
				return NaN;
			int sign = left._sign ^ right._sign;
			int lExp = left._exp == 0 ? 1 : left._exp;
			int rExp = right._exp == 0 ? 1 : right._exp;
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
			if (IsNaN(left) || IsNaN(right) || (IsInfinity(left) && IsInfinity(right)))
				return NaN;
			if (right == Zero)
			{
				if (left == Zero)
					return NaN;
#pragma warning disable CS8509 // The switch statement will always return a value. (_sign can only be 0 or 1)
				return left._sign switch
				{
					0 => PositiveInfinity,
					1 => NegativeInfinity
				};
#pragma warning restore CS8509
			}
			int sign = left._sign ^ right._sign;
			int lExp = left._exp == 0 ? 1 : left._exp;
			int rExp = right._exp == 0 ? 1 : right._exp;
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
			if (IsNaN(left) || IsNaN(right))
				return NaN;
			int lExp = left._exp == 0 ? 1 : left._exp;
			int rExp = right._exp == 0 ? 1 : right._exp;
			int exponent = Math.Max(lExp, rExp) - Bias;
			UInt256 l = new UInt256(UInt128.Zero, GetSignificand(left));
			UInt256 r = new UInt256(UInt128.Zero, GetSignificand(right));
			if ((left._sign ^ right._sign) != 0)
			{
				if (left._sign == 1)
					l = UInt256.TwosComplement(l);
				else
					r = UInt256.TwosComplement(r);
			}
			int diff = Math.Abs(lExp - rExp);
			if (lExp > rExp)
			{
				if (r[255])
					r = r >> diff | (UInt256.MaxValue << diff);
				else
					r >>= diff;
			}
			else if (rExp > lExp)
			{
				if (l[255])
					l = l >> diff | (UInt256.MaxValue << diff);
				else
					l >>= diff;
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
			if (IsNaN(left) || IsNaN(right))
				return NaN;
			int lExp = left._exp == 0 ? 1 : left._exp;
			int rExp = right._exp == 0 ? 1 : right._exp;
			int exponent = Math.Max(lExp, rExp) - Bias;
			UInt256 l = new UInt256(UInt128.Zero, GetSignificand(left));
			UInt256 r = new UInt256(UInt128.Zero, GetSignificand(right));
			if ((left._sign ^ right._sign) != 0)
			{
				if (left._sign == 1)
					l = UInt256.TwosComplement(l);
				else
					r = UInt256.TwosComplement(r);
			}
			int diff = Math.Abs(lExp - rExp);
			if (lExp > rExp)
			{
				if (r[255])
					r = r >> diff | (UInt256.MaxValue << diff);
				else
					r >>= diff;
			}
			else if (rExp > lExp)
			{
				if (l[255])
					l = l >> diff | (UInt256.MaxValue << diff);
				else
					l >>= diff;
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

		private static UInt128 RoundToEven(UInt128 value, byte grs)
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
		private static UInt128 GetSignificand(Quadruple quad)
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

		private static Quadruple FromExponent(int exponent)
		{
			exponent = Math.Clamp(exponent + Bias, 0, short.MaxValue);
			UInt128 u = new UInt128(0, (ulong)exponent << 48);
			return FromUInt128(u);
		}


		internal static string FormatQuadruple(Quadruple value, string? format, NumberFormatInfo info)
		{
			ValueStringBuilder sb = new ValueStringBuilder(stackalloc char[32]);
			return QuadrupleParsing.FormatQuadruple(ref sb, value, format, info) ?? sb.ToString();
		}
	}
}