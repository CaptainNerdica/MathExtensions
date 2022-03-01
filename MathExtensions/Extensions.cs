using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	internal static class Extensions
	{
		public static string ToBinaryString(this ReadOnlySpan<byte> bytes)
		{
			Span<char> chars = bytes.Length <= 128 ? stackalloc char[bytes.Length * 8] : new char[bytes.Length * 8];
			for (int i = 0; i < bytes.Length; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					chars[(bytes.Length - i) * 8 - (j + 1)] = (char)('0' + ((bytes[i] >> j) & 1));
				}
			}
			return chars.ToString();
		}
	}

	public static class RandomExtensions
	{
#if PREVIEW_FEATURES
		[RequiresPreviewFeatures]
#endif
		public static Quadruple NextQuadruple(this Random random)
		{
			Span<byte> b = stackalloc byte[14];
			random.NextBytes(b);
			throw new NotImplementedException();
		}
	}
}
