using MathExpressions.Lexing;
using MathExpressions.SyntaxTrees;
using MathExpressions;
using MathExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;

namespace ConsoleTester
{
	class Program
	{
		static unsafe void Main()
		{
			//MethodInfo? m = typeof(Quadruple).GetMethod("ToStringHex", BindingFlags.NonPublic | BindingFlags.Instance);
			double p = Math.Pow(2, -1074);
			Console.WriteLine(p);
			long l = BitConverter.DoubleToInt64Bits(p);
			Console.WriteLine(l.ToString("X16"));
			Quadruple q0 = p;
			Console.WriteLine(q0);
			Console.WriteLine((double)q0);
		}

		static Quadruple InvFactorial(int q) => Quadruple.One / Factorial(q);

		private static int Factorial(int q)
		{
			if (q < 0)
				throw new ArgumentOutOfRangeException(nameof(q), "Value cannot be negative");
			if (q == 0)
				return 1;
			return q * Factorial(q - 1);
		}
	}
}