using MathExtensions.Internal;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using BigInteger = MathExtensions.Internal.BigInteger;

namespace MathExtensions
{
	internal static unsafe class QuadrupleParsing
	{
		internal const int DefaultPrecisionExponentialFormat = 6;
		internal const int QuadruplePrecisionCustomFormat = 33;
		internal const int QuadrupleNumberBufferLength = 11563 + 1;
		internal const int QuadruplePrecision = 36;

		private const int _maxUInt32DecDigits = 10;
		private const string _posNumberFormat = "#";

		private static readonly string[] _posCurrencyFormats =
		{
			"$#", "#$", "$ #", "# $"
		};

		private static readonly string[] _negCurrencyFormats =
		{
			"($#)", "-$#", "$-#", "$#-",
			"(#$)", "-#$", "#-$", "#$-",
			"-# $", "-$ #", "# $-", "$ #-",
			"$ -#", "#- $", "($ #)", "(# $)",
			"$- #"
		};

		private static readonly string[] _posPercentFormats =
		{
			"# %", "#%", "%#", "% #"
		};

		private static readonly string[] _negPercentFormats =
		{
			"-# %", "-#%", "-%#",
			"%-#", "%#-",
			"#-%", "#%-",
			"-% #", "# %-", "% #-",
			"% -#", "#- %"
		};

		private static readonly string[] _negNumberFormats =
		{
			"(#)", "-#", "- #", "#-", "# -",
		};
		internal static unsafe char ParseFormatSpecifier(ReadOnlySpan<char> format, out int digits)
		{
			char c = default;
			if (format.Length > 0)
			{
				// If the format begins with a symbol, see if it's a standard format
				// with or without a specified number of digits.
				c = format[0];
				if ((uint)(c - 'A') <= 'Z' - 'A' ||
					(uint)(c - 'a') <= 'z' - 'a')
				{
					// Fast path for sole symbol, e.g. "D"
					if (format.Length == 1)
					{
						digits = -1;
						return c;
					}

					if (format.Length == 2)
					{
						// Fast path for symbol and single digit, e.g. "X4"
						int d = format[1] - '0';
						if ((uint)d < 10)
						{
							digits = d;
							return c;
						}
					}
					else if (format.Length == 3)
					{
						// Fast path for symbol and double digit, e.g. "F12"
						int d1 = format[1] - '0', d2 = format[2] - '0';
						if ((uint)d1 < 10 && (uint)d2 < 10)
						{
							digits = d1 * 10 + d2;
							return c;
						}
					}

					// Fallback for symbol and any length digits.  The digits value must be >= 0 && <= 99,
					// but it can begin with any number of 0s, and thus we may need to check more than two
					// digits.  Further, for compat, we need to stop when we hit a null char.
					int n = 0;
					int i = 1;
					while (i < format.Length && (((uint)format[i] - '0') < 10) && n < 10)
					{
						n = (n * 10) + format[i++] - '0';
					}

					// If we're at the end of the digits rather than having stopped because we hit something
					// other than a digit or overflowed, return the standard format info.
					if (i == format.Length || format[i] == '\0')
					{
						digits = n;
						return c;
					}
				}
			}

			// Default empty format to be "G"; custom format is signified with '\0'.
			digits = -1;
			return format.Length == 0 || c == '\0' ? // For compat, treat '\0' as the end of the specifier, even if the specifier extends beyond it.
				'G' :
				'\0';
		}

		private static int GetFloatingPointMaxDigitsAndPrecision(char fmt, ref int precision, NumberFormatInfo info, out bool isSignificantDigits)
		{
			if (fmt == 0)
			{
				isSignificantDigits = true;
				return precision;
			}

			int maxDigits = precision;

			switch (fmt)
			{
				case 'C':
				case 'c':
					{
						// The currency format uses the precision specifier to indicate the number of
						// decimal digits to format. This defaults to NumberFormatInfo.CurrencyDecimalDigits.

						if (precision == -1)
						{
							precision = info.CurrencyDecimalDigits;
						}
						isSignificantDigits = false;

						break;
					}

				case 'E':
				case 'e':
					{
						// The exponential format uses the precision specifier to indicate the number of
						// decimal digits to format. This defaults to 6. However, the exponential format
						// also always formats a single integral digit, so we need to increase the precision
						// specifier and treat it as the number of significant digits to account for this.

						if (precision == -1)
						{
							precision = DefaultPrecisionExponentialFormat;
						}

						precision++;
						isSignificantDigits = true;

						break;
					}

				case 'F':
				case 'f':
				case 'N':
				case 'n':
					{
						// The fixed-point and number formats use the precision specifier to indicate the number
						// of decimal digits to format. This defaults to NumberFormatInfo.NumberDecimalDigits.

						if (precision == -1)
						{
							precision = info.NumberDecimalDigits;
						}
						isSignificantDigits = false;

						break;
					}

				case 'G':
				case 'g':
					{
						// The general format uses the precision specifier to indicate the number of significant
						// digits to format. This defaults to the shortest roundtrippable string. Additionally,
						// given that we can't return zero significant digits, we treat 0 as returning the shortest
						// roundtrippable string as well.

						if (precision == 0)
						{
							precision = -1;
						}
						isSignificantDigits = true;

						break;
					}

				case 'P':
				case 'p':
					{
						// The percent format uses the precision specifier to indicate the number of
						// decimal digits to format. This defaults to NumberFormatInfo.PercentDecimalDigits.
						// However, the percent format also always multiplies the number by 100, so we need
						// to increase the precision specifier to ensure we get the appropriate number of digits.

						if (precision == -1)
						{
							precision = info.PercentDecimalDigits;
						}

						precision += 2;
						isSignificantDigits = false;

						break;
					}

				case 'R':
				case 'r':
					{
						// The roundtrip format ignores the precision specifier and always returns the shortest
						// roundtrippable string.

						precision = -1;
						isSignificantDigits = true;

						break;
					}

				default:
					{
						throw new FormatException("Bad format specifier");
					}
			}

			return maxDigits;
		}

