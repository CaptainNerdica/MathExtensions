using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Linq;

namespace MathExtensions
{
	// Implementation heavily inspired by base implementation of System.Numerics.Complex:
	// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Runtime.Numerics/src/System/Numerics/Complex.cs

	/// <inheritdoc cref="Complex"/>
	/// <typeparam name="T">The backing type of the real and imaginary parts of the complex number.</typeparam>
	[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "<Pending>")]
	public readonly struct Complex<T> :
		IEquatable<Complex<T>>,
		IFormattable,
		INumberBase<Complex<T>>,
		ISignedNumber<Complex<T>>
	where T :
		IComparable<T>,
		IEquatable<T>,
		IFormattable,
		INumberBase<T>,
		ISignedNumber<T>
	{
		private const NumberStyles DefaultNumberStyle = NumberStyles.Float | NumberStyles.AllowThousands;

		private const NumberStyles InvalidNumberStyles = ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite
														 | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign
														 | NumberStyles.AllowParentheses | NumberStyles.AllowDecimalPoint
														 | NumberStyles.AllowThousands | NumberStyles.AllowExponent
														 | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowHexSpecifier);

		private static readonly bool SupportsNaN = TypeInit.ImplementsInterface(typeof(IFloatingPointIeee754<>));
		private static readonly bool SupportsInfinity = TypeInit.ImplementsInterface(typeof(IFloatingPointIeee754<>));

		private static readonly T _tNaN = SupportsNaN ? T.Zero / T.Zero : T.Zero;
		private static readonly T _tInfin = SupportsInfinity ? T.One / T.Zero : T.Zero;
		private static readonly T _two = T.One + T.One;

		public static Complex<T> Zero => new Complex<T>(T.Zero, T.Zero);
		public static Complex<T> One => new Complex<T>(T.One, T.Zero);
		public static Complex<T> ImaginaryOne => new Complex<T>(T.Zero, T.One);
		public static Complex<T> NaN => SupportsNaN ? new Complex<T>(_tNaN, _tNaN) : throw new InvalidOperationException("NaN is not supported for this type.");
		public static Complex<T> Infinity => SupportsInfinity ? new Complex<T>(_tInfin, _tInfin) : throw new InvalidOperationException("Infinity is not supported for this type.");

		private readonly T _real;
		private readonly T _imag;

		public T Real => _real;
		public T Imaginary => _imag;

		public T Magnitude => Abs(this);

		public Complex(T real, T imaginary)
		{
			_real = real;
			_imag = imaginary;
		}

		private static readonly bool _isSimpleSqrt;
		private static Func<T, T> SqrtFunc { get; } = TypeInit.CreateSqrtFunc(out _isSimpleSqrt);

		/// <inheritdoc cref="ValueType.Equals(object?)" />
		public override bool Equals([NotNullWhen(true)] object? obj) => obj is Complex<T> o && Equals(o);
		/// <inheritdoc cref="IEquatable{T}.Equals(T)" />
		public bool Equals(Complex<T> value) => _real.Equals(value._real) && _imag.Equals(value._imag);

		/// <inheritdoc cref="ValueType.GetHashCode" />
		public override int GetHashCode()
		{
			int n1 = 99999997;
			int realHash = _real.GetHashCode() % n1;
			int imaginaryHash = _imag.GetHashCode();
			int finalHash = realHash ^ imaginaryHash;
			return finalHash;
		}

		/// <inheritdoc cref="ValueType.ToString" />
		public override string ToString() => $"<{_real}; {_imag}>";
		public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format) => ToString(format, null);
		public string ToString(IFormatProvider? provider) => ToString(null, provider);
		public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? provider) => string.Format(provider, "<{0}; {1}>", _real.ToString(format, provider), _imag.ToString(format, provider));

		public static bool IsFinite(Complex<T> value) => T.IsFinite(value._real) && T.IsFinite(value._imag);
		public static bool IsInfinity(Complex<T> value) => T.IsInfinity(value._real) || T.IsInfinity(value._imag);
		public static bool IsNaN(Complex<T> value) => !IsInfinity(value) && !IsFinite(value);

		public static bool operator ==(Complex<T> left, Complex<T> right) => left._real == right._real && left._imag == right._imag;
		public static bool operator !=(Complex<T> left, Complex<T> right) => left._real != right._real || left._imag != right._imag;

		public static Complex<T> operator +(Complex<T> value) => value;
		public static Complex<T> operator -(Complex<T> value) => new Complex<T>(-value._real, -value._imag);
		public static Complex<T> operator checked -(Complex<T> value) => checked(new Complex<T>(-value._real, -value._imag));

		/// <inheritdoc cref="IIncrementOperators{TSelf}.op_Increment(TSelf)" />
		public static Complex<T> operator ++(Complex<T> value) => new Complex<T>(value._real + T.One, value._imag);
		/// <inheritdoc cref="IIncrementOperators{TSelf}.op_Increment(TSelf)" />
		public static Complex<T> operator checked ++(Complex<T> value) => checked(new Complex<T>(value._real + T.One, value._imag));

		/// <inheritdoc cref="IDecrementOperators{TSelf}.op_Decrement(TSelf)" />
		public static Complex<T> operator --(Complex<T> value) => new Complex<T>(value._real - T.One, value._imag);
		/// <inheritdoc cref="IDecrementOperators{TSelf}.op_Decrement(TSelf)" />
		public static Complex<T> operator checked --(Complex<T> value) => checked(new Complex<T>(value._real - T.One, value._imag));

		public static Complex<T> operator +(Complex<T> left, Complex<T> right) => new Complex<T>(left._real + right._real, left._imag + right._imag);
		public static Complex<T> operator +(Complex<T> left, T right) => new Complex<T>(left._real + right, left._real);
		public static Complex<T> operator +(T left, Complex<T> right) => new Complex<T>(left + right._real, right._imag);

		public static Complex<T> operator checked +(Complex<T> left, Complex<T> right) => checked(new Complex<T>(left._real + right._real, left._imag + right._imag));

		public static Complex<T> operator -(Complex<T> left, Complex<T> right) => new Complex<T>(left._real - right._real, left._imag - right._imag);
		public static Complex<T> operator -(Complex<T> left, T right) => new Complex<T>(left._real - right, left._imag);
		public static Complex<T> operator -(T left, Complex<T> right) => new Complex<T>(left - right._real, -right._imag);

		public static Complex<T> operator checked -(Complex<T> left, Complex<T> right) => checked(new Complex<T>(left._real - right._real, left._imag - right._imag));

		public static Complex<T> operator *(Complex<T> left, Complex<T> right)
		{
			T realPart = (left._real * right._real) - (left._imag * right._imag);
			T imagPart = (left._imag * right._real) + (left._real * right._imag);
			return new Complex<T>(realPart, imagPart);
		}
		public static Complex<T> operator *(Complex<T> left, T right)
		{
			// Non-IEEE-754 numbers do not have NaN defined, so must throw overflow exceptions.
			if (!SupportsNaN && !(T.IsFinite(left._imag) && T.IsFinite(left._imag)))
				throw new OverflowException();
			else
			{
				if (!T.IsFinite(left._real))
				{
					if (!T.IsFinite(left._imag))
					{
						return NaN;
					}

					return new Complex<T>(left._real * right, _tNaN);
				}

				if (!T.IsFinite(left._imag))
				{
					return new Complex<T>(_tNaN, left._imag * right);
				}
			}

			return new Complex<T>(left._real * right, left._imag * right);
		}
		public static Complex<T> operator *(T left, Complex<T> right)
		{
			// Non-IEEE-754 numbers do not have NaN defined, so must throw overflow exceptions.
			if (!SupportsNaN && !(T.IsFinite(right._imag) && T.IsFinite(right._imag)))
				throw new OverflowException();
			else
			{

				if (!T.IsFinite(right._real))
				{
					if (!T.IsFinite(right._imag))
					{
						return NaN;
					}

					return new Complex<T>(left * right._real, _tNaN);
				}

				if (!T.IsFinite(right._imag))
				{
					return new Complex<T>(_tNaN, left * right._imag);
				}
			}

			return new Complex<T>(left * right._real, left * right._imag);
		}

		public static Complex<T> operator checked *(Complex<T> left, Complex<T> right)
		{
			T realPart = checked((left._real * right._real) - (left._imag * right._imag));
			T imagPart = checked((left._imag * right._real) + (left._real * right._imag));
			return new Complex<T>(realPart, imagPart);
		}

		public static Complex<T> operator /(Complex<T> left, Complex<T> right)
		{
			// Division : Smith's formula.

			T a = left._real;
			T b = left._imag;
			T c = right._real;
			T d = right._imag;

			// Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
			if (T.Abs(d).CompareTo(T.Abs(c)) < 0)
			{
				T doc = d / c;
				return new Complex<T>((a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc));
			}
			else
			{
				T cod = c / d;
				return new Complex<T>((b + a * cod) / (d + c * cod), (-a + b * cod) / (d + c * cod));
			}
		}

		public static Complex<T> operator /(Complex<T> left, T right)
		{
			// IEEE prohibit optimizations which are value changing
			// so we make sure that behaviour for the simplified version exactly match
			// full version.

			// Non-IEEE-754 numbers do not have NaN defined, so must throw overflow and divide-by-zero exceptions.
			if (!SupportsNaN)
			{
				if (right == T.Zero)
					throw new DivideByZeroException();
				if (!(T.IsFinite(left._real) && T.IsFinite(left._imag)))
					throw new OverflowException();
			}
			else
			{
				if (right == T.Zero)
				{
					return NaN;
				}

				if (!T.IsFinite(left._real))
				{
					if (!T.IsFinite(left._imag))
					{
						return NaN;
					}

					return new Complex<T>(left._real / right, _tNaN);
				}

				if (!T.IsFinite(left._imag))
				{
					return new Complex<T>(_tNaN, left._imag / right);
				}
			}

			// Here the actual optimized version of code.
			return new Complex<T>(left._real / right, left._imag / right);
		}

		public static Complex<T> operator /(T left, Complex<T> right)
		{
			// Division : Smith's formula.
			T a = left;
			T c = right._real;
			T d = right._imag;

			// Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
			if (T.Abs(d).CompareTo(T.Abs(c)) < 0)
			{
				T doc = d / c;
				return new Complex<T>(a / (c + d * doc), (-a * doc) / (c + d * doc));
			}
			else
			{
				T cod = c / d;
				return new Complex<T>(a * cod / (d + c * cod), -a / (d + c * cod));
			}
		}

		public static Complex<T> operator checked /(Complex<T> left, Complex<T> right)
		{
			// Division : Smith's formula.

			T a = left._real;
			T b = left._imag;
			T c = right._real;
			T d = right._imag;

			// Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
			if (T.Abs(d).CompareTo(T.Abs(c)) < 0)
			{
				T doc = d / c;
				return checked(new Complex<T>((a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc)));
			}
			else
			{
				T cod = c / d;
				return checked(new Complex<T>((b + a * cod) / (d + c * cod), (-a + b * cod) / (d + c * cod)));
			}
		}

		public static T Abs(Complex<T> value) => Hypot(value._real, value._imag);

		private static T Hypot(T a, T b)
		{
			if (!_isSimpleSqrt)
			{
				return SqrtFunc(a * a + b * b);
			}
			else
			{
				// Using
				//   sqrt(a^2 + b^2) = |a| * sqrt(1 + (b/a)^2)
				// we can factor out the larger component to dodge overflow even when a * a would overflow.
				a = T.Abs(a);
				b = T.Abs(b);

				T small, large;
				if (a.CompareTo(b) < 0)
				{
					small = a;
					large = b;
				}
				else
				{
					small = b;
					large = a;
				}

				if (small == T.Zero)
					return large;
				else if (T.IsPositiveInfinity(large) && !T.IsNaN(small))
					return _tInfin; // Should always have a representable infinity
				else
				{
					T ratio = small / large;
					return large * SqrtFunc(T.One + ratio * ratio);
				}
			}
		}

		//
		// Identities
		//

		/// <inheritdoc cref="IAdditiveIdentity{TSelf, TResult}.AdditiveIdentity" />
		static Complex<T> IAdditiveIdentity<Complex<T>, Complex<T>>.AdditiveIdentity => Zero;
		/// <inheritdoc cref="IMultiplicativeIdentity{TSelf, TResult}" />
		static Complex<T> IMultiplicativeIdentity<Complex<T>, Complex<T>>.MultiplicativeIdentity => One;

		//
		// INumberBase
		//

		#region INumberBase

		/// <inheritdoc cref="INumber{TSelf}.One" />
		static Complex<T> INumberBase<Complex<T>>.One => new Complex<T>(T.One, T.Zero);

		/// <inheritdoc cref="INumberBase{TSelf}.Radix" />
		static int INumberBase<Complex<T>>.Radix => T.Radix;

		/// <inheritdoc cref="INumberBase{TSelf}.Zero" />
		static Complex<T> INumberBase<Complex<T>>.Zero => new Complex<T>(T.Zero, T.Zero);

		/// <inheritdoc cref="INumberBase{TSelf}.Abs(TSelf)"/>
		static Complex<T> INumberBase<Complex<T>>.Abs(Complex<T> value) => new Complex<T>(Abs(value), T.Zero);

		/// <inheritdoc cref="INumberBase{TSelf}.IsCanonical(TSelf)" />
		static bool INumberBase<Complex<T>>.IsCanonical(Complex<T> value) => true;

		/// <inheritdoc cref="INumberBase{TSelf}.IsComplexNumber(TSelf)" />
		public static bool IsComplexNumber(Complex<T> value) => value._real != T.Zero && value._imag != T.Zero;

		/// <inheritdoc cref="INumberBase{TSelf}.IsEvenInteger(TSelf)" />
		public static bool IsEvenInteger(Complex<T> value) => value._imag == T.Zero && T.IsEvenInteger(value._real);

		/// <inheritdoc cref="INumberBase{TSelf}.IsImaginaryNumber(TSelf)" />
		public static bool IsImaginaryNumber(Complex<T> value) => value._real == T.Zero && T.IsRealNumber(value._imag);

		/// <inheritdoc cref="INumberBase{TSelf}.IsInteger(TSelf)" />
		public static bool IsInteger(Complex<T> value) => value._imag == T.Zero && T.IsInteger(value._real);

		/// <inheritdoc cref="INumberBase{TSelf}.IsNegative(TSelf)" />
		public static bool IsNegative(Complex<T> value) => value._imag == T.Zero && T.IsNegative(value._real);

		/// <inheritdoc cref="INumberBase{TSelf}.IsNegativeInfinity(TSelf)" />
		public static bool IsNegativeInfinity(Complex<T> value) => value._imag == T.Zero && T.IsNegativeInfinity(value._real);

		/// <inheritdoc cref="INumberBase{TSelf}.IsNormal(TSelf)" />
		public static bool IsNormal(Complex<T> value) => T.IsNormal(value._real) && (value._imag == T.Zero || T.IsNormal(value._imag));

		/// <inheritdoc cref="INumberBase{TSelf}.IsOddInteger(TSelf)" />
		public static bool IsOddInteger(Complex<T> value) => value._imag == T.Zero && T.IsOddInteger(value._real);

		/// <inheritdoc cref="INumberBase{TSelf}.IsPositive(TSelf)" />
		public static bool IsPositive(Complex<T> value) => value._imag == T.Zero && T.IsPositive(value._real);

		/// <inheritdoc cref="INumberBase{TSelf}.IsPositiveInfinity(TSelf)" />
		public static bool IsPositiveInfinity(Complex<T> value) => value._imag == T.Zero && T.IsPositiveInfinity(value._real);

		/// <inheritdoc cref="INumberBase{TSelf}.IsRealNumber(TSelf)" />
		public static bool IsRealNumber(Complex<T> value) => value._imag == T.Zero && T.IsRealNumber(value._real);

		/// <inheritdoc cref="INumberBase{TSelf}.IsSubnormal(TSelf)" />
		public static bool IsSubnormal(Complex<T> value) => T.IsSubnormal(value._real) || T.IsSubnormal(value._imag);

		/// <inheritdoc cref="INumberBase{TSelf}.IsZero(TSelf)" />
		public static bool IsZero(Complex<T> value) => value._real == T.Zero && value._imag == T.Zero;

		/// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitude(TSelf, TSelf)" />
		public static Complex<T> MaxMagnitude(Complex<T> x, Complex<T> y)
		{
			// complex numbers are not normally comparable, however every complex
			// number has a real magnitude (absolute value) and so we can provide
			// an implementation for MaxMagnitude

			// This matches the IEEE 754:2019 `maximumMagnitude` function
			//
			// It propagates NaN inputs back to the caller and
			// otherwise returns the input with a larger magnitude.
			// It treats +0 as larger than -0 as per the specification.

			T ax = Abs(x);
			T ay = Abs(y);

			if ((ax.CompareTo(ay) > 0) || T.IsNaN(ax))
			{
				return x;
			}

			if (ax == ay)
			{
				// We have two equal magnitudes which means we have two of the following
				//   `+a + ib`
				//   `-a + ib`
				//   `+a - ib`
				//   `-a - ib`
				//
				// We want to treat `+a + ib` as greater than everything and `-a - ib` as
				// lesser. For `-a + ib` and `+a - ib` its "ambiguous" which should be preferred
				// so we will just preference `+a - ib` since that's the most correct choice
				// in the face of something like `+a - i0.0` vs `-a + i0.0`. This is the "most
				// correct" choice because both represent real numbers and `+a` is preferred
				// over `-a`.

				if (T.IsNegative(y._real))
				{
					if (T.IsNegative(y._imag))
					{
						// when `y` is `-a - ib` we always prefer `x` (its either the same as
						// `x` or some part of `x` is positive).

						return x;
					}
					else
					{
						if (T.IsNegative(x._real))
						{
							// when `y` is `-a + ib` and `x` is `-a + ib` or `-a - ib` then
							// we either have same value or both parts of `x` are negative
							// and we want to prefer `y`.

							return y;
						}
						else
						{
							// when `y` is `-a + ib` and `x` is `+a + ib` or `+a - ib` then
							// we want to prefer `x` because either both parts are positive
							// or we want to prefer `+a - ib` due to how it handles when `x`
							// represents a real number.

							return x;
						}
					}
				}
				else if (T.IsNegative(y._imag))
				{
					if (T.IsNegative(x._real))
					{
						// when `y` is `+a - ib` and `x` is `-a + ib` or `-a - ib` then
						// we either both parts of `x` are negative or we want to prefer
						// `+a - ib` due to how it handles when `y` represents a real number.

						return y;
					}
					else
					{
						// when `y` is `+a - ib` and `x` is `+a + ib` or `+a - ib` then
						// we want to prefer `x` because either both parts are positive
						// or they represent the same value.

						return x;
					}
				}
			}

			return y;
		}

		/// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitudeNumber(TSelf, TSelf)" />
		public static Complex<T> MaxMagnitudeNumber(Complex<T> x, Complex<T> y)
		{
			// complex numbers are not normally comparable, however every complex
			// number has a real magnitude (absolute value) and so we can provide
			// an implementation for MaxMagnitudeNumber

			// This matches the IEEE 754:2019 `maximumMagnitudeNumber` function
			//
			// It does not propagate NaN inputs back to the caller and
			// otherwise returns the input with a larger magnitude.
			// It treats +0 as larger than -0 as per the specification.

			T ax = Abs(x);
			T ay = Abs(y);

			if ((ax.CompareTo(ay) > 0) || T.IsNaN(ay))
			{
				return x;
			}

			if (ax == ay)
			{
				// We have two equal magnitudes which means we have two of the following
				//   `+a + ib`
				//   `-a + ib`
				//   `+a - ib`
				//   `-a - ib`
				//
				// We want to treat `+a + ib` as greater than everything and `-a - ib` as
				// lesser. For `-a + ib` and `+a - ib` its "ambiguous" which should be preferred
				// so we will just preference `+a - ib` since that's the most correct choice
				// in the face of something like `+a - i0.0` vs `-a + i0.0`. This is the "most
				// correct" choice because both represent real numbers and `+a` is preferred
				// over `-a`.

				if (T.IsNegative(y._real))
				{
					if (T.IsNegative(y._imag))
					{
						// when `y` is `-a - ib` we always prefer `x` (its either the same as
						// `x` or some part of `x` is positive).

						return x;
					}
					else
					{
						if (T.IsNegative(x._real))
						{
							// when `y` is `-a + ib` and `x` is `-a + ib` or `-a - ib` then
							// we either have same value or both parts of `x` are negative
							// and we want to prefer `y`.

							return y;
						}
						else
						{
							// when `y` is `-a + ib` and `x` is `+a + ib` or `+a - ib` then
							// we want to prefer `x` because either both parts are positive
							// or we want to prefer `+a - ib` due to how it handles when `x`
							// represents a real number.

							return x;
						}
					}
				}
				else if (T.IsNegative(y._imag))
				{
					if (T.IsNegative(x._real))
					{
						// when `y` is `+a - ib` and `x` is `-a + ib` or `-a - ib` then
						// we either both parts of `x` are negative or we want to prefer
						// `+a - ib` due to how it handles when `y` represents a real number.

						return y;
					}
					else
					{
						// when `y` is `+a - ib` and `x` is `+a + ib` or `+a - ib` then
						// we want to prefer `x` because either both parts are positive
						// or they represent the same value.

						return x;
					}
				}
			}

			return y;
		}

		/// <inheritdoc cref="INumberBase{TSelf}.MinMagnitude(TSelf, TSelf)" />
		public static Complex<T> MinMagnitude(Complex<T> x, Complex<T> y)
		{
			// complex numbers are not normally comparable, however every complex
			// number has a real magnitude (absolute value) and so we can provide
			// an implementation for MaxMagnitude

			// This matches the IEEE 754:2019 `minimumMagnitude` function
			//
			// It propagates NaN inputs back to the caller and
			// otherwise returns the input with a smaller magnitude.
			// It treats -0 as smaller than +0 as per the specification.

			T ax = Abs(x);
			T ay = Abs(y);

			if ((ax.CompareTo(ay) < 0) || T.IsNaN(ax))
			{
				return x;
			}

			if (ax == ay)
			{
				// We have two equal magnitudes which means we have two of the following
				//   `+a + ib`
				//   `-a + ib`
				//   `+a - ib`
				//   `-a - ib`
				//
				// We want to treat `+a + ib` as greater than everything and `-a - ib` as
				// lesser. For `-a + ib` and `+a - ib` its "ambiguous" which should be preferred
				// so we will just preference `-a + ib` since that's the most correct choice
				// in the face of something like `+a - i0.0` vs `-a + i0.0`. This is the "most
				// correct" choice because both represent real numbers and `-a` is preferred
				// over `+a`.

				if (T.IsNegative(y._real))
				{
					if (T.IsNegative(y._imag))
					{
						// when `y` is `-a - ib` we always prefer `y` as both parts are negative
						return y;
					}
					else
					{
						if (T.IsNegative(x._real))
						{
							// when `y` is `-a + ib` and `x` is `-a + ib` or `-a - ib` then
							// we either have same value or both parts of `x` are negative
							// and we want to prefer it.

							return x;
						}
						else
						{
							// when `y` is `-a + ib` and `x` is `+a + ib` or `+a - ib` then
							// we want to prefer `y` because either both parts of 'x' are positive
							// or we want to prefer `-a - ib` due to how it handles when `y`
							// represents a real number.

							return y;
						}
					}
				}
				else if (T.IsNegative(y._imag))
				{
					if (T.IsNegative(x._real))
					{
						// when `y` is `+a - ib` and `x` is `-a + ib` or `-a - ib` then
						// either both parts of `x` are negative or we want to prefer
						// `-a - ib` due to how it handles when `x` represents a real number.

						return x;
					}
					else
					{
						// when `y` is `+a - ib` and `x` is `+a + ib` or `+a - ib` then
						// we want to prefer `y` because either both parts of x are positive
						// or they represent the same value.

						return y;
					}
				}
				else
				{
					return x;
				}
			}

			return y;
		}

		/// <inheritdoc cref="INumberBase{TSelf}.MinMagnitudeNumber(TSelf, TSelf)" />
		public static Complex<T> MinMagnitudeNumber(Complex<T> x, Complex<T> y)
		{
			// complex numbers are not normally comparable, however every complex
			// number has a real magnitude (absolute value) and so we can provide
			// an implementation for MinMagnitudeNumber

			// This matches the IEEE 754:2019 `minimumMagnitudeNumber` function
			//
			// It does not propagate NaN inputs back to the caller and
			// otherwise returns the input with a smaller magnitude.
			// It treats -0 as smaller than +0 as per the specification.

			T ax = Abs(x);
			T ay = Abs(y);

			if (ax.CompareTo(ay) < 0 || T.IsNaN(ay))
				return x;

			if (ax == ay)
			{
				// We have two equal magnitudes which means we have two of the following
				//   `+a + ib`
				//   `-a + ib`
				//   `+a - ib`
				//   `-a - ib`
				//
				// We want to treat `+a + ib` as greater than everything and `-a - ib` as
				// lesser. For `-a + ib` and `+a - ib` its "ambiguous" which should be preferred
				// so we will just preference `-a + ib` since that's the most correct choice
				// in the face of something like `+a - i0.0` vs `-a + i0.0`. This is the "most
				// correct" choice because both represent real numbers and `-a` is preferred
				// over `+a`.

				if (T.IsNegative(y._real))
				{
					if (T.IsNegative(y._imag))
					{
						// when `y` is `-a - ib` we always prefer `y` as both parts are negative
						return y;
					}
					else
					{
						if (T.IsNegative(x._real))
						{
							// when `y` is `-a + ib` and `x` is `-a + ib` or `-a - ib` then
							// we either have same value or both parts of `x` are negative
							// and we want to prefer it.

							return x;
						}
						else
						{
							// when `y` is `-a + ib` and `x` is `+a + ib` or `+a - ib` then
							// we want to prefer `y` because either both parts of 'x' are positive
							// or we want to prefer `-a - ib` due to how it handles when `y`
							// represents a real number.

							return y;
						}
					}
				}
				else if (T.IsNegative(y._imag))
				{
					if (T.IsNegative(x._real))
					{
						// when `y` is `+a - ib` and `x` is `-a + ib` or `-a - ib` then
						// either both parts of `x` are negative or we want to prefer
						// `-a - ib` due to how it handles when `x` represents a real number.

						return x;
					}
					else
					{
						// when `y` is `+a - ib` and `x` is `+a + ib` or `+a - ib` then
						// we want to prefer `y` because either both parts of x are positive
						// or they represent the same value.

						return y;
					}
				}
				else
				{
					return x;
				}
			}

			return y;
		}

		/// <inheritdoc cref="INumberBase{TSelf}.Parse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?)" />
		public static Complex<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
		{
			if (!TryParse(s, style, provider, out Complex<T> result))
				throw new OverflowException();
			return result;
		}

		/// <inheritdoc cref="INumberBase{TSelf}.Parse(string, NumberStyles, IFormatProvider?)" />
		public static Complex<T> Parse(string s, NumberStyles style, IFormatProvider? provider)
		{
			ArgumentNullException.ThrowIfNull(s);
			return Parse(s.AsSpan(), style, provider);
		}

		/// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromChecked{TOther}(TOther, out TSelf)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool INumberBase<Complex<T>>.TryConvertFromChecked<TOther>(TOther value, out Complex<T> result)
		{
			result = default;

			// Add special case for System.Numerics.Complex to set both real and imaginary parts
			if (value is Complex _complex)
			{
				// Convert the double parts of the complex number to be of type T
				if (!T.TryConvertFromChecked(_complex.Real, out T? real) || real is null)
					return false;
				if (!T.TryConvertFromChecked(_complex.Imaginary, out T? imag) || imag is null)
					return false;

				result = new Complex<T>(real, imag);
				return true;
			}
			else
			{
				// Treat all other cases as scalars and set only the real part

				if (!T.TryConvertFromChecked(value, out T? real) || real is null)
					return false;

				result = new Complex<T>(real, T.Zero);
				return true;
			}
		}

		/// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromSaturating{TOther}(TOther, out TSelf)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool INumberBase<Complex<T>>.TryConvertFromSaturating<TOther>(TOther value, out Complex<T> result)
		{
			result = default;

			// Add special case for System.Numerics.Complex to set both real and imaginary parts
			if (value is Complex _complex)
			{
				// Convert the double parts of the complex number to be of type T
				if (!T.TryConvertFromSaturating(_complex.Real, out T? real) || real is null)
					return false;
				if (!T.TryConvertFromSaturating(_complex.Imaginary, out T? imag) || imag is null)
					return false;

				result = new Complex<T>(real, imag);
				return true;
			}
			else
			{
				// Treat all other cases as scalars and set only the real part

				if (!T.TryConvertFromSaturating(value, out T? real) || real is null)
					return false;

				result = new Complex<T>(real, T.Zero);
				return true;
			}
		}

		/// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromTruncating{TOther}(TOther, out TSelf)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool INumberBase<Complex<T>>.TryConvertFromTruncating<TOther>(TOther value, out Complex<T> result)
		{
			result = default;

			// Add special case for System.Numerics.Complex to set both real and imaginary parts
			if (value is Complex _complex)
			{
				// Convert the double parts of the complex number to be of type T
				if (!T.TryConvertFromTruncating(_complex.Real, out T? real) || real is null)
					return false;
				if (!T.TryConvertFromTruncating(_complex.Imaginary, out T? imag) || imag is null)
					return false;

				result = new Complex<T>(real, imag);
				return true;
			}
			else
			{
				// Treat all other cases as scalars and set only the real part

				if (!T.TryConvertFromTruncating(value, out T? real) || real is null)
					return false;

				result = new Complex<T>(real, T.Zero);
				return true;
			}
		}

		/// <inheritdoc cref="INumberBase{TSelf}.TryConvertToChecked{TOther}(TSelf, out TOther)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool INumberBase<Complex<T>>.TryConvertToChecked<TOther>(Complex<T> value, out TOther result)
		{
			if (typeof(TOther) == typeof(Complex)) // Special case for System.Numerics.Complex
			{
				result = default!;

				if (!T.TryConvertToChecked(value._real, out double real))
					return false;
				if (!T.TryConvertToChecked(value._imag, out double imag))
					return false;

				result = (TOther)(object)new Complex(real, imag);
				return true;
			}
			else // Default case
			{
				if (value._imag != T.Zero)
					throw new OverflowException();
				if (!T.TryConvertToChecked(value._real, out TOther? actualResult) || actualResult is null)
				{
					result = default!;
					return false;
				}

				result = actualResult;
				return true;
			}
		}

		/// <inheritdoc cref="INumberBase{TSelf}.TryConvertToSaturating{TOther}(TSelf, out TOther)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool INumberBase<Complex<T>>.TryConvertToSaturating<TOther>(Complex<T> value, out TOther result)
		{
			if (typeof(TOther) == typeof(Complex)) // Special case for System.Numerics.Complex
			{
				result = default!;

				if (!T.TryConvertToSaturating(value._real, out double real))
					return false;
				if (!T.TryConvertToSaturating(value._imag, out double imag))
					return false;

				result = (TOther)(object)new Complex(real, imag);
				return true;
			}
			else // Default case
			{
				if (value._imag != T.Zero)
					throw new OverflowException();
				if (!T.TryConvertToSaturating(value._real, out TOther? actualResult) || actualResult is null)
				{
					result = default!;
					return false;
				}

				result = actualResult;
				return true;
			}
		}

		/// <inheritdoc cref="INumberBase{TSelf}.TryConvertToTruncating{TOther}(TSelf, out TOther)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool INumberBase<Complex<T>>.TryConvertToTruncating<TOther>(Complex<T> value, out TOther result)
		{
			if (typeof(TOther) == typeof(Complex)) // Special case for System.Numerics.Complex
			{
				result = default!;

				if (!T.TryConvertToTruncating(value._real, out double real))
					return false;
				if (!T.TryConvertToTruncating(value._imag, out double imag))
					return false;

				result = (TOther)(object)new Complex(real, imag);
				return true;
			}
			else // Default case
			{
				if (value._imag != T.Zero)
					throw new OverflowException();
				if (!T.TryConvertToTruncating(value._real, out TOther? actualResult) || actualResult is null)
				{
					result = default!;
					return false;
				}

				result = actualResult;
				return true;
			}
		}

		/// <inheritdoc cref="INumberBase{TSelf}.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?, out TSelf)" />
		public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Complex<T> result)
		{
			// Modified from System.Numerics.Complex.TryParse() for generic support

			ValidateParseStyleFloatingPoint(style, typeof(T).IsAssignableTo(typeof(IFloatingPoint<>)));

			int openBracket = s.IndexOf('<');
			int semicolon = s.IndexOf(';');
			int closeBracket = s.IndexOf('>');

			if (s.Length < 5 || openBracket == -1 || semicolon == -1 || closeBracket == -1 || openBracket > semicolon || openBracket > closeBracket || semicolon > closeBracket)
			{
				// We need at least 5 characters for `<0;0>`
				// We also expect a to find an open bracket, a semicolon, and a closing bracket in that order

				result = default;
				return false;
			}

			if (openBracket != 0 && (((style & NumberStyles.AllowLeadingWhite) == 0) || !s.Slice(0, openBracket).IsWhiteSpace()))
			{
				// The opening bracket wasn't the first and we either didn't allow leading whitespace
				// or one of the leading characters wasn't whitespace at all.

				result = default;
				return false;
			}

			if (!T.TryParse(s.Slice(openBracket + 1, semicolon), style, provider, out T real))
			{
				result = default;
				return false;
			}

			if (char.IsWhiteSpace(s[semicolon + 1]))
			{
				// We allow a single whitespace after the semicolon regardless of style, this is so that
				// the output of `ToString` can be correctly parsed by default and values will roundtrip.
				semicolon += 1;
			}

			if (!T.TryParse(s.Slice(semicolon + 1, closeBracket - semicolon), style, provider, out T imaginary))
			{
				result = default;
				return false;
			}

			if ((closeBracket != (s.Length - 1)) && (((style & NumberStyles.AllowTrailingWhite) == 0) || !s.Slice(closeBracket).IsWhiteSpace()))
			{
				// The closing bracket wasn't the last and we either didn't allow trailing whitespace
				// or one of the trailing characters wasn't whitespace at all.

				result = default;
				return false;
			}

			result = new Complex<T>(real, imaginary);
			return true;

			static void ValidateParseStyleFloatingPoint(NumberStyles style, bool disallowHex)
			{
				// Check for undefined flags or hex number
				if ((style & (InvalidNumberStyles | (disallowHex ? NumberStyles.AllowHexSpecifier : 0))) != 0)
				{
					ThrowInvalid(style);

					static void ThrowInvalid(NumberStyles value)
					{
						if ((value & InvalidNumberStyles) != 0)
						{
							throw new ArgumentException("An undefined NumberStyles value is being used.", nameof(style));
						}

						throw new ArgumentException("The number style AllowHexSpecifier is not supported on floating point data types.");
					}
				}
			}
		}

		/// <inheritdoc cref="INumberBase{TSelf}.TryParse(string?, NumberStyles, IFormatProvider?, out TSelf)" />
		public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Complex<T> result)
		{
			if (s is null)
			{
				result = default;
				return true;
			}

			return TryParse(s.AsSpan(), style, provider, out result);
		}

		#endregion

		//
		// IParsable
		//

		#region IParsable

		/// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider?)" />
		public static Complex<T> Parse(string s, IFormatProvider? provider) => Parse(s, DefaultNumberStyle, provider);

		/// <inheritdoc cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)" />
		public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Complex<T> result) => TryParse(s, DefaultNumberStyle, provider, out result);

		#endregion

		//
		// ISignedNumber
		//

		#region ISignedNumber

		/// <inheritdoc cref="ISignedNumber{TSelf}.NegativeOne" />
		static Complex<T> ISignedNumber<Complex<T>>.NegativeOne => new Complex<T>(T.NegativeOne, T.Zero);

		#endregion

		//
		// ISpanFormattable
		//

		#region ISpanFormattable

		private static readonly int _zeroStringLength = T.Zero.ToString(null, null).Length;

		/// <inheritdoc cref="ISpanFormattable.TryFormat(Span{char}, out int, ReadOnlySpan{char}, IFormatProvider?)" />
		public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
		{
			// Based on System.Numerics.Complex.TryFormat()

			int charsWrittenSoFar = 0;

			// We have at least 4 + _zeroStringLength more characters for: <*; *>
			if (destination.Length < 4 + _zeroStringLength)
			{
				charsWritten = charsWrittenSoFar;
				return false;
			}

			destination[charsWrittenSoFar++] = '<';

			bool tryFormatSucceeded = _real.TryFormat(destination.Slice(charsWrittenSoFar), out int tryFormatCharsWritten, format, provider);
			charsWrittenSoFar += tryFormatCharsWritten;

			// We have at least 3 + _zeroStringLength more characters for: ; *>
			if (!tryFormatSucceeded || (destination.Length < (charsWrittenSoFar + 3 + _zeroStringLength)))
			{
				charsWritten = charsWrittenSoFar;
				return false;
			}

			destination[charsWrittenSoFar++] = ';';
			destination[charsWrittenSoFar++] = ' ';

			tryFormatSucceeded = _imag.TryFormat(destination.Slice(charsWrittenSoFar), out tryFormatCharsWritten, format, provider);
			charsWrittenSoFar += tryFormatCharsWritten;

			// We have at least 1 more character for: >
			if (!tryFormatSucceeded || (destination.Length < (charsWrittenSoFar + 1)))
			{
				charsWritten = charsWrittenSoFar;
				return false;
			}

			destination[charsWrittenSoFar++] = '>';

			charsWritten = charsWrittenSoFar;
			return true;

			///
		}

		#endregion

		//
		// ISpanParsable
		//

		#region ISpanParsable

		/// <inheritdoc cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)" />
		public static Complex<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, DefaultNumberStyle, provider);

		/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)" />
		public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Complex<T> result) => TryParse(s, DefaultNumberStyle, provider, out result);

		#endregion

		static class TypeInit
		{
			public static bool ImplementsInterface(Type interfaceType)
			{
				if (typeof(T).IsAssignableTo(interfaceType))
					return true;
				else
				{
					if (interfaceType.IsGenericType)
					{
						interfaceType = interfaceType.GetGenericTypeDefinition();
						return typeof(T).GetInterfaces().Any(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == interfaceType);
					}
					else
						return typeof(T).GetInterfaces().Any(i => i == interfaceType);
				}
			}

			public static Func<T, T> CreateSqrtFunc(out bool isPreciseSqrt)
			{
				Type[] interfaces = typeof(T).GetInterfaces();
				if (ImplementsInterface(typeof(IRootFunctions<>)))
				{
					MethodInfo info = typeof(T).GetInterfaceMap(typeof(IRootFunctions<>).MakeGenericType(typeof(T))).InterfaceMethods.First(i => i.Name == "Sqrt");
					isPreciseSqrt = true;
					return info.CreateDelegate<Func<T, T>>();
				}
				else if (ImplementsInterface(typeof(IFloatingPoint<>)))
				{
					[MethodImpl(MethodImplOptions.AggressiveOptimization)]
					static T SqrtConverting<TOther>(T value) where TOther : INumberBase<TOther>, IRootFunctions<TOther>
					{
						if (T.TryConvertToChecked(value, out TOther? d) && d is not null &&
							TOther.TryConvertToChecked(TOther.Sqrt(d), out T? result) && result is not null)
							return result;
						throw new OverflowException();
					}

					isPreciseSqrt = true;
					return SqrtConverting<double>;
				}
				else
				{
					[MethodImpl(MethodImplOptions.AggressiveOptimization)]
					static T BabylonianSqrt(T value)
					{
						T x0 = value / _two;

						int comp = x0.CompareTo(T.Zero);
						if (comp < 0)
							throw new InvalidOperationException();
						if (comp == 0)
							return value;
						else
						{
							T x1 = (x0 + value / x0) / _two;

							while (x1.CompareTo(x0) < 0)
							{
								x0 = x1;
								x1 = (x0 + value / x0) / _two;
							}

							return x0;
						}
					}

					isPreciseSqrt = false;
					return BabylonianSqrt;
				}
			}
		}
	}
}
