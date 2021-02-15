using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public unsafe partial struct Quadruple
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Equals(Quadruple left, Quadruple right) => !(IsNaN(left) || IsNaN(right)) && ((ulong*)left._bytes)[0] == ((ulong*)right._bytes)[0] & ((ulong*)left._bytes)[1] == ((ulong*)right._bytes)[1];
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetHashCode(Quadruple quad) => HashCode.Combine(((long*)quad._bytes)[0], ((long*)quad._bytes)[1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static short GetExp(Quadruple quad) => (short)(((short*)quad._bytes)[7] & 0x7fff);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Quadruple Multiply(Quadruple left, Quadruple right)
		{
			if (IsNaN(left) || IsNaN(right))
				return NaN;
			int sign = left._sign ^ right._sign;
			long exponent = left._exp + right._exp - 2 * Bias;
			UInt128 lMantissa = CopyMantissa(left);
			UInt128 rMantissa = CopyMantissa(right);


			return default;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static UInt128 CopyMantissa(Quadruple quad)
		{
			const long mask = (1 << 48) - 1;
			const long normalBit = 1 << 48;
			ulong* p = (ulong*)quad._bytes;
			ulong lo = p[0];
			ulong hi = p[1] & mask;
			if (!IsSubnormal(quad))
				hi |= normalBit;
			return new UInt128(hi, lo);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static UInt128 AsUInt128(Quadruple quad) => new UInt128(quad._bytes, (uint)sizeof(Quadruple));
	}
}
