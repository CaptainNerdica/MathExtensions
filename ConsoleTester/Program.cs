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

namespace ConsoleTester
{
	class Program
	{
		static readonly Lexer _lexer = new Lexer();
		static unsafe void Main()
		{
			Random r = new Random();
			Span<byte> b = stackalloc byte[32];
			r.NextBytes(b.Slice(0, 14));
			UInt128 u = new UInt128(MemoryMarshal.Cast<byte, uint>(b));
			b.Clear();
			r.NextBytes(b.Slice(0, 7));
			Span<uint> cast = MemoryMarshal.Cast<byte, uint>(b);
			UInt128 v = new UInt128(cast);
			Console.WriteLine((BigInteger)u);
			Console.WriteLine(u);
			int iterations = 200_000_000;
			long t1 = MethodTiming.TimeMethod(UInt128.Divide, iterations, u, v, out _);
			Console.WriteLine($"{(double)t1 / iterations}");
			long t2 = MethodTiming.TimeMethod(BigInteger.Divide, iterations, (BigInteger)u, (BigInteger)v, out _);
			Console.WriteLine($"{(double)t2 / iterations}");
		}

		static void LexerTestMain()
		{
			string input = "f(x, y) = 2 * x * y + 3 * y";
			IEnumerable<Token> tokens = Lexer.Trim(_lexer.Tokenize(input));
			SyntaxTreeBuilder builder = new SyntaxTreeBuilder(tokens);
		}
	}
}