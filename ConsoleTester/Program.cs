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
			BitArray b = new BitArray(stackalloc byte[] { 0b01010101, 0b01010101, 0b01010101, 0b01010101, 1 }, 33);
			Console.WriteLine(b[1]);
			//_ = BenchmarkRunner.Run<Benchmarks>();
		}

		static unsafe string WriteBytes(void* p, int startOffset, int length)
		{
			StringBuilder sb = new StringBuilder();
			ReadOnlySpan<nint> s = new ReadOnlySpan<nint>((void*)((nint)p + startOffset * sizeof(nint)), length);
			for (int i = 0; i < s.Length; i++)
				sb.Append(s[i].ToString("X").PadLeft(sizeof(nint) * 2, '0')).Append(' ');
			return sb.ToString();
		}
	}

	[RankColumn]
	[DisassemblyDiagnoser]
	[IterationCount(20)]
	public class Benchmarks
	{
		[Benchmark]
		[ArgumentsSource(nameof(Data))]
		public double Abs1(Complex value) => Complex.Abs(value);
		[Benchmark]
		[ArgumentsSource(nameof(Data))]
		[SkipLocalsInit]
		public unsafe double Abs2(Complex value)
		{
			Vector128<double> v = Sse2.LoadVector128((double*)&value);
			return Sse41.DotProduct(v, v, 0b0000).ToScalar();
		}

		public static IEnumerable<object[]> Data()
		{
			static double NextDouble(Random r) => r.Next(-1024, 1024) * r.NextDouble();
			Random r = new Random(1234567);
			yield return new object[] { new Complex(NextDouble(r), NextDouble(r)) };
			yield return new object[] { new Complex(NextDouble(r), NextDouble(r)) };
		}
	}
}