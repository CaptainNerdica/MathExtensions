using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace MathExtensions;

[StructLayout(LayoutKind.Sequential, Size = 16)]
[DebuggerDisplay("{ToString()}")]
public readonly unsafe partial struct UInt128 : IEquatable<UInt128>, IComparable<UInt128>, IComparable, IFormattable
{
	internal readonly ulong _u0;
	internal readonly ulong _u1;

	public static UInt128 Zero => default;
	public static UInt128 One => new UInt128(0x0000_0000_0000_0000, 0x0000_0000_0000_0001);
	public static UInt128 MinValue => default;
	public static UInt128 MaxValue => new UInt128(0xFFFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal UInt128(ulong hi, ulong lo)
	{
		_u0 = lo;
		_u1 = hi;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal UInt128(void* ptr) => this = *(UInt128*)ptr;

	public UInt128(ReadOnlySpan<byte> span)
	{
		if (span.Length < sizeof(UInt128))
			throw new IndexOutOfRangeException();
		this = Unsafe.ReadUnaligned<UInt128>(ref MemoryMarshal.GetReference(span));
	}

	public override readonly bool Equals(object? obj) => obj is UInt128 u && Equals(u);
	public override readonly int GetHashCode() => HashCode.Combine(_u0, _u1);
	public readonly bool Equals(UInt128 other) => this == other;

	[SkipLocalsInit]
	public override readonly string ToString()
	{
		const int maxChars = 39;    // Maximum length is 39 characters.
		UInt128 value = this;
		Span<char> buffer = stackalloc char[maxChars];
		int index = maxChars;
		do
		{
			value = DivRem(value, 10, out ulong r);
			buffer[--index] = (char)('0' + r);
		} while (!IsZero(value));
		return buffer[index..].ToString();
	}

	public readonly string ToString(string? format, IFormatProvider? provider) => ToString();

	internal readonly string ToStringHex()
	{
		const int groups = 16 / sizeof(ushort);
		const int groupSize = 32 / groups;
		const string format = "X4";
		const int length = 32 + groups - 1;
		char* buffer = stackalloc char[length];

		fixed (void* p = &this)
		{
			ushort* u = (ushort*)p;
			int charIdx = 0;
			for (int i = groups - 1; i >= 0; i--)
			{
				u[i].TryFormat(new Span<char>(buffer + charIdx, groupSize), out _, format);
				if (i > 0)
				{
					buffer[charIdx + groupSize] = ' ';
					charIdx += groupSize + 1;
				}
			}
		}

		return new string(buffer, 0, length);
	}

	public int CompareTo(UInt128 other)
	{
		if (this == other)
			return 0;
		if (this < other)
			return -1;
		return 1;
	}

	public int CompareTo(object? other) => other is UInt128 o ? CompareTo(o) : throw new ArgumentException("Instances are not the same type", nameof(other));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsZero(UInt128 value) => (value._u1 | value._u0) == 0;
	public static bool operator ==(UInt128 left, UInt128 right) => ((left._u0 - right._u0) | (left._u1 - right._u1)) == 0;
	public static bool operator !=(UInt128 left, UInt128 right) => ((left._u0 - right._u0) | (left._u1 - right._u1)) != 0;

	public static bool operator >(UInt128 left, UInt128 right) => left._u1 > right._u1 || (left._u1 == right._u1 && left._u0 > right._u0);
	public static bool operator <(UInt128 left, UInt128 right) => left._u1 < right._u1 || (left._u1 == right._u1 && left._u0 < right._u0);
	public static bool operator >=(UInt128 left, UInt128 right) => left._u1 >= right._u1 && (left._u1 != right._u1 || left._u0 >= right._u0);
	public static bool operator <=(UInt128 left, UInt128 right) => left._u1 <= right._u1 && (left._u1 != right._u1 || left._u0 <= right._u0);

	public static UInt128 operator ~(UInt128 value) => new UInt128(~value._u1, ~value._u0);
	public static UInt128 operator &(UInt128 left, UInt128 right) => new UInt128(left._u1 & right._u1, left._u0 & right._u0);
	public static UInt128 operator |(UInt128 left, UInt128 right) => new UInt128(left._u1 | right._u1, left._u0 | right._u0);
	public static UInt128 operator ^(UInt128 left, UInt128 right) => new UInt128(left._u1 ^ right._u1, left._u0 ^ right._u0);

	public static UInt128 operator +(UInt128 value) => value;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UInt128 operator -(UInt128 value)
	{
		ulong u0 = ~value._u0 + 1;
		ulong u1 = ~value._u1;
		if (u0 < 1)
			u1++;
		return new UInt128(u1, u0);
	}

	public static UInt128 operator +(UInt128 left, UInt128 right)
	{
		ulong u0 = left._u0 + right._u0;
		ulong u1 = left._u1 + right._u1;
		if (u0 < left._u0)
			u1++;
		return new UInt128(u1, u0);
	}
	public static UInt128 operator +(UInt128 left, ulong right)
	{
		ulong u0 = left._u0 + right;
		ulong u1 = left._u1;
		if (u0 < left._u0)
			u1++;
		return new UInt128(u1, u0);
	}
	public static UInt128 operator +(ulong left, UInt128 right)
	{
		ulong u0 = right._u0 + left;
		ulong u1 = right._u1;
		if (u0 < right._u0)
			u1++;
		return new UInt128(u1, u0);
	}

	public static UInt128 operator -(UInt128 left, UInt128 right)
	{
		ulong u0 = left._u0 - right._u0;
		ulong u1 = left._u1 - right._u1;
		if (u0 > left._u0)
			u1--;
		return new UInt128(u1, u0);
	}
	public static UInt128 operator -(UInt128 left, ulong right)
	{
		ulong u0 = left._u0 - right;
		ulong u1 = left._u1;
		if (u0 > left._u0)
			u1--;
		return new UInt128(u1, u0);
	}
	public static UInt128 operator -(ulong left, UInt128 right)
	{
		ulong u0 = right._u0 - left;
		ulong u1 = right._u1;
		if (u0 > right._u0)
			u1--;
		return new UInt128(u1, u0);
	}

	public static UInt128 operator ++(UInt128 value)
	{
		ulong u0 = value._u0 + 1;
		ulong u1 = value._u1;
		if (u0 < 1)
			u1++;
		return new UInt128(u1, u0);
	}
	public static UInt128 operator --(UInt128 value)
	{
		ulong u0 = value._u0 - 1;
		ulong u1 = value._u1;
		if (u0 > 1)
			u1--;
		return new UInt128(u1, u0);
	}

	public static UInt128 operator *(UInt128 left, UInt128 right)
	{
		// Use slightly more efficient method of getting high half of first ulong on little endian machines
		ulong xh = BitConverter.IsLittleEndian ? ((uint*)&left._u0)[1] : left._u0 >> 32;
		ulong xl = (uint)left._u0;
		ulong yh = BitConverter.IsLittleEndian ? ((uint*)&right._u0)[1] : right._u0 >> 32;
		ulong yl = (uint)right._u0;

		ulong ll = xl * yl;
		ulong lh = xl * yh;
		ulong hl = xh * yl;
		ulong hh = xh * yh;
		ulong m = (ll >> 32) + lh + (uint)hl;
		ulong l = (uint)ll | (m << 32);
		ulong h = (m >> 32) + (hl >> 32) + hh;

		return new UInt128(h + left._u0 * right._u1 + left._u1 * right._u0, l);
	}

	public static UInt128 operator /(UInt128 x, UInt128 y) => DivRem(x, y, out _);
	public static UInt128 operator /(UInt128 x, ulong y) => DivRem(x, y, out _);

	public static UInt128 operator %(UInt128 x, UInt128 y)
	{
		_ = DivRem(x, y, out UInt128 remainder);
		return remainder;
	}
	public static ulong operator %(UInt128 x, ulong y)
	{
		_ = DivRem(x, y, out ulong remainder);
		return remainder;
	}

	public static UInt128 DivRem(UInt128 x, UInt128 y, out UInt128 remainder)
	{
		if (IsZero(y))
			throw new DivideByZeroException();

		UInt128 quotient = default;
		remainder = default;

		for (int i = 127; i >= 0; --i)
		{
			remainder = ShiftLeftLogical(remainder);
			remainder |= (x._u1 >> 63) & 1;
			x = ShiftLeftLogical(x);
			if (remainder >= y)
			{
				remainder -= y;
				quotient = ShiftInLeft(quotient);
			}
			else
				quotient = ShiftLeftLogical(quotient);
		}
		return quotient;
	}

	public static UInt128 DivRem(UInt128 x, ulong y, out ulong remainder)
	{
		if (y == 0)
			throw new DivideByZeroException();
		UInt128 quotient = default;
		remainder = 0;

		for (int i = 127; i >= 0; --i)
		{
			remainder <<= 1;
			remainder |= (x._u1 >> 63) & 1;
			x = ShiftLeftLogical(x);
			if (remainder >= y)
			{
				remainder -= y;
				quotient = ShiftInLeft(quotient);
			}
			else
				quotient = ShiftLeftLogical(quotient);
		}
		return quotient;
	}

	public static UInt128 operator <<(UInt128 value, int bits)
	{
		const int wordBits = sizeof(ulong) * 8;
		if (bits % 128 == 0)
			return value;
		ulong u1 = (value._u1 << bits) | (value._u0 >> (wordBits - bits));
		ulong u0 = value._u0 << bits;
		return ((uint)bits / wordBits % 2) switch
		{
			0 => new UInt128(u1, u0),
			_ => new UInt128(u0, 0)
		};
	}

	public static UInt128 operator >>(UInt128 value, int bits)
	{
		const int wordBits = sizeof(ulong) * 8;
		if (bits % 128 == 0)
			return value;
		ulong u0 = (value._u0 >> bits) | (value._u1 << (wordBits - bits));
		ulong u1 = value._u1 >> bits;
		return ((uint)bits / wordBits) switch
		{
			0 => new UInt128(u1, u0),
			_ => new UInt128(0, u1)
		};
	}

	internal static UInt128 ShiftLeftLogical(UInt128 value) => new UInt128((value._u1 << 1) | (value._u0 >> 63), value._u0 << 1);
	internal static UInt128 ShiftRightLogical(UInt128 value) => new UInt128(value._u1 >> 1, (value._u1 << 63) | (value._u0 >> 1));
	internal static UInt128 ShiftInLeft(UInt128 value) => new UInt128((value._u1 << 1) | (value._u0 >> 63), (value._u0 << 1) + 1);
	internal static UInt128 ShiftInRight(UInt128 value) => new UInt128((1UL << 63) + (value._u1 >> 1), (value._u1 << 63) | (value._u0 >> 1));

	public static int Log2(UInt128 value)
	{
		if (IsZero(value))
			return 0;
		if (value._u1 != 0)
			return BitOperations.Log2(value._u1) + 64;
		return BitOperations.Log2(value._u0);
	}

	public static explicit operator UInt128(int value) => new UInt128((ulong)((long)value >> 63), (ulong)value);
	public static implicit operator UInt128(uint value) => new UInt128(0, value);
	public static explicit operator UInt128(long value) => new UInt128((ulong)(value >> 63), (ulong)value);
	public static implicit operator UInt128(ulong value) => new UInt128(0, value);

	public static explicit operator UInt128(float value)
	{
		if (value < 0)
			return default;
		const int MANTISSA_BITS = 23;
		const int BIAS = 127;
		const int MANTISSA_MASK = (1 << MANTISSA_BITS) - 1;

		int bits = BitConverter.SingleToInt32Bits(value);
		UInt128 mantissa = (UInt128)(uint)(bits & MANTISSA_MASK);
		int exp = (bits >> MANTISSA_BITS) - BIAS;
		if (exp < 0 || exp > BIAS)
			return default;
		UInt128 output = One << exp;
		if (exp < MANTISSA_BITS)
			mantissa >>= MANTISSA_BITS - exp;
		else
			mantissa <<= exp - MANTISSA_BITS;
		return output | mantissa;
	}

	public static explicit operator UInt128(double value)
	{
		if (value < 0)
			return default;
		const int MANTISSA_BITS = 52;
		const int BIAS = 1023;
		const long MANTISSA_MASK = (1L << MANTISSA_BITS) - 1;

		long bits = BitConverter.DoubleToInt64Bits(value);
		UInt128 mantissa = (ulong)(bits & MANTISSA_MASK);
		int exp = (int)(bits >> MANTISSA_BITS) - BIAS;
		if (exp < 0 || exp > 127)
			return default;
		UInt128 output = One << exp;
		if (exp < MANTISSA_BITS)
			mantissa >>= MANTISSA_BITS - exp;
		else
			mantissa <<= exp - MANTISSA_BITS;
		return output | mantissa;
	}

	public static explicit operator int(UInt128 value) => (int)value._u0;
	public static explicit operator uint(UInt128 value) => (uint)value._u0;
	public static explicit operator long(UInt128 value) => (long)value._u0;
	public static explicit operator ulong(UInt128 value) => value._u0;

	public static explicit operator float(UInt128 value) => (float)(value._u0 + 18446744073709551616d * value._u1);
	public static explicit operator double(UInt128 value) => value._u0 + 18446744073709551616d * value._u1;
}
