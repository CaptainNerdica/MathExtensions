using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static MathExtensions.Quadruple;

namespace MathExtensions
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Implementation not completed")]
	public static unsafe partial class MathQ
	{
		public static readonly Quadruple PI =			new Quadruple(0x4000_921F_B544_42D1, 0x8469_898C_C517_0B18);
		public static readonly Quadruple HALF_PI =		new Quadruple(0x3FFF_921F_B544_42D1, 0x8469_898C_C517_01B8);
		public static readonly Quadruple TAU =			new Quadruple(0x4001_921F_B544_42D1, 0x8469_898C_C517_0B18);
		public static readonly Quadruple E =			new Quadruple(0x4000_5BF0_A8B1_4576, 0x9535_5FB8_AC40_4E7A);
		public static readonly Quadruple SQRT2 =		new Quadruple(0x3FFF_6A09_E667_F3BC, 0xC908_B2FB_1366_EA95);
		internal static readonly Quadruple HALF =		new Quadruple(0x3FFE_0000_0000_0000, 0x0000_0000_0000_0000);

		/// <summary>
		/// Returns the absolute value of a quadruple-precision floating-point number.
		/// </summary>
		/// <param name="x"> A number that is greater than or equal to <c>Quadruple.MinValue</c>, but less than or equal to <c>Quadruple.MaxValue</c>.</param>
		/// <returns>A quadruple-precision floating-point number, x, such that 0 ≤ x ≤ S<c>Quadruple.MaxValue</c></returns>
		public static Quadruple Abs(Quadruple x) => IsNaN(x) ? x : IsNegative(x) ? -x : x;
		public static Quadruple Acos(Quadruple x) => throw new NotImplementedException();
		public static Quadruple Acosh(Quadruple x) => throw new NotImplementedException();
		public static Quadruple Asin(Quadruple x) => throw new NotImplementedException();
		public static Quadruple Asinh(Quadruple x) => throw new NotImplementedException();
		public static Quadruple Atan(Quadruple x) => throw new NotImplementedException();
		public static Quadruple Atan2(Quadruple x, Quadruple y) => throw new NotImplementedException();
		public static Quadruple Atanh(Quadruple x) => throw new NotImplementedException();
		public static Quadruple BitIncrement(Quadruple x)
		{
			if (IsNaN(x))
				return NaN;
			if (IsPositiveInfinity(x))
				return PositiveInfinity;
			UInt128 u = AsUInt128(x);
			if (IsPositive(x))
				u += 1;
			else if (x == NegativeZero)
				u = UInt128.One;
			else
				u -= 1;
			return FromUInt128(u);
		}
		public static Quadruple BitDecrement(Quadruple x)
		{
			if (IsNaN(x))
				return NaN;
			if (IsNegativeInfinity(x))
				return NegativeInfinity;
			UInt128 u = AsUInt128(x);
			if (u == UInt128.Zero)
				u = new UInt128(1, 0, 0, unchecked((uint)int.MinValue));
			else if (IsPositive(x))
				u -= 1;
			else
				u += 1;
			return FromUInt128(u);
		}
		public static Quadruple Cbrt(Quadruple x) => throw new NotImplementedException();
		public static partial Quadruple Ceiling(Quadruple x);
		public static Quadruple CopySign(Quadruple x, Quadruple y)
		{
			QuadUnion* r = (QuadUnion*)&x;
			r->i._u[3] = (r->i._u[3] & 0x7FFF_FFFF) | (y._b[3] & 0x8000_0000);
			return r->x;
		}
		public static Quadruple Cos(Quadruple x) => throw new NotImplementedException();
		public static Quadruple Cosh(Quadruple x) => throw new NotImplementedException();
		public static partial Quadruple Exp(Quadruple x);
		public static partial Quadruple Floor(Quadruple x);
		public static partial Quadruple FusedMultiplyAdd(Quadruple x, Quadruple y, Quadruple z);
		public static Quadruple IEEERemainder(Quadruple x, Quadruple y) => throw new NotImplementedException();
		public static int ILogB(Quadruple x)
		{
			if (x == Zero)
				return int.MinValue;
			if (!IsFinite(x))
				return int.MaxValue;
			if (IsSubnormal(x))
			{
				UInt128 u = GetSignificand(x);
				int bit = 112 - UInt128.GetHighestBit(u);
				return 1 - bit - Bias;
			}
			else
				return x.Exp - Bias;
		}
		public static Quadruple InverseSquareRoot(Quadruple x) => throw new NotImplementedException();
		public static Quadruple Log(Quadruple x, Quadruple y) => throw new NotImplementedException();
		public static Quadruple Log(Quadruple x) => throw new NotImplementedException();
		public static Quadruple Log10(Quadruple x) => throw new NotImplementedException();
		public static Quadruple Log2(Quadruple x) => throw new NotImplementedException();
		public static Quadruple Max(Quadruple x, Quadruple y) => throw new NotImplementedException();
		public static Quadruple MaxMagnitude(Quadruple x, Quadruple y) => throw new NotImplementedException();
		public static Quadruple Min(Quadruple x, Quadruple y) => throw new NotImplementedException();
		public static Quadruple MinMagnitude(Quadruple x, Quadruple y) => throw new NotImplementedException();
		public static Quadruple Pow(Quadruple x, Quadruple y) => throw new NotImplementedException();
		//public static Quadruple ReciprocalEstimate(Quadruple x) => throw new NotImplementedException();
		//public static Quadruple ReciprocalSqrtEstimate(Quadruple x) => throw new NotImplementedException();
		public static partial Quadruple Round(Quadruple x, MidpointRounding mode);
		public static partial Quadruple Round(Quadruple x, int digits, MidpointRounding mode);
		public static partial Quadruple Round(Quadruple x, int digits);
		public static partial Quadruple Round(Quadruple x);
		public static Quadruple ScaleB(Quadruple x, int n) => throw new NotImplementedException();
		public static int Sign(Quadruple x) => IsZero(x) ? 0 : (IsNegative(x) ? -1 : 1);
		public static partial Quadruple Sin(Quadruple x);
		public static (Quadruple Sin, Quadruple Cos) SinCos(Quadruple x) => throw new NotImplementedException();
		public static Quadruple Sinh(Quadruple x) => throw new NotImplementedException();
		public static partial Quadruple Sqrt(Quadruple x);
		public static partial UInt128 ISqrt(UInt128 x);
		public static partial UInt256 ISqrt(UInt256 x);
		public static Quadruple Tan(Quadruple x) => throw new NotImplementedException();
		public static Quadruple Tanh(Quadruple x) => throw new NotImplementedException();
		public static unsafe partial Quadruple Truncate(Quadruple x);
	}
}
