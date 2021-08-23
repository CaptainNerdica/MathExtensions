using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public abstract class NDArrayBase<T> : INDArray<T>
	{
		public abstract Span<T> Elements { get; }
		public abstract T this[int index] { get; set; }
		public abstract T this[ReadOnlySpan<uint> index] { get; set; }
		public abstract IShape Shape { get; set; }
		public int Rank => Shape.Rank;
		public uint Size => Shape.Size;
		public uint Length => Shape.Size;

		IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);
		IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

		public abstract void Fill(T value);

		public abstract void Clear();
		public abstract void Add(T item);
		public abstract bool Contains(T item);
		public abstract void CopyTo(T[] array, int arrayIndex);
		public abstract bool Remove(T item);

		public class Enumerator : IEnumerator<T>
		{
			private readonly NDArrayBase<T> _array;
			private uint _index;
			private T? _current;

			internal Enumerator(NDArrayBase<T> array)
			{
				_array = array;
				_index = 0;
				_current = default;
			}

			public T Current => _current ?? default!;

			object? IEnumerator.Current => _index == 0 || _index == _array.Size + 1 ? throw new InvalidOperationException("Enumeration has either not started or has already finished") : Current;

			void IDisposable.Dispose() => GC.SuppressFinalize(this);
			bool IEnumerator.MoveNext()
			{
				if (_index < _array.Size)
				{
					_current = _array[(int)_index];
					_index++;
					return true;
				}
				_index = _array.Size + 1U;
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
