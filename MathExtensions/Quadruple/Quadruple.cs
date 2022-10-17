using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Text;

namespace MathExtensions
{
	/// <summary>
	/// Represents a IEEE 754-2008 binary128 quadruple-precision floating-point number.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe readonly partial struct Quadruple
		: IComparable,
		  IComparable<Quadruple>,
		  IEquatable<Quadruple>,
		  IBinaryFloatingPointIeee754<Quadruple>,
		  IMinMaxValue<Quadruple>
	{
		internal const int Size = 16;

		internal const int Bias = 0x3FFF;
		internal const int BiasedSpecialExp = 0x4000;
		internal const int SpecialExp = 0x7FFF;

		internal const ulong SignMaskUpper = 0x8000_0000_0000_0000;
		internal const ulong ExponentMaskUpper = 0x7FFF_0000_0000_0000;
		internal const uint ShiftedBiasedExponentMask = 0x7FFF;
		internal const int ExponentStartBitUpper = 48;
		internal const int SignBit = 63;
		internal const int ExponentBits = 15;
		internal const int FractionBits = 113;

		internal const ulong UpperNormalBit = 0x0001_0000_0000_0000;
		internal const ulong UpperFractionMask = 0x0000_FFFF_FFFF_FFFF;

		internal const ulong PositiveInfinityUpper = 0x7FFF_0000_0000_0000;
		internal const ulong NegativeInfinityUpper = 0xFFFF_0000_0000_0000;
		public readonly ulong _lower;   // bits 0...63
		public readonly ulong _upper;   // bits 64...127

		/// <summary>Represents the largest possible value of a <see cref="Quadruple"/>.</summary>
		/// <remarks><see cref="MaxValue"/> is approximately 1.1897314953572317650857593266280070162 × 10^4932.</remarks>
		public static Quadruple MaxValue { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Quadruple(0x7FFE_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF); }
		/// <summary>Represents the smallest possible value of a <see cref="Quadruple"/>.</summary>
		/// <remarks><see cref="MinValue"/> is approximately -1.1897314953572317650857593266280070162 × 10^4932.</remarks>
		public static Quadruple MinValue { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Quadruple(0xFFFE_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF); }

		/// <summary>Represents the value zero.</summary>
		public static Quadruple Zero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Quadruple(0x0000_0000_0000_0000, 0x0000_0000_0000_0000); }
		/// <summary>Represents the value negative zero.</summary>
		public static Quadruple NegativeZero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Quadruple(0x8000_0000_0000_0000, 0x0000_0000_0000_0000); }

		/// <summary>Represents the value one.</summary>
		public static Quadruple One { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Quadruple(0x3FFF_0000_0000_0000, 0x0000_0000_0000_0000); }
		/// <summary>Represents the value negative one.</summary>
		public static Quadruple NegativeOne { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Quadruple(0xBFFF_0000_0000_0000, 0x0000_0000_0000_0000); }

		/// <summary>Represents positive infintiy.</summary>
		public static Quadruple PositiveInfinity { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Quadruple(0x7FFF_0000_0000_0000, 0x0000_0000_0000_0000); }
		/// <summary>Represents negative infinity.</summary>
		public static Quadruple NegativeInfinity { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Quadruple(0xFFFF_0000_0000_0000, 0x0000_0000_0000_0000); }

		/// <summary>Represents the smallest positive <see cref="Quadruple"/> value that is greater than zero.</summary>
		/// <remarks>Epislon is approximately 6.4751751194380251109244389582276465525 × 10^-4966.</remarks>
		public static Quadruple Epsilon { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Quadruple(0x0000_0000_0000_0000, 0x0000_0000_0000_0001); }
		/// <summary>Represents a value that is not a number (NaN).</summary>
		public static Quadruple NaN { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Quadruple(0xFFFF_8000_0000_0000, 0x0000_0000_0000_0000); }

		/// <summary>Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π.</summary>
		/// <remarks>Pi is approximately 3.14159265358979323846264338327950280.</remarks>
		public static Quadruple Pi { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Quadruple(0x4000_921F_B544_42D1, 0x8469_898C_C517_01B8); }
		/// <summary>Represents the number of radians in one turn, specified by the constant, τ.</summary>
		/// <remarks>Tau is approximately 6.28318530717958647692528676655900559.</remarks>
		public static Quadruple Tau { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Quadruple(0x4001_921F_B544_42D1, 0x8469_898C_C517_01B8); }
		/// <summary>Represents the natural logarithmic base, specified by the constant, e.</summary>
		/// <remarks>E is apporximately 2.71828182836969448633845487297936844.</remarks>
		public static Quadruple E { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Quadruple(0x4000_5BF0_A8B1_1457, 0x9535_5FB8_AC40_4E7A); }

		/// <summary>
		/// Initializes a new instance of the <see cref="Quadruple"/> struct.
		/// </summary>
		/// <param name="upper">The upper 64-bits of the <see cref="Quadruple"/> value.</param>
		/// <param name="lower">The lower 64-bits of the <see cref="Quadruple"/> value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Quadruple(ulong upper, ulong lower)
		{
			_lower = lower;
			_upper = upper;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		// Constructs a Quadruple from the sign, exponent, and upper and lower parts of the fraction
		internal Quadruple(ulong sign, int exponent, ulong upperFrac, ulong lowerFrac)
		{
			const ulong UPPER_FRAC_MASK = 0x0000_FFFF_FFFF_FFFF;
			const uint EXP_MASK = 0x7FFF;

			ulong biasedExponent = (uint)(exponent + Bias) & EXP_MASK;
			_lower = lowerFrac;
			_upper = (sign << SignBit) | (biasedExponent << ExponentStartBitUpper) | (upperFrac & UPPER_FRAC_MASK);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		// Constructs a Quadruple from the sign, exponent, and upper and lower parts of the fraction
		// Requires everything is in range before being passed to constructor
		private Quadruple(ulong sign, uint biasedExponent, ulong upperFrac, ulong lowerFrac)
		{
			const ulong UPPER_FRAC_MASK = 0x0000_FFFF_FFFF_FFFF;

			_lower = lowerFrac;
			_upper = (sign << SignBit) | (biasedExponent << ExponentStartBitUpper) | (upperFrac & UPPER_FRAC_MASK);
		}

		/// <summary>
		/// Determines whether the specified value is finite (zero, subnormal, or normal).
		/// </summary>
		/// <param name="q">A quadruple-precision floating-point number.</param>
		/// <returns><see langword="true"/> if the value is finite (zero, subnormal, or normal); <see langword="false" /> otherwise.</returns>
		public static bool IsFinite(Quadruple q) => ((q._upper << 1) >> (ExponentStartBitUpper + 1)) != SpecialExp;

		/// <summary>
		/// Returns a value indicating whether the specified number evaluates to negative or positive infinity.
		/// </summary>
		/// <param name="q">A quadruple-precision floating-point number.</param>
		/// <returns><see langword="true"/> if <paramref name="q"/> evaluates to <see cref="PositiveInfinity"/> or <see cref="NegativeInfinity"/>; otherwise, <see langword="false"/></returns>
		public static bool IsInfinity(Quadruple q) => (q._upper << 1) == 0xFFFE_0000_0000_0000 && q._lower == 0;

		/// <summary>
		/// Returns a value that indicates whether the specified value is not a number (<see cref="NaN"/>).
		/// </summary>
		/// <param name="q">A quadruple-precision floating-point number.</param>
		/// <returns><see langword="true" /> if <paramref name="q"/> evaluates to <see cref="NaN" />; otherwise, <see langword="false"/></returns>
		public static bool IsNaN(Quadruple q)
		{
			return
				((q._upper << 1) >> (ExponentStartBitUpper + 1)) == SpecialExp &&
				((q._upper << 16) | q._lower) != 0;
		}

		/// <summary>
		/// Determines whether the specified value is negative.
		/// </summary>
		/// <param name="q">A quadruple-precision floating-point number.</param>
		/// <returns><see langword="true" /> if the value is negative; <see langword="false" /> otherwise.</returns>
		public static bool IsNegative(Quadruple q) => (long)q._upper < 0;

		/// <summary>
		/// Returns a value indicating whether the specified number evaluates to negative infinity.
		/// </summary>
		/// <param name="q">A quadruple-precision floating-point number</param>
		/// <returns><see langword="true"/> if <paramref name="q"/> evaluates to <see cref="NegativeInfinity"/>; otherwise, <see langword="false" /></returns>
		public static bool IsNegativeInfinity(Quadruple q) => q._upper == 0xFFFF_0000_0000_0000 && q._lower == 0;

		/// <summary>
		/// Determines whether the specified value is normal.
		/// </summary>
		/// <param name="q">A quadruple-precision floating-point number</param>
		/// <returns><see langword="true"/> if the value is normal; otherwise, <see langword="false" /></returns>
		public static bool IsNormal(Quadruple q)
		{
			ulong upper = q._upper << 1;
			return upper >= 0x0002_0000_0000_0000 && upper < 0xFFFE_0000_0000_0000;
		}

		/// <summary>
		/// Returns a value indicating whether the specified number evaluates to positive infinity.
		/// </summary>
		/// <param name="q">A quadruple-precision floating-point number</param>
		/// <returns><see langword="true"/> if <paramref name="q"/> evaluates to <see cref="PositiveInfinity"/>; otherwise, <see langword="false" /></returns>
		public static bool IsPositiveInfinity(Quadruple q) => q._upper == 0x7FFF_0000_0000_0000 && q._lower == 0;

		/// <summary>
		/// Determines whether the specified value is subnormal.
		/// </summary>
		/// <param name="q">A quadruple-precision floating-point number</param>
		/// <returns><see langword="true"/> if the value is subnormal; otherwise, <see langword="false" /></returns>
		public static bool IsSubnormal(Quadruple q)
		{
			ulong upper = q._upper << 1;    // shift out sign bit
			return !((upper | q._lower) == 0) && (upper >> 49) == 0; // check not a zero, and exponent is zero
		}

		/// <summary>
		/// Determines whether the specified value is zero.
		/// </summary>
		/// <param name="q">A quadruple-precision floating-point number</param>
		/// <returns><see langword="true"/> if the value is zero; otherwise, <see langword="false" /></returns>
		public static bool IsZero(Quadruple q) => ((q._upper << 1) | q._lower) == 0;

		private static bool AreZero(Quadruple left, Quadruple right) => (((left._upper | right._upper) << 1) | (left._lower | right._lower)) == 0;

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is an instance of <see cref="Quadruple"/> and equals the value of this instance; otherwise, <see langword="false"/></returns>
		public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is Quadruple q && Equals(q);

		/// <inheritdoc cref="ValueType.GetHashCode"/>
		public override readonly int GetHashCode()
		{
			ulong lower = _lower;
			ulong upper = _upper;

			if (IsNaN(this) || IsZero(this))
			{
				upper &= 0x7FFF_0000_0000_0000;
				lower = 0;
			}

			return HashCode.Combine(lower, upper);
		}

		//
		// IEquatable
		//

		/// <summary>
		/// Returns a value indicating whether this instance and a speicied <see cref="Quadruple"/> object represent the same value.
		/// </summary>
		/// <param name="value">A <see cref="Quadruple"/> object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="value"/> is equal to this instance; otherwise <see langword="false"/>.</returns>
		public readonly bool Equals(Quadruple value) => this == value || (IsNaN(value) && IsNaN(this));

		public override readonly string ToString() => $"{_upper:X16}{_lower:X16}";

		//
		// IFormattable
		//

		/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
		public string ToString(string? format, IFormatProvider? provider) => throw new NotImplementedException();

		//
		// IComparable
		//

		/// <summary>
		/// Compares this object to another object, returning an integer that indicates the relationship.
		/// </summary>
		/// <returns>A value less than zero if this is less than <paramref name="obj"/>, zero if this is equal to <paramref name="obj"/>, or a value greater than zero if this is greater than <paramref name="obj"/>.</returns>
		/// <exception cref="ArgumentException">Thrown when <paramref name="obj"/> is not of type <see cref="Quadruple"/>.</exception>
		public readonly int CompareTo(object? obj)
		{
			if (obj is not Quadruple q)
				return (obj is null) ? 1 : throw new ArgumentException($"Object must be of type {nameof(Quadruple)}.");

			return CompareTo(q);
		}

		//
		// IComparable
		//

		/// <summary>
		/// Compares this object to another object, returning an integer that indicates the relationship.
		/// </summary>
		/// <param name="other">The value to compare to.</param>
		/// <returns>A value less than zero if this is less than <paramref name="other"/>, zero if this is equal to <paramref name="other"/>, or a value greater than zero if this is greater than <paramref name="other"/>.</returns>
		public readonly int CompareTo(Quadruple other)
		{
			if (this < other)
				return -1;

			if (this > other)
				return 1;

			if (this == other)
				return 0;

			if (IsNaN(this))
				return IsNaN(other) ? 0 : -1;

			return 1;
		}

		/// <inheritdoc cref="IComparisonOperators{TSelf, TOther}.op_LessThan(TSelf, TOther)" />
		public static bool operator <(Quadruple left, Quadruple right)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool UInt128LessThan(Quadruple left, Quadruple right) => (left._upper < right._upper) || ((left._upper == right._upper) && (left._lower < right._lower));

			if (IsNaN(left) || IsNaN(right))
				return false;   // NaNs are unordered.

			bool leftIsNegative = IsNegative(left);

			if (leftIsNegative != IsNegative(right))
				return leftIsNegative && !AreZero(left, right);

			return (left._upper != right._lower || left._lower != right._lower) && UInt128LessThan(left, right) ^ leftIsNegative;
		}

		/// <inheritdoc cref="IComparisonOperators{TSelf, TOther}.op_GreaterThan(TSelf, TOther)" />
		public static bool operator >(Quadruple left, Quadruple right) => right < left;

		/// <inheritdoc cref="IComparisonOperators{TSelf, TOther}.op_LessThanOrEqual(TSelf, TOther)" />
		public static bool operator <=(Quadruple left, Quadruple right)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool UInt128LessThan(Quadruple left, Quadruple right) => (left._upper < right._upper) || ((left._upper == right._upper) && (left._lower < right._lower));

			if (IsNaN(left) || IsNaN(right))
				return false;   // NaNs are unordered.

			bool leftIsNegative = IsNegative(left);

			if (leftIsNegative != IsNegative(right))
				return leftIsNegative || !AreZero(left, right);

			return (left._upper == right._lower && left._lower == right._lower) || UInt128LessThan(left, right) ^ leftIsNegative;
		}

		/// <inheritdoc cref="IComparisonOperators{TSelf, TOther}.op_GreaterThanOrEqual(TSelf, TOther)" />
		public static bool operator >=(Quadruple left, Quadruple right) => right <= left;

		/// <summary>
		/// Returns a value that indicates whether two specified <see cref="Quadruple"/> values are equal.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns><see langword="true" /> if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(Quadruple left, Quadruple right)
		{
			return (!IsNaN(left) && !IsNaN(right) && // Numbers are unequal if either are NaN
				left._upper == right._upper && left._lower == right._lower) || AreZero(left, right);
		}

		/// <summary>
		/// Returns a value that indicates whether two specified <see cref="Quadruple"/> values are not equal.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns><see langword="true" /> if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(Quadruple left, Quadruple right) => !(left == right);

		internal static bool BitEquals(Quadruple left, Quadruple right) => left._upper == right._upper && left._lower == right._lower;

		public static implicit operator Quadruple(Half value)
		{
			const int BIT_COUNT = 16;
			const int EXPONENT_START = 10;
			const int EXPONENT_BIAS = ExponentBits;
			const int SPECIAL_EXP = EXPONENT_BIAS + 1;
			const int SIGN_BIT = ExponentBits;
			const uint FRACTION_MASK = 0x03FF;
			const uint SIGN_MASK = 0x8000;

			uint bits = BitConverter.HalfToUInt16Bits(value);
			uint sign = bits >> SIGN_BIT;
			int exp = (int)((bits & ~SIGN_MASK) >> EXPONENT_START) - EXPONENT_BIAS;
			uint frac = bits & FRACTION_MASK;

			if (exp == -EXPONENT_BIAS)  // Normalize subnormal values
			{
				int lzc = BitOperations.LeadingZeroCount(frac) - 16;

				if (lzc == BIT_COUNT)
					return new Quadruple(sign, -Bias, 0, 0);

				int shift = lzc - (BIT_COUNT - EXPONENT_START);

				exp -= shift;
				frac <<= shift + 1;
				frac &= FRACTION_MASK;
			}

			if (exp == SPECIAL_EXP)
				exp = BiasedSpecialExp;

			return new Quadruple(sign, exp, (ulong)frac << (48 - EXPONENT_START), 0);
		}

		public static implicit operator Quadruple(float value)
		{
			const int BIT_COUNT = sizeof(float) * 8;
			const int EXPONENT_START = 23;
			const int EXPONENT_BIAS = 127;
			const int SPECIAL_EXP = EXPONENT_BIAS + 1;
			const int SIGN_BIT = 31;
			const uint FRACTION_MASK = 0x007F_FFFF;
			const uint SIGN_MASK = 0x8000_0000;

			uint bits = BitConverter.SingleToUInt32Bits(value);
			uint sign = bits >> SIGN_BIT;
			int exp = (int)((bits & ~SIGN_MASK) >> EXPONENT_START) - EXPONENT_BIAS;
			uint frac = bits & FRACTION_MASK;

			if (exp == -EXPONENT_BIAS)  // Normalize subnormal values
			{
				int lzc = BitOperations.LeadingZeroCount(frac);

				if (lzc == BIT_COUNT)
					return new Quadruple(sign, -Bias, 0, 0);

				int shift = lzc - (BIT_COUNT - EXPONENT_START);

				exp -= shift;
				frac <<= shift + 1;
				frac &= FRACTION_MASK;
			}

			if (exp == SPECIAL_EXP)
				exp = BiasedSpecialExp;

			return new Quadruple(sign, exp, (ulong)frac << (48 - EXPONENT_START), 0);
		}

		public static implicit operator Quadruple(double value)
		{
			const int BIT_COUNT = sizeof(double) * 8;
			const int EXPONENT_START = 52;
			const int EXPONENT_BIAS = 1023;
			const int SPECIAL_EXP = EXPONENT_BIAS + 1;
			const int SIGN_BIT = 63;
			const ulong FRACTION_MASK = 0x000F_FFFF_FFFF_FFFF;
			const ulong SIGN_MASK = 0x8000_0000_0000_0000;

			ulong bits = BitConverter.DoubleToUInt64Bits(value);
			uint sign = (uint)(bits >> SIGN_BIT);
			int exp = (int)((bits & ~SIGN_MASK) >> EXPONENT_START) - EXPONENT_BIAS;
			ulong frac = bits & FRACTION_MASK;

			if (exp == -EXPONENT_BIAS)  // Normalize subnormal values
			{
				int lzc = BitOperations.LeadingZeroCount(frac);

				if (lzc == BIT_COUNT)
					return new Quadruple(sign, -Bias, 0, 0);

				int shift = lzc - (BIT_COUNT - EXPONENT_START);

				exp -= shift;
				frac <<= shift + 1;
				frac &= FRACTION_MASK;
			}

			if (exp == SPECIAL_EXP)
				exp = BiasedSpecialExp;

			return new Quadruple(sign, exp, frac >> (EXPONENT_START - 48), frac << (BIT_COUNT - (EXPONENT_START - 48)));
		}

		public static implicit operator Quadruple(int value)
		{
			const int ULONG_BITS = sizeof(ulong) * 8;

			if (value == 0)
				return default;

			uint sign = (uint)(value >> 31);
			ulong abs = (ulong)(long)value;
			if (sign > 0)
			{
				abs = (ulong)-(long)abs;
			}

			int exp = ULONG_BITS - 1 - BitOperations.LeadingZeroCount(abs);
			abs <<= ExponentStartBitUpper - exp;

			return new Quadruple(sign, exp, abs, 0);
		}

		public static implicit operator Quadruple(long value)
		{
			const int ULONG_BITS = sizeof(ulong) * 8;

			if (value == 0)
				return default;

			ulong sign = (uint)(value >> 63);
			ulong abs = (ulong)value;
			if (sign > 0)
			{
				abs = (ulong)-(long)abs;
			}

			int exp = ULONG_BITS - 1 - BitOperations.LeadingZeroCount(abs);

			ulong upper, lower;
			if (exp < ExponentStartBitUpper)
			{
				upper = abs << (ExponentStartBitUpper - exp);
				lower = 0;
			}
			else
			{
				upper = abs >> (exp - ExponentStartBitUpper);
				lower = abs << (ULONG_BITS - (exp - ExponentStartBitUpper));
			}

			return new Quadruple(sign, exp, upper, lower);
		}

		public static implicit operator Quadruple(Int128 value)
		{
			const int UINT128_BITS = 128;
			const int ULONG_BITS = sizeof(ulong) * 8;

			if (value == 0)
				return Zero;

			UInt128 abs = (UInt128)value;
			ulong sign = 0;
			if (Int128.IsNegative(value))
			{
				sign = 1;
				abs = (UInt128)(-(Int128)abs);
			}

			int exp = UINT128_BITS - 1 - (int)UInt128.LeadingZeroCount(abs);

			if (exp < ExponentStartBitUpper)
				abs <<= ULONG_BITS + ExponentStartBitUpper - exp;
			else
				abs >>= exp - ExponentStartBitUpper - ULONG_BITS;

			ulong lower = (ulong)abs;
			ulong upper = (ulong)(abs >> 64);

			return new Quadruple(sign, exp, upper, lower);
		}

		public static implicit operator Quadruple(uint value)
		{
			const int ULONG_BITS = sizeof(ulong) * 8;

			if (value == 0)
				return default;

			ulong abs = value;

			int exp = ULONG_BITS - 1 - BitOperations.LeadingZeroCount(abs);
			abs <<= ExponentStartBitUpper - exp;

			return new Quadruple(0, exp, abs, 0);
		}

		public static implicit operator Quadruple(ulong value)
		{
			const int ULONG_BITS = sizeof(ulong) * 8;

			if (value == 0)
				return default;

			ulong abs = value;

			int exp = ULONG_BITS - 1 - BitOperations.LeadingZeroCount(abs);

			ulong upper, lower;
			if (exp < ExponentStartBitUpper)
			{
				upper = abs << (ExponentStartBitUpper - exp);
				lower = 0;
			}
			else
			{
				upper = abs >> (exp - ExponentStartBitUpper);
				lower = abs << (ULONG_BITS - (exp - ExponentStartBitUpper));
			}

			return new Quadruple(0, exp, upper, lower);
		}

		//
		// IAdditionOperators
		//

		/// <inheritdoc cref="IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
		public static Quadruple operator +(Quadruple left, Quadruple right)
		{
			// Propogate NaNs
			if (IsNaN(left))
				return left;
			if (IsNaN(right))
				return right;

			if (IsZero(left))
				return right;
			if (IsZero(right))
				return left;

			// ∞ + -∞ and -∞ + ∞ are indeterminate
			// Only one infinity check is required since if left is infinity and equals -right, right = -infinity
			if ((left._upper ^ right._upper) >> 63 == 1 && left._lower == right._lower && !IsFinite(left))
				return NaN;

			// Not a trivial case
			return AddSlow(left, right);

			static Quadruple AddSlow(Quadruple left, Quadruple right)
			{
				ulong lowerL = left._lower;
				ulong upperL = left._upper;
				ulong lowerR = right._lower;
				ulong upperR = right._upper;

				// Calculate Biased Exponents
				int expL = (int)((upperL >>> ExponentStartBitUpper) & ShiftedBiasedExponentMask);
				int expR = (int)((upperR >>> ExponentStartBitUpper) & ShiftedBiasedExponentMask);

				// Mask upper bits and add normal bit if necessary

				upperL &= UpperFractionMask;
				upperR &= UpperFractionMask;

				if (expL > 0)
					upperL |= UpperNormalBit;

				if (expR > 0)
					upperR |= UpperNormalBit;

				UInt128 fracL = new UInt128(upperL, lowerL);
				UInt128 fracR = new UInt128(upperR, lowerR);

				// Calculate true biased exponents of numbers and the exponent for calculation
				if (expL == 0)
					expL = 1;
				if (expR == 0)
					expR = 1;

				int exp = Math.Max(expL, expR); // Calculations use the max exponent to preserve precision
				int expDiff = expL - expR;

				// Align fractions
				if (expDiff >= 0)
					fracR >>>= expDiff;
				else
					fracL >>= -expDiff;

				// Compute signs
				// signL and signR will be 1 if negative and 0 if positive
				ulong signL = left._upper >>> 63;
				ulong signR = right._upper >>> 63;

				if ((signL ^ signR) > 0)
				{
					fracR = -fracR;
				}

				// Add numbers
				UInt128 fraction = fracL + fracR;

				if (fraction == default)	// Fraction is zero (quick exit)
				{
					return default;
				}

				// Recompute sign
				ulong sign = signL;
				if ((long)fraction.Upper() < 0)
				{
					sign ^= 1;
					fraction = -fraction;
				}

				ulong signBit = sign << SignBit;

				// Renormalize Number
				expDiff = 8 * Size - FractionBits - (int)UInt128.LeadingZeroCount(fraction);
				int tempExp = exp + expDiff;

				if (tempExp >= SpecialExp)  // Handle infinities
				{
					return new Quadruple(signBit, 0);
				}
				else if (tempExp <= 0)      // Handle subnormals
				{
					exp = 0;
				}
				else                        // Handle normal numbers
				{
					exp = tempExp;

					_ = (expDiff >= 0) ? 
						fraction >>>= expDiff : 
						fraction <<= -expDiff;
				}

				ulong lower = fraction.Lower();
				ulong upper = fraction.Upper();

				return new Quadruple(signBit | ((ulong)exp << ExponentStartBitUpper) | (upper & UpperFractionMask), lower);
			}
		}

		//
		// IAdditiveIdentity
		//

		/// <inheritdoc cref="IAdditiveIdentity{Quadruple, Quadruple}.AdditiveIdentity" />
		static Quadruple IAdditiveIdentity<Quadruple, Quadruple>.AdditiveIdentity => default;

		//
		// IBinaryNumber
		//

		/// <inheritdoc cref="IBinaryNumber{TSelf}.AllBitsSet" />
		static Quadruple IBinaryNumber<Quadruple>.AllBitsSet => new Quadruple(ulong.MaxValue, ulong.MaxValue);

		/// <inheritdoc cref="IBinaryNumber{TSelf}.IsPow2(TSelf)"/>
		public static bool IsPow2(Quadruple value)
		{
			uint exp = (uint)(value._upper >>> 48) & 0xFFFF;
			ulong upper = value._upper & UpperFractionMask;

			return exp > 0 && exp < SpecialExp && (upper | value._lower) == 0;
		}

		/// <inheritdoc cref="IBinaryNumber{TSelf}.Log2(TSelf)" />
		public static Quadruple Log2(Quadruple value)
		{
			throw new NotImplementedException();
		}

		//
		// IBitwiseOperators
		//

		/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_BitwiseAnd(TSelf, TOther)"/>
		static Quadruple IBitwiseOperators<Quadruple, Quadruple, Quadruple>.operator &(Quadruple left, Quadruple right) => new Quadruple(left._upper & right._upper, left._lower & right._lower);

		/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_ExclusiveOr(TSelf, TOther)"/>
		static Quadruple IBitwiseOperators<Quadruple, Quadruple, Quadruple>.operator ^(Quadruple left, Quadruple right) => new Quadruple(left._upper ^ right._upper, left._lower ^ right._lower);

		/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_BitwiseOr(TSelf, TOther)"/>
		static Quadruple IBitwiseOperators<Quadruple, Quadruple, Quadruple>.operator |(Quadruple left, Quadruple right) => new Quadruple(left._upper | right._upper, left._lower | right._lower);

		/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_OnesComplement(TSelf)"/>
		static Quadruple IBitwiseOperators<Quadruple, Quadruple, Quadruple>.operator ~(Quadruple value) => new Quadruple(~value._upper, ~value._lower);

		//
		// IDecrementOperators
		//

		/// <inheritdoc cref="IDecrementOperators{TSelf}.op_Decrement(TSelf)"/>
		public static Quadruple operator --(Quadruple value)
		{
			// TODO: Replace with optimized implementation
			return value - One;
		}

		//
		// IDivisionOperators
		//

		/// <inheritdoc cref="IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"/>
		public static Quadruple operator /(Quadruple left, Quadruple right)
		{
			if (IsNaN(left))
				return left;
			if (IsNaN(right))
				return right;

			if (IsZero(right))
			{
				// 0 / 0
				if (IsZero(left))
					return NaN;

				// Anything else divided by zero is either positive or negative infinity
				return (left._upper ^ right._upper) >> 63 == 0 ? PositiveInfinity : NegativeInfinity;
			}

			// Do not need to check NaNs because check was already performed
			// !IsFinite is faster than IsInfinity
			if (!IsFinite(left) && !IsFinite(right))
				return NaN;

			// 0 / right = 0
			if (IsZero(left))
				return default;

			// left / 1 = left
			if (BitEquals(right, One))
				return left;

			return DivideSlow(left, right);

			static Quadruple DivideSlow(Quadruple left, Quadruple right) => throw new NotImplementedException();
		}

		//
		// IExponentialFunctions
		//

		/// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp(TSelf)"/>
		public static Quadruple Exp(Quadruple value) => throw new NotImplementedException();

		/// <inheritdoc cref="IExponentialFunctions{TSelf}.ExpM1(TSelf)"/>
		public static Quadruple ExpM1(Quadruple value) => throw new NotImplementedException();

		/// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp10(TSelf)"/>
		public static Quadruple Exp10(Quadruple value) => throw new NotImplementedException();

		/// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp10M1(TSelf)"/>
		public static Quadruple Exp10M1(Quadruple value) => throw new NotImplementedException();

		/// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp2(TSelf)"/>
		public static Quadruple Exp2(Quadruple value) => throw new NotImplementedException();

		/// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp2M1(TSelf)"/>
		public static Quadruple Exp2M1(Quadruple value) => throw new NotImplementedException();

		//
		// IFloatingPoint
		//

		/// <inheritdoc cref="IFloatingPoint{TSelf}.GetExponentByteCount()"/>
		int IFloatingPoint<Quadruple>.GetExponentByteCount() => sizeof(short);

		/// <inheritdoc cref="IFloatingPoint{TSelf}.GetExponentShortestBitLength()"/>
		int IFloatingPoint<Quadruple>.GetExponentShortestBitLength()
		{
			short exponent = (short)((int)((_upper >> ExponentStartBitUpper) & ShiftedBiasedExponentMask) - Bias);

			if (exponent >= 0)
			{
				return (sizeof(short) * 8) - short.LeadingZeroCount(exponent);
			}
			else
			{
				return (sizeof(short) * 8) + 1 - short.LeadingZeroCount((short)~exponent);
			}
		}

		/// <inheritdoc cref="IFloatingPoint{TSelf}.GetSignificandBitLength()"/>
		int IFloatingPoint<Quadruple>.GetSignificandBitLength() => FractionBits;

		/// <inheritdoc cref="IFloatingPoint{TSelf}.GetSignificandByteCount()"/>
		int IFloatingPoint<Quadruple>.GetSignificandByteCount() => sizeof(Quadruple);

		/// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf, int, MidpointRounding)" />
		public static Quadruple Round(Quadruple value, int digits, MidpointRounding mode) => throw new NotImplementedException();

		/// <inheritdoc cref="IFloatingPoint{TSelf}.TryWriteExponentBigEndian(Span{byte}, out int)"/>
		bool IFloatingPoint<Quadruple>.TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

		/// <inheritdoc cref="IFloatingPoint{TSelf}.TryWriteExponentLittleEndian(Span{byte}, out int)"/>
		bool IFloatingPoint<Quadruple>.TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

		/// <inheritdoc cref="IFloatingPoint{TSelf}.TryWriteSignificandBigEndian(Span{byte}, out int)"/>
		bool IFloatingPoint<Quadruple>.TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

		/// <inheritdoc cref="IFloatingPoint{TSelf}.TryWriteSignificandLittleEndian(Span{byte}, out int)"/>
		bool IFloatingPoint<Quadruple>.TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

		//
		// IFloatingPointIeee754
		//

		/// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Atan2(TSelf, TSelf)"/>
		public static Quadruple Atan2(Quadruple y, Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Atan2Pi(TSelf, TSelf)"/>
		public static Quadruple Atan2Pi(Quadruple y, Quadruple x) => Atan2(y, x) / Pi;

		/// <inheritdoc cref="IFloatingPointIeee754{TSelf}.BitDecrement(TSelf)"/>
		public static Quadruple BitDecrement(Quadruple value) => throw new NotImplementedException();

		/// <inheritdoc cref="IFloatingPointIeee754{TSelf}.BitIncrement(TSelf)"/>
		public static Quadruple BitIncrement(Quadruple value) => throw new NotImplementedException();

		/// <inheritdoc cref="IFloatingPointIeee754{TSelf}.FusedMultiplyAdd(TSelf, TSelf, TSelf)"/>
		public static Quadruple FusedMultiplyAdd(Quadruple left, Quadruple right, Quadruple addend) => left * right + addend;

		/// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Ieee754Remainder(TSelf, TSelf)"/>
		public static Quadruple Ieee754Remainder(Quadruple left, Quadruple right) => throw new NotImplementedException();

		/// <inheritdoc cref="IFloatingPointIeee754{TSelf}.ILogB(TSelf)"/>
		public static int ILogB(Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="IFloatingPointIeee754{TSelf}.ScaleB(TSelf, int)"/>
		public static Quadruple ScaleB(Quadruple x, int n) => throw new NotImplementedException();

		//
		// IHyperbolicFunctions
		//

		/// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Acosh(TSelf)"/>
		public static Quadruple Acosh(Quadruple x) => Log(x + Sqrt(x * x - 1));

		/// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Asinh(TSelf)"/>
		public static Quadruple Asinh(Quadruple x) => Log(x + Sqrt(x * x + 1));

		/// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Atanh(TSelf)"/>
		public static Quadruple Atanh(Quadruple x) => ScaleB(Log((1 + x) / (1 - x)), -1);

		/// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Cosh(TSelf)"/>
		public static Quadruple Cosh(Quadruple x) => ScaleB(Exp(x) + Exp(-x), -1);

		/// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Sinh(TSelf)"/>
		public static Quadruple Sinh(Quadruple x) => ScaleB(Exp(x) - Exp(-x), -1);

		/// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Tanh(TSelf)"/>
		public static Quadruple Tanh(Quadruple x)
		{
			Quadruple exp = Exp(x);
			Quadruple exm = Exp(-x);

			return (exp - exm) / (exp + exm);
		}

		//
		// IIncrementOperators
		//

		/// <inheritdoc cref="IIncrementOperators{TSelf}.op_Increment(TSelf)"/>
		public static Quadruple operator ++(Quadruple value)
		{
			// TODO: replace with optimized version
			return value + One;
		}

		//
		// ILogarithmicFunctions
		//

		/// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log(TSelf)"/>
		public static Quadruple Log(Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log(TSelf, TSelf)"/>
		public static Quadruple Log(Quadruple x, Quadruple n) => throw new NotImplementedException();

		/// <inheritdoc cref="ILogarithmicFunctions{TSelf}.LogP1(TSelf)"/>
		public static Quadruple LogP1(Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log2P1(TSelf)"/>
		public static Quadruple Log2P1(Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log10(TSelf)"/>
		public static Quadruple Log10(Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log10P1(TSelf)"/>
		public static Quadruple Log10P1(Quadruple x) => throw new NotImplementedException();

		//
		// IModulusOperators
		//

		/// <inheritdoc cref="IModulusOperators{TSelf, TOther, TResult}.op_Modulus(TSelf, TOther)"/>
		public static Quadruple operator %(Quadruple left, Quadruple right) => throw new NotImplementedException();

		//
		// IMultiplicativeIdentity
		//

		/// <inheritdoc cref="IMultiplicativeIdentity{TSelf, TResult}.MultiplicativeIdentity"/>
		static Quadruple IMultiplicativeIdentity<Quadruple, Quadruple>.MultiplicativeIdentity => One;

		//
		// IMultiplyOperators
		//

		/// <inheritdoc cref="IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)"/>
		public static Quadruple operator *(Quadruple left, Quadruple right) => throw new NotImplementedException();

		//
		// INumberBase
		//

		/// <inheritdoc cref="INumberBase{TSelf}.Radix"/>
		static int INumberBase<Quadruple>.Radix => 2;

		/// <inheritdoc cref="INumberBase{TSelf}.Abs(TSelf)"/>
		public static Quadruple Abs(Quadruple value) => new Quadruple(value._upper & ~SignMaskUpper, value._lower);

		/// <inheritdoc cref="INumberBase{TSelf}.IsCanonical(TSelf)"/>
		static bool INumberBase<Quadruple>.IsCanonical(Quadruple value) => true;

		/// <inheritdoc cref="INumberBase{TSelf}.IsComplexNumber(TSelf)"/>
		static bool INumberBase<Quadruple>.IsComplexNumber(Quadruple value) => false;

		/// <inheritdoc cref="INumberBase{TSelf}.IsEvenInteger(TSelf)"/>
		public static bool IsEvenInteger(Quadruple value) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.IsImaginaryNumber(TSelf)"/>
		static bool INumberBase<Quadruple>.IsImaginaryNumber(Quadruple value) => false;

		/// <inheritdoc cref="INumberBase{TSelf}.IsInteger(TSelf)"/>
		public static bool IsInteger(Quadruple value) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.IsOddInteger(TSelf)"/>
		public static bool IsOddInteger(Quadruple value) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.IsPositive(TSelf)"/>
		public static bool IsPositive(Quadruple value) => !IsNaN(value) && (value._upper >> SignBit) == 0;

		/// <inheritdoc cref="INumberBase{TSelf}.IsRealNumber(TSelf)"/>
		static bool INumberBase<Quadruple>.IsRealNumber(Quadruple value) => !IsNaN(value);

		/// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitude(TSelf, TSelf)"/>
		public static Quadruple MaxMagnitude(Quadruple x, Quadruple y) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitudeNumber(TSelf, TSelf)"/>
		public static Quadruple MaxMagnitudeNumber(Quadruple x, Quadruple y) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.MinMagnitude(TSelf, TSelf)"/>
		public static Quadruple MinMagnitude(Quadruple x, Quadruple y) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.MinMagnitudeNumber(TSelf, TSelf)"/>
		public static Quadruple MinMagnitudeNumber(Quadruple x, Quadruple y) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromChecked{TOther}(TOther, out TSelf)"/>
		static bool INumberBase<Quadruple>.TryConvertFromChecked<TOther>(TOther value, out Quadruple result) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromSaturating{TOther}(TOther, out TSelf)"/>
		static bool INumberBase<Quadruple>.TryConvertFromSaturating<TOther>(TOther value, out Quadruple result) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromTruncating{TOther}(TOther, out TSelf)"/>
		static bool INumberBase<Quadruple>.TryConvertFromTruncating<TOther>(TOther value, out Quadruple result) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.TryConvertToChecked{TOther}(TSelf, out TOther)"/>
		static bool INumberBase<Quadruple>.TryConvertToChecked<TOther>(Quadruple value, out TOther result) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.TryConvertToSaturating{TOther}(TSelf, out TOther)"/>
		static bool INumberBase<Quadruple>.TryConvertToSaturating<TOther>(Quadruple value, out TOther result) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.TryConvertToTruncating{TOther}(TSelf, out TOther)"/>
		static bool INumberBase<Quadruple>.TryConvertToTruncating<TOther>(Quadruple value, out TOther result) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.Parse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?)"/>
		public static Quadruple Parse(ReadOnlySpan<char> s, NumberStyles styles, IFormatProvider? provider) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.Parse(string, NumberStyles, IFormatProvider?)"/>
		public static Quadruple Parse(string s, NumberStyles styles, IFormatProvider? provider) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?, out TSelf)"/>
		public static bool TryParse(ReadOnlySpan<char> s, NumberStyles styles, IFormatProvider? provider, out Quadruple value) => throw new NotImplementedException();

		/// <inheritdoc cref="INumberBase{TSelf}.TryParse(string?, NumberStyles, IFormatProvider?, out TSelf)"/>
		public static bool TryParse(string s, NumberStyles styles, IFormatProvider? provider, out Quadruple value) => throw new NotImplementedException();

		//
		// IParsable
		//

		/// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider?)"/>
		public static Quadruple Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();

		/// <inheritdoc cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)"/>
		public static bool TryParse(string? s, IFormatProvider? provider, out Quadruple value) => throw new NotImplementedException();

		//
		// IPowerFunctions
		//

		/// <inheritdoc cref="IPowerFunctions{TSelf}.Pow(TSelf, TSelf)"/>
		public static Quadruple Pow(Quadruple x, Quadruple y) => throw new NotImplementedException();

		//
		// IRootFuncitons
		//

		/// <inheritdoc cref="IRootFunctions{TSelf}.Cbrt(TSelf)"/>
		public static Quadruple Cbrt(Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="IRootFunctions{TSelf}.Hypot(TSelf, TSelf)"/>
		public static Quadruple Hypot(Quadruple x, Quadruple y) => throw new NotImplementedException();

		/// <inheritdoc cref="IRootFunctions{TSelf}.RootN(TSelf, int)"/>
		public static Quadruple RootN(Quadruple x, int n) => throw new NotImplementedException();

		/// <inheritdoc cref="IRootFunctions{TSelf}.Sqrt(TSelf)"/>
		public static Quadruple Sqrt(Quadruple x) => throw new NotImplementedException();

		//
		// ISparnFormattable
		//

		/// <inheritdoc cref="ISpanFormattable.TryFormat(Span{char}, out int, ReadOnlySpan{char}, IFormatProvider?)"/>
		public bool TryFormat(Span<char> destination, out int written, ReadOnlySpan<char> format, IFormatProvider? provider) => throw new NotImplementedException();

		//
		// ISpanParsable
		//

		/// <inheritdoc cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)"/>
		public static Quadruple Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotImplementedException();

		/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)"/>
		public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Quadruple value) => throw new NotImplementedException();

		//
		// ISubtractionOperators
		//

		/// <inheritdoc cref="ISubtractionOperators{TSelf, TOther, TResult}.op_Subtraction(TSelf, TOther)"/>
		public static Quadruple operator -(Quadruple left, Quadruple right)
		{
			// Propogate NaNs
			if (IsNaN(left))
				return left;
			if (IsNaN(right))
				return right;

			if (IsZero(left))
				return -right;
			if (IsZero(right))
				return left;

			// ∞ + -∞ and -∞ + ∞ are indeterminate
			// Only one infinity check is required since if left is infinity and equals -right, right = -infinity
			if ((left._upper ^ right._upper) >> SignBit == 0 && left._lower == right._lower && !IsFinite(left))
				return NaN;

			// Not a trivial case
			return SubtractSlow(left, right);

			static Quadruple SubtractSlow(Quadruple left, Quadruple right)
			{
				ulong lowerL = left._lower;
				ulong upperL = left._upper;
				ulong lowerR = right._lower;
				ulong upperR = right._upper;

				// Calculate Biased Exponents
				int expL = (int)((upperL >>> ExponentStartBitUpper) & ShiftedBiasedExponentMask);
				int expR = (int)((upperR >>> ExponentStartBitUpper) & ShiftedBiasedExponentMask);

				// Mask upper bits and add normal bit if necessary

				upperL &= UpperFractionMask;
				upperR &= UpperFractionMask;

				if (expL > 0)
					upperL |= UpperNormalBit;

				if (expR > 0)
					upperR |= UpperNormalBit;

				UInt128 fracL = new UInt128(upperL, lowerL);
				UInt128 fracR = new UInt128(upperR, lowerR);

				// Calculate true biased exponents of numbers and the exponent for calculation
				if (expL == 0)
					expL = 1;
				if (expR == 0)
					expR = 1;

				int exp = Math.Max(expL, expR); // Calculations use the max exponent to preserve precision
				int expDiff = expL - expR;

				// Align fractions
				if (expDiff >= 0)
					fracR >>>= expDiff;
				else
					fracL >>= -expDiff;

				// Compute signs
				// signL and signR will be 1 if negative and 0 if positive
				ulong signL = left._upper >>> 63;
				ulong signR = right._upper >>> 63;

				if ((signL ^ signR) > 0)
				{
					fracR = -fracR;
				}

				// Add numbers
				UInt128 fraction = fracL - fracR;

				if (fraction == default)    // Fraction is zero (quick exit)
				{
					return default;
				}

				// Recompute sign
				ulong sign = signL;
				if ((long)fraction.Upper() < 0)
				{
					sign ^= 1;
					fraction = -fraction;
				}

				ulong signBit = sign << SignBit;

				// Renormalize Number
				expDiff = 8 * Size - FractionBits - (int)UInt128.LeadingZeroCount(fraction);
				int tempExp = exp + expDiff;

				if (tempExp >= SpecialExp)  // Handle infinities
				{
					return new Quadruple(signBit, 0);
				}
				else if (tempExp <= 0)      // Handle subnormals
				{
					exp = 0;
				}
				else                        // Handle normal numbers
				{
					exp = tempExp;

					_ = (expDiff >= 0) ?
						fraction >>>= expDiff :
						fraction <<= -expDiff;
				}

				ulong lower = fraction.Lower();
				ulong upper = fraction.Upper();

				return new Quadruple(signBit | ((ulong)exp << ExponentStartBitUpper) | (upper & UpperFractionMask), lower);
			}
		}

		//
		// ITrigonometricFunctions
		//

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Acos(TSelf)"/>
		public static Quadruple Acos(Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.AcosPi(TSelf)"/>
		public static Quadruple AcosPi(Quadruple x) => Acos(x) / Pi;

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Asin(TSelf)"/>
		public static Quadruple Asin(Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.AsinPi(TSelf)"/>
		public static Quadruple AsinPi(Quadruple x) => Asin(x) / Pi;

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Atan(TSelf)"/>
		public static Quadruple Atan(Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.AtanPi(TSelf)"/>
		public static Quadruple AtanPi(Quadruple x) => Atan(x) / Pi;

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Cos(TSelf)"/>
		public static Quadruple Cos(Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.CosPi(TSelf)"/>
		public static Quadruple CosPi(Quadruple x) => Cos(x * Pi);

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Sin(TSelf)"/>
		public static Quadruple Sin(Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.SinPi(TSelf)"/>
		public static Quadruple SinPi(Quadruple x) => Sin(x * Pi);

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.SinCos(TSelf)"/>
		public static (Quadruple Sin, Quadruple Cos) SinCos(Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.SinCosPi(TSelf)"/>
		public static (Quadruple SinPi, Quadruple CosPi) SinCosPi(Quadruple x) => SinCos(x * Pi);

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Tan(TSelf)"/>
		public static Quadruple Tan(Quadruple x) => throw new NotImplementedException();

		/// <inheritdoc cref="ITrigonometricFunctions{TSelf}.TanPi(TSelf)"/>
		public static Quadruple TanPi(Quadruple x) => Tan(x * Pi);

		//
		// IUnaryNegationOperator
		//

		public static Quadruple operator -(Quadruple value)
		{
			return new Quadruple(value._upper ^ (1UL << SignBit), value._lower);
		}

		//
		// IUnaryPlusOperators
		//

		public static Quadruple operator +(Quadruple value) => value;
	}
}