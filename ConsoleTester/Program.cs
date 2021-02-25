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

namespace ConsoleTester
{
	class Program
	{
		static unsafe void Main()
		{
			Quadruple a = 10;

			for (int i = 0; i < 20; ++i)
				Console.WriteLine("{0:F}",a *= a);
		}
	}
}