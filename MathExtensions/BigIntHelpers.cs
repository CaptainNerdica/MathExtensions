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
		internal static readonly uint[] IntMasks1Bit = new uint[32];
		internal static ulong[] LongMasks1Bit = new ulong[64];

		static BigIntHelpers()
		{
			for (int i = 0; i < IntMasks1Bit.Length; ++i)
				IntMasks1Bit[i] = 1U << i;
			for (int i = 0; i < LongMasks1Bit.Length; ++i)
				LongMasks1Bit[i] = 1UL << i;
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
			for(int i = leftLength - 1; i >= 0; --i)
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
