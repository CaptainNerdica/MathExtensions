using MathExtensions;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Runtime.CompilerServices;
using System.Runtime;
using System.Text;
using System.Runtime.Intrinsics;
using System.Numerics;
using Complex = MathExtensions.Complex;
using System.Diagnostics;
using static MathExtensions.NDArrayHelpers;

namespace ConsoleTester
{
	class Program
	{
		static unsafe void Main()
		{
			Shape s = new Shape(stackalloc uint[] { 4, 4, 4 });
			int i = 63;
			Span<uint> buff = stackalloc uint[s.Rank];
			ReadOnlySpan<uint> index = s.FromLinearIndex(i, buff);
			Console.WriteLine(index.ToString<uint>());
		}
	}

	class Benchmarks
	{

	}
}