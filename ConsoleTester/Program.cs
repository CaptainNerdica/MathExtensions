using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using MathExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Versioning;
using System.Security.Cryptography;

namespace ConsoleTester
{
	static class Program
	{
		public unsafe static void Main()
		{
			Quadruple q1 = Int128.MaxValue;
			Quadruple q5 = Int128.One;
			Quadruple q6 = Int128.MinValue;
			Quadruple q7 = (1L << 63) + (1L << 62);

			Console.Read();
		}

		public static Int128 Add(Int128 x, Int128 y) => x + y;
	}

	[KeepBenchmarkFiles(false)]
	[DisassemblyDiagnoser(exportCombinedDisassemblyReport: true, exportHtml: true)]
	[RankColumn]
	[MaxIterationCount(30)]
	[IterationCount(5)]
	public class Benchmarks
	{
		static readonly int _globalSeed = (int)DateTime.UtcNow.Ticks;
		static unsafe readonly int _sizeQuad = sizeof(Quadruple);

		[Benchmark(Baseline = true)]
		[ArgumentsSource(nameof(Data))]
		public bool Eq1(Quadruple left, Quadruple right)
		{
			if (Quadruple.IsNaN(left) || Quadruple.IsNaN(right))
				return false;
			if (Quadruple.IsZero(left))
#pragma warning disable IDE0075 // Simplify conditional expression
				return Quadruple.IsZero(right) ? true : false;
#pragma warning restore IDE0075 // Simplify conditional expression

			return (left._upper == right._upper && left._lower == right._lower) ? true : false;
		}

		[Benchmark]
		[ArgumentsSource(nameof(Data))]
		public bool Eq2(Quadruple left, Quadruple right)
		{
			if ((((left._upper >> 48) & 0x7FFF) == 0x7FFF && (left._upper << 16 | left._lower) != 0) ||
				(((right._upper >> 48) & 0x7FFF) == 0x7FFF && (right._upper << 16 | right._lower) != 0))
				return false;

			if ((left._upper << 1 | left._lower) == 0)
				return (right._upper << 1 | right._lower) == 0 ? true : false;

			return left._upper == right._upper && left._lower == right._lower ? true : false;
		}

		public IEnumerable<object[]> Data()
		{
			Span<byte> b = stackalloc byte[2 * _sizeQuad];
			Random r = new Random(_globalSeed);
			List<object[]> data = new List<object[]>();

			int byteIdx;
			for (int i = 0; i < 1; i++)
			{
				r.NextBytes(b);
				byteIdx = 0;
				data.Add(new object[] { MemoryMarshal.Read<Quadruple>(b[byteIdx..(byteIdx += _sizeQuad)]), MemoryMarshal.Read<Quadruple>(b[byteIdx..(byteIdx += _sizeQuad)]) });
			}

			data.Add(new object[] { Quadruple.One, Quadruple.NaN });
			data.Add(new object[] { Quadruple.Zero, Quadruple.NegativeZero });
			data.Add(new object[] { Quadruple.Pi, Quadruple.Pi });

			return data;
		}
	}
}
