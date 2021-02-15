using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public readonly struct Shape : IEquatable<Shape>
	{
		private readonly int[] _dimensions;
		public int[] Dimensions => _dimensions ?? Array.Empty<int>();
		public int Rank => _dimensions?.Length ?? 0;
		public int Size => _dimensions?.Aggregate(1, (value, accumulate) => value * accumulate) ?? 0;
		public Index MinIndex => new Index(new int[Rank]);
		public Index MaxIndex => new Index(_dimensions.Select(i => i - 1).ToArray());

		public int this[int index] => _dimensions?[index] ?? throw new IndexOutOfRangeException();

		public static readonly Shape Empty = new Shape(0);
		public Shape(params int[] dimensions)
		{
			CheckAllPositive(dimensions);
			_dimensions = new int[dimensions.Length];
			dimensions.CopyTo(_dimensions, 0);
		}
		public Shape(ITuple dimensions)
		{
			if (!dimensions.GetType().GetGenericArguments().All(t => t == typeof(int)))
				throw new ArgumentException("All Tuple type parameters must be int", nameof(dimensions));
			_dimensions = new int[dimensions.Length];
			for (int i = 0; i < dimensions.Length; i++)
				_dimensions[i] = (int)(dimensions[i] ?? 0);
			CheckAllPositive(_dimensions);
		}
		#region ValueTuple Constructors
		public Shape(int dimension)
		{
			if (dimension == 0)
				_dimensions = Array.Empty<int>();
			else
				_dimensions = new int[] { dimension };
		}
		public Shape((int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2 };
			CheckAllPositive(_dimensions);
		}
		public Shape((int, int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2, dimensions.Item3 };
			CheckAllPositive(_dimensions);
		}
		public Shape((int, int, int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4 };
			CheckAllPositive(_dimensions);
		}
		public Shape((int, int, int, int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4, dimensions.Item5 };
			CheckAllPositive(_dimensions);
		}
		public Shape((int, int, int, int, int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4, dimensions.Item5, dimensions.Item6 };
			CheckAllPositive(_dimensions);
		}
		public Shape((int, int, int, int, int, int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4, dimensions.Item5, dimensions.Item6, dimensions.Item7 };
			CheckAllPositive(_dimensions);
		}
		public Shape((int, int, int, int, int, int, int, int) dimensions)
		{
			_dimensions = new int[] { dimensions.Item1, dimensions.Item2, dimensions.Item3, dimensions.Item4, dimensions.Item5, dimensions.Item6, dimensions.Item7, dimensions.Item8 };
			CheckAllPositive(_dimensions);
		}
		#endregion

		private static void CheckAllPositive(int[] dimensions)
		{
			if (dimensions.Any(i => i < 0))
				throw new IndexOutOfRangeException("All dimension sizes must be positive");
		}

		public override bool Equals(object? obj) => obj is Shape shape && Equals(shape);
		public bool Equals(Shape other) => _dimensions.SequenceEqual(other._dimensions);
		public override int GetHashCode() => HashCode.Combine(_dimensions);

		public static implicit operator Shape(int dimension) => new Shape(dimension);
		public static implicit operator Shape((int, int) tuple) => new Shape(tuple);
		public static implicit operator Shape((int, int, int) tuple) => new Shape(tuple);
		public static implicit operator Shape((int, int, int, int) tuple) => new Shape(tuple);
		public static implicit operator Shape((int, int, int, int, int) tuple) => new Shape(tuple);
		public static implicit operator Shape((int, int, int, int, int, int) tuple) => new Shape(tuple);
		public static implicit operator Shape((int, int, int, int, int, int, int) tuple) => new Shape(tuple);
		public static implicit operator Shape((int, int, int, int, int, int, int, int) tuple) => new Shape(tuple);
		public static implicit operator Index(Shape s) => new Index(s._dimensions);

		public static bool operator ==(Shape left, Shape right) => left._dimensions.SequenceEqual(right._dimensions);
		public static bool operator !=(Shape left, Shape right) => !left._dimensions.SequenceEqual(right._dimensions);

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append('(');
			sb.AppendJoin(',', _dimensions ?? Array.Empty<int>());
			sb.Append(')');
			return sb.ToString();
		}
	}
}
