using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public partial class NDArray<T> : NDArrayBase<T> where T : struct
	{
		private IShape _shape;
		private readonly T[] _items;

		public override IShape Shape
		{
			get => _shape;
			set
			{
				if (value.Size != _shape.Size)
					throw new ArgumentException($"Shapes of size {value.Size} and {_shape.Size} are incompatible", nameof(value));
				_shape = value;
			}
		}
		public override T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _items[index];
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => _items[index] = value;
		}
		public override T this[ReadOnlySpan<uint> index]
		{
			get => _items[_shape.ToLinearIndex(index)];
			set => _items[_shape.ToLinearIndex(index)] = value;
		}
		public override Span<T> Elements => _items;
		#region Constructors
		public NDArray()
		{
			_shape = new Shape();
			_items = Array.Empty<T>();
		}
		public NDArray(uint size)
		{
			if (size == 0)
			{
				_items = Array.Empty<T>();
				_shape = new Shape();
			}
			else
			{
				_items = new T[size];
				_shape = new Shape(size);
			}
		}
		public NDArray(params uint[] dimensions) : this(new Shape(dimensions)) { }
		public NDArray(IShape shape)
		{
			_shape = shape;
			if (shape.Size == 0)
				_items = Array.Empty<T>();
			else
				_items = new T[shape.Size];
		}
		public NDArray(T[] items, bool copyItems = true) : this(new Shape((uint)items.Length), items, copyItems) { }
		public NDArray(IShape shape, T[] items, bool copyItems = true)
		{
			uint size = shape.Size;
			if (size < items.Length)
				throw new ArgumentException($"Shape of size {shape.Size} cannot be applied to an array of size {items.Length}", nameof(shape));
			ArraySegment<T> s = new ArraySegment<T>(items, 0, (int)size);
			if (copyItems)
			{
				_items = new T[size];
				s.CopyTo(_items);
			}
			else
				_items = items[0..(int)size];
			_shape = shape;
		}
		public NDArray(ICollection<T> items) : this(new Shape((uint)items.Count), items) { }
		public NDArray(IShape shape, ICollection<T> items)
		{
			if (shape.Size != items.Count)
				throw new ArgumentException($"Shape of size {shape.Size} cannot be applied to a collection of size {items.Count}", nameof(shape));
			_items = new T[items.Count];
			items.CopyTo(_items, 0);
			_shape = shape;
		}

		private NDArray(IShape shape, IEnumerable<T> items)
		{
			if (shape.Size >= items.Count())
				throw new ArgumentException($"Shape of size {shape.Size} cannot be applied to a collection of size {items.Count()}", nameof(shape));
			_items = new T[shape.Size];
			items.Take((int)shape.Size).ToArray().CopyTo(_items, 0);
			_shape = shape;
		}
		#endregion
		#region ICollection<T> Interface Methods
		public override void Add(T item) => throw new NotSupportedException($"The {nameof(NDArray<T>)} is read-only");
		public override void Clear() => Array.Clear(_items, 0, _items.Length);
		public override bool Contains(T item) => _items.Contains(item);
		public override void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
		public override bool Remove(T item) => throw new NotSupportedException($"The {nameof(NDArray<T>)} is read-only");
		#endregion

		public override void Fill(T value) => Array.Fill(_items, value);
		public NDArray<TCast> Cast<TCast>() where TCast : struct => new NDArray<TCast>(Shape, _items.Cast<TCast>());
	}
}
