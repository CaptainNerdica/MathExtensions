using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using static MathExtensions.Extensions;

namespace MathExtensions
{
	/// <summary>
	/// An Implementation of IEEE 754-2008 binary128 Quadruple precision floating point numbers.
	/// </summary>
	[ReadOnly(true)]
	public unsafe partial struct Quadruple : IEquatable<Quadruple>
	{
		private fixed byte _bytes[16];

		internal int _sign { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => (_bytes[15] & 0x80) >> 7; }
		internal short _exp { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => GetExp(this); }

		internal const int Bias = 16383;
		internal const int SubnormalMantissaBits = 112;
		internal const int NormalMantissaBits = 113;
		public static readonly Quadruple Epsilon = new Quadruple(0x0000_0000_0000_0000, 0x0000_0000_0000_0001);
		public static readonly Quadruple MaxValue = new Quadruple(0x7fff_ffff_ffff_ffff, 0xffff_ffff_ffff_ffff);
		public static readonly Quadruple MinValue = new Quadruple(0xffff_ffff_ffff_ffff, 0xffff_ffff_ffff_ffff);
		public static readonly Quadruple NaN = new Quadruple(0x7fff_0000_0000_0000, 0x0000_0000_0000_0001);
		public static readonly Quadruple NegativeInfinity = new Quadruple(0xffff_0000_0000_0000, 0x0000_0000_0000_0000);
		public static readonly Quadruple PositiveInfinity = new Quadruple(0x7fff_0000_0000_0000, 0x0000_0000_0000_0000);
		public static readonly Quadruple Zero = new Quadruple(0x0000_0000_0000_0000, 0x0000_0000_0000_0000);
		public static readonly Quadruple One = new Quadruple(0x3fff_0000_0000_0000, 0x0000_0000_0000_0000);

		public static bool IsFinite(Quadruple q) => q._exp != 0x7fff;
		public static bool IsInfinity(Quadruple q) => q == PositiveInfinity || q == NegativeInfinity;
		public static bool IsNaN(Quadruple q) => q._exp == 0x7fff && !(((ulong*)q._bytes)[0] == 0 && (((ulong*)q._bytes)[1] & 0xffff_ffff_ffff) == 0);
		public static bool IsNegative(Quadruple q) => q._sign == 1;
		public static bool IsNegativeInfinity(Quadruple q) => q == NegativeInfinity;
		public static bool IsNormal(Quadruple q) => 0x7fff > q._exp && q._exp > 0;
		public static bool IsPositiveInfinity(Quadruple q) => q == PositiveInfinity;
		public static bool IsSubnormal(Quadruple q) => q._exp == 0;

		internal Quadruple(ulong hi, ulong lo)
		{
			fixed (void* buf = _bytes)
			{
				((ulong*)buf)[0] = lo;
				((ulong*)buf)[1] = hi;
			}
		}

		public Quadruple(float value)
		{
			const int bias = 127;
			const int expMask = 0x7F80_0000;
			const int fracMask = 0x007F_FFFF;
			const long signMask = 0x8000_0000;
			fixed (void* buf = _bytes)
			{
				long* p = (long*)buf;
				int s = BitConverter.SingleToInt32Bits(value);
				long exponent = ((s & expMask) >> 23) - bias;
				long frac = s & fracMask;
				p[1] = ((s & signMask) << 32) | ((exponent + Bias) << 48) | (frac << 25);
			}
		}
		public Quadruple(double value)
		{
			const int bias = 1023;
			const long expMask = 0x7FF0_0000_0000_0000;
			const long fracMask = 0x000F_FFFF_FFFF_FFFF;
			const ulong signMask = 0x8000_0000_0000_0000;
			fixed (void* buf = _bytes)
			{
				long* p = (long*)buf;
				long s = BitConverter.DoubleToInt64Bits(value);
				long exponent = ((s & expMask) >> 52) - bias;
				long frac = s & fracMask;
				p[0] = frac << 48;
				p[1] = (long)(((ulong)s & signMask) | (ulong)((exponent + Bias) << 48) | (ulong)(frac >> 16));
			}
		}

		public override bool Equals(object? obj) => obj is Quadruple quad && Equals(quad);
		public bool Equals(Quadruple other) => Equals(this, other);
		public override int GetHashCode() => GetHashCode(this);

		public override string ToString()
		{
			fixed (void* p = _bytes)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < 8; ++i)
					sb.AppendFormat("{0:X4} ", ((short*)p)[7 - i]);
				sb.Remove(39, 1);
				return sb.ToString();
			}
		}

		public static bool operator ==(Quadruple left, Quadruple right) => Equals(left, right);
		public static bool operator !=(Quadruple left, Quadruple right) => !(left == right);

		public static Quadruple operator -(Quadruple quad) => unchecked(new Quadruple((ulong)-((long*)quad._bytes)[1], ((ulong*)quad._bytes)[0]));

		public static implicit operator Quadruple(double value) => new Quadruple(value);
		public static implicit operator Quadruple(float value) => new Quadruple(value);
	}
}
