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
	[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	class Program
	{
		static readonly Lexer _lexer = new Lexer();
		static unsafe void Main()
		{
			Quadruple a = (Quadruple)12 / 10;

			for (int i = 0; i < 20; ++i)
				Console.WriteLine(a *= a);
		}



		static void LexerTestMain()
		{
			string input = "f(x, y) = 2 * x * y + 3 * y";
			IEnumerable<Token> tokens = Lexer.Trim(_lexer.Tokenize(input));
			SyntaxTreeBuilder builder = new SyntaxTreeBuilder(tokens);
		}
	}
}