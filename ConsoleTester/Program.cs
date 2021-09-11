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
			Span<Vector4D> v = stackalloc Vector4D[] { Vector4D.Normalize((r.NextDouble(), r.NextDouble(), r.NextDouble(), r.NextDouble())), Vector4D.Normalize((r.NextDouble(), r.NextDouble(), r.NextDouble(), r.NextDouble())) };
			var a = stackalloc double[2];

			Console.WriteLine($"{v[0]:G5}\n{v[1]:G5}");
			long iterations = 1L << 30;
			GC.TryStartNoGCRegion(1 << 20);
			long t1 = MethodTiming.TimeMethod(Benchmarks.Dot1, iterations, v[0], v[1], out a[0]);
			GC.EndNoGCRegion();
			GC.Collect();
			Console.WriteLine($"Dot1:\t\t{(double)t1 / iterations,4:G4}, {a[0],7:G7}");

			GC.TryStartNoGCRegion(1 << 20);
			long t2 = MethodTiming.TimeMethod(Benchmarks.Dot2, iterations, v[0], v[1], out a[1]);
			GC.EndNoGCRegion();
			GC.Collect();
			Console.WriteLine($"Dot2:\t\t{(double)t2 / iterations,4:G4}, {a[1],7:G7}");
		}
	}

	public static unsafe class Benchmarks
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Dot1(Vector4D value1, Vector4D value2)
		{
			Vector256<double> m = Avx.Multiply(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X));
			return Sse2.AddScalar(Sse3.HorizontalAdd(m.GetLower(), m.GetLower()), Sse3.HorizontalAdd(m.GetUpper(), m.GetUpper())).ToScalar();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Dot2(Vector4D value1, Vector4D value2)
		{
			Vector256<double> m = Avx.Multiply(Avx.LoadVector256(&value1.X), Avx.LoadVector256(&value2.X));
			m = Avx.HorizontalAdd(m, m);
			return Sse2.AddScalar(m.GetLower(), m.GetUpper()).ToScalar();
		}
	}
}