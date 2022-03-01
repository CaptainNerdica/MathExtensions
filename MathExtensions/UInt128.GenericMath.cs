using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions;
#if PREVIEW_FEATURES
[RequiresPreviewFeatures]
unsafe partial struct UInt128 : IMinMaxValue<UInt128>, IUnsignedNumber<UInt128>, IBinaryNumber<UInt128>
{
	public static UInt128 AdditiveIdentity => default;
	public static UInt128 MultiplicativeIdentity => One;

	public static UInt128 Abs(UInt128 value) => value;
	public static UInt128 Clamp(UInt128 value, UInt128 min, UInt128 max) => Max(Min(value, max), min);
	public static (UInt128 Quotient, UInt128 Remainder) DivRem(UInt128 left, UInt128 right) => (DivRem(left, right, out UInt128 r), r);

	static UInt128 IBinaryNumber<UInt128>.Log2(UInt128 value) => (UInt128)Log2(value);
	public static bool IsPow2(UInt128 value) => throw new NotImplementedException();

	public static UInt128 Create<TOther>(TOther value) where TOther : INumber<TOther>
	{
		if (TryCreate(value, out UInt128 result))
			return result;
		throw new InvalidOperationException();
	}
	public static UInt128 CreateSaturating<TOther>(TOther value) where TOther : INumber<TOther> => throw new NotSupportedException();
	public static UInt128 CreateTruncating<TOther>(TOther value) where TOther : INumber<TOther> => throw new NotSupportedException();
	public static bool TryCreate<TOther>(TOther value, out UInt128 result) where TOther : INumber<TOther>
	{
		bool output = true;
		if (value is byte _byte)
			result = (UInt128)(uint)_byte;
		else if (value is sbyte _sbyte)
			result = (UInt128)(uint)_sbyte;
		else if (value is short _short)
			result = (UInt128)(uint)_short;
		else if (value is ushort _ushort)
			result = (UInt128)(uint)_ushort;
		else if (value is int _int)
			result = (UInt128)_int;
		else if (value is uint _uint)
			result = (UInt128)_uint;
		else if (value is long _long)
			result = (UInt128)_long;
		else if (value is ulong _ulong)
			result = (UInt128)_ulong;
		else if (value is float _float)
			result = (UInt128)_float;
		else if (value is double _double)
			result = (UInt128)_double;
		else if (value is Half _half)
			result = (UInt128)(int)_half;
		else
		{
			output = false;
			Unsafe.SkipInit(out result);
		}
		return output;
	}

	public static UInt128 Parse(string s!!, IFormatProvider? provider = null) => Parse(s.AsSpan(), NumberStyles.None, provider);
	public static UInt128 Parse(string s!!, NumberStyles style = NumberStyles.None, IFormatProvider? provider = null) => Parse(s.AsSpan(), style, provider);
	public static UInt128 Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null) => Parse(s, NumberStyles.None, provider);
	public static UInt128 Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.None, IFormatProvider? provider = null)
	{
		if (TryParse(s, style, provider, out UInt128 result))
			return result;
		throw new FormatException();
	}
	public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out UInt128 result)
	{
		Unsafe.SkipInit(out result);
		if (s is null)
			return false;
		return TryParse(s.AsSpan(), style, provider, out result);
	}
	public static bool TryParse(string? s, IFormatProvider? provider, out UInt128 result) => TryParse(s, NumberStyles.None, provider, out result);
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out UInt128 result) => TryParse(s, NumberStyles.None, provider, out result);
	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out UInt128 result) => throw new NotImplementedException();

	public bool TryFormat(Span<char> buffer, out int written, ReadOnlySpan<char> format, IFormatProvider? provider = null) => throw new NotImplementedException();

	public static UInt128 Max(UInt128 x, UInt128 y) => x > y ? x : y;
	public static UInt128 Min(UInt128 x, UInt128 y) => x < y ? x : y;

	public static UInt128 Sign(UInt128 value) => IsZero(value) ? value : One;
}
#endif
