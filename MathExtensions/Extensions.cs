using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	internal static class Extensions
	{
		public static string ToBinaryString(this byte[] bytes, bool reverse = false)
		{
			IEnumerable<string> byteArray = bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0'));
			if (reverse)
				return string.Join("", byteArray.Reverse());
			else
				return string.Join("", byteArray);
		}
	}

	public static class RandomExtensions
	{
		public static Quadruple NextQuadruple(this Random random)
		{
			Span<byte> b = stackalloc byte[14];
			random.NextBytes(b);
			UInt128 s = default;
			throw new NotImplementedException();
		}
	}
}
