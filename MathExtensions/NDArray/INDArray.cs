using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	/// <summary>
	/// A collection of objects that can be organized into any arrangement of dimensions.
	/// </summary>
	/// <typeparam name="T">The type of element</typeparam>
	public interface INDArray<T> : IEnumerable<T>
	{
		public IShape Shape { get; set; }
		public uint Length { get; }
		public T this[int index] { get; set; }
		public T this[ReadOnlySpan<uint> index] { get; set; }
		public Span<T> Elements { get; }
	}

	public static class NDArrayExtensions
	{
		public static void Fill<T>(this INDArray<T> array, T value)
		{
			for (int i = 0; i < array.Length; ++i)
				array[i] = value;
		}

		public static void Clear<T>(this INDArray<T?> array) => Fill(array, default);
	}
}
