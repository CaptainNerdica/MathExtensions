using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public unsafe struct QuaternionD : IEquatable<QuaternionD>
	{
		private const double SlerpEpsilon = 1e-14;

		public double X, Y, Z, W;

		public QuaternionD(double x, double y, double z, double w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public QuaternionD(Vector3D vectorPart, double scalarPart)
		{
			X = vectorPart.X;
			Y = vectorPart.Y;
			Z = vectorPart.Z;
			W = scalarPart;
		}

		public static QuaternionD Zero => default;

		public static QuaternionD Identity => new QuaternionD(0, 0, 0, 1);

		public readonly bool IsIdentity => this == Identity;

		public static QuaternionD operator -(QuaternionD value)
		{
			const ulong mask = 0x8000_0000_0000_0000;
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&value, Avx.Xor(Vector256.Create(mask).AsDouble(), Avx.LoadVector256(&value.X)));
				return value;
			}
			QuaternionD ans;
			ans.X = -value.X;
			ans.Y = -value.Y;
			ans.Z = -value.Z;
			ans.W = -value.W;
			return ans;
		}

		public static QuaternionD operator +(QuaternionD value1, QuaternionD value2)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&value1, Avx.Add(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X)));
				return value1;
			}
			QuaternionD ans;
			ans.X = value1.X + value2.X;
			ans.Y = value1.Y + value2.Y;
			ans.Z = value1.Z + value2.Z;
			ans.W = value1.W + value2.W;
			return ans;
		}

		public static QuaternionD operator -(QuaternionD value1, QuaternionD value2)
		{
			if (Avx.IsSupported)
			{
				Avx.Store((double*)&value1, Avx.Subtract(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X)));
				return value1;
			}
			QuaternionD ans;
			ans.X = value1.X - value2.X;
			ans.Y = value1.Y - value2.Y;
			ans.Z = value1.Z - value2.Z;
			ans.W = value1.W - value2.W;
			return ans;
		}

		public static QuaternionD operator *(QuaternionD value1, QuaternionD value2)
		{
			QuaternionD ans;

			double q1x = value1.X;
			double q1y = value1.Y;
			double q1z = value1.Z;
			double q1w = value1.W;

			double q2x = value2.X;
			double q2y = value2.Y;
			double q2z = value2.Z;
			double q2w = value2.W;

			// cross(av, bv)
			double cx = q1y * q2z - q1z * q2y;
			double cy = q1z * q2x - q1x * q2z;
			double cz = q1x * q2y - q1y * q2x;

			double dot = q1x * q2x + q1y * q2y + q1z * q2z;

			ans.X = q1x * q2w + q2x * q1w + cx;
			ans.Y = q1y * q2w + q2y * q1w + cy;
			ans.Z = q1z * q2w + q2z * q1w + cz;
			ans.W = q1w * q2w - dot;

			return ans;
		}

		public static QuaternionD operator /(QuaternionD value1, QuaternionD value2)
		{
			QuaternionD ans;

			double q1x = value1.X;
			double q1y = value1.Y;
			double q1z = value1.Z;
			double q1w = value1.W;

			//-------------------------------------
			// Inverse part.
			double ls = value2.X * value2.X + value2.Y * value2.Y +
					   value2.Z * value2.Z + value2.W * value2.W;
			double invNorm = 1.0 / ls;

			double q2x = -value2.X * invNorm;
			double q2y = -value2.Y * invNorm;
			double q2z = -value2.Z * invNorm;
			double q2w = value2.W * invNorm;

			//-------------------------------------
			// Multiply part.

			// cross(av, bv)
			double cx = q1y * q2z - q1z * q2y;
			double cy = q1z * q2x - q1x * q2z;
			double cz = q1x * q2y - q1y * q2x;

			double dot = q1x * q2x + q1y * q2y + q1z * q2z;

			ans.X = q1x * q2w + q2x * q1w + cx;
			ans.Y = q1y * q2w + q2y * q1w + cy;
			ans.Z = q1z * q2w + q2z * q1w + cz;
			ans.W = q1w * q2w - dot;

			return ans;
		}

		public static bool operator ==(QuaternionD value1, QuaternionD value2)
		{
			if (Avx.IsSupported)
				return VectorMath.Equal(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X));
			return value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z && value1.W == value2.W;
		}

		public static bool operator !=(QuaternionD value1, QuaternionD value2)
		{
			if (Avx.IsSupported)
				return VectorMath.NotEqual(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X));
			return value1.X != value2.X || value1.Y != value2.Y || value1.Z != value2.Z || value1.W != value2.W;
		}

		public static implicit operator QuaternionD((double x, double y, double z, double w) value) => new QuaternionD(value.x, value.y, value.z, value.w);
		public static implicit operator QuaternionD(Quaternion value) => new QuaternionD(value.X, value.Y, value.Z, value.W);
		public static explicit operator Quaternion(QuaternionD value) => new Quaternion((float)value.X, (float)value.Y, (float)value.Z, (float)value.W);

		public static QuaternionD Concatenate(QuaternionD value1, QuaternionD value2)
		{
			QuaternionD ans;

			// Concatenate rotation is actually q2 * q1 instead of q1 * q2.
			// So that's why value2 goes q1 and value1 goes q2.
			double q1x = value2.X;
			double q1y = value2.Y;
			double q1z = value2.Z;
			double q1w = value2.W;

			double q2x = value1.X;
			double q2y = value1.Y;
			double q2z = value1.Z;
			double q2w = value1.W;

			// cross(av, bv)
			double cx = q1y * q2z - q1z * q2y;
			double cy = q1z * q2x - q1x * q2z;
			double cz = q1x * q2y - q1y * q2x;

			double dot = q1x * q2x + q1y * q2y + q1z * q2z;

			ans.X = q1x * q2w + q2x * q1w + cx;
			ans.Y = q1y * q2w + q2y * q1w + cy;
			ans.Z = q1z * q2w + q2z * q1w + cz;
			ans.W = q1w * q2w - dot;

			return ans;
		}

		public static QuaternionD Conjugate(QuaternionD value)
		{
			QuaternionD ans;

			ans.X = -value.X;
			ans.Y = -value.Y;
			ans.Z = -value.Z;
			ans.W = value.W;

			return ans;
		}

		public static QuaternionD CreateFromAxisAngle(Vector3D axis, double angle)
		{
			QuaternionD ans;

			double halfAngle = angle * 0.5;
			double s = Math.Sin(halfAngle);
			double c = Math.Cos(halfAngle);

			ans.X = axis.X * s;
			ans.Y = axis.Y * s;
			ans.Z = axis.Z * s;
			ans.W = c;

			return ans;
		}

		public static QuaternionD CreateFromRotationMatrix(Matrix4x4D matrix)
		{
			double trace = matrix.M11 + matrix.M22 + matrix.M33;

			QuaternionD q = default;

			if (trace > 0.0f)
			{
				double s = Math.Sqrt(trace + 1.0f);
				q.W = s * 0.5;
				s = 0.5 / s;
				q.X = (matrix.M23 - matrix.M32) * s;
				q.Y = (matrix.M31 - matrix.M13) * s;
				q.Z = (matrix.M12 - matrix.M21) * s;
			}
			else
			{
				if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
				{
					double s = Math.Sqrt(1.0 + matrix.M11 - matrix.M22 - matrix.M33);
					double invS = 0.5 / s;
					q.X = 0.5 * s;
					q.Y = (matrix.M12 + matrix.M21) * invS;
					q.Z = (matrix.M13 + matrix.M31) * invS;
					q.W = (matrix.M23 - matrix.M32) * invS;
				}
				else if (matrix.M22 > matrix.M33)
				{
					double s = Math.Sqrt(1.0 + matrix.M22 - matrix.M11 - matrix.M33);
					double invS = 0.5 / s;
					q.X = (matrix.M21 + matrix.M12) * invS;
					q.Y = 0.5 * s;
					q.Z = (matrix.M32 + matrix.M23) * invS;
					q.W = (matrix.M31 - matrix.M13) * invS;
				}
				else
				{
					double s = Math.Sqrt(1.0 + matrix.M33 - matrix.M11 - matrix.M22);
					double invS = 0.5 / s;
					q.X = (matrix.M31 + matrix.M13) * invS;
					q.Y = (matrix.M32 + matrix.M23) * invS;
					q.Z = 0.5 * s;
					q.W = (matrix.M12 - matrix.M21) * invS;
				}
			}

			return q;
		}

		public static QuaternionD CreateFromYawPitchRoll(double yaw, double pitch, double roll)
		{
			//  Roll first, about axis the object is facing, then
			//  pitch upward, then yaw to face into the new heading
			double sr, cr, sp, cp, sy, cy;

			double halfRoll = roll * 0.5;
			sr = Math.Sin(halfRoll);
			cr = Math.Cos(halfRoll);

			double halfPitch = pitch * 0.5;
			sp = Math.Sin(halfPitch);
			cp = Math.Cos(halfPitch);

			double halfYaw = yaw * 0.5f;
			sy = Math.Sin(halfYaw);
			cy = Math.Cos(halfYaw);

			QuaternionD result;

			result.X = cy * sp * cr + sy * cp * sr;
			result.Y = sy * cp * cr - cy * sp * sr;
			result.Z = cy * cp * sr - sy * sp * cr;
			result.W = cy * cp * cr + sy * sp * sr;

			return result;
		}

		public static double Dot(QuaternionD quaternion1, QuaternionD quaternion2)
		{
			return quaternion1.X * quaternion2.X +
				   quaternion1.Y * quaternion2.Y +
				   quaternion1.Z * quaternion2.Z +
				   quaternion1.W * quaternion2.W;
		}

		public static QuaternionD Inverse(QuaternionD value)
		{
			//  -1   (       a              -v       )
			// q   = ( -------------   ------------- )
			//       (  a^2 + |v|^2  ,  a^2 + |v|^2  )

			QuaternionD ans;

			double ls = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
			double invNorm = 1.0 / ls;

			ans.X = -value.X * invNorm;
			ans.Y = -value.Y * invNorm;
			ans.Z = -value.Z * invNorm;
			ans.W = value.W * invNorm;

			return ans;
		}

		public static QuaternionD Lerp(QuaternionD quaternion1, QuaternionD quaternion2, double amount)
		{
			double t = amount;
			double t1 = 1.0f - t;

			QuaternionD r = default;

			double dot = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y +
						quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;

			if (dot >= 0.0)
			{
				r.X = t1 * quaternion1.X + t * quaternion2.X;
				r.Y = t1 * quaternion1.Y + t * quaternion2.Y;
				r.Z = t1 * quaternion1.Z + t * quaternion2.Z;
				r.W = t1 * quaternion1.W + t * quaternion2.W;
			}
			else
			{
				r.X = t1 * quaternion1.X - t * quaternion2.X;
				r.Y = t1 * quaternion1.Y - t * quaternion2.Y;
				r.Z = t1 * quaternion1.Z - t * quaternion2.Z;
				r.W = t1 * quaternion1.W - t * quaternion2.W;
			}

			// Normalize it.
			double ls = r.X * r.X + r.Y * r.Y + r.Z * r.Z + r.W * r.W;
			double invNorm = 1.0 / Math.Sqrt(ls);

			r.X *= invNorm;
			r.Y *= invNorm;
			r.Z *= invNorm;
			r.W *= invNorm;

			return r;
		}

		public static QuaternionD Normalize(QuaternionD value)
		{
			QuaternionD ans;

			double ls = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;

			double invNorm = 1.0 / Math.Sqrt(ls);

			ans.X = value.X * invNorm;
			ans.Y = value.Y * invNorm;
			ans.Z = value.Z * invNorm;
			ans.W = value.W * invNorm;

			return ans;
		}

		public static QuaternionD Slerp(QuaternionD quaternion1, QuaternionD quaternion2, double amount)
		{
			double t = amount;

			double cosOmega = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y +
							  quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;

			bool flip = false;

			if (cosOmega < 0.0f)
			{
				flip = true;
				cosOmega = -cosOmega;
			}

			double s1, s2;

			if (cosOmega > (1.0 - SlerpEpsilon))
			{
				// Too close, do straight linear interpolation.
				s1 = 1.0 - t;
				s2 = (flip) ? -t : t;
			}
			else
			{
				double omega = Math.Acos(cosOmega);
				double invSinOmega = 1 / Math.Sin(omega);

				s1 = Math.Sin((1.0 - t) * omega) * invSinOmega;
				s2 = (flip)
					? -Math.Sin(t * omega) * invSinOmega
					: Math.Sin(t * omega) * invSinOmega;
			}

			QuaternionD ans;

			ans.X = s1 * quaternion1.X + s2 * quaternion2.X;
			ans.Y = s1 * quaternion1.Y + s2 * quaternion2.Y;
			ans.Z = s1 * quaternion1.Z + s2 * quaternion2.Z;
			ans.W = s1 * quaternion1.W + s2 * quaternion2.W;

			return ans;
		}

		public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is QuaternionD q && Equals(q);
		public readonly bool Equals(QuaternionD other) => this == other;
		public readonly override int GetHashCode() => unchecked(X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode());

		public readonly double Length()
		{
			double lengthSquared = LengthSquared();
			return Math.Sqrt(lengthSquared);
		}

		/// <summary>Calculates the squared length of the quaternion.</summary>
		/// <returns>The length squared of the quaternion.</returns>
		public readonly double LengthSquared()
		{
			return X * X + Y * Y + Z * Z + W * W;
		}

		public override readonly string ToString() =>
			$"{{X:{X} Y:{Y} Z:{Z} W:{W}}}";
	}
}
