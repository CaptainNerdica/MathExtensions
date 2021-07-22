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
		static double NextDouble(Random r) => r.Next(-16, 16) * r.NextDouble();
		static unsafe void Main()
		{
			_ = BenchmarkRunner.Run<Benchmarks>();
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
		public Complex Multiply1(Complex left, Complex right) => new Complex(left._real * right._real - left._imag * right._imag, left._imag * right._real + left._real * right._imag);
		[Benchmark]
		[ArgumentsSource(nameof(Data))]
		[SkipLocalsInit]
		public unsafe Complex Multiply2(Complex left, Complex right)
		{
			Vector128<double> l = Sse2.LoadVector128((double*)&left);
			Vector128<double> r = Sse2.LoadVector128((double*)&right);
			Vector128<double> rReal = Sse2.Shuffle(r.AsInt32(), 0b01_00_11_10).AsDouble();
			Vector128<double> rImag = Sse2.Multiply(r, Vector128.Create(1d, -1));
			Vector128<double> mReal = Sse2.Multiply(l, rImag);
			Vector128<double> mImag = Sse2.Multiply(l, rReal);
			Vector128<double> v = Sse3.HorizontalAdd(mReal, mImag);
			Sse2.Store((double*)&left, v);
			return left;
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