		internal static unsafe string? FormatQuadruple(ref ValueStringBuilder sb, Quadruple value, ReadOnlySpan<char> format, NumberFormatInfo info)
		{
			if (!Quadruple.IsFinite(value))
			{
				if (Quadruple.IsNaN(value))
					return info.NaNSymbol;
				return Quadruple.IsNegative(value) ? info.NegativeInfinitySymbol : info.PositiveInfinitySymbol;
			}

			char fmt = ParseFormatSpecifier(format, out int precision);
			byte* pDigits = stackalloc byte[QuadrupleNumberBufferLength];

			if (fmt == '\0')
				precision = QuadruplePrecisionCustomFormat;

			NumberBuffer number = new NumberBuffer(NumberBufferKind.FloatingPoint, pDigits, QuadrupleNumberBufferLength)
			{
				IsNegative = Quadruple.IsNegative(value)
			};

			// We need to track the original precision requested since some formats
			// accept values like 0 and others may require additional fixups.
			int nMaxDigits = GetFloatingPointMaxDigitsAndPrecision(fmt, ref precision, info, out bool isSignificantDigits);
			Dragon4Quadruple(value, precision, isSignificantDigits, ref number);

			number.CheckConsistency();

			if (fmt != 0)
			{
				if (precision == -1)
				{
					// For the roundtrip and general format specifiers, when returning the shortest roundtrippable
					// string, we need to update the maximum number of digits to be the greater of number.DigitsCount
					// or SinglePrecision. This ensures that we continue returning "pretty" strings for values with
					// less digits. One example this fixes is "-60", which would otherwise be formatted as "-6E+01"
					// since DigitsCount would be 1 and the formatter would almost immediately switch to scientific notation.

					nMaxDigits = System.Math.Max(number.DigitsCount, QuadruplePrecision);
				}
				NumberToString(ref sb, ref number, fmt, nMaxDigits, info);
			}
			else
			{
				NumberToStringFormat(ref sb, ref number, format, info);
			}
			return null;
		}

		internal const int QuadrupleImplicitBitIndex = 112;
		private static readonly UInt128 _onePointZero = (UInt128)1 << 112;
		private static void Dragon4Quadruple(Quadruple value, int cutoffNumber, bool isSignificantDigits, ref NumberBuffer number)
		{
			Quadruple v = Quadruple.IsNegative(value) ? -value : value;

			Debug.Assert(Quadruple.IsFinite(v));

			UInt128 mantissa = ExtractFractionAndBiasedExponent(v, out int exponent);

			uint mantissaHighBitIdx;
			bool hasUnequalMargins = false;

			if ((mantissa >> QuadrupleImplicitBitIndex) != 0)
			{
				mantissaHighBitIdx = QuadrupleImplicitBitIndex;
				hasUnequalMargins = mantissa == _onePointZero;
			}
			else
			{
				Debug.Assert(mantissa != 0);
				mantissaHighBitIdx = (uint)UInt128.HighestBit(mantissa);
			}

			int length = (int)Dragon4(mantissa, exponent, mantissaHighBitIdx, hasUnequalMargins, cutoffNumber, isSignificantDigits, number.Digits, out int decimalExponent);

			number.Scale = decimalExponent + 1;
			number.Digits[length] = (byte)'\0';
			number.DigitsCount = length;
		}

