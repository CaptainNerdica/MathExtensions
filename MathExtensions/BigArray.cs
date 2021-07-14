using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public class BigArray<T> : ICollection<T>, IList<T>
	{
		const int defaultBinSize = 1 << 20;
		private readonly T[][] _bins;

		public int BinSize { get; }
		public long Length { get; }
		public int Count => _bins.Length;
		public bool IsReadOnly => false;

		public T this[int index]
		{
			get => CreateOrGetBin(index / BinSize)[index % BinSize];
			set => CreateOrGetBin(index / BinSize)[index % BinSize] = value;
		}
		public T this[long index]
		{
			get => CreateOrGetBin(index / BinSize)[index % BinSize];
			set => CreateOrGetBin(index / BinSize)[index % BinSize] = value;
		}

		public BigArray()
		{
			Length = 0;
			BinSize = 0;
			_bins = Array.Empty<T[]>();
		}

		public BigArray(long length, bool initBins = false) : this(length, defaultBinSize, initBins) { }

		public BigArray(long length, int binSize, bool initBins = false)
		{
			Length = length;
			BinSize = binSize;
			long fullBins = Length / BinSize;
			long count = (long)Math.Ceiling((double)Length / BinSize);
			_bins = new T[count][];
			if (initBins)
			{
				for (long i = 0; i < fullBins; ++i)
					_bins[i] = new T[BinSize];
				if (count != fullBins)
					_bins[^1] = new T[length % BinSize];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		private T[] CreateOrGetBin(long bin) => _bins[bin] ?? (_bins[bin] = _bins.Length - bin > 1 ? new T[BinSize] : new T[Length - BinSize * (_bins.Length - 1L)]);

		void ICollection<T>.Add(T item) => throw new NotSupportedException("Collection is of fixed size");

		public void Clear()
		{
			for (int i = 0; i < _bins.Length; ++i)
				if (_bins[i] != null)
					Array.Clear(_bins[i], 0, _bins[i].Length);
		}

		bool ICollection<T>.Contains(T item) => _bins.Any(b => b.Contains(item));

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			if (array.Length - arrayIndex > Length)
				throw new ArgumentException("The number of elements in the source is greater than the available space from arrayIndex to the end of the destination array.", nameof(array));
			int index = 0;
			for (int i = 0; i < _bins.Length; ++i)
			{
				_bins[i].CopyTo(array, index);
				index += _bins[i].Length;
			}
		}

		bool ICollection<T>.Remove(T item) => throw new NotSupportedException("Collection is of fixed size");

		IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

		IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

		public int IndexOf(T item)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		private class Enumerator : IEnumerator<T>
		{
			private long _currentIndex;
			private readonly BigArray<T> _array;
			private T? _current;

			internal Enumerator(BigArray<T> array)
			{
				_array = array;
				_currentIndex = -1;
				_current = default;
			}

			public T Current => _current!;

			object IEnumerator.Current => Current!;

			void IDisposable.Dispose() { }

			bool IEnumerator.MoveNext()
			{
				if (++_currentIndex >= _array.Length)
					return false;
				else
					_current = _array[_currentIndex];
				return true;
			}

			void IEnumerator.Reset()
			{
				_currentIndex = -1;
				_current = default;
			}
		}
	}
}
