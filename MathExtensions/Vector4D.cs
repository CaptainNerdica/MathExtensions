using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector4D
	{
		public double X;
		public double Y;
		public double Z;
		public double W;

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector4D(Vector4 vector) => new Vector4D(vector.X, vector.Y, vector.Z, vector.W);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector4(Vector4D vector) => new Vector4((float)vector.X, (float)vector.Y, (float)vector.Z, (float)vector.W);
	}
}
