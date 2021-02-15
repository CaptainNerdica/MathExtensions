using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public abstract class NDArrayBase<T> : IEnumerable<T>, ICollection<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
	{
		private Shape _shape;
		public abstract T this[int index] { get; set; }
		public abstract T this[Index index] { get; set; }
		public Shape Shape
		{
			get => _shape;
			set
			{
				if (value.Size != _shape.Size)
					throw new ArgumentException($"Shapes of size {value.Size} and {_shape.Size} are incompatible", nameof(value));
				_shape = value;
			}
		}
		public int Rank => Shape.Rank;
		public int Size => Shape.Size;
		public int Count => Shape.Size;

		public abstract bool IsReadOnly { get; }

		public NDArrayBase()
		{
			_shape = Shape.Empty;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);
		IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

		public abstract void Fill(T value);
		public abstract NDArrayBase<TCast> Cast<TCast>();

		public abstract void Clear();
		public abstract void Add(T item);
		public abstract bool Contains(T item);
		public abstract void CopyTo(T[] array, int arrayIndex);
		public abstract bool Remove(T item);

		public class Enumerator : IEnumerator<T>
		{
			private readonly NDArrayBase<T> _array;
			private int _index;
			private T? _current;

			internal Enumerator(NDArrayBase<T> array)
			{
				_array = array;
				_index = 0;
				_current = default;
			}

			public T Current => _current!;

			object? IEnumerator.Current => _index == 0 || _index == _array.Size + 1 ? throw new InvalidOperationException("Enumeration has either not started or has already finished") : Current;

			void IDisposable.Dispose() => GC.SuppressFinalize(this);
			bool IEnumerator.MoveNext()
			{
				if ((uint)_index < (uint)_array.Size)
				{
					_current = _array[_index];
					_index++;
					return true;
				}
				_index = _array.Size + 1;
				_current = default;
				return false;
			}
			void IEnumerator.Reset()
			{
				_index = 0;
				_current = default;
			}

			~Enumerator() => ((IDisposable)this).Dispose();
		}
	}
}
