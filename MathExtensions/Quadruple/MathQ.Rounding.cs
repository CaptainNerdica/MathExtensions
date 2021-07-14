using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MathExtensions.Quadruple;

namespace MathExtensions
{
	partial class MathQ
	{
		private const int maxRoundingDigits = 33;
		private static readonly Quadruple quadrupleRoundingLimit = new Quadruple(0x406F_ED09_BEAD_87C0, 0x378D_8E64_1600_0000);
		private static readonly Quadruple[] roundPower10Quadruple = new Quadruple[]
		{
			new Quadruple(0x3FFF_0000_0000_0000, 0x0000_0000_0000_0000), //	1E0
			new Quadruple(0x4002_4000_0000_0000, 0x0000_0000_0000_0000), //	1E1
			new Quadruple(0x4005_9000_0000_0000, 0x0000_0000_0000_0000), //	1E2
			new Quadruple(0x4008_F400_0000_0000, 0x0000_0000_0000_0000), //	1E3
			new Quadruple(0x400C_3880_0000_0000, 0x0000_0000_0000_0000), //	1E4
			new Quadruple(0x400F_86A0_0000_0000, 0x0000_0000_0000_0000), //	1E5
			new Quadruple(0x4012_E848_0000_0000, 0x0000_0000_0000_0000), //	1E6
			new Quadruple(0x4016_312D_0000_0000, 0x0000_0000_0000_0000), //	1E7
			new Quadruple(0x4019_7D78_4000_0000, 0x0000_0000_0000_0000), //	1E8
			new Quadruple(0x401C_DCD6_5000_0000, 0x0000_0000_0000_0000), //	1E9
			new Quadruple(0x4020_2A05_F200_0000, 0x0000_0000_0000_0000), //	1E10
			new Quadruple(0x4023_7487_6E80_0000, 0x0000_0000_0000_0000), //	1E11
			new Quadruple(0x4026_D1A9_4A20_0000, 0x0000_0000_0000_0000), //	1E12
			new Quadruple(0x402A_2309_CE54_0000, 0x0000_0000_0000_0000), //	1E13
			new Quadruple(0x402D_6BCC_41E9_0000, 0x0000_0000_0000_0000), //	1E14
			new Quadruple(0x4030_C6BF_5263_4000, 0x0000_0000_0000_0000), //	1E15
			new Quadruple(0x4034_1C37_937E_0800, 0x0000_0000_0000_0000), //	1E16
			new Quadruple(0x4037_6345_785D_8A00, 0x0000_0000_0000_0000), //	1E17
			new Quadruple(0x403A_BC16_D674_EC80, 0x0000_0000_0000_0000), //	1E18
			new Quadruple(0x403E_158E_4609_13D0, 0x0000_0000_0000_0000), //	1E19
			new Quadruple(0x4041_5AF1_D78B_58C4, 0x0000_0000_0000_0000), //	1E20
			new Quadruple(0x4044_B1AE_4D6E_2EF5, 0x0000_0000_0000_0000), //	1E21
			new Quadruple(0x4048_0F0C_F064_DD59, 0x2000_0000_0000_0000), //	1E22
			new Quadruple(0x404B_52D0_2C7E_14AF, 0x6800_0000_0000_0000), //	1E23
			new Quadruple(0x404E_A784_379D_99DB, 0x4200_0000_0000_0000), //	1E24
			new Quadruple(0x4052_08B2_A2C2_8029, 0x0940_0000_0000_0000), //	1E25
			new Quadruple(0x4055_4ADF_4B73_2033, 0x4B90_0000_0000_0000), //	1E26
			new Quadruple(0x4058_9D97_1E4F_E840, 0x1E74_0000_0000_0000), //	1E27
			new Quadruple(0x405C_027E_72F1_F128, 0x1308_8000_0000_0000), //	1E28
			new Quadruple(0x405F_431E_0FAE_6D72, 0x17CA_A000_0000_0000), //	1E29
			new Quadruple(0x4062_93E5_939A_08CE, 0x9DBD_4800_0000_0000), //	1E30
			new Quadruple(0x4065_F8DE_F880_8B02, 0x452C_9A00_0000_0000), //	1E31
			new Quadruple(0x4069_3B8B_5B50_56E1, 0x6B3B_E040_0000_0000), //	1E32
			new Quadruple(0x406C_8A6E_3224_6C99, 0xC60A_D850_0000_0000), //	1E33
		};

