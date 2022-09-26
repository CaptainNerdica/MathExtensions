using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions;

// Based on System.Number internal class
static partial class Number
{
	private const int DefaultPrecisionExponentialFormat = 6;

	private const int MaxUInt32DecDigits = 10;
	private const int CharStackBufferSize = 32;
	private const string PosNumberFormat = "#";

	private static readonly string[] _singleDigitStringCache = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

	private static readonly string[] _posCurrencyFormats = new string[]
	{
			"$#", "#$", "$ #", "# $"
	};

	private static readonly string[] _negCurrencyFormats = new string[]
	{
		"($#)", "-$#", "$-#", "$#-",
		"(#$)", "-#$", "#-$", "#$-",
		"-# $", "-$ #", "# $-", "$ #-",
		"$ -#", "#- $", "($ #)", "(# $)",
		"$- #"
	};

	private static readonly string[] _posPercentFormats = new string[]
	{
			"# %", "#%", "%#", "% #"
		};

	private static readonly string[] _negPercentFormats = new string[]
	{
			"-# %", "-#%", "-%#",
			"%-#", "%#-",
			"#-%", "#%-",
			"-% #", "# %-", "% #-",
			"% -#", "#- %"
	};

	private static readonly string[] _negNumberFormats = new string[]
	{
			"(#)", "-#", "- #", "#-", "# -",
	};

	private static char GetHexBase(char fmt)
	{
		// The fmt-(X-A+10) hack has the effect of dictating whether we produce uppercase or lowercase
		// hex numbers for a-f. 'X' as the fmt code produces uppercase. 'x' as the format code produces lowercase.
		return (char)(fmt - ('X' - 'A' + 10));
	}

	internal static string FormatUInt256(UInt256 value, string? format, IFormatProvider? provider)
	{
		if (string.IsNullOrEmpty(format))
		{
			return UInt256ToDecStr(value, -1);
		}

		return FormatUInt256Slow(value, format, provider);

		static unsafe string FormatUInt256Slow(UInt256 value, string? format, IFormatProvider? provider)
		{
			ReadOnlySpan<char> formatSpan = format;
			char fmt = ParseFormatSpecifier(formatSpan, out int digits);
			char fmtUpper = (char)(fmt & 0xFFDF); // ensure fmt is upper-cased for purposes of comparison

			if (fmtUpper == 'G' ? digits < 1 : fmtUpper == 'D')
			{
				return UInt256ToDecStr(value, digits);
			}
			else if (fmtUpper == 'X')
			{
				return Int256ToHexStr(value, GetHexBase(fmt), digits);
			}
			else
			{
				NumberFormatInfo info = NumberFormatInfo.GetInstance(provider);

				byte* pDigits = stackalloc byte[UInt256NumberBufferLength];
				NumberBuffer number = new NumberBuffer(NumberBufferKind.Integer, pDigits, UInt256NumberBufferLength);

				UInt256ToNumber(value, ref number);

				char* stackPtr = stackalloc char[CharStackBufferSize];
				ValueStringBuilder sb = new ValueStringBuilder(new Span<char>(stackPtr, CharStackBufferSize));

				if (fmt != 0)
				{
					NumberToString(ref sb, ref number, fmt, digits, info);
				}
				else
				{
					NumberToStringFormat(ref sb, ref number, formatSpan, info);
				}

				return sb.ToString();
			}
		}
	}

