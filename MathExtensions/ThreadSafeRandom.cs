using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public static class ThreadSafeRandom
	{
		private static readonly Random _global = new Random();
		[ThreadStatic]
		private static Random? _local;

		private static Random NewInstance()
		{
			int seed;
			lock (_global) seed = _global.Next();
			return new Random(seed);
		}

		public static int Next()
		{
			Random? inst = _local;
			if (inst is null)
				_local = inst = NewInstance();
			return inst.Next();
		}
		public static int Next(int max)
		{
			Random? inst = _local;
			if (inst is null)
				_local = inst = NewInstance();
			return inst.Next(max);
		}
		public static int Next(int min, int max)
		{
			Random? inst = _local;
			if (inst is null)
				_local = inst = NewInstance();
			return inst.Next(min, max);
		}
		public static void NextBytes(byte[] buffer)
		{
			Random? inst = _local;
			if (inst is null)
				_local = inst = NewInstance();
			inst.NextBytes(buffer);
		}
		public static void NextBytes(Span<byte> buffer)
		{
			Random? inst = _local;
			if (inst is null)
				_local = inst = NewInstance();
			inst.NextBytes(buffer);
		}
		public static double NextDouble()
		{
			Random? inst = _local;
			if (inst is null)
				_local = inst = NewInstance();
			return inst.NextDouble();
		}
	}
}
