using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions;
public readonly struct UInt256
	: IBinaryInteger<UInt256>,
	  IMinMaxValue<UInt256>,
	  IUnsignedNumber<UInt256>

{
	internal const int Size = 32;

	private readonly UInt128 _lower;
	private readonly UInt128 _upper;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UInt256(ulong u3, ulong u2, ulong u1, ulong u0) : this(new UInt128(u3, u2), new UInt128(u1, u0)) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UInt256(UInt128 upper, UInt128 lower)
	{
		_lower = lower;
		_upper = upper;
	}

	/// <inheritdoc cref="IComparable.CompareTo(object)" />
	public int CompareTo(object? value)
	{
		if (value is UInt256 other)
		{
			return CompareTo(other);
		}
		else if (value is null)
		{
			return 1;
		}
		else
		{
			throw new ArgumentException($"Argument must be of type {nameof(UInt256)}");
		}
	}

	/// <inheritdoc cref="IComparable{T}.CompareTo(T)" />
	public int CompareTo(UInt256 value)
	{
		if (this < value)
		{
			return -1;
		}
		else if (this > value)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}

	/// <inheritdoc cref="ValueType.Equals(object?)"/>
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is UInt256 other && Equals(other);

	/// <inheritdoc cref="ValueType.GetHashCode()"/>
	public override int GetHashCode() => HashCode.Combine(_lower, _upper);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	public bool Equals(UInt256 other) => this == other;

	/// <inheritdoc cref="object.ToString()"/>
	public override string ToString() => $"{_upper:X32}{_lower:X32}";

	public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format) => throw new NotImplementedException();

	public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? provider) => throw new NotImplementedException();

	public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => throw new NotImplementedException();

	public static UInt256 Parse(string s)
	{
		ArgumentNullException.ThrowIfNull(s);
		throw new NotImplementedException();
	}

	public static UInt256 Parse(string s, NumberStyles style)
	{
		ArgumentNullException.ThrowIfNull(s);
		throw new NotImplementedException();
	}

	public static UInt256 Parse(string s, IFormatProvider? provier)
	{
		ArgumentNullException.ThrowIfNull(s);
		throw new NotImplementedException();
	}

	public static UInt256 Parse(string s, NumberStyles style, IFormatProvider? provider)
	{
		ArgumentNullException.ThrowIfNull(s);
		throw new NotImplementedException();
	}

	public static UInt256 Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
	{
		throw new NotImplementedException();
	}

	public static bool TryParse([NotNullWhen(true)] string? s, out UInt256 result)
	{
		if (s is not null)
		{
			throw new NotImplementedException();
		}
		else
		{
			result = default;
			return false;
		}
	}

	public static bool TryParse(ReadOnlySpan<char> s, out UInt256 result)
	{
		throw new NotImplementedException();
	}

	public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out UInt256 result)
	{
		if (s is not null)
		{
			throw new NotImplementedException();
		}
		else
		{
			result = default;
			return false;
		}
	}

	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out UInt256 result)
	{
		throw new NotImplementedException();
	}

	//
	// Explicit Conversions From UInt256
	//

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="byte" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="byte" />.</returns>
	public static explicit operator byte(UInt256 value) => (byte)value._lower;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="char" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="char" />.</returns>
	public static explicit operator char(UInt256 value) => (char)value._lower;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="short" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="short" />.</returns>
	public static explicit operator short(UInt256 value) => (short)value._lower;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="int" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="int" />.</returns>
	public static explicit operator int(UInt256 value) => (int)value._lower;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="long" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="long" />.</returns>
	public static explicit operator long(UInt256 value) => (long)value._lower;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="IntPtr" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="IntPtr" />.</returns>
	public static explicit operator nint(UInt256 value) => (nint)value._lower;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="Int128" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="Int128" />.</returns>
	public static explicit operator Int128(UInt256 value) => (Int128)value._lower;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="sbyte" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="sbyte" />.</returns>
	public static explicit operator sbyte(UInt256 value) => (sbyte)value._lower;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="ushort" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="ushort" />.</returns>
	public static explicit operator ushort(UInt256 value) => (ushort)value._lower;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="uint" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="uint" />.</returns>
	public static explicit operator uint(UInt256 value) => (uint)value._lower;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="ulong" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="ulong" />.</returns>
	public static explicit operator ulong(UInt256 value) => (ulong)value._lower;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="UIntPtr" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="UIntPtr" />.</returns>
	public static explicit operator nuint(UInt256 value) => (nuint)value._lower;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="UInt128" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="UInt128" />.</returns>
	public static explicit operator UInt128(UInt256 value) => value._lower;

	//
	// Explicit Conversions To UInt256
	//

	/// <summary>Explicitly converts a <see cref="short" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static explicit operator UInt256(short value)
	{
		long lower = value;
		ulong upper = (ulong)(lower >> 63);
		return new UInt256(upper, upper, upper, (ulong)lower);
	}

	/// <summary>Explicitly converts a <see cref="int" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static explicit operator UInt256(int value)
	{
		long lower = value;
		ulong upper = (ulong)(lower >> 63);
		return new UInt256(upper, upper, upper, (ulong)lower);
	}

	/// <summary>Explicitly converts a <see cref="long" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static explicit operator UInt256(long value)
	{
		long lower = value;
		ulong upper = (ulong)(lower >> 63);
		return new UInt256(upper, upper, upper, (ulong)lower);
	}

	/// <summary>Explicitly converts a <see cref="nint" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static explicit operator UInt256(nint value)
	{
		long lower = value;
		ulong upper = (ulong)(lower >> 63);
		return new UInt256(upper, upper, upper, (ulong)lower);
	}

	/// <summary>Explicitly converts a <see cref="Int128" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static explicit operator UInt256(Int128 value) => new UInt256((UInt128)(value >> 127), (UInt128)value);

	/// <summary>Explicitly converts a <see cref="sbyte" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static explicit operator UInt256(sbyte value)
	{
		long lower = value;
		ulong upper = (ulong)(lower >> 63);
		return new UInt256(upper, upper, upper, (ulong)lower);
	}

	//
	// Implicit Conversions To UInt256
	//

	/// <summary>Implicitly converts a <see cref="byte" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static implicit operator UInt256(byte value) => new UInt256(0, 0, 0, value);

	/// <summary>Implicitly converts a <see cref="char" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static implicit operator UInt256(char value) => new UInt256(0, value);

	/// <summary>Implicitly converts a <see cref="ushort" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static implicit operator UInt256(ushort value) => new UInt256(0, value);

	/// <summary>Implicitly converts a <see cref="uint" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static implicit operator UInt256(uint value) => new UInt256(0, value);

	/// <summary>Implicitly converts a <see cref="ulong" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static implicit operator UInt256(ulong value) => new UInt256(0, value);

	/// <summary>Implicitly converts a <see cref="UIntPtr" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static implicit operator UInt256(nuint value) => new UInt256(0, value);

	/// <summary>Implicitly converts a <see cref="UIntPtr" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static implicit operator UInt256(UInt128 value) => new UInt256(0, value);

	//
	// IAdditionOperators
	//

	/// <inheritdoc cref="IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
	public static UInt256 operator +(UInt256 left, UInt256 right)
	{
		UInt128 lower = left._lower + right._lower;
		UInt128 carry = (lower < left._lower) ? 1UL : 0UL;

		UInt128 upper = left._upper + right._upper + carry;
		return new UInt256(upper, lower);
	}

	/// <inheritdoc cref="IAdditionOperators{TSelf, TOther, TResult}.op_CheckedAddition(TSelf, TOther)" />
	public static UInt256 operator checked +(UInt256 left, UInt256 right)
	{
		UInt128 lower = left._lower + right._lower;
		UInt128 carry = (lower < left._lower) ? 1UL : 0UL;

		UInt128 upper = checked(left._upper + right._upper + carry);
		return new UInt256(upper, lower);
	}

	//
	// IAdditiveIdentity
	//

	/// <inheritdoc cref="IAdditiveIdentity{TSelf, TResult}.AdditiveIdentity" />
	static UInt256 IAdditiveIdentity<UInt256, UInt256>.AdditiveIdentity => default;

	//
	// IBinaryInteger
	//

	/// <inheritdoc cref="IBinaryInteger{TSelf}.DivRem(TSelf, TSelf)" />
	public static (UInt256 Quotient, UInt256 Remainder) DivRem(UInt256 left, UInt256 right)
	{
		UInt256 quotient = left / right;
		return (quotient, left - (quotient * right));
	}

	/// <inheritdoc cref="IBinaryInteger{TSelf}.LeadingZeroCount(TSelf)" />
	public static UInt256 LeadingZeroCount(UInt256 value)
	{
		if (value._upper == 0UL)
			return 128 + UInt128.LeadingZeroCount(value._lower);

		return UInt128.LeadingZeroCount(value._upper);
	}

	/// <inheritdoc cref="IBinaryInteger{TSelf}.PopCount(TSelf)" />
	public static UInt256 PopCount(UInt256 value) => UInt128.PopCount(value._lower) + UInt128.PopCount(value._upper);

	/// <inheritdoc cref="IBinaryInteger{TSelf}.RotateLeft(TSelf, int)" />
	public static UInt256 RotateLeft(UInt256 value, int rotateAmount) => (value << rotateAmount) | (value >>> (256 - rotateAmount));

	/// <inheritdoc cref="IBinaryInteger{TSelf}.RotateRight(TSelf, int)" />
	public static UInt256 RotateRight(UInt256 value, int rotateAmount) => (value >>> rotateAmount) | (value << (256 - rotateAmount));

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TrailingZeroCount(TSelf)" />
	public static UInt256 TrailingZeroCount(UInt256 value)
	{
		if (value._lower == Zero)
			return 128 + UInt128.TrailingZeroCount(value._upper);

		return UInt128.TrailingZeroCount(value._lower);
	}

	/// <inheritdoc cref="IBinaryInteger{TSelf}.GetShortestBitLength()" />
	int IBinaryInteger<UInt256>.GetShortestBitLength()
	{
		UInt256 value = this;
		return (Size * 8) - (int)LeadingZeroCount(value);
	}

	/// <inheritdoc cref="IBinaryInteger{TSelf}.GetByteCount()" />
	int IBinaryInteger<UInt256>.GetByteCount() => Size;

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteBigEndian(Span{byte}, out int)" />
	bool IBinaryInteger<UInt256>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten)
	{
		if (destination.Length >= Size)
		{
			IBinaryInteger<UInt128> lower, upper;
			if (BitConverter.IsLittleEndian)
			{
				lower = _lower;
				upper = _upper;
			}
			else
			{
				lower = _upper;
				upper = _lower;
			}

			if (!upper.TryWriteBigEndian(destination, out int written))
			{
				bytesWritten = written;
				return false;
			}

			bytesWritten = written;
			if (!lower.TryWriteBigEndian(destination[written..], out written))
			{
				bytesWritten += written;
				return false;
			}

			bytesWritten += written;
			return true;
		}
		else
		{
			bytesWritten = 0;
			return false;
		}
	}

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteLittleEndian(Span{byte}, out int)"/>
	bool IBinaryInteger<UInt256>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
	{
		if (destination.Length >= Size)
		{
			IBinaryInteger<UInt128> lower, upper;
			if (BitConverter.IsLittleEndian)
			{
				lower = _lower;
				upper = _upper;
			}
			else
			{
				lower = _upper;
				upper = _lower;
			}

			if (!lower.TryWriteLittleEndian(destination, out int written))
			{
				bytesWritten = written;
				return false;
			}

			bytesWritten = written;
			if (!upper.TryWriteLittleEndian(destination[written..], out written))
			{
				bytesWritten += written;
				return false;
			}

			bytesWritten += written;
			return true;
		}
		else
		{
			bytesWritten = 0;
			return false;
		}
	}

	//
	// IBinaryNumber
	//

	/// <inheritdoc cref="IBinaryNumber{TSelf}.IsPow2(TSelf)"/>
	public static bool IsPow2(UInt256 value) => PopCount(value) == One;

	/// <inheritdoc cref="IBinaryNumber{TSelf}.Log2(TSelf)"/>
	public static UInt256 Log2(UInt256 value)
	{
		if (value._upper == UInt128.Zero)
			return UInt128.Log2(value._lower);

		return 128 + UInt128.Log2(value._upper);
	}

	//
	// IBitwiseOperators
	//

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_BitwiseAnd(TSelf, TOther)" />
	public static UInt256 operator &(UInt256 left, UInt256 right) => new UInt256(left._upper & right._upper, left._lower & right._lower);

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_BitwiseOr(TSelf, TOther)" />
	public static UInt256 operator |(UInt256 left, UInt256 right) => new UInt256(left._upper | right._upper, left._lower | right._lower);

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_ExclusiveOr(TSelf, TOther)" />
	public static UInt256 operator ^(UInt256 left, UInt256 right) => new UInt256(left._upper ^ right._upper, left._lower ^ right._lower);

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_OnesComplement(TSelf)" />
	public static UInt256 operator ~(UInt256 value) => new UInt256(~value._upper, ~value._lower);

	//
	// IComparisonOperators
	//

	/// <inheritdoc cref="IComparisonOperators{TSelf, TOther}.op_LessThan(TSelf, TOther)" />
	public static bool operator <(UInt256 left, UInt256 right)
	{
		return (left._upper < right._upper)
			|| ((left._upper == right._upper) && (left._lower < right._lower));
	}

	/// <inheritdoc cref="IComparisonOperators{TSelf, TOther}.op_LessThanOrEqual(TSelf, TOther)" />
	public static bool operator <=(UInt256 left, UInt256 right)
	{
		return (left._upper < right._upper)
			|| ((left._upper == right._upper) && (left._lower <= right._lower));
	}

	/// <inheritdoc cref="IComparisonOperators{TSelf, TOther}.op_GreaterThan(TSelf, TOther)" />
	public static bool operator >(UInt256 left, UInt256 right)
	{
		return (left._upper > right._upper)
			|| ((left._upper == right._upper) && (left._lower > right._lower));
	}

	/// <inheritdoc cref="IComparisonOperators{TSelf, TOther}.op_GreaterThanOrEqual(TSelf, TOther)" />
	public static bool operator >=(UInt256 left, UInt256 right)
	{
		return (left._upper > right._upper)
			|| ((left._upper == right._upper) && (left._lower >= right._lower));
	}

	//
	// IDecrementOperators
	//

	/// <inheritdoc cref="IDecrementOperators{TSelf}.op_Decrement(TSelf)" />
	public static UInt256 operator --(UInt256 value) => value - One;

	/// <inheritdoc cref="IDecrementOperators{TSelf}.op_Decrement(TSelf)" />
	public static UInt256 operator checked --(UInt256 value) => checked(value - One);

	//
	// IDivisionOperators
	//

	/// <inheritdoc cref="IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
	public static UInt256 operator /(UInt256 left, UInt256 right)
	{
		if ((right._upper == UInt128.Zero) && (left._upper == UInt128.Zero))
		{
			// left and right are both uint64
			return left._lower / right._lower;
		}

		if (right >= left)
		{
			return (right == left) ? One : Zero;
		}

		return DivideSlow(left, right);

		static uint AddDivisor(Span<uint> left, ReadOnlySpan<uint> right)
		{
			Debug.Assert(left.Length >= right.Length);

			// Repairs the dividend, if the last subtract was too much

			ulong carry = 0UL;

			for (int i = 0; i < right.Length; i++)
			{
				ref uint leftElement = ref left[i];
				ulong digit = (leftElement + carry) + right[i];

				leftElement = unchecked((uint)digit);
				carry = digit >> 32;
			}

			return (uint)carry;
		}

		static bool DivideGuessTooBig(UInt128 q, UInt128 valHi, ulong valLo, ulong divHi, ulong divLo)
		{
			Debug.Assert(q <= 0xFFFFFFFF);

			// We multiply the two most significant limbs of the divisor
			// with the current guess for the quotient. If those are bigger
			// than the three most significant limbs of the current dividend
			// we return true, which means the current guess is still too big.

			UInt128 chkHi = divHi * q;
			UInt128 chkLo = divLo * q;

			chkHi += (chkLo >> 64);
			chkLo = (ulong)(chkLo);

			return (chkHi > valHi) || ((chkHi == valHi) && (chkLo > valLo));
		}

		unsafe static UInt256 DivideSlow(UInt256 quotient, UInt256 divisor)
		{
#pragma warning disable IDE0056
			// This is the same algorithm currently used by BigInteger so
			// we need to get a Span<uint> containing the value represented
			// in the least number of elements possible.

			// We need to ensure that we end up with 8x uints representing the bits from
			// least significant to most significant so the math will be correct on both
			// little and big endian systems. So we'll just allocate the relevant buffer
			// space and then write out the four parts using the native endianness of the
			// system.

			uint* pLeft = stackalloc uint[Size / sizeof(uint)];

			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 0), (uint)(quotient._lower >> 00));
			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 1), (uint)(quotient._lower >> 32));
			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 2), (uint)(quotient._lower >> 64));
			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 3), (uint)(quotient._lower >> 96));

			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 4), (uint)(quotient._upper >> 00));
			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 5), (uint)(quotient._upper >> 32));
			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 6), (uint)(quotient._upper >> 64));
			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 7), (uint)(quotient._upper >> 96));

			Span<uint> left = new Span<uint>(pLeft, (Size / sizeof(uint)) - ((int)LeadingZeroCount(quotient) / 32));

			// Repeat the same operation with the divisor

			uint* pRight = stackalloc uint[Size / sizeof(uint)];

			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 0), (uint)(divisor._lower >> 00));
			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 1), (uint)(divisor._lower >> 32));
			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 2), (uint)(divisor._lower >> 64));
			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 3), (uint)(divisor._lower >> 96));

			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 4), (uint)(divisor._upper >> 00));
			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 5), (uint)(divisor._upper >> 32));
			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 6), (uint)(divisor._upper >> 64));
			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 7), (uint)(divisor._upper >> 96));

			Span<uint> right = new Span<uint>(pLeft, (Size / sizeof(uint)) - ((int)LeadingZeroCount(divisor) / 32));

			Span<uint> rawBits = stackalloc uint[Size / sizeof(uint)];
			rawBits.Clear();
			Span<uint> bits = rawBits[..(left.Length - right.Length + 1)];

			Debug.Assert(left.Length >= 1);
			Debug.Assert(right.Length >= 1);
			Debug.Assert(left.Length >= right.Length);

			// Executes the "grammar-school" algorithm for computing q = a / b.
			// Before calculating q_i, we get more bits into the highest bit
			// block of the divisor. Thus, guessing digits of the quotient
			// will be more precise. Additionally we'll get r = a % b.

			uint divHi = right[right.Length - 1];
			uint divLo = right.Length > 1 ? right[right.Length - 2] : 0;

			// We measure the leading zeros of the divisor
			int shift = BitOperations.LeadingZeroCount(divHi);
			int backShift = 32 - shift;

			// And, we make sure the most significant bit is set
			if (shift > 0)
			{
				uint divNx = right.Length > 2 ? right[right.Length - 3] : 0;

				divHi = (divHi << shift) | (divLo >> backShift);
				divLo = (divLo << shift) | (divNx >> backShift);
			}

			// Then, we divide all of the bits as we would do it using
			// pen and paper: guessing the next digit, subtracting, ...
			for (int i = left.Length; i >= right.Length; i--)
			{
				int n = i - right.Length;
				uint t = ((uint)(i) < (uint)(left.Length)) ? left[i] : 0;

				ulong valHi = ((ulong)(t) << 32) | left[i - 1];
				uint valLo = (i > 1) ? left[i - 2] : 0;

				// We shifted the divisor, we shift the dividend too
				if (shift > 0)
				{
					uint valNx = i > 2 ? left[i - 3] : 0;

					valHi = (valHi << shift) | (valLo >> backShift);
					valLo = (valLo << shift) | (valNx >> backShift);
				}

				// First guess for the current digit of the quotient,
				// which naturally must have only 32 bits...
				ulong digit = valHi / divHi;

				if (digit > 0xFFFFFFFF)
				{
					digit = 0xFFFFFFFF;
				}

				// Our first guess may be a little bit to big
				while (DivideGuessTooBig(digit, valHi, valLo, divHi, divLo))
				{
					--digit;
				}

				if (digit > 0)
				{
					// Now it's time to subtract our current quotient
					uint carry = SubtractDivisor(left[n..], right, digit);

					if (carry != t)
					{
						Debug.Assert(carry == (t + 1));

						// Our guess was still exactly one too high
						carry = AddDivisor(left[n..], right);

						--digit;
						Debug.Assert(carry == 1);
					}
				}

				// We have the digit!
				if ((uint)(n) < (uint)(bits.Length))
				{
					bits[n] = (uint)(digit);
				}

				if ((uint)(i) < (uint)(left.Length))
				{
					left[i] = 0;
				}
			}

			ulong u0 = ((ulong)(rawBits[1]) << 32) | rawBits[0];
			ulong u1 = ((ulong)(rawBits[3]) << 32) | rawBits[2];
			ulong u2 = ((ulong)(rawBits[5]) << 32) | rawBits[4];
			ulong u3 = ((ulong)(rawBits[7]) << 32) | rawBits[6];

			return new UInt256(u3, u2, u1, u0);
		}

		static uint SubtractDivisor(Span<uint> left, ReadOnlySpan<uint> right, ulong q)
		{
			Debug.Assert(left.Length >= right.Length);
			Debug.Assert(q <= 0xFFFFFFFF);

			// Combines a subtract and a multiply operation, which is naturally
			// more efficient than multiplying and then subtracting...

			ulong carry = 0UL;

			for (int i = 0; i < right.Length; i++)
			{
				carry += right[i] * q;

				uint digit = (uint)(carry);
				carry >>= 32;

				ref uint leftElement = ref left[i];

				if (leftElement < digit)
					++carry;

				leftElement -= digit;
			}

			return (uint)(carry);
		}
