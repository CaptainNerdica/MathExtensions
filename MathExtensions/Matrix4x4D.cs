using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public unsafe struct Matrix4x4D : IEquatable<Matrix4x4D>
	{
		public double M11, M12, M13, M14;
		public double M21, M22, M23, M24;
		public double M31, M32, M33, M34;
		public double M41, M42, M43, M44;

		public static Matrix4x4D Identity { get; } = new Matrix4x4D(
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1);

		public Matrix4x4D(
			double m11, double m12, double m13, double m14,
			double m21, double m22, double m23, double m24,
			double m31, double m32, double m33, double m34,
			double m41, double m42, double m43, double m44)
		{
			M11 = m11; M12 = m12; M13 = m13; M14 = m14;
			M21 = m21; M22 = m22; M23 = m23; M24 = m24;
			M31 = m31; M32 = m32; M33 = m33; M34 = m34;
			M41 = m41; M42 = m42; M43 = m43; M44 = m44;
		}

		public Matrix4x4D(Matrix3x2D matrix)
		{
			M11 = matrix.M11;
			M12 = matrix.M12;
			M13 = 0;
			M14 = 0;

			M21 = matrix.M21;
			M22 = matrix.M22;
			M23 = 0;
			M24 = 0;

			M31 = 0;
			M32 = 0;
			M33 = 1;
			M34 = 0;

			M41 = matrix.M31;
			M42 = matrix.M32;
			M43 = 0;
			M44 = 1;
		}

		public readonly bool IsIdentity
		{
			get
			{
				return M11 == 1 && M22 == 1 && M33 == 1 && M44 == 1 &&
								   M12 == 0 && M13 == 0 && M14 == 0 &&
					   M21 == 0 && M23 == 0 && M24 == 0 &&
					   M31 == 0 && M32 == 0 && M34 == 0 &&
					   M41 == 0 && M42 == 0 && M43 == 0;
			}
		}

		public Vector3D Translation
		{
			readonly get => new Vector3D(M41, M42, M43);

			set
			{
				M41 = value.X;
				M42 = value.Y;
				M43 = value.Z;
			}
		}

		[SkipLocalsInit]
		public static Matrix4x4D operator -(Matrix4x4D value)
		{
			if (Avx.IsSupported)
			{
				const ulong mask = 0x8000_0000_0000_0000;
				Vector256<double> m = Vector256.Create(mask).AsDouble();
				Avx.Store(&value.M11, Avx.Xor(Avx.LoadVector256(&value.M11), m));
				Avx.Store(&value.M21, Avx.Xor(Avx.LoadVector256(&value.M21), m));
				Avx.Store(&value.M31, Avx.Xor(Avx.LoadVector256(&value.M31), m));
				Avx.Store(&value.M41, Avx.Xor(Avx.LoadVector256(&value.M41), m));
				return value;
			}
			Matrix4x4D matrix;
			matrix.M11 = -value.M11;
			matrix.M12 = -value.M12;
			matrix.M13 = -value.M13;
			matrix.M14 = -value.M14;
			matrix.M21 = -value.M21;
			matrix.M22 = -value.M22;
			matrix.M23 = -value.M23;
			matrix.M24 = -value.M24;
			matrix.M31 = -value.M31;
			matrix.M32 = -value.M32;
			matrix.M33 = -value.M33;
			matrix.M34 = -value.M34;
			matrix.M41 = -value.M41;
			matrix.M42 = -value.M42;
			matrix.M43 = -value.M43;
			matrix.M44 = -value.M44;
			return matrix;
		}

		[SkipLocalsInit]
		public static Matrix4x4D operator +(Matrix4x4D left, Matrix4x4D right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store(&left.M11, Avx.Add(Avx.LoadVector256(&left.M11), Avx.LoadVector256(&right.M11)));
				Avx.Store(&left.M21, Avx.Add(Avx.LoadVector256(&left.M21), Avx.LoadVector256(&right.M21)));
				Avx.Store(&left.M31, Avx.Add(Avx.LoadVector256(&left.M31), Avx.LoadVector256(&right.M31)));
				Avx.Store(&left.M41, Avx.Add(Avx.LoadVector256(&left.M41), Avx.LoadVector256(&right.M41)));
				return left;
			}
			Matrix4x4D m;

			m.M11 = left.M11 + right.M11;
			m.M12 = left.M12 + right.M12;
			m.M13 = left.M13 + right.M13;
			m.M14 = left.M14 + right.M14;
			m.M21 = left.M21 + right.M21;
			m.M22 = left.M22 + right.M22;
			m.M23 = left.M23 + right.M23;
			m.M24 = left.M24 + right.M24;
			m.M31 = left.M31 + right.M31;
			m.M32 = left.M32 + right.M32;
			m.M33 = left.M33 + right.M33;
			m.M34 = left.M34 + right.M34;
			m.M41 = left.M41 + right.M41;
			m.M42 = left.M42 + right.M42;
			m.M43 = left.M43 + right.M43;
			m.M44 = left.M44 + right.M44;

			return m;
		}

		[SkipLocalsInit]
		public static Matrix4x4D operator -(Matrix4x4D left, Matrix4x4D right)
		{
			if (Avx.IsSupported)
			{
				Avx.Store(&left.M11, Avx.Subtract(Avx.LoadVector256(&left.M11), Avx.LoadVector256(&right.M11)));
				Avx.Store(&left.M21, Avx.Subtract(Avx.LoadVector256(&left.M21), Avx.LoadVector256(&right.M21)));
				Avx.Store(&left.M31, Avx.Subtract(Avx.LoadVector256(&left.M31), Avx.LoadVector256(&right.M31)));
				Avx.Store(&left.M41, Avx.Subtract(Avx.LoadVector256(&left.M41), Avx.LoadVector256(&right.M41)));
				return left;
			}
			Matrix4x4D m;

			m.M11 = left.M11 - right.M11;
			m.M12 = left.M12 - right.M12;
			m.M13 = left.M13 - right.M13;
			m.M14 = left.M14 - right.M14;
			m.M21 = left.M21 - right.M21;
			m.M22 = left.M22 - right.M22;
			m.M23 = left.M23 - right.M23;
			m.M24 = left.M24 - right.M24;
			m.M31 = left.M31 - right.M31;
			m.M32 = left.M32 - right.M32;
			m.M33 = left.M33 - right.M33;
			m.M34 = left.M34 - right.M34;
			m.M41 = left.M41 - right.M41;
			m.M42 = left.M42 - right.M42;
			m.M43 = left.M43 - right.M43;
			m.M44 = left.M44 - right.M44;

			return m;
		}

		[SkipLocalsInit]
		public static Matrix4x4D operator *(Matrix4x4D value1, Matrix4x4D value2)
		{
			if (Avx.IsSupported)
			{
				Vector256<double> v21 = Avx.LoadVector256(&value2.M11);
				Vector256<double> v22 = Avx.LoadVector256(&value2.M21);
				Vector256<double> v23 = Avx.LoadVector256(&value2.M31);
				Vector256<double> v24 = Avx.LoadVector256(&value2.M41);

				Vector256<double> row = Avx.LoadVector256(&value1.M11);
				Avx.Store(&value1.M11,
					Avx.Add(Avx.Add(Avx.Multiply(Avx.Shuffle(row, row, 0x00), v21),
									Avx.Multiply(Avx.Shuffle(row, row, 0x55), v22)),
							Avx.Add(Avx.Multiply(Avx.Shuffle(row, row, 0xAA), v23),
									Avx.Multiply(Avx.Shuffle(row, row, 0xFF), v24))));

				row = Avx.LoadVector256(&value1.M21);
				Avx.Store(&value1.M21,
					Avx.Add(Avx.Add(Avx.Multiply(Avx.Shuffle(row, row, 0x00), v21),
									Avx.Multiply(Avx.Shuffle(row, row, 0x55), v22)),
							Avx.Add(Avx.Multiply(Avx.Shuffle(row, row, 0xAA), v23),
									Avx.Multiply(Avx.Shuffle(row, row, 0xFF), v24))));

				row = Avx.LoadVector256(&value1.M31);
				Avx.Store(&value1.M31,
					Avx.Add(Avx.Add(Avx.Multiply(Avx.Shuffle(row, row, 0x00), v21),
									Avx.Multiply(Avx.Shuffle(row, row, 0x55), v22)),
							Avx.Add(Avx.Multiply(Avx.Shuffle(row, row, 0xAA), v23),
									Avx.Multiply(Avx.Shuffle(row, row, 0xFF), v24))));

				row = Avx.LoadVector256(&value1.M41);
				Avx.Store(&value1.M41,
					Avx.Add(Avx.Add(Avx.Multiply(Avx.Shuffle(row, row, 0x00), v21),
									Avx.Multiply(Avx.Shuffle(row, row, 0x55), v22)),
							Avx.Add(Avx.Multiply(Avx.Shuffle(row, row, 0xAA), v23),
									Avx.Multiply(Avx.Shuffle(row, row, 0xFF), v24))));
				return value1;
			}
			Matrix4x4D m;

			// First row
			m.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31 + value1.M14 * value2.M41;
			m.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32 + value1.M14 * value2.M42;
			m.M13 = value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33 + value1.M14 * value2.M43;
			m.M14 = value1.M11 * value2.M14 + value1.M12 * value2.M24 + value1.M13 * value2.M34 + value1.M14 * value2.M44;

			// Second row
			m.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31 + value1.M24 * value2.M41;
			m.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32 + value1.M24 * value2.M42;
			m.M23 = value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33 + value1.M24 * value2.M43;
			m.M24 = value1.M21 * value2.M14 + value1.M22 * value2.M24 + value1.M23 * value2.M34 + value1.M24 * value2.M44;

			// Third row
			m.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31 + value1.M34 * value2.M41;
			m.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32 + value1.M34 * value2.M42;
			m.M33 = value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33 + value1.M34 * value2.M43;
			m.M34 = value1.M31 * value2.M14 + value1.M32 * value2.M24 + value1.M33 * value2.M34 + value1.M34 * value2.M44;

			// Fourth row
			m.M41 = value1.M41 * value2.M11 + value1.M42 * value2.M21 + value1.M43 * value2.M31 + value1.M44 * value2.M41;
			m.M42 = value1.M41 * value2.M12 + value1.M42 * value2.M22 + value1.M43 * value2.M32 + value1.M44 * value2.M42;
			m.M43 = value1.M41 * value2.M13 + value1.M42 * value2.M23 + value1.M43 * value2.M33 + value1.M44 * value2.M43;
			m.M44 = value1.M41 * value2.M14 + value1.M42 * value2.M24 + value1.M43 * value2.M34 + value1.M44 * value2.M44;

			return m;
		}

		[SkipLocalsInit]
		public static Matrix4x4D operator *(Matrix4x4D value1, double value2)
		{
			if (Avx.IsSupported)
			{
				Vector256<double> v = Vector256.Create(value2);
				Avx.Store(&value1.M11, Avx.Multiply(Avx.LoadVector256(&value1.M11), v));
				Avx.Store(&value1.M21, Avx.Multiply(Avx.LoadVector256(&value1.M21), v));
				Avx.Store(&value1.M31, Avx.Multiply(Avx.LoadVector256(&value1.M31), v));
				Avx.Store(&value1.M41, Avx.Multiply(Avx.LoadVector256(&value1.M41), v));
				return value1;
			}
			Matrix4x4D m;

			m.M11 = value1.M11 * value2;
			m.M12 = value1.M12 * value2;
			m.M13 = value1.M13 * value2;
			m.M14 = value1.M14 * value2;
			m.M21 = value1.M21 * value2;
			m.M22 = value1.M22 * value2;
			m.M23 = value1.M23 * value2;
			m.M24 = value1.M24 * value2;
			m.M31 = value1.M31 * value2;
			m.M32 = value1.M32 * value2;
			m.M33 = value1.M33 * value2;
			m.M34 = value1.M34 * value2;
			m.M41 = value1.M41 * value2;
			m.M42 = value1.M42 * value2;
			m.M43 = value1.M43 * value2;
			m.M44 = value1.M44 * value2;
			return m;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4D operator *(double value1, Matrix4x4D value2) => value2 * value1;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4D operator /(Matrix4x4D value1, double value2) => value1 * (1 / value2);

		public static bool operator ==(Matrix4x4D value1, Matrix4x4D value2)
		{
			if (Avx.IsSupported)
			{
				return VectorMath.Equal(Avx.LoadVector256(&value1.M11), Avx.LoadVector256(&value2.M11)) &&
					   VectorMath.Equal(Avx.LoadVector256(&value1.M21), Avx.LoadVector256(&value2.M21)) &&
					   VectorMath.Equal(Avx.LoadVector256(&value1.M31), Avx.LoadVector256(&value2.M31)) &&
					   VectorMath.Equal(Avx.LoadVector256(&value1.M41), Avx.LoadVector256(&value2.M41));
			}

			return (value1.M11 == value2.M11 && value1.M22 == value2.M22 && value1.M33 == value2.M33 && value1.M44 == value2.M44 && // Check diagonal element first for early out.
					value1.M12 == value2.M12 && value1.M13 == value2.M13 && value1.M14 == value2.M14 && value1.M21 == value2.M21 &&
					value1.M23 == value2.M23 && value1.M24 == value2.M24 && value1.M31 == value2.M31 && value1.M32 == value2.M32 &&
					value1.M34 == value2.M34 && value1.M41 == value2.M41 && value1.M42 == value2.M42 && value1.M43 == value2.M43);
		}
		public static bool operator !=(Matrix4x4D value1, Matrix4x4D value2)
		{
			if (Avx.IsSupported)
			{
				return VectorMath.NotEqual(Avx.LoadVector256(&value1.M11), Avx.LoadVector256(&value2.M11)) &&
					   VectorMath.NotEqual(Avx.LoadVector256(&value1.M21), Avx.LoadVector256(&value2.M21)) &&
					   VectorMath.NotEqual(Avx.LoadVector256(&value1.M31), Avx.LoadVector256(&value2.M31)) &&
					   VectorMath.NotEqual(Avx.LoadVector256(&value1.M41), Avx.LoadVector256(&value2.M41));
			}

			return (value1.M11 != value2.M11 || value1.M12 != value2.M12 || value1.M13 != value2.M13 || value1.M14 != value2.M14 ||
					value1.M21 != value2.M21 || value1.M22 != value2.M22 || value1.M23 != value2.M23 || value1.M24 != value2.M24 ||
					value1.M31 != value2.M31 || value1.M32 != value2.M32 || value1.M33 != value2.M33 || value1.M34 != value2.M34 ||
					value1.M41 != value2.M41 || value1.M42 != value2.M42 || value1.M43 != value2.M43 || value1.M44 != value2.M44);
		}

		public override int GetHashCode()
		{
			HashCode h = new HashCode();
			h.AddBytes(new ReadOnlySpan<byte>((byte*)Unsafe.AsPointer(ref M11), sizeof(Matrix4x4D)));
			return h.ToHashCode();
		}
		public override bool Equals([NotNullWhen(true)] object? obj) => obj is Matrix4x4D m && Equals(m);
		public bool Equals(Matrix4x4D matrix) => this == matrix;

		public static implicit operator Matrix4x4D(Matrix4x4 matrix)
		{
			if (Avx.IsSupported)
			{
				Unsafe.SkipInit(out Matrix4x4D m);
				Avx.Store(&m.M11, Avx.ConvertToVector256Double(Sse.LoadVector128(&matrix.M11)));
				Avx.Store(&m.M21, Avx.ConvertToVector256Double(Sse.LoadVector128(&matrix.M21)));
				Avx.Store(&m.M31, Avx.ConvertToVector256Double(Sse.LoadVector128(&matrix.M31)));
				Avx.Store(&m.M41, Avx.ConvertToVector256Double(Sse.LoadVector128(&matrix.M41)));
				return m;
			}
			return new Matrix4x4D(
				matrix.M11, matrix.M12, matrix.M13, matrix.M14,
				matrix.M21, matrix.M22, matrix.M23, matrix.M24,
				matrix.M31, matrix.M32, matrix.M33, matrix.M34,
				matrix.M41, matrix.M42, matrix.M43, matrix.M44);
		}
		public static explicit operator Matrix4x4(Matrix4x4D matrix)
		{
			if (Avx.IsSupported)
			{
				Unsafe.SkipInit(out Matrix4x4 m);
				Sse.Store(&m.M11, Avx.ConvertToVector128Single(Avx.LoadVector256(&matrix.M11)));
				Sse.Store(&m.M21, Avx.ConvertToVector128Single(Avx.LoadVector256(&matrix.M21)));
				Sse.Store(&m.M31, Avx.ConvertToVector128Single(Avx.LoadVector256(&matrix.M31)));
				Sse.Store(&m.M41, Avx.ConvertToVector128Single(Avx.LoadVector256(&matrix.M41)));
				return m;
			}
			return new Matrix4x4(
				(float)matrix.M11, (float)matrix.M12, (float)matrix.M13, (float)matrix.M14,
				(float)matrix.M21, (float)matrix.M22, (float)matrix.M23, (float)matrix.M24,
				(float)matrix.M31, (float)matrix.M32, (float)matrix.M33, (float)matrix.M34,
				(float)matrix.M41, (float)matrix.M42, (float)matrix.M43, (float)matrix.M44);
		}

		//public static Matrix4x4D CreateBillboard(Vector3D objectPosition, Vector3D cameraPosition, Vector3D cameraUpVector, Vector3D cameraForwardVector)
		//{
		//	Vector3D zaxis = objectPosition - cameraPosition;
		//}
	}
}
