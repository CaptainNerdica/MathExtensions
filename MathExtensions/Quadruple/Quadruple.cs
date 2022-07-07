using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Versioning;
using System.Text;

namespace MathExtensions
{
	/// <summary>
	/// Represents a IEEE 754-2008 binary128 quadruple-precision floating-point number.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe readonly partial struct Quadruple
		: 
		  IComparable<Quadruple>,
		  IEquatable<Quadruple>,
		  IMinMaxValue<Quadruple>
	{
		internal const int Size = 16;

		internal const int Bias = 0x3FFF;
		internal const int SpecialExp = 0x4000;
		internal const ulong SignMaskUpper = 0x8000_0000_0000_0000;

		public readonly ulong _lower;   // bits 0...63
		public readonly ulong _upper;   // bits 64...127

		/// <summary>Represents the largest possible value of a <see cref="Quadruple"/>.</summary>
		/// <remarks><see cref="MaxValue"/> is approximately 1.1897314953572317650857593266280070162 × 10^4932.</remarks>
		public static readonly Quadruple MaxValue = new Quadruple(0x7FFE_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF);
		/// <summary>Represents the smallest possible value of a <see cref="Quadruple"/>.</summary>
		/// <remarks><see cref="MinValue"/> is approximately -1.1897314953572317650857593266280070162 × 10^4932.</remarks>
		public static readonly Quadruple MinValue = new Quadruple(0xFFFE_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF);

		/// <summary>Represents the value zero.</summary>
		public static readonly Quadruple Zero = new Quadruple(0x0000_0000_0000_0000, 0x0000_0000_0000_0000);
		/// <summary>Represents the value negative zero.</summary>
		public static readonly Quadruple NegativeZero = new Quadruple(0x8000_0000_0000_0000, 0x0000_0000_0000_0000);

		/// <summary>Represents the value one.</summary>
		public static readonly Quadruple One = new Quadruple(0x3FFF_0000_0000_0000, 0x0000_0000_0000_0000);
		/// <summary>Represents the value negative one.</summary>
		public static readonly Quadruple NegativeOne = new Quadruple(0xBFFF_0000_0000_0000, 0x0000_0000_0000_0000);

		/// <summary>Represents positive infintiy.</summary>
		public static readonly Quadruple PositiveInfinity = new Quadruple(0x7FFF_0000_0000_0000, 0x0000_0000_0000_0000);
		/// <summary>Represents negative infinity.</summary>
		public static readonly Quadruple NegativeInfinity = new Quadruple(0xFFFF_0000_0000_0000, 0x0000_0000_0000_0000);

		/// <summary>Represents the smallest positive <see cref="Quadruple"/> value that is greater than zero.</summary>
		/// <remarks>Epislon is approximately 6.4751751194380251109244389582276465525 × 10^-4966.</remarks>
		public static readonly Quadruple Epsilon = new Quadruple(0x0000_0000_0000_0000, 0x0000_0000_0000_0001);
		/// <summary>Represents a value that is not a number (NaN).</summary>
		public static readonly Quadruple NaN = new Quadruple(0xFFFF_8000_0000_0000, 0x0000_0000_0000_0000);

		/// <summary>Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π.</summary>
		/// <remarks>Pi is approximately 3.14159265358979323846264338327950280.</remarks>
		public static readonly Quadruple Pi = new Quadruple(0x4000_921F_B544_42D1, 0x8469_898C_C517_01B8);
		/// <summary>Represents the number of radians in one turn, specified by the constant, τ.</summary>
		/// <remarks>Tau is approximately 6.28318530717958647692528676655900559.</remarks>
		public static readonly Quadruple Tau = new Quadruple(0x4001_921F_B544_42D1, 0x8469_898C_C517_01B8);
		/// <summary>Represents the natural logarithmic base, specified by the constant, e.</summary>
		/// <remarks>E is apporximately 2.71828182836969448633845487297936844.</remarks>
		public static readonly Quadruple E = new Quadruple(0x4000_5BF0_A8B1_1457, 0x9535_5FB8_AC40_4E7A);

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
		internal Quadruple(nuint sign, int exponent, ulong upperFrac, ulong lowerFrac)
		{
			const ulong UPPER_FRAC_MASK = 0x0000_FFFF_FFFF_FFFF;
			const ulong EXP_MASK = 0x7FFF;
			ulong biasedExp = (ulong)(exponent + Bias) & EXP_MASK;

			_lower = lowerFrac;
			_upper = ((sign & 1UL) << 63) | (biasedExp << 48) | (upperFrac & UPPER_FRAC_MASK);
		}

		/// <summary>
		/// Determines whether the specified value is finite (zero, subnormal, or normal).
		/// </summary>
		/// <param name="q">A quadruple-precision floating-point number.</param>
		/// <returns><see langword="true"/> if the value is finite (zero, subnormal, or normal); <see langword="false" /> otherwise.</returns>
		public static bool IsFinite(Quadruple q) => (q._upper << 1) < 0xFFFE_0000_0000_0000;

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
		/// <returns><see langword="true" /> <paramref name="q"/> evaluates to <see cref="NaN" />; otherwise, <see langword="false"/></returns>
		public static bool IsNaN(Quadruple q) => (q._upper & 0x7FFF_0000_0000_0000) == 0x7FFF_0000_0000_0000 && (((q._upper << 16) | q._lower) != 0); // check exponent is 0x7FFF and significand is not zero.

		/// <summary>
		/// Determines whether the specified value is negative.
		/// </summary>
		/// <param name="q">A quadruple-precision floating-point number.</param>
		/// <returns><see langword="true" /> if the value is negative; <see langword="false" /> otherwise.</returns>
		public static bool IsNegative(Quadruple q) => ((long)q._upper) < 0;

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

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		private static bool AreZero(Quadruple left, Quadruple right)
		{
			return ((left._upper | right._upper) << 1) == 0 && (left._lower | right._lower) == 0;
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is an instance of <see cref="Quadruple"/> and equals the value of this instance; otherwise, <see langword="false"/></returns>
		public override bool Equals([NotNullWhen(true)] object? obj) => obj is Quadruple q && Equals(q);

		/// <inheritdoc cref="ValueType.GetHashCode"/>
		public override int GetHashCode()
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

		/// <summary>
		/// Returns a value indicating whether this instance and a speicied <see cref="Quadruple"/> object represent the same value.
		/// </summary>
		/// <param name="value">A <see cref="Quadruple"/> object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="value"/> is equal to this instance; otherwise <see langword="false"/>.</returns>
		public bool Equals(Quadruple value) => this == value || (IsNaN(value) && IsNaN(this));

		public override string ToString() => $"{_upper:X16}{_lower:X16}";

		/// <summary>
		/// Compares this object to another object, returning an integer that indicates the relationship.
		/// </summary>
		/// <returns>A value less than zero if this is less than <paramref name="obj"/>, zero if this is equal to <paramref name="obj"/>, or a value greater than zero if this is greater than <paramref name="obj"/>.</returns>
		/// <exception cref="ArgumentException">Thrown when <paramref name="obj"/> is not of type <see cref="Quadruple"/>.</exception>
		public int CompareTo(object? obj)
		{
			if (obj is not Quadruple q)
				return (obj is null) ? 1 : throw new ArgumentException($"Object must be of type {nameof(Quadruple)}.");

			return CompareTo(q);
		}

		/// <summary>
		/// Compares this object to another object, returning an integer that indicates the relationship.
		/// </summary>
		/// <param name="other">The value to compare to.</param>
		/// <returns>A value less than zero if this is less than <paramref name="other"/>, zero if this is equal to <paramref name="other"/>, or a value greater than zero if this is greater than <paramref name="other"/>.</returns>
		public int CompareTo(Quadruple other)
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
#pragma warning disable IDE0075
			if (IsNaN(left) || IsNaN(right))	// Numbers are unequal if either are NaN
				return false;

			return ((left._upper == right._upper && left._lower == right._lower) || AreZero(left, right)) ? true : false; // Ternary produces slightly more efficient assembly.
#pragma warning restore IDE0075
		}

		/// <summary>
		/// Returns a value that indicates whether two specified <see cref="Quadruple"/> values are not equal.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns><see langword="true" /> if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(Quadruple left, Quadruple right) => !(left == right);

		/// <inheritdoc cref="IUnaryNegationOperators{TSelf, TResult}.op_UnaryNegation(TSelf)"/>
		public static Quadruple operator -(Quadruple value) => new Quadruple(value._upper ^ SignMaskUpper, value._lower);
		/// <inheritdoc cref="IUnaryNegationOperators{TSelf, TResult}.op_CheckedUnaryNegation(TSelf)"/>
		public static Quadruple operator checked -(Quadruple value) => unchecked(-value);

		public static implicit operator Quadruple(Half value)
		{
			const int BIT_COUNT = 16;
			const int EXPONENT_START = 10;
			const int EXPONENT_BIAS = 15;
			const int SPECIAL_EXP = EXPONENT_BIAS + 1;
			const int SIGN_BIT = 15;
			const uint FRACTION_MASK = 0x03FF;
			const uint SIGN_MASK = 0x8000;

			uint bits = BitConverter.HalfToUInt16Bits(value);
			uint sign = bits >> SIGN_BIT;
			int exp = (int)((bits & ~SIGN_MASK) >> EXPONENT_START) - EXPONENT_BIAS;
			uint frac = bits & FRACTION_MASK;

			if (exp == -EXPONENT_BIAS)
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
				exp = SpecialExp;

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

			if (exp == -EXPONENT_BIAS)
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
				exp = SpecialExp;

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
			ulong sign = bits >> SIGN_BIT;
			int exp = (int)((bits & ~SIGN_MASK) >> EXPONENT_START) - EXPONENT_BIAS;
			ulong frac = bits & FRACTION_MASK;

			if (exp == -EXPONENT_BIAS)
			{
				int lzc = BitOperations.LeadingZeroCount(frac);

				if (lzc == BIT_COUNT)
					return new Quadruple((nuint)sign, -Bias, 0, 0);

				int shift = lzc - (BIT_COUNT - EXPONENT_START);

				exp -= shift;
				frac <<= shift + 1;
				frac &= FRACTION_MASK;
			}

			if (exp == SPECIAL_EXP)
				exp = SpecialExp;

			return new Quadruple((nuint)sign, exp, frac >> (EXPONENT_START - 48), frac << (BIT_COUNT - (EXPONENT_START - 48)));
		}

		//
		// IMinMaxValue
		//

		/// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue" />
		static Quadruple IMinMaxValue<Quadruple>.MaxValue => MaxValue;

		/// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue" />
		static Quadruple IMinMaxValue<Quadruple>.MinValue => MinValue;
	}
}