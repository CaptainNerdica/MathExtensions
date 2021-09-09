using MathExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace ConsoleTester
{
	class Program
	{
		public unsafe static void Main()
		{
			Process p = Process.GetCurrentProcess();
			p.ProcessorAffinity = (IntPtr)0b00_00_00_00_00_00_00_01;
			Random r = new Random();
			Span<Vector3D> v = stackalloc Vector3D[] { (r.NextDouble(), r.NextDouble(), r.NextDouble()), (r.NextDouble(), r.NextDouble(), r.NextDouble()) };
			var a = stackalloc Vector3D[2];

			Console.WriteLine($"{v[0]:G5}\n{v[1]:G5}");
			long iterations = 1L << 30;
			GC.TryStartNoGCRegion(1 << 20);
			long t1 = MethodTiming.TimeMethod(Benchmarks.Reflect1, iterations, v[0], v[1], out a[0]);
			GC.EndNoGCRegion();
			GC.Collect();
			Console.WriteLine($"Reflect1:\t\t{(double)t1 / iterations,4:G4}, {a[0],7:G7}");

			GC.TryStartNoGCRegion(1 << 20);
			long t2 = MethodTiming.TimeMethod(Benchmarks.Reflect2, iterations, v[0], v[1], out a[1]);
			GC.EndNoGCRegion();
			GC.Collect();
			Console.WriteLine($"Reflect2:\t\t{(double)t2 / iterations,4:G4}, {a[1],7:G7}");
		}
	}

	public static unsafe class Benchmarks
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D Reflect1(Vector3D vector, Vector3D normal)
		{
			Vector256<double> v = Avx.LoadVector256(&vector.X);
			Vector256<double> n = Avx.LoadVector256(&normal.X);

			Vector256<double> m = Avx.Multiply(v, n);
			Vector128<double> b = Sse2.AddScalar(Sse3.HorizontalAdd(m.GetLower(), m.GetLower()), m.GetUpper());
			Vector256<double> a = Avx.Multiply(Vector256.Create(Sse2.AddScalar(b, b).ToScalar()), n);

			Avx.Store(&vector.X, Avx.Subtract(v, a));
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3D Reflect2(Vector3D vector, Vector3D normal)
		{
			Vector256<double> v = Avx.LoadVector256(&vector.X);
			Vector256<double> n = Avx.LoadVector256(&normal.X);

			Vector256<double> m = Avx.Multiply(v, n);
			Avx.Store(&vector.X, Avx.Subtract(v, Avx.Multiply(Vector256.Create(2 * Sse2.AddScalar(Sse3.HorizontalAdd(m.GetLower(), m.GetLower()), m.GetUpper()).ToScalar()), n)));
			return vector;
		}
	}
}