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
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions;
[StructLayout(LayoutKind.Explicit, Size = Size)]
public readonly struct UInt256
	: IBinaryInteger<UInt256>,
	  IMinMaxValue<UInt256>,
	  IUnsignedNumber<UInt256>

{
	internal const int Size = 4 * sizeof(ulong);

	[FieldOffset(0)]
	private readonly UInt128 _lower;
	[FieldOffset(16)]
	private readonly UInt128 _upper;

	[FieldOffset(0)]
	private readonly ulong _u0;
	[FieldOffset(8)]
	private readonly ulong _u1;
	[FieldOffset(16)]
	private readonly ulong _u2;
	[FieldOffset(24)]
	private readonly ulong _u3;

	internal ulong U0 { get => _u0; private init => _u0 = value; }
	internal ulong U1 { get => _u1; private init => _u1 = value; }
	internal ulong U2 { get => _u2; private init => _u2 = value; }
	internal ulong U3 { get => _u3; private init => _u3 = value; }

	[StructLayout(LayoutKind.Explicit)]
	internal struct MutableUInt256
	{
		[FieldOffset(0 * sizeof(ulong))]
		internal ulong _u0;
		[FieldOffset(1 * sizeof(ulong))]
		internal ulong _u1;
		[FieldOffset(2 * sizeof(ulong))]
		internal ulong _u2;
		[FieldOffset(3 * sizeof(ulong))]
		internal ulong _u3;

		[FieldOffset(0)]
		internal UInt256 _value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UInt256(ulong u3, ulong u2, ulong u1, ulong u0) => (_u3, _u2, _u1, _u0) = (u3, u2, u1, u0);

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
			return -1;
		else if (this > value)
			return 1;
		else
			return 0;
	}

	/// <inheritdoc cref="ValueType.Equals(object?)"/>
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is UInt256 other && Equals(other);

	/// <inheritdoc cref="ValueType.GetHashCode()"/>
	public override int GetHashCode() => HashCode.Combine(_lower, _upper);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	public bool Equals(UInt256 other) => this == other;

	/// <inheritdoc cref="object.ToString()"/>
	public override string ToString() => ToString(null, null);
	
	/// <summary></summary>
	/// <param name="format"></param>
	/// <returns></returns>
	public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format) => ToString(format, null);

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)" />
	public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? provider) => 
		Number.FormatUInt256(this, format, provider);

	/// <inheritdoc cref="ISpanFormattable.TryFormat(Span{char}, out int, ReadOnlySpan{char}, IFormatProvider?)"/>
	public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => 
		Number.TryFormatUInt256(this, format, provider, destination, out charsWritten);

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
	public static explicit operator byte(UInt256 value) => (byte)value._u0;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="char" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="char" />.</returns>
	public static explicit operator char(UInt256 value) => (char)value._u0;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="short" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="short" />.</returns>
	public static explicit operator short(UInt256 value) => (short)value._u0;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="int" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="int" />.</returns>
	public static explicit operator int(UInt256 value) => (int)value._u0;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="long" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="long" />.</returns>
	public static explicit operator long(UInt256 value) => (long)value._u0;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="IntPtr" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="IntPtr" />.</returns>
	public static explicit operator nint(UInt256 value) => (nint)value._u0;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="Int128" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="Int128" />.</returns>
	public static explicit operator Int128(UInt256 value) => (Int128)value._lower;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="sbyte" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="sbyte" />.</returns>
	public static explicit operator sbyte(UInt256 value) => (sbyte)value._u0;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="ushort" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="ushort" />.</returns>
	public static explicit operator ushort(UInt256 value) => (ushort)value._u0;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="uint" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="uint" />.</returns>
	public static explicit operator uint(UInt256 value) => (uint)value._u0;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="ulong" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="ulong" />.</returns>
	public static explicit operator ulong(UInt256 value) => value._u0;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="UIntPtr" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="UIntPtr" />.</returns>
	public static explicit operator nuint(UInt256 value) => (nuint)value._u0;

	/// <summary>Explicitly converts a 256-bit unsigned integer to a <see cref="UInt128" /> value.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a <see cref="UInt128" />.</returns>
	public static explicit operator UInt128(UInt256 value) => new UInt128(value._u1, value._u0);

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
	public static unsafe implicit operator UInt256(byte value) => new UInt256(0, 0, 0, value);

	/// <summary>Implicitly converts a <see cref="char" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static implicit operator UInt256(char value) => new UInt256(0, 0, 0, value);

	/// <summary>Implicitly converts a <see cref="ushort" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static implicit operator UInt256(ushort value) => new UInt256(0, 0, 0, value);

	/// <summary>Implicitly converts a <see cref="uint" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static implicit operator UInt256(uint value) => new UInt256(0, 0, 0, value);

	/// <summary>Implicitly converts a <see cref="ulong" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static implicit operator UInt256(ulong value) => new UInt256(0, 0, 0, value);

	/// <summary>Implicitly converts a <see cref="UIntPtr" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static implicit operator UInt256(nuint value) => new UInt256(0, 0, 0, value);

	/// <summary>Implicitly converts a <see cref="UInt128" /> value to a 256-bit unsigned integer.</summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><paramref name="value" /> converted to a 256-bit unsigned integer.</returns>
	public static unsafe implicit operator UInt256(UInt128 value)
	{
		UInt256 o = default;
		if (BitConverter.IsLittleEndian)
			((UInt128*)&o)[0] = value;
		else
			((UInt128*)&o)[1] = value;
		return o;
	}

	//
	// IAdditionOperators
	//

	/// <inheritdoc cref="IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SkipLocalsInit]
	public static unsafe UInt256 operator +(UInt256 left, UInt256 right)
	{
		Unsafe.SkipInit(out MutableUInt256 mut);

		if (Vector256.IsHardwareAccelerated)
		{
			Vector256<ulong> l = Vector256.LoadUnsafe(ref Unsafe.AsRef(in left._u0));
			Vector256<ulong> o = l + Vector256.LoadUnsafe(ref Unsafe.AsRef(in right._u0));

			o.StoreUnsafe(ref mut._u0);
		}
		else if (Vector128.IsHardwareAccelerated)
		{
			Vector128<ulong> ll = Vector128.LoadUnsafe(ref Unsafe.AsRef(in left._u0));
			Vector128<ulong> lu = Vector128.LoadUnsafe(ref Unsafe.AsRef(in left._u2));

			Vector128<ulong> ol = ll + Vector128.LoadUnsafe(ref Unsafe.AsRef(in right._u0));
			Vector128<ulong> ou = lu + Vector128.LoadUnsafe(ref Unsafe.AsRef(in right._u2));

			ol.StoreUnsafe(ref mut._u0);
			ou.StoreUnsafe(ref mut._u2);
		}
		else
		{
			mut._u0 = left._u0 + right._u0;
			mut._u1 = left._u1 + right._u1;
			mut._u2 = left._u2 + right._u2;
			mut._u3 = left._u3 + right._u3;
		}

		if (mut._u0 < right._u0)
			mut._u1++;
		if (mut._u1 < right._u1)
			mut._u2++;
		if (mut._u2 < right._u2)
			mut._u3++;

		return mut._value;
	}

	/// <inheritdoc cref="IAdditionOperators{TSelf, TOther, TResult}.op_CheckedAddition(TSelf, TOther)" />
	public static unsafe UInt256 operator checked +(UInt256 left, UInt256 right)
	{
		Unsafe.SkipInit(out MutableUInt256 mut);

		if (Vector256.IsHardwareAccelerated)
		{
			Vector256<ulong> l = Vector256.LoadUnsafe(ref Unsafe.AsRef(in left._u0));
			Vector256<ulong> o = l + Vector256.LoadUnsafe(ref Unsafe.AsRef(in right._u0));

			o.StoreUnsafe(ref mut._u0);
		}
		else if (Vector128.IsHardwareAccelerated)
		{
			Vector128<ulong> ll = Vector128.LoadUnsafe(ref Unsafe.AsRef(in left._u0));
			Vector128<ulong> lu = Vector128.LoadUnsafe(ref Unsafe.AsRef(in left._u2));

			Vector128<ulong> ol = ll + Vector128.LoadUnsafe(ref Unsafe.AsRef(in right._u0));
			Vector128<ulong> ou = lu + Vector128.LoadUnsafe(ref Unsafe.AsRef(in right._u2));

			ol.StoreUnsafe(ref mut._u0);
			ou.StoreUnsafe(ref mut._u2);
		}
		else
		{
			mut._u0 = left._u0 + right._u0;
			mut._u1 = left._u1 + right._u1;
			mut._u2 = left._u2 + right._u2;
			mut._u3 = left._u3 + right._u3;
		}

		if (mut._u0 < right._u0)
			mut._u1++;
		if (mut._u1 < right._u1)
			mut._u2++;
		if (mut._u2 < right._u2)
			checked { mut._u3++; }

		return mut._value;
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
	[MethodImpl(MethodImplOptions.NoInlining)]
	[SkipLocalsInit]
	public static (UInt256 Quotient, UInt256 Remainder) DivRem(UInt256 left, UInt256 right)
	{
		UInt256 quotient = left / right;
		return (quotient, left - (quotient * right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Lzcnt(UInt256 value)
	{
		return value switch
		{
			{ U3: not 0 } => 0 * 8 * sizeof(ulong) + BitOperations.LeadingZeroCount(value._u3),
			{ U2: not 0 } => 1 * 8 * sizeof(ulong) + BitOperations.LeadingZeroCount(value._u2),
			{ U1: not 0 } => 2 * 8 * sizeof(ulong) + BitOperations.LeadingZeroCount(value._u1),
			_ => 3 * 8 * sizeof(ulong) + BitOperations.LeadingZeroCount(value._u0),
		};
	}

	/// <inheritdoc cref="IBinaryInteger{TSelf}.LeadingZeroCount(TSelf)" />
	public static UInt256 LeadingZeroCount(UInt256 value)
	{
		MutableUInt256 mut = default;

		mut._u0 = (uint)(value switch       // Manual inlining of Lzcnt to avoid additional copies
		{
			{ U3: not 0 } => 0 * 8 * sizeof(ulong) + BitOperations.LeadingZeroCount(value._u3),
			{ U2: not 0 } => 1 * 8 * sizeof(ulong) + BitOperations.LeadingZeroCount(value._u2),
			{ U1: not 0 } => 2 * 8 * sizeof(ulong) + BitOperations.LeadingZeroCount(value._u1),
			_ => 3 * 8 * sizeof(ulong) + BitOperations.LeadingZeroCount(value._u0),
		});

		return mut._value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Popcnt(UInt256 value) => BitOperations.PopCount(value._u3) + BitOperations.PopCount(value._u2) + BitOperations.PopCount(value._u1) + BitOperations.PopCount(value._u0);

	/// <inheritdoc cref="IBinaryInteger{TSelf}.PopCount(TSelf)" />
	public static UInt256 PopCount(UInt256 value)
	{
		MutableUInt256 mut = default;

		int v = BitOperations.PopCount(value._u3) + BitOperations.PopCount(value._u2) + BitOperations.PopCount(value._u1) + BitOperations.PopCount(value._u0);
		mut._u0 = (uint)v;

		return mut._value;
	}

	/// <inheritdoc cref="IBinaryInteger{TSelf}.RotateLeft(TSelf, int)" />
	public static UInt256 RotateLeft(UInt256 value, int rotateAmount) => (value << rotateAmount) | (value >>> (256 - rotateAmount));

	/// <inheritdoc cref="IBinaryInteger{TSelf}.RotateRight(TSelf, int)" />
	public static UInt256 RotateRight(UInt256 value, int rotateAmount) => (value >>> rotateAmount) | (value << (256 - rotateAmount));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Tzcnt(UInt256 value)
	{
		return value switch
		{
			{ U0: not 0 } => 0 * 8 * sizeof(ulong) + BitOperations.TrailingZeroCount(value._u0),
			{ U1: not 0 } => 1 * 8 * sizeof(ulong) + BitOperations.TrailingZeroCount(value._u1),
			{ U2: not 0 } => 2 * 8 * sizeof(ulong) + BitOperations.TrailingZeroCount(value._u2),
			_ => 3 * 8 * sizeof(ulong) + BitOperations.TrailingZeroCount(value._u3),
		};
	}

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TrailingZeroCount(TSelf)" />
	public static UInt256 TrailingZeroCount(UInt256 value)
	{
		MutableUInt256 mut = default;

		mut._u0 = (uint)(value switch    // Manual inlining of Tzcnt to avoid additional copies
		{
			{ U0: not 0 } => 0 * 8 * sizeof(ulong) + BitOperations.TrailingZeroCount(value._u0),
			{ U1: not 0 } => 1 * 8 * sizeof(ulong) + BitOperations.TrailingZeroCount(value._u1),
			{ U2: not 0 } => 2 * 8 * sizeof(ulong) + BitOperations.TrailingZeroCount(value._u2),
			_ => 3 * 8 * sizeof(ulong) + BitOperations.TrailingZeroCount(value._u3),
		});

		return mut._value;
	}

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryReadBigEndian(ReadOnlySpan{byte}, bool, out TSelf)"/>
	static bool IBinaryInteger<UInt256>.TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out UInt256 value)
	{
		if (source.Length < Size)
		{
			value = default;
			return false;
		}

		Unsafe.SkipInit(out value);

		ref MutableUInt256 mut = ref Unsafe.As<UInt256, MutableUInt256>(ref value);
		ref byte b = ref Unsafe.AsRef(in source[0]);

		mut._u0 = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 3 * sizeof(ulong)))) : Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 3 * sizeof(ulong)));
		mut._u1 = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 2 * sizeof(ulong)))) : Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 2 * sizeof(ulong)));
		mut._u2 = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 1 * sizeof(ulong)))) : Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 1 * sizeof(ulong)));
		mut._u3 = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 0 * sizeof(ulong)))) : Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 0 * sizeof(ulong)));

		return true;
	}

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryReadLittleEndian(ReadOnlySpan{byte}, bool, out TSelf)"/>
	static bool IBinaryInteger<UInt256>.TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out UInt256 value)
	{
		if (source.Length < Size)
		{
			value = default;
			return false;
		}

		Unsafe.SkipInit(out value);

		ref MutableUInt256 mut = ref Unsafe.As<UInt256, MutableUInt256>(ref value);
		ref byte b = ref Unsafe.AsRef(in source[0]);

		mut._u0 = BitConverter.IsLittleEndian ? Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 0 * sizeof(ulong))) : BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 0 * sizeof(ulong))));
		mut._u1 = BitConverter.IsLittleEndian ? Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 1 * sizeof(ulong))) : BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 1 * sizeof(ulong))));
		mut._u2 = BitConverter.IsLittleEndian ? Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 2 * sizeof(ulong))) : BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 2 * sizeof(ulong))));
		mut._u3 = BitConverter.IsLittleEndian ? Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 3 * sizeof(ulong))) : BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref b, 3 * sizeof(ulong))));

		return true;
	}

	/// <inheritdoc cref="IBinaryInteger{TSelf}.GetShortestBitLength()" />
	int IBinaryInteger<UInt256>.GetShortestBitLength()
	{
		return (Size * 8) - Lzcnt(this);
	}

	/// <inheritdoc cref="IBinaryInteger{TSelf}.GetByteCount()" />
	int IBinaryInteger<UInt256>.GetByteCount() => Size;

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteBigEndian(Span{byte}, out int)" />
	unsafe bool IBinaryInteger<UInt256>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten)
	{
		if (destination.Length < Size)
		{
			bytesWritten = 0;
			return false;
		}
		else
		{
			ulong u0 = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(_u0) : _u0;
			ulong u1 = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(_u1) : _u1;
			ulong u2 = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(_u2) : _u2;
			ulong u3 = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(_u3) : _u3;

			Unsafe.WriteUnaligned(ref destination[0 * sizeof(ulong)], u3);
			Unsafe.WriteUnaligned(ref destination[1 * sizeof(ulong)], u2);
			Unsafe.WriteUnaligned(ref destination[2 * sizeof(ulong)], u1);
			Unsafe.WriteUnaligned(ref destination[3 * sizeof(ulong)], u0);

			bytesWritten = Size;
			return true;
		}
	}

	/// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteLittleEndian(Span{byte}, out int)"/>
	bool IBinaryInteger<UInt256>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
	{
		if (destination.Length < Size)
		{
			bytesWritten = 0;
			return false;
		}
		else
		{
			ulong u0 = BitConverter.IsLittleEndian ? _u0 : BinaryPrimitives.ReverseEndianness(_u0);
			ulong u1 = BitConverter.IsLittleEndian ? _u1 : BinaryPrimitives.ReverseEndianness(_u1);
			ulong u2 = BitConverter.IsLittleEndian ? _u2 : BinaryPrimitives.ReverseEndianness(_u2);
			ulong u3 = BitConverter.IsLittleEndian ? _u3 : BinaryPrimitives.ReverseEndianness(_u3);

			Unsafe.WriteUnaligned(ref destination[0 * sizeof(ulong)], u0);
			Unsafe.WriteUnaligned(ref destination[1 * sizeof(ulong)], u1);
			Unsafe.WriteUnaligned(ref destination[2 * sizeof(ulong)], u2);
			Unsafe.WriteUnaligned(ref destination[3 * sizeof(ulong)], u3);

			bytesWritten = Size;
			return true;
		}
	}

	//
	// IBinaryNumber
	//

	/// <inheritdoc cref="IBinaryNumber{TSelf}.AllBitsSet"/>
	static UInt256 IBinaryNumber<UInt256>.AllBitsSet => MaxValue;

	/// <inheritdoc cref="IBinaryNumber{TSelf}.IsPow2(TSelf)"/>
	public static bool IsPow2(UInt256 value) => Popcnt(value) == 1;

	/// <inheritdoc cref="IBinaryNumber{TSelf}.Log2(TSelf)"/>
	public static UInt256 Log2(UInt256 value)
	{
		int v = value switch    // Manual inlining to avoid additional copies
		{
			{ U3: not 0 } => 0 * 8 * sizeof(ulong) + BitOperations.Log2(value._u3),
			{ U2: not 0 } => 1 * 8 * sizeof(ulong) + BitOperations.Log2(value._u2),
			{ U1: not 0 } => 2 * 8 * sizeof(ulong) + BitOperations.Log2(value._u1),
			_ => 3 * 8 * sizeof(ulong) + BitOperations.Log2(value._u0),
		};

		return Zero with { U0 = (uint)v };
	}

	//
	// IBitwiseOperators
	//

	public static UInt256 And(UInt256 left, UInt256 right) => left & right;

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_BitwiseAnd(TSelf, TOther)" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SkipLocalsInit]
	public unsafe static UInt256 operator &(UInt256 left, UInt256 right)
	{
		Unsafe.SkipInit(out MutableUInt256 mut);

		if (Vector256.IsHardwareAccelerated)
		{
			Vector256<ulong> l = Vector256.Load((ulong*)&left);

			Vector256<ulong> o = l & Vector256.Load((ulong*)&right);

			o.Store((ulong*)&mut);

			return mut._value;
		}
		else if (Vector128.IsHardwareAccelerated)
		{
			Vector128<ulong> ll = Vector128.Load(&left._u0);
			Vector128<ulong> lu = Vector128.Load(&left._u2);

			Vector128<ulong> ol = ll & Vector128.Load(&right._u0);
			Vector128<ulong> ou = lu & Vector128.Load(&right._u2);

			ol.Store((ulong*)&mut + 0);
			ou.Store((ulong*)&mut + 2);

			return mut._value;
		}
		else
		{

			mut._u0 = left._u0 & right._u0;
			mut._u1 = left._u1 & right._u1;
			mut._u2 = left._u2 & right._u2;
			mut._u3 = left._u3 & right._u3;

			return mut._value;
		}
	}

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_BitwiseOr(TSelf, TOther)" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SkipLocalsInit]
	public unsafe static UInt256 operator |(UInt256 left, UInt256 right)
	{
		Unsafe.SkipInit(out MutableUInt256 mut);

		if (Vector256.IsHardwareAccelerated)
		{
			Vector256<ulong> l = Vector256.Load((ulong*)&left);
			Vector256<ulong> o = l | Vector256.Load((ulong*)&right);

			o.Store((ulong*)&mut);

			return mut._value;
		}
		else if (Vector128.IsHardwareAccelerated)
		{

			Vector128<ulong> ll = Vector128.Load(&left._u0);
			Vector128<ulong> lu = Vector128.Load(&left._u2);

			Vector128<ulong> ol = ll | Vector128.Load(&right._u0);
			Vector128<ulong> ou = lu | Vector128.Load(&right._u2);

			ol.Store((ulong*)&mut + 0);
			ou.Store((ulong*)&mut + 2);

			return mut._value;
		}
		else
		{
			mut._u0 = left._u0 | right._u0;
			mut._u1 = left._u1 | right._u1;
			mut._u2 = left._u2 | right._u2;
			mut._u3 = left._u3 | right._u3;

			return mut._value;
		}
	}

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_ExclusiveOr(TSelf, TOther)" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SkipLocalsInit]
	public unsafe static UInt256 operator ^(UInt256 left, UInt256 right)
	{
		Unsafe.SkipInit(out MutableUInt256 mut);

		if (Vector256.IsHardwareAccelerated)
		{
			Vector256<ulong> l = Vector256.Load((ulong*)&left);

			Vector256<ulong> o = l ^ Vector256.Load((ulong*)&right);

			o.Store((ulong*)&mut);

			return mut._value;
		}
		else if (Vector128.IsHardwareAccelerated)
		{

			Vector128<ulong> ll = Vector128.Load(&left._u0);
			Vector128<ulong> lu = Vector128.Load(&left._u2);

			Vector128<ulong> ol = ll ^ Vector128.Load(&right._u0);
			Vector128<ulong> ou = lu ^ Vector128.Load(&right._u2);

			ol.Store((ulong*)&mut + 0);
			ou.Store((ulong*)&mut + 2);

			return mut._value;
		}
		else
		{
			mut._u0 = left._u0 ^ right._u0;
			mut._u1 = left._u1 ^ right._u1;
			mut._u2 = left._u2 ^ right._u2;
			mut._u3 = left._u3 ^ right._u3;

			return mut._value;
		}
	}

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_OnesComplement(TSelf)" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SkipLocalsInit]
	public unsafe static UInt256 operator ~(UInt256 value)
	{
		Unsafe.SkipInit(out MutableUInt256 mut);

		if (Vector256.IsHardwareAccelerated)
		{
			Vector256<ulong> v = Vector256.Load((ulong*)&value);
			Vector256<ulong> o = ~v;

			o.Store((ulong*)&mut);
			return mut._value;
		}
		else if (Vector128.IsHardwareAccelerated)
		{
			Vector128<ulong> vl = Vector128.Load(&value._u0);
			Vector128<ulong> vu = Vector128.Load(&value._u2);

			Vector128<ulong> ol = ~vl;
			Vector128<ulong> ou = ~vu;

			ol.Store((ulong*)&mut._value + 0);
			ou.Store((ulong*)&mut._value + 2);

			return mut._value;
		}
		else
		{
			mut._u0 = ~value._u0;
			mut._u1 = ~value._u1;
			mut._u2 = ~value._u2;
			mut._u3 = ~value._u3;

			return mut._value;
		}
	}

	//
	// IComparisonOperators
	//

	/// <inheritdoc cref="IComparisonOperators{TSelf, TOther}.op_LessThan(TSelf, TOther)" />
	public static bool operator <(UInt256 left, UInt256 right)
	{
		if (left._u3 == right._u3)
		{
			if (left._u2 == right._u2)
			{
				if (left._u1 == right._u1)
					return left._u0 < right._u0;
				else
					return left._u1 < right._u1;
			}
			else
				return left._u2 < right._u2;
		}
		else
			return left._u3 < right._u3;
	}

	/// <inheritdoc cref="IComparisonOperators{TSelf, TOther}.op_LessThanOrEqual(TSelf, TOther)" />
	public static bool operator <=(UInt256 left, UInt256 right)
	{
		if (left._u3 == right._u3)
		{
			if (left._u2 == right._u2)
			{
				if (left._u1 == right._u1)
					return left._u0 <= right._u0;
				else
					return left._u1 <= right._u1;
			}
			else
				return left._u2 <= right._u2;
		}
		else
			return left._u3 <= right._u3;
	}

	/// <inheritdoc cref="IComparisonOperators{TSelf, TOther}.op_GreaterThan(TSelf, TOther)" />
	public static bool operator >(UInt256 left, UInt256 right)
	{
		if (left._u3 == right._u3)
		{
			if (left._u2 == right._u2)
			{
				if (left._u1 == right._u1)
					return left._u0 > right._u0;
				else
					return left._u1 > right._u1;
			}
			else
				return left._u2 > right._u2;
		}
		else
			return left._u3 > right._u3;
	}

	/// <inheritdoc cref="IComparisonOperators{TSelf, TOther}.op_GreaterThanOrEqual(TSelf, TOther)" />
	public static bool operator >=(UInt256 left, UInt256 right)
	{
		if (left._u3 == right._u3)
		{
			if (left._u2 == right._u2)
			{
				if (left._u1 == right._u1)
					return left._u0 >= right._u0;
				else
					return left._u1 >= right._u1;
			}
			else
				return left._u2 >= right._u2;
		}
		else
			return left._u3 >= right._u3;
	}

	//
	// IDecrementOperators
	//

	/// <inheritdoc cref="IDecrementOperators{TSelf}.op_Decrement(TSelf)" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SkipLocalsInit]
	public static unsafe UInt256 operator --(UInt256 value)
	{
		Unsafe.SkipInit(out MutableUInt256 o);
		o._value = value;

		o._u0--;
		if (o._u0 == ulong.MaxValue)
		{
			o._u1--;
			if (o._u1 == ulong.MaxValue)
			{
				o._u2--;
				if (o._u2 == ulong.MaxValue)
					o._u3--;
			}
		}

		return o._value;
	}

	/// <inheritdoc cref="IDecrementOperators{TSelf}.op_Decrement(TSelf)" />
	[SkipLocalsInit]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe UInt256 operator checked --(UInt256 value)
	{
		Unsafe.SkipInit(out MutableUInt256 o);
		o._value = value;

		o._u0--;
		if (o._u0 == ulong.MaxValue)
		{
			o._u1--;
			if (o._u1 == ulong.MaxValue)
			{
				o._u2--;
				if (o._u2 == ulong.MaxValue)
					_ = checked(o._u3--);
			}
		}

		return o._value;
	}

	//
	// IDivisionOperators
	//

	/// <inheritdoc cref="IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
	[SkipLocalsInit]
	public static UInt256 operator /(UInt256 left, UInt256 right)
	{
		if (right._u3 == 0 && left._u3 == 0 && right._u2 == 0 && left._u2 == 0 && right._u1 == 0 && left._u1 == 0)
		{
			// left and right are both uint64
			return left._u0 / right._u0;
		}

		if (right >= left)
			return (right == left) ? 1U : 0U;

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

		static bool DivideGuessTooBig(ulong q, ulong valHi, uint valLo, uint divHi, uint divLo)
		{
			Debug.Assert(q <= 0xFFFFFFFF);

			// We multiply the two most significant limbs of the divisor
			// with the current guess for the quotient. If those are bigger
			// than the three most significant limbs of the current dividend
			// we return true, which means the current guess is still too big.

			ulong chkHi = divHi * q;
			ulong chkLo = divLo * q;

			chkHi += (chkLo >> 32);
			chkLo = (uint)(chkLo);

			return (chkHi > valHi) || ((chkHi == valHi) && (chkLo > valLo));
		}

		unsafe static UInt256 DivideSlow(UInt256 quotient, UInt256 divisor)
		{
			// This is the same algorithm currently used by BigInteger so
			// we need to get a Span<uint> containing the value represented
			// in the least number of elements possible.

			// We need to ensure that we end up with 4x uints representing the bits from
			// least significant to most significant so the math will be correct on both
			// little and big endian systems. So we'll just allocate the relevant buffer
			// space and then write out the four parts using the native endianness of the
			// system.

			uint* pLeft = stackalloc uint[Size / sizeof(uint)];

			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 0), (uint)(quotient._u0 >> 00));
			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 1), (uint)(quotient._u0 >> 32));

			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 2), (uint)(quotient._u1 >> 00));
			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 3), (uint)(quotient._u1 >> 32));

			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 4), (uint)(quotient._u2 >> 00));
			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 5), (uint)(quotient._u2 >> 32));

			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 6), (uint)(quotient._u3 >> 00));
			Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 7), (uint)(quotient._u3 >> 32));

			Span<uint> left = new Span<uint>(pLeft, (Size / sizeof(uint)) - (Lzcnt(quotient) / 32));

			// Repeat the same operation with the divisor

			uint* pRight = stackalloc uint[Size / sizeof(uint)];

			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 0), (uint)(divisor._u0 >> 00));
			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 1), (uint)(divisor._u0 >> 32));

			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 2), (uint)(divisor._u1 >> 00));
			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 3), (uint)(divisor._u1 >> 32));

			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 4), (uint)(divisor._u2 >> 00));
			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 5), (uint)(divisor._u2 >> 32));

			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 6), (uint)(divisor._u3 >> 00));
			Unsafe.WriteUnaligned(ref *(byte*)(pRight + 7), (uint)(divisor._u3 >> 32));

			Span<uint> right = new Span<uint>(pRight, (Size / sizeof(uint)) - (Lzcnt(divisor) / 32));

			Span<uint> rawBits = stackalloc uint[Size / sizeof(uint)];
			rawBits.Clear();
			Span<uint> bits = rawBits.Slice(0, left.Length - right.Length + 1);

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
					uint carry = SubtractDivisor(left.Slice(n), right, digit);

					if (carry != t)
					{
						Debug.Assert(carry == (t + 1));

						// Our guess was still exactly one too high
						carry = AddDivisor(left.Slice(n), right);

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

			return new UInt256(
				((ulong)(rawBits[7]) << 32) | rawBits[6],
				((ulong)(rawBits[5]) << 32) | rawBits[4],
				((ulong)(rawBits[3]) << 32) | rawBits[2],
				((ulong)(rawBits[1]) << 32) | rawBits[0]
			);
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
				{
					++carry;
				}

				leftElement -= digit;
			}

			return (uint)(carry);
		}
	}

	/// <inheritdoc cref="IDivisionOperators{TSelf, TOther, TResult}.op_CheckedDivision(TSelf, TOther)" />
	public static UInt256 operator checked /(UInt256 left, UInt256 right) => left / right;

	//
	// IEqualityOperators
	//

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther}.op_Equality(TSelf, TOther)"/>
	[SkipLocalsInit]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe bool operator ==(UInt256 left, UInt256 right)
	{
		if (Vector256.IsHardwareAccelerated)
		{
			Vector256<ulong> l = Vector256.Load((ulong*)&left);
			Vector256<ulong> r = Vector256.Load((ulong*)&right);

			return l == r;
		}
		else
			return left._u0 == right._u0 && left._u1 == right._u1 && left._u2 == right._u2 && left._u3 == right._u3;
	}

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther}.op_Inequality(TSelf, TOther)"/>
	public static unsafe bool operator !=(UInt256 left, UInt256 right)
	{
		if (!Vector256.IsHardwareAccelerated)
		{
			Vector256<ulong> l = Vector256.Load((ulong*)&left);
			Vector256<ulong> r = Vector256.Load((ulong*)&right);

			return l != r;
		}
		else
			return left._u0 != right._u0 || left._u1 != right._u1 || left._u2 != right._u2 || left._u3 != right._u3;
	}

	//
	// IIncrementOperators
	//

	/// <inheritdoc cref="IIncrementOperators{TSelf}.op_Increment(TSelf)" />
	[SkipLocalsInit]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe UInt256 operator ++(UInt256 value)
	{
		Unsafe.SkipInit(out MutableUInt256 o);
		o._value = value;

		o._u0++;
		if (o._u0 == 0)
		{
			o._u1++;
			if (o._u1 == 0)
			{
				o._u2++;
				if (o._u2 == 0)
					o._u3++;
			}
		}

		return o._value;
	}

	/// <inheritdoc cref="IIncrementOperators{TSelf}.op_CheckedIncrement(TSelf)" />
	[SkipLocalsInit]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UInt256 operator checked ++(UInt256 value)
	{
		Unsafe.SkipInit(out MutableUInt256 o);
		o._value = value;

		o._u0++;
		if (o._u0 == 0)
		{
			o._u1++;
			if (o._u1 == 0)
			{
				o._u2++;
				if (o._u2 == 0)
					_ = checked(o._u3++);
			}
		}

		return o._value;
	}

	//
	// IMinMaxValue
	//

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue"/>
	public static UInt256 MinValue => default;

	public static UInt256 M() => MaxValue;

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue"/>
	public unsafe static UInt256 MaxValue
	{
		[SkipLocalsInit]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (Vector256.IsHardwareAccelerated)
			{
				Unsafe.SkipInit(out UInt256 o);
				Vector256<ulong>.AllBitsSet.Store((ulong*)&o);

				return o;
			}
			else if (Vector128.IsHardwareAccelerated)
			{
				Unsafe.SkipInit(out UInt256 o);

				Vector128<ulong>.AllBitsSet.Store((ulong*)&o + 0);
				Vector128<ulong>.AllBitsSet.Store((ulong*)&o + 2);

				return o;
			}
			else
				return new UInt256(ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
		}
	}

	//
	// IModulusOperators
	//

	/// <inheritdoc cref="IModulusOperators{TSelf, TOther, TResult}.op_Modulus(TSelf, TOther)" />
	[SkipLocalsInit]
	public static UInt256 operator %(UInt256 left, UInt256 right)
	{
		if (right._u3 == 0 && left._u3 == 0 && right._u2 == 0 && left._u2 == 0 && right._u1 == 0 && left._u1 == 0)
		{
			// left and right are both uint64
			return left._u0 % right._u0;
		}

		if (right >= left)
			return (right == left) ? Zero : left;

		return ModSlow(left, right);
	}

	[SkipLocalsInit]
	static unsafe UInt256 ModSlow(UInt256 quotient, UInt256 divisor)
	{
		uint* pLeft = stackalloc uint[sizeof(UInt256) / sizeof(uint)];

		if (BitConverter.IsLittleEndian)
			Unsafe.Write(pLeft, quotient);
		else
		{
			*(pLeft + 0) = (uint)(quotient._u0 >> 00);
			*(pLeft + 1) = (uint)(quotient._u0 >> 32);
			*(pLeft + 2) = (uint)(quotient._u1 >> 00);
			*(pLeft + 3) = (uint)(quotient._u1 >> 32);
			*(pLeft + 4) = (uint)(quotient._u2 >> 00);
			*(pLeft + 5) = (uint)(quotient._u2 >> 32);
			*(pLeft + 6) = (uint)(quotient._u3 >> 00);
			*(pLeft + 7) = (uint)(quotient._u3 >> 32);
		}

		Span<uint> left = new Span<uint>(pLeft, 8 - (Lzcnt(quotient) / 32));

		// Setup buffer for right UInt256
		uint* pRight = stackalloc uint[sizeof(UInt256) / sizeof(uint)];

		if (BitConverter.IsLittleEndian)
			Unsafe.Write(pRight, divisor);
		else
		{
			*(pRight + 0) = (uint)(divisor._u0 >> 00);
			*(pRight + 1) = (uint)(divisor._u0 >> 32);
			*(pRight + 2) = (uint)(divisor._u1 >> 00);
			*(pRight + 3) = (uint)(divisor._u1 >> 32);
			*(pRight + 4) = (uint)(divisor._u2 >> 00);
			*(pRight + 5) = (uint)(divisor._u2 >> 32);
			*(pRight + 6) = (uint)(divisor._u3 >> 00);
			*(pRight + 7) = (uint)(divisor._u3 >> 32);
		}

		Span<uint> right = new Span<uint>(pRight, 8 - (Lzcnt(divisor) / 32));

		BigIntegerCalculator.Divide(left, right, default);

		return new UInt256(
			(ulong)(left[7] << 32) | left[6],
			(ulong)(left[5] << 32) | left[4],
			(ulong)(left[3] << 32) | left[2],
			(ulong)(left[1] << 32) | left[0]
		);
	}

	//
	// IMultiplicativeIdentity
	//

	/// <inheritdoc cref="IMultiplicativeIdentity{TSelf, TResult}.MultiplicativeIdentity" />
	static UInt256 IMultiplicativeIdentity<UInt256, UInt256>.MultiplicativeIdentity => One;

	//
	// IMultiplyOperators
	//

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SkipLocalsInit]
	public static unsafe UInt256 operator -(UInt256 left, UInt256 right)
	{
		Unsafe.SkipInit(out MutableUInt256 mut);

		if (Vector256.IsHardwareAccelerated)
		{
			Vector256<ulong> l = Vector256.LoadUnsafe(ref Unsafe.AsRef(in left._u0));
			Vector256<ulong> o = l - Vector256.LoadUnsafe(ref Unsafe.AsRef(in right._u0));

			o.Store((ulong*)&mut);
		}
		else if (Vector128.IsHardwareAccelerated)
		{
			Vector128<ulong> ll = Vector128.LoadUnsafe(ref Unsafe.AsRef(in left._u0));
			Vector128<ulong> lu = Vector128.LoadUnsafe(ref Unsafe.AsRef(in left._u2));

			Vector128<ulong> ol = ll - Vector128.LoadUnsafe(ref Unsafe.AsRef(in right._u0));
			Vector128<ulong> ou = lu - Vector128.LoadUnsafe(ref Unsafe.AsRef(in right._u2));

			ol.StoreUnsafe(ref Unsafe.AsRef(in mut._u0));
			ou.StoreUnsafe(ref Unsafe.AsRef(in mut._u2));
		}
		else
		{
			mut._u0 = left._u0 - right._u0;
			mut._u1 = left._u1 - right._u1;
			mut._u2 = left._u2 - right._u2;
			mut._u3 = left._u3 - right._u3;
		}

		if (mut._u0 > right._u0)
			mut._u1--;
		if (mut._u1 > right._u1)
			mut._u2--;
		if (mut._u2 > right._u2)
			mut._u3--;

		return mut._value;
	}

	/// <inheritdoc cref="ISubtractionOperators{TSelf, TOther, TResult}.op_CheckedSubtraction(TSelf, TOther)"/>
	public unsafe static UInt256 operator checked -(UInt256 left, UInt256 right)
	{
		Unsafe.SkipInit(out MutableUInt256 mut);

		if (Vector256.IsHardwareAccelerated)
		{
			Vector256<ulong> l = Vector256.LoadUnsafe(ref Unsafe.AsRef(in left._u0));
			Vector256<ulong> o = l - Vector256.LoadUnsafe(ref Unsafe.AsRef(in right._u0));

			o.Store((ulong*)&mut);
		}
		else if (Vector128.IsHardwareAccelerated)
		{
			Vector128<ulong> ll = Vector128.LoadUnsafe(ref Unsafe.AsRef(in left._u0));
			Vector128<ulong> lu = Vector128.LoadUnsafe(ref Unsafe.AsRef(in left._u2));

			Vector128<ulong> ol = ll - Vector128.LoadUnsafe(ref Unsafe.AsRef(in right._u0));
			Vector128<ulong> ou = lu - Vector128.LoadUnsafe(ref Unsafe.AsRef(in right._u2));

			ol.StoreUnsafe(ref Unsafe.AsRef(in mut._u0));
			ou.StoreUnsafe(ref Unsafe.AsRef(in mut._u2));
		}
		else
		{
			mut._u0 = left._u0 - right._u0;
			mut._u1 = left._u1 - right._u1;
			mut._u2 = left._u2 - right._u2;
			mut._u3 = left._u3 - right._u3;
		}

		if (mut._u0 > right._u0)
			mut._u1--;
		if (mut._u1 > right._u1)
			mut._u2--;
		if (mut._u2 > right._u2)
			_ = checked(mut._u3--);

		return mut._value;
	}

	//
	// IUnaryNegationOperators
	//

	/// <inheritdoc cref="IUnaryNegationOperators{TSelf, TResult}.op_UnaryNegation(TSelf)" />
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SkipLocalsInit]
	public static unsafe UInt256 operator -(UInt256 value)
	{
		Unsafe.SkipInit(out MutableUInt256 mut);

		if (Vector256.IsHardwareAccelerated)
		{
			Vector256<ulong> o = Vector256<ulong>.Zero - Vector256.LoadUnsafe(ref Unsafe.AsRef(in value._u0));

			o.StoreUnsafe(ref Unsafe.AsRef(in mut._u0));
		}
		else if (Vector128.IsHardwareAccelerated)
		{
			Vector128<ulong> rl = Vector128.LoadUnsafe(ref Unsafe.AsRef(in value._u0));
			Vector128<ulong> ru = Vector128.LoadUnsafe(ref Unsafe.AsRef(in value._u2));

			Vector128<ulong> ol = Vector128<ulong>.Zero - rl;
			Vector128<ulong> ou = Vector128<ulong>.Zero - ru;

			ol.StoreUnsafe(ref Unsafe.AsRef(in mut._u0));
			ou.StoreUnsafe(ref Unsafe.AsRef(in mut._u2));
		}
		else
		{
			mut._u0 = 0 - value._u0;
			mut._u1 = 0 - value._u1;
			mut._u2 = 0 - value._u2;
			mut._u3 = 0 - value._u3;
		}

		if (mut._u0 > value._u0)
			mut._u1--;
		if (mut._u1 > value._u1)
			mut._u2--;
		if (mut._u2 > value._u2)
			mut._u3--;

		return mut._value;
	}

	/// <inheritdoc cref="IUnaryNegationOperators{TSelf, TResult}.op_CheckedUnaryNegation(TSelf)" />
	public unsafe static UInt256 operator checked -(UInt256 value)
	{
		Unsafe.SkipInit(out MutableUInt256 mut);

		if (Vector256.IsHardwareAccelerated)
		{
			Vector256<ulong> o = Vector256<ulong>.Zero - Vector256.LoadUnsafe(ref Unsafe.AsRef(in value._u0));

			o.StoreUnsafe(ref Unsafe.AsRef(in mut._u0));
		}
		else if (Vector128.IsHardwareAccelerated)
		{
			Vector128<ulong> rl = Vector128.LoadUnsafe(ref Unsafe.AsRef(in value._u0));
			Vector128<ulong> ru = Vector128.LoadUnsafe(ref Unsafe.AsRef(in value._u2));

			Vector128<ulong> ol = Vector128<ulong>.Zero - rl;
			Vector128<ulong> ou = Vector128<ulong>.Zero - ru;

			ol.StoreUnsafe(ref Unsafe.AsRef(in mut._u0));
			ou.StoreUnsafe(ref Unsafe.AsRef(in mut._u2));
		}
		else
		{
			mut._u0 = 0 - value._u0;
			mut._u1 = 0 - value._u1;
			mut._u2 = 0 - value._u2;
			mut._u3 = 0 - value._u3;
		}

		if (mut._u0 > value._u0)
			mut._u1--;
		if (mut._u1 > value._u1)
			mut._u2--;
		if (mut._u2 > value._u2)
			_ = checked(mut._u3--);

		return mut._value;
	}

	//
	// IUnaryPlusOperators
	//

	/// <inheritdoc cref="IUnaryPlusOperators{TSelf, TResult}.op_UnaryPlus(TSelf)" />
	public static UInt256 operator +(UInt256 value) => value;
}
