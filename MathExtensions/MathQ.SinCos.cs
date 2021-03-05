using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static MathExtensions.Quadruple;

namespace MathExtensions
{
	static unsafe partial class MathQ
	{
		private static class SinCosLookup
		{
			internal static readonly Quadruple IF2 = new Quadruple(0x3FFE_0000_0000_0000, 0x0000_0000_0000_0000); //	1 / 2!
			internal static readonly Quadruple IF3 = new Quadruple(0x3FFC_5555_5555_5555, 0x5555_5555_5555_5556); //	1 / 3!
			internal static readonly Quadruple IF4 = new Quadruple(0x3FFA_5555_5555_5555, 0x5555_5555_5555_5556); //	1 / 4!
			internal static readonly Quadruple IF5 = new Quadruple(0x3FF8_1111_1111_1111, 0x1111_1111_1111_1112); //	1 / 5!
			internal static readonly Quadruple IF6 = new Quadruple(0x3FF5_6C16_C16C_16C1, 0x6C16_C16C_16C1_6C16); //	1 / 6!
			internal static readonly Quadruple IF7 = new Quadruple(0x3FF2_A01A_01A0_1A01, 0xA01A_01A0_1A01_A01A); //	1 / 7!
			internal static readonly Quadruple IF8 = new Quadruple(0x3FEF_A01A_01A0_1A01, 0xA01A_01A0_1A01_A01A); //	1 / 8!
		}

		public static partial Quadruple Sin(Quadruple x)
		{
			int sign = x.Sign;
			int exp = x.Exp;
			if (!IsFinite(x))
				return NaN;
			UInt128 u = AsUInt128(x);
			uint upper = u._u[3] & 0x7FFF_FFFF;
			if (upper < 0x3FD9_0000)
				return x;
			else if (upper < 0x3FFE_B600)
				return DoSin(x, Zero);
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Quadruple DoSin(Quadruple x, Quadruple dx)
		{

			return default;
		}

		private static Quadruple TaylorSin(Quadruple quadruple, Quadruple x, Quadruple dx)
		{
			throw new NotImplementedException();
		}
	}
}
