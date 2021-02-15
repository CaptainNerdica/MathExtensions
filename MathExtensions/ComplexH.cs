using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
#if NET5_0
namespace MathExtensions
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0032:Use auto property", Justification = "<Pending>")]
	public readonly struct ComplexH
	{
		private readonly Half _real;
		private readonly Half _imag;

		public static readonly ComplexH ImaginaryOne = new ComplexH((Half)0, (Half)1);
		public static readonly ComplexH Infinity = new ComplexH(Half.PositiveInfinity, Half.PositiveInfinity);
		public static readonly ComplexH NaN = new ComplexH(Half.NaN, Half.NaN);
		public static readonly ComplexH One = new ComplexH((Half)1, (Half)0);
		public static readonly ComplexH Zero = new ComplexH((Half)0, (Half)0);

		public Half Magnitude => Abs(this);
		public Half Imaginary => _imag;
		public Half Phase => (Half)MathF.Atan2((float)_imag, (float)_real);
		public Half Real => _real;

		private ComplexH(float real, float imaginary)
		{
			_real = (Half)real;
			_imag = (Half)imaginary;
		}
		public ComplexH(Half real, Half imaginary)
		{
			_real = real;
			_imag = imaginary;
		}

		public static bool IsFinite(ComplexH value) => Half.IsFinite(value._real) && Half.IsFinite(value._imag);
		public static bool IsInfinite(ComplexH value) => Half.IsInfinity(value._real) || Half.IsInfinity(value._imag);
		public static bool IsNaN(ComplexH value) => Half.IsNaN(value._real) || Half.IsNaN(value._imag);

		public static Half Abs(ComplexH value) => (Half)MathF.Sqrt((float)value._real * (float)value._real + (float)value._imag * (float)value._imag);
		public static ComplexH Acos(ComplexH value) => -ImaginaryOne * Log(ImaginaryOne * Sqrt(1 - value * value) - value);
		public static ComplexH Acosh(ComplexH value) => Log(value + Sqrt(value * value - 1));
		public static ComplexH Acot(ComplexH value) => Atan(Reciprocal(value));
		public static ComplexH Acoth(ComplexH value) => 0.5f * Log((value + 1) / (value - 1));
		public static ComplexH Acsc(ComplexH value) => Asin(Reciprocal(value));
		public static ComplexH Acsch(ComplexH value) => Log(Reciprocal(value) + Sqrt(Reciprocal(value * value) + 1));
		public static ComplexH Add(ComplexH left, ComplexH right) => new ComplexH((float)left._real + (float)right._real, (float)left._imag + (float)right._imag);
		public static ComplexH Add(ComplexH left, Half right) => new ComplexH((float)left._real + (float)right, (float)left._imag);
		public static ComplexH Add(Half left, ComplexH right) => new ComplexH((float)left + (float)right._real, (float)right._imag);
		public static ComplexH Asec(ComplexH value) => Acos(Reciprocal(value));
		public static ComplexH Asech(ComplexH value) => Log(Reciprocal(value) + Sqrt(Reciprocal(value) + 1) * Sqrt(Reciprocal(value) - 1));
		public static ComplexH Asin(ComplexH value) => ImaginaryOne * Log(Sqrt(1 - value * value) - ImaginaryOne * value);
		public static ComplexH Asinh(ComplexH value) => Log(value + Sqrt(value * value + 1));
		public static ComplexH Atan(ComplexH value) => -ImaginaryOne / 2 * Log((value + ImaginaryOne) / (value - ImaginaryOne));
		public static ComplexH Atanh(ComplexH value) => 0.5f * Log((1 + value) / (1 - value));
		public static ComplexH Conjugate(ComplexH value) => new ComplexH((float)value._real, -(float)value._imag);
		public static ComplexH Cos(ComplexH value) => new ComplexH(MathF.Cos((float)value._real) * MathF.Cosh((float)value._imag), MathF.Sin((float)value._real) * MathF.Sinh((float)value._imag));
		public static ComplexH Cosh(ComplexH value) => new ComplexH(MathF.Cosh((float)value._real) * MathF.Cos((float)value._imag), MathF.Sinh((float)value._real) * MathF.Sin((float)value._imag));
		public static ComplexH Coth(ComplexH value) => ImaginaryOne * Cot(ImaginaryOne * value);
		public static ComplexH Cot(ComplexH value)
		{
			float sinReal = MathF.Sin((float)value._real);
			float sinhImag = MathF.Sinh((float)value._imag);
			float divisor = sinReal * sinReal + sinhImag * sinhImag;
			return new ComplexH(sinReal * MathF.Cos((float)value._real) / divisor, -sinhImag * MathF.Cosh((float)value._imag) / divisor);
		}
		public static ComplexH Csc(ComplexH value) => Reciprocal(Sin(value));
		public static ComplexH Csch(ComplexH value) => ImaginaryOne * Csc(ImaginaryOne * value);
		public static ComplexH Divide(ComplexH dividend, ComplexH divisor)
		{
			float sqrMag = (float)divisor._real * (float)divisor._real + (float)divisor._imag * (float)divisor._imag;
			return new ComplexH(((float)divisor._real * (float)dividend._real + (float)divisor._imag * (float)dividend._imag) / sqrMag, ((float)divisor._imag * (float)dividend._real - (float)divisor._real * (float)dividend._imag) / sqrMag);
		}
		public static ComplexH Divide(ComplexH dividend, Half divisor) => new ComplexH((float)dividend._real / (float)divisor, (float)dividend._imag / (float)divisor);
		public static ComplexH Divide(Half dividend, ComplexH divisor)
		{
			float sqrMag = (float)divisor._real * (float)divisor._real + (float)divisor._imag * (float)divisor._imag;
			return new ComplexH((float)dividend * (float)divisor._real / sqrMag, (float)dividend * (float)divisor._imag / sqrMag);
		}
		public static ComplexH Exp(ComplexH value)
		{
			float exp = MathF.Exp((float)value._real);
			return new ComplexH(exp * MathF.Cos((float)value._imag), exp * MathF.Sin((float)value._imag));
		}
		public static ComplexH FromPolarCoordinates(Half magnitude, Half phase) => magnitude * Exp(ImaginaryOne * phase);
		public static ComplexH Log(ComplexH value) => new ComplexH(MathF.Log((float)value.Magnitude), (float)value.Phase);
		public static ComplexH Log(ComplexH value, Half baseValue) => Log(value) / MathF.Log((float)baseValue);
		public static ComplexH Log(ComplexH value, ComplexH baseValue) => Log(value) / Log(baseValue);
		public static ComplexH Log10(ComplexH value) => Log(value) / MathF.Log(10);
		public static ComplexH Multiply(ComplexH left, ComplexH right) => new ComplexH((float)left._real * (float)right._real - (float)left._imag * (float)right._imag, (float)left._real * (float)right._imag + (float)left._imag * (float)right._real);
		public static ComplexH Multiply(ComplexH left, Half right) => new ComplexH((float)left._real * (float)right, (float)left._imag * (float)right);
		public static ComplexH Multiply(Half left, ComplexH right) => new ComplexH((float)right._real * (float)left, (float)right._imag * (float)left);
		public static ComplexH Negate(ComplexH value) => new ComplexH(-(float)value._real, -(float)value._imag);
		public static ComplexH Pow(ComplexH value, ComplexH power) => Pow(value.Magnitude, power) * Exp(ImaginaryOne * power * value.Phase);
		public static ComplexH Pow(ComplexH value, Half power) => MathF.Pow((float)value.Magnitude, (float)power) * new ComplexH(MathF.Cos((float)value.Phase * (float)power), MathF.Sin((float)value.Phase * (float)power));
		public static ComplexH Pow(Half value, ComplexH power) => Exp(power * MathF.Log((float)value));
		public static ComplexH Reciprocal(ComplexH value)
		{
			float sqrMag = (float)value._real * (float)value._real + (float)value._imag * (float)value._imag;
			return new ComplexH((float)value._real / sqrMag, (float)value._imag / sqrMag);
		}
		public static ComplexH Sec(ComplexH value) => Reciprocal(Cos(value));
		public static ComplexH Sech(ComplexH value) => Sech(ImaginaryOne * value);
		public static ComplexH Sin(ComplexH value) => new ComplexH(MathF.Sin((float)value._real) * MathF.Cosh((float)value._imag), MathF.Cos((float)value._real) * MathF.Sinh((float)value._imag));
		public static ComplexH Sinh(ComplexH value) => new ComplexH(MathF.Sinh((float)value._real) * MathF.Cos((float)value._imag), MathF.Cosh((float)value._real) * MathF.Sin((float)value._imag));
		public static ComplexH Subtract(ComplexH left, ComplexH right) => new ComplexH((float)left._real - (float)right._real, (float)left._imag - (float)right._imag);
		public static ComplexH Subtract(ComplexH left, Half right) => new ComplexH((float)left._real - (float)right, (float)left._imag);
		public static ComplexH Subtract(Half left, ComplexH right) => new ComplexH((float)left - (float)right._real, -(float)right._imag);
		public static ComplexH Sqrt(ComplexH value) => new ComplexH(MathF.Sqrt(((float)value._real + (float)Abs(value)) / 2), MathF.Sign((float)value._imag) * MathF.Sqrt((-(float)value._real + (float)Abs(value)) / 2));
		public static ComplexH Tan(ComplexH value)
		{
			float cosReal = MathF.Cos((float)value._real);
			float sinhImag = MathF.Sinh((float)value._imag);
			float divisor = cosReal * cosReal + sinhImag * sinhImag;
			return new ComplexH(MathF.Sin((float)value._real) * cosReal / divisor, sinhImag * MathF.Cosh((float)value._imag) / divisor);
		}
		public static ComplexH Tanh(ComplexH value) => -ImaginaryOne * Tan(ImaginaryOne * value);

		public override bool Equals(object? obj) => base.Equals(obj);
		public bool Equals(ComplexH value) => _real == value._real && _imag == value._imag;
		public override int GetHashCode() => HashCode.Combine(_real, _imag);
		public override string ToString() => string.Format("({0}, {1})", _real, _imag);


		public static ComplexH operator +(ComplexH value) => value;
		public static ComplexH operator -(ComplexH value) => Negate(value);
		public static ComplexH operator +(ComplexH left, ComplexH right) => Add(left, right);
		public static ComplexH operator +(ComplexH left, Half right) => Add(left, right);
		public static ComplexH operator +(Half left, ComplexH right) => Add(left, right);
		public static ComplexH operator -(ComplexH left, ComplexH right) => Subtract(left, right);
		public static ComplexH operator -(ComplexH left, Half right) => Subtract(left, right);
		public static ComplexH operator -(Half left, ComplexH right) => Subtract(left, right);
		public static ComplexH operator *(ComplexH left, ComplexH right) => Multiply(left, right);
		public static ComplexH operator *(ComplexH left, Half right) => Multiply(left, right);
		public static ComplexH operator *(Half left, ComplexH right) => Multiply(left, right);
		public static ComplexH operator /(ComplexH left, ComplexH right) => Divide(left, right);
		public static ComplexH operator /(ComplexH left, Half right) => Divide(left, right);
		public static ComplexH operator /(Half left, ComplexH right) => Divide(left, right);
		public static bool operator ==(ComplexH left, ComplexH right) => left.Equals(right);
		public static bool operator !=(ComplexH left, ComplexH right) => !left.Equals(right);

		public static implicit operator ComplexH(ulong value) => new ComplexH((Half)value, (Half)0);
		public static implicit operator ComplexH(uint value) => new ComplexH((Half)value, (Half)0);
		public static implicit operator ComplexH(ushort value) => new ComplexH((Half)value, (Half)0);
		public static implicit operator ComplexH(float value) => new ComplexH((Half)value, (Half)0);
		public static implicit operator ComplexH(sbyte value) => new ComplexH((Half)value, (Half)0);
		public static implicit operator ComplexH(long value) => new ComplexH((Half)value, (Half)0);
		public static implicit operator ComplexH(int value) => new ComplexH((Half)value, (Half)0);
		public static implicit operator ComplexH(short value) => new ComplexH((Half)value, (Half)0);
		public static implicit operator ComplexH(byte value) => new ComplexH((Half)value, (Half)0);
		public static explicit operator ComplexH(double value) => new ComplexH((Half)value, (Half)0);
		public static explicit operator ComplexH(decimal value) => new ComplexH((Half)(float)value, (Half)0);
		public static explicit operator ComplexH(Complex value) => new ComplexH((Half)value.Real, (Half)value.Imaginary);
		public static explicit operator ComplexH(ComplexF value) => new ComplexH((Half)value.Real, (Half)value.Imaginary);
		public static implicit operator ComplexF(ComplexH value) => new ComplexF((float)value._real, (float)value._imag);
		public static implicit operator Complex(ComplexH value) => new Complex((double)value._real, (double)value._imag);
	}
}
#endif
