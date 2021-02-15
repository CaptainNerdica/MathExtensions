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
	public partial class NDArray<T> : NDArrayBase<T>
	{
		private readonly T[] _items;

		public override T this[int i]
		{
			get => _items[i];
			set => _items[i] = value;
		}
		public override T this[Index i]
		{
			get => _items[i.ToLinear(Shape)];
			set => _items[i.ToLinear(Shape)] = value;
		}
		public override bool IsReadOnly => true;
		#region Constructors
		public NDArray() : base()
		{
			_items = Array.Empty<T>();
		}
		public NDArray(int size)
		{
			if (size == 0)
			{
				_items = Array.Empty<T>();
				Shape = Shape.Empty;
			}
			else
			{
				_items = new T[size];
				Shape = new Shape(size);
			}
		}
		public NDArray(params int[] dimensions) : this(new Shape(dimensions)) { }
		public NDArray(Shape shape)
		{
			Shape = shape;
			if (shape.Size == 0)
				_items = Array.Empty<T>();
			else
				_items = new T[shape.Size];
		}
		public NDArray(T[] items, bool copyItems = true) : this(new Shape(items.Length), items, copyItems) { }
		public NDArray(Shape shape, T[] items, bool copyItems = true)
		{
			int size = shape.Size;
			if (size < items.Length)
				throw new ArgumentException($"Shape of size {shape.Size} cannot be applied to an array of size {items.Length}", nameof(shape));
			ArraySegment<T> s = new ArraySegment<T>(items, 0, size);
			if (copyItems)
			{
				_items = new T[size];
				s.CopyTo(_items);
			}
			else
				_items = items[0..size];
			Shape = shape;
		}
		public NDArray(ICollection<T> items) : this(new Shape(items.Count), items) { }
		public NDArray(Shape shape, ICollection<T> items)
		{
			if (shape.Size != items.Count)
				throw new ArgumentException($"Shape of size {shape.Size} cannot be applied to a collection of size {items.Count}", nameof(shape));
			_items = new T[items.Count];
			items.CopyTo(_items, 0);
			Shape = shape;
		}

		private NDArray(Shape shape, IEnumerable<T> items)
		{
			if (shape.Size >= items.Count())
				throw new ArgumentException($"Shape of size {shape.Size} cannot be applied to a collection of size {items.Count()}", nameof(shape));
			_items = new T[shape.Size];
			items.Take(shape.Size).ToArray().CopyTo(_items, 0);
			Shape = shape;
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
		public override NDArray<TCast> Cast<TCast>() => new NDArray<TCast>(Shape, _items.Cast<TCast>());

		public NDArray<TOut> Operation<TOut>(Func<T, TOut> func) where TOut : struct
		{
			NDArray<TOut> output = new NDArray<TOut>(Shape);
			Parallel.For(0, Size, i => output._items[i] = func(_items[i]));
			return output;
		}
		public NDArray<TOut> Operation<TOther, TOut>(NDArray<TOther> other, Func<T, TOther, TOut> func) where TOther : struct where TOut : struct
		{
			if (Shape != other.Shape)
				throw new ArgumentException("Shapes are unequal", nameof(other));
			NDArray<TOut> output = new NDArray<TOut>(Shape);
			Parallel.For(0, Size, i => output._items[i] = func(_items[i], other._items[i]));
			return output;
		}
		public NDArray<TOut> Operation<TOther, TOut>(NDArray<TOther> other, Func<T, TOther, int, TOut> func) where TOther : struct where TOut : struct
		{
			if (Shape != other.Shape)
				throw new ArgumentException("Shapes are unequal", nameof(other));
			NDArray<TOut> output = new NDArray<TOut>(Shape);
			Parallel.For(0, Size, i => output._items[i] = func(_items[i], other._items[i], i));
			return output;
		}
		public NDArray<TOut> Operation<TOther, TOut>(NDArray<TOther> other, Func<T, TOther, Index, TOut> func) where TOther : struct where TOut : struct
		{
			if (Shape != other.Shape)
				throw new ArgumentException("Shapes are unequal", nameof(other));
			NDArray<TOut> output = new NDArray<TOut>(Shape);
			Parallel.For(0, Size, i => output._items[i] = func(_items[i], other._items[i], Index.FromLinear(i, Shape)));
			return output;
		}


		public NDArray<TOut> Add<TOther, TOut>(NDArray<TOther> other)
		{
			if (Shape != other.Shape)
				throw new ArgumentException("Shapes are unequal", nameof(other));
			NDArray<TOut> output = new NDArray<TOut>(Shape);
			Parallel.For(0, Size, i => output._items[i] = Arithmetic<T, TOther, TOut>.Add(_items[i], other._items[i]));
			return output;
		}
	}
}
