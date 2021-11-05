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
			Process p = Process.GetCurrentProcess();
			p.ProcessorAffinity = (IntPtr)0b0000_0000_0000_0011;
			p.PriorityClass = ProcessPriorityClass.High;

			Random r = new Random();
			Span<byte> buff = stackalloc byte[sizeof(UInt256)];
			r.NextBytes(buff);
			UInt256 u = MemoryMarshal.Read<UInt256>(buff);
			int s = r.Next(256);

			Benchmarks b = new Benchmarks();
			Console.WriteLine(u >> s);
			Console.WriteLine(b.Shift1(u, s));
			Console.WriteLine(b.Shift4(u, s));
			Console.WriteLine();

			s = 128;
			Console.WriteLine(u >> s);
			Console.WriteLine(b.Shift1(u, s));
			Console.WriteLine(b.Shift4(u, s));

			Console.WriteLine();
			Console.Read();

			BenchmarkRunner.Run<Benchmarks>();
		}
	}

	[DisassemblyDiagnoser(exportCombinedDisassemblyReport: true)]
	[RankColumn]
	public unsafe class Benchmarks
	{
		[Benchmark(Baseline = true)]
		[ArgumentsSource(nameof(Data))]
		public UInt256 Shift1(UInt256 value, int bits)
		{
			const int bitSize = sizeof(uint) * 8;
			bits &= 0xFF;
			int rs = bits % bitSize;
			int ls = bitSize - rs;
			int b = bits / bitSize;
			uint* u = (uint*)&value;
			uint* s = stackalloc uint[2 * sizeof(UInt256) / sizeof(uint) - 1];
			if (rs % 32 != 0)
			{
				s[0] = u[0] >> rs | u[1] << ls;
				s[1] = u[1] >> rs | u[2] << ls;
				s[2] = u[2] >> rs | u[3] << ls;
				s[3] = u[3] >> rs | u[4] << ls;
				s[4] = u[4] >> rs | u[5] << ls;
				s[5] = u[5] >> rs | u[6] << ls;
				s[6] = u[6] >> rs | u[7] << ls;
				s[7] = u[7] >> rs;
			}
			else
				Unsafe.Copy(s, ref value);
			return Unsafe.Read<UInt256>(s + b);
		}

		[Benchmark]
		[ArgumentsSource(nameof(Data))]
		public UInt256 Shift0(UInt256 value, int bits) => value >> bits;

		[Benchmark]
		[ArgumentsSource(nameof(Data))]
		public UInt256 Shift4(UInt256 value, int bits)
		{
			const int bitSize = sizeof(uint) * 8;
			bits &= 0xFF;
			int rs = bits % bitSize;
			int ls = bitSize - rs;
			int b = bits / bitSize;
			uint* u = (uint*)&value;

			Vector128<uint> vl = Sse2.LoadVector128(u);
			Vector128<uint> vh = Sse2.LoadVector128(u + 4);

			if (rs % 32 != 0)
			{
				Vector128<uint> ll, lr, hl, hr;
				Vector128<uint> lsv = Vector128.CreateScalarUnsafe(ls).AsUInt32();
				Vector128<uint> rsv = Vector128.CreateScalarUnsafe(rs).AsUInt32();

				ll = Sse2.ShiftLeftLogical(vl, lsv);
				hl = Sse2.ShiftLeftLogical(vh, lsv);

				lr = Sse2.ShiftRightLogical(vl, rsv);
				hr = Sse2.ShiftRightLogical(vh, rsv);

				Vector128<uint> ll1 = Sse2.ShiftRightLogical128BitLane(ll, 0x4);
				Vector128<uint> hl1 = Sse2.ShiftRightLogical128BitLane(hl, 0x4);
				ll1 = Sse2.Or(ll1, Sse2.ShiftLeftLogical128BitLane(hl, 0xC));

				vl = Sse2.Or(lr, ll1);
				vh = Sse2.Or(hr, hl1);
			}
			Vector128<uint> vh1;
			switch (b)
			{
				case 0:
					break;
				case 1:
					vh1 = vh;
					vl = Sse2.ShiftRightLogical128BitLane(vl, 0x4);
					vh = Sse2.ShiftRightLogical128BitLane(vh, 0x4);
					vl = Sse2.Or(vl, Sse2.ShiftLeftLogical128BitLane(vh1, 0xC));
					break;
				case 2:
					vh1 = vh;
					vl = Sse2.ShiftRightLogical128BitLane(vl, 0x8);
					vh = Sse2.ShiftRightLogical128BitLane(vh, 0x8);
					vl = Sse2.Or(vl, Sse2.ShiftLeftLogical128BitLane(vh1, 0x8));
					break;
				case 3:
					vh1 = vh;
					vl = Sse2.ShiftRightLogical128BitLane(vl, 0xC);
					vh = Sse2.ShiftRightLogical128BitLane(vh, 0xC);
					vl = Sse2.Or(vl, Sse2.ShiftLeftLogical128BitLane(vh1, 0x4));
					break;
				case 4:
					vl = vh;
					vh = Vector128<uint>.Zero;
					break;
				case 5:
					vl = vh;
					vh = Vector128<uint>.Zero;
					vl = Sse2.ShiftRightLogical128BitLane(vl, 0x4);
					break;
				case 6:
					vl = vh;
					vh = Vector128<uint>.Zero;
					vl = Sse2.ShiftRightLogical128BitLane(vl, 0x8);
					break;
				case 7:
					vl = vh;
					vh = Vector128<uint>.Zero;
					vl = Sse2.ShiftRightLogical128BitLane(vl, 0xC);
					break;
			}
			Sse2.Store(u, vl);
			Sse2.Store(u + 4, vh);
			return value;
		}

		public static IEnumerable<object[]> Data()
		{
			const int count = 2;
			object[][] o = new object[count][];
			Random r = new Random(4);
			Span<byte> bytes = stackalloc byte[32];
			for (int i = 0; i < count - 1; i++)
			{
				r.NextBytes(bytes);
				o[i] = new object[] { MemoryMarshal.Read<UInt256>(bytes), r.Next(256) };
			}
			r.NextBytes(bytes);
			o[^1] = new object[] { MemoryMarshal.Read<UInt256>(bytes), r.Next(1, 8) * 32 };
			return o;
		}
	}
}