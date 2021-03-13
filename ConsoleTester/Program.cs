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
			Quadruple q = -12345 * MathQ.PI;
			Console.WriteLine(q);
			Console.WriteLine(MathQ.Truncate(q));
		}

		static Quadruple InvFactorial(Quadruple q) => Quadruple.One / Factorial(q);

		private static Quadruple Factorial(Quadruple q)
		{
			if (q == Quadruple.Zero)
				return Quadruple.One;
			return q * Factorial(q - Quadruple.One);
		}
	}
}