	internal static bool TryFormatUInt256(UInt256 value, ReadOnlySpan<char> format, IFormatProvider? provider, Span<char> destination, out int charsWritten)
	{
		// Fast path for default format
		if (format.Length == 0)
		{
			return TryUInt256ToDecStr(value, digits: -1, destination, out charsWritten);
		}

		return TryFormatUInt256Slow(value, format, provider, destination, out charsWritten);

		static unsafe bool TryFormatUInt256Slow(UInt256 value, ReadOnlySpan<char> format, IFormatProvider? provider, Span<char> destination, out int charsWritten)
		{
			char fmt = ParseFormatSpecifier(format, out int digits);
			char fmtUpper = (char)(fmt & 0xFFDF); // ensure fmt is upper-cased for purposes of comparison

			if (fmtUpper == 'G' ? digits < 1 : fmtUpper == 'D')
			{
				return TryUInt256ToDecStr(value, digits, destination, out charsWritten);
			}
			else if (fmtUpper == 'X')
			{
				return TryInt256ToHexStr((UInt256)value, GetHexBase(fmt), digits, destination, out charsWritten);
			}
			else
			{
				NumberFormatInfo info = NumberFormatInfo.GetInstance(provider);

				byte* pDigits = stackalloc byte[UInt256NumberBufferLength];
				NumberBuffer number = new NumberBuffer(NumberBufferKind.Integer, pDigits, UInt256NumberBufferLength);

				UInt256ToNumber(value, ref number);

				char* stackPtr = stackalloc char[CharStackBufferSize];
				ValueStringBuilder sb = new ValueStringBuilder(new Span<char>(stackPtr, CharStackBufferSize));

				if (fmt != 0)
				{
					NumberToString(ref sb, ref number, fmt, digits, info);
				}
				else
				{
					NumberToStringFormat(ref sb, ref number, format, info);
				}

				return sb.TryCopyTo(destination, out charsWritten);
			}
		}
	}

	private static unsafe string Int256ToHexStr(UInt256 value, char hexBase, int digits)
	{
		if (digits < 1)
			digits = 1;

		UInt256 uValue = (UInt256)value;

		int bufferLength = Math.Max(digits, CountHexDigits(uValue));
		string result = string.Create(bufferLength, uValue, (Span<char> span, UInt256 uValue) =>
		{
			fixed (char* buffer = span)
			{
				char* p = Int256ToHexChars(buffer + bufferLength, uValue, hexBase, digits);
				Debug.Assert(p == buffer);
			}
		});
		return result;
	}