		public static unsafe partial Quadruple Ceiling(Quadruple value)
		{
			ref UInt128 i0 = ref Unsafe.As<Quadruple, UInt128>(ref value);
			UInt128 i;
			int j0;
			j0 = value.Exp - Bias;
			int sign = value.Sign;
			if (j0 <= SignificandBits - 1)
			{
				if (j0 < 0)
				{
					if (value.Sign == 1)
						return NegativeZero;
					else if (i0 != default)
						return One;
				}
				else
				{
					i = BigIntHelpers.Int128MasksLowBits[SignificandBits - j0 - 1];
					if ((i0 & i) == default)
						return value;
					if (sign == 0)
						i0 += BigIntHelpers.Int128Masks1Bit[SignificandBits - j0];
					i0 &= ~i;
					return value;
				}
			}
			else
			{
				if (j0 == 0x4000)
					return value + value;
				else
					return value;
			}
			return value;
		}
		public static unsafe partial Quadruple Floor(Quadruple value)
		{
			ref UInt128 i0 = ref Unsafe.As<Quadruple, UInt128>(ref value);
			int j0 = value.Exp - Bias;
			int sign = value.Sign;
			if (j0 < SignificandBits)
			{
				if (j0 < 0)
				{
					if (sign == 0)
						return default;
					else if ((i0 & BigIntHelpers.Int128MasksLowBits[127]) != UInt128.Zero)
						return MinusOne;
				}
				else
				{
					UInt128 i = BigIntHelpers.Int128MasksLowBits[SignificandBits - j0 - 1];
					if ((i0 & i) == default)
						return value;
					if (sign == 1)
						i0 += BigIntHelpers.Int128Masks1Bit[SignificandBits - j0];
					i0 &= ~i;
					return value;
				}
			}
			else if (j0 == 0x4000)
				return value + value;
			return value;
		}

		public static unsafe partial Quadruple Round(Quadruple value)
		{
			ref UInt128 i0 = ref Unsafe.As<Quadruple, UInt128>(ref value);
			int j0 = value.Exp - Bias;
			int sign = value.Sign;
			if (j0 < SignificandBits)
			{
				if (j0 < 0)
				{
					if (j0 == -1)
					{
						if (sign == 0)
							return One;
						else
							return MinusOne;
					}
					else
					{
						if (sign == 0)
							return default;
						else
							return NegativeZero;
					}
				}
				else
				{
					UInt128 i = BigIntHelpers.Int128MasksLowBits[SignificandBits - j0 - 1];
					if ((i0 & i) == default)
						return value;
					i0 += BigIntHelpers.Int128Masks1Bit[SignificandBits - j0 - 1];
					i0 &= ~i;
				}
			}
			else
			{
				if (j0 == 0x4000)
					return value + value;
				else
					return value;
			}
			return value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static partial Quadruple Round(Quadruple value, int digits) => Round(value, digits, MidpointRounding.ToEven);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static partial Quadruple Round(Quadruple value, MidpointRounding mode) => Round(value, 0, mode);
		public static unsafe partial Quadruple Round(Quadruple value, int digits, MidpointRounding mode)
		{
			if ((digits < 0) || (digits > maxRoundingDigits))
				throw new ArgumentOutOfRangeException(nameof(digits), "Rounding digits must be between 0 and 33");
			if (mode < MidpointRounding.ToEven || mode > MidpointRounding.ToPositiveInfinity)
				throw new ArgumentException(string.Format("The value '{0}' is not valid for this usage of the type {1}.", mode, nameof(MidpointRounding)), nameof(mode));
			if (Abs(value) < quadrupleRoundingLimit)
			{
				Quadruple power10 = roundPower10Quadruple[digits];

				value *= power10;

				switch (mode)
				{
					case MidpointRounding.ToEven:
						value = Round(value);
						break;
					case MidpointRounding.AwayFromZero:
						Quadruple fraction = ModF(value, &value);
						if (Abs(fraction) >= HALF)
							value += Sign(fraction);
						break;
					case MidpointRounding.ToZero:
						value = Truncate(value);
						break;
					case MidpointRounding.ToNegativeInfinity:
						value = Floor(value);
						break;
					case MidpointRounding.ToPositiveInfinity:
						value = Ceiling(value);
						break;
					default:
						throw new ArgumentException(string.Format("The value '{0}' is not valid for this usage of the type {1}.", mode, nameof(MidpointRounding)), nameof(mode));
				}

				value /= power10;
			}
			return value;
		}

		public static unsafe partial Quadruple Truncate(Quadruple value)
		{
			if (!IsFinite(value))
				return value;
			int exp = value.Exp - Bias;
			if (exp > SignificandBits)
				return value;
			if (exp < 0)
				return default;
			UInt128 mask = BigIntHelpers.Int128MasksHighBits[SignificandBits - exp - 1];
			UInt128 s = ((QuadUnion*)&value)->i;
			s &= mask;
			return ((QuadUnion*)&s)->x;
		}


		private static unsafe Quadruple ModF(Quadruple x, Quadruple* iptr)
		{
			UInt128* i0 = (UInt128*)&x;
			int j0 = x.Exp - Bias;
			int sign = x.Sign;
			if (j0 < SignificandBits)
			{
				if (j0 < 0)
				{
					*iptr = x;
					*i0 &= BigIntHelpers.Int128Masks1Bit[127];
					return x;
				}
				else
				{
					UInt128 i = BigIntHelpers.Int128MasksLowBits[SignificandBits - j0 - 1];
					if ((*i0 & i) == default)
					{
						*iptr = x;
						*i0 &= BigIntHelpers.Int128Masks1Bit[127];
						return x;
					}
					else
					{
						
						*iptr = ((QuadUnion)(*i0 & ~i)).x;
						return x - *iptr;
					}
				}
			}
			else
			{
				*iptr = x * One;
				if (j0 == 0x4000 && (*i0 & FractionMask) != default)
					return x * One;
				*i0 &= BigIntHelpers.Int128Masks1Bit[127];
				return x;
			}
		}
	}
}
