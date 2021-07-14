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

namespace ConsoleTester
{
	class Program
	{
		static unsafe void Main()
		{
			_ = BenchmarkRunner.Run<Benchmarks>();
		}
	}

	[RankColumn]
	[DisassemblyDiagnoser]
	[IterationCount(20)]
	public class Benchmarks
	{
		[Benchmark]
		[ArgumentsSource(nameof(Data))]
		public Complex Add1(Complex left, Complex right) => left + right;
		[Benchmark]
		[ArgumentsSource(nameof(Data))]
		[SkipLocalsInit]
		public unsafe Complex Add2(Complex left, Complex right)
		{
			//Unsafe.SkipInit(out Complex o);
			Vector128<double> l = Sse2.LoadVector128((double*)&left);
			Vector128<double> r = Sse2.LoadVector128((double*)&right);
			Vector128<double> v = Sse2.Add(l, r);
			//Sse.Store((float*)&o, v.AsSingle());
			return new Complex(v.GetElement(0), v.GetElement(1));
		}

		public static IEnumerable<object[]> Data()
		{
			static double NextDouble(Random r) => r.Next(-1024, 1024) * r.NextDouble();
			Random r = new Random(1234567);
			yield return new object[] { new Complex(NextDouble(r), NextDouble(r)), new Complex(NextDouble(r), NextDouble(r)) };
			yield return new object[] { new Complex(NextDouble(r), NextDouble(r)), new Complex(NextDouble(r), NextDouble(r)) };
		}
	}
}