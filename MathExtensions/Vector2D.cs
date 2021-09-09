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
	public unsafe struct Vector2D : IEquatable<Vector2D>, IFormattable
	{
		public double X, Y;

		public static Vector2D Zero { get; } = new Vector2D(0);
		public static Vector2D One { get; } = new Vector2D(1);
		public static Vector2D UnitX { get; } = new Vector2D(1, 0);
		public static Vector2D UnitY { get; } = new Vector2D(0, 1);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2D(double x, double y)
		{
			X = x;
			Y = y;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2D(double value) : this(value, value) { }

		public Vector2D(ReadOnlySpan<double> values)
		{
			if (values.Length < 2)
				throw new IndexOutOfRangeException();
			this = Unsafe.ReadUnaligned<Vector2D>(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetReference(values)));
		}

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
			sb.Append('>');
			return sb.ToString();
		}

		#region CopyTo
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void CopyTo(double[] array) => CopyTo(array, 0);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void CopyTo(double[] array, int index)
		{
			if (array is null)
				throw new ArgumentNullException(nameof(array), "Array is null");
			if (array.Length + index < 2)
				throw new ArgumentException("The number of elements in the current instance is greater than in the array.", nameof(array));
			if (array.Rank != 1)
				throw new RankException("Array is multidimensional.");
			Unsafe.WriteUnaligned(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetArrayDataReference(array)), this);
		}
		public readonly void CopyTo(Span<double> destination)
		{
			if (destination.Length < 2)
				throw new ArgumentException("Destination too short.", nameof(destination));
			Unsafe.WriteUnaligned(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetReference(destination)), this);
		}
		public readonly bool TryCopyTo(Span<double> destination)
		{
			if (destination.Length < 2)
				return false;
			Unsafe.WriteUnaligned(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetReference(destination)), this);
			return true;
		}
		#endregion

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly double Length()
		{
			if (Sse2.IsSupported)
			{
				fixed (void* p = &this)
				{
					Vector128<double> v = LoadVector128(p);
					if (Sse41.IsSupported)
						return Sse2.SqrtScalar(Sse41.DotProduct(v, v, 0x31)).ToScalar();
					Vector128<double> m = Sse2.Multiply(v, v);
					if (Sse3.IsSupported)
						return Sse2.SqrtScalar(Sse3.HorizontalAdd(m, m)).ToScalar();
					Vector128<double> m1 = Sse2.Shuffle(m.AsInt32(), 0b0100_1110).AsDouble();
					return Sse2.SqrtScalar(Sse2.Add(m, m1)).ToScalar();
				}
			}
			return Math.Sqrt(X * X + Y * Y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly double LengthSquared()
		{
			if (Sse2.IsSupported)
			{
				fixed (void* p = &this)
				{
					Vector128<double> v = LoadVector128(p);
					if (Sse41.IsSupported)
						return Sse41.DotProduct(v, v, 0x31).ToScalar();
					Vector128<double> m = Sse2.Multiply(v, v);
					if (Sse3.IsSupported)
						return Sse3.HorizontalAdd(m, m).ToScalar();
					Vector128<double> m1 = Sse2.Shuffle(m.AsInt32(), 0b0100_1110).AsDouble();
					return Sse2.Add(m, m1).ToScalar();
				}
			}
			return X * X + Y * Y;
		}

		public readonly override int GetHashCode() => HashCode.Combine(X, Y);
		public readonly override bool Equals(object? other) => other is Vector2D && Equals(other);
		public readonly bool Equals(Vector2D other) => this == other;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector128<double> LoadVector128(void* ptr) => Sse2.LoadVector128((double*)ptr);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void StoreVector128(void* ptr, Vector128<double> vector) => Sse2.Store((double*)ptr, vector);

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D operator -(Vector2D value)
		{
			if (!Sse2.IsSupported)
				return new Vector2D(-value.X, -value.Y);
			Vector128<double> v = LoadVector128(&value);
			Vector128<double> m = Vector128.Create(0x8000_0000_0000_0000, 0x8000_0000_0000_0000).AsDouble();
			StoreVector128(&value, Sse2.Xor(m, v));
			return value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D operator +(Vector2D left, Vector2D right)
		{
			if (!Sse2.IsSupported)
				return new Vector2D(left.X + right.X, left.Y + right.Y);
			Vector128<double> l = LoadVector128(&left);
			Vector128<double> r = LoadVector128(&right);
			StoreVector128(&left, Sse2.Add(l, r));
			return left;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D operator -(Vector2D left, Vector2D right)
		{
			if (!Sse2.IsSupported)
				return new Vector2D(left.X - right.X, left.Y - right.Y);
			Vector128<double> l = LoadVector128(&left);
			Vector128<double> r = LoadVector128(&right);
			StoreVector128(&left, Sse2.Subtract(l, r));
			return left;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D operator *(Vector2D left, Vector2D right)
		{
			if (!Sse2.IsSupported)
				return new Vector2D(left.X * right.X, left.Y * right.Y);
			Vector128<double> l = LoadVector128(&left);
			Vector128<double> r = LoadVector128(&right);
			StoreVector128(&left, Sse2.Multiply(l, r));
			return left;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D operator *(Vector2D left, double right)
		{
			if (!Sse2.IsSupported)
				return new Vector2D(left.X * right, left.Y * right);
			Vector128<double> l = LoadVector128(&left);
			Vector128<double> r = Vector128.Create(right);
			StoreVector128(&left, Sse2.Multiply(l, r));
			return left;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D operator *(double left, Vector2D right)
		{
			if (!Sse2.IsSupported)
				return new Vector2D(left * right.X, left * right.Y);
			Vector128<double> l = Vector128.Create(left);
			Vector128<double> r = LoadVector128(&right);
			StoreVector128(&right, Sse2.Multiply(l, r));
			return right;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D operator /(Vector2D left, Vector2D right)
		{
			if (!Sse2.IsSupported)
				return new Vector2D(left.X / right.X, left.Y / right.Y);
			Vector128<double> l = LoadVector128(&left);
			Vector128<double> r = LoadVector128(&right);
			StoreVector128(&left, Sse2.Divide(l, r));
			return left;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D operator /(Vector2D left, double right)
		{
			if (!Sse2.IsSupported)
				return new Vector2D(left.X / right, left.Y / right);
			Vector128<double> l = LoadVector128(&left);
			Vector128<double> r = Vector128.Create(right);
			StoreVector128(&left, Sse2.Divide(l, r));
			return left;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector2D left, Vector2D right)
		{
			if (!Sse2.IsSupported)
				return left.X == right.X && left.Y == right.Y;
			return (Sse2.MoveMask(Sse2.CompareEqual(LoadVector128(&left), LoadVector128(&right))) & 3) == 3;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector2D left, Vector2D right)
		{
			if (!Sse2.IsSupported)
				return left.X != right.X || left.Y != right.Y;
			return (Sse2.MoveMask(Sse2.CompareEqual(LoadVector128(&left), LoadVector128(&right))) & 3) != 3;
		}
		#endregion

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2D((double x, double y) value) => new Vector2D(value.x, value.y);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2D(Vector2 vector) => new Vector2D(vector.X, vector.Y);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector2(Vector2D vector) => new Vector2((float)vector.X, (float)vector.Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D Abs(Vector2D value)
		{
			const ulong mask = 0x7FFF_FFFF_FFFF_FFFF;
			if (!Sse2.IsSupported)
				return new Vector2D(Math.Abs(value.X), Math.Abs(value.Y));
			Vector128<double> v = LoadVector128(&value);
			v = Sse2.And(v.AsInt64(), Vector128.Create(mask, mask).AsInt64()).AsDouble();
			StoreVector128(&value, v);
			return value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D Clamp(Vector2D value, Vector2D min, Vector2D max) => Min(Max(value, min), max);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Distance(Vector2D value1, Vector2D value2) => (value1 - value2).Length();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double DistanceSquared(Vector2D value1, Vector2D value2) => (value1 - value2).LengthSquared();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Dot(Vector2D value1, Vector2D value2)
		{
			if (!Sse2.IsSupported)
				return value1.X * value2.X + value1.Y * value2.Y;
			Vector128<double> v1 = LoadVector128(&value1);
			Vector128<double> v2 = LoadVector128(&value2);
			if (Sse41.IsSupported)
				return Sse41.DotProduct(v1, v2, 0x31).ToScalar();
			Vector128<double> m = Sse2.Multiply(v1, v2);
			if (Sse3.IsSupported)
				return Sse3.HorizontalAdd(m, m).ToScalar();
			Vector128<double> m2 = Sse2.Shuffle(m.AsInt32(), 0b0100_1110).AsDouble();
			return Sse2.Add(m, m2).ToScalar();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D Lerp(Vector2D value1, Vector2D value2, double amount)
		{
			if (!Sse2.IsSupported)
				return value1 * (1 - amount) + value2 * amount;
			Vector128<double> v1 = LoadVector128(&value1);
			Vector128<double> v2 = LoadVector128(&value2);
			Vector128<double> a1 = Vector128.Create(1 - amount);
			Vector128<double> a2 = Vector128.Create(amount);
			Vector128<double> v;
			Vector128<double> i1 = Sse2.Multiply(v2, a2);
			if (Fma.IsSupported)
				v = Fma.MultiplyAdd(v1, a1, i1);
			else
			{
				Vector128<double> i0 = Sse2.Multiply(v1, a1);
				v = Sse2.Add(i0, i1);
			}
			StoreVector128(&value1, v);
			return value1;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D Max(Vector2D value1, Vector2D value2)
		{
			if (!Sse2.IsSupported)
				return new Vector2D((value1.X > value2.X) ? value1.X : value2.X, (value1.Y > value2.Y) ? value1.Y : value2.Y);
			Vector128<double> v1 = LoadVector128(&value1);
			Vector128<double> v2 = LoadVector128(&value2);
			StoreVector128(&value1, Sse2.Max(v1, v2));
			return value1;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D Min(Vector2D value1, Vector2D value2)
		{
			if (!Sse2.IsSupported)
				return new Vector2D((value1.X < value2.X) ? value1.X : value2.X, (value1.Y < value2.Y) ? value1.Y : value2.Y);
			Vector128<double> v1 = LoadVector128(&value1);
			Vector128<double> v2 = LoadVector128(&value2);
			StoreVector128(&value1, Sse2.Min(v1, v2));
			return value1;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D Normalize(Vector2D value)
		{
			if (!Sse2.IsSupported)
				return value / value.Length();
			Vector128<double> v = LoadVector128(&value);
			Vector128<double> dp;
			if (Sse41.IsSupported)
				dp = Sse41.DotProduct(v, v, 0x33);
			else
			{
				Vector128<double> m = Sse2.Multiply(v, v);
				if (Sse3.IsSupported)
					dp = Sse3.HorizontalAdd(m, m);
				else
				{
					Vector128<double> a = Sse2.Shuffle(v.AsInt32(), 0x4E).AsDouble();
					dp = Sse2.Add(a, a);
				}
			}
			Vector128<double> mag = Sse2.SqrtScalar(dp);
			mag = Sse.MoveLowToHigh(mag.AsSingle(), mag.AsSingle()).AsDouble();
			StoreVector128(&value, Sse2.Divide(v, mag));
			return value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D Reflect(Vector2D vector, Vector2D normal)
		{
			if (!Sse2.IsSupported)
				return vector - 2 * Dot(vector, normal) * normal;
			Vector128<double> v = LoadVector128(&vector);
			Vector128<double> n = LoadVector128(&normal);
			Vector128<double> dp;
			if (Sse41.IsSupported)
				dp = Sse41.DotProduct(v, n, 0x33);
			else
			{
				Vector128<double> m = Sse2.Multiply(v, n);
				if (Sse3.IsSupported)
					dp = Sse3.HorizontalAdd(m, m);
				else
					dp = Sse2.Add(m, Sse2.Shuffle(m.AsInt32(), 0b0100_1110).AsDouble());
			}
			dp = Sse2.Add(dp, dp);
			dp = Sse2.Multiply(dp, n);
			StoreVector128(&vector, Sse2.Subtract(v, dp));
			return vector;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D SquareRoot(Vector2D vector)
		{
			if (!Sse2.IsSupported)
				return new Vector2D(System.Math.Sqrt(vector.X), System.Math.Sqrt(vector.Y));
			StoreVector128(&vector, Sse2.Sqrt(LoadVector128(&vector)));
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D Transform(Vector2D position, Matrix3x2D matrix)
		{
			return new Vector2D(
				(position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M31,
				(position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M32
			);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D Transform(Vector2D position, Matrix4x4D matrix)
		{
			return new Vector2D(
				(position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41,
				(position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42
			);
		}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D Transform(Vector2D value, QuaternionD rotation)
		{
			double x2 = rotation.X + rotation.X;
			double y2 = rotation.Y + rotation.Y;
			double z2 = rotation.Z + rotation.Z;

			double wz2 = rotation.W * z2;
			double xx2 = rotation.X * x2;
			double xy2 = rotation.X * y2;
			double yy2 = rotation.Y * y2;
			double zz2 = rotation.Z * z2;

			return new Vector2D(
				value.X * (1.0f - yy2 - zz2) + value.Y * (xy2 - wz2),
				value.X * (xy2 + wz2) + value.Y * (1.0f - xx2 - zz2)
			);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D TransformNormal(Vector2D normal, Matrix3x2D matrix)
		{
			return new Vector2D(
				(normal.X * matrix.M11) + (normal.Y * matrix.M21),
				(normal.X * matrix.M12) + (normal.Y * matrix.M22)
			);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2D TransformNormal(Vector2D normal, Matrix4x4D matrix)
		{
			return new Vector2D(
				(normal.X * matrix.M11) + (normal.Y * matrix.M21),
				(normal.X * matrix.M12) + (normal.Y * matrix.M22)
			);
		}
	}
}
