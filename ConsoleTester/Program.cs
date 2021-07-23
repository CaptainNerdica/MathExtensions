using MathExpressions.Lexing;
using MathExpressions.SyntaxTrees;
using MathExpressions;
using MathExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Running;
using System.Threading.Tasks;
using System.Numerics;
using Complex = MathExtensions.Complex;
using BenchmarkDotNet.Jobs;

namespace ConsoleTester
{
	class Program
	{
		static double NextDouble(Random r) => r.Next(-16, 16) * r.NextDouble();
		static unsafe void Main()
		{
			BenchmarkRunner.Run<BenchmarksComplex>();
		}
	}

	[RankColumn]
	[DisassemblyDiagnoser]
	public class BenchmarksVector
	{
		[Benchmark]
		[ArgumentsSource(nameof(Data))]
		public float Abs1(Vector4 value) => MathF.Sqrt(value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W);
		[Benchmark]
		[ArgumentsSource(nameof(Data))]
		[SkipLocalsInit]
		public unsafe float Abs2(Vector4 value)
		{
			//Vector128<double> v = Sse2.LoadVector128((double*)&value);
			//Vector128<double> dp = Sse41.DotProduct(v, v, 0x31);
			//return Sse2.SqrtScalar(dp).ToScalar();
			return value.Length();
		}

		public static IEnumerable<object> Data()
		{
			static double NextDouble(Random r) => r.Next(-32, 32) * r.NextDouble();
			static float NextSingle(Random r) => r.Next(-32, 32) * (float)r.NextDouble();
			Random r = new Random(1234567);
			//yield return new Complex(NextDouble(r), NextDouble(r));
			//yield return new Complex(NextDouble(r), NextDouble(r));
			yield return new Vector4(NextSingle(r), NextSingle(r), NextSingle(r), NextSingle(r));
		}
	}

	[RankColumn]
	[DisassemblyDiagnoser]
	//[MaxIterationCount(30)]
	public class BenchmarksComplex
	{
		[Benchmark(Baseline = true)]
		[ArgumentsSource(nameof(Data))]
		public unsafe Complex LoadFields(Complex value)
		{
			Vector128<double> r = Sse2.LoadScalarVector128((double*)&value);
			Vector128<double> i = Sse2.LoadScalarVector128((double*)&value + 1);
			Vector128<double> v = Sse2.UnpackLow(r, i);
			Sse2.Store((double*)&value, v);
			return value;
		}
		[Benchmark]
		[ArgumentsSource(nameof(Data))]
		public unsafe Complex LoadVector(Complex value)
		{
			Vector128<double> v = ComplexToVector(&value);
			Sse2.Store((double*)&value, v);
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static Vector128<double> ComplexToVector(Complex* value)
		{
			Vector128<double> x = Sse2.LoadVector128((double*)value).AsDouble();
			//Vector128<double> r = Sse2.LoadScalarVector128((double*)value);
			//Vector128<double> v = Sse2.LoadHigh(r, (double*)value + 1);
			return x;
		}

		public static IEnumerable<object> Data()
		{
			static double NextDouble(Random r) => r.Next(-32, 32) * r.NextDouble();
			Random r = new Random(12345678);
			yield return new Complex(NextDouble(r), NextDouble(r));
		}
	}
}