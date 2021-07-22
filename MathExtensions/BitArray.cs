using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public class BitArray : IEnumerable<bool>, IList<bool>
	{
		private const int size = sizeof(byte) * 8;
		private readonly byte[] _bits;
		public int Count { get; }
		public bool IsReadOnly => false;

		public bool this[int index]
		{
			get
			{
				if (index >= Count)
					throw new IndexOutOfRangeException();
				GetIndices(index, out int p, out int q);
				return (_bits[p] & (1U << q)) != 0;
			}
			set
			{
				if (index >= Count)
					throw new IndexOutOfRangeException();
				GetIndices(index, out int p, out int q);
				uint v = 1U << q;
				uint c = _bits[p] & ~v;
				if (value)
					c |= v;
				_bits[p] = (byte)c;
			}
		}

		public BitArray()
		{
			Count = 0;
			_bits = Array.Empty<byte>();
		}

		public BitArray(int length)
		{
			Count = length;
			int arrayLength = MathExtensions.DivideRoundAway(length, size);
			_bits = new byte[arrayLength];
		}

		public BitArray(byte[] bits) : this(bits.AsSpan()) { }
		public BitArray(byte[] bits, int length) : this(bits.AsSpan(), length) { }

		public BitArray(Span<byte> bits)
		{
			Count = bits.Length * size;
			_bits = bits.ToArray();
		}

		public BitArray(Span<byte> bits, int length)
		{
			Count = length;
			if (bits.Length * size < length)
				throw new ArgumentException("Source is too short", nameof(bits));
			int arrayLength = MathExtensions.DivideRoundAway(length, size);
			_bits = bits.Slice(0, arrayLength).ToArray();
			int r = length % size;
			_bits[^1] &= (byte)((1U << r) - 1);
		}

		private static void GetIndices(int index, out int p, out int q)
		{
			p = (int)((uint)index / size);
			q = (int)((uint)index % size);
		}

		public static BitArray operator ~(BitArray array)
		{
			if (array is null)
				throw new ArgumentNullException(nameof(array));
			BitArray b = new BitArray(array.Count);
			for (int i = 0; i < array.Count; i++)
				b._bits[i] = (byte)~array._bits[i];
			int r = (int)((uint)array.Count % size);
			if (r > 0)
				b._bits[^1] &= (byte)((1U << r) - 1);
			return b;
		}
		public static BitArray operator &(BitArray left, BitArray right)
		{
			if (left is null)
				throw new ArgumentNullException(nameof(left));
			if (right is null)
				throw new ArgumentNullException(nameof(right));
			if (left.Count != right.Count)
				throw new ArgumentException("Arrays are not the same length.");
			BitArray b = new BitArray(left.Count);
			for (int i = 0; i < left._bits.Length; i++)
				b._bits[i] = (byte)(left._bits[i] & right._bits[i]);
			return b;
		}
		public static BitArray operator |(BitArray left, BitArray right)
		{
			if (left is null)
				throw new ArgumentNullException(nameof(left));
			if (right is null)
				throw new ArgumentNullException(nameof(right));
			if (left.Count != right.Count)
				throw new ArgumentException("Arrays are not the same length.");
			BitArray b = new BitArray(left.Count);
			for (int i = 0; i < left._bits.Length; i++)
				b._bits[i] = (byte)(left._bits[i] | right._bits[i]);
			return b;
		}
		public static BitArray operator ^(BitArray left, BitArray right)
		{
			if (left is null)
				throw new ArgumentNullException(nameof(left));
			if (right is null)
				throw new ArgumentNullException(nameof(right));
			if (left.Count != right.Count)
				throw new ArgumentException("Arrays are not the same length.");
			BitArray b = new BitArray(left.Count);
			for (int i = 0; i < left._bits.Length; i++)
				b._bits[i] = (byte)(left._bits[i] ^ right._bits[i]);
			return b;
		}

		public override bool Equals(object? obj) => base.Equals(obj);
		public override int GetHashCode()
		{
			HashCode hc = new HashCode();
			for (int i = 0; i < _bits.Length; i++)
				hc.Add(_bits[i]);
			return hc.ToHashCode();
		}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			int b, s;
			for (int i = 0; i < _bits.Length - 1; i++)
			{
				b = _bits[i];
				s = 1;
				for (int j = 0; j < size; j++)
				{
					bool v = (b & s) != 0;
					sb.Insert(0, v ? '1' : '0');
					s <<= 1;
				}
			}
			b = _bits[^1];
			s = 1;
			for (int j = 0; j < Count % size; j++)
			{
				bool v = (b & s) != 0;
				sb.Insert(0, v ? '1' : '0');
				s <<= 1;
			}
			return sb.ToString();
		}

		public int IndexOf(bool value)
		{
			int i = 0;
			int j = 0;
			if (value)
			{
				while (_bits[i] == 0 && i < _bits.Length)
					i++;
				if (i >= _bits.Length)
					return -1;
				int v = _bits[i];
				while ((v & (1 << j)) == 0 && j < size)
					j++;
			}
			else
			{
				while (_bits[i] != byte.MaxValue && i < _bits.Length)
					i++;
				if (i >= _bits.Length)
					return -1;
				int v = _bits[i];
				while ((v & (1 << j)) != 0 && j < size)
					j++;
			}
			int index = i * size + j;
			if (j >= size || index >= Count)
				return -1;
			return index;
		}
		public void Insert(int index, bool value) => throw new NotSupportedException("Collection is of fixed size.");
		public void RemoveAt(int index) => throw new NotSupportedException("Collection is of fixed size.");
		public void Add(bool value) => throw new NotSupportedException("Collection is of fixed size.");
		public void Clear() => _bits.AsSpan().Clear();
		public bool Contains(bool value)
		{
			int i = 0;
			if (value)
			{
				while (_bits[i] == 0 && i < _bits.Length)
					i++;
			}
			else
			{
				while (_bits[i] != byte.MaxValue && i < _bits.Length)
					i++;
			}
			return i < _bits.Length;
		}
		public void CopyTo(bool[] array, int index)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array), "Destination array cannot be null.");
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index), "The starting array index cannot be negative.");
			if (Count > array.Length - index)
				throw new ArgumentException("The destination array has fewer elements than the collection.", nameof(array));
			for (int i = 0; i < Count; i++)
				array[i + index] = this[i];
		}
		public bool Remove(bool item) => throw new NotSupportedException("Collection is of fixed size.");

		public IEnumerator<bool> GetEnumerator() => new Enumerator(this);

		IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

		private struct Enumerator : IEnumerator<bool>
		{
			private readonly BitArray _array;
			private int _index;

			public Enumerator(BitArray array)
			{
				_array = array;
				_index = -1;
				Current = default;
			}

			public bool Current { get; private set; }

			object IEnumerator.Current => Current;

			void IDisposable.Dispose() { }

			bool IEnumerator.MoveNext()
			{
				if (++_index >= _array.Count)
					return false;
				Current = _array[_index];
				return true;
			}

			void IEnumerator.Reset()
			{
				_index = -1;
				Current = default;
			}
		}
	}
}
