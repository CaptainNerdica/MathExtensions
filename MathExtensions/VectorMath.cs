using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	internal static class VectorMath
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Equal(Vector128<double> vector1, Vector128<double> vector2)
		{
			if (AdvSimd.Arm64.IsSupported)
			{
				Vector128<uint> vResult = AdvSimd.Arm64.CompareEqual(vector1, vector2).AsUInt32();

				Vector64<byte> vResult0 = vResult.GetLower().AsByte();
				Vector64<byte> vResult1 = vResult.GetUpper().AsByte();

				Vector64<byte> vTemp10 = AdvSimd.Arm64.ZipLow(vResult0, vResult1);
				Vector64<byte> vTemp11 = AdvSimd.Arm64.ZipHigh(vResult0, vResult1);

				Vector64<ushort> vTemp21 = AdvSimd.Arm64.ZipHigh(vTemp10.AsUInt16(), vTemp11.AsUInt16());
				return vTemp21.AsUInt32().GetElement(1) == 0xFFFFFFFF;
			}
			else if (Sse2.IsSupported)
				return Sse2.MoveMask(Sse2.CompareNotEqual(vector1, vector2)) == 0;
			else
				// Redundant test so we won't prejit remainder of this method on platforms without AdvSimd.
				throw new PlatformNotSupportedException();
		}

		public static bool Equal(Vector256<double> vector1, Vector256<double> vector2)
		{
			if (Avx.IsSupported)
				return Avx.MoveMask(Avx.CompareNotEqual(vector1, vector2)) == 0;
			else
				// Redundant test so we won't prejit remainder of this method on platforms without AdvSimd.
				throw new PlatformNotSupportedException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool NotEqual(Vector128<float> vector1, Vector128<float> vector2)
		{
			// This implementation is based on the DirectX Math Library XMVector4NotEqual method
			// https://github.com/microsoft/DirectXMath/blob/master/Inc/DirectXMathVector.inl

			if (AdvSimd.IsSupported)
			{
				Vector128<uint> vResult = AdvSimd.CompareEqual(vector1, vector2).AsUInt32();

				Vector64<byte> vResult0 = vResult.GetLower().AsByte();
				Vector64<byte> vResult1 = vResult.GetUpper().AsByte();

				Vector64<byte> vTemp10 = AdvSimd.Arm64.ZipLow(vResult0, vResult1);
				Vector64<byte> vTemp11 = AdvSimd.Arm64.ZipHigh(vResult0, vResult1);

				Vector64<ushort> vTemp21 = AdvSimd.Arm64.ZipHigh(vTemp10.AsUInt16(), vTemp11.AsUInt16());
				return vTemp21.AsUInt32().GetElement(1) != 0xFFFFFFFF;
			}
			else if (Sse.IsSupported)
			{
				return Sse.MoveMask(Sse.CompareNotEqual(vector1, vector2)) != 0;
			}
			else
			{
				// Redundant test so we won't prejit remainder of this method on platforms without AdvSimd.
				throw new PlatformNotSupportedException();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool NotEqual(Vector256<double> vector1, Vector256<double> vector2)
		{
			if (Avx.IsSupported)
				return Avx.MoveMask(Avx.CompareNotEqual(vector1, vector2)) != 0;
			else
				// Redundant test so we won't prejit remainder of this method on platforms without AdvSimd.
				throw new PlatformNotSupportedException();
		}
	}
}
