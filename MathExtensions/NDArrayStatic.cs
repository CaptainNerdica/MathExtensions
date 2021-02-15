using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public partial class NDArray<T>
	{
		private static class EmptyNDArray<TArray>
		{
			public static readonly NDArray<TArray> Value = new NDArray<TArray>();
		}
		public static readonly NDArray<T> Empty = EmptyNDArray<T>.Value;

		public static NDArray<TArray> Zeros<TArray>(Shape s) where TArray : notnull
		{
			NDArray<TArray> a = new NDArray<TArray>(s);
			Array.Fill(a._items, TypeConverter<int, TArray>.Convert(0));
			return a;
		}
		public static NDArray<TArray> ZerosLike<TArray>(NDArray<TArray> ndarray) where TArray : notnull => Zeros<TArray>(ndarray.Shape);
		public static NDArray<TArray> Ones<TArray>(Shape s) where TArray : notnull
		{
			NDArray<TArray> a = new NDArray<TArray>(s);
			Array.Fill(a._items, TypeConverter<int, TArray>.Convert(1));
			return a;
		}
		public static NDArray<TArray> OnesLike<TArray>(NDArray<TArray> ndarray) where TArray : notnull => Ones<TArray>(ndarray.Shape);
		public static NDArray<TArray> Full<TArray>(Shape s, TArray fillValue) where TArray : notnull
		{
			NDArray<TArray> a = new NDArray<TArray>(s);
			Array.Fill(a._items, fillValue);
			return a;
		}
		public static NDArray<TArray> FullLike<TArray>(NDArray<TArray> ndarray, TArray fillValue) where TArray : notnull => Full<TArray>(ndarray.Shape, fillValue);
		public static NDArray<TArray> Indentity<TArray>(int size) where TArray : notnull
		{
			if (size < 0)
				throw new IndexOutOfRangeException($"{nameof(size)} must be positive");
			if (size == 0)
				return new NDArray<TArray>((0, 0));
			NDArray<TArray> a = Full((size, size), TypeConverter<int, TArray>.Convert(0));
			for (int i = 0; i < size; i++)
				a._items[i * (size + 1)] = TypeConverter<int, TArray>.Convert(1);
			return a;
		}
	}
}
