using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using MathExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace ConsoleTester
{
	static class Program
	{
		public unsafe static void Main()
		{
			Console.Read();
			Random r = new Random();
			Span<byte> buff = stackalloc byte[sizeof(UInt128)];
			r.NextBytes(buff);
			UInt128 u = MemoryMarshal.Read<UInt128>(buff);
			int s = r.Next(128);

			BenchmarkRunner.Run<Benchmarks>();
		}
	}

	[DisassemblyDiagnoser(exportCombinedDisassemblyReport: true)]
	[RankColumn]
	public unsafe class Benchmarks
	{


		public static IEnumerable<object[]> Data()
		{
			const int count = 4;
			object[][] o = new object[count][];
			Random r = new Random(3);
			Span<byte> bytes = stackalloc byte[16];
			for (int i = 0; i < count - 1; i++)
			{
				r.NextBytes(bytes);
				o[i] = new object[] { MemoryMarshal.Read<UInt128>(bytes), r.Next(128) };
			}
			r.NextBytes(bytes);
			o[^1] = new object[] { MemoryMarshal.Read<UInt128>(bytes), r.Next(1, 4) * 32 };
			return o;
		}
	}
}