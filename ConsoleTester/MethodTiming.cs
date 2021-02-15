using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTester
{
	internal static class MethodTiming
	{
		public static long TimeMethod<TOut>(Func<TOut> method, int iterations, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (int i = 0; i < iterations - 1; i++)
				_ = method();
			output = method();
			s.Stop();
			return s.ElapsedTicks;
		}
		public static long TimeMethod<TOut>(Func<int, TOut> method, int iterations, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (int i = 0; i < iterations - 1; i++)
				_ = method(i);
			output = method(iterations - 1);
			s.Stop();
			return s.ElapsedTicks;
		}
		public static long TimeMethod<TIn, TOut>(Func<TIn, TOut> method, int iterations, TIn param1, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (int i = 0; i < iterations - 1; i++)
				_ = method(param1);
			output = method(param1);
			s.Stop();
			return s.ElapsedTicks;
		}
		public static long TimeMethod<TIn, TOut>(Func<TIn, int, TOut> method, int iterations, TIn param1, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (int i = 0; i < iterations - 1; i++)
				_ = method(param1, i);
			output = method(param1, iterations - 1);
			s.Stop();
			return s.ElapsedTicks;
		}
		public static long TimeMethod<T1, T2, TOut>(Func<T1, T2, TOut> method, int iterations, T1 param1, T2 param2, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (int i = 0; i < iterations - 1; i++)
				_ = method(param1, param2);
			output = method(param1, param2);
			s.Stop();
			return s.ElapsedTicks;
		}
		public static long TimeMethod<T1, T2, TOut>(Func<T1, T2, int, TOut> method, int iterations, T1 param1, T2 param2, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (int i = 0; i < iterations - 1; i++)
				_ = method(param1, param2, i);
			output = method(param1, param2, iterations - 1);
			s.Stop();
			return s.ElapsedTicks;
		}
		public static long TimeMethod<T1, T2, T3, TOut>(Func<T1, T2, T3, TOut> method, int iterations, T1 param1, T2 param2, T3 param3, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (int i = 0; i < iterations - 1; i++)
				_ = method(param1, param2, param3);
			output = method(param1, param2, param3);
			s.Stop();
			return s.ElapsedTicks;
		}
		public static long TimeMethod<T1, T2, T3, TOut>(Func<T1, T2, T3, int, TOut> method, int iterations, T1 param1, T2 param2, T3 param3, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (int i = 0; i < iterations - 1; i++)
				_ = method(param1, param2, param3, i);
			output = method(param1, param2, param3, iterations - 1);
			s.Stop();
			return s.ElapsedTicks;
		}
	}
}
