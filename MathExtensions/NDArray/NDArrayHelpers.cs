using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public static class NDArrayHelpers
	{
		public static void ThrowIfAnyNegative(ReadOnlySpan<uint> dimensions)
		{
			if (!CheckAllPositive(dimensions))
				throw new IndexOutOfRangeException("All dimension sizes must be positive");
		}
		public static bool CheckAllPositive(ReadOnlySpan<uint> dimensions)
		{
			bool b = true;
			for (int i = 0; i < dimensions.Length; ++i)
				b &= dimensions[i] >= 0;
			return b;
		}
		public static bool ValidIndex(this IShape shape, ReadOnlySpan<uint> index)
		{
			if (shape is null || shape.Rank != index.Length || !CheckAllPositive(index))
				return false;
			bool b = true;
			for (int i = 0; i < index.Length; ++i)
				b &= index[i] < shape[i];
			return b;

		}
		public static bool ValidIndex(this RefShape shape, ReadOnlySpan<uint> index)
		{
			if (shape.Rank != index.Length || !CheckAllPositive(index))
				return false;
			bool b = true;
			for (int i = 0; i < index.Length; ++i)
				b &= index[i] < shape[i];
			return b;

		}
		public static bool ValidIndex<TShape>(this TShape shape, ReadOnlySpan<uint> index) where TShape : struct, IShape
		{
			if (shape.Rank != index.Length || !CheckAllPositive(index))
				return false;
			bool b = true;
			for (int i = 0; i < index.Length; ++i)
				b &= index[i] < shape[i];
			return b;
		}

		public static int ToLinearIndex(this IShape shape, ReadOnlySpan<uint> index)
		{
			if (shape.ValidIndex(index))
				return ToLinearIndex(shape.Span, index);
			else
				throw new Exception($"{ToString(index)} is not a valid index of {shape}");
		}
		public static int ToLinearIndex(this RefShape shape, ReadOnlySpan<uint> index)
		{
			if (shape.ValidIndex(index))
				return ToLinearIndex(shape.Span, index);
			else
				throw new Exception($"{ToString(index)} is not a valid index of {shape.ToString()}");
		}
		public static int ToLinearIndex<TShape>(this TShape shape, ReadOnlySpan<uint> index) where TShape : struct, IShape
		{
			if (shape.ValidIndex(index))
				return ToLinearIndex(shape.Span, index);
			else
				throw new Exception($"{ToString(index)} is not a valid index of {shape}");
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int ToLinearIndex(ReadOnlySpan<uint> shape, ReadOnlySpan<uint> index)
		{
			uint linearIndex = 0;
			for (int j = index.Length - 1; j >= 1; j--)
				linearIndex = shape[index.Length - 1 - j] * (linearIndex + index[j]);
			linearIndex += index[0];
			return (int)linearIndex;
		}

		public static ReadOnlySpan<uint> FromLinearIndex(this IShape shape, int index, Span<uint> buffer) => FromLinearIndex(shape.Span, shape.Size, (uint)index, buffer);
		public static ReadOnlySpan<uint> FromLinearIndex(this RefShape shape, int index, Span<uint> buffer) => FromLinearIndex(shape.Span, shape.Size, (uint)index, buffer);
		public static ReadOnlySpan<uint> FromLinearIndex<TShape>(this TShape shape, int index, Span<uint> buffer) where TShape : struct, IShape => FromLinearIndex(shape.Span, shape.Size, (uint)index, buffer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ReadOnlySpan<uint> FromLinearIndex(ReadOnlySpan<uint> shape, uint size, uint index, Span<uint> buffer)
		{
			if (buffer.Length < shape.Length)
				throw new ArgumentException("Buffer is too small.", nameof(buffer));
			if (0 > index || index >= size)
				throw new IndexOutOfRangeException();
			uint v = 1;	
			for (int i = 0; i < shape.Length; ++i)
			{
				buffer[i] = index / v % shape[i];
				v *= shape[i];
			}
			return buffer[..shape.Length];
		}

		public static string ToString<T>(this ReadOnlySpan<T> index)
		{
			if (index.Length == 0)
				return "()";
			StringBuilder sb = new StringBuilder();
			sb.Append('(');
			for (int i = 0; i < index.Length - 1; ++i)
				sb.Append(index[i]).Append(", ");
			sb.Append(index[^1]);
			sb.Append(')');
			return sb.ToString();
		}
	}
}
