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
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Versioning;

namespace ConsoleTester
{
	static class Program
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
		public unsafe static void Main()
		{
			Process p = Process.GetCurrentProcess();
			p.ProcessorAffinity = (IntPtr)0b0000_0000_0000_0011;
			p.PriorityClass = ProcessPriorityClass.RealTime;

			MathExtensions.StrongBox<int> a = 2;
			var b = a with { Value = 4 };
			Console.WriteLine(b);
			MathExtensions.StrongBox<Guid> d = Guid.NewGuid();
			BenchmarkRunner.Run<Benchmarks>();
		}
	}

	[GcServer]
	[HardwareCounters(
		HardwareCounter.TotalCycles
	)]
	[KeepBenchmarkFiles(false)]
	[DisassemblyDiagnoser(exportCombinedDisassemblyReport: true)]
	[RankColumn]
	public unsafe class Benchmarks
	{
		//[Benchmark(Baseline = true)]
		[ArgumentsSource(nameof(Data1))]
		public UInt128 Op1(UInt128 left, UInt128 right) => left + right;

		[Benchmark(Baseline = true)]
		[ArgumentsSource(nameof(Data2))]
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public UInt128_Long Op2(UInt128_Long left, UInt128_Long right) => left + right;

		const int count = 1;
		const int seed = 7;
		public static IEnumerable<object[]> Data1()
		{
			object[][] o = new object[count][];
			Random r = new Random(seed);
			Span<byte> bytes = stackalloc byte[64];
			for (int i = 0; i < count; i++)
			{
				r.NextBytes(bytes);
				o[i] = new object[] { MemoryMarshal.Read<UInt128>(bytes), MemoryMarshal.Read<UInt128>(bytes[16..]) };
			}
			return o;
		}
		public static IEnumerable<object[]> Data2()
		{
			object[][] o = new object[count][];
			Random r = new Random(seed);
			Span<byte> bytes = stackalloc byte[64];
			for (int i = 0; i < count; i++)
			{
				r.NextBytes(bytes);
				o[i] = new object[] { MemoryMarshal.Read<UInt128_Long>(bytes), MemoryMarshal.Read<UInt128_Long>(bytes[16..]) };
			}
			return o;
		}
	}
}