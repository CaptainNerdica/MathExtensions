using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace MathExtensions
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe readonly struct Complex : IEquatable<Complex>, IFormattable
	{
#pragma warning disable IDE0032 // Use auto property
		public readonly double _real;
		public readonly double _imag;
#pragma warning restore IDE0032 // Use auto property

		public double Real => _real;
		public double Imaginary => _imag;
		public double Magnitude => Abs(this);
		public double Phase => System.Math.Atan2(_real, _imag);

		public static readonly Complex Zero = new Complex(0, 0);
		public static readonly Complex One = new Complex(1, 0);
		public static readonly Complex ImaginaryOne = new Complex(0, 1);
		public static readonly Complex NaN = new Complex(double.NaN, double.NaN);
		public static readonly Complex Infinity = new Complex(double.PositiveInfinity, double.PositiveInfinity);

		public Complex(double real, double imaginary)
		{
			_real = real;
			_imag = imaginary;
		}

		public Complex(double real)
		{
			_real = real;
			_imag = 0;
		}

		public static bool IsFinite(Complex value) => double.IsFinite(value._real) && double.IsFinite(value._imag);
		public static bool IsInfinity(Complex value) => double.IsInfinity(value._real) || double.IsInfinity(value._imag);
		public static bool IsNaN(Complex value) => !IsInfinity(value) && !IsFinite(value);

		public static double Abs(Complex value) => Math.Sqrt(value._real * value._real + value._imag * value._imag);
		public static Complex Conjugate(Complex value)
		{
			if (Sse2.IsSupported)
			{
				Sse2.Store((double*)&value, Sse2.Xor(Vector128.Create(0x0000_0000_0000_0000, 0x8000_0000_0000_0000).AsDouble(), Sse2.LoadVector128((double*)&value)));
				return value;
			}
			return new Complex(value._real, -value._imag);
		}
		public static Complex Reciprocal(Complex value) => value._real == 0 && value._imag == 0 ? Zero : One / value;

		public static Complex operator +(Complex value) => value;
		public static Complex operator -(Complex value)
		{
			if (Sse2.IsSupported)
			{
				Sse2.Store((double*)&value, Sse2.Xor(Vector128.Create(0x8000_0000_0000_0000, 0x8000_0000_0000_0000).AsDouble(), Sse2.LoadVector128((double*)&value)));
				return value;
			}
			return new Complex(-value._real, -value._imag);
		}

		public static Complex operator +(Complex left, Complex right) => new Complex(left._real + right._real, left._imag + right._imag);
		public static Complex operator +(Complex left, double right) => new Complex(left._real + right, left._imag);
		public static Complex operator +(double left, Complex right) => new Complex(left + right._real, right._imag);

		public static Complex operator -(Complex left, Complex right) => new Complex(left._real - right._real, left._imag - right._imag);
		public static Complex operator -(Complex left, double right) => new Complex(left._real - right, left._imag);
		public static Complex operator -(double left, Complex right) => new Complex(left - right._real, -right._imag);

		public static Complex operator *(Complex left, Complex right) => new Complex(left._real * right._real - left._imag * right._imag, left._imag * right._real + left._real * right._imag);
		public static Complex operator *(Complex left, double right) => new Complex(left._real * right, left._imag * right);
		public static Complex operator *(double left, Complex right) => new Complex(left * right._real, left * right._imag);

		public static Complex operator /(Complex left, Complex right)
		{
			if (System.Math.Abs(right._imag) < System.Math.Abs(right._real))
			{
				double doc = right._imag / right._real;
				return new Complex((left._real + left._imag * doc) / (right._real + right._imag * doc), (left._imag - left._real * doc) / (right._real + right._imag * doc));
			}
			else
			{
				double cod = right._real / right._imag;
				return new Complex((left._imag + left._real * cod) / (right._imag + right._real * cod), (-left._real + left._imag * cod) / (right._imag + right._real * cod));
			}
		}
		public static Complex operator /(Complex left, double right)
		{
			if (right == 0)
				return NaN;
			if (!double.IsFinite(left._real))
			{
				if (!double.IsFinite(left._imag))
					return NaN;
				return new Complex(left._real / right, double.NaN);
			}
			if (!double.IsFinite(left._imag))
				return new Complex(double.NaN, left._imag / right);
			return new Complex(left._real / right, left._imag / right);
		}
		public static Complex operator /(double left, Complex right)
		{
			if (System.Math.Abs(right._imag) < System.Math.Abs(right._real))
			{
				double doc = right._imag / right._real;
				return new Complex(left / (right._real + right._imag * doc), (-left * doc) / (right._real + right._imag * doc));
			}
			else
			{
				double cod = right._real / right._imag;
				return new Complex(left * cod / (right._imag + right._real * cod), -left / (right._imag + right._real * cod));
			}
		}

		public static bool operator ==(Complex left, Complex right) => left._real == right._real && left._imag == right._imag;
		public static bool operator !=(Complex left, Complex right) => left._real != right._real || left._imag != right._imag;

		public static implicit operator Complex(double value) => new Complex(value, 0);
		public static implicit operator Complex(float value) => new Complex(value, 0);
		public static implicit operator System.Numerics.Complex(Complex value) => new System.Numerics.Complex(value._real, value._imag);
		public static implicit operator Complex(ComplexF value) => new Complex(value._real, value._imag);
		public static explicit operator ComplexF(Complex value) => new ComplexF((float)value._real, (float)value._imag);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static Vector128<double> LoadVector128(void* ptr) => Sse2.LoadVector128((double*)ptr);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static void StoreVector128(void* ptr, Vector128<double> vector) => Sse2.Store((double*)ptr, vector);

		public override bool Equals(object? obj) => obj is Complex c && Equals(c);
		public bool Equals(Complex other) => this == other;

		public override int GetHashCode() => HashCode.Combine(_real, _imag);
		public override string ToString() => $"({_real}, {_imag})";
		public string ToString(string? format) => ToString(format, null);
		public string ToString(IFormatProvider? provider) => ToString(null, provider);
		public string ToString(string? format, IFormatProvider? provider) => new StringBuilder().Append('(').Append(_real.ToString(format, provider)).Append(stackalloc char[] { ',', ' ' }).Append(_imag.ToString(format, provider)).Append(')').ToString();

		public static Complex Sin(Complex value)
		{
			double exp = System.Math.Exp(value._imag);
			double invExp = 1 / exp;
			return new Complex(System.Math.Sin(value._real) * (exp + invExp) * 0.5, System.Math.Cos(value._real) * (exp - invExp) * 0.5);
		}

		public static Complex Cos(Complex value)
		{
			double exp = System.Math.Exp(value._imag);
			double invExp = 1 / exp;
			return new Complex(System.Math.Cos(value._real) * (exp + invExp) * 0.5, -System.Math.Sin(value._real) * (exp - invExp) * 0.5);
		}

		public static Complex Tan(Complex value)
		{
			const double maxY = 354.891356446692;
			if (System.Math.Abs(value._imag) >= maxY)
				return new Complex(0, System.Math.Sign(value._imag));
			double sin = System.Math.Sin(value._real);
			double cos = System.Math.Cos(value._real);
			double p = System.Math.Exp(value._imag);
			double q = 1 / p;

			double x, y, r2;
			double p2 = p * p;
			double q2 = q * q;
			double s4 = System.Math.ScaleB(sin, 2);
			x = s4 * cos;
			y = p2 - q2;

			r2 = p2 + q2 - s4 * sin + 2;
			return new Complex(x / r2, y / r2);
		}

		public static Complex Sinh(Complex value)
		{
			Complex sin = Sin(new Complex(-value._imag, value._real));
			return new Complex(sin._imag, -sin._real);
		}

		public static Complex Cosh(Complex value) => Cos(new Complex(-value._imag, value._real));

		public static Complex Tanh(Complex value)
		{
			Complex tan = Tan(new Complex(-value._imag, value._real));
			return new Complex(tan._imag, -tan._real);
		}

		public static Complex Log(Complex value) => new Complex(System.Math.Log(Abs(value)), System.Math.Atan2(value._imag, value._real));
		public static Complex Log(Complex value, Complex logBase) => Log(value) / Log(logBase);
		public static Complex Log(Complex value, double logBase) => Log(value) / System.Math.Log(logBase);
		const double invLog10 = 0.43429448190325176;
		public static Complex Log10(Complex value) => Log(value) * invLog10;

		public static Complex Exp(Complex value)
		{
			double exp = System.Math.Exp(value._real);
			return new Complex(exp * System.Math.Cos(value._imag), exp * System.Math.Sin(value._imag));
		}
	}
}