	private static unsafe bool TryInt256ToHexStr(UInt256 value, char hexBase, int digits, Span<char> destination, out int charsWritten)
	{
		if (digits < 1)
			digits = 1;

		UInt256 uValue = (UInt256)value;

		int bufferLength = Math.Max(digits, CountHexDigits(uValue));
		if (bufferLength > destination.Length)
		{
			charsWritten = 0;
			return false;
		}

		charsWritten = bufferLength;
		fixed (char* buffer = &MemoryMarshal.GetReference(destination))
		{
			char* p = Int256ToHexChars(buffer + bufferLength, uValue, hexBase, digits);
			Debug.Assert(p == buffer);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe char* Int256ToHexChars(char* buffer, UInt256 value, int hexBase, int digits)
	{
		ulong u0 = value.U0;
		ulong u1 = value.U1;
		ulong u2 = value.U2;
		ulong u3 = value.U3;

		if (u3 != 0)
		{
			buffer = Int64ToHexChars(buffer, u0, hexBase, 16);
			buffer = Int64ToHexChars(buffer, u1, hexBase, 32);
			buffer = Int64ToHexChars(buffer, u2, hexBase, 48);
			return Int64ToHexChars(buffer, u3, hexBase, digits - 48);
		}
		else if (u2 != 0)
		{
			buffer = Int64ToHexChars(buffer, u0, hexBase, 16);
			buffer = Int64ToHexChars(buffer, u1, hexBase, 32);
			return Int64ToHexChars(buffer, u2, hexBase, digits - 32);
		}
		else if (u1 != 0)
		{
			buffer = Int64ToHexChars(buffer, u0, hexBase, 16);
			return Int64ToHexChars(buffer, u1, hexBase, digits - 16);
		}
		else
		{
			return Int64ToHexChars(buffer, u0, hexBase, Math.Max(digits, 1));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe char* Int64ToHexChars(char* buffer, ulong value, int hexBase, int digits)
	{
		while (--digits >= 0 || value != 0)
		{
			byte digit = (byte)(value & 0xF);
			*(--buffer) = (char)(digit + (digit < 10 ? (byte)'0' : hexBase));
			value >>= 4;
		}
		return buffer;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe char* UInt32ToDecChars(char* bufferEnd, uint value, int digits)
	{
		while (--digits >= 0 || value != 0)
		{
			uint remainder;
			(value, remainder) = Math.DivRem(value, 10);
			*(--bufferEnd) = (char)(remainder + '0');
		}

		return bufferEnd;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]

	internal static unsafe byte* UInt64ToDecChars(byte* bufferEnd, ulong value)
	{
		do
		{
			ulong remainder;
			(value, remainder) = Math.DivRem(value, 10);
			*(--bufferEnd) = (byte)(remainder + '0');
		}
		while (value != 0);

		return bufferEnd;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe byte* UInt64ToDecChars(byte* bufferEnd, ulong value, int digits)
	{
		while (--digits >= 0 || value != 0)
		{
			ulong remainder;
			(value, remainder) = Math.DivRem(value, 10);
			*(--bufferEnd) = (byte)(remainder + '0');
		}

		return bufferEnd;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe char* UInt64ToDecChars(char* bufferEnd, ulong value)
	{
		do
		{
			ulong remainder;
			(value, remainder) = Math.DivRem(value, 10);
			*(--bufferEnd) = (char)(remainder + '0');
		}
		while (value != 0);

		return bufferEnd;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe char* UInt64ToDecChars(char* bufferEnd, ulong value, int digits)
	{
		while (--digits >= 0 || value != 0)
		{
			ulong remainder;
			(value, remainder) = Math.DivRem(value, 10);
			*(--bufferEnd) = (char)(remainder + '0');
		}

		return bufferEnd;
	}

	private static unsafe void UInt256ToNumber(UInt256 value, ref NumberBuffer number)
	{
		number.DigitsCount = UInt256Precision;
		number.IsNegative = false;

		byte* buffer = number.GetDigitsPointer();
		byte* p = UInt256ToDecChars(buffer + UInt256Precision, value, 0);

		int i = (int)(buffer + UInt256Precision - p);

		number.DigitsCount = i;
		number.Scale = i;

		byte* dst = number.GetDigitsPointer();
		while (--i >= 0)
			*dst++ = *p++;
		*dst = (byte)('\0');

		number.CheckConsistency();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong Int256DivMod1E19(ref UInt256 value)
	{
		(value, UInt256 remainder) = UInt256.DivRem(value, new UInt256(0, 0, 0, 10_000_000_000_000_000_000));
		return remainder.U0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe byte* UInt256ToDecChars(byte* bufferEnd, UInt256 value)
	{
		while (value.U3 != 0 && value.U2 != 0 && value.U1 != 0)
		{
			bufferEnd = UInt64ToDecChars(bufferEnd, Int256DivMod1E19(ref value), 19);
		}

		return UInt64ToDecChars(bufferEnd, value.U0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe byte* UInt256ToDecChars(byte* bufferEnd, UInt256 value, int digits)
	{
		while (value.U3 != 0 && value.U2 != 0 && value.U1 != 0)
		{
			bufferEnd = UInt64ToDecChars(bufferEnd, Int256DivMod1E19(ref value), 19);
			digits -= 19;
		}

		return UInt64ToDecChars(bufferEnd, value.U0, digits);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe char* UInt256ToDecChars(char* bufferEnd, UInt256 value)
	{
		while (value.U3 != 0 || value.U2 != 0 || value.U1 != 0)
		{
			bufferEnd = UInt64ToDecChars(bufferEnd, Int256DivMod1E19(ref value), 19);
		}
		return UInt64ToDecChars(bufferEnd, value.U0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe char* UInt256ToDecChars(char* bufferEnd, UInt256 value, int digits)
	{
		while (value.U3 != 0 && value.U2 != 0 && value.U1 != 0)
		{
			bufferEnd = UInt64ToDecChars(bufferEnd, Int256DivMod1E19(ref value), 19);
			digits -= 19;
		}
		return UInt64ToDecChars(bufferEnd, value.U0, digits);
	}

	internal static unsafe string UInt256ToDecStr(UInt256 value)
	{
		// Intrinsified in mono interpreter
		int bufferLength = CountDigits(value);

		// For single-digit values that are very common, especially 0 and 1, just return cached strings.
		if (bufferLength == 1)
		{
			return _singleDigitStringCache[value.U0];
		}

		string result = string.Create(bufferLength, value, (Span<char> span, UInt256 value) =>
		{
			fixed (char* buffer = span)
			{
				char* p = buffer + bufferLength;
				p = UInt256ToDecChars(p, value);
				Debug.Assert(p == buffer);
			}
		});

		return result;
	}

	internal static unsafe string UInt256ToDecStr(UInt256 value, int digits)
	{
		if (digits <= 1)
			return UInt256ToDecStr(value);

		int bufferLength = Math.Max(digits, CountDigits(value));

		string result = string.Create(bufferLength, value, (Span<char> span, UInt256 value) =>
		{
			fixed (char* buffer = span)
			{
				char* p = buffer + bufferLength;
				p = UInt256ToDecChars(p, value, digits);
				Debug.Assert(p == buffer);
			}
		});

		return result;
	}

	private static unsafe bool TryUInt256ToDecStr(UInt256 value, int digits, Span<char> destination, out int charsWritten)
	{
		int bufferLength = Math.Max(digits, CountDigits(value));
		if (bufferLength > destination.Length)
		{
			charsWritten = 0;
			return false;
		}

		charsWritten = bufferLength;
		fixed (char* buffer = &MemoryMarshal.GetReference(destination))
		{
			char* p = buffer + bufferLength;
			if (digits <= 1)
			{
				p = UInt256ToDecChars(p, value);
			}
			else
			{
				p = UInt256ToDecChars(p, value, digits);
			}
			Debug.Assert(p == buffer);
		}
		return true;
	}

	internal static int CountDigits(UInt256 value)
	{
		UInt256 divisor = new UInt256(0, 0, 0, 0x8ac7_2304_89e8_0000); // 1e19
		int digits = 0;

		while (value.U3 != 0 || value.U2 != 0 || value.U1 != 0)
		{
			value /= divisor;
			digits += 19;
		}

		return digits + CountDigits(value.U0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int CountDigits(ulong value)
	{
		int digits = 1;
		uint part;
		if (value >= 10000000)
		{
			if (value >= 100000000000000)
			{
				part = (uint)(value / 100000000000000);
				digits += 14;
			}
			else
			{
				part = (uint)(value / 10000000);
				digits += 7;
			}
		}
		else
		{
			part = (uint)value;
		}

		if (part < 10)
		{
			// no-op
		}
		else if (part < 100)
		{
			digits++;
		}
		else if (part < 1000)
		{
			digits += 2;
		}
		else if (part < 10000)
		{
			digits += 3;
		}
		else if (part < 100000)
		{
			digits += 4;
		}
		else if (part < 1000000)
		{
			digits += 5;
		}
		else
		{
			Debug.Assert(part < 10000000);
			digits += 6;
		}

		return digits;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int CountHexDigits(UInt256 value)
	{
		// The number of hex digits is log16(value) + 1, or log2(value) / 4 + 1
		return ((int)UInt256.Log2(value) >> 2) + 1;
	}

	internal static unsafe char ParseFormatSpecifier(ReadOnlySpan<char> format, out int digits)
	{
		char c = default;
		if (format.Length > 0)
		{
			// If the format begins with a symbol, see if it's a standard format
			// with or without a specified number of digits.
			c = format[0];
			if (char.IsAsciiLetter(c))
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

				// Fallback for symbol and any length digits.  The digits value must be >= 0 && <= 999_999_999,
				// but it can begin with any number of 0s, and thus we may need to check more than 9
				// digits.  Further, for compat, we need to stop when we hit a null char.
				int n = 0;
				int i = 1;
				while ((uint)i < (uint)format.Length && char.IsAsciiDigit(format[i]))
				{
					// Check if we are about to overflow past our limit of 9 digits
					if (n >= 100_000_000)
					{
						throw new FormatException("Bad Format Specifier");
					}

					n = ((n * 10) + format[i++] - '0');
				}

				// If we're at the end of the digits rather than having stopped because we hit something
				// other than a digit or overflowed, return the standard format info.
				if ((uint)i >= (uint)format.Length || format[i] == '\0')
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
					format = (char)(format - ('R' - 'G'));
					Debug.Assert((format == 'G') || (format == 'g'));
					goto case 'G';
				}

			default:
				throw new FormatException("Bad Format Specifier");
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
						var newThousandsSepPos = new int[thousandsSepPos.Length * 2];
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
							throw new ArgumentOutOfRangeException(); // If we overflow
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
				int zeroes = Math.Min(-digPos, nMaxDigits);
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
			PosNumberFormat;

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

		char* digits = stackalloc char[MaxUInt32DecDigits];
		char* p = UInt32ToDecChars(digits + MaxUInt32DecDigits, (uint)value, minDigits);
		sb.Append(p, (int)(digits + MaxUInt32DecDigits - p));
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