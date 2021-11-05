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
	/// An implementation of IEEE 754-2008 binary128 Quadruple precision floating point numbers.
	/// </summary>
	/// <remarks>
	/// Uses Subnormal Is Zero for all calculations.
	/// </remarks>
	[StructLayout(LayoutKind.Explicit)]
	public unsafe readonly partial struct Quadruple : IComparable, IComparable<Quadruple>, IEquatable<Quadruple>, IFormattable
	{
		[FieldOffset(0)]
		internal readonly uint _b0;
		[FieldOffset(4)]
		internal readonly uint _b1;
		[FieldOffset(8)]
		internal readonly uint _b2;
		[FieldOffset(12)]
		internal readonly uint _b3;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		internal readonly uint* _b
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				fixed (void* p = &this)
				{
					return (uint*)p;
				}
			}
		}

		internal readonly int Sign { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => (int)(_b3 >> 31); }
		internal readonly int Exp { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => (int)((_b3 & 0x7FFF_0000U) >> 16); }

		internal const int Bias = 16383;
		internal const int SignificandBits = 112;
		internal const int NormalMantissaBits = 113;
		internal static readonly UInt128 FractionMask = new UInt128(0xFFFF_FFFF, 0xFFFF_FFFF, 0xFFFF_FFFF, 0x0000_FFFF);

		public static readonly Quadruple Epsilon = new Quadruple(0x0000_0000, 0x0000_0000, 0x0000_0000, 0x0000_0001);

		public static readonly Quadruple MaxValue = new Quadruple(0x7FFE_FFFF, 0xFFFF_FFFF, 0xFFFF_FFFF, 0xFFFF_FFFF);
		public static readonly Quadruple MinValue = new Quadruple(0xFFFE_FFFF, 0xFFFF_FFFF, 0xFFFF_FFFF, 0xFFFF_FFFF);

		public static readonly Quadruple NaN = new Quadruple(0xFFFF_8000, 0x0000_0000, 0x0000_0000, 0x0000_0000);
		public static readonly Quadruple NegativeInfinity = new Quadruple(0xFFFF_0000, 0x0000_0000, 0x0000_0000, 0x0000_0000);
		public static readonly Quadruple PositiveInfinity = new Quadruple(0x7FFF_0000, 0x0000_0000, 0x0000_0000, 0x0000_0000);

		public static readonly Quadruple NegativeZero = new Quadruple(0x8000_0000, 0x0000_0000, 0x0000_0000, 0x0000_0000);
		public static readonly Quadruple Zero = new Quadruple(0x0000_0000, 0x0000_0000, 0x0000_0000, 0x0000_0000);

		public static readonly Quadruple One = new Quadruple(0x3FFF_0000, 0x0000_0000, 0x0000_0000, 0x0000_0000);
		internal static readonly Quadruple MinusOne = new Quadruple(0xBFFF_0000, 0x0000_0000, 0x0000_0000, 0x0000_0000);

		public static bool IsFinite(Quadruple q) => (q._b3 & 0x7FFF_0000) != 0x7FFF_0000;
		public static bool IsInfinity(Quadruple q) => (q._b3 & 0x7FFF_FFFF) == 0x7FFF_0000 && q._b2 == 0 && q._b1 == 0 && q._b0 == 0;
		public static bool IsNaN(Quadruple q) => (q._b3 & 0x7FFF_0000) == 0x7FFF_0000 && !((q._b3 & 0x0000_FFFF) == 0 && q._b2 == 0 && q._b1 == 0 && q._b0 == 0);
		public static bool IsNegativeInfinity(Quadruple q) => q._b3 == 0xFFFF_0000 && q._b2 == 0 && q._b1 == 0 && q._b0 == 0;
		public static bool IsNormal(Quadruple q) => 0x7FFF > q.Exp && q.Exp > 0;
		public static bool IsPositive(Quadruple q) => (q._b3 & 0x8000_0000) == 0;
		public static bool IsPositiveInfinity(Quadruple q) => q._b3 == 0x7FFF_0000 && q._b2 == 0 && q._b1 == 0 && q._b0 == 0;
		public static bool IsSubnormal(Quadruple q) => q.Exp == 0;
		internal static bool IsNegative(Quadruple q) => (q._b3 & 0x8000_0000) == 0x8000_0000;
		internal static bool IsZero(Quadruple q) => (q._b3 & 0x7FFF_FFFF) == 0 && q._b2 == 0 && q._b1 == 0 && q._b0 == 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Quadruple(uint u3, uint u2, uint u1, uint u0)
		{
			_b0 = u0;
			_b1 = u1;
			_b2 = u2;
			_b3 = u3;
		}

		public Quadruple(ReadOnlySpan<byte> span)
		{
			if (span.Length < sizeof(Quadruple))
				throw new ArgumentException("Span too small", nameof(span));
			this = MemoryMarshal.Read<Quadruple>(span);
		}

		public Quadruple(float value)
		{
			const int floatBias = 127;
			const uint expMask = 0x7F80_0000;
			const uint signMask = 0x8000_0000;
			const uint infMask = 0x7F80_0000;
			const uint fracMask = 0x007F_FFFF;
			int bits = BitConverter.SingleToInt32Bits(value);
			if (bits != 0)
			{
				uint exponent;
				if ((bits & infMask) == infMask)
					exponent = 0x7FFF;
				else
					exponent = (uint)((bits & expMask) >> 23) - floatBias + Bias;
				uint frac = (uint)bits & fracMask;

				_b3 = (uint)(bits & signMask) | (exponent << 16) | (frac >> 7);
				_b2 = frac << 25;
				_b1 = 0;
				_b0 = 0;
			}
			else
				this = default;
		}

		public Quadruple(double value)
		{
			const int doubleBias = 1023;
			const long expMask = 0x7FF0_0000_0000_0000;
			const long signMask = unchecked((long)0x8000_0000_0000_0000);
			const long infMask = 0x7FF0_0000_0000_0000;
			const long fracMask = 0x000F_FFFF_FFFF_FFFF;
			long bits = BitConverter.DoubleToInt64Bits(value);
			if (bits != 0)
			{
				uint exponent;
				if ((bits & infMask) == infMask)
					exponent = 0x7FFF;
				else
					exponent = (uint)((bits & expMask) >> 52) - doubleBias + Bias;
				long frac = bits & fracMask;
				uint hi = ((uint*)&frac)[1];
				uint lo = ((uint*)&frac)[0];

				_b3 = (uint)((bits & signMask) >> 32) | (exponent << 16) | (hi >> 4);
				_b2 = (hi << 28) | (lo >> 4);
				_b1 = lo << 28;
				_b0 = 0;
			}
			else
				this = default;
		}

		public Quadruple(int value)
		{
			Unsafe.SkipInit(out _b0);
			Unsafe.SkipInit(out _b1);
			Unsafe.SkipInit(out _b2);
			Unsafe.SkipInit(out _b3);
			if (value == 0)
				return;
			int sign = value < 0 ? 1 : 0;
			if (sign == 1)
				value = -value;

			int hbit = BigIntHelpers.GetHighestBit((uint)value);
			int exp = hbit;
			UInt128 u = (uint)value;
			u <<= SignificandBits - hbit;
			u &= FractionMask;
			u |= (UInt128)((exp + Bias) | sign << 15) << 112;
			fixed (void* b = &_b0)
				Unsafe.CopyBlock(b, &u, (uint)sizeof(Quadruple));
		}

		public Quadruple(uint value)
		{
			Unsafe.SkipInit(out _b0);
			Unsafe.SkipInit(out _b1);
			Unsafe.SkipInit(out _b2);
			Unsafe.SkipInit(out _b3);
			if (value == 0)
				return;
			int hbit = BigIntHelpers.GetHighestBit(value);
			int exp = hbit;
			UInt128 u = value;
			u <<= SignificandBits - hbit;
			u &= FractionMask;
			u |= (UInt128)(exp + Bias) << 112;
			fixed (void* b = &_b0)
				Unsafe.CopyBlock(b, &u, (uint)sizeof(Quadruple));
		}

		public Quadruple(long value)
		{
			Unsafe.SkipInit(out _b0);
			Unsafe.SkipInit(out _b1);
			Unsafe.SkipInit(out _b2);
			Unsafe.SkipInit(out _b3);
			if (value == 0)
				return;
			int sign = value < 0 ? 1 : 0;
			if (sign == 1)
				value = -value;
			int hbit = BigIntHelpers.GetHighestBit((ulong)value);
			int exp = hbit;
			UInt128 u = (ulong)value;
			u <<= SignificandBits - hbit;
			u &= FractionMask;
			u |= (UInt128)((exp + Bias) | sign << 15) << 112;
			fixed (void* b = &_b0)
				Unsafe.CopyBlock(b, &u, (uint)sizeof(Quadruple));
		}

		public Quadruple(ulong value)
		{
			Unsafe.SkipInit(out _b0);
			Unsafe.SkipInit(out _b1);
			Unsafe.SkipInit(out _b2);
			Unsafe.SkipInit(out _b3);
			if (value == 0)
				return;
			int hbit = BigIntHelpers.GetHighestBit(value);
			int exp = hbit;
			UInt128 u = value;
			u <<= SignificandBits - hbit;
			u &= FractionMask;
			u |= (UInt128)(exp + Bias) << 112;
			fixed (void* b = &_b0)
				Unsafe.CopyBlock(b, &u, (uint)sizeof(Quadruple));
		}

		public Quadruple(UInt128 value)
		{
			Unsafe.SkipInit(out _b0);
			Unsafe.SkipInit(out _b1);
			Unsafe.SkipInit(out _b2);
			Unsafe.SkipInit(out _b3);
			if (value == UInt128.Zero)
				return;
			int hbit = UInt128.HighestBit(value);
			int exp = hbit;
			UInt128 u = value;
			u <<= SignificandBits - hbit;
			u &= FractionMask;
			u |= (UInt128)(exp + Bias) << 112;
			fixed (void* b = &_b0)
				Unsafe.CopyBlock(b, &u, (uint)sizeof(Quadruple));
		}

		public Quadruple(UInt256 value)
		{
			Unsafe.SkipInit(out _b0);
			Unsafe.SkipInit(out _b1);
			Unsafe.SkipInit(out _b2);
			Unsafe.SkipInit(out _b3);
			if (value == UInt256.Zero)
				return;
			int hbit = UInt256.GetHighestBit(value);
			int exp = hbit;
			UInt128 u = (UInt128)(value << (SignificandBits - hbit));
			u &= FractionMask;
			u |= (UInt128)(exp + Bias) << 112;
			fixed (void* b = &_b0)
				Unsafe.CopyBlock(b, &u, (uint)sizeof(Quadruple));
		}

		public override bool Equals(object? obj) => obj is Quadruple quad && Equals(quad);
		public bool Equals(Quadruple other) => this == other;
		public override int GetHashCode() => IsNaN(this) ? int.MinValue : HashCode.Combine(_b0, _b1, _b2, _b3);

		public override string ToString() => FormatQuadruple(this, null, NumberFormatInfo.CurrentInfo);
		public string ToString(string? format) => FormatQuadruple(this, format, NumberFormatInfo.CurrentInfo);
		public string ToString(IFormatProvider? formatProvider) => FormatQuadruple(this, null, NumberFormatInfo.GetInstance(formatProvider));
		public string ToString(string? format, IFormatProvider? formatProvider) => FormatQuadruple(this, format, NumberFormatInfo.GetInstance(formatProvider));

		internal string ToStringHex()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < 4; ++i)
				sb.AppendFormat("{0:X8} ", _b[3 - i]);
			return sb.ToString().Trim();
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

		public static bool operator ==(Quadruple left, Quadruple right) => (IsZero(left) && IsZero(right)) || (NeitherNaN(left, right) &&
																											   left._b3 == right._b3 &&
																											   left._b2 == right._b2 &&
																											   left._b1 == right._b1 &&
																											   left._b0 == right._b0);
		public static bool operator !=(Quadruple left, Quadruple right) => !(left == right);
		public static bool operator >(Quadruple left, Quadruple right) => NeitherNaN(left, right) && CompareToInternal(left, right) > 0;
		public static bool operator <(Quadruple left, Quadruple right) => NeitherNaN(left, right) && CompareToInternal(left, right) < 0;
		public static bool operator >=(Quadruple left, Quadruple right) => NeitherNaN(left, right) && CompareToInternal(left, right) >= 0;
		public static bool operator <=(Quadruple left, Quadruple right) => NeitherNaN(left, right) && CompareToInternal(left, right) <= 0;

		public static Quadruple operator -(Quadruple quad) => IsNaN(quad) ? quad : new Quadruple(quad._b3 ^ 0x8000_0000, quad._b2, quad._b1, quad._b0);
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
