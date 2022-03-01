using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Versioning;
using System.Text;

namespace MathExtensions
{
	public unsafe readonly struct UInt256 : IEquatable<UInt256>, IComparable<UInt256>
	{
		internal const int Bits = 256;
		internal const int Bytes = 32;
		internal const int Words = 16;
		internal const int DWords = 8;
		internal const int QWords = 4;
		internal readonly uint _u0, _u1, _u2, _u3, _u4, _u5, _u6, _u7;

		public static readonly UInt256 MaxValue = new UInt256(uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue);
		public static readonly UInt256 MinValue = default;
		public static readonly UInt256 Zero = default;
		public static readonly UInt256 One = 1;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UInt256(uint u0, uint u1, uint u2, uint u3, uint u4, uint u5, uint u6, uint u7)
		{
			_u0 = u0;
			_u1 = u1;
			_u2 = u2;
			_u3 = u3;
			_u4 = u4;
			_u5 = u5;
			_u6 = u6;
			_u7 = u7;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UInt256(UInt128 low, UInt128 high)
		{
			Unsafe.SkipInit(out _u0);
			Unsafe.SkipInit(out _u1);
			Unsafe.SkipInit(out _u2);
			Unsafe.SkipInit(out _u3);
			Unsafe.SkipInit(out _u4);
			Unsafe.SkipInit(out _u5);
			Unsafe.SkipInit(out _u6);
			Unsafe.SkipInit(out _u7);
			fixed (uint* u = &_u0)
			{
				Unsafe.Copy(u, ref low);
				Unsafe.Copy(u + 4, ref high);
			}
		}

		public override bool Equals(object? obj) => obj is UInt256 other && Equals(other);
		public bool Equals(UInt256 other) => this == other;
		public override readonly int GetHashCode() => HashCode.Combine(_u0, _u1, _u2, _u3, _u4, _u5, _u6, _u7);

		public override readonly string ToString()
		{
			// Numbers used to calculate the number of base 10 digits from the number of base 2 digits.
			// The ratio between nln2 and nln10 is ~log10(2) and will give the max amount of base 10 digits
			// for any number of bits up to 289, so this well within 256 bits.
			const int nln2 = 171;
			const int nln10 = 568;
			if (this == MinValue)
				return "0";
			int bits = GetHighestBit(this) + 1;
			int digits = nln2 * bits / nln10 + 1;
			Span<char> dest = stackalloc char[digits];
			fixed (char* buffer = &dest.GetPinnableReference())
			{
				char* p = buffer + digits;
				UInt256 value = this;
				while (--digits >= 0 || value != default)
				{
					value = DivRem(value, 10, out uint remainder);
					*--p = (char)(remainder + '0');
				}
			}
			return new string(dest.TrimStart('\0').TrimStart('0'));
		}
		internal readonly string ToStringHex()
		{
			StringBuilder sb = new StringBuilder();
			fixed (void* u = &this)
				for (int i = 0; i < Words; ++i)
					sb.AppendFormat("{0:X4} ", ((ushort*)u)[Words - 1 - i]);
			return sb.ToString().Trim();
		}

		internal static int GetHighestBit(UInt256 value)
		{
			for (int i = Bits - 1; i >= 0; --i)
				if (((int)(((uint*)&value)[i / 32] >> (i % 32)) & 1) == 1)
					return i;
			return 0;
		}
		internal static int GetLowestBit(UInt256 value)
		{
			for (int i = 0; i < Bits; ++i)
				if (((int)(((uint*)&value)[i / 32] >> (i % 32)) & 1) == 1)
					return i;
			return Bits - 1;
		}
		public static int HighestBit(UInt256 value)
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
			return 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int HighDWordIdx(UInt256 value)
		{
			if (value._u7 != 0)
				return 7;
			if (value._u6 != 0)
				return 6;
			if (value._u5 != 0)
				return 5;
			if (value._u4 != 0)
				return 4;
			if (value._u3 != 0)
				return 3;
			if (value._u2 != 0)
				return 2;
			if (value._u1 != 0)
				return 1;
			return 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetDWordCount(UInt256 value) => HighDWordIdx(value) + 1;

		public int CompareTo(UInt256 other)
		{
			fixed (void* p = &this)
			{
				uint* u = (uint*)p;
				int thisBits = GetHighestBit(this) + 1;
				int otherBits = GetHighestBit(other) + 1;
				if (thisBits - otherBits != 0)
					return thisBits - otherBits;
				for (int i = (thisBits - 1) / 32; i >= 0; --i)
				{
					long diff = (long)u[i] - ((uint*)&other)[i];
					if (diff != 0)
						return (int)diff;
				}
				return 0;
			}
		}

		public static UInt256 TwosComplement(UInt256 value) => ~value + 1;

		public static UInt256 Add(UInt256 left, UInt256 right)
		{
			Unsafe.SkipInit(out UInt256 v);
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

		public static UInt256 Subtract(UInt256 left, UInt256 right)
		{
			Unsafe.SkipInit(out UInt256 v);
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

		public static UInt256 Multiply(UInt256 left, UInt256 right)
		{
			UInt256 output;
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

		public static UInt256 Divide(UInt256 dividend, UInt256 divisor)
		{
			UInt256 output;
			uint* left = (uint*)&dividend;
			uint* right = (uint*)&divisor;
			uint* bits = (uint*)&output;
			int leftLength = GetDWordCount(dividend);
			int rightLength = GetDWordCount(divisor);
			int bitsLength = leftLength - rightLength + 1;
			BigIntHelpers.Divide(left, leftLength, right, rightLength, bits, bitsLength);
			return output;
		}
		public static UInt256 Divide(UInt256 dividend, uint divisor)
		{
			UInt256 output;
			uint* left = (uint*)&dividend;
			uint* bits = (uint*)&output;
			int leftLength = GetDWordCount(dividend);
			BigIntHelpers.Divide(left, leftLength, divisor, bits);
			return output;
		}

		public static UInt256 DivRem(UInt256 dividend, UInt256 divisor, out UInt256 remainder)
		{
			UInt256 output;
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

		public static UInt256 DivRem(UInt256 dividend, uint divisor, out uint remainder)
		{
			UInt256 output;
			uint* left = (uint*)&dividend;
			uint* bits = (uint*)&output;
			int leftLength = GetDWordCount(dividend);
			BigIntHelpers.DivRem(left, leftLength, divisor, bits, out remainder);
			return output;
		}

		public static UInt256 Remainder(UInt256 dividend, UInt256 divisor)
		{
			uint* left =  (uint*)&dividend;
			uint* right = (uint*)&divisor;
			int leftLength = GetDWordCount(dividend);
			int rightLength = GetDWordCount(divisor);
			BigIntHelpers.Divide(left, leftLength, right, rightLength, null, 0);
			return dividend;
		}
		public static uint Remainder(UInt256 dividend, uint divisor)
		{
			uint* left = (uint*)&dividend;
			int leftLength = GetDWordCount(dividend);
			return BigIntHelpers.Remainder(left, leftLength, divisor);
		}

		public static UInt256 operator +(UInt256 left, UInt256 right) => Add(left, right);
		public static UInt256 operator -(UInt256 left, UInt256 right) => Subtract(left, right);
		public static UInt256 operator *(UInt256 left, UInt256 right) => Multiply(left, right);
		public static UInt256 operator /(UInt256 left, UInt256 right) => Divide(left, right);
		public static UInt256 operator /(UInt256 left, uint right) => Divide(left, right);
		public static UInt256 operator %(UInt256 left, UInt256 right) => Remainder(left, right);
		public static UInt256 operator %(UInt256 left, uint right) => Remainder(left, right);
		public static UInt256 operator ++(UInt256 value) => Add(value, 1);
		public static UInt256 operator --(UInt256 value) => Subtract(value, 1);
		public static UInt256 operator <<(UInt256 value, int bits)
		{
			const int bitSize = sizeof(uint) * 8;
			bits &= 0xFF;
			int ls = bits % bitSize;
			int rs = bitSize - ls;
			int b = bits / bitSize;
			uint* u = (uint*)&value;

			if (Sse41.IsSupported)
			{
				Vector128<uint> vl = Sse2.LoadVector128(u);
				Vector128<uint> vh = Sse2.LoadVector128(u + 4);

				if (rs % 32 != 0)
				{
					Vector128<uint> ll, lr, hl, hr;
					Vector128<uint> lsv = Vector128.CreateScalarUnsafe(ls).AsUInt32();
					Vector128<uint> rsv = Vector128.CreateScalarUnsafe(rs).AsUInt32();

					ll = Sse2.ShiftLeftLogical(vl, lsv);
					hl = Sse2.ShiftLeftLogical(vh, lsv);

					lr = Sse2.ShiftRightLogical(vl, rsv);
					hr = Sse2.ShiftRightLogical(vh, rsv);

					Vector128<uint> lr1 = Sse2.ShiftLeftLogical128BitLane(lr, 0x4);
					Vector128<uint> hr1 = Sse2.ShiftLeftLogical128BitLane(hr, 0x4);
					hr1 = Sse2.Or(hr1, Sse2.ShiftRightLogical128BitLane(lr, 0xC));

					vl = Sse2.Or(ll, lr1);
					vh = Sse2.Or(hl, hr1);

				}
				Vector128<uint> vl1;
				switch (b)
				{
					case 0:
						break;
					case 1:
						vl1 = vl;
						vl = Sse2.ShiftLeftLogical128BitLane(vl, 0x4);
						vh = Sse2.ShiftLeftLogical128BitLane(vh, 0x4);
						vh = Sse2.Or(vh, Sse2.ShiftRightLogical128BitLane(vl1, 0xC));
						break;
					case 2:
						vl1 = vl;
						vl = Sse2.ShiftLeftLogical128BitLane(vl, 0x8);
						vh = Sse2.ShiftLeftLogical128BitLane(vh, 0x8);
						vh = Sse2.Or(vh, Sse2.ShiftRightLogical128BitLane(vl1, 0x8));
						break;
					case 3:
						vl1 = vl;
						vl = Sse2.ShiftLeftLogical128BitLane(vl, 0xC);
						vh = Sse2.ShiftLeftLogical128BitLane(vh, 0xC);
						vh = Sse2.Or(vh, Sse2.ShiftRightLogical128BitLane(vl1, 0x4));
						break;
					case 4:
						vh = vl;
						vl = Vector128<uint>.Zero;
						break;
					case 5:
						vh = vl;
						vl = Vector128<uint>.Zero;
						vh = Sse2.ShiftLeftLogical128BitLane(vh, 0x4);
						break;
					case 6:
						vh = vl;
						vl = Vector128<uint>.Zero;
						vh = Sse2.ShiftLeftLogical128BitLane(vh, 0x8);
						break;
					case 7:
						vh = vl;
						vl = Vector128<uint>.Zero;
						vh = Sse2.ShiftLeftLogical128BitLane(vh, 0xC);
						break;
				}

				Sse2.Store(u, vl);
				Sse2.Store(u + 4, vh);
				return value;
			}
			else
			{
				uint* s = stackalloc uint[2 * sizeof(UInt256) / sizeof(uint) - 1];
				if (rs % 32 != 0)
				{
					s[0x7] = (u[0] << ls);
					s[0x8] = (u[1] << ls) | (u[0] >> rs);
					s[0x9] = (u[2] << ls) | (u[1] >> rs);
					s[0xA] = (u[3] << ls) | (u[2] >> rs);
					s[0xB] = (u[4] << ls) | (u[3] >> rs);
					s[0xC] = (u[5] << ls) | (u[4] >> rs);
					s[0xD] = (u[6] << ls) | (u[5] >> rs);
					s[0xE] = (u[7] << ls) | (u[6] >> rs);
				}
				else
					Unsafe.Copy(s + 7, ref value);
				return Unsafe.Read<UInt256>(s + 7 - b);
			}
		}

		public static UInt256 operator >>(UInt256 value, int bits)
		{
			const int bitSize = sizeof(uint) * 8;
			bits &= 0xFF;
			int rs = bits % bitSize;
			int ls = bitSize - rs;
			int b = bits / bitSize;
			uint* u = (uint*)&value;

			if (Sse2.IsSupported)
			{
				Vector128<uint> vl = Sse2.LoadVector128(u);
				Vector128<uint> vh = Sse2.LoadVector128(u + 4);

				if (rs % 32 != 0)
				{
					Vector128<uint> ll, lr, hl, hr;
					Vector128<uint> lsv = Vector128.CreateScalarUnsafe(ls).AsUInt32();
					Vector128<uint> rsv = Vector128.CreateScalarUnsafe(rs).AsUInt32();

					ll = Sse2.ShiftLeftLogical(vl, lsv);
					hl = Sse2.ShiftLeftLogical(vh, lsv);

					lr = Sse2.ShiftRightLogical(vl, rsv);
					hr = Sse2.ShiftRightLogical(vh, rsv);

					Vector128<uint> ll1 = Sse2.ShiftRightLogical128BitLane(ll, 0x4);
					Vector128<uint> hl1 = Sse2.ShiftRightLogical128BitLane(hl, 0x4);
					ll1 = Sse2.Or(ll1, Sse2.ShiftLeftLogical128BitLane(hl, 0xC));

					vl = Sse2.Or(lr, ll1);
					vh = Sse2.Or(hr, hl1);
				}
				Vector128<uint> vh1;
				switch (b)
				{
					case 0:
						break;
					case 1:
						vh1 = vh;
						vl = Sse2.ShiftRightLogical128BitLane(vl, 0x4);
						vh = Sse2.ShiftRightLogical128BitLane(vh, 0x4);
						vl = Sse2.Or(vl, Sse2.ShiftLeftLogical128BitLane(vh1, 0xC));
						break;
					case 2:
						vh1 = vh;
						vl = Sse2.ShiftRightLogical128BitLane(vl, 0x8);
						vh = Sse2.ShiftRightLogical128BitLane(vh, 0x8);
						vl = Sse2.Or(vl, Sse2.ShiftLeftLogical128BitLane(vh1, 0x8));
						break;
					case 3:
						vh1 = vh;
						vl = Sse2.ShiftRightLogical128BitLane(vl, 0xC);
						vh = Sse2.ShiftRightLogical128BitLane(vh, 0xC);
						vl = Sse2.Or(vl, Sse2.ShiftLeftLogical128BitLane(vh1, 0x4));
						break;
					case 4:
						vl = vh;
						vh = Vector128<uint>.Zero;
						break;
					case 5:
						vl = vh;
						vh = Vector128<uint>.Zero;
						vl = Sse2.ShiftRightLogical128BitLane(vl, 0x4);
						break;
					case 6:
						vl = vh;
						vh = Vector128<uint>.Zero;
						vl = Sse2.ShiftRightLogical128BitLane(vl, 0x8);
						break;
					case 7:
						vl = vh;
						vh = Vector128<uint>.Zero;
						vl = Sse2.ShiftRightLogical128BitLane(vl, 0xC);
						break;
				}
				Sse2.Store(u, vl);
				Sse2.Store(u + 4, vh);
				return value;
			}
			else
			{
				uint* s = stackalloc uint[2 * sizeof(UInt256) / sizeof(uint) - 1];
				if (rs % 32 != 0)
				{
					s[0] = u[0] >> rs | u[1] << ls;
					s[1] = u[1] >> rs | u[2] << ls;
					s[2] = u[2] >> rs | u[3] << ls;
					s[3] = u[3] >> rs | u[4] << ls;
					s[4] = u[4] >> rs | u[5] << ls;
					s[5] = u[5] >> rs | u[6] << ls;
					s[6] = u[6] >> rs | u[7] << ls;
					s[7] = u[7] >> rs;
				}
				else
					Unsafe.Copy(s, ref value);
				return Unsafe.Read<UInt256>(s + b);
			}
		}
		public static UInt256 operator ~(UInt256 value)
		{
			UInt256 o;
			if (Avx.IsSupported)
			{
				Vector256<uint> v = Avx.LoadVector256((uint*)&value);
				Vector256<uint> ov = Avx.Xor(v.AsDouble(), Vector256<double>.AllBitsSet).AsUInt32(); ;
				Avx.Store((uint*)&o, ov);
			}
			else if (Sse2.IsSupported)
			{
				Vector128<uint> vl = Sse2.LoadVector128((uint*)&value);
				Vector128<uint> vh = Sse2.LoadVector128((uint*)&value + 4);
				Vector128<uint> ovl = Sse2.Xor(vl, Vector128<uint>.AllBitsSet);
				Vector128<uint> ovh = Sse2.Xor(vh, Vector128<uint>.AllBitsSet);
				Sse2.Store((uint*)&o, ovl);
				Sse2.Store((uint*)&o + 4, ovh);
			}
			else if (AdvSimd.IsSupported)
			{
				Vector128<uint> vl = AdvSimd.LoadVector128((uint*)&value);
				Vector128<uint> vh = AdvSimd.LoadVector128((uint*)&value + 4);
				Vector128<uint> ovl = AdvSimd.Not(vl);
				Vector128<uint> ovh = AdvSimd.Not(vh);
				AdvSimd.Store((uint*)&o, ovl);
				AdvSimd.Store((uint*)&o + 4, ovh);
			}
			else
				return new UInt256(~value._u0, ~value._u1, ~value._u2, ~value._u3, ~value._u4, ~value._u5, ~value._u6, ~value._u7);
			return o;
		}
		public static UInt256 operator &(UInt256 left, UInt256 right)
		{
			UInt256 o;
			if (Avx.IsSupported)
			{
				Vector256<uint> lv = Avx.LoadVector256((uint*)&left);
				Vector256<uint> rv = Avx.LoadVector256((uint*)&right);
				Vector256<uint> ov;
				if (Avx2.IsSupported)
					ov = Avx2.And(lv, rv);
				else
					ov = Avx.And(lv.AsDouble(), rv.AsDouble()).AsUInt32();
				Avx.Store((uint*)&o, ov);
			}
			else if (Sse2.IsSupported)
			{
				Vector128<uint> lvl = Sse2.LoadVector128((uint*)&left);
				Vector128<uint> rvl = Sse2.LoadVector128((uint*)&right);
				Vector128<uint> lvh = Sse2.LoadVector128((uint*)&left + 4);
				Vector128<uint> rvh = Sse2.LoadVector128((uint*)&right + 4);
				Vector128<uint> ovl = Sse2.And(lvl, rvl);
				Vector128<uint> ovh = Sse2.And(lvh, rvh);
				Sse2.Store((uint*)&o, ovl);
				Sse2.Store((uint*)&o + 4, ovh);
			}
			else if (AdvSimd.IsSupported)
			{
				Vector128<uint> lvl = AdvSimd.LoadVector128((uint*)&left);
				Vector128<uint> rvl = AdvSimd.LoadVector128((uint*)&right);
				Vector128<uint> lvh = AdvSimd.LoadVector128((uint*)&left + 4);
				Vector128<uint> rvh = AdvSimd.LoadVector128((uint*)&right + 4);
				Vector128<uint> ovl = AdvSimd.And(lvl, rvl);
				Vector128<uint> ovh = AdvSimd.And(lvh, rvh);
				AdvSimd.Store((uint*)&o, ovl);
				AdvSimd.Store((uint*)&o + 4, ovh);
			}
			else
				return new UInt256(left._u0 & right._u0, left._u1 & right._u1, left._u2 & right._u2, left._u3 & right._u3, left._u4 & right._u4, left._u5 & right._u5, left._u6 & right._u6, left._u7 & right._u7);
			return o;
		}
		public static UInt256 operator ^(UInt256 left, UInt256 right)
		{
			UInt256 o;
			if (Avx.IsSupported)
			{
				Vector256<uint> lv = Avx.LoadVector256((uint*)&left);
				Vector256<uint> rv = Avx.LoadVector256((uint*)&right);
				Vector256<uint> ov;
				if (Avx2.IsSupported)
					ov = Avx2.Xor(lv, rv);
				else
					ov = Avx.Xor(lv.AsDouble(), rv.AsDouble()).AsUInt32();
				Avx.Store((uint*)&o, ov);
			}
			else if (Sse2.IsSupported)
			{
				Vector128<uint> lvl = Sse2.LoadVector128((uint*)&left);
				Vector128<uint> rvl = Sse2.LoadVector128((uint*)&right);
				Vector128<uint> lvh = Sse2.LoadVector128((uint*)&left + 4);
				Vector128<uint> rvh = Sse2.LoadVector128((uint*)&right + 4);
				Vector128<uint> ovl = Sse2.Xor(lvl, rvl);
				Vector128<uint> ovh = Sse2.Xor(lvh, rvh);
				Sse2.Store((uint*)&o, ovl);
				Sse2.Store((uint*)&o + 4, ovh);
			}
			else if (AdvSimd.IsSupported)
			{
				Vector128<uint> lvl = AdvSimd.LoadVector128((uint*)&left);
				Vector128<uint> rvl = AdvSimd.LoadVector128((uint*)&right);
				Vector128<uint> lvh = AdvSimd.LoadVector128((uint*)&left + 4);
				Vector128<uint> rvh = AdvSimd.LoadVector128((uint*)&right + 4);
				Vector128<uint> ovl = AdvSimd.Xor(lvl, rvl);
				Vector128<uint> ovh = AdvSimd.Xor(lvh, rvh);
				AdvSimd.Store((uint*)&o, ovl);
				AdvSimd.Store((uint*)&o + 4, ovh);
			}
			else
				return new UInt256(left._u0 ^ right._u0, left._u1 ^ right._u1, left._u2 ^ right._u2, left._u3 ^ right._u3, left._u4 ^ right._u4, left._u5 ^ right._u5, left._u6 ^ right._u6, left._u7 ^ right._u7);
			return o;
		}
		public static UInt256 operator |(UInt256 left, UInt256 right)
		{
			{
				UInt256 o;
				if (Avx.IsSupported)
				{
					Vector256<uint> lv = Avx.LoadVector256((uint*)&left);
					Vector256<uint> rv = Avx.LoadVector256((uint*)&right);
					Vector256<uint> ov;
					if (Avx2.IsSupported)
						ov = Avx2.Or(lv, rv);
					else
						ov = Avx.Or(lv.AsDouble(), rv.AsDouble()).AsUInt32();
					Avx.Store((uint*)&o, ov);
				}
				else if (Sse2.IsSupported)
				{
					Vector128<uint> lvl = Sse2.LoadVector128((uint*)&left);
					Vector128<uint> rvl = Sse2.LoadVector128((uint*)&right);
					Vector128<uint> lvh = Sse2.LoadVector128((uint*)&left + 4);
					Vector128<uint> rvh = Sse2.LoadVector128((uint*)&right + 4);
					Vector128<uint> ovl = Sse2.Or(lvl, rvl);
					Vector128<uint> ovh = Sse2.Or(lvh, rvh);
					Sse2.Store((uint*)&o, ovl);
					Sse2.Store((uint*)&o + 4, ovh);
				}
				else if (AdvSimd.IsSupported)
				{
					Vector128<uint> lvl = AdvSimd.LoadVector128((uint*)&left);
					Vector128<uint> rvl = AdvSimd.LoadVector128((uint*)&right);
					Vector128<uint> lvh = AdvSimd.LoadVector128((uint*)&left + 4);
					Vector128<uint> rvh = AdvSimd.LoadVector128((uint*)&right + 4);
					Vector128<uint> ovl = AdvSimd.Or(lvl, rvl);
					Vector128<uint> ovh = AdvSimd.Or(lvh, rvh);
					AdvSimd.Store((uint*)&o, ovl);
					AdvSimd.Store((uint*)&o + 4, ovh);
				}
				else
					return new UInt256(left._u0 | right._u0, left._u1 | right._u1, left._u2 | right._u2, left._u3 | right._u3, left._u4 | right._u4, left._u5 | right._u5, left._u6 | right._u6, left._u7 | right._u7);
				return o;
			}
		}

		public static bool operator ==(UInt256 left, UInt256 right) => left._u0 == right._u0 && left._u1 == right._u1 && left._u2 == right._u2 && left._u3 == right._u3 && left._u4 == right._u4 && left._u5 == right._u5 && left._u6 == right._u6 && left._u7 == right._u7;
		public static bool operator !=(UInt256 left, UInt256 right) => !(left == right);

		public static bool operator >(UInt256 left, UInt256 right) => left.CompareTo(right) > 0;
		public static bool operator <(UInt256 left, UInt256 right) => left.CompareTo(right) < 0;
		public static bool operator >=(UInt256 left, UInt256 right) => left.CompareTo(right) >= 0;
		public static bool operator <=(UInt256 left, UInt256 right) => left.CompareTo(right) <= 0;

		public static implicit operator UInt256(UInt128 u) => new UInt256(u, default);
		public static explicit operator UInt128(UInt256 u) => *(UInt128*)&u;

		public static implicit operator UInt256(uint u) => new UInt256(u, 0, 0, 0, 0, 0, 0, 0);
		public static explicit operator UInt256(int i) => new UInt256((uint)i, 0, 0, 0, 0, 0, 0, 0);

		public static implicit operator UInt256(ulong u) => new UInt256((uint)u, (uint)(u >> 32), 0, 0, 0, 0, 0, 0);
		public static explicit operator UInt256(long l) => new UInt256((uint)l, (uint)((ulong)l >> 32), 0, 0, 0, 0, 0, 0);

		public static explicit operator uint(UInt256 u) => u._u0;
		public static explicit operator ulong(UInt256 u) => ((ulong*)&u)[0];

		public static implicit operator BigInteger(UInt256 u)
		{
			Span<byte> b = stackalloc byte[Bytes];
			Span<byte> s = new Span<byte>(&u, Bytes);
			s.CopyTo(b);
			return new BigInteger(b);
		}
	}
}
