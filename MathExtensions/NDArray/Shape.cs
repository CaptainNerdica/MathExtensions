using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MathExtensions.NDArrayHelpers;

namespace MathExtensions
{
	public readonly struct Shape : IShape, IEquatable<Shape>
	{
		private readonly uint[] _dimensions;

		public int Rank => _dimensions?.Length ?? 0;
		public uint Size { get; }
		public ReadOnlySpan<uint> Span => _dimensions;

		public uint this[int index] => _dimensions?[index] ?? throw new IndexOutOfRangeException();

		public static Shape Empty { get; } = new Shape(0);
		public Shape(params uint[] dimensions) : this((Span<uint>)dimensions) { }
		public Shape(ReadOnlySpan<uint> dimensions)
		{
			if (dimensions.Length == 0)
			{
				_dimensions = Array.Empty<uint>();
				Size = 0;
				return;
			}
			ThrowIfAnyNegative(dimensions);
			_dimensions = dimensions.ToArray();
			Size = _dimensions.Aggregate(1U, (value, accumulate) => value * accumulate);
		}
		#region ValueTuple Constructors
		public Shape(uint dimension)
		{
			_dimensions = dimension == 0 ? Array.Empty<uint>() : new uint[] { dimension };
			Size = _dimensions.Aggregate(1U, (value, accumulate) => value * accumulate);
			ThrowIfAnyNegative(_dimensions);
		}
		public Shape((uint, uint) dimensions)
		{
			_dimensions = new uint[] { dimensions.Item1, dimensions.Item2 };
			Size = _dimensions.Aggregate(1U, (value, accumulate) => value * accumulate);
			ThrowIfAnyNegative(_dimensions);
		}
		public Shape((uint, uint, uint) dimensions)
		{
			_dimensions = new uint[] { dimensions.Item1, dimensions.Item2, dimensions.Item3 };
			Size = _dimensions.Aggregate(1U, (value, accumulate) => value * accumulate);
			ThrowIfAnyNegative(_dimensions);
		}
		public Shape((uint, uint, uint, uint) dimensions)
		{
			_dimensions = new uint[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4 };
			Size = _dimensions.Aggregate(1U, (value, accumulate) => value * accumulate);
			ThrowIfAnyNegative(_dimensions);
		}
		public Shape((uint, uint, uint, uint, uint) dimensions)
		{
			_dimensions = new uint[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4, dimensions.Item5 };
			Size = _dimensions.Aggregate(1U, (value, accumulate) => value * accumulate);
			ThrowIfAnyNegative(_dimensions);
		}
		public Shape((uint, uint, uint, uint, uint, uint) dimensions)
		{
			_dimensions = new uint[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4, dimensions.Item5, dimensions.Item6 };
			Size = _dimensions.Aggregate(1U, (value, accumulate) => value * accumulate);
			ThrowIfAnyNegative(_dimensions);
		}
		public Shape((uint, uint, uint, uint, uint, uint, uint) dimensions)
		{
			_dimensions = new uint[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4, dimensions.Item5, dimensions.Item6, dimensions.Item7 };
			Size = _dimensions.Aggregate(1U, (value, accumulate) => value * accumulate);
			ThrowIfAnyNegative(_dimensions);
		}
		public Shape((uint, uint, uint, uint, uint, uint, uint, uint) dimensions)
		{
			_dimensions = new uint[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4, dimensions.Item5, dimensions.Item6, dimensions.Item7, dimensions.Item8 };
			Size = _dimensions.Aggregate(1U, (value, accumulate) => value * accumulate);
			ThrowIfAnyNegative(_dimensions);
		}
		#endregion

		public override bool Equals(object? obj) => obj is Shape shape && Equals(shape);
		public bool Equals(Shape other) => _dimensions == other._dimensions || _dimensions.SequenceEqual(other._dimensions);
		public bool Equals(IShape? other)
		{
			if (other is null)
				return false;
			int r1 = Rank, r2 = other.Rank;
			if (r1 == r2)
			{
				bool b = true;
				for (int i = 0; i < r1; ++i)
					b &= this[i] == other[i];
				return b;
			}
			return false;
		}
		public override int GetHashCode() => HashCode.Combine(_dimensions);

		public static implicit operator Shape(uint dimension) => new Shape(dimension);
		public static implicit operator Shape((uint, uint) tuple) => new Shape(tuple);
		public static implicit operator Shape((uint, uint, uint) tuple) => new Shape(tuple);
		public static implicit operator Shape((uint, uint, uint, uint) tuple) => new Shape(tuple);
		public static implicit operator Shape((uint, uint, uint, uint, uint) tuple) => new Shape(tuple);
		public static implicit operator Shape((uint, uint, uint, uint, uint, uint) tuple) => new Shape(tuple);
		public static implicit operator Shape((uint, uint, uint, uint, uint, uint, uint) tuple) => new Shape(tuple);
		public static implicit operator Shape((uint, uint, uint, uint, uint, uint, uint, uint) tuple) => new Shape(tuple);

		public static bool operator ==(Shape left, Shape right) => left._dimensions.SequenceEqual(right._dimensions);
		public static bool operator !=(Shape left, Shape right) => !left._dimensions.SequenceEqual(right._dimensions);

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append('(');
			sb.AppendJoin(',', _dimensions ?? Array.Empty<uint>());
			sb.Append(')');
			return sb.ToString();
		}
	}
}