		private static unsafe uint Dragon4(UInt128 mantissa, int exponent, uint mantissaHighBitIdx, bool hasUnequalMargins, int cutoffNumber, bool isSignificantDigits, Span<byte> buffer, out int decimalExponent)
		{
			throw new NotImplementedException();
#if false
			int curDigit = 0;
			BigInteger scale;
			BigInteger scaledValue;
			BigInteger scaledMarginLow;
			BigInteger* pScaledMarginHigh;
			BigInteger optionalMarginHigh;

			if (hasUnequalMargins)
			{
				if (exponent > 0)   // We have no fractional component
				{
					// 1) Expand the input value by multiplying out the mantissa and exponent.
					//    This represents the input value in its whole number representation.
					// 2) Apply an additional scale of 2 such that later comparisons against the margin values are simplified.
					// 3) Set the margin value to the loweset mantissa bit's scale.

					// scaledValue      = 2 * 2 * mantissa * 2^exponent
					BigInteger.SetUInt128(out scaledValue, mantissa << 2);
					scaledValue.ShiftLeft((uint)(exponent));

					// scale            = 2 * 2 * 1
					BigInteger.SetUInt32(out scale, 4);

					// scaledMarginLow  = 2 * 2^(exponent - 1)
					BigInteger.Pow2((uint)(exponent), out scaledMarginLow);

					// scaledMarginHigh = 2 * 2 * 2^(exponent + 1)
					BigInteger.Pow2((uint)(exponent + 1), out optionalMarginHigh);
				}
				else                // We have a fractional exponent
				{
					// In order to track the mantissa data as an integer, we store it as is with a large scale

					// scaledValue      = 2 * 2 * mantissa
					BigInteger.SetUInt128(out scaledValue, mantissa << 2);

					// scale            = 2 * 2 * 2^(-exponent)
					BigInteger.Pow2((uint)(-exponent + 2), out scale);

					// scaledMarginLow  = 2 * 2^(-1)
					BigInteger.SetUInt32(out scaledMarginLow, 1);

					// scaledMarginHigh = 2 * 2 * 2^(-1)
					BigInteger.SetUInt32(out optionalMarginHigh, 2);
				}

				// The high and low margins are different
				pScaledMarginHigh = &optionalMarginHigh;
			}
			else
			{
				if (exponent > 0)   // We have no fractional component
				{
					// 1) Expand the input value by multiplying out the mantissa and exponent.
					//    This represents the input value in its whole number representation.
					// 2) Apply an additional scale of 2 such that later comparisons against the margin values are simplified.
					// 3) Set the margin value to the lowest mantissa bit's scale.

					// scaledValue     = 2 * mantissa*2^exponent
					BigInteger.SetUInt128(out scaledValue, mantissa << 1);
					scaledValue.ShiftLeft((uint)(exponent));

					// scale           = 2 * 1
					BigInteger.SetUInt32(out scale, 2);

					// scaledMarginLow = 2 * 2^(exponent-1)
					BigInteger.Pow2((uint)(exponent), out scaledMarginLow);
				}
				else                // We have a fractional exponent
				{
					// In order to track the mantissa data as an integer, we store it as is with a large scale

					// scaledValue     = 2 * mantissa
					BigInteger.SetUInt128(out scaledValue, mantissa << 1);

					// scale           = 2 * 2^(-exponent)
					BigInteger.Pow2((uint)(-exponent + 1), out scale);

					// scaledMarginLow = 2 * 2^(-1)
					BigInteger.SetUInt32(out scaledMarginLow, 1);
				}

				// The high and low margins are equal
				pScaledMarginHigh = &scaledMarginLow;
			}

			// Compute an estimate for digitExponent that will be correct or undershoot by one.
			//
			// This optimization is based on the paper "Printing Floating-Point Numbers Quickly and Accurately" by Burger and Dybvig http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.72.4656&rep=rep1&type=pdf
			//
			// We perform an additional subtraction of 0.69 to increase the frequency of a failed estimate because that lets us take a faster branch in the code.
			// 0.69 is chosen because 0.69 + log10(2) is less than one by a reasonable epsilon that will account for any floating point error.
			//
			// We want to set digitExponent to floor(log10(v)) + 1
			//      v = mantissa * 2^exponent
			//      log2(v) = log2(mantissa) + exponent;
			//      log10(v) = log2(v) * log10(2)
			//      floor(log2(v)) = mantissaHighBitIdx + exponent;
			//      log10(v) - log10(2) < (mantissaHighBitIdx + exponent) * log10(2) <= log10(v)
			//      log10(v) < (mantissaHighBitIdx + exponent) * log10(2) + log10(2) <= log10(v) + log10(2)
			//      floor(log10(v)) < ceil((mantissaHighBitIdx + exponent) * log10(2)) <= floor(log10(v)) + 1
			const double Log10V2 = 0.30102999566398119521373889472449;
			int digitExponent = (int)(System.Math.Ceiling(((int)(mantissaHighBitIdx) + exponent) * Log10V2 - 0.69));

			// Divide value by 10^digitExponent.
			if (digitExponent > 0)
			{
				// The exponent is positive creating a division so we multiply up the scale.
				scale.MultiplyPow10((uint)(digitExponent));
			}
			else if (digitExponent < 0)
			{
				// The exponent is negative creating a multiplication so we multiply up the scaledValue, scaledMarginLow and scaledMarginHigh.

				BigInteger.Pow10((uint)(-digitExponent), out BigInteger pow10);

				scaledValue.Multiply(ref pow10);
				scaledMarginLow.Multiply(ref pow10);

				if (pScaledMarginHigh != &scaledMarginLow)
				{
					BigInteger.Multiply(ref scaledMarginLow, 2, out *pScaledMarginHigh);
				}
			}

			bool isEven = (mantissa % 2) == 0;
			bool estimateTooLow = false;

			if (cutoffNumber == -1)
			{
				// When printing the shortest possible string, we want to
				// take IEEE unbiased rounding into account so we can return
				// shorter strings for various edge case values like 1.23E+22

				BigInteger.Add(ref scaledValue, ref *pScaledMarginHigh, out BigInteger scaledValueHigh);
				int cmpHigh = BigInteger.Compare(ref scaledValueHigh, ref scale);
				estimateTooLow = isEven ? (cmpHigh >= 0) : (cmpHigh > 0);
			}
			else
			{
				estimateTooLow = BigInteger.Compare(ref scaledValue, ref scale) >= 0;
			}

			// Was our estimate for digitExponent was too low?
			if (estimateTooLow)
			{
				// The exponent estimate was incorrect.
				// Increment the exponent and don't perform the premultiply needed for the first loop iteration.
				digitExponent++;
			}
			else
			{
				// The exponent estimate was correct.
				// Multiply larger by the output base to prepare for the first loop iteration.
				scaledValue.Multiply10();
				scaledMarginLow.Multiply10();

				if (pScaledMarginHigh != &scaledMarginLow)
				{
					BigInteger.Multiply(ref scaledMarginLow, 2, out *pScaledMarginHigh);
				}
			}

			// Compute the cutoff exponent (the exponent of the final digit to print).
			// Default to the maximum size of the output buffer.
			int cutoffExponent = digitExponent - buffer.Length;

			if (cutoffNumber != -1)
			{
				int desiredCutoffExponent = 0;

				if (isSignificantDigits)
				{
					// We asked for a specific number of significant digits.
					Debug.Assert(cutoffNumber > 0);
					desiredCutoffExponent = digitExponent - cutoffNumber;
				}
				else
				{
					// We asked for a specific number of fractional digits.
					Debug.Assert(cutoffNumber >= 0);
					desiredCutoffExponent = -cutoffNumber;
				}

				if (desiredCutoffExponent > cutoffExponent)
				{
					// Only select the new cutoffExponent if it won't overflow the destination buffer.
					cutoffExponent = desiredCutoffExponent;
				}
			}

			// Output the exponent of the first digit we will print
			decimalExponent = --digitExponent;

			// In preparation for calling BigInteger.HeuristicDivie(), we need to scale up our values such that the highest block of the denominator is greater than or equal to 8.
			// We also need to guarantee that the numerator can never have a length greater than the denominator after each loop iteration.
			// This requires the highest block of the denominator to be less than or equal to 429496729 which is the highest number that can be multiplied by 10 without overflowing to a new block.

			Debug.Assert(scale.GetLength() > 0);
			uint hiBlock = scale.GetBlock((uint)(scale.GetLength() - 1));

			if ((hiBlock < 8) || (hiBlock > 429496729))
			{
				// Perform a bit shift on all values to get the highest block of the denominator into the range [8,429496729].
				// We are more likely to make accurate quotient estimations in BigInteger.HeuristicDivide() with higher denominator values so we shift the denominator to place the highest bit at index 27 of the highest block.
				// This is safe because (2^28 - 1) = 268435455 which is less than 429496729.
				// This means that all values with a highest bit at index 27 are within range.
				Debug.Assert(hiBlock != 0);
				uint hiBlockLog2 = (uint)BitOperations.Log2(hiBlock);
				Debug.Assert((hiBlockLog2 < 3) || (hiBlockLog2 > 27));
				uint shift = (32 + 27 - hiBlockLog2) % 32;

				scale.ShiftLeft(shift);
				scaledValue.ShiftLeft(shift);
				scaledMarginLow.ShiftLeft(shift);

				if (pScaledMarginHigh != &scaledMarginLow)
				{
					BigInteger.Multiply(ref scaledMarginLow, 2, out *pScaledMarginHigh);
				}
			}

			// These values are used to inspect why the print loop terminated so we can properly round the final digit.
			bool low;            // did the value get within marginLow distance from zero
			bool high;           // did the value get within marginHigh distance from one
			uint outputDigit;    // current digit being output

			if (cutoffNumber == -1)
			{
				Debug.Assert(isSignificantDigits);
				Debug.Assert(digitExponent >= cutoffExponent);

				// For the unique cutoff mode, we will try to print until we have reached a level of precision that uniquely distinguishes this value from its neighbors.
				// If we run out of space in the output buffer, we terminate early.

				while (true)
				{
					// divide out the scale to extract the digit
					outputDigit = BigInteger.HeuristicDivide(ref scaledValue, ref scale);
					Debug.Assert(outputDigit < 10);

					// update the high end of the value
					BigInteger.Add(ref scaledValue, ref *pScaledMarginHigh, out BigInteger scaledValueHigh);

					// stop looping if we are far enough away from our neighboring values or if we have reached the cutoff digit
					int cmpLow = BigInteger.Compare(ref scaledValue, ref scaledMarginLow);
					int cmpHigh = BigInteger.Compare(ref scaledValueHigh, ref scale);

					if (isEven)
					{
						low = (cmpLow <= 0);
						high = (cmpHigh >= 0);
					}
					else
					{
						low = (cmpLow < 0);
						high = (cmpHigh > 0);
					}

					if (low || high || (digitExponent == cutoffExponent))
					{
						break;
					}

					// store the output digit
					buffer[curDigit] = (byte)('0' + outputDigit);
					curDigit++;

					// multiply larger by the output base
					scaledValue.Multiply10();
					scaledMarginLow.Multiply10();

					if (pScaledMarginHigh != &scaledMarginLow)
					{
						BigInteger.Multiply(ref scaledMarginLow, 2, out *pScaledMarginHigh);
					}

					digitExponent--;
				}
			}
			else if (digitExponent >= cutoffExponent)
			{
				Debug.Assert((cutoffNumber > 0) || ((cutoffNumber == 0) && !isSignificantDigits));

				// For length based cutoff modes, we will try to print until we have exhausted all precision (i.e. all remaining digits are zeros) or until we reach the desired cutoff digit.
				low = false;
				high = false;

				while (true)
				{
					// divide out the scale to extract the digit
					outputDigit = BigInteger.HeuristicDivide(ref scaledValue, ref scale);
					Debug.Assert(outputDigit < 10);

					if (scaledValue.IsZero() || (digitExponent <= cutoffExponent))
					{
						break;
					}

					// store the output digit
					buffer[curDigit] = (byte)('0' + outputDigit);
					curDigit++;

					// multiply larger by the output base
					scaledValue.Multiply10();
					digitExponent--;
				}
			}
			else
			{
				// In the scenario where the first significant digit is after the cutoff, we want to treat that
				// first significant digit as the rounding digit. If the first significant would cause the next
				// digit to round, we will increase the decimalExponent by one and set the previous digit to one.
				// This  ensures we correctly handle the case where the first significant digit is exactly one after
				// the cutoff, it is a 4, and the subsequent digit would round that to 5 inducing a double rounding
				// bug when NumberToString does its own rounding checks. However, if the first significant digit
				// would not cause the next one to round, we preserve that digit as is.

				// divide out the scale to extract the digit
				outputDigit = BigInteger.HeuristicDivide(ref scaledValue, ref scale);
				Debug.Assert((0 < outputDigit) && (outputDigit < 10));

				if ((outputDigit > 5) || ((outputDigit == 5) && !scaledValue.IsZero()))
				{
					decimalExponent++;
					outputDigit = 1;
				}

				buffer[curDigit] = (byte)('0' + outputDigit);
				curDigit++;

				// return the number of digits output
				return (uint)curDigit;
			}

			// round off the final digit
			// default to rounding down if value got too close to 0
			bool roundDown = low;

			if (low == high)    // is it legal to round up and down
			{
				// round to the closest digit by comparing value with 0.5.
				//
				// To do this we need to convert the inequality to large integer values.
				//      compare(value, 0.5)
				//      compare(scale * value, scale * 0.5)
				//      compare(2 * scale * value, scale)
				scaledValue.ShiftLeft(1); // Multiply by 2
				int compare = BigInteger.Compare(ref scaledValue, ref scale);
				roundDown = compare < 0;

				// if we are directly in the middle, round towards the even digit (i.e. IEEE rouding rules)
				if (compare == 0)
				{
					roundDown = (outputDigit & 1) == 0;
				}
			}

			// print the rounded digit
			if (roundDown)
			{
				buffer[curDigit] = (byte)('0' + outputDigit);
				curDigit++;
			}
			else if (outputDigit == 9)      // handle rounding up
			{
				// find the first non-nine prior digit
				while (true)
				{
					// if we are at the first digit
					if (curDigit == 0)
					{
						// output 1 at the next highest exponent

						buffer[curDigit] = (byte)('1');
						curDigit++;
						decimalExponent++;

						break;
					}

					curDigit--;

					if (buffer[curDigit] != '9')
					{
						// increment the digit

						buffer[curDigit]++;
						curDigit++;

						break;
					}
				}
			}
			else
			{
				// values in the range [0,8] can perform a simple round up
				buffer[curDigit] = (byte)('0' + outputDigit + 1);
				curDigit++;
			}

			// return the number of digits output
			uint outputLen = (uint)curDigit;
			Debug.Assert(outputLen <= buffer.Length);
			return outputLen;
#endif
		}
		private static readonly UInt128 _fracMask = _onePointZero - 1;
		private static UInt128 ExtractFractionAndBiasedExponent(Quadruple value, out int exponent)
		{
			UInt128 bits = Quadruple.AsUInt128(value);
			UInt128 fraction = bits & _fracMask;
			exponent = (int)(uint)(bits >> 112) & 0x7FFF;

			if (exponent != 0)
			{
				// For normalized value, according to https://en.wikipedia.org/wiki/Quadruple-precision_floating-point_format
				// value = 1.fraction * 2^(exp - 16383)
				//       = (1 + mantissa / 2^112) * 2^(exp - 16383)
				//       = (2^112 + mantissa) * 2^(exp - 16383 - 112)
				//
				// So f = (2^112 + mantissa), e = exp - 16495;

				fraction |= _onePointZero;
				exponent -= 16495;
			}
			else
			{
				// For denormalized value, according to https://en.wikipedia.org/wiki/Quadruple-precision_floating-point_format
				// value = 0.fraction * 2^(1 - 16383)
				//       = (mantissa / 2^112) * 2^(-16382)
				//       = mantissa * 2^(-16382 - 112)
				//       = mantissa * 2^(-16494)
				// So f = mantissa, e = -16494
				exponent = -16494;
			}

			return fraction;
		}

