using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
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
using Complex = MathExtensions.Complex;

namespace ConsoleTester
{
	static class Program
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
		public unsafe static void Main()
		{
			//UInt128L u = UInt128L.MaxValue;
			//string s = u.ToString();

			Int128 i0 = 2;
			i0.ToString();
			Int128 i1 = 1;

			Int128 i2 = Int128.DivRem(i0, i1, out _);
			Int128.DivRem(uint.MaxValue, 0x1_0000_0000_0000UL, out _);
		}
	}

	[KeepBenchmarkFiles(false)]
	//[HardwareCounters(HardwareCounter.TotalCycles, HardwareCounter.Timer)]
	[DisassemblyDiagnoser(exportCombinedDisassemblyReport: true)]
	[RankColumn]
	public unsafe class Benchmarks
	{

		public IEnumerable<object[]> Data()
		{
			const int count = 1;
			Random r = new Random(3);
			byte[] s = new byte[128];
			for (int i = 0; i < count; i++)
			{
				r.NextBytes(s);
				yield return new object[] { new Int128(s.AsSpan().Slice(0, 16)), new Int128(s.AsSpan().Slice(16, 16)) };
			}
		}
	}
}
