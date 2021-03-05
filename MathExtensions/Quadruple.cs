using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using static MathExtensions.Extensions;

namespace MathExtensions
{
	/// <summary>
	/// An Implementation of IEEE 754-2008 binary128 Quadruple precision floating point numbers.
	/// </summary>
	[ReadOnly(true)]
	public unsafe partial struct Quadruple : IComparable, IComparable<Quadruple>, IEquatable<Quadruple>, IFormattable
	{
		fixed byte _b[16];

		internal int Sign { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => (_b[15] & 0x80) >> 7; }
		internal short Exp { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => GetExp(this); }

		internal const int Bias = 16383;
		internal const int SignificandBits = 112;
		internal const int SubnormalMantissaBits = 112;
		internal const int NormalMantissaBits = 113;
		internal const int MaxExponent = 0x3FFF;
		internal const int MinExponent = -0x3FFF;
		internal const int InfinityExponent = 0x4000;
		internal static readonly UInt128 FractionMask = new UInt128(0xFFFF_FFFF, 0xFFFF_FFFF, 0xFFFF_FFFF, 0x0000_FFFF);
		public static readonly Quadruple Epsilon = new Quadruple(0x0000_0000_0000_0000, 0x0000_0000_0000_0001);
		public static readonly Quadruple MaxValue = new Quadruple(0x7FFE_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF);
		public static readonly Quadruple MinValue = new Quadruple(0xFFFE_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF);
		public static readonly Quadruple NaN = new Quadruple(0xFFFF_8000_0000_0000, 0x0000_0000_0000_0000);
		public static readonly Quadruple NegativeInfinity = new Quadruple(0xFFFF_0000_0000_0000, 0x0000_0000_0000_0000);
		public static readonly Quadruple PositiveInfinity = new Quadruple(0x7FFF_0000_0000_0000, 0x0000_0000_0000_0000);
		public static readonly Quadruple NegativeZero = new Quadruple(0x8000_0000_0000_0000, 0x0000_0000_0000_0000);
		public static readonly Quadruple Zero = new Quadruple(0x0000_0000_0000_0000, 0x0000_0000_0000_0000);
		public static readonly Quadruple One = new Quadruple(0x3FFF_0000_0000_0000, 0x0000_0000_0000_0000);

		public static bool IsFinite(Quadruple q) => q.Exp != 0x7FFF;
		public static bool IsInfinity(Quadruple q) => q.Exp == 0x7FFF && ((ulong*)q._b)[0] == 0 && (((ulong*)q._b)[1] & 0x0000_FFFF_FFFF_FFFF) == 0;
		public static bool IsNaN(Quadruple q) => q.Exp == 0x7FFF && !(((ulong*)q._b)[0] == 0 && (((ulong*)q._b)[1] & 0x0000_FFFF_FFFF_FFFF) == 0);
		public static bool IsNegative(Quadruple q) => ((q._b[15] & 0x80) >> 7) == 1;
		public static bool IsNegativeInfinity(Quadruple q) => ((ulong*)q._b)[0] == 0xFFFF_0000_0000_0000 && ((ulong*)q._b)[1] == 0;
		public static bool IsNormal(Quadruple q) => 0x7FFF > q.Exp && q.Exp > 0;
		public static bool IsPositive(Quadruple q) => q.Sign == 0;
		public static bool IsPositiveInfinity(Quadruple q) => ((ulong*)q._b)[0] == 0x7FFF_0000_0000_0000 && ((ulong*)q._b)[1] == 0;
		public static bool IsSubnormal(Quadruple q) => q.Exp == 0;
		internal static bool IsZero(Quadruple q) => ((ulong*)q._b)[0] == 0 && (((ulong*)q._b)[1] & 0x7FFF_FFFF_FFFF_FFFF) == 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Quadruple(ulong hi, ulong lo)
		{
			fixed (void* buf = _b)
			{
				((ulong*)buf)[0] = lo;
				((ulong*)buf)[1] = hi;
			}
		}

		public Quadruple(ReadOnlySpan<byte> span)
		{
			fixed (void* p = _b)
			{
				Span<byte> s = new Span<byte>(p, sizeof(Quadruple));
				span.Slice(0, Math.Min(sizeof(Quadruple), span.Length)).CopyTo(s);
			}
		}

		public Quadruple(float value)
		{
			const int bias = 127;
			const int expMask = 0x7F80_0000;
			const int fracMask = 0x007F_FFFF;
			const long signMask = 0x8000_0000;
			const int infinityMask = 0x7F80_0000;
			fixed (void* buf = _b)
			{
				long* p = (long*)buf;
				int s = BitConverter.SingleToInt32Bits(value);
				if (s != 0)
				{
					long exponent;
					if ((s & infinityMask) == infinityMask)
						exponent = 0x7FFF - Bias;
					else
						exponent = ((s & expMask) >> 23) - bias;
					long frac = s & fracMask;
					p[1] = ((s & signMask) << 32) | ((exponent + Bias) << 48) | (frac << 25);
				}
			}
		}
		public Quadruple(double value)
		{
			const int bias = 1023;
			const long expMask = 0x7FF0_0000_0000_0000;
			const long fracMask = 0x000F_FFFF_FFFF_FFFF;
			const ulong signMask = 0x8000_0000_0000_0000;
			const long infinityMask = 0x7FF0_0000_0000_0000;
			fixed (void* buf = _b)
			{
				long* p = (long*)buf;
				long s = BitConverter.DoubleToInt64Bits(value);
				if (s != 0)
				{
					long exponent;
					if ((s & infinityMask) == infinityMask)
						exponent = 0x7FFF - Bias;
					else
						exponent = ((s & expMask) >> 52) - bias;
					long frac = s & fracMask;
					p[0] = frac << 60;
					p[1] = (long)(((ulong)s & signMask) | (ulong)((exponent + Bias) << 48) | (ulong)(frac >> 4));
				}
			}
		}
		public Quadruple(int value)
		{
			int sign = value < 0 ? 1 : 0;
			if (sign == 1)
				value = -value;
			int hbit = BigIntHelpers.GetHighestBit((uint)value);
			int exp = hbit;
			UInt128 u = (uint)value;
			u <<= SignificandBits - hbit;
			u &= FractionMask;
			u |= (UInt128)((exp + Bias) | sign << 15) << 112;
			fixed (void* b = _b)
				Unsafe.CopyBlock(b, u._u, (uint)sizeof(Quadruple));
		}
		public Quadruple(uint value)
		{
			int hbit = BigIntHelpers.GetHighestBit(value);
			int exp = hbit;
			UInt128 u = value;
			u <<= SignificandBits - hbit;
			u &= FractionMask;
			u |= (UInt128)(exp + Bias) << 112;
			fixed (void* b = _b)
				Unsafe.CopyBlock(b, u._u, (uint)sizeof(Quadruple));
		}
		public Quadruple(long value)
		{
			int sign = value < 0 ? 1 : 0;
			if (sign == 1)
				value = -value;
			int hbit = BigIntHelpers.GetHighestBit((ulong)value);
			int exp = hbit;
			UInt128 u = (ulong)value;
			u <<= SignificandBits - hbit;
			u &= FractionMask;
			u |= (UInt128)((exp + Bias) | sign << 15) << 112;
			fixed (void* b = _b)
				Unsafe.CopyBlock(b, u._u, (uint)sizeof(Quadruple));
		}
		public Quadruple(ulong value)
		{
			int hbit = BigIntHelpers.GetHighestBit(value);
			int exp = hbit;
			UInt128 u = value;
			u <<= SignificandBits - hbit;
			u &= FractionMask;
			u |= (UInt128)(exp + Bias) << 112;
			fixed (void* b = _b)
				Unsafe.CopyBlock(b, u._u, (uint)sizeof(Quadruple));
		}
		public Quadruple(UInt128 value)
		{
			int hbit = UInt128.GetHighestBit(value);
			int exp = hbit;
			UInt128 u = value;
			u <<= SignificandBits - hbit;
			u &= FractionMask;
			u |= (UInt128)(exp + Bias) << 112;
			fixed (void* b = _b)
				Unsafe.CopyBlock(b, u._u, (uint)sizeof(Quadruple));
		}
		public Quadruple(UInt256 value)
		{
			int hbit = UInt256.GetHighestBit(value);
			int exp = hbit;
			UInt128 u = (UInt128)(value << (SignificandBits - hbit));
			u &= FractionMask;
			u |= (UInt128)(exp + Bias) << 112;
			fixed (void* b = _b)
				Unsafe.CopyBlock(b, u._u, (uint)sizeof(Quadruple));
		}

