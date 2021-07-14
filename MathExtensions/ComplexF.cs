#if PROCESSOR_ARCHITECTURE == AMD64 || PROCESSOR_ARCHITECTURE == X86
#define X86
#elif PROCESSOR_ARCHITECTURE == Arm
#define ARM
#endif
#nullable enable
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Diagnostics;
using System.Globalization;
#if NET5_0
using System.Runtime.Intrinsics.Arm;
#endif
using System.Text;

namespace MathExtensions
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0032:Use auto property", Justification = "<Pending>")]
	public readonly struct ComplexF : IEquatable<ComplexF>, IFormattable
	{
		private readonly float _real;
		private readonly float _imag;

		public static readonly ComplexF ImaginaryOne = new ComplexF(0, 1);
		public static readonly ComplexF Infinity = new ComplexF(float.PositiveInfinity, float.PositiveInfinity);
		public static readonly ComplexF NaN = new ComplexF(float.NaN, float.NaN);
		public static readonly ComplexF One = new ComplexF(1, 0);
		public static readonly ComplexF Zero = new ComplexF(0, 0);

		public float Magnitude => Abs(this);
		public float Imaginary => _imag;
		public float Phase => MathF.Atan2(_imag, _real);
		public float Real => _real;

		public ComplexF(float real, float imaginary)
		{
			_real = real;
			_imag = imaginary;
		}

		public static bool IsFinite(ComplexF value) => float.IsFinite(value._real) && float.IsFinite(value._imag);
		public static bool IsInfinite(ComplexF value) => float.IsInfinity(value._real) || float.IsInfinity(value._imag);
		public static bool IsNaN(ComplexF value) => float.IsNaN(value._real) || float.IsNaN(value._imag);

		#region Addition
		public static ComplexF Add(ComplexF left, ComplexF right) => new ComplexF(left._real + right._real, left._imag + right._imag);
		public static ComplexF Add(ComplexF left, float right) => new ComplexF(left._real + right, left._imag);
		public static ComplexF Add(float left, ComplexF right) => new ComplexF(left + right._real, right._imag);
		#endregion
		#region Subtraction
		public static ComplexF Subtract(ComplexF left, ComplexF right) => new ComplexF(left._real - right._real, left._imag - right._imag);
		public static ComplexF Subtract(ComplexF left, float right) => new ComplexF(left._real - right, left._imag);
		public static ComplexF Subtract(float left, ComplexF right) => new ComplexF(left - right._real, -right._imag);
		#endregion
		#region Multiplication
		public static ComplexF Multiply(ComplexF left, ComplexF right)
		{
			float real = (left._real * right._real) - (left._imag * right._imag);
			float imag = (left._imag * right._real) + (left._real * right._imag);
			return new ComplexF(real, imag);
		}
		public static ComplexF Multiply(ComplexF left, float right)
		{
			if (!float.IsFinite(left._real))
			{
				if (!float.IsFinite(left._imag))
					return new ComplexF(float.NaN, float.NaN);
				return new ComplexF(left._real * right, float.NaN);
			}
			if (!float.IsFinite(left._imag))
				return new ComplexF(float.NaN, left._imag * right);
			return new ComplexF(left._real * right, left._imag * right);
		}
		public static ComplexF Multiply(float left, ComplexF right)
		{
			if (!float.IsFinite(right._real))
			{
				if (!float.IsFinite(right._imag))
					return new ComplexF(float.NaN, float.NaN);
				return new ComplexF(left * right._real, float.NaN);
			}
			if (!float.IsFinite(right._imag))
				return new ComplexF(float.NaN, left * right._imag);
			return new ComplexF(left * right._real, left * right._imag);
		}
		#endregion
		#region Division
		public static ComplexF Divide(ComplexF dividend, ComplexF divisor)
		{
			// Division : Smith's formula.
			float a = dividend._real;
			float b = dividend._imag;
			float c = divisor._real;
			float d = divisor._imag;
			// Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
			if (MathF.Abs(d) < MathF.Abs(c))
			{
				float doc = d / c;
				return new ComplexF((a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc));
			}
			else
			{
				float cod = c / d;
				return new ComplexF((b + a * cod) / (d + c * cod), (-a + b * cod) / (d + c * cod));
			}
		}
		public static ComplexF Divide(ComplexF dividend, float divisor)
		{
			if (divisor == 0)
				return new ComplexF(float.NaN, float.NaN);
			if (!float.IsFinite(dividend._real))
			{
				if (!float.IsFinite(dividend._imag))
					return new ComplexF(float.NaN, float.NaN);
				return new ComplexF(dividend._real / divisor, float.NaN);
			}
			if (!float.IsFinite(dividend._imag))
				return new ComplexF(float.NaN, dividend._imag / divisor);
			return new ComplexF(dividend._real / divisor, dividend._imag / divisor);
		}
		public static ComplexF Divide(float dividend, ComplexF divisor)
		{
			// Division : Smith's formula.
			float a = dividend;
			float c = divisor._real;
			float d = divisor._imag;
			// Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
			if (MathF.Abs(d) < MathF.Abs(c))
			{
				float doc = d / c;
				return new ComplexF(a / (c + d * doc), (-a * doc) / (c + d * doc));
			}
			else
			{
				float cod = c / d;
				return new ComplexF(a * cod / (d + c * cod), -a / (d + c * cod));
			}
		}
		#endregion
		public static ComplexF Negate(ComplexF value) => new ComplexF(-value._real, -value._imag);
		public static float Abs(ComplexF value) => Hypot(value._real, value._imag);
		public static ComplexF Conjugate(ComplexF value) => new ComplexF(value._real, -value._imag);
		public static ComplexF Reciprocal(ComplexF value)
		{
			if (value._real == 0 && value._imag == 0)
				return Zero;
			return One / value;
		}

		public override bool Equals(object? obj) => base.Equals(obj);
		public bool Equals(ComplexF value) => _real.Equals(value._real) && _imag.Equals(value._imag);
		public override int GetHashCode()
		{
			int n1 = 99999997;
			int realHash = _real.GetHashCode() % n1;
			int imaginaryHash = _imag.GetHashCode();
			int finalHash = realHash ^ imaginaryHash;
			return finalHash;
		}
		public override string ToString() => string.Format(CultureInfo.CurrentCulture, "({0}, {1})", _real, _imag);
		public string ToString(string? format) => string.Format(CultureInfo.CurrentCulture, "({0}, {1})", _real.ToString(format, CultureInfo.CurrentCulture), _imag.ToString(format, CultureInfo.CurrentCulture));
		public string ToString(IFormatProvider? provider) => string.Format(provider, "({0}, {1})", _real, _imag);
		public string ToString(string? format, IFormatProvider? provider) => string.Format(provider, "({0}, {1})", _real.ToString(format, provider), _imag.ToString(format, provider));
		#region Operators
		public static ComplexF operator +(ComplexF value) => value;
		public static ComplexF operator -(ComplexF value) => Negate(value);
		public static ComplexF operator +(ComplexF left, ComplexF right) => Add(left, right);
		public static ComplexF operator +(ComplexF left, float right) => Add(left, right);
		public static ComplexF operator +(float left, ComplexF right) => Add(left, right);
		public static ComplexF operator -(ComplexF left, ComplexF right) => Subtract(left, right);
		public static ComplexF operator -(ComplexF left, float right) => Subtract(left, right);
		public static ComplexF operator -(float left, ComplexF right) => Subtract(left, right);
		public static ComplexF operator *(ComplexF left, ComplexF right) => Multiply(left, right);
		public static ComplexF operator *(ComplexF left, float right) => Multiply(left, right);
		public static ComplexF operator *(float left, ComplexF right) => Multiply(left, right);
		public static ComplexF operator /(ComplexF left, ComplexF right) => Divide(left, right);
		public static ComplexF operator /(ComplexF left, float right) => Divide(left, right);
		public static ComplexF operator /(float left, ComplexF right) => Divide(left, right);
		public static bool operator ==(ComplexF left, ComplexF right) => left.Equals(right);
		public static bool operator !=(ComplexF left, ComplexF right) => !left.Equals(right);
		#endregion

		#region Private Methods
		public static float Log1P(float x)
		{
			// Compute log(1 + x) without loss of accuracy when x is small.

			// Our only use case so far is for positive values, so this isn't coded to handle negative values.
			Debug.Assert((x >= 0.0f) || float.IsNaN(x));

			float xp1 = 1.0f + x;
			if (xp1 == 1.0f)
				return x;
			else if (x < 0.75f)
			{
				// This is accurate to within 5 ulp with any floating-point system that uses a guard digit,
				// as proven in Theorem 4 of "What Every Computer Scientist Should Know About Floating-Point
				// Arithmetic" (https://docs.oracle.com/cd/E19957-01/806-3568/ncg_goldberg.html)
				return x * MathF.Log(xp1) / (xp1 - 1.0f);
			}
			else
				return MathF.Log(xp1);
		}
		private static float Hypot(float a, float b)
		{
			a = MathF.Abs(a);
			b = MathF.Abs(b);
			float small, large;
			if (a < b)
			{
				small = a;
				large = b;
			}
			else
			{
				small = b;
				large = a;
			}
			if (small == 0.0)
				return large;
			else if (float.IsPositiveInfinity(large) && !float.IsNaN(small))
				return float.PositiveInfinity;
			else
			{
				float ratio = small / large;
				return large * MathF.Sqrt(1.0f + ratio * ratio);
			}
		}
		#endregion
		#region Trig Functions
		public static ComplexF Sin(ComplexF value)
		{
			float p = MathF.Exp(value._imag);
			float q = 1.0f / p;
			float sinh = (p - q) * 0.5f;
			float cosh = (p + q) * 0.5f;
			return new ComplexF(MathF.Sin(value._real) * cosh, MathF.Cos(value._real) * sinh);
		}
		public static ComplexF Sinh(ComplexF value)
		{
			ComplexF sin = Sin(new ComplexF(-value._imag, value._real));
			return new ComplexF(sin._imag, -sin._real);
		}
		#endregion

		public static implicit operator ComplexF(ulong value) => new ComplexF(value, 0);
		public static implicit operator ComplexF(uint value) => new ComplexF(value, 0);
		public static implicit operator ComplexF(ushort value) => new ComplexF(value, 0);
		public static implicit operator ComplexF(float value) => new ComplexF(value, 0);
		public static implicit operator ComplexF(sbyte value) => new ComplexF(value, 0);
		public static implicit operator ComplexF(long value) => new ComplexF(value, 0);
		public static implicit operator ComplexF(int value) => new ComplexF(value, 0);
		public static implicit operator ComplexF(short value) => new ComplexF(value, 0);
		public static implicit operator ComplexF(byte value) => new ComplexF(value, 0);
		public static explicit operator ComplexF(double value) => new ComplexF((float)value, 0);
		public static explicit operator ComplexF(decimal value) => new ComplexF((float)value, 0);
		public static explicit operator ComplexF(BigInteger value) => new ComplexF((float)value, 0);
		public static explicit operator ComplexF(Complex value) => new ComplexF((float)value.Real, (float)value.Imaginary);
		public static implicit operator Complex(ComplexF value) => new Complex(value._real, value._imag);
	}
}
#nullable restore
