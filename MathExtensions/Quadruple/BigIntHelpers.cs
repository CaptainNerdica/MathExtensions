using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	internal static class BigIntHelpers
	{
		internal static readonly uint[] Int32Masks1Bit = new uint[32];
		internal static readonly ulong[] Int64Masks1Bit = new ulong[64];
		internal static readonly UInt128[] Int128Masks1Bit = new UInt128[128];
		internal static readonly UInt128[] Int128MasksLowBits = new UInt128[128];
		internal static readonly UInt128[] Int128MasksHighBits = new UInt128[128];
		internal static readonly UInt256[] Int256Masks1Bit = new UInt256[256];

		static BigIntHelpers()
		{
			for (int i = 0; i < Int32Masks1Bit.Length; ++i)
				Int32Masks1Bit[i] = 1U << i;
			for (int i = 0; i < Int64Masks1Bit.Length; ++i)
				Int64Masks1Bit[i] = 1UL << i;
			for (int i = 0; i < Int128Masks1Bit.Length; ++i)
				Int128Masks1Bit[i] = (UInt128)1 << i;
			for (int i = 0; i < Int128MasksLowBits.Length; ++i)
				Int128MasksLowBits[i] = Int128Masks1Bit[(i + 1) & 0x7F] - UInt128.One;
			for (int i = 0; i < Int128MasksHighBits.Length; ++i)
				Int128MasksHighBits[i] = ~Int128MasksLowBits[i];
			for (int i = 0; i < Int256Masks1Bit.Length; ++i)
				Int256Masks1Bit[i] = (UInt256)1 << i;
		}

		internal static int GetHighestBit(ulong value)
		{
			for (int i = sizeof(long) * 8 - 1; i >= 0; --i)
				if ((value & Int64Masks1Bit[i]) != 0)
					return i;
			return 0;
		}
		internal static int GetHighestBit(uint value)
		{
			for (int i = sizeof(int) * 8 - 1; i >= 0; --i)
				if ((value & Int32Masks1Bit[i]) != 0)
					return i;
			return 0;
		}

		public static UInt128 IntPower(UInt128 x, int power)
		{
			if (power == 0) return 1;
			if (power == 1) return x;

			int n = 31;
			while ((power <<= 1) >= 0) n--;

			UInt128 tmp = x;
			while (--n > 0)
				tmp = tmp * tmp *
					 (((power <<= 1) < 0) ? x : 1);
			return tmp;
		}
		public static UInt256 IntPower(UInt256 x, int power)
		{
			if (power == 0) return 1;
			if (power == 1) return x;

			int n = 31;
			while ((power <<= 1) >= 0) n--;

			UInt256 tmp = x;
			while (--n > 0)
				tmp = tmp * tmp *
					 (((power <<= 1) < 0) ? x : 1);
			return tmp;
		}

		private static unsafe uint AddDivisor(uint* left, int leftLength,
													  uint* right, int rightLength)
		{
			ulong carry = 0UL;

			for (int i = 0; i < rightLength; i++)
			{
				ulong digit = (left[i] + carry) + right[i];
				left[i] = unchecked((uint)digit);
				carry = digit >> 32;
			}

			return (uint)carry;
		}

		private static unsafe uint SubtractDivisor(uint* left, int leftLength,
												   uint* right, int rightLength,
												   ulong q)
		{

			ulong carry = 0UL;

			for (int i = 0; i < rightLength; i++)
			{
				carry += right[i] * q;
				uint digit = unchecked((uint)carry);
				carry >>= 32;
				if (left[i] < digit)
					++carry;
				left[i] = unchecked(left[i] - digit);
			}

			return (uint)carry;
		}

		private static bool DivideGuessTooBig(ulong q, ulong valHi, uint valLo,
											  uint divHi, uint divLo)
		{
			ulong chkHi = divHi * q;
			ulong chkLo = divLo * q;

			chkHi += (chkLo >> 32);
			chkLo &= 0xFFFFFFFF;

			if (chkHi < valHi)
				return false;
			if (chkHi > valHi)
				return true;

			if (chkLo < valLo)
				return false;
			if (chkLo > valLo)
				return true;

			return false;
		}

		private static int LeadingZeros(uint value)
		{
			if (value == 0)
				return 32;

			int count = 0;
			if ((value & 0xFFFF0000) == 0)
			{
				count += 16;
				value <<= 16;
			}
			if ((value & 0xFF000000) == 0)
			{
				count += 8;
				value <<= 8;
			}
			if ((value & 0xF0000000) == 0)
			{
				count += 4;
				value <<= 4;
			}
			if ((value & 0xC0000000) == 0)
			{
				count += 2;
				value <<= 2;
			}
			if ((value & 0x80000000) == 0)
			{
				count += 1;
			}

			return count;
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		internal static unsafe void Divide(uint* left, int leftLength, uint* right, int rightLength, uint* bits, int bitsLength)
		{
			uint divHi = right[rightLength - 1];
			uint divLo = rightLength > 1 ? right[rightLength - 2] : 0;

			int shift = LeadingZeros(divHi);
			int backShift = 32 - shift;

			if (shift > 0)
			{
				uint divNx = rightLength > 2 ? right[rightLength - 3] : 0;

				divHi = (divHi << shift) | (divLo >> backShift);
				divLo = (divLo << shift) | (divNx >> backShift);
			}

			for (int i = leftLength; i >= rightLength; i--)
			{
				int n = i - rightLength;
				uint t = i < leftLength ? left[i] : 0;

				ulong valHi = ((ulong)t << 32) | left[i - 1];
				uint valLo = i > 1 ? left[i - 2] : 0;

				if (shift > 0)
				{
					uint valNx = i > 2 ? left[i - 3] : 0;

					valHi = (valHi << shift) | (valLo >> backShift);
					valLo = (valLo << shift) | (valNx >> backShift);
				}

				ulong digit = valHi / divHi;
				if (digit > 0xFFFFFFFF)
					digit = 0xFFFFFFFF;

				while (DivideGuessTooBig(digit, valHi, valLo, divHi, divLo))
					--digit;

				if (digit > 0)
				{
					uint carry = SubtractDivisor(left + n, leftLength - n,
												 right, rightLength, digit);
					if (carry != t)
					{
						carry = AddDivisor(left + n, leftLength - n,
										   right, rightLength);
						--digit;
					}
				}

				if (bitsLength != 0)
					bits[n] = (uint)digit;
				if (i < leftLength)
					left[i] = 0;
			}
		}

		internal static unsafe void DivRem(uint* left, int leftLength, uint right, uint* bits, out uint remainder)
		{
			ulong carry = 0UL;
			for (int i = leftLength - 1; i >= 0; --i)
			{
				ulong value = (carry << 32) | left[i];
				ulong digit = value / right;
				bits[i] = (uint)digit;
				carry = value - digit * right;
			}
			remainder = (uint)carry;
		}

		internal static unsafe void Divide(uint* left, int leftLength, uint right, uint* bits)
		{
			ulong carry = 0UL;
			for (int i = leftLength - 1; i >= 0; --i)
			{
				ulong value = (carry << 32) | left[i];
				ulong digit = value / right;
				bits[i] = (uint)digit;
				carry = value - digit * right;
			}
		}

		internal static unsafe uint Remainder(uint* left, int leftLength, uint right)
		{
			ulong carry = 0UL;
			for (int i = leftLength - 1; i >= 0; --i)
			{
				ulong value = (carry << 32) | left[i];
				carry = value % right;
			}
			return (uint)carry;
		}
	}
}
