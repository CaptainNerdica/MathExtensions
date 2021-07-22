﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions.Internal
{
	public unsafe ref partial struct BigInteger
	{
		// The longest binary mantissa requires: explicit mantissa bits + abs(min exponent)
		// * Half:     10 +    14 =    24
		// * Single:   23 +   126 =   149
		// * Double:   52 +  1022 =  1074
		// * Quad:    112 + 16382 = 16494
		internal const int BitsForLongestBinaryMantissa = 16494;

		// The longest digit sequence requires: ceil(log2(pow(10, max significant digits + 1 rounding digit)))
		// * Half:    ceil(log2(pow(10,    21 + 1))) =    74
		// * Single:  ceil(log2(pow(10,   112 + 1))) =   376
		// * Double:  ceil(log2(pow(10,   767 + 1))) =  2552
		// * Quad:    ceil(log2(pow(10, 11563 + 1))) = 38415
		internal const int BitsForLongestDigitSequence = 38415;

		// We require BitsPerBlock additional bits for shift space used during the pre-division preparation
		internal const int MaxBits = BitsForLongestBinaryMantissa + BitsForLongestDigitSequence + BitsPerBlock;

		internal const int BitsPerBlock = sizeof(int) * 8;
		internal const int MaxBlockCount = (MaxBits + (BitsPerBlock - 1)) / BitsPerBlock;

		private static readonly uint[] _pow10UInt32Table = new uint[]
		{
				1,          // 10^0
				10,         // 10^1
				100,        // 10^2
				1000,       // 10^3
				10000,      // 10^4
				100000,     // 10^5
				1000000,    // 10^6
				10000000,   // 10^7
				// These last two are accessed only by MultiplyPow10.
				100000000,  // 10^8
				1000000000  // 10^9
		};

		private static readonly int[] _pow10BigNumTableIndices = new int[]
		{
				0,          // 10^8
				2,          // 10^16
				5,          // 10^32
				10,         // 10^64
				18,         // 10^128
				33,         // 10^256
				61,         // 10^512
				116,        // 10^1024
				224,		// 10^2048
				438,		// 10^4096
				865,		// 10^8192
				1717,		// 10^16384
		};

		private int _length;
		private fixed uint _blocks[MaxBlockCount];

		public static void Add(ref BigInteger lhs, ref BigInteger rhs, out BigInteger result)
		{
			// determine which operand has the smaller length
			ref BigInteger large = ref (lhs._length < rhs._length) ? ref rhs : ref lhs;
			ref BigInteger small = ref (lhs._length < rhs._length) ? ref lhs : ref rhs;

			int largeLength = large._length;
			int smallLength = small._length;

			// The output will be at least as long as the largest input
			result._length = largeLength;

			// Add each block and add carry the overflow to the next block
			ulong carry = 0;

			int largeIndex = 0;
			int smallIndex = 0;
			int resultIndex = 0;

			while (smallIndex < smallLength)
			{
				ulong sum = carry + large._blocks[largeIndex] + small._blocks[smallIndex];
				carry = sum >> 32;
				result._blocks[resultIndex] = (uint)(sum);

				largeIndex++;
				smallIndex++;
				resultIndex++;
			}

			// Add the carry to any blocks that only exist in the large operand
			while (largeIndex < largeLength)
			{
				ulong sum = carry + large._blocks[largeIndex];
				carry = sum >> 32;
				result._blocks[resultIndex] = (uint)(sum);

				largeIndex++;
				resultIndex++;
			}

			// If there's still a carry, append a new block
			if (carry != 0)
			{
				Debug.Assert(carry == 1);
				Debug.Assert((resultIndex == largeLength) && (largeLength < MaxBlockCount));

				result._blocks[resultIndex] = 1;
				result._length++;
			}
		}

		public static int Compare(ref BigInteger lhs, ref BigInteger rhs)
		{
			Debug.Assert(unchecked((uint)(lhs._length)) <= MaxBlockCount);
			Debug.Assert(unchecked((uint)(rhs._length)) <= MaxBlockCount);

			int lhsLength = lhs._length;
			int rhsLength = rhs._length;

			int lengthDelta = (lhsLength - rhsLength);

			if (lengthDelta != 0)
			{
				return lengthDelta;
			}

			if (lhsLength == 0)
			{
				Debug.Assert(rhsLength == 0);
				return 0;
			}

			for (int index = (lhsLength - 1); index >= 0; index--)
			{
				long delta = (long)(lhs._blocks[index]) - rhs._blocks[index];

				if (delta != 0)
				{
					return delta > 0 ? 1 : -1;
				}
			}

			return 0;
		}

		public static uint CountSignificantBits(uint value) => 32 - (uint)BitOperations.LeadingZeroCount(value);

		public static uint CountSignificantBits(ulong value) => 64 - (uint)BitOperations.LeadingZeroCount(value);

		public static uint CountSignificantBits(ref BigInteger value)
		{
			if (value.IsZero())
			{
				return 0;
			}

			// We don't track any unused blocks, so we only need to do a BSR on the
			// last index and add that to the number of bits we skipped.

			uint lastIndex = (uint)(value._length - 1);
			return (lastIndex * BitsPerBlock) + CountSignificantBits(value._blocks[lastIndex]);
		}

		//public static void DivRem(ref BigInteger lhs, ref BigInteger rhs, out BigInteger quo, out BigInteger rem)
		//{
		//	// This is modified from the libraries BigIntegerCalculator.DivRem.cs implementation:
		//	// https://github.com/dotnet/runtime/blob/master/src/libraries/System.Runtime.Numerics/src/System/Numerics/BigIntegerCalculator.DivRem.cs

		//	Debug.Assert(!rhs.IsZero());

		//	if (lhs.IsZero())
		//	{
		//		SetZero(out quo);
		//		SetZero(out rem);
		//		return;
		//	}

		//	int lhsLength = lhs._length;
		//	int rhsLength = rhs._length;

		//	if ((lhsLength == 1) && (rhsLength == 1))
		//	{
		//		(uint quotient, uint remainder) = Math.DivRem(lhs._blocks[0], rhs._blocks[0]);
		//		SetUInt32(out quo, quotient);
		//		SetUInt32(out rem, remainder);
		//		return;
		//	}

		//	if (rhsLength == 1)
		//	{
		//		// We can make the computation much simpler if the rhs is only one block

		//		int quoLength = lhsLength;

		//		ulong rhsValue = rhs._blocks[0];
		//		ulong carry = 0;

		//		for (int i = quoLength - 1; i >= 0; i--)
		//		{
		//			ulong value = (carry << 32) | lhs._blocks[i];
		//			ulong digit;
		//			(digit, carry) = Math.DivRem(value, rhsValue);

		//			if ((digit == 0) && (i == (quoLength - 1)))
		//			{
		//				quoLength--;
		//			}
		//			else
		//			{
		//				quo._blocks[i] = (uint)(digit);
		//			}
		//		}

		//		quo._length = quoLength;
		//		SetUInt32(out rem, (uint)(carry));

		//		return;
		//	}
		//	else if (rhsLength > lhsLength)
		//	{
		//		// Handle the case where we have no quotient
		//		SetZero(out quo);
		//		SetValue(out rem, ref lhs);
		//		return;
		//	}
		//	else
		//	{
		//		int quoLength = lhsLength - rhsLength + 1;
		//		SetValue(out rem, ref lhs);
		//		int remLength = lhsLength;

		//		// Executes the "grammar-school" algorithm for computing q = a / b.
		//		// Before calculating q_i, we get more bits into the highest bit
		//		// block of the divisor. Thus, guessing digits of the quotient
		//		// will be more precise. Additionally we'll get r = a % b.

		//		uint divHi = rhs._blocks[rhsLength - 1];
		//		uint divLo = rhs._blocks[rhsLength - 2];

		//		// We measure the leading zeros of the divisor
		//		int shiftLeft = BitOperations.LeadingZeroCount(divHi);
		//		int shiftRight = 32 - shiftLeft;

		//		// And, we make sure the most significant bit is set
		//		if (shiftLeft > 0)
		//		{
		//			divHi = (divHi << shiftLeft) | (divLo >> shiftRight);
		//			divLo <<= shiftLeft;

		//			if (rhsLength > 2)
		//			{
		//				divLo |= (rhs._blocks[rhsLength - 3] >> shiftRight);
		//			}
		//		}

		//		// Then, we divide all of the bits as we would do it using
		//		// pen and paper: guessing the next digit, subtracting, ...
		//		for (int i = lhsLength; i >= rhsLength; i--)
		//		{
		//			int n = i - rhsLength;
		//			uint t = i < lhsLength ? rem._blocks[i] : 0;

		//			ulong valHi = ((ulong)(t) << 32) | rem._blocks[i - 1];
		//			uint valLo = i > 1 ? rem._blocks[i - 2] : 0;

		//			// We shifted the divisor, we shift the dividend too
		//			if (shiftLeft > 0)
		//			{
		//				valHi = (valHi << shiftLeft) | (valLo >> shiftRight);
		//				valLo <<= shiftLeft;

		//				if (i > 2)
		//				{
		//					valLo |= (rem._blocks[i - 3] >> shiftRight);
		//				}
		//			}

		//			// First guess for the current digit of the quotient,
		//			// which naturally must have only 32 bits...
		//			ulong digit = valHi / divHi;

		//			if (digit > uint.MaxValue)
		//			{
		//				digit = uint.MaxValue;
		//			}

		//			// Our first guess may be a little bit to big
		//			while (DivideGuessTooBig(digit, valHi, valLo, divHi, divLo))
		//			{
		//				digit--;
		//			}

		//			if (digit > 0)
		//			{
		//				// Now it's time to subtract our current quotient
		//				uint carry = SubtractDivisor(ref rem, n, ref rhs, digit);

		//				if (carry != t)
		//				{
		//					Debug.Assert(carry == t + 1);

		//					// Our guess was still exactly one too high
		//					carry = AddDivisor(ref rem, n, ref rhs);
		//					digit--;

		//					Debug.Assert(carry == 1);
		//				}
		//			}

		//			// We have the digit!
		//			if (quoLength != 0)
		//			{
		//				if ((digit == 0) && (n == (quoLength - 1)))
		//				{
		//					quoLength--;
		//				}
		//				else
		//				{
		//					quo._blocks[n] = (uint)(digit);
		//				}
		//			}

		//			if (i < remLength)
		//			{
		//				remLength--;
		//			}
		//		}

		//		quo._length = quoLength;

		//		// We need to check for the case where remainder is zero

		//		for (int i = remLength - 1; i >= 0; i--)
		//		{
		//			if (rem._blocks[i] == 0)
		//			{
		//				remLength--;
		//			}
		//		}

		//		rem._length = remLength;
		//	}
		//}

		public static uint HeuristicDivide(ref BigInteger dividend, ref BigInteger divisor)
		{
			int divisorLength = divisor._length;

			if (dividend._length < divisorLength)
			{
				return 0;
			}

			// This is an estimated quotient. Its error should be less than 2.
			// Reference inequality:
			// a/b - floor(floor(a)/(floor(b) + 1)) < 2
			int lastIndex = (divisorLength - 1);
			uint quotient = dividend._blocks[lastIndex] / (divisor._blocks[lastIndex] + 1);

			if (quotient != 0)
			{
				// Now we use our estimated quotient to update each block of dividend.
				// dividend = dividend - divisor * quotient
				int index = 0;

				ulong borrow = 0;
				ulong carry = 0;

				do
				{
					ulong product = ((ulong)(divisor._blocks[index]) * quotient) + carry;
					carry = product >> 32;

					ulong difference = (ulong)(dividend._blocks[index]) - (uint)(product) - borrow;
					borrow = (difference >> 32) & 1;

					dividend._blocks[index] = (uint)(difference);

					index++;
				}
				while (index < divisorLength);

				// Remove all leading zero blocks from dividend
				while ((divisorLength > 0) && (dividend._blocks[divisorLength - 1] == 0))
				{
					divisorLength--;
				}

				dividend._length = divisorLength;
			}

			// If the dividend is still larger than the divisor, we overshot our estimate quotient. To correct,
			// we increment the quotient and subtract one more divisor from the dividend (Because we guaranteed the error range).
			if (Compare(ref dividend, ref divisor) >= 0)
			{
				quotient++;

				// dividend = dividend - divisor
				int index = 0;
				ulong borrow = 0;

				do
				{
					ulong difference = (ulong)(dividend._blocks[index]) - divisor._blocks[index] - borrow;
					borrow = (difference >> 32) & 1;

					dividend._blocks[index] = (uint)(difference);

					index++;
				}
				while (index < divisorLength);

				// Remove all leading zero blocks from dividend
				while ((divisorLength > 0) && (dividend._blocks[divisorLength - 1] == 0))
				{
					divisorLength--;
				}

				dividend._length = divisorLength;
			}

			return quotient;
		}

		public static void Multiply(ref BigInteger lhs, uint value, out BigInteger result)
		{
			if (lhs._length <= 1)
			{
				SetUInt64(out result, (ulong)lhs.ToUInt32() * value);
				return;
			}

			if (value <= 1)
			{
				if (value == 0)
				{
					SetZero(out result);
				}
				else
				{
					SetValue(out result, ref lhs);
				}
				return;
			}

			int lhsLength = lhs._length;
			int index = 0;
			uint carry = 0;

			while (index < lhsLength)
			{
				ulong product = ((ulong)(lhs._blocks[index]) * value) + carry;
				result._blocks[index] = (uint)(product);
				carry = (uint)(product >> 32);

				index++;
			}

			if (carry != 0)
			{
				Debug.Assert(unchecked((uint)(lhsLength)) + 1 <= MaxBlockCount);
				result._blocks[index] = carry;
				result._length = (lhsLength + 1);
			}
			else
			{
				result._length = lhsLength;
			}
		}

		public static void Multiply(ref BigInteger lhs, ref BigInteger rhs, out BigInteger result)
		{
			if (lhs._length <= 1)
			{
				Multiply(ref rhs, lhs.ToUInt32(), out result);
				return;
			}

			if (rhs._length <= 1)
			{
				Multiply(ref lhs, rhs.ToUInt32(), out result);
				return;
			}

			ref readonly BigInteger large = ref lhs;
			int largeLength = lhs._length;

			ref readonly BigInteger small = ref rhs;
			int smallLength = rhs._length;

			if (largeLength < smallLength)
			{
				large = ref rhs;
				largeLength = rhs._length;

				small = ref lhs;
				smallLength = lhs._length;
			}

			int maxResultLength = smallLength + largeLength;
			Debug.Assert(unchecked((uint)(maxResultLength)) <= MaxBlockCount);

			// Zero out result internal blocks.
			result._length = maxResultLength;
			result.Clear((uint)maxResultLength);

			int smallIndex = 0;
			int resultStartIndex = 0;

			while (smallIndex < smallLength)
			{
				// Multiply each block of large BigNum.
				if (small._blocks[smallIndex] != 0)
				{
					int largeIndex = 0;
					int resultIndex = resultStartIndex;

					ulong carry = 0;

					do
					{
						ulong product = result._blocks[resultIndex] + ((ulong)(small._blocks[smallIndex]) * large._blocks[largeIndex]) + carry;
						carry = product >> 32;
						result._blocks[resultIndex] = (uint)(product);

						resultIndex++;
						largeIndex++;
					}
					while (largeIndex < largeLength);

					result._blocks[resultIndex] = (uint)(carry);
				}

				smallIndex++;
				resultStartIndex++;
			}

			if ((maxResultLength > 0) && (result._blocks[maxResultLength - 1] == 0))
			{
				result._length--;
			}
		}

		public static void Pow2(uint exponent, out BigInteger result)
		{
			uint blocksToShift = DivRem32(exponent, out uint remainingBitsToShift);
			result._length = (int)blocksToShift + 1;
			Debug.Assert(unchecked((uint)result._length) <= MaxBlockCount);
			if (blocksToShift > 0)
			{
				result.Clear(blocksToShift);
			}
			result._blocks[blocksToShift] = 1U << (int)(remainingBitsToShift);
		}

		public static void Pow10(uint exponent, out BigInteger result)
		{
			// We leverage two arrays - s_Pow10UInt32Table and s_Pow10BigNumTable to speed up the Pow10 calculation.
			//
			// s_Pow10UInt32Table stores the results of 10^0 to 10^7.
			// s_Pow10BigNumTable stores the results of 10^8, 10^16, 10^32, 10^64, 10^128, 10^256, and 10^512
			//
			// For example, let's say exp = 0b111111. We can split the exp to two parts, one is small exp,
			// which 10^smallExp can be represented as uint, another part is 10^bigExp, which must be represented as BigNum.
			// So the result should be 10^smallExp * 10^bigExp.
			//
			// Calculating 10^smallExp is simple, we just lookup the 10^smallExp from s_Pow10UInt32Table.
			// But here's a bad news: although uint can represent 10^9, exp 9's binary representation is 1001.
			// That means 10^(1011), 10^(1101), 10^(1111) all cannot be stored as uint, we cannot easily say something like:
			// "Any bits <= 3 is small exp, any bits > 3 is big exp". So instead of involving 10^8, 10^9 to s_Pow10UInt32Table,
			// consider 10^8 and 10^9 as a bigNum, so they fall into s_Pow10BigNumTable. Now we can have a simple rule:
			// "Any bits <= 3 is small exp, any bits > 3 is big exp".
			//
			// For 0b111111, we first calculate 10^(smallExp), which is 10^(7), now we can shift right 3 bits, prepare to calculate the bigExp part,
			// the exp now becomes 0b000111.
			//
			// Apparently the lowest bit of bigExp should represent 10^8 because we have already shifted 3 bits for smallExp, so s_Pow10BigNumTable[0] = 10^8.
			// Now let's shift exp right 1 bit, the lowest bit should represent 10^(8 * 2) = 10^16, and so on...
			//
			// That's why we just need the values of s_Pow10BigNumTable be power of 2.
			//
			// More details of this implementation can be found at: https://github.com/dotnet/coreclr/pull/12894#discussion_r128890596

			// Validate that `s_Pow10BigNumTable` has exactly enough trailing elements to fill a BigInteger (which contains MaxBlockCount + 1 elements)
			// We validate here, since this is the only current consumer of the array
			Debug.Assert((_pow10BigNumTableIndices[^1] + MaxBlockCount + 2) == _pow10BigNumTable.Length);

			SetUInt32(out BigInteger temp1, _pow10UInt32Table[exponent & 0x7]);
			ref BigInteger lhs = ref temp1;

			SetZero(out BigInteger temp2);
			ref BigInteger product = ref temp2;

			exponent >>= 3;
			uint index = 0;

			while (exponent != 0)
			{
				// If the current bit is set, multiply it with the corresponding power of 10
				if ((exponent & 1) != 0)
				{
					// Multiply into the next temporary
					fixed (uint* pBigNumEntry = &_pow10BigNumTable[_pow10BigNumTableIndices[index]])
					{
						ref BigInteger rhs = ref *(BigInteger*)(pBigNumEntry);
						Multiply(ref lhs, ref rhs, out product);
					}

					// Swap to the next temporary
					ref BigInteger temp = ref product;
					product = ref lhs;
					lhs = ref temp;
				}

				// Advance to the next bit
				++index;
				exponent >>= 1;
			}

			SetValue(out result, ref lhs);
		}

		private static uint AddDivisor(ref BigInteger lhs, int lhsStartIndex, ref BigInteger rhs)
		{
			int lhsLength = lhs._length;
			int rhsLength = rhs._length;

			Debug.Assert(lhsLength >= 0);
			Debug.Assert(rhsLength >= 0);
			Debug.Assert(lhsLength >= rhsLength);

			// Repairs the dividend, if the last subtract was too much

			ulong carry = 0UL;

			for (int i = 0; i < rhsLength; i++)
			{
				ref uint lhsValue = ref lhs._blocks[lhsStartIndex + i];

				ulong digit = lhsValue + carry + rhs._blocks[i];
				lhsValue = unchecked((uint)digit);
				carry = digit >> 32;
			}

			return (uint)(carry);
		}

		private static bool DivideGuessTooBig(ulong q, ulong valHi, uint valLo, uint divHi, uint divLo)
		{
			Debug.Assert(q <= 0xFFFFFFFF);

			// We multiply the two most significant limbs of the divisor
			// with the current guess for the quotient. If those are bigger
			// than the three most significant limbs of the current dividend
			// we return true, which means the current guess is still too big.

			ulong chkHi = divHi * q;
			ulong chkLo = divLo * q;

			chkHi += (chkLo >> 32);
			chkLo &= uint.MaxValue;

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

		private static uint SubtractDivisor(ref BigInteger lhs, int lhsStartIndex, ref BigInteger rhs, ulong q)
		{
			int lhsLength = lhs._length - lhsStartIndex;
			int rhsLength = rhs._length;

			Debug.Assert(lhsLength >= 0);
			Debug.Assert(rhsLength >= 0);
			Debug.Assert(lhsLength >= rhsLength);
			Debug.Assert(q <= uint.MaxValue);

			// Combines a subtract and a multiply operation, which is naturally
			// more efficient than multiplying and then subtracting...

			ulong carry = 0;

			for (int i = 0; i < rhsLength; i++)
			{
				carry += rhs._blocks[i] * q;
				uint digit = unchecked((uint)carry);
				carry >>= 32;

				ref uint lhsValue = ref lhs._blocks[lhsStartIndex + i];

				if (lhsValue < digit)
				{
					carry++;
				}

				lhsValue = unchecked(lhsValue - digit);
			}

			return (uint)(carry);
		}

		public void Add(uint value)
		{
			int length = _length;
			if (length == 0)
			{
				SetUInt32(out this, value);
				return;
			}

			_blocks[0] += value;
			if (_blocks[0] >= value)
			{
				// No carry
				return;
			}

			for (int index = 1; index < length; index++)
			{
				_blocks[index]++;
				if (_blocks[index] > 0)
				{
					// No carry
					return;
				}
			}

			Debug.Assert(unchecked((uint)(length)) + 1 <= MaxBlockCount);
			_blocks[length] = 1;
			_length = length + 1;
		}

		public uint GetBlock(uint index)
		{
			Debug.Assert(index < _length);
			return _blocks[index];
		}

		public int GetLength() => _length;

		public bool IsZero() => _length == 0;

		public void Multiply(uint value) => Multiply(ref this, value, out this);

		public void Multiply(ref BigInteger value)
		{
			if (value._length <= 1)
			{
				Multiply(ref this, value.ToUInt32(), out this);
			}
			else
			{
				SetValue(out BigInteger temp, ref this);
				Multiply(ref temp, ref value, out this);
			}
		}

		public void Multiply10()
		{
			if (IsZero())
			{
				return;
			}

			int index = 0;
			int length = _length;
			ulong carry = 0;

			do
			{
				ulong block = (ulong)(_blocks[index]);
				ulong product = (block << 3) + (block << 1) + carry;
				carry = product >> 32;
				_blocks[index] = (uint)(product);

				index++;
			} while (index < length);

			if (carry != 0)
			{
				Debug.Assert(unchecked((uint)(_length)) + 1 <= MaxBlockCount);
				_blocks[index] = (uint)carry;
				_length++;
			}
		}

		public void MultiplyPow10(uint exponent)
		{
			if (exponent <= 9)
			{
				Multiply(_pow10UInt32Table[exponent]);
			}
			else if (!IsZero())
			{
				Pow10(exponent, out BigInteger poweredValue);
				Multiply(ref poweredValue);
			}
		}

		public static void SetUInt32(out BigInteger result, uint value)
		{
			if (value == 0)
			{
				SetZero(out result);
			}
			else
			{
				result._blocks[0] = value;
				result._length = 1;
			}
		}

		public static void SetUInt64(out BigInteger result, ulong value)
		{
			if (value <= uint.MaxValue)
			{
				SetUInt32(out result, (uint)(value));
			}
			else
			{
				result._blocks[0] = (uint)(value);
				result._blocks[1] = (uint)(value >> 32);

				result._length = 2;
			}
		}

		public static void SetUInt128(out BigInteger result, UInt128 value)
		{
			if (value <= ulong.MaxValue)
				SetUInt64(out result, (ulong)value);
			else
			{
				result._blocks[0] = value._u[0];
				result._blocks[1] = value._u[1];
				result._blocks[2] = value._u[2];
				result._blocks[3] = value._u[3];

				result._length = 4;
			}
		}

		public static void SetValue(out BigInteger result, ref BigInteger value)
		{
			int rhsLength = value._length;
			result._length = rhsLength;
			Unsafe.CopyBlock(ref Unsafe.As<uint, byte>(ref result._blocks[0]), ref Unsafe.As<uint, byte>(ref value._blocks[0]), (uint)rhsLength * sizeof(uint));
		}

		public static void SetZero(out BigInteger result) => result._length = 0;

		public void ShiftLeft(uint shift)
		{
			// Process blocks high to low so that we can safely process in place
			int length = _length;

			if ((length == 0) || (shift == 0))
			{
				return;
			}

			uint blocksToShift = DivRem32(shift, out uint remainingBitsToShift);

			// Copy blocks from high to low
			int readIndex = (length - 1);
			int writeIndex = readIndex + (int)(blocksToShift);

			// Check if the shift is block aligned
			if (remainingBitsToShift == 0)
			{
				Debug.Assert(writeIndex < MaxBlockCount);

				while (readIndex >= 0)
				{
					_blocks[writeIndex] = _blocks[readIndex];
					readIndex--;
					writeIndex--;
				}

				_length += (int)(blocksToShift);

				// Zero the remaining low blocks
				Clear(blocksToShift);
			}
			else
			{
				// We need an extra block for the partial shift
				writeIndex++;
				Debug.Assert(writeIndex < MaxBlockCount);

				// Set the length to hold the shifted blocks
				_length = writeIndex + 1;

				// Output the initial blocks
				uint lowBitsShift = (32 - remainingBitsToShift);
				uint highBits = 0;
				uint block = _blocks[readIndex];
				uint lowBits = block >> (int)(lowBitsShift);
				while (readIndex > 0)
				{
					_blocks[writeIndex] = highBits | lowBits;
					highBits = block << (int)(remainingBitsToShift);

					--readIndex;
					--writeIndex;

					block = _blocks[readIndex];
					lowBits = block >> (int)lowBitsShift;
				}

				// Output the final blocks
				_blocks[writeIndex] = highBits | lowBits;
				_blocks[writeIndex - 1] = block << (int)(remainingBitsToShift);

				// Zero the remaining low blocks
				Clear(blocksToShift);

				// Check if the terminating block has no set bits
				if (_blocks[_length - 1] == 0)
				{
					_length--;
				}
			}
		}

		public uint ToUInt32()
		{
			if (_length > 0)
			{
				return _blocks[0];
			}

			return 0;
		}

		public ulong ToUInt64()
		{
			if (_length > 1)
			{
				return ((ulong)(_blocks[1]) << 32) + _blocks[0];
			}

			if (_length > 0)
			{
				return _blocks[0];
			}

			return 0;
		}

		private void Clear(uint length) => Unsafe.InitBlockUnaligned(ref Unsafe.As<uint, byte>(ref _blocks[0]), 0, length * sizeof(uint));

		private static uint DivRem32(uint value, out uint remainder)
		{
			remainder = value & 31;
			return value >> 5;
		}
	}
}
