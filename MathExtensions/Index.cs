using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public readonly struct Index : IEquatable<Index>, IComparable, IComparable<Index>
	{
		private readonly int[] _dimensions;
		public int[] Dimensions => _dimensions ?? Array.Empty<int>();
		public int Rank => _dimensions?.Length ?? 0;
		public int Size => _dimensions?.Aggregate(1, (value, accumulate) => value * accumulate) ?? 0;

		public int this[int index] => _dimensions?[index] ?? throw new IndexOutOfRangeException();

		public static readonly Index Empty = new Index(0);
		public Index(params int[] dimensions) : this((Span<int>)dimensions) { }

		public Index(Span<int> dimensions)
		{
			if (dimensions.Length == 0)
			{
				_dimensions = Array.Empty<int>();
				return;
			}
			CheckAllPositive(dimensions);
			_dimensions = dimensions.ToArray();
		}
		#region ValueTuple Constructors
		public Index(int dimension)
		{
			if (dimension == 0)
				_dimensions = Array.Empty<int>();
			else
				_dimensions = new int[] { dimension };
		}
		public Index((int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2 };
			CheckAllPositive(_dimensions);
		}
		public Index((int, int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2, dimensions.Item3 };
			CheckAllPositive(_dimensions);
		}
		public Index((int, int, int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4 };
			CheckAllPositive(_dimensions);
		}
		public Index((int, int, int, int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4, dimensions.Item5 };
			CheckAllPositive(_dimensions);
		}
		public Index((int, int, int, int, int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4, dimensions.Item5, dimensions.Item6 };
			CheckAllPositive(_dimensions);
		}
		public Index((int, int, int, int, int, int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4, dimensions.Item5, dimensions.Item6, dimensions.Item7 };
			CheckAllPositive(_dimensions);
		}
		public Index((int, int, int, int, int, int, int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4, dimensions.Item5, dimensions.Item6, dimensions.Item7, dimensions.Item8 };
			CheckAllPositive(_dimensions);
		}
		#endregion

		private static void CheckAllPositive(Span<int> dimensions)
		{
			for (int i = 0; i < dimensions.Length; ++i)
				if (dimensions[i] < 0)
					throw new IndexOutOfRangeException("All dimension sizes must be positive");
		}
		private static double GetDistance(Index index)
		{
			int sum = 0;
			int rank = index.Rank;
			for (int i = 0; i < rank; i++)
				sum += index._dimensions[i] * index._dimensions[i];
			return Math.Sqrt(sum);
		}

		public bool Equals(Index other) => !(_dimensions is null || other._dimensions is null) && _dimensions.SequenceEqual(other._dimensions);
		public override bool Equals(object? obj) => obj is Index index && Equals(index);
		public int CompareTo(Index value)
		{
			int rank0 = Rank, rank1 = value.Rank;
			if (rank0 < rank1)
				return -1;
			else if (rank0 > rank1)
				return 1;
			else if (this == value)
				return 0;
			else
				return GetDistance(this).CompareTo(GetDistance(value));
		}
		[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
		public int CompareTo(object? value) => value == null ? 1 : value is Index i ? CompareTo(i) : throw new ArgumentException($"Object must be of type {nameof(Index)}");
		public override int GetHashCode()
		{
			HashCode h = new HashCode();
			int rank = Rank;
			for (int i = 0; i < rank; i++)
				h.Add(_dimensions[i]);
			return h.ToHashCode();
		}

		public int ToLinear(Shape s) => ToLinear(this, s);
		public static int ToLinear(Index i, Shape s)
		{
			if (ValidIndex(i, s))
			{
				int linearIndex = 0;
				for (int j = i.Rank - 1; j >= 1; j--)
					linearIndex = s[i.Rank - 1 - j] * (linearIndex + i._dimensions[j]);
				linearIndex += i._dimensions[0];
				return linearIndex;
			}
			else
				throw new Exception($"{i} is not a valid index of {s}");
		}
		public static Index FromLinear(int i, Shape s)
		{
			if (i < 0 || i >= s.Size)
				throw new IndexOutOfRangeException();
			if (s.Rank == 0)
				return Empty;
			int[] index = new int[s.Rank];
			int size = 1;
			for (int j = 0; j < s.Rank; j++)
			{
				index[i] = i / size % s[i];
				size *= s[i];
			}
			return new Index(index);
		}

		public static bool ValidIndex(Index index, Shape shape)
		{
			if (index.Rank != shape.Rank)
				return false;
			for (int i = 0; i < index.Rank; i++)
				if (index[i] >= shape[i])
					return false;
			return true;
		}

		public static implicit operator Index(int dimension) => new Index(dimension);
		public static implicit operator Index((int, int) tuple) => new Index(tuple);
		public static implicit operator Index((int, int, int) tuple) => new Index(tuple);
		public static implicit operator Index((int, int, int, int) tuple) => new Index(tuple);
		public static implicit operator Index((int, int, int, int, int) tuple) => new Index(tuple);
		public static implicit operator Index((int, int, int, int, int, int) tuple) => new Index(tuple);
		public static implicit operator Index((int, int, int, int, int, int, int) tuple) => new Index(tuple);
		public static implicit operator Index((int, int, int, int, int, int, int, int) tuple) => new Index(tuple);
		public static implicit operator Shape(Index i) => new Shape(i._dimensions);

		public static bool operator ==(Index left, Index right) => left.Equals(right);
		public static bool operator !=(Index left, Index right) => !left.Equals(right);
		public static bool operator <(Index left, Index right) => left.CompareTo(right) == -1;
		public static bool operator <=(Index left, Index right)
		{
			int comp = left.CompareTo(right);
			return comp == -1 || comp == 0;
		}
		public static bool operator >(Index left, Index right) => left.CompareTo(right) == 1;
		public static bool operator >=(Index left, Index right)
		{
			int comp = left.CompareTo(right);
			return comp == 1 || comp == 0;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append('(');
			sb.AppendJoin(", ", _dimensions ?? Array.Empty<int>());
			sb.Append(')');
			return sb.ToString();
		}
	}
}
