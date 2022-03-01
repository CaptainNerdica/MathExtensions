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

#pragma warning disable CA2252 // This API requires opting into preview features
public unsafe partial struct UInt128 : IMinMaxValue<UInt128>, IUnsignedNumber<UInt128>, IBinaryNumber<UInt128>
#pragma warning restore CA2252 // This API requires opting into preview features
{
	[RequiresPreviewFeatures]
	public static UInt128 AdditiveIdentity => default;
	[RequiresPreviewFeatures]
	public static UInt128 MultiplicativeIdentity => One;

	[RequiresPreviewFeatures]
	public static UInt128 Abs(UInt128 value) => value;
	[RequiresPreviewFeatures]
	public static UInt128 Clamp(UInt128 value, UInt128 min, UInt128 max) => Max(Min(value, max), min);
	[RequiresPreviewFeatures]
	public static (UInt128 Quotient, UInt128 Remainder) DivRem(UInt128 left, UInt128 right) => (DivRem(left, right, out UInt128 r), r);

	[RequiresPreviewFeatures]
	static UInt128 IBinaryNumber<UInt128>.Log2(UInt128 value) => (UInt128)Log2(value);
	[RequiresPreviewFeatures]
	public static bool IsPow2(UInt128 value) => !IsZero(value) && IsZero(value & --value);

	[RequiresPreviewFeatures]
	public static UInt128 Create<TOther>(TOther value) where TOther : INumber<TOther>
	{
		if (TryCreate(value, out UInt128 result))
			return result;
		throw new InvalidOperationException();
	}
	[RequiresPreviewFeatures]
	public static UInt128 CreateSaturating<TOther>(TOther value) where TOther : INumber<TOther> => Create(value);
	[RequiresPreviewFeatures]
	public static UInt128 CreateTruncating<TOther>(TOther value) where TOther : INumber<TOther> => Create(value);
	[RequiresPreviewFeatures]
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

	[RequiresPreviewFeatures]
	public static UInt128 Parse(string s!!, IFormatProvider? provider = null) => Parse(s.AsSpan(), NumberStyles.None, provider);
	[RequiresPreviewFeatures]
	public static UInt128 Parse(string s!!, NumberStyles style = NumberStyles.None, IFormatProvider? provider = null) => Parse(s.AsSpan(), style, provider);
	[RequiresPreviewFeatures]
	public static UInt128 Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null) => Parse(s, NumberStyles.None, provider);
	[RequiresPreviewFeatures]
	public static UInt128 Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.None, IFormatProvider? provider = null)
	{
		if (TryParse(s, style, provider, out UInt128 result))
			return result;
		throw new FormatException();
	}
	[RequiresPreviewFeatures]
	public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out UInt128 result)
	{
		Unsafe.SkipInit(out result);
		if (s is null)
			return false;
		return TryParse(s.AsSpan(), style, provider, out result);
	}
	[RequiresPreviewFeatures]
	public static bool TryParse(string? s, IFormatProvider? provider, out UInt128 result) => TryParse(s, NumberStyles.None, provider, out result);
	[RequiresPreviewFeatures]
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out UInt128 result) => TryParse(s, NumberStyles.None, provider, out result);
	[RequiresPreviewFeatures]
	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out UInt128 result) => throw new NotImplementedException();

	[RequiresPreviewFeatures]
	public bool TryFormat(Span<char> buffer, out int written, ReadOnlySpan<char> format, IFormatProvider? provider = null) => throw new NotImplementedException();

	[RequiresPreviewFeatures]
	public static UInt128 Max(UInt128 x, UInt128 y) => x > y ? x : y;
	[RequiresPreviewFeatures]
	public static UInt128 Min(UInt128 x, UInt128 y) => x < y ? x : y;

	[RequiresPreviewFeatures]
	public static UInt128 Sign(UInt128 value) => IsZero(value) ? value : One;

	[RequiresPreviewFeatures]
	static UInt128 INumber<UInt128>.Zero => default;
	[RequiresPreviewFeatures]
	static UInt128 INumber<UInt128>.One => One;

	[RequiresPreviewFeatures]
	static UInt128 IMinMaxValue<UInt128>.MinValue => MinValue;
	[RequiresPreviewFeatures]
	static UInt128 IMinMaxValue<UInt128>.MaxValue => MaxValue;

	[RequiresPreviewFeatures]
	static bool IEqualityOperators<UInt128, UInt128>.operator ==(UInt128 x, UInt128 y) => x == y;
	[RequiresPreviewFeatures]
	static bool IEqualityOperators<UInt128, UInt128>.operator !=(UInt128 x, UInt128 y) => x != y;

	[RequiresPreviewFeatures]
	static bool IComparisonOperators<UInt128, UInt128>.operator >(UInt128 x, UInt128 y) => x > y;
	[RequiresPreviewFeatures]
	static bool IComparisonOperators<UInt128, UInt128>.operator <(UInt128 x, UInt128 y) => x < y;
	[RequiresPreviewFeatures]
	static bool IComparisonOperators<UInt128, UInt128>.operator >=(UInt128 x, UInt128 y) => x >= y;
	[RequiresPreviewFeatures]
	static bool IComparisonOperators<UInt128, UInt128>.operator <=(UInt128 x, UInt128 y) => x <= y;

	[RequiresPreviewFeatures]
	static UInt128 IBitwiseOperators<UInt128, UInt128, UInt128>.operator ~(UInt128 value) => ~value;
	[RequiresPreviewFeatures]
	static UInt128 IBitwiseOperators<UInt128, UInt128, UInt128>.operator &(UInt128 left, UInt128 right) => left & right;
	[RequiresPreviewFeatures]
	static UInt128 IBitwiseOperators<UInt128, UInt128, UInt128>.operator |(UInt128 left, UInt128 right) => left | right;
	[RequiresPreviewFeatures]
	static UInt128 IBitwiseOperators<UInt128, UInt128, UInt128>.operator ^(UInt128 left, UInt128 right) => left ^ right;

	[RequiresPreviewFeatures]
	static UInt128 IUnaryPlusOperators<UInt128, UInt128>.operator +(UInt128 value) => value;
	[RequiresPreviewFeatures]
	static UInt128 IUnaryNegationOperators<UInt128, UInt128>.operator -(UInt128 value) => -value;

	[RequiresPreviewFeatures]
	static UInt128 IIncrementOperators<UInt128>.operator ++(UInt128 value) => ++value;
	[RequiresPreviewFeatures]
	static UInt128 IDecrementOperators<UInt128>.operator --(UInt128 value) => --value;

	[RequiresPreviewFeatures]
	static UInt128 IAdditionOperators<UInt128, UInt128, UInt128>.operator +(UInt128 left, UInt128 right) => left + right;
	[RequiresPreviewFeatures]
	static UInt128 ISubtractionOperators<UInt128, UInt128, UInt128>.operator -(UInt128 left, UInt128 right) => left - right;
	[RequiresPreviewFeatures]
	static UInt128 IMultiplyOperators<UInt128, UInt128, UInt128>.operator *(UInt128 left, UInt128 right) => left * right;
	[RequiresPreviewFeatures]
	static UInt128 IDivisionOperators<UInt128, UInt128, UInt128>.operator /(UInt128 left, UInt128 right) => left / right;
	[RequiresPreviewFeatures]
	static UInt128 IModulusOperators<UInt128, UInt128, UInt128>.operator %(UInt128 left, UInt128 right) => left % right;
}
#endif
