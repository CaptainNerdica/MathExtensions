using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MathExtensions.NDArrayHelpers;

namespace MathExtensions
{
	public ref struct RefNDArray<T> where T : struct
	{
		private RefShape _shape;
		private readonly Span<T> _elements;
		public int Rank => _shape.Rank;
		public int Length => _elements.Length;

		public RefShape Shape
		{
			get => _shape;
			set => _shape = (value.Size != _shape.Size) ? value : throw new ArgumentException($"Shapes of size {value.Size} and {_shape.Size} are incompatible", nameof(value));
		}

		public T this[int index]
		{
			get => _elements[index];
			set => _elements[index] = value;
		}
		public T this[ReadOnlySpan<uint> index]
		{
			get => _elements[_shape.ToLinearIndex(index)];
			set => _elements[_shape.ToLinearIndex(index)] = value;
		}

		public RefNDArray(IShape shape, Span<T> elements)
		{
			_elements = elements;
			_shape = shape.Span;
		}
		public RefNDArray(RefShape shape, Span<T> elements)
		{
			_elements = elements;
			_shape = shape;
		}
		public RefNDArray(ReadOnlySpan<uint> shape, Span<T> elements) : this((RefShape)shape, elements) { }

		public void Fill(T value) => _elements.Fill(value);
		public void Clear() => _elements.Clear();

		public static implicit operator RefNDArray<T>(NDArrayBase<T> array) => new RefNDArray<T>(array.Shape, array.Elements);
		public static explicit operator NDArrayBase<T>(RefNDArray<T> array) => new NDArray<T>((Shape)array.Shape, array._elements.ToArray());

	}

	public readonly ref struct RefShape
	{
		public uint this[int index] => Span[index];
		public int Rank => Span.Length;
		public uint Size { get; }
		public ReadOnlySpan<uint> Span { get; }

		public RefShape(ReadOnlySpan<uint> dimensions)
		{
			NDArrayHelpers.ThrowIfAnyNegative(dimensions);
			Span = dimensions;
			uint size = 1;
			for (int i = 0; i < Span.Length; ++i)
				size *= Span[i];
			Size = size;
		}

		public bool Equals(RefShape other)
		{
			if (Span == other.Span)
				return true;
			if (Span.Length == other.Span.Length)
			{
				bool b = true;
				for (int i = 0; i < Span.Length; ++i)
					b &= Span[i] == other.Span[i];
				return b;
			}
			return false;
		}

		public override string ToString()
		{
			if (Rank == 0)
				return "()";
			StringBuilder sb = new StringBuilder();
			sb.Append('(');
			for (int i = 0; i < Rank - 1; ++i)
				sb.Append(Span[i]).Append(", ");
			sb.Append(Span[Rank - 1]);
			sb.Append(')');
			return sb.ToString();
		}

		public static implicit operator RefShape(Shape shape) => new RefShape(shape.Span);
		public static explicit operator Shape(RefShape shape) => new Shape(shape.Span);
		public static implicit operator RefShape(ReadOnlySpan<uint> dimensions) => new RefShape(dimensions);
		public static implicit operator ReadOnlySpan<uint>(RefShape shape) => shape.Span;
	}
}