#pragma warning restore IDE0056
	}

	/// <inheritdoc cref="IDivisionOperators{TSelf, TOther, TResult}.op_CheckedDivision(TSelf, TOther)" />
	public static UInt256 operator checked /(UInt256 left, UInt256 right) => left / right;

	//
	// IEqualityOperators
	//

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther}.op_Equality(TSelf, TOther)"/>
	public static bool operator ==(UInt256 left, UInt256 right) => left._upper == right._upper && left._lower == right._lower;

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther}.op_Inequality(TSelf, TOther)"/>
	public static bool operator !=(UInt256 left, UInt256 right) => left._upper != right._lower || left._lower != right._lower;

	//
	// IIncrementOperators
	//

	/// <inheritdoc cref="IIncrementOperators{TSelf}.op_Increment(TSelf)" />
	public static UInt256 operator ++(UInt256 value) => value + One;

	/// <inheritdoc cref="IIncrementOperators{TSelf}.op_CheckedIncrement(TSelf)" />
	public static UInt256 operator checked ++(UInt256 value) => checked(value + One);

	//
	// IMinMaxValue
	//

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue"/>
	public static UInt256 MinValue => new UInt256(UInt128.Zero, UInt128.Zero);

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue"/>
	public static UInt256 MaxValue => new UInt256(UInt128.MaxValue, UInt128.MaxValue);

	//
	// IModulusOperators
	//

	/// <inheritdoc cref="IModulusOperators{TSelf, TOther, TResult}.op_Modulus(TSelf, TOther)" />
	public static UInt256 operator %(UInt256 left, UInt256 right)
	{
		UInt256 quotient = left / right;
		return left - (quotient * right);
	}

	//
	// IMultiplicativeIdentity
	//

	/// <inheritdoc cref="IMultiplicativeIdentity{TSelf, TResult}.MultiplicativeIdentity" />
	static UInt256 IMultiplicativeIdentity<UInt256, UInt256>.MultiplicativeIdentity => One;

	//
	// IMultiplyOperators

	/// <inheritdoc cref="IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)" />
	public static UInt256 operator *(UInt256 left, UInt256 right)
	{
		UInt128 upper = BigMul(left._lower, right._lower, out UInt128 lower);
		upper += (left._upper * right._lower) + (left._lower * right._upper);
		return new UInt256(upper, lower);
	}

	/// <inheritdoc cref="IMultiplyOperators{TSelf, TOther, TResult}.op_CheckedMultiply(TSelf, TOther)"/>
	public static UInt256 operator checked *(UInt256 left, UInt256 right)
	{
		UInt256 upper = BigMul(left, right, out UInt256 lower);

		if (upper != 0U)
			throw new OverflowException();

		return lower;
	}

	internal unsafe static UInt128 BigMul(UInt128 left, UInt128 right, out UInt128 lower)
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ulong GetUpper(UInt128* value) => BitConverter.IsLittleEndian ? ((ulong*)value)[1] : ((ulong*)value)[0];
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ulong GetLower(UInt128* value) => BitConverter.IsLittleEndian ? ((ulong*)value)[0] : ((ulong*)value)[1];

		// Adaptation of algorithm for multiplication
		// of 32-bit unsigned integers described
		// in Hacker's Delight by Henry S. Warren, Jr. (ISBN 0-201-91465-4), Chapter 8
		// Basically, it's an optimized version of FOIL method applied to
		// low and high qwords of each operand

		UInt128 al = GetLower(&left);
		UInt128 ah = GetUpper(&left);

		UInt128 bl = GetLower(&right);
		UInt128 bh = GetUpper(&right);

		UInt128 mull = al * bl;
		UInt128 t = ah * bl + GetUpper(&mull);
		UInt128 tl = al * bh + GetLower(&t);

		lower = new UInt128(GetLower(&tl), GetLower(&mull));
		return ah * bh + GetUpper(&t) + GetUpper(&tl);
	}

	internal static UInt256 BigMul(UInt256 left, UInt256 right, out UInt256 lower)
	{
		// Adaptation of algorithm for multiplication
		// of 32-bit unsigned integers described
		// in Hacker's Delight by Henry S. Warren, Jr. (ISBN 0-201-91465-4), Chapter 8
		// Basically, it's an optimized version of FOIL method applied to
		// low and high qwords of each operand

		UInt256 al = left._lower;
		UInt256 ah = left._upper;

		UInt256 bl = right._lower;
		UInt256 bh = right._upper;

		UInt256 mull = al * bl;
		UInt256 t = ah * bl + mull._upper;
		UInt256 tl = al * bh + t._lower;

		lower = new UInt256(tl._lower, mull._lower);
		return ah * bh + t._upper + tl._upper;
	}

	//
	// INumber
	//

	/// <inheritdoc cref="INumber{TSelf}.Clamp(TSelf, TSelf, TSelf)" />
	public static UInt256 Clamp(UInt256 value, UInt256 min, UInt256 max)
	{
		if (min > max)
			throw new ArgumentException($"'{min}' cannot be greater than {max}");

		if (value < min)
			return min;
		else if (value > max)
			return max;

		return value;
	}

	/// <inheritdoc cref="INumber{TSelf}.CopySign(TSelf, TSelf)" />
	static UInt256 INumber<UInt256>.CopySign(UInt256 value, UInt256 sign) => value;

	/// <inheritdoc cref="INumber{TSelf}.Max(TSelf, TSelf)" />
	public static UInt256 Max(UInt256 x, UInt256 y) => (x >= y) ? x : y;

	/// <inheritdoc cref="INumber{TSelf}.MaxNumber(TSelf, TSelf)" />
	static UInt256 INumber<UInt256>.MaxNumber(UInt256 x, UInt256 y) => Max(x, y);

	/// <inheritdoc cref="INumber{TSelf}.Min(TSelf, TSelf)" />
	public static UInt256 Min(UInt256 x, UInt256 y) => (x <= y) ? x : y;

	/// <inheritdoc cref="INumber{TSelf}.MinNumber(TSelf, TSelf)" />
	static UInt256 INumber<UInt256>.MinNumber(UInt256 x, UInt256 y) => Min(x, y);

	/// <inheritdoc cref="INumber{TSelf}.Sign(TSelf)" />
	public static int Sign(UInt256 value) => (value == default) ? 0 : 1;

	//
	// INumberBase
	//

	/// <inheritdoc cref="INumberBase{TSelf}.One"/>
	public static UInt256 One => new UInt256(0, 0, 0, 1);

	/// <inheritdoc cref="INumberBase{TSelf}.Radix"/>
	static int INumberBase<UInt256>.Radix => 2;

	/// <inheritdoc cref="INumberBase{TSelf}.Zero"/>
	public static UInt256 Zero => default;

	/// <inheritdoc cref="INumberBase{TSelf}.Abs(TSelf)"/>
	static UInt256 INumberBase<UInt256>.Abs(UInt256 value) => value;
	/// <inheritdoc cref="INumberBase{TSelf}.IsCanonical(TSelf)" />
	static bool INumberBase<UInt256>.IsCanonical(UInt256 value) => true;

	/// <inheritdoc cref="INumberBase{TSelf}.IsComplexNumber(TSelf)" />
	static bool INumberBase<UInt256>.IsComplexNumber(UInt256 value) => false;

	/// <inheritdoc cref="INumberBase{TSelf}.IsEvenInteger(TSelf)" />
	public static bool IsEvenInteger(UInt256 value) => (value._lower & UInt128.One) == UInt128.Zero;

	/// <inheritdoc cref="INumberBase{TSelf}.IsFinite(TSelf)" />
	static bool INumberBase<UInt256>.IsFinite(UInt256 value) => true;

	/// <inheritdoc cref="INumberBase{TSelf}.IsImaginaryNumber(TSelf)" />
	static bool INumberBase<UInt256>.IsImaginaryNumber(UInt256 value) => false;

	/// <inheritdoc cref="INumberBase{TSelf}.IsInfinity(TSelf)" />
	static bool INumberBase<UInt256>.IsInfinity(UInt256 value) => false;

	/// <inheritdoc cref="INumberBase{TSelf}.IsInteger(TSelf)" />
	static bool INumberBase<UInt256>.IsInteger(UInt256 value) => true;

	/// <inheritdoc cref="INumberBase{TSelf}.IsNaN(TSelf)" />
	static bool INumberBase<UInt256>.IsNaN(UInt256 value) => false;

	/// <inheritdoc cref="INumberBase{TSelf}.IsNegative(TSelf)" />
	static bool INumberBase<UInt256>.IsNegative(UInt256 value) => false;

	/// <inheritdoc cref="INumberBase{TSelf}.IsNegativeInfinity(TSelf)" />
	static bool INumberBase<UInt256>.IsNegativeInfinity(UInt256 value) => false;

	/// <inheritdoc cref="INumberBase{TSelf}.IsNormal(TSelf)" />
	static bool INumberBase<UInt256>.IsNormal(UInt256 value) => value != default;

	/// <inheritdoc cref="INumberBase{TSelf}.IsOddInteger(TSelf)" />
	public static bool IsOddInteger(UInt256 value) => (value._lower & UInt128.One) != UInt128.Zero;

	/// <inheritdoc cref="INumberBase{TSelf}.IsPositive(TSelf)" />
	static bool INumberBase<UInt256>.IsPositive(UInt256 value) => true;

	/// <inheritdoc cref="INumberBase{TSelf}.IsPositiveInfinity(TSelf)" />
	static bool INumberBase<UInt256>.IsPositiveInfinity(UInt256 value) => false;

	/// <inheritdoc cref="INumberBase{TSelf}.IsRealNumber(TSelf)" />
	static bool INumberBase<UInt256>.IsRealNumber(UInt256 value) => true;

	/// <inheritdoc cref="INumberBase{TSelf}.IsSubnormal(TSelf)" />
	static bool INumberBase<UInt256>.IsSubnormal(UInt256 value) => false;

	/// <inheritdoc cref="INumberBase{TSelf}.IsZero(TSelf)" />
	static bool INumberBase<UInt256>.IsZero(UInt256 value) => value == Zero;

	/// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitude(TSelf, TSelf)" />
	static UInt256 INumberBase<UInt256>.MaxMagnitude(UInt256 x, UInt256 y) => Max(x, y);

	/// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitudeNumber(TSelf, TSelf)" />
	static UInt256 INumberBase<UInt256>.MaxMagnitudeNumber(UInt256 x, UInt256 y) => Max(x, y);

	/// <inheritdoc cref="INumberBase{TSelf}.MinMagnitude(TSelf, TSelf)" />
	static UInt256 INumberBase<UInt256>.MinMagnitude(UInt256 x, UInt256 y) => Min(x, y);

	/// <inheritdoc cref="INumberBase{TSelf}.MinMagnitudeNumber(TSelf, TSelf)" />
	static UInt256 INumberBase<UInt256>.MinMagnitudeNumber(UInt256 x, UInt256 y) => Min(x, y);

	/// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromChecked{TOther}(TOther, out TSelf)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool INumberBase<UInt256>.TryConvertFromChecked<TOther>(TOther value, out UInt256 result) => throw new NotImplementedException();

	/// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromSaturating{TOther}(TOther, out TSelf)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool INumberBase<UInt256>.TryConvertFromSaturating<TOther>(TOther value, out UInt256 result) => throw new NotImplementedException();

	/// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromTruncating{TOther}(TOther, out TSelf)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool INumberBase<UInt256>.TryConvertFromTruncating<TOther>(TOther value, out UInt256 result) => throw new NotImplementedException();

	/// <inheritdoc cref="INumberBase{TSelf}.TryConvertToChecked{TOther}(TSelf, out TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool INumberBase<UInt256>.TryConvertToChecked<TOther>(UInt256 value, [NotNullWhen(true)] out TOther result) => throw new NotImplementedException();

	/// <inheritdoc cref="INumberBase{TSelf}.TryConvertToSaturating{TOther}(TSelf, out TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool INumberBase<UInt256>.TryConvertToSaturating<TOther>(UInt256 value, [NotNullWhen(true)] out TOther result) => throw new NotImplementedException();

	/// <inheritdoc cref="INumberBase{TSelf}.TryConvertToTruncating{TOther}(TSelf, out TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool INumberBase<UInt256>.TryConvertToTruncating<TOther>(UInt256 value, [NotNullWhen(true)] out TOther result) => throw new NotImplementedException();

	//
	// IParsable
	//

	/// <inheritdoc cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)"/>
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out UInt256 result) => TryParse(s, NumberStyles.Integer, provider, out result);

	//
	// IShiftOperators
	//

	/// <inheritdoc cref="IShiftOperators{TSelf, TResult}.op_LeftShift(TSelf, int)"/>
	public static UInt256 operator <<(UInt256 value, int shiftAmount)
	{
		shiftAmount &= 0xFF;

		if ((shiftAmount & 0x80) != 0)
		{
			UInt128 upper = value._lower << shiftAmount;
			return new UInt256(upper, UInt128.Zero);
		}
		else if (shiftAmount != 0)
		{
			UInt128 lower = value._lower << shiftAmount;
			UInt128 upper = (value._upper << shiftAmount) | (value._lower >> (128 - shiftAmount));

			return new UInt256(upper, lower);
		}

		return value;
	}

	/// <inheritdoc cref="IShiftOperators{TSelf, TResult}.op_RightShift(TSelf, int)"/>
	public static UInt256 operator >>(UInt256 value, int shiftAmount) => value >>> shiftAmount;

	/// <inheritdoc cref="IShiftOperators{TSelf, TResult}.op_UnsignedRightShift(TSelf, int)"/>
	public static UInt256 operator >>>(UInt256 value, int shiftAmount)
	{
		shiftAmount &= 0xFF;

		if ((shiftAmount & 0x80) != 0)
		{
			UInt128 lower = value._upper >> shiftAmount;
			return new UInt256(UInt128.Zero, lower);
		}
		else if (shiftAmount != 0)
		{
			UInt128 lower = (value._lower >> shiftAmount) | (value._upper << (64 - shiftAmount));
			UInt128 upper = value._upper >> shiftAmount;

			return new UInt256(upper, lower);
		}

		return value;
	}

	//
	// ISpanParsable
	//

	/// <inheritdoc cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)" />
	public static UInt256 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, NumberStyles.Integer, provider);

	/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)" />
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out UInt256 result) => TryParse(s, NumberStyles.Integer, provider, out result);

	//
	// ISubtractionOperators
	//

	/// <inheritdoc cref="ISubtractionOperators{TSelf, TOther, TResult}.op_Subtraction(TSelf, TOther)"/>
	public static UInt256 operator -(UInt256 left, UInt256 right)
	{
		UInt128 lower = left._lower - right._lower;
		UInt128 borrow = (lower > left._lower) ? UInt128.One : UInt128.Zero;

		UInt128 upper = left._upper - right._upper - borrow;
		return new UInt256(upper, lower);
	}

	/// <inheritdoc cref="ISubtractionOperators{TSelf, TOther, TResult}.op_CheckedSubtraction(TSelf, TOther)"/>
	public static UInt256 operator checked -(UInt256 left, UInt256 right)
	{
		UInt128 lower = left._lower - right._lower;
		UInt128 borrow = (lower > left._lower) ? UInt128.One : UInt128.Zero;

		UInt128 upper = checked(left._upper - right._upper - borrow);
		return new UInt256(upper, lower);
	}

	//
	// IUnaryNegationOperators
	//

	/// <inheritdoc cref="IUnaryNegationOperators{TSelf, TResult}.op_UnaryNegation(TSelf)" />
	public static UInt256 operator -(UInt256 value) => Zero - value;

	/// <inheritdoc cref="IUnaryNegationOperators{TSelf, TResult}.op_CheckedUnaryNegation(TSelf)" />
	public static UInt256 operator checked -(UInt256 value) => checked(Zero - value);

	//
	// IUnaryPlusOperators
	//

	/// <inheritdoc cref="IUnaryPlusOperators{TSelf, TResult}.op_UnaryPlus(TSelf)" />
	public static UInt256 operator +(UInt256 value) => value;
}
