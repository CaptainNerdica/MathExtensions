using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions;

[StructLayout(LayoutKind.Sequential, Size = 16)]
[DebuggerDisplay("{ToString()}")]
public readonly unsafe struct Int128 : IEquatable<Int128>, IComparable<Int128>
{
	public readonly ulong _u0;
	public readonly ulong _u1;

	public static readonly Int128 Zero = new Int128(0x0000_0000_0000_0000, 0x0000_0000_0000_0000);
	public static readonly Int128 One = new Int128(0x0000_0000_0000_0000, 0x0000_0000_0000_0001);
	public static readonly Int128 MinValue = new Int128(0x8000_0000_0000_0000, 0x0000_0000_0000_0000);
	public static readonly Int128 MaxValue = new Int128(0x7FFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Int128(ulong hi, ulong lo)
	{
		_u0 = lo;
		_u1 = hi;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Int128(void* ptr) => this = *(Int128*)ptr;

	public Int128(ReadOnlySpan<byte> span)
	{
		if (span.Length < sizeof(Int128))
			throw new IndexOutOfRangeException();
		this = Unsafe.ReadUnaligned<Int128>(ref MemoryMarshal.GetReference(span));
	}

	public override readonly bool Equals(object? obj) => obj is Int128 o && Equals(o);
	public override readonly int GetHashCode() => HashCode.Combine(_u0, _u1);
	public readonly bool Equals(Int128 other) => this == other;

	public override readonly string ToString()
	{
		const int maxChars = 39 + 1;	// Maximum length is 39 characters + 1 character for sign.
		UInt128 value;					// Use an unsigned Int128 because it can store exactly the absolute value
		bool isNegative = IsNegative(this);
		if (isNegative)
			value = (UInt128)(-this);
		else
			value = (UInt128)this;
		Span<char> buffer = stackalloc char[maxChars];
		int index = maxChars;
		do
		{
			value = UInt128.DivRem(value, 10, out ulong r);
			buffer[--index] = (char)('0' + r);
		} while (!UInt128.IsZero(value));
		if (isNegative)
			buffer[--index] = '-';
		return buffer[index..].ToString();
	}

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

	public int CompareTo(Int128 other)
	{
		Int128 diff = this - other;
		if (diff == 0)
			return 0;
		if ((long)diff._u1 < 0)
			return -1;
		return 1;
	}

	internal static bool IsZero(Int128 value) => (value._u1 | value._u0) == 0;
	internal static bool IsNegative(Int128 value) => (long)value._u1 < 0;

	public static bool operator ==(Int128 left, Int128 right) => ((left._u0 - right._u0) | (left._u1 - right._u1)) == 0;
	public static bool operator !=(Int128 left, Int128 right) => ((left._u0 - right._u0) | (left._u1 - right._u1)) != 0;

	public static bool operator >(Int128 left, Int128 right)
	{
		Int128 diff = right - left;
		return (long)diff._u1 < 0;
	}
	public static bool operator <(Int128 left, Int128 right)
	{
		Int128 diff = left - right;
		return (long)diff._u1 < 0;
	}
	public static bool operator >=(Int128 left, Int128 right) => !(left < right);
	public static bool operator <=(Int128 left, Int128 right) => !(left > right);

	public static Int128 operator ~(Int128 value) => new Int128(~value._u1, ~value._u0);
	public static Int128 operator &(Int128 left, Int128 right) => new Int128(left._u1 & right._u1, left._u0 & right._u0);
	public static Int128 operator |(Int128 left, Int128 right) => new Int128(left._u1 | right._u1, left._u0 | right._u0);
	public static Int128 operator ^(Int128 left, Int128 right) => new Int128(left._u1 ^ right._u1, left._u0 ^ right._u0);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Int128 operator -(Int128 value)
	{
		ulong u0 = ~value._u0 + 1;
		ulong u1 = ~value._u1;
		if (u0 < 1)
			u1++;
		return new Int128(u1, u0);
	}

	public static Int128 operator +(Int128 left, Int128 right)
	{
		ulong u0 = left._u0 + right._u0;
		ulong u1 = left._u1 + right._u1;
		if (u0 < left._u0)
			u1++;
		return new Int128(u1, u0);
	}
	public static Int128 operator +(Int128 left, long right)
	{
		ulong u0 = left._u0 + (ulong)right;
		ulong u1 = left._u1;
		if (right < 0)
			u1 += ulong.MaxValue;
		if (u0 < left._u0)
			u1++;
		return new Int128(u1, u0);
	}
	public static Int128 operator +(long left, Int128 right)
	{
		ulong u0 = right._u0 + (ulong)left;
		ulong u1 = right._u1;
		if (left < 0)
			u1 += ulong.MaxValue;
		if (u0 < right._u0)
			u1++;
		return new Int128(u1, u0);
	}

	public static Int128 operator -(Int128 left, Int128 right)
	{
		ulong u0 = left._u0 - right._u0;
		ulong u1 = left._u1 - right._u1;
		if (u0 > left._u0)
			u1--;
		return new Int128(u1, u0);
	}
	public static Int128 operator -(Int128 left, long right)
	{
		ulong u0 = left._u0 - (ulong)right;
		ulong u1 = left._u1;
		if (right < 0)
			u1 -= ulong.MaxValue;
		if (u0 > left._u0)
			u1--;
		return new Int128(u1, u0);
	}
	public static Int128 operator -(long left, Int128 right)
	{
		ulong u0 = right._u0 - (ulong)left;
		ulong u1 = right._u1;
		if (left < 0)
			u1 -= ulong.MaxValue;
		if (u0 > right._u0)
			u1--;
		return new Int128(u1, u0);
	}

	public static Int128 operator ++(Int128 value)
	{
		ulong u0 = value._u0 + 1;
		ulong u1 = value._u1;
		if (u0 < 1)
			u1++;
		return new Int128(u1, u0);
	}
	public static Int128 operator --(Int128 value)
	{
		ulong u0 = value._u0 - 1;
		ulong u1 = value._u1;
		if (u0 > 1)
			u1--;
		return new Int128(u1, u0);
	}

	public static Int128 operator *(Int128 left, Int128 right)
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

		return new Int128(h + left._u0 * right._u1 + left._u1 * right._u0, l);
	}

	public static Int128 operator /(Int128 dividend, Int128 divisor) => DivRem(dividend, divisor, out _);
	public static Int128 operator %(Int128 value, Int128 divisor)
	{
		_ = DivRem(value, divisor, out Int128 remainder);
		return remainder;
	}

	public static Int128 DivRem(Int128 x, Int128 y, out Int128 remainder)
	{
		if (IsZero(y))
			throw new DivideByZeroException();
		int xs = (int)(x._u1 >> 63);
		int ys = (int)(y._u1 >> 63);
		int newSign = xs ^ ys;
		if (xs > 0)
			x = -x;
		if (ys > 0)
			y = -y;

		Int128 quotient = default;
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

		if (newSign > 0)
		{
			quotient = -quotient;
			remainder = -remainder;
		}
		return quotient;
	}

	public static Int128 operator <<(Int128 value, int bits)
	{
		const int wordBits = sizeof(ulong) * 8;
		if (bits % 128 == 0)
			return value;
		ulong u1 = (value._u1 << bits) | (value._u0 >> (wordBits - bits));
		ulong u0 = value._u0 << bits;
		return ((uint)bits / wordBits % 2) switch
		{
			0 => new Int128(u1, u0),
			_ => new Int128(u0, 0)
		};
	}

	// More complex algorithm to perform arithmetic shift and extend sign
	public static Int128 operator >>(Int128 value, int bits)
	{
		const int wordBits = sizeof(ulong) * 8;
		if (bits % 128 == 0)
			return value;
		ulong u0 = (value._u0 >> bits) | (value._u1 << (wordBits - bits));
		ulong u1 = (ulong)((long)value._u1 >> bits);
		return ((uint)bits / wordBits % 2) switch
		{
			0 => new Int128(u1, u0),
			_ => new Int128((ulong)((long)value._u1 >> 63), u1) // Do arithmetic shift to fill top word with 1s if negative, 0s if positive
		};
	}

	internal static Int128 ShiftLeftLogical(Int128 value) => new Int128((value._u1 << 1) | (value._u0 >> 63), value._u0 << 1);
	internal static Int128 ShiftRightLogical(Int128 value) => new Int128(value._u1 >> 1, (value._u1 << 63) | (value._u0 >> 1));
	internal static Int128 ShiftInLeft(Int128 value) => new Int128((value._u1 << 1) | (value._u0 >> 63), (value._u0 << 1) + 1);
	internal static Int128 ShiftInRight(Int128 value) => new Int128((1UL << 63) + (value._u1 >> 1), (value._u1 << 63) | (value._u0 >> 1));

	public static implicit operator Int128(int value) => new Int128((ulong)((long)value >> 63), (ulong)value);
	public static implicit operator Int128(uint value) => new Int128(0, value);
	public static implicit operator Int128(long value) => new Int128((ulong)(value >> 63), (ulong)value);
	public static implicit operator Int128(ulong value) => new Int128(0, value);

	public static explicit operator int(Int128 value) => (int)value._u0;
	public static explicit operator uint(Int128 value) => (uint)value._u0;
	public static explicit operator long(Int128 value) => (long)value._u0;
	public static explicit operator ulong(Int128 value) => value._u0;

	public static explicit operator Int128(UInt128 value) => Unsafe.As<UInt128, Int128>(ref value);
	public static explicit operator UInt128(Int128 value) => Unsafe.As<Int128, UInt128>(ref value);
}
