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
	public unsafe struct Vector4D : ISpanFormattable, IEquatable<Vector4D>
	{
		public double X;
		public double Y;
		public double Z;
		public double W;

		public static Vector4D One { get; } = new Vector4D(1);
		public static Vector4D Zero { get; } = new Vector4D(0);
		public static Vector4D UnitX { get; } = new Vector4(1, 0, 0, 0);
		public static Vector4D UnitY { get; } = new Vector4(0, 1, 0, 0);
		public static Vector4D UnitZ { get; } = new Vector4(0, 0, 1, 0);
		public static Vector4D UnitW { get; } = new Vector4(0, 0, 0, 1);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4D(double x, double y, double z, double w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4D(double value) : this(value, value, value, value) { }
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4D(Vector2D vector, double z, double w) : this(vector.X, vector.Y, z, w) { }
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4D(Vector3D vector, double w) : this(vector.X, vector.Y, vector.Z, w) { }

		public Vector4D(ReadOnlySpan<double> values)
		{
			if (values.Length < 4)
				throw new IndexOutOfRangeException();
			this = Unsafe.ReadUnaligned<Vector4D>(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetReference(values)));
		}

		#region CopyTo
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void CopyTo(double[] array) => CopyTo(array, 0);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void CopyTo(double[] array, int index)
		{
			if (array is null)
				throw new ArgumentNullException(nameof(array), "Array is null");
			if (array.Length + index < 4)
				throw new ArgumentException("The number of elements in the current instance is greater than in the array.", nameof(array));
			if (array.Rank != 1)
				throw new RankException("Array is multidimensional.");
			Unsafe.WriteUnaligned(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetArrayDataReference(array)), this);
		}
		public readonly void CopyTo(Span<double> destination)
		{
			if (destination.Length < 4)
				throw new ArgumentException("Destination too short.", nameof(destination));
			Unsafe.WriteUnaligned(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetReference(destination)), this);
		}
		public readonly bool TryCopyTo(Span<double> destination)
		{
			if (destination.Length < 4)
				return false;
			Unsafe.WriteUnaligned(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetReference(destination)), this);
			return true;
		}
		#endregion

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
			sb.Append(separator);
			sb.Append(' ');
			sb.Append(W.ToString(format, provider));
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
			if (!WriteSeparator(destination[charsWritten..], out tempWritten, separator))
				return false;
			charsWritten += tempWritten;
			if (!W.TryFormat(destination[charsWritten..], out tempWritten, format, provider))
				return false;
			charsWritten += tempWritten;
			if (destination.Length < 1 + charsWritten)
				return false;
			destination[charsWritten] = '>';
			charsWritten++;
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly double Length()
		{
			if (Avx.IsSupported)
			{
				void* ptr = Unsafe.AsPointer(ref Unsafe.AsRef(in this));
				Vector256<double> v = Avx.LoadVector256((double*)ptr);
				Vector256<double> m = Avx.Multiply(v, v);
				Vector256<double> a = Avx.HorizontalAdd(m, m);
				return Sse2.SqrtScalar(Sse2.AddScalar(a.GetLower(), a.GetUpper())).ToScalar();
			}
			return Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly double LengthSquared()
		{
			if (Avx.IsSupported)
			{
				void* ptr = Unsafe.AsPointer(ref Unsafe.AsRef(in this));
				Vector256<double> v = Avx.LoadVector256((double*)ptr);
				Vector256<double> m = Avx.Multiply(v, v);
				Vector256<double> a = Avx.HorizontalAdd(m, m);
				return Sse2.AddScalar(a.GetLower(), a.GetUpper()).ToScalar();
			}
			return X * X + Y * Y + Z * Z + W * W;
		}

		public readonly override int GetHashCode() => HashCode.Combine(X, Y, Z, W);
		public readonly override bool Equals(object? other) => other is Vector4D v && Equals(v);
		public readonly bool Equals(Vector4D other) => this == other;

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public static Vector4D operator -(Vector4D value)
		{
			const ulong mask = 0x8000_0000_0000_0000;
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&value, Avx.Xor(Vector256.Create(mask).AsDouble(), Avx.LoadVector256(&value.X)));
				return value;
			}
			return new Vector4D(-value.X, -value.Y, -value.Z, -value.W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public static Vector4D operator +(Vector4D left, Vector4D right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&left, Avx.Add(Avx.LoadVector256(&left.X), Avx.LoadVector256(&right.X)));
				return left;
			}
			return new Vector4D(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public static Vector4D operator -(Vector4D left, Vector4D right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&left, Avx.Subtract(Avx.LoadVector256(&left.X), Avx.LoadVector256(&right.X)));
				return left;
			}
			return new Vector4D(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public static Vector4D operator *(Vector4D left, Vector4D right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&left, Avx.Multiply(Avx.LoadVector256(&left.X), Avx.LoadVector256(&right.X)));
				return left;
			}
			return new Vector4D(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public static Vector4D operator *(Vector4D left, double right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&left, Avx.Multiply(Avx.LoadVector256(&left.X), Vector256.Create(right)));
				return left;
			}
			return new Vector4D(left.X * right, left.Y * right, left.Z * right, left.W * right);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public static Vector4D operator *(double left, Vector4D right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&right, Avx.Multiply(Vector256.Create(left), Avx.LoadVector256(&right.X)));
				return right;
			}
			return new Vector4D(left * right.X, left * right.Y, left * right.Z, left * right.W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4D operator /(Vector4D left, Vector4D right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&left, Avx.Divide(Avx.LoadVector256(&left.X), Avx.LoadVector256(&right.X)));
				return left;
			}
			return new Vector4D(left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4D operator /(Vector4D left, double right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&left, Avx.Divide(Avx.LoadVector256(&left.X), Vector256.Create(right)));
				return left;
			}
			return new Vector4D(left.X / right, left.Y / right, left.Z / right, left.W / right);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector4D left, Vector4D right)
		{
			if (Avx.IsSupported)
				return VectorMath.Equal(Avx.LoadVector256(&left.X), Avx.LoadVector256(&right.X));
			return left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector4D left, Vector4D right)
		{
			if (Avx.IsSupported)
				return VectorMath.NotEqual(Avx.LoadVector256(&left.X), Avx.LoadVector256(&right.X));
			return left.X != right.X || left.Y != right.Y || left.Z != right.Z || left.W != right.W;
		}
		#endregion Operators

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector4D((double x, double y, double z, double w) value) => new Vector4D(value.x, value.y, value.z, value.w);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector4D(Vector4 vector) => new Vector4D(vector.X, vector.Y, vector.Z, vector.W);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector4(Vector4D vector) => new Vector4((float)vector.X, (float)vector.Y, (float)vector.Z, (float)vector.W);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4D Abs(Vector4D value)
		{
			const ulong mask = 0x7FFF_FFFF_FFFF_FFFF;
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&value, Avx.And(Avx.LoadVector256(&value.X), Vector256.Create(mask).AsDouble()));
				return value;
			}
			return new Vector4D(Math.Abs(value.X), Math.Abs(value.Y), Math.Abs(value.Z), Math.Abs(value.W));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4D Clamp(Vector4D value, Vector4D min, Vector4D max)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&value, Avx.Max(Avx.LoadVector256(&min.X), Avx.Min(Avx.LoadVector256(&max.X), Avx.LoadVector256(&value.X))));
				return value;
			}
			return Max(min, Min(max, value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Distance(Vector4D value1, Vector4D value2)
		{
			if (Avx.IsSupported)
			{
				Vector256<double> v = Avx.Subtract(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X));
				Vector256<double> m = Avx.Multiply(v, v);
				m = Avx.HorizontalAdd(m, m);
				return Sse2.SqrtScalar(Sse2.AddScalar(m.GetLower(), m.GetUpper())).ToScalar();
			}
			return Math.Sqrt((value1 - value2).LengthSquared());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double DistanceSquared(Vector4D value1, Vector4D value2)
		{
			if (Avx.IsSupported)
			{
				Vector256<double> v = Avx.Subtract(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X));
				Vector256<double> m = Avx.Multiply(v, v);
				Vector256<double> a = Avx.HorizontalAdd(m, m);
				return Sse2.AddScalar(a.GetLower(), a.GetUpper()).ToScalar();
			}
			return (value1 - value2).LengthSquared();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Dot(Vector4D value1, Vector4D value2)
		{
			if (Avx.IsSupported)
			{
				Vector256<double> m = Avx.Multiply(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X));
				Vector256<double> a = Avx.HorizontalAdd(m, m);
				return Sse2.AddScalar(a.GetLower(), a.GetUpper()).ToScalar();
			}
			return value1.X * value2.X + value1.Y * value2.Y + value1.Z * value2.Z + value1.W * value2.W;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4D Lerp(Vector4D value1, Vector4D value2, double amount)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&value1, Avx.Add(Avx.Multiply(Avx.LoadVector256(&value1.X), Vector256.Create(1 - amount)), Avx.Multiply(Avx.LoadVector256(&value2.X), Vector256.Create(amount))));
				return value1;
			}
			return (1 - amount) * value1 + amount * value2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4D Max(Vector4D value1, Vector4D value2)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&value1, Avx.Max(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X)));
				return value1;
			}
			return new Vector4D(Math.Max(value1.X, value2.X), Math.Max(value1.Y, value2.Y), Math.Max(value1.Z, value2.Z), Math.Max(value1.W, value2.W));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4D Min(Vector4D value1, Vector4D value2)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&value1, Avx.Min(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X)));
				return value1;
			}
			return new Vector4D(Math.Min(value1.X, value2.X), Math.Min(value1.Y, value2.Y), Math.Min(value1.Z, value2.Z), Math.Min(value1.W, value2.W));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4D Normalize(Vector4D value)
		{
			if (Avx.IsSupported)
			{
				Vector256<double> v = Avx.LoadVector256((double*)&value);
				Vector256<double> m = Avx.Multiply(v, v);
				Vector256<double> a = Avx.HorizontalAdd(m, m);
				Vector256<double> d = Avx.Divide(v, Vector256.Create(Sse2.SqrtScalar(Sse2.AddScalar(a.GetLower(), a.GetUpper())).ToScalar()));
				Avx.Store((double*)&value, d);
				return value;
			}
			return value / value.Length();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4D Reflect(Vector4D vector, Vector4D normal)
		{
			if (Avx.IsSupported)
			{
				Vector256<double> v = Avx.LoadVector256(&vector.X);
				Vector256<double> n = Avx.LoadVector256(&normal.X);
				Vector256<double> m = Avx.Multiply(v, n);
				Vector256<double> a = Avx.HorizontalAdd(m, m);
				Avx.Store((double*)&vector, Avx.Subtract(v, Avx.Multiply(Vector256.Create(2 * Sse2.AddScalar(a.GetLower(), a.GetUpper()).ToScalar()), n)));
				return vector;
			}
			return vector - 2 * Dot(vector, normal) * normal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4D SquareRoot(Vector4D value)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&value, Avx.Sqrt(Avx.LoadVector256(&value.X)));
				return value;
			}
			return new Vector4D(Math.Sqrt(value.X), Math.Sqrt(value.Y), Math.Sqrt(value.Z), Math.Sqrt(value.W));
		}

#pragma warning disable IDE0079, IDE0060 // Remove unused parameter
		public static Vector4 Transform(Vector2D position, Matrix4x4D matrix) => throw new NotImplementedException();
		public static Vector4 Transform(Vector2D value, Quaternion rotation) => throw new NotImplementedException();
		public static Vector4 Transform(Vector3D position, Matrix4x4 matrix) => throw new NotImplementedException();
		public static Vector4 Transform(Vector3D value, QuaternionD rotation) => throw new NotImplementedException();
		public static Vector4 Transform(Vector4D vector, Matrix4x4D matrix) => throw new NotImplementedException();
		public static Vector4 Transform(Vector4D value, QuaternionD rotation) => throw new NotImplementedException();
#pragma warning restore IDE0079, IDE0060 // Remove unused parameter
	}
}