		internal static unsafe void NumberToString(ref ValueStringBuilder sb, ref NumberBuffer number, char format, int nMaxDigits, NumberFormatInfo info)
		{
			number.CheckConsistency();
			bool isCorrectlyRounded = (number.Kind == NumberBufferKind.FloatingPoint);

			switch (format)
			{
				case 'C':
				case 'c':
					{
						if (nMaxDigits < 0)
							nMaxDigits = info.CurrencyDecimalDigits;

						RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded); // Don't change this line to use digPos since digCount could have its sign changed.

						FormatCurrency(ref sb, ref number, nMaxDigits, info);

						break;
					}

				case 'F':
				case 'f':
					{
						if (nMaxDigits < 0)
							nMaxDigits = info.NumberDecimalDigits;

						RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);

						if (number.IsNegative)
							sb.Append(info.NegativeSign);

						FormatFixed(ref sb, ref number, nMaxDigits, null, info.NumberDecimalSeparator, null);

						break;
					}

				case 'N':
				case 'n':
					{
						if (nMaxDigits < 0)
							nMaxDigits = info.NumberDecimalDigits; // Since we are using digits in our calculation

						RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);

						FormatNumber(ref sb, ref number, nMaxDigits, info);

						break;
					}

				case 'E':
				case 'e':
					{
						if (nMaxDigits < 0)
							nMaxDigits = DefaultPrecisionExponentialFormat;
						nMaxDigits++;

						RoundNumber(ref number, nMaxDigits, isCorrectlyRounded);

						if (number.IsNegative)
							sb.Append(info.NegativeSign);

						FormatScientific(ref sb, ref number, nMaxDigits, info, format);

						break;
					}