		public override bool Equals(object? obj) => obj is Quadruple quad && Equals(quad);
		public bool Equals(Quadruple other) => Equals(this, other);
		public override int GetHashCode() => GetHashCode(this);

		public override string ToString() => FormatQuadruple(this, null, NumberFormatInfo.InvariantInfo);
		public string ToString(string? format) => FormatQuadruple(this, format, NumberFormatInfo.InvariantInfo);
		public string ToString(IFormatProvider? formatProvider) => FormatQuadruple(this, null, NumberFormatInfo.GetInstance(formatProvider));
		public string ToString(string? format, IFormatProvider? formatProvider) => FormatQuadruple(this, format, NumberFormatInfo.GetInstance(formatProvider));

		internal string ToStringHex()
		{
			fixed (void* p = _b)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < 8; ++i)
					sb.AppendFormat("{0:X4} ", ((ushort*)p)[7 - i]);
				return sb.ToString().Trim();
			}
		}

		public int CompareTo(Quadruple value) => CompareTo(this, value);

		public int CompareTo(object? value)
		{
			if (value is null)
				return 1;
			if (value is Quadruple q)
				return CompareTo(this, q);
			throw new ArgumentException($"Object must be of type {nameof(Quadruple)}");
		}

		public static bool operator ==(Quadruple left, Quadruple right) => Equals(left, right);
		public static bool operator !=(Quadruple left, Quadruple right) => !Equals(left, right);
		public static bool operator >(Quadruple left, Quadruple right) => NeitherNaN(left, right) && CompareToInternal(left, right) > 0;
		public static bool operator <(Quadruple left, Quadruple right) => NeitherNaN(left, right) && CompareToInternal(left, right) < 0;
		public static bool operator >=(Quadruple left, Quadruple right) => NeitherNaN(left, right) && CompareToInternal(left, right) >= 0;
		public static bool operator <=(Quadruple left, Quadruple right) => NeitherNaN(left, right) && CompareToInternal(left, right) <= 0;

		public static Quadruple operator -(Quadruple quad) => IsNaN(quad) ? quad : new Quadruple(((ulong*)quad._b)[1] ^ 0x8000_0000_0000_0000, ((ulong*)quad._b)[0]);
		public static Quadruple operator +(Quadruple left, Quadruple right) => Add(left, right);
		public static Quadruple operator -(Quadruple left, Quadruple right) => Subtract(left, right);
		public static Quadruple operator *(Quadruple left, Quadruple right) => Multiply(left, right);
		public static Quadruple operator /(Quadruple left, Quadruple right) => Divide(left, right);

		public static implicit operator Quadruple(float value) => new Quadruple(value);
		public static implicit operator Quadruple(double value) => new Quadruple(value);
		public static implicit operator Quadruple(int value) => new Quadruple(value);
		public static implicit operator Quadruple(uint value) => new Quadruple(value);
		public static implicit operator Quadruple(long value) => new Quadruple(value);
		public static implicit operator Quadruple(ulong value) => new Quadruple(value);
		public static implicit operator Quadruple(UInt128 value) => new Quadruple(value);
		public static implicit operator Quadruple(UInt256 value) => new Quadruple(value);

		public static explicit operator float(Quadruple q) => ConvertToSingle(q);
		public static explicit operator double(Quadruple q) => ConvertToDouble(q);
		public static explicit operator int(Quadruple q) => ConvertToInt32(q);
		public static explicit operator uint(Quadruple q) => ConvertToUInt32(q);
		public static explicit operator long(Quadruple q) => ConvertToInt64(q);
		public static explicit operator ulong(Quadruple q) => ConvertToUInt64(q);
		public static explicit operator UInt128(Quadruple q) => ConvertToUInt128(q);
		public static explicit operator UInt256(Quadruple q) => ConvertToUInt256(q);
	}
}
