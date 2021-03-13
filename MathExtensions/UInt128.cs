using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MathExtensions
{
	[ReadOnly(true)]
	[DebuggerDisplay("{ToString()}")]
	public unsafe struct UInt128 : IEquatable<UInt128>, IComparable<UInt128>
	{
		internal const int Bits = 128;
		internal const int Bytes = 16;
		internal const int Words = 8;
		internal const int DWords = 4;
		internal const int QWords = 2;
		internal fixed uint _u[4];

		public static readonly UInt128 MaxValue = new UInt128(uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue);
		public static readonly UInt128 MinValue = 0;
		public static readonly UInt128 Zero = 0;
		public static readonly UInt128 One = 1;

		internal bool this[int index]
		{
			get => index < Bits && index >= 0 ? ((_u[index / 32] >> (index % 32)) & 1) == 1 : throw new IndexOutOfRangeException();
			private set
			{
				if (index > Bits || index < 0)
					throw new IndexOutOfRangeException();
				if (!value)
					_u[index / 32] = _u[index / 32] & ~BigIntHelpers.Int32Masks1Bit[index % 32];
				else
					_u[index / 32] = _u[index / 32] | BigIntHelpers.Int32Masks1Bit[index % 32];

			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UInt128(uint u0, uint u1, uint u2, uint u3)
		{
			_u[0] = u0;
			_u[1] = u1;
			_u[2] = u2;
			_u[3] = u3;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UInt128(ulong u0, ulong u1)
		{
			fixed (void* b = _u)
			{
				ulong* p = (ulong*)b;
				p[0] = u0;
				p[1] = u1;
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UInt128(ReadOnlySpan<uint> span) : this(MemoryMarshal.Cast<uint, byte>(span)) { }
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UInt128(ReadOnlySpan<byte> span)
		{
			fixed (void* p = _u)
			{
				Span<byte> s = new Span<byte>(p, Bytes);
				span.Slice(0, Math.Min(Bytes, span.Length)).CopyTo(s);
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal UInt128(void* src, uint length)
		{
			fixed (void* p = _u)
				Unsafe.CopyBlock(p, src, Math.Min(Bytes, length));
		}

		public override bool Equals(object? obj) => obj is UInt128 other && Equals(other);
		public bool Equals(UInt128 other)
		{
			fixed (void* b = _u)
			{
				ulong* u = (ulong*)b;
				ulong* o = (ulong*)other._u;
				return u[0] == o[0] && u[1] == o[1];
			}
		}
		public override int GetHashCode() => HashCode.Combine(_u[0], _u[1], _u[2], _u[3]);
		public override string ToString()
		{
			// Numbers used to calculate the number of base 10 digits from the number of base 2 digits.
			// The ratio between nln2 and nln10 is ~log10(2) and will give the exact amount of base 10 digits
			// for any number of bits up to 289, so this well within 128 bits.
			const int nln2 = 171;
			const int nln10 = 568;
			if (this == MinValue)
				return "0";
			int bits = GetBitCount(this);
			int digits = nln2 * bits / nln10 + 1;
			Span<char> dest = stackalloc char[digits];
			fixed (char* buffer = &dest.GetPinnableReference())
			{
				char* p = buffer + digits;
				UInt128 value = this;
				while (--digits >= 0 || value != default)
				{
					value = DivRem(value, 10, out uint remainder);
					*--p = (char)(remainder + '0');
				}
			}
			string s = new string(dest.TrimStart('\0').TrimStart('0'));
			return s;
		}
		internal string ToStringHex()
		{
			StringBuilder sb = new StringBuilder();
			fixed (void* u = _u)
				for (int i = 0; i < Words; ++i)
					sb.AppendFormat("{0:X4} ", ((ushort*)u)[Words - 1 - i]);
			return sb.ToString().Trim();
		}

		private static int GetBitCount(UInt128 value)
		{
			for (int i = Bits - 1; i >= 0; --i)
				if (((int)(value._u[i / 32] >> (i % 32)) & 1) == 1)
					return i + 1;
			return 1;
		}
		internal static int GetHighestBit(UInt128 value)
		{
			for (int i = Bits - 1; i >= 0; --i)
				if (((int)(value._u[i / 32] >> (i % 32)) & 1) == 1)
					return i;
			return 0;
		}
		internal static int GetLowestBit(UInt128 value)
		{
			for (int i = 0; i < Bits; ++i)
				if (((int)(value._u[i / 32] >> (i % 32)) & 1) == 1)
					return i;
			return Bits - 1;
		}

		public static int LeadingZeroCount(UInt128 value)
		{
			if (value == Zero) return Bits;
			for (int i = Bits - 1; i >= 0; --i)
			{
				if (value[i])
					return i + 1;
			}
			return Bits; //should never be reached.
		}




		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetDWordCount(UInt128 value)
		{
			for (int i = DWords - 1; i >= 0; --i)
				if (value._u[i] != 0)
					return i + 1;
			return 1;
		}

		public int CompareTo(UInt128 other)
		{
			int thisBits = GetBitCount(this);
			int otherBits = GetBitCount(other);
			if (thisBits - otherBits != 0)
				return thisBits - otherBits;
			for (int i = (thisBits - 1) / 32; i >= 0; --i)
			{
				long diff = (long)_u[i] - other._u[i];
				if (diff != 0)
					return diff > 0 ? 1 : -1;
			}
			return 0;
		}

		public static UInt128 TwosComplement(UInt128 value) => ~value + 1;

		public static UInt128 Add(UInt128 left, UInt128 right)
		{
			UInt128 v;
			long carry = 0;
			long digit;

			for (int i = 0; i < DWords; ++i)
			{
				digit = left._u[i] + carry + right._u[i];
				v._u[i] = unchecked((uint)digit);
				carry = digit >> 32;
			}
			return v;
		}

		public static UInt128 Subtract(UInt128 left, UInt128 right)
		{
			UInt128 v;
			long carry = 0;
			long digit;

			for (int i = 0; i < DWords; ++i)
			{
				digit = left._u[i] + carry - right._u[i];
				v._u[i] = unchecked((uint)digit);
				carry = digit >> 32;
			}
			return v;
		}

		public static UInt128 Multiply(UInt128 left, UInt128 right)
		{
			UInt128 output;
			uint* l = left._u;
			uint* r = right._u;
			uint* o = output._u;
			int ldw = GetDWordCount(left);
			int rdw = GetDWordCount(right);
			int min = Math.Min(ldw, rdw);
			for (int i = 0; i < min; ++i)
			{
				ulong carry = 0UL;
				for (int j = 0; j + i < DWords; ++j)
				{
					ulong digits = o[i + j] + carry + (ulong)l[j] * r[i];
					o[i + j] = unchecked((uint)digits);
					carry = digits >> 32;
				}
			}
			return output;
		}

		public static UInt128 Divide(UInt128 dividend, UInt128 divisor)
		{
			UInt128 output;
			uint* left = dividend._u;
			uint* right = divisor._u;
			uint* bits = output._u;
			int leftLength = GetDWordCount(dividend);
			int rightLength = GetDWordCount(divisor);
			int bitsLength = leftLength - rightLength + 1;
			BigIntHelpers.Divide(left, leftLength, right, rightLength, bits, bitsLength);
			return output;
		}
		public static UInt128 Divide(UInt128 dividend, uint divisor)
		{
			UInt128 output;
			uint* left = dividend._u;
			uint* bits = output._u;
			int leftLength = GetDWordCount(dividend);
			BigIntHelpers.Divide(left, leftLength, divisor, bits);
			return output;
		}

		public static UInt128 DivRem(UInt128 dividend, UInt128 divisor, out UInt128 remainder)
		{
			UInt128 output;
			uint* left = dividend._u;
			uint* right = divisor._u;
			uint* o = output._u;
			int leftLength = GetDWordCount(dividend);
			int rightLength = GetDWordCount(divisor);
			int bitsLength = leftLength - rightLength + 1;
			BigIntHelpers.Divide(left, leftLength, right, rightLength, o, bitsLength);
			remainder = dividend;
			return output;
		}
		public static UInt128 DivRem(UInt128 dividend, uint divisor, out uint remainder)
		{
			UInt128 output;
			uint* left = dividend._u;
			uint* bits = output._u;
			int leftLength = GetDWordCount(dividend);
			BigIntHelpers.DivRem(left, leftLength, divisor, bits, out remainder);
			return output;
		}

		public static UInt128 Remainder(UInt128 dividend, UInt128 divisor)
		{
			uint* left = dividend._u;
			uint* right = divisor._u;
			int leftLength = GetDWordCount(dividend);
			int rightLength = GetDWordCount(divisor);
			BigIntHelpers.Divide(left, leftLength, right, rightLength, null, 0);
			return dividend;
		}
		public static uint Remainder(UInt128 dividend, uint divisor)
		{
			uint* left = dividend._u;
			int leftLength = GetDWordCount(dividend);
			return BigIntHelpers.Remainder(left, leftLength, divisor);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt128 ShiftLeft(UInt128 value) => new UInt128(((ulong*)value._u)[0] << 1, (((ulong*)value._u)[0] >> 63) | (((ulong*)value._u)[1] << 1));
		public static UInt128 ShiftLeft(UInt128 value, int bits)
		{
			if (bits < 0)
				return ShiftRight(value, -bits);
			int ls = bits % 64;
			int rs = 64 - ls;
			int b = bits / 64;
			ulong* u = (ulong*)value._u;
			if (ls != 0)
			{
				return b switch
				{
					0 => new UInt128(u[0] << ls, (u[0] >> rs) | (u[1] << ls)),
					1 => new UInt128(0, u[0] << ls),
					_ => Zero
				};
			}
			else
			{
				return b switch
				{
					0 => value,
					1 => new UInt128(0, u[0]),
					_ => Zero
				};
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt128 ShiftRight(UInt128 value) => new UInt128((((ulong*)value._u)[0] >> 1) | (((ulong*)value._u)[1] << 63), ((ulong*)value._u)[1] >> 1);
		public static UInt128 ShiftRight(UInt128 value, int bits)
		{
			if (bits < 0)
				return ShiftLeft(value, -bits);
			int rs = bits % 64;
			int ls = 64 - rs;
			int b = bits / 64;
			ulong* u = (ulong*)value._u;
			if (rs != 0)
			{
				return b switch
				{
					0 => new UInt128((u[0] >> rs) | (u[1] << ls), u[1] >> rs),
					1 => new UInt128(u[1] >> rs, 0),
					_ => default,
				};
			}
			else
			{
				return b switch
				{
					0 => value,
					1 => new UInt128(u[1], 0),
					_ => default,
				};
			}
		}

		public static UInt128 operator +(UInt128 left, UInt128 right) => Add(left, right);
		public static UInt128 operator -(UInt128 left, UInt128 right) => Subtract(left, right);
		public static UInt128 operator *(UInt128 left, UInt128 right) => Multiply(left, right);
		public static UInt128 operator /(UInt128 left, UInt128 right) => Divide(left, right);
		public static UInt128 operator /(UInt128 left, uint right) => Divide(left, right);
		public static UInt128 operator %(UInt128 left, UInt128 right) => Remainder(left, right);
		public static UInt128 operator %(UInt128 left, uint right) => Remainder(left, right);
		public static UInt128 operator ++(UInt128 value) => Add(value, 1);
		public static UInt128 operator --(UInt128 value) => Subtract(value, 1);
		public static UInt128 operator <<(UInt128 value, int bits) => ShiftLeft(value, bits);
		public static UInt128 operator >>(UInt128 value, int bits) => ShiftRight(value, bits);
		public static UInt128 operator ~(UInt128 value) => new UInt128(~value._u[0], ~value._u[1], ~value._u[2], ~value._u[3]);
		public static UInt128 operator &(UInt128 left, UInt128 right) => new UInt128(left._u[0] & right._u[0], left._u[1] & right._u[1], left._u[2] & right._u[2], left._u[3] & right._u[3]);
		public static UInt128 operator ^(UInt128 left, UInt128 right) => new UInt128(left._u[0] ^ right._u[0], left._u[1] ^ right._u[1], left._u[2] ^ right._u[2], left._u[3] ^ right._u[3]);
		public static UInt128 operator |(UInt128 left, UInt128 right) => new UInt128(left._u[0] | right._u[0], left._u[1] | right._u[1], left._u[2] | right._u[2], left._u[3] | right._u[3]);

		public static bool operator ==(UInt128 left, UInt128 right) => left.Equals(right);
		public static bool operator !=(UInt128 left, UInt128 right) => !(left == right);

		public static bool operator >(UInt128 left, UInt128 right) => left.CompareTo(right) > 0;
		public static bool operator <(UInt128 left, UInt128 right) => left.CompareTo(right) < 0;
		public static bool operator >=(UInt128 left, UInt128 right) => left.CompareTo(right) >= 0;
		public static bool operator <=(UInt128 left, UInt128 right) => left.CompareTo(right) <= 0;


		public static implicit operator UInt128(uint u) => new UInt128(u, 0, 0, 0);
		public static explicit operator UInt128(int i) => new UInt128((uint)i, 0, 0, 0);
		public static implicit operator UInt128(ulong u) => new UInt128((uint)u, (uint)(u >> 32), 0, 0);
		public static explicit operator UInt128(long l) => new UInt128((uint)l, (uint)((ulong)l >> 32), 0, 0);

		public static explicit operator uint(UInt128 u) => u._u[0];
		public static explicit operator ulong(UInt128 u) => ((ulong*)u._u)[0];

		public static implicit operator BigInteger(UInt128 u)
		{
			Span<byte> b = stackalloc byte[Bytes];
			Span<byte> s = new Span<byte>(u._u, Bytes);
			s.CopyTo(b);
			return new BigInteger(b);
		}
	}
}
