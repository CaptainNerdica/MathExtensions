using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Vector3D : IEquatable<Vector3D>, IFormattable
	{
		public double X, Y, Z;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3D(double value) : this(value, value, value) { }
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3D(Vector2D value, double z) : this(value.X, value.Y, z) { }

		public Vector3D(ReadOnlySpan<double> values)
		{
			if (values.Length < 3)
				throw new IndexOutOfRangeException();
			this = Unsafe.ReadUnaligned<Vector3D>(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetReference(values)));
		}

		public static Vector3D Zero { get; } = new Vector3D(0);
		public static Vector3D One { get; } = new Vector3D(1);
		public static Vector3D UnitX { get; } = new Vector3D(1, 0, 0);
		public static Vector3D UnitY { get; } = new Vector3D(0, 1, 0);
		public static Vector3D UnitZ { get; } = new Vector3D(0, 0, 1);

		public override readonly string ToString() => ToString("G", CultureInfo.CurrentCulture);
		public readonly string ToString(string? format) => ToString(format, CultureInfo.CurrentCulture);
		public readonly string ToString(string? format, IFormatProvider? provider)
		{
			StringBuilder sb = new StringBuilder();
			string separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;
			sb.Append('<');
			sb.Append(X.ToString(format, provider));
			sb.Append(separator);
			sb.Append(' ');
			sb.Append(Y.ToString(format, provider));
			sb.Append(separator);
			sb.Append(' ');
			sb.Append(Z.ToString(format, provider));
			sb.Append('>');
			return sb.ToString();
		}

		public override bool Equals(object? other) => other is Vector3D o && Equals(o);
		public bool Equals(Vector3D other) => this == other;
		public override int GetHashCode() => HashCode.Combine(X, Y, Z);

		public double Length { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => System.Math.Sqrt(LengthSquared); }
		public double LengthSquared
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (!Sse2.IsSupported)
					return X * X + Y * Y + Z * Z;
				void* p = Unsafe.AsPointer(ref this);
				if (Avx.IsSupported)
				{
					Vector256<double> v = LoadVector256Masked(p);
					Vector256<double> m = Avx.Multiply(v, v);
					Vector256<double> a = Avx.HorizontalAdd(m, m);
					Vector256<double> b = Avx.Permute2x128(a, a, 0x41);
					return Avx.Add(a, b).ToScalar();
				}
				Vector128<double> lo = LoadLowVector128(p);
				Vector128<double> hi = LoadHighVector128(p);
				Vector128<double> ml;
				Vector128<double> mh = Sse2.MultiplyScalar(hi, hi);
				if (Sse41.IsSupported)
					ml = Sse41.DotProduct(lo, lo, 0x31);
				else
				{
					ml = Sse2.Multiply(lo, lo);
					if (Sse3.IsSupported)
						ml = Sse3.HorizontalAdd(ml, ml);
					else
						ml = Sse2.AddScalar(ml, Sse2.Shuffle(ml, ml, 0x4E));
				}
				return Sse2.AddScalar(ml, mh).ToScalar();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector128<double> LoadLowVector128(void* ptr) => Sse2.LoadVector128((double*)ptr);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector128<double> LoadHighVector128(void* ptr) => Sse2.LoadScalarVector128((double*)ptr + 2);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void StoreLowVector128(void* ptr, Vector128<double> vector) => Sse2.Store((double*)ptr, vector);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void StoreHighVector128(void* ptr, Vector128<double> vector) => Sse2.StoreScalar((double*)ptr + 2, vector);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector256<double> LoadVector256Masked(void* ptr) => Avx.And(Avx.LoadVector256((double*)ptr), Vector256.Create(0xFFFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF, 0).AsDouble());
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector256<double> LoadVector256(void* ptr) => Avx.LoadVector256((double*)ptr);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void StoreVector256(void* ptr, Vector256<double> vector)
		{
			Sse2.Store((double*)ptr, vector.GetLower());
			Sse2.StoreScalar((double*)ptr + 2, vector.GetUpper());
		}

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D operator +(Vector3D left, Vector3D right)
		{
			if (!Sse2.IsSupported)
				return new Vector3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
			if (Avx.IsSupported)
			{
				Vector256<double> l = LoadVector256(&left);
				Vector256<double> r = LoadVector256(&right);
				Vector256<double> a = Avx.Add(l, r);
				StoreVector256(&left, a);
				return left;
			}
			Vector128<double> ll = LoadLowVector128(&left);
			Vector128<double> lh = LoadHighVector128(&left);
			Vector128<double> rl = LoadLowVector128(&right);
			Vector128<double> rh = LoadHighVector128(&right);

			Vector128<double> lo = Sse2.Add(ll, rl);
			Vector128<double> hi = Sse2.Add(lh, rh);
			StoreLowVector128(&left, lo);
			StoreHighVector128(&left, hi);
			return left;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector3D left, Vector3D right)
		{
			if (!Sse2.IsSupported)
				return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
			if (Avx.IsSupported)
			{
				Vector256<double> l = LoadVector256(&left);
				Vector256<double> r = LoadVector256(&right);
				Vector256<double> m = Avx.CompareEqual(l, r);
				return (Avx.MoveMask(m) & 0x07) == 0x07;
			}
			Vector128<double> ll = LoadLowVector128(&left);
			Vector128<double> rl = LoadLowVector128(&right);
			Vector128<double> c = Sse2.CompareEqual(ll, rl);
			return (Sse2.MoveMask(c) & 0x03) == 0x03 && left.Z == right.Z;

		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector3D left, Vector3D right) => !(left == right);
		#endregion

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector3D(Vector3 vector) => new Vector3D(vector.X, vector.Y, vector.Z);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector3(Vector3D vector) => new Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
	}
}
