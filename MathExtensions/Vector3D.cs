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
	public unsafe struct Vector3D : IEquatable<Vector3D>, ISpanFormattable
	{
		public double X, Y, Z;

		public static Vector3D Zero { get; } = new Vector3D(0);
		public static Vector3D One { get; } = new Vector3D(1);
		public static Vector3D UnitX { get; } = new Vector3D(1, 0, 0);
		public static Vector3D UnitY { get; } = new Vector3D(0, 1, 0);
		public static Vector3D UnitZ { get; } = new Vector3D(0, 0, 1);

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

		public readonly override string ToString() => ToString("G", CultureInfo.CurrentCulture);
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
		public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
		{
			static bool WriteSeparator(Span<char> destination, out int written, ReadOnlySpan<char> separator)
			{
				written = 0;
				if (destination.Length < 1 + separator.Length)
					return false;
				separator.CopyTo(destination);
				written += separator.Length;
				destination[written] = ' ';
				written++;
				return true;
			}
			ReadOnlySpan<char> separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;
			Unsafe.SkipInit(out charsWritten);
			if (destination.Length < 1)
				return false;
			charsWritten = 0;
			Unsafe.SkipInit(out int tempWritten);
			destination[0] = '<';
			charsWritten++;
			if (!X.TryFormat(destination[charsWritten..], out tempWritten, format, provider))
				return false;
			charsWritten += tempWritten;
			if (!WriteSeparator(destination[charsWritten..], out tempWritten, separator))
				return false;
			charsWritten += tempWritten;
			if (!Y.TryFormat(destination[charsWritten..], out tempWritten, format, provider))
				return false;
			charsWritten += tempWritten;
			if (!WriteSeparator(destination[charsWritten..], out tempWritten, separator))
				return false;
			charsWritten += tempWritten;
			if (!Z.TryFormat(destination[charsWritten..], out tempWritten, format, provider))
				return false;
			charsWritten += tempWritten;
			if (destination.Length < 1 + charsWritten)
				return false;
			destination[charsWritten] = '>';
			charsWritten++;
			return true;
		}

		#region CopyTo
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void CopyTo(double[] array) => CopyTo(array, 0);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void CopyTo(double[] array, int index)
		{
			if (array is null)
				throw new ArgumentNullException(nameof(array), "Array is null");
			if (array.Length + index < 3)
				throw new ArgumentException("The number of elements in the current instance is greater than in the array.", nameof(array));
			if (array.Rank != 1)
				throw new RankException("Array is multidimensional.");
			Unsafe.WriteUnaligned(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetArrayDataReference(array)), this);
		}
		public readonly void CopyTo(Span<double> destination)
		{
			if (destination.Length < 3)
				throw new ArgumentException("Destination too short.", nameof(destination));
			Unsafe.WriteUnaligned(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetReference(destination)), this);
		}
		public readonly bool TryCopyTo(Span<double> destination)
		{
			if (destination.Length < 3)
				return false;
			Unsafe.WriteUnaligned(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetReference(destination)), this);
			return true;
		}
		#endregion

		public readonly override int GetHashCode() => HashCode.Combine(X, Y, Z);
		public readonly override bool Equals(object? other) => other is Vector3D o && Equals(o);
		public readonly bool Equals(Vector3D other) => this == other;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly double Length()
		{
			if (Avx.IsSupported)
			{
				fixed (void* p = &this)
				{
					Vector256<double> v = Avx.LoadVector256((double*)p);
					Vector256<double> m = Avx.Multiply(v, v);
					return Sse2.SqrtScalar(Sse2.AddScalar(Sse3.HorizontalAdd(m.GetLower(), m.GetLower()), m.GetUpper())).ToScalar();
				}
			}
			return Math.Sqrt(LengthSquared());
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly double LengthSquared()
		{
			if (Avx.IsSupported)
			{
				fixed (void* p = &this)
				{
					Vector256<double> v = Avx.LoadVector256((double*)p);
					Vector256<double> m = Avx.Multiply(v, v);
					return Sse2.AddScalar(Sse3.HorizontalAdd(m.GetLower(), m.GetLower()), m.GetUpper()).ToScalar();
				}
			}
			return X * X + Y * Y + Z * Z;
		}

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D operator -(Vector3D value)
		{
			if (Avx.IsSupported)
			{
				ulong mask = 0x8000_0000_0000_0000;
				Avx.Store(&value.X, Avx.Xor(Avx.LoadVector256(&value.X), Vector256.Create(mask).AsDouble()));
				return value;
			}
			Vector3D v;
			v.X = -value.X;
			v.Y = -value.Y;
			v.Z = -value.Z;
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D operator +(Vector3D left, Vector3D right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store(&left.X, Avx.Add(Avx.LoadVector256(&left.X), Avx.LoadVector256(&right.X)));
				return left;
			}
			Vector3D v;
			v.X = left.X + right.X;
			v.Y = left.Y + right.Y;
			v.Z = left.Z + right.Z;
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D operator -(Vector3D left, Vector3D right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store(&left.X, Avx.Subtract(Avx.LoadVector256(&left.X), Avx.LoadVector256(&right.X)));
				return left;
			}
			Vector3D v;
			v.X = left.X - right.X;
			v.Y = left.Y - right.Y;
			v.Z = left.Z - right.Z;
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D operator *(Vector3D left, Vector3D right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store(&left.X, Avx.Multiply(Avx.LoadVector256(&left.X), Avx.LoadVector256(&right.X)));
				return left;
			}
			Vector3D v;
			v.X = left.X * right.X;
			v.Y = left.Y * right.Y;
			v.Z = left.Z * right.Z;
			return v;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D operator *(Vector3D left, double right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store(&left.X, Avx.Multiply(Avx.LoadVector256(&left.X), Vector256.Create(right)));
				return left;
			}
			Vector3D v;
			v.X = left.X * right;
			v.Y = left.Y * right;
			v.Z = left.Z * right;
			return v;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D operator *(double left, Vector3D right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store(&right.X, Avx.Multiply(Vector256.Create(left), Avx.LoadVector256(&right.X)));
				return right;
			}
			Vector3D v;
			v.X = left * right.X;
			v.Y = left * right.Y;
			v.Z = left * right.Z;
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D operator /(Vector3D left, Vector3D right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store(&left.X, Avx.Divide(Avx.LoadVector256(&left.X), Avx.LoadVector256(&right.X)));
				return left;
			}
			Vector3D v;
			v.X = left.X / right.X;
			v.Y = left.Y / right.Y;
			v.Z = left.Z / right.Z;
			return v;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D operator /(Vector3D left, double right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store(&left.X, Avx.Divide(Avx.LoadVector256(&left.X), Vector256.Create(right)));
				return left;
			}
			Vector3D v;
			v.X = left.X / right;
			v.Y = left.Y / right;
			v.Z = left.Z / right;
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector3D left, Vector3D right)
		{
			if (Avx.IsSupported)
				return VectorMath.Equal(Avx.LoadVector256(&left.X), Avx.LoadVector256(&right.X));
			return left.X == right.X && left.Y == right.Y && left.Z == right.Z;

		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector3D left, Vector3D right)
		{
			if (Avx.IsSupported)
				return VectorMath.NotEqual(Avx.LoadVector256(&left.X), Avx.LoadVector256(&right.X));
			return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
		}
		#endregion

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D TransformNormal(Vector3D normal, Matrix4x4D matrix)
		{
			return new Vector3D(
				(normal.X * matrix.M11) + (normal.Y * matrix.M21) + (normal.Z * matrix.M31),
				(normal.X * matrix.M12) + (normal.Y * matrix.M22) + (normal.Z * matrix.M32),
				(normal.X * matrix.M13) + (normal.Y * matrix.M23) + (normal.Z * matrix.M33)
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector3D((double x, double y, double z) value) => Unsafe.Read<Vector3D>(&value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector3D(Vector3 vector)
		{
			if (Avx.IsSupported)
			{
				Vector3D* v = (Vector3D*)&vector;
				Avx.Store(&v->X, Avx.ConvertToVector256Double(Sse.LoadVector128(&vector.X)));
				return *v;
			}
			Vector3D vec;
			vec.X = vector.X;
			vec.Y = vector.Y;
			vec.Z = vector.Z;
			return vec;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector3(Vector3D vector)
		{
			if (Avx.IsSupported)
			{
				Vector3* v = (Vector3*)&vector;
				Sse.Store(&v->X, Avx.ConvertToVector128Single(Avx.LoadVector256(&vector.X)));
				return *v;
			}
			Vector3 vec;
			vec.X = (float)vector.X;
			vec.Y = (float)vector.Y;
			vec.Z = (float)vector.Z;
			return vec;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D Abs(Vector3D value)
		{
			if (Avx.IsSupported)
			{
				ulong mask = 0x7FFF_FFFF_FFFF_FFFF;
				Avx.Store(&value.X, Avx.And(Avx.LoadVector256(&value.X), Vector256.Create(mask).AsDouble()));
				return value;
			}
			Vector3D v;
			v.X = Math.Abs(value.X);
			v.Y = Math.Abs(value.Y);
			v.Z = Math.Abs(value.Z);
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D Clamp(Vector3D value, Vector3D min, Vector3D max)
		{
			if (Avx.IsSupported)
			{
				Avx.Store(&value.X, Avx.Min(Avx.Max(Avx.LoadVector256(&value.X), Avx.LoadVector256(&min.X)), Avx.LoadVector256(&max.X)));
				return value;
			}
			return Min(Max(value, min), max);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D Cross(Vector3D vector1, Vector3D vector2)
		{
			return new Vector3D(
				(vector1.Y * vector2.Z) - (vector1.Z * vector2.Y),
				(vector1.Z * vector2.X) - (vector1.X * vector2.Z),
				(vector1.X * vector2.Y) - (vector1.Y * vector2.X)
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Distance(Vector3D value1, Vector3D value2)
		{
			if (Avx.IsSupported)
			{
				Vector256<double> v = Avx.Subtract(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X));
				Vector256<double> m = Avx.Multiply(v, v);
				return Sse2.SqrtScalar(Sse2.AddScalar(Sse3.HorizontalAdd(m.GetLower(), m.GetLower()), m.GetUpper())).ToScalar();
			}
			return Math.Sqrt(DistanceSquared(value1, value2));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double DistanceSquared(Vector3D value1, Vector3D value2)
		{
			if (Avx.IsSupported)
			{
				Vector256<double> v = Avx.Subtract(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X));
				Vector256<double> m = Avx.Multiply(v, v);
				return Sse2.AddScalar(Sse3.HorizontalAdd(m.GetLower(), m.GetLower()), m.GetUpper()).ToScalar();
			}
			return (value1 - value2).LengthSquared();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Dot(Vector3D value1, Vector3D value2)
		{
			if (Avx.IsSupported)
			{
				Vector256<double> m = Avx.Multiply(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X));
				return Sse2.AddScalar(Sse3.HorizontalAdd(m.GetLower(), m.GetLower()), m.GetUpper()).ToScalar();
			}
			return value1.X * value2.X + value1.Y * value2.Y + value1.Z * value2.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D Lerp(Vector3D value1, Vector3D value2, double amount)
		{
			if (Avx.IsSupported)
			{
				Vector256<double> a1 = Vector256.Create(1 - amount);
				Vector256<double> a2 = Vector256.Create(amount);

				Vector256<double> m2 = Avx.Multiply(Avx.LoadVector256(&value2.X), a2);
				if (Fma.IsSupported)
				{
					Avx.Store(&value1.X, Fma.MultiplyAdd(Avx.LoadVector256(&value1.X), a1, m2));
					return value1;
				}
				Vector256<double> m1 = Avx.Multiply(Avx.LoadVector256(&value1.X), a1);
				Avx.Store(&value1.X, Avx.Add(m1, m2));
				return value1;
			}
			return (value1 * (1 - amount)) + value2 * amount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D Max(Vector3D value1, Vector3D value2)
		{
			if (Avx.IsSupported)
			{
				Avx.Store(&value1.X, Avx.Max(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X)));
				return value1;
			}
			return new Vector3D(
				(value1.X > value2.X) ? value1.X : value2.X,
				(value1.Y > value2.Y) ? value1.Y : value2.Y,
				(value1.Z > value2.Z) ? value1.Z : value2.Z
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D Min(Vector3D value1, Vector3D value2)
		{
			if (Avx.IsSupported)
			{
				Avx.Store(&value1.X, Avx.Min(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X)));
				return value1;
			}
			return new Vector3D(
				(value1.X < value2.X) ? value1.X : value2.X,
				(value1.Y < value2.Y) ? value1.Y : value2.Y,
				(value1.Z < value2.Z) ? value1.Z : value2.Z
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D Normalize(Vector3D value)
		{
			if (Avx.IsSupported)
			{
				Vector256<double> v = Avx.LoadVector256((double*)&value);
				Vector256<double> m = Avx.Multiply(v, v);
				Vector256<double> a = Vector256.Create(Sse2.SqrtScalar(Sse2.AddScalar(Sse3.HorizontalAdd(m.GetLower(), m.GetLower()), m.GetUpper())).ToScalar());
				Avx.Store((double*)&value, Avx.Divide(v, a));
				return value;
			}
			return value / value.Length();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D Reflect(Vector3D vector, Vector3D normal)
		{
			if (Avx.IsSupported)
			{
				Vector256<double> v = Avx.LoadVector256(&vector.X);
				Vector256<double> n = Avx.LoadVector256(&normal.X);

				Vector256<double> m = Avx.Multiply(v, n);
				Avx.Store(&vector.X, Avx.Subtract(v, Avx.Multiply(Vector256.Create(2 * Sse2.AddScalar(Sse3.HorizontalAdd(m.GetLower(), m.GetLower()), m.GetUpper()).ToScalar()), n)));
				return vector;
			}
			return vector - 2 * Dot(vector, normal) * normal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D SquareRoot(Vector3D vector)
		{
			if (Avx.IsSupported)
			{
				Avx.Store(&vector.X, Avx.Sqrt(Avx.LoadVector256(&vector.X)));
				return vector;
			}
			vector.X = Math.Sqrt(vector.X);
			vector.Y = Math.Sqrt(vector.Y);
			vector.Z = Math.Sqrt(vector.Z);
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D Transform(Vector3D position, Matrix4x4D matrix)
		{
			return new Vector3D(
				(position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41,
				(position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42,
				(position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D Transform(Vector3D value, QuaternionD rotation)
		{
			double x2 = rotation.X + rotation.X;
			double y2 = rotation.Y + rotation.Y;
			double z2 = rotation.Z + rotation.Z;
			double wx2 = rotation.W * x2;
			double wy2 = rotation.W * y2;
			double wz2 = rotation.W * z2;
			double xx2 = rotation.X * x2;
			double xy2 = rotation.X * y2;
			double xz2 = rotation.X * z2;
			double yy2 = rotation.Y * y2;
			double yz2 = rotation.Y * z2;
			double zz2 = rotation.Z * z2;

			return new Vector3D(
				value.X * (1.0 - yy2 - zz2) + value.Y * (xy2 - wz2) + value.Z * (xz2 + wy2),
				value.X * (xy2 + wz2) + value.Y * (1.0 - xx2 - zz2) + value.Z * (yz2 - wx2),
				value.X * (xz2 - wy2) + value.Y * (yz2 + wx2) + value.Z * (1.0 - xx2 - yy2)
			);
		}
	}
}