				case 'G':
				case 'g':
					{
						bool noRounding = false;
						if (nMaxDigits < 1)
						{
							if ((number.Kind == NumberBufferKind.Decimal) && (nMaxDigits == -1))
							{
								noRounding = true;  // Turn off rounding for ECMA compliance to output trailing 0's after decimal as significant

								if (number.Digits[0] == 0)
								{
									// -0 should be formatted as 0 for decimal. This is normally handled by RoundNumber (which we are skipping)
									goto SkipSign;
								}

								goto SkipRounding;
							}
							else
							{
								// This ensures that the PAL code pads out to the correct place even when we use the default precision
								nMaxDigits = number.DigitsCount;
							}
						}

						RoundNumber(ref number, nMaxDigits, isCorrectlyRounded);

					SkipRounding:
						if (number.IsNegative)
							sb.Append(info.NegativeSign);

						SkipSign:
						FormatGeneral(ref sb, ref number, nMaxDigits, info, (char)(format - ('G' - 'E')), noRounding);

						break;
					}

				case 'P':
				case 'p':
					{
						if (nMaxDigits < 0)
							nMaxDigits = info.PercentDecimalDigits;
						number.Scale += 2;

						RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);

						FormatPercent(ref sb, ref number, nMaxDigits, info);

						break;
					}

				case 'R':
				case 'r':
					{
						if (number.Kind != NumberBufferKind.FloatingPoint)
						{
							goto default;
						}

						format = (char)(format - ('R' - 'G'));
						Debug.Assert((format == 'G') || (format == 'g'));
						goto case 'G';
					}

				default:
					throw new FormatException("Bad format specifier.");
			}
		}
		internal static unsafe void NumberToStringFormat(ref ValueStringBuilder sb, ref NumberBuffer number, ReadOnlySpan<char> format, NumberFormatInfo info)
		{
			number.CheckConsistency();

			int digitCount;
			int decimalPos;
			int firstDigit;
			int lastDigit;
			int digPos;
			bool scientific;
			int thousandPos;
			int thousandCount = 0;
			bool thousandSeps;
			int scaleAdjust;
			int adjust;

			int section;
			int src;
			byte* dig = number.GetDigitsPointer();
			char ch;

			section = FindSection(format, dig[0] == 0 ? 2 : number.IsNegative ? 1 : 0);

			while (true)
			{
				digitCount = 0;
				decimalPos = -1;
				firstDigit = 0x7FFFFFFF;
				lastDigit = 0;
				scientific = false;
				thousandPos = -1;
				thousandSeps = false;
				scaleAdjust = 0;
				src = section;

				fixed (char* pFormat = &MemoryMarshal.GetReference(format))
				{
					while (src < format.Length && (ch = pFormat[src++]) != 0 && ch != ';')
					{
						switch (ch)
						{
							case '#':
								digitCount++;
								break;
							case '0':
								if (firstDigit == 0x7FFFFFFF)
									firstDigit = digitCount;
								digitCount++;
								lastDigit = digitCount;
								break;
							case '.':
								if (decimalPos < 0)
									decimalPos = digitCount;
								break;
							case ',':
								if (digitCount > 0 && decimalPos < 0)
								{
									if (thousandPos >= 0)
									{
										if (thousandPos == digitCount)
										{
											thousandCount++;
											break;
										}
										thousandSeps = true;
									}
									thousandPos = digitCount;
									thousandCount = 1;
								}
								break;
							case '%':
								scaleAdjust += 2;
								break;
							case '\x2030':
								scaleAdjust += 3;
								break;
							case '\'':
							case '"':
								while (src < format.Length && pFormat[src] != 0 && pFormat[src++] != ch)
									;
								break;
							case '\\':
								if (src < format.Length && pFormat[src] != 0)
									src++;
								break;
							case 'E':
							case 'e':
								if ((src < format.Length && pFormat[src] == '0') ||
									(src + 1 < format.Length && (pFormat[src] == '+' || pFormat[src] == '-') && pFormat[src + 1] == '0'))
								{
									while (++src < format.Length && pFormat[src] == '0')
										;
									scientific = true;
								}
								break;
						}
					}
				}

				if (decimalPos < 0)
					decimalPos = digitCount;

				if (thousandPos >= 0)
				{
					if (thousandPos == decimalPos)
						scaleAdjust -= thousandCount * 3;
					else
						thousandSeps = true;
				}

				if (dig[0] != 0)
				{
					number.Scale += scaleAdjust;
					int pos = scientific ? digitCount : number.Scale + digitCount - decimalPos;
					RoundNumber(ref number, pos, isCorrectlyRounded: false);
					if (dig[0] == 0)
					{
						src = FindSection(format, 2);
						if (src != section)
						{
							section = src;
							continue;
						}
					}
				}
				else
				{
					if (number.Kind != NumberBufferKind.FloatingPoint)
					{
						// The integer types don't have a concept of -0 and decimal always format -0 as 0
						number.IsNegative = false;
					}
					number.Scale = 0;      // Decimals with scale ('0.00') should be rounded.
				}

				break;
			}

			firstDigit = firstDigit < decimalPos ? decimalPos - firstDigit : 0;
			lastDigit = lastDigit > decimalPos ? decimalPos - lastDigit : 0;
			if (scientific)
			{
				digPos = decimalPos;
				adjust = 0;
			}
			else
			{
				digPos = number.Scale > decimalPos ? number.Scale : decimalPos;
				adjust = number.Scale - decimalPos;
			}
			src = section;

			// Adjust can be negative, so we make this an int instead of an unsigned int.
			// Adjust represents the number of characters over the formatting e.g. format string is "0000" and you are trying to
			// format 100000 (6 digits). Means adjust will be 2. On the other hand if you are trying to format 10 adjust will be
			// -2 and we'll need to fixup these digits with 0 padding if we have 0 formatting as in this example.
			Span<int> thousandsSepPos = stackalloc int[4];
			int thousandsSepCtr = -1;

			if (thousandSeps)
			{
				// We need to precompute this outside the number formatting loop
				if (info.NumberGroupSeparator.Length > 0)
				{
					// We need this array to figure out where to insert the thousands separator. We would have to traverse the string
					// backwards. PIC formatting always traverses forwards. These indices are precomputed to tell us where to insert
					// the thousands separator so we can get away with traversing forwards. Note we only have to compute up to digPos.
					// The max is not bound since you can have formatting strings of the form "000,000..", and this
					// should handle that case too.

					int[] groupDigits = info.NumberGroupSizes;

					int groupSizeIndex = 0;     // Index into the groupDigits array.
					int groupTotalSizeCount = 0;
					int groupSizeLen = groupDigits.Length;    // The length of groupDigits array.
					if (groupSizeLen != 0)
						groupTotalSizeCount = groupDigits[groupSizeIndex];   // The current running total of group size.
					int groupSize = groupTotalSizeCount;

					int totalDigits = digPos + ((adjust < 0) ? adjust : 0); // Actual number of digits in o/p
					int numDigits = (firstDigit > totalDigits) ? firstDigit : totalDigits;
					while (numDigits > groupTotalSizeCount)
					{
						if (groupSize == 0)
							break;
						++thousandsSepCtr;
						if (thousandsSepCtr >= thousandsSepPos.Length)
						{
							int[] newThousandsSepPos = new int[thousandsSepPos.Length * 2];
							thousandsSepPos.CopyTo(newThousandsSepPos);
							thousandsSepPos = newThousandsSepPos;
						}

						thousandsSepPos[thousandsSepCtr] = groupTotalSizeCount;
						if (groupSizeIndex < groupSizeLen - 1)
						{
							groupSizeIndex++;
							groupSize = groupDigits[groupSizeIndex];
						}
						groupTotalSizeCount += groupSize;
					}
				}
			}

			if (number.IsNegative && (section == 0) && (number.Scale != 0))
				sb.Append(info.NegativeSign);

			bool decimalWritten = false;

			fixed (char* pFormat = &MemoryMarshal.GetReference(format))
			{
				byte* cur = dig;

				while (src < format.Length && (ch = pFormat[src++]) != 0 && ch != ';')
				{
					if (adjust > 0)
					{
						switch (ch)
						{
							case '#':
							case '0':
							case '.':
								while (adjust > 0)
								{
									// digPos will be one greater than thousandsSepPos[thousandsSepCtr] since we are at
									// the character after which the groupSeparator needs to be appended.
									sb.Append(*cur != 0 ? (char)(*cur++) : '0');
									if (thousandSeps && digPos > 1 && thousandsSepCtr >= 0)
									{
										if (digPos == thousandsSepPos[thousandsSepCtr] + 1)
										{
											sb.Append(info.NumberGroupSeparator);
											thousandsSepCtr--;
										}
									}
									digPos--;
									adjust--;
								}
								break;
						}
					}

					switch (ch)
					{
						case '#':
						case '0':
							{
								if (adjust < 0)
								{
									adjust++;
									ch = digPos <= firstDigit ? '0' : '\0';
								}
								else
								{
									ch = *cur != 0 ? (char)(*cur++) : digPos > lastDigit ? '0' : '\0';
								}
								if (ch != 0)
								{
									sb.Append(ch);
									if (thousandSeps && digPos > 1 && thousandsSepCtr >= 0)
									{
										if (digPos == thousandsSepPos[thousandsSepCtr] + 1)
										{
											sb.Append(info.NumberGroupSeparator);
											thousandsSepCtr--;
										}
									}
								}

								digPos--;
								break;
							}
						case '.':
							{
								if (digPos != 0 || decimalWritten)
								{
									// For compatibility, don't echo repeated decimals
									break;
								}
								// If the format has trailing zeros or the format has a decimal and digits remain
								if (lastDigit < 0 || (decimalPos < digitCount && *cur != 0))
								{
									sb.Append(info.NumberDecimalSeparator);
									decimalWritten = true;
								}
								break;
							}
						case '\x2030':
							sb.Append(info.PerMilleSymbol);
							break;
						case '%':
							sb.Append(info.PercentSymbol);
							break;
						case ',':
							break;
						case '\'':
						case '"':
							while (src < format.Length && pFormat[src] != 0 && pFormat[src] != ch)
								sb.Append(pFormat[src++]);
							if (src < format.Length && pFormat[src] != 0)
								src++;
							break;
						case '\\':
							if (src < format.Length && pFormat[src] != 0)
								sb.Append(pFormat[src++]);
							break;
						case 'E':
						case 'e':
							{
								bool positiveSign = false;
								int i = 0;
								if (scientific)
								{
									if (src < format.Length && pFormat[src] == '0')
									{
										// Handles E0, which should format the same as E-0
										i++;
									}
									else if (src + 1 < format.Length && pFormat[src] == '+' && pFormat[src + 1] == '0')
									{
										// Handles E+0
										positiveSign = true;
									}
									else if (src + 1 < format.Length && pFormat[src] == '-' && pFormat[src + 1] == '0')
									{
										// Handles E-0
										// Do nothing, this is just a place holder s.t. we don't break out of the loop.
									}
									else
									{
										sb.Append(ch);
										break;
									}

									while (++src < format.Length && pFormat[src] == '0')
										i++;
									if (i > 10)
										i = 10;

									int exp = dig[0] == 0 ? 0 : number.Scale - decimalPos;
									FormatExponent(ref sb, info, exp, ch, i, positiveSign);
									scientific = false;
								}
								else
								{
									sb.Append(ch); // Copy E or e to output
									if (src < format.Length)
									{
										if (pFormat[src] == '+' || pFormat[src] == '-')
											sb.Append(pFormat[src++]);
										while (src < format.Length && pFormat[src] == '0')
											sb.Append(pFormat[src++]);
									}
								}
								break;
							}
						default:
							sb.Append(ch);
							break;
					}
				}
			}

			if (number.IsNegative && (section == 0) && (number.Scale == 0) && (sb.Length > 0))
				sb.Insert(0, info.NegativeSign);
		}

		internal static unsafe void RoundNumber(ref NumberBuffer number, int pos, bool isCorrectlyRounded)
		{
			byte* dig = number.GetDigitsPointer();

			int i = 0;
			while (i < pos && dig[i] != '\0')
				i++;

			if ((i == pos) && ShouldRoundUp(dig, i, number.Kind, isCorrectlyRounded))
			{
				while (i > 0 && dig[i - 1] == '9')
					i--;

				if (i > 0)
				{
					dig[i - 1]++;
				}
				else
				{
					number.Scale++;
					dig[0] = (byte)('1');
					i = 1;
				}
			}
			else
			{
				while (i > 0 && dig[i - 1] == '0')
					i--;
			}

			if (i == 0)
			{
				if (number.Kind != NumberBufferKind.FloatingPoint)
				{
					// The integer types don't have a concept of -0 and decimal always format -0 as 0
					number.IsNegative = false;
				}
				number.Scale = 0;      // Decimals with scale ('0.00') should be rounded.
			}

			dig[i] = (byte)('\0');
			number.DigitsCount = i;
			number.CheckConsistency();

			static bool ShouldRoundUp(byte* dig, int i, NumberBufferKind numberKind, bool isCorrectlyRounded)
			{
				// We only want to round up if the digit is greater than or equal to 5 and we are
				// not rounding a floating-point number. If we are rounding a floating-point number
				// we have one of two cases.
				//
				// In the case of a standard numeric-format specifier, the exact and correctly rounded
				// string will have been produced. In this scenario, pos will have pointed to the
				// terminating null for the buffer and so this will return false.
				//
				// However, in the case of a custom numeric-format specifier, we currently fall back
				// to generating Single/DoublePrecisionCustomFormat digits and then rely on this
				// function to round correctly instead. This can unfortunately lead to double-rounding
				// bugs but is the best we have right now due to back-compat concerns.

				byte digit = dig[i];

				if ((digit == '\0') || isCorrectlyRounded)
				{
					// Fast path for the common case with no rounding
					return false;
				}

				// Values greater than or equal to 5 should round up, otherwise we round down. The IEEE
				// 754 spec actually dictates that ties (exactly 5) should round to the nearest even number
				// but that can have undesired behavior for custom numeric format strings. This probably
				// needs further thought for .NET 5 so that we can be spec compliant and so that users
				// can get the desired rounding behavior for their needs.

				return digit >= '5';
			}
		}
		private static void FormatCurrency(ref ValueStringBuilder sb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info)
		{
			string fmt = number.IsNegative ?
				_negCurrencyFormats[info.CurrencyNegativePattern] :
				_posCurrencyFormats[info.CurrencyPositivePattern];

			foreach (char ch in fmt)
			{
				switch (ch)
				{
					case '#':
						FormatFixed(ref sb, ref number, nMaxDigits, info.CurrencyGroupSizes, info.CurrencyDecimalSeparator, info.CurrencyGroupSeparator);
						break;
					case '-':
						sb.Append(info.NegativeSign);
						break;
					case '$':
						sb.Append(info.CurrencySymbol);
						break;
					default:
						sb.Append(ch);
						break;
				}
			}
		}
		private static unsafe void FormatFixed(ref ValueStringBuilder sb, ref NumberBuffer number, int nMaxDigits, int[]? groupDigits, string? sDecimal, string? sGroup)
		{
			int digPos = number.Scale;
			byte* dig = number.GetDigitsPointer();

			if (digPos > 0)
			{
				if (groupDigits != null)
				{
					Debug.Assert(sGroup != null, "Must be null when groupDigits != null");
					int groupSizeIndex = 0;                             // Index into the groupDigits array.
					int bufferSize = digPos;                            // The length of the result buffer string.
					int groupSize = 0;                                  // The current group size.

					// Find out the size of the string buffer for the result.
					if (groupDigits.Length != 0) // You can pass in 0 length arrays
					{
						int groupSizeCount = groupDigits[groupSizeIndex];   // The current total of group size.

						while (digPos > groupSizeCount)
						{
							groupSize = groupDigits[groupSizeIndex];
							if (groupSize == 0)
								break;

							bufferSize += sGroup.Length;
							if (groupSizeIndex < groupDigits.Length - 1)
								groupSizeIndex++;

							groupSizeCount += groupDigits[groupSizeIndex];
							if (groupSizeCount < 0 || bufferSize < 0)
								throw new ArgumentOutOfRangeException(nameof(nMaxDigits)); // If we overflow
						}

						groupSize = groupSizeCount == 0 ? 0 : groupDigits[0]; // If you passed in an array with one entry as 0, groupSizeCount == 0
					}

					groupSizeIndex = 0;
					int digitCount = 0;
					int digLength = number.DigitsCount;
					int digStart = (digPos < digLength) ? digPos : digLength;
					fixed (char* spanPtr = &MemoryMarshal.GetReference(sb.AppendSpan(bufferSize)))
					{
						char* p = spanPtr + bufferSize - 1;
						for (int i = digPos - 1; i >= 0; i--)
						{
							*(p--) = (i < digStart) ? (char)(dig[i]) : '0';

							if (groupSize > 0)
							{
								digitCount++;
								if ((digitCount == groupSize) && (i != 0))
								{
									for (int j = sGroup.Length - 1; j >= 0; j--)
										*(p--) = sGroup[j];

									if (groupSizeIndex < groupDigits.Length - 1)
									{
										groupSizeIndex++;
										groupSize = groupDigits[groupSizeIndex];
									}
									digitCount = 0;
								}
							}
						}

						Debug.Assert(p >= spanPtr - 1, "Underflow");
						dig += digStart;
					}
				}
				else
				{
					do
					{
						sb.Append(*dig != 0 ? (char)(*dig++) : '0');
					}
					while (--digPos > 0);
				}
			}
			else
			{
				sb.Append('0');
			}

			if (nMaxDigits > 0)
			{
				Debug.Assert(sDecimal != null);
				sb.Append(sDecimal);
				if ((digPos < 0) && (nMaxDigits > 0))
				{
					int zeroes = System.Math.Min(-digPos, nMaxDigits);
					sb.Append('0', zeroes);
					digPos += zeroes;
					nMaxDigits -= zeroes;
				}

				while (nMaxDigits > 0)
				{
					sb.Append((*dig != 0) ? (char)(*dig++) : '0');
					nMaxDigits--;
				}
			}
		}
		private static void FormatNumber(ref ValueStringBuilder sb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info)
		{
			string fmt = number.IsNegative ?
				_negNumberFormats[info.NumberNegativePattern] :
				_posNumberFormat;

			foreach (char ch in fmt)
			{
				switch (ch)
				{
					case '#':
						FormatFixed(ref sb, ref number, nMaxDigits, info.NumberGroupSizes, info.NumberDecimalSeparator, info.NumberGroupSeparator);
						break;
					case '-':
						sb.Append(info.NegativeSign);
						break;
					default:
						sb.Append(ch);
						break;
				}
			}
		}
		private static unsafe void FormatScientific(ref ValueStringBuilder sb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info, char expChar)
		{
			byte* dig = number.GetDigitsPointer();

			sb.Append((*dig != 0) ? (char)(*dig++) : '0');

			if (nMaxDigits != 1) // For E0 we would like to suppress the decimal point
				sb.Append(info.NumberDecimalSeparator);

			while (--nMaxDigits > 0)
				sb.Append((*dig != 0) ? (char)(*dig++) : '0');

			int e = number.Digits[0] == 0 ? 0 : number.Scale - 1;
			FormatExponent(ref sb, info, e, expChar, 3, true);
		}
		private static unsafe void FormatExponent(ref ValueStringBuilder sb, NumberFormatInfo info, int value, char expChar, int minDigits, bool positiveSign)
		{
			sb.Append(expChar);

			if (value < 0)
			{
				sb.Append(info.NegativeSign);
				value = -value;
			}
			else
			{
				if (positiveSign)
					sb.Append(info.PositiveSign);
			}

			char* digits = stackalloc char[_maxUInt32DecDigits];
			char* p = UInt32ToDecChars(digits + _maxUInt32DecDigits, (uint)value, minDigits);
			sb.Append(p, (int)(digits + _maxUInt32DecDigits - p));
		}
		private static unsafe void FormatGeneral(ref ValueStringBuilder sb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info, char expChar, bool bSuppressScientific)
		{
			int digPos = number.Scale;
			bool scientific = false;

			if (!bSuppressScientific)
			{
				// Don't switch to scientific notation
				if (digPos > nMaxDigits || digPos < -3)
				{
					digPos = 1;
					scientific = true;
				}
			}

			byte* dig = number.GetDigitsPointer();

			if (digPos > 0)
			{
				do
				{
					sb.Append((*dig != 0) ? (char)(*dig++) : '0');
				} while (--digPos > 0);
			}
			else
			{
				sb.Append('0');
			}

			if (*dig != 0 || digPos < 0)
			{
				sb.Append(info.NumberDecimalSeparator);

				while (digPos < 0)
				{
					sb.Append('0');
					digPos++;
				}

				while (*dig != 0)
					sb.Append((char)(*dig++));
			}

			if (scientific)
				FormatExponent(ref sb, info, number.Scale - 1, expChar, 2, true);
		}
		private static void FormatPercent(ref ValueStringBuilder sb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info)
		{
			string fmt = number.IsNegative ?
				_negPercentFormats[info.PercentNegativePattern] :
				_posPercentFormats[info.PercentPositivePattern];

			foreach (char ch in fmt)
			{
				switch (ch)
				{
					case '#':
						FormatFixed(ref sb, ref number, nMaxDigits, info.PercentGroupSizes, info.PercentDecimalSeparator, info.PercentGroupSeparator);
						break;
					case '-':
						sb.Append(info.NegativeSign);
						break;
					case '%':
						sb.Append(info.PercentSymbol);
						break;
					default:
						sb.Append(ch);
						break;
				}
			}
		}
		internal static unsafe char* UInt32ToDecChars(char* bufferEnd, uint value, int digits)
		{
			while (--digits >= 0 || value != 0)
			{
				uint remainder;
				(value, remainder) = System.Math.DivRem(value, 10);
				*(--bufferEnd) = (char)(remainder + '0');
			}
			return bufferEnd;
		}
		private static unsafe int FindSection(ReadOnlySpan<char> format, int section)
		{
			int src;
			char ch;

			if (section == 0)
				return 0;

			fixed (char* pFormat = &MemoryMarshal.GetReference(format))
			{
				src = 0;
				while (true)
				{
					if (src >= format.Length)
					{
						return 0;
					}

					switch (ch = pFormat[src++])
					{
						case '\'':
						case '"':
							while (src < format.Length && pFormat[src] != 0 && pFormat[src++] != ch) ;
							break;
						case '\\':
							if (src < format.Length && pFormat[src] != 0)
								src++;
							break;
						case ';':
							if (--section != 0)
								break;
							if (src < format.Length && pFormat[src] != 0 && pFormat[src] != ';')
								return src;
							goto case '\0';
						case '\0':
							return 0;
					}
				}
			}
		}
	}
}