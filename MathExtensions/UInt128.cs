using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace MathExtensions
{
	[StructLayout(LayoutKind.Explicit)]
	public readonly unsafe struct UInt128 : IEquatable<UInt128>, IComparable<UInt128>
	{
		internal const int Bits = 128;
		internal const int Bytes = 16;
		internal const int Words = 8;
		internal const int DWords = 4;
		internal const int QWords = 2;
		[FieldOffset(0)]
		internal readonly uint _u0;
		[FieldOffset(4)]
		internal readonly uint _u1;
		[FieldOffset(8)]
		internal readonly uint _u2;
		[FieldOffset(12)]
		internal readonly uint _u3;

		public static readonly UInt128 MaxValue = new UInt128(uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue);
		public static readonly UInt128 MinValue = default;
		public static UInt128 One { get; } = 1;
		public static UInt128 Zero { get; } = default;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UInt128(uint u0, uint u1, uint u2, uint u3)
		{
			_u0 = u0;
			_u1 = u1;
			_u2 = u2;
			_u3 = u3;
		}

		public override bool Equals(object? obj) => obj is UInt128 other && Equals(other);
		public bool Equals(UInt128 other) => this == other;
		public override readonly int GetHashCode() => HashCode.Combine(_u0, _u1, _u2, _u3);
		public override readonly string ToString()
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
			string s = new string(dest.TrimStart(stackalloc char[] { '\0', '0' }));
			return s;
		}
		internal readonly string ToStringHex()
		{
			StringBuilder sb = new StringBuilder();
			fixed (void* u = &this)
				for (int i = 0; i < Words; ++i)
					sb.AppendFormat("{0:X4} ", ((ushort*)u)[Words - 1 - i]);
			return sb.ToString().Trim();
		}

		private static int GetBitCount(UInt128 value)
		{
			for (int i = Bits - 1; i >= 0; --i)
				if (((int)(((uint*)&value)[i / 32] >> (i % 32)) & 1) == 1)
					return i + 1;
			return 1;
		}

		public static int HighestBit(UInt128 value)
		{
			if (value == Zero) return 0;
			int c = HighDWordIdx(value);
			if (Lzcnt.IsSupported)
				return c * 32 + 31 - (int)Lzcnt.LeadingZeroCount(((uint*)&value)[c]);
			else if (ArmBase.IsSupported)
				return c * 32 + 31 - ArmBase.LeadingZeroCount(((uint*)&value)[c]);
			else
			{
				for (int i = 31; i >= 0; --i)
				{
					if (((((uint*)&value)[c] >> i) & 1) != 0)
						return i;
				}
			}
			return 0; // Should never be reached.
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int HighDWordIdx(UInt128 value)
		{
			if (value._u3 != 0)
				return 3;
			if (value._u2 != 0)
				return 2;
			if (value._u1 != 0)
				return 1;
			return 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetDWordCount(UInt128 value) => HighDWordIdx(value) + 1;

		public int CompareTo(UInt128 other)
		{
			fixed (void* p = &this)
			{
				uint* u = (uint*)p;
				int thisBits = GetBitCount(this);
				int otherBits = GetBitCount(other);
				if (thisBits - otherBits != 0)
					return thisBits - otherBits;
				for (int i = (thisBits - 1) / 32; i >= 0; --i)
				{
					long diff = (long)u[i] - ((uint*)&other)[i];
					if (diff != 0)
						return diff > 0 ? 1 : -1;
				}
				return 0;
			}
		}

		public static UInt128 TwosComplement(UInt128 value) => ~value + 1;

		public static UInt128 Add(UInt128 left, UInt128 right)
		{
			Unsafe.SkipInit(out UInt128 v);
			long carry = 0;
			long digit;

			for (int i = 0; i < DWords; ++i)
			{
				digit = ((uint*)&left)[i] + carry + ((uint*)&right)[i];
				((uint*)&v)[i] = unchecked((uint)digit);
				carry = digit >> 32;
			}
			return v;
		}

		public static UInt128 Subtract(UInt128 left, UInt128 right)
		{
			Unsafe.SkipInit(out UInt128 v);
			long carry = 0;
			long digit;

			for (int i = 0; i < DWords; ++i)
			{
				digit = ((uint*)&left)[i] + carry - ((uint*)&right)[i];
				((uint*)&v)[i] = unchecked((uint)digit);
				carry = digit >> 32;
			}
			return v;
		}

		public static UInt128 Multiply(UInt128 left, UInt128 right)
		{
			UInt128 output;
			uint* l = (uint*)&left;
			uint* r = (uint*)&right;
			uint* o = (uint*)&output;
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
			uint* left = (uint*)&dividend;
			uint* right = (uint*)&divisor;
			uint* bits = (uint*)&output;
			int leftLength = GetDWordCount(dividend);
			int rightLength = GetDWordCount(divisor);
			int bitsLength = leftLength - rightLength + 1;
			BigIntHelpers.Divide(left, leftLength, right, rightLength, bits, bitsLength);
			return output;
		}
		public static UInt128 Divide(UInt128 dividend, uint divisor)
		{
			UInt128 output;
			uint* left = (uint*)&dividend;
			uint* bits = (uint*)&output;
			int leftLength = GetDWordCount(dividend);
			BigIntHelpers.Divide(left, leftLength, divisor, bits);
			return output;
		}

		public static UInt128 DivRem(UInt128 dividend, UInt128 divisor, out UInt128 remainder)
		{
			UInt128 output;
			uint* left = (uint*)&dividend;
			uint* right = (uint*)&divisor;
			uint* o = (uint*)&output;
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
			uint* left = (uint*)&dividend;
			uint* bits = (uint*)&output;
			int leftLength = GetDWordCount(dividend);
			BigIntHelpers.DivRem(left, leftLength, divisor, bits, out remainder);
			return output;
		}

		public static UInt128 Remainder(UInt128 dividend, UInt128 divisor)
		{
			uint* left = (uint*)&dividend;
			uint* right = (uint*)&divisor;
			int leftLength = GetDWordCount(dividend);
			int rightLength = GetDWordCount(divisor);
			BigIntHelpers.Divide(left, leftLength, right, rightLength, null, 0);
			return dividend;
		}
		public static uint Remainder(UInt128 dividend, uint divisor)
		{
			uint* left = (uint*)&dividend;
			int leftLength = GetDWordCount(dividend);
			return BigIntHelpers.Remainder(left, leftLength, divisor);
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

		public static UInt128 operator <<(UInt128 value, int bits)
		{
			const int bitSize = sizeof(uint) * 8;
			bits &= 0x7F;
			int ls = bits % bitSize;
			int rs = bitSize - ls;
			int b = bits / bitSize;
			uint* u = (uint*)&value;
			if (Sse2.IsSupported)
			{
				Vector128<uint> v = Sse2.LoadVector128(u);
				if (rs % 32 != 0)
				{
					Vector128<uint> l = Sse2.ShiftLeftLogical(v, Vector128.CreateScalarUnsafe(ls).AsUInt32());
					Vector128<uint> r = Sse2.ShiftRightLogical(v, Vector128.CreateScalarUnsafe(rs).AsUInt32());
					r = Sse2.ShiftLeftLogical128BitLane(r, 0b10_01_00_11);
					r = Sse2.And(r, Vector128.Create(0, uint.MaxValue, uint.MaxValue, uint.MaxValue));
					v = Sse2.Or(l, r);
				}
				v = b switch
				{
					0 => v,
					1 => Sse2.ShiftLeftLogical128BitLane(v, 0x4),
					2 => Sse2.ShiftLeftLogical128BitLane(v, 0x8),
					3 => Sse2.ShiftLeftLogical128BitLane(v, 0xC),
					_ => v
				};
				Sse2.Store(u, v);
				return value;
			}
			else
			{
				uint* s = stackalloc uint[2 * sizeof(UInt128) / sizeof(uint) - 1];
				if (rs % 32 != 0)
				{
					s[3] = (u[0] << ls);
					s[4] = (u[1] << ls) | (u[0] >> rs);
					s[5] = (u[2] << ls) | (u[1] >> rs);
					s[6] = (u[3] << ls) | (u[2] >> rs);
				}
				else
					Unsafe.Copy(s + 3, ref value);
				return Unsafe.Read<UInt128>(s + 3 - b);
			}
		}
		public static UInt128 operator >>(UInt128 value, int bits)
		{
			const int bitSize = sizeof(uint) * 8;
			bits &= 0x7F;
			int rs = bits % bitSize;
			int ls = bitSize - rs;
			int b = bits / bitSize;
			uint* u = (uint*)&value;
			if (Sse2.IsSupported)
			{
				Vector128<uint> v = Sse2.LoadVector128(u);
				if (rs % 32 != 0)
				{
					Vector128<uint> r = Sse2.ShiftRightLogical(v, Vector128.CreateScalarUnsafe(rs).AsUInt32());
					Vector128<uint> l = Sse2.ShiftLeftLogical(v, Vector128.CreateScalarUnsafe(ls).AsUInt32());
					l = Sse2.Shuffle(l, 0b00_11_10_01);
					l = Sse2.And(l, Vector128.Create(uint.MaxValue, uint.MaxValue, uint.MaxValue, 0));
					v = Sse2.Or(l, r);
				}
				v = b switch
				{
					0 => v,
					1 => Sse2.ShiftRightLogical128BitLane(v, 0x4),
					2 => Sse2.ShiftRightLogical128BitLane(v, 0x8),
					3 => Sse2.ShiftRightLogical128BitLane(v, 0xC),
					_ => v
				};
				Sse2.Store(u, v);
				return value;
			}
			else
			{
				uint* s = stackalloc uint[2 * sizeof(UInt128) / sizeof(uint) - 1];
				if (rs % 32 != 0)
				{
					s[0] = u[0] >> rs | u[1] << ls;
					s[1] = u[1] >> rs | u[2] << ls;
					s[2] = u[2] >> rs | u[3] << ls;
					s[3] = u[3] >> rs;
				}
				else
					Unsafe.Copy(s, ref value);
				return Unsafe.Read<UInt128>(s + b);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt128 operator ~(UInt128 value)
		{
			UInt128 o;
			if (Sse2.IsSupported)
			{
				Vector128<uint> v = Sse2.LoadVector128((uint*)&value);
				Vector128<uint> ov = Sse2.Xor(v, Vector128<uint>.AllBitsSet);
				Sse2.Store((uint*)&o, ov);
			}
			else if (AdvSimd.IsSupported)
			{
				Vector128<uint> v = AdvSimd.LoadVector128((uint*)&value);
				Vector128<uint> ov = AdvSimd.Xor(v, Vector128<uint>.AllBitsSet);
				AdvSimd.Store((uint*)&o, ov);
			}
			else
				return new UInt128(~value._u0, ~value._u1, ~value._u2, ~value._u3);
			return o;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt128 operator &(UInt128 left, UInt128 right)
		{
			UInt128 o;
			if (Sse2.IsSupported)
			{
				Vector128<uint> lv = Sse2.LoadVector128((uint*)&left);
				Vector128<uint> rv = Sse2.LoadVector128((uint*)&right);
				Vector128<uint> ov = Sse2.And(lv, rv);
				Sse2.Store((uint*)&o, ov);
			}
			else if (AdvSimd.IsSupported)
			{
				Vector128<uint> lv = AdvSimd.LoadVector128((uint*)&left);
				Vector128<uint> rv = AdvSimd.LoadVector128((uint*)&right);
				Vector128<uint> ov = AdvSimd.And(lv, rv);
				AdvSimd.Store((uint*)&o, ov);
			}
			else
				return new UInt128(left._u0 & right._u0, left._u1 & right._u1, left._u2 & right._u2, left._u3 & right._u3);
			return o;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt128 operator ^(UInt128 left, UInt128 right)
		{
			UInt128 o;
			if (Sse2.IsSupported)
			{
				Vector128<uint> lv = Sse2.LoadVector128((uint*)&left);
				Vector128<uint> rv = Sse2.LoadVector128((uint*)&right);
				Vector128<uint> ov = Sse2.Xor(lv, rv);
				Sse2.Store((uint*)&o, ov);
			}
			else if (AdvSimd.IsSupported)
			{
				Vector128<uint> lv = AdvSimd.LoadVector128((uint*)&left);
				Vector128<uint> rv = AdvSimd.LoadVector128((uint*)&right);
				Vector128<uint> ov = AdvSimd.Xor(lv, rv);
				AdvSimd.Store((uint*)&o, ov);
			}
			else
				return new UInt128(left._u0 ^ right._u0, left._u1 ^ right._u1, left._u2 ^ right._u2, left._u3 ^ right._u3);
			return o;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt128 operator |(UInt128 left, UInt128 right)
		{
			UInt128 o;
			if (Sse2.IsSupported)
			{
				Vector128<uint> lv = Sse2.LoadVector128((uint*)&left);
				Vector128<uint> rv = Sse2.LoadVector128((uint*)&right);
				Vector128<uint> ov = Sse2.Xor(lv, rv);
				Sse2.Store((uint*)&o, ov);
			}
			else if (AdvSimd.IsSupported)
			{
				Vector128<uint> lv = AdvSimd.LoadVector128((uint*)&left);
				Vector128<uint> rv = AdvSimd.LoadVector128((uint*)&right);
				Vector128<uint> ov = AdvSimd.Xor(lv, rv);
				AdvSimd.Store((uint*)&o, ov);
			}
			else
				return new UInt128(left._u0 | right._u0, left._u1 | right._u1, left._u2 | right._u2, left._u3 | right._u3);
			return o;
		}

		public static bool operator ==(UInt128 left, UInt128 right) => left._u0 == right._u0 && left._u1 == right._u1 && left._u2 == right._u2 && left._u3 == right._u3;
		public static bool operator !=(UInt128 left, UInt128 right) => !(left == right);

		public static bool operator >(UInt128 left, UInt128 right) => left.CompareTo(right) > 0;
		public static bool operator <(UInt128 left, UInt128 right) => left.CompareTo(right) < 0;
		public static bool operator >=(UInt128 left, UInt128 right) => left.CompareTo(right) >= 0;
		public static bool operator <=(UInt128 left, UInt128 right) => left.CompareTo(right) <= 0;

		public static implicit operator UInt128(uint u) => new UInt128(u, 0, 0, 0);
		public static explicit operator UInt128(int i) => new UInt128((uint)i, 0, 0, 0);
		public static implicit operator UInt128(ulong u) => new UInt128((uint)u, (uint)(u >> 32), 0, 0);
		public static explicit operator UInt128(long l) => new UInt128((uint)l, (uint)((ulong)l >> 32), 0, 0);

		public static explicit operator uint(UInt128 u) => u._u0;
		public static explicit operator ulong(UInt128 u) => ((ulong*)&u)[0];

		public static implicit operator BigInteger(UInt128 u)
		{
			Span<byte> b = stackalloc byte[Bytes];
			Span<byte> s = new Span<byte>(&u, Bytes);
			s.CopyTo(b);
			return new BigInteger(b);
		}
	}
}
