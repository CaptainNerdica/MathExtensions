using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTester
{
	internal static class MethodTiming
	{
		private static class EmptyAction<T>
		{
			internal static readonly Action<T> Empty =[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)] (T value) => { };
		}
		private static class EmptyAction<T1, T2>
		{
			internal static readonly Action<T1, T2> Empty =[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)] (T1 v1, T2 v2) => { };
		}
		private static class EmptyAction<T1, T2, T3>
		{
			internal static readonly Action<T1, T2, T3> Empty =[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)] (T1 v1, T2 v2, T3 v3) => { };
		}
		static readonly Action _emptyAction =[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)] () => { };
		private static long CalculateOverhead(long iterations)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (long i = 0; i < iterations; i++)
				_emptyAction();
			s.Stop();
			return s.ElapsedTicks;
		}
		private static long CalculateOverhead<T>(long iterations, T value)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (long i = 0; i < iterations; i++)
				EmptyAction<T>.Empty(value);
			s.Stop();
			return s.ElapsedTicks;
		}
		private static long CalculateOverhead<T1, T2>(long iterations, T1 v1, T2 v2)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (long i = 0; i < iterations; i++)
				EmptyAction<T1, T2>.Empty(v1, v2);
			s.Stop();
			return s.ElapsedTicks;
		}
		private static long CalculateOverhead<T1, T2, T3>(long iterations, T1 v1, T2 v2, T3 v3)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (long i = 0; i < iterations; i++)
				EmptyAction<T1, T2, T3>.Empty(v1, v2, v3);
			s.Stop();
			return s.ElapsedTicks;
		}

		private static (double mult, string units) GetMultiplierUnits(double i)
		{
			return i switch
			{
				< 1d / 10_000_000 => (1_000_000_000, "ns"),
				< 1d / 10_000 => (1_000_000, "μs"),
				< 1d / 10 => (1_000, "ms"),
				_ => (1, "s")
			};
		}

		public static void TestMethod<TOut>(Func<TOut> method, long iterations, long noGCSize = 1 << 24)
		{
			GC.Collect();
			GC.TryStartNoGCRegion(noGCSize);
			long t = TimeMethod(method, iterations, out TOut v);
			GC.EndNoGCRegion();
			Console.WriteLine(v);
			double its = (double)t / iterations / TimeSpan.TicksPerSecond;
			(double mult, string units) = GetMultiplierUnits(its);
			Console.WriteLine($"{method.Method.Name}: {its * mult:N3} {units}/iter");
		}

		public static void TestMethod<T1, TOut>(Func<T1, TOut> method, long iterations, T1 value1, long noGCSize = 1 << 24)
		{
			GC.Collect();
			GC.TryStartNoGCRegion(noGCSize);
			long t = TimeMethod(method, iterations, value1, out TOut v);
			GC.EndNoGCRegion();
			Console.WriteLine(v);
			double its = (double)t / iterations / TimeSpan.TicksPerSecond;
			(double mult, string units) = GetMultiplierUnits(its);
			Console.WriteLine($"{method.Method.Name}: {its * mult:N3} {units}/iter");
		}

		public static void TestMethod<T1, T2, TOut>(Func<T1, T2, TOut> method, long iterations, T1 value1, T2 value2, long noGCSize = 1 << 24)
		{
			GC.Collect();
			GC.TryStartNoGCRegion(noGCSize);
			long t = TimeMethod(method, iterations, value1, value2, out TOut v);
			GC.EndNoGCRegion();
			Console.WriteLine(v);
			double its = (double)t / iterations / TimeSpan.TicksPerSecond;
			(double mult, string units) = GetMultiplierUnits(its);
			Console.WriteLine($"{method.Method.Name}: {its * mult:N3} {units}/iter");
		}

		public static void TestMethod<T1, T2, T3, TOut>(Func<T1, T2, T3, TOut> method, long iterations, T1 value1, T2 value2, T3 value3, long noGCSize = 1 << 24)
		{
			GC.Collect();
			GC.TryStartNoGCRegion(noGCSize);
			long t = TimeMethod(method, iterations, value1, value2, value3, out TOut v);
			GC.EndNoGCRegion();
			Console.WriteLine(v);
			double its = (double)t / iterations / TimeSpan.TicksPerSecond;
			(double mult, string units) = GetMultiplierUnits(its);
			Console.WriteLine($"{method.Method.Name}: {its * mult:N3} {units}/iter");
		}

		public static long TimeMethod<TOut>(Func<TOut> method, long iterations, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (long i = 0; i < iterations - 1; i++)
				_ = method();
			output = method();
			s.Stop();
			long loop = CalculateOverhead(iterations);
			return s.ElapsedTicks - loop;
		}
		public static long TimeMethod<TOut>(Func<long, TOut> method, long iterations, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (long i = 0; i < iterations - 1; i++)
				_ = method(i);
			output = method(iterations - 1);
			s.Stop();
			long loop = CalculateOverhead(iterations);
			return s.ElapsedTicks - loop;
		}
		public static long TimeMethod<TIn, TOut>(Func<TIn, TOut> method, long iterations, TIn param1, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (long i = 0; i < iterations - 1; i++)
				_ = method(param1);
			output = method(param1);
			s.Stop();
			long loop = CalculateOverhead(iterations, param1);
			return s.ElapsedTicks - loop;
		}
		public static long TimeMethod<TIn, TOut>(Func<TIn, long, TOut> method, long iterations, TIn param1, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (long i = 0; i < iterations - 1; i++)
				_ = method(param1, i);
			output = method(param1, iterations - 1);
			s.Stop();
			long loop = CalculateOverhead(iterations, param1);
			return s.ElapsedTicks - loop;
		}
		public static long TimeMethod<T1, T2, TOut>(Func<T1, T2, TOut> method, long iterations, T1 param1, T2 param2, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (long i = 0; i < iterations - 1; i++)
				_ = method(param1, param2);
			output = method(param1, param2);
			s.Stop();
			long loop = CalculateOverhead(iterations, param1, param2);
			return s.ElapsedTicks - loop;
		}
		public static long TimeMethod<T1, T2, TOut>(Func<T1, T2, long, TOut> method, long iterations, T1 param1, T2 param2, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (long i = 0; i < iterations - 1; i++)
				_ = method(param1, param2, i);
			output = method(param1, param2, iterations - 1);
			s.Stop();
			long loop = CalculateOverhead(iterations, param1, param2);
			return s.ElapsedTicks - loop;
		}
		public static long TimeMethod<T1, T2, T3, TOut>(Func<T1, T2, T3, TOut> method, long iterations, T1 param1, T2 param2, T3 param3, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (long i = 0; i < iterations - 1; i++)
				_ = method(param1, param2, param3);
			output = method(param1, param2, param3);
			s.Stop();
			long loop = CalculateOverhead(iterations, param1, param2, param3);
			return s.ElapsedTicks - loop;
		}
		public static long TimeMethod<T1, T2, T3, TOut>(Func<T1, T2, T3, long, TOut> method, long iterations, T1 param1, T2 param2, T3 param3, out TOut output)
		{
			Stopwatch s = Stopwatch.StartNew();
			for (long i = 0; i < iterations - 1; i++)
				_ = method(param1, param2, param3, i);
			output = method(param1, param2, param3, iterations - 1);
			s.Stop();
			long loop = CalculateOverhead(iterations, param1, param2, param3);
			return s.ElapsedTicks - loop;
		}

		public static unsafe class Unsafe
		{
			private static long CalculateLoopTime(long iterations)
			{
				Stopwatch s = Stopwatch.StartNew();
				for (long i = 0; i < iterations; i++) { }
				s.Stop();
				return s.ElapsedTicks;
			}
			public static long TimeMethod<TOut>(delegate*<TOut> method, long iterations, out TOut output)
			{
				Stopwatch s = Stopwatch.StartNew();
				for (long i = 0; i < iterations - 1; i++)
					_ = method();
				output = method();
				s.Stop();
				return s.ElapsedTicks - CalculateLoopTime(iterations);
			}
			public static long TimeMethod<TIn, TOut>(delegate*<TIn, TOut> method, long iterations, TIn param1, out TOut output)
			{
				Stopwatch s = Stopwatch.StartNew();
				for (long i = 0; i < iterations - 1; i++)
					_ = method(param1);
				output = method(param1);
				s.Stop();
				return s.ElapsedTicks - CalculateLoopTime(iterations);
			}
			public static long TimeMethod<T1, T2, TOut>(delegate*<T1, T2, TOut> method, long iterations, T1 param1, T2 param2, out TOut output)
			{
				Stopwatch s = Stopwatch.StartNew();
				for (long i = 0; i < iterations - 1; i++)
					method(param1, param2);
				output = method(param1, param2);
				s.Stop();
				return s.ElapsedTicks - CalculateLoopTime(iterations);
			}
			public static long TimeMethod<T1, T2, T3, TOut>(delegate*<T1, T2, T3, TOut> method, long iterations, T1 param1, T2 param2, T3 param3, out TOut output)
			{
				Stopwatch s = Stopwatch.StartNew();
				for (long i = 0; i < iterations - 1; i++)
					_ = method(param1, param2, param3);
				output = method(param1, param2, param3);
				s.Stop();
				return s.ElapsedTicks - CalculateLoopTime(iterations);
			}
		}
	}
}
