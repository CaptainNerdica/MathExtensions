using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	internal static class BigIntHelpers
	{
		internal static readonly uint[] Int32Masks1Bit = new uint[32];
		internal static readonly ulong[] Int64Masks1Bit = new ulong[64];
		internal static readonly UInt128[] Int128Masks1Bit = new UInt128[128];

		static BigIntHelpers()
		{
			for (int i = 0; i < Int32Masks1Bit.Length; ++i)
				Int32Masks1Bit[i] = 1U << i;
			for (int i = 0; i < Int64Masks1Bit.Length; ++i)
				Int64Masks1Bit[i] = 1UL << i;
			for (int i = 0; i < Int128Masks1Bit.Length; ++i)
				Int128Masks1Bit[i] = UInt128.One << i;
		}
	}
}
