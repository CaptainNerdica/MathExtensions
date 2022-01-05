using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public partial class NDArray<T>
	{
		private static class EmptyNDArray<TArray> where TArray : struct
		{
			public static readonly NDArray<TArray> Value = new NDArray<TArray>();
		}
		public static NDArray<T> Empty => EmptyNDArray<T>.Value;
	}

	public static class NDArray
	{
		//	public static INDArray<T> Add<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, IAdditionOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] + right[i];
		//		return a;
		//	}
		//	public static INDArray<T> Add<T>(INDArray<T> left, T right)
		//		where T : unmanaged, IAdditionOperators<T, T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] + right;
		//		return a;
		//	}
		//	public static INDArray<T> Add<T>(T left, INDArray<T> right)
		//		where T : unmanaged, IAdditionOperators<T, T, T>
		//		=> Add(right, left);

		//	public static void Add<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IAdditionOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] + right[i];
		//	}
		//	public static void Add<T>(RefNDArray<T> left, T right, RefNDArray<T> output)
		//		where T : unmanaged, IAdditionOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] + right;
		//	}
		//	public static void Add<T>(T left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IAdditionOperators<T, T, T>
		//		=> Add(right, left, output);

		//	public static INDArray<T> Subtract<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, ISubtractionOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] - right[i];
		//		return a;
		//	}
		//	public static INDArray<T> Subtract<T>(INDArray<T> left, T right)
		//		where T : unmanaged, ISubtractionOperators<T, T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] - right;
		//		return a;
		//	}
		//	public static INDArray<T> Subtract<T>(T left, INDArray<T> right)
		//		where T : unmanaged, ISubtractionOperators<T, T, T>
		//		=> Subtract(right, left);

		//	public static void Subtract<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, ISubtractionOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] - right[i];
		//	}
		//	public static void Subtract<T>(RefNDArray<T> left, T right, RefNDArray<T> output)
		//		where T : unmanaged, ISubtractionOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] - right;
		//	}
		//	public static void Subtract<T>(T left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, ISubtractionOperators<T, T, T>
		//		=> Subtract(right, left, output);

		//	public static INDArray<T> Multiply<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, IMultiplyOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<T> a = new NDArray<T>(left.Shape);

		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] * right[i];
		//		return a;
		//	}
		//	public static INDArray<T> Multiply<T>(INDArray<T> left, T right)
		//		where T : unmanaged, IMultiplyOperators<T, T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] * right;
		//		return a;
		//	}
		//	public static INDArray<T> Multiply<T>(T left, INDArray<T> right)
		//		where T : unmanaged, IMultiplyOperators<T, T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(right.Shape);
		//		for (int i = 0; i < right.Length; i++)
		//			a[i] = left * right[i];
		//		return a;
		//	}

		//	public static void Multiply<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IMultiplyOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] * right[i];
		//	}
		//	public static void Multiply<T>(RefNDArray<T> left, T right, RefNDArray<T> output)
		//		where T : unmanaged, IMultiplyOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] * right;
		//	}
		//	public static void Multiply<T>(T left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IMultiplyOperators<T, T, T>
		//		=> Multiply(right, left, output);

		//	public static INDArray<T> Divide<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, IDivisionOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] / right[i];
		//		return a;
		//	}
		//	public static INDArray<T> Divide<T>(INDArray<T> left, T right)
		//		where T : unmanaged, IDivisionOperators<T, T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] / right;
		//		return a;
		//	}
		//	public static INDArray<T> Divide<T>(T left, INDArray<T> right)
		//		where T : unmanaged, IDivisionOperators<T, T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(right.Shape);
		//		for (int i = 0; i < right.Length; i++)
		//			a[i] = left / right[i];
		//		return a;
		//	}

		//	public static void Divide<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IDivisionOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] / right[i];
		//	}
		//	public static void Divide<T>(RefNDArray<T> left, T right, RefNDArray<T> output)
		//		where T : unmanaged, IDivisionOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] / right;
		//	}
		//	public static void Divide<T>(T left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IDivisionOperators<T, T, T>
		//		=> Divide(right, left, output);

		//	public static INDArray<T> Modulus<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, IModulusOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] % right[i];
		//		return a;
		//	}
		//	public static INDArray<T> Modulus<T>(INDArray<T> left, T right)
		//		where T : unmanaged, IModulusOperators<T, T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] % right;
		//		return a;
		//	}
		//	public static INDArray<T> Modulus<T>(T left, INDArray<T> right)
		//		where T : unmanaged, IModulusOperators<T, T, T>
		//		=> Modulus(right, left);

		//	public static void Modulus<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IModulusOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] % right[i];
		//	}
		//	public static void Modulus<T>(RefNDArray<T> left, T right, RefNDArray<T> output)
		//		where T : unmanaged, IModulusOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] % right;
		//	}
		//	public static void Modulus<T>(T left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IModulusOperators<T, T, T>
		//		=> Modulus(right, left, output);

		//	public static INDArray<T> Negate<T>(INDArray<T> value)
		//		where T : unmanaged, IUnaryNegationOperators<T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(value.Shape);
		//		for (int i = 0; i < value.Length; i++)
		//			a[i] = -value[i];
		//		return a;
		//	}
		//	public static void Negate<T>(RefNDArray<T> value, RefNDArray<T> output)
		//		where T : unmanaged, IUnaryNegationOperators<T, T>
		//	{
		//		for (int i = 0; i < value.Length; i++)
		//			output[i] = -value[i];
		//	}
		//	public static INDArray<T> Plus<T>(INDArray<T> value)
		//		where T : unmanaged, IUnaryPlusOperators<T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(value.Shape);
		//		for (int i = 0; i < value.Length; i++)
		//			a[i] = +value[i];
		//		return a;
		//	}
		//	public static void Plus<T>(RefNDArray<T> value, RefNDArray<T> output)
		//		where T : unmanaged, IUnaryPlusOperators<T, T>
		//	{
		//		for (int i = 0; i < value.Length; i++)
		//			output[i] = +value[i];
		//	}

		//	public static INDArray<T> BitwiseAnd<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] & right[i];
		//		return a;
		//	}
		//	public static INDArray<T> BitwiseAnd<T>(INDArray<T> left, T right)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] & right;
		//		return a;
		//	}
		//	public static INDArray<T> BitwiseAnd<T>(T left, INDArray<T> right)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//		=> BitwiseAnd(right, left);

		//	public static void BitwiseAnd<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] & right[i];
		//	}
		//	public static void BitwiseAnd<T>(RefNDArray<T> left, T right, RefNDArray<T> output)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] & right;
		//	}
		//	public static void BitwiseAnd<T>(T left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//		=> BitwiseAnd(right, left, output);

		//	public static INDArray<T> BitwiseOr<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] | right[i];
		//		return a;
		//	}
		//	public static INDArray<T> BitwiseOr<T>(INDArray<T> left, T right)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] | right;
		//		return a;
		//	}
		//	public static INDArray<T> BitwiseOr<T>(T left, INDArray<T> right)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//		=> BitwiseOr(right, left);

		//	public static void BitwiseOr<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] | right[i];
		//	}
		//	public static void BitwiseOr<T>(RefNDArray<T> left, T right, RefNDArray<T> output)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] | right;
		//	}
		//	public static void BitwiseOr<T>(T left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//		=> BitwiseOr(right, left, output);

		//	public static INDArray<T> BitwiseExclusiveOr<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] ^ right[i];
		//		return a;
		//	}
		//	public static INDArray<T> BitwiseExclusiveOr<T>(INDArray<T> left, T right)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] ^ right;
		//		return a;
		//	}
		//	public static INDArray<T> BitwiseExclusiveOr<T>(T left, INDArray<T> right)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//		=> BitwiseExclusiveOr(right, left);

		//	public static void BitwiseExclusiveOr<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] ^ right[i];
		//	}
		//	public static void BitwiseExclusiveOr<T>(RefNDArray<T> left, T right, RefNDArray<T> output)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] ^ right;
		//	}
		//	public static void BitwiseExclusiveOr<T>(T left, RefNDArray<T> right, RefNDArray<T> output)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//		=> BitwiseExclusiveOr(right, left, output);

		//	public static INDArray<T> BitwiseComplement<T>(INDArray<T> value)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(value.Shape);
		//		for (int i = 0; i < value.Length; i++)
		//			a[i] = ~value[i];
		//		return a;
		//	}

		//	public static void BitwiseComplement<T>(RefNDArray<T> value, RefNDArray<T> output)
		//		where T : unmanaged, IBitwiseOperators<T, T, T>
		//	{
		//		for (int i = 0; i < value.Length; i++)
		//			output[i] = ~value[i];
		//	}

		//	public static INDArray<T> ShiftLeft<T>(INDArray<T> value, int amount)
		//		where T : unmanaged, IShiftOperators<T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(value.Shape);
		//		for (int i = 0; i < value.Length; i++)
		//			a[i] = value[i] << amount;
		//		return a;
		//	}
		//	public static INDArray<T> ShiftLeft<T>(INDArray<T> value, INDArray<int> amount)
		//		where T : unmanaged, IShiftOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(value.Shape, amount.Shape);
		//		NDArray<T> a = new NDArray<T>(value.Shape);
		//		for (int i = 0; i < value.Length; i++)
		//			a[i] = value[i] << amount[i];
		//		return a;
		//	}

		//	public static void ShiftLeft<T>(RefNDArray<T> value, int amount, RefNDArray<T> output)
		//		where T : unmanaged, IShiftOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(value.Shape, output.Shape);
		//		for (int i = 0; i < value.Length; i++)
		//			output[i] = value[i] << amount;
		//	}
		//	public static void ShiftLeft<T>(RefNDArray<T> value, RefNDArray<int> amount, RefNDArray<T> output)
		//		where T : unmanaged, IShiftOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(value.Shape, amount.Shape);
		//		ThrowIfShapesUnequal(value.Shape, output.Shape);
		//		for (int i = 0; i < value.Length; i++)
		//			output[i] = value[i] << amount[i];
		//	}

		//	public static INDArray<T> ShiftRight<T>(INDArray<T> value, int amount)
		//		where T : unmanaged, IShiftOperators<T, T>
		//	{
		//		NDArray<T> a = new NDArray<T>(value.Shape);
		//		for (int i = 0; i < value.Length; i++)
		//			a[i] = value[i] >> amount;
		//		return a;
		//	}
		//	public static INDArray<T> ShiftRight<T>(INDArray<T> value, NDArray<int> amount)
		//		where T : unmanaged, IShiftOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(value.Shape, amount.Shape);
		//		NDArray<T> a = new NDArray<T>(value.Shape);
		//		for (int i = 0; i < value.Length; i++)
		//			a[i] = value[i] >> amount[i];
		//		return a;
		//	}

		//	public static void ShiftRight<T>(RefNDArray<T> value, int amount, RefNDArray<T> output)
		//		where T : unmanaged, IShiftOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(value.Shape, output.Shape);
		//		for (int i = 0; i < value.Length; i++)
		//			output[i] = value[i] >> amount;
		//	}
		//	public static void ShiftRight<T>(RefNDArray<T> value, RefNDArray<int> amount, RefNDArray<T> output)
		//		where T : unmanaged, IShiftOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(value.Shape, amount.Shape);
		//		ThrowIfShapesUnequal(value.Shape, output.Shape);
		//		for (int i = 0; i < value.Length; i++)
		//			output[i] = value[i] >> amount[i];
		//	}

		//	public static INDArray<bool> GreaterThan<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<bool> a = new NDArray<bool>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] > right[i];
		//		return a;
		//	}
		//	public static INDArray<bool> GreaterThan<T>(INDArray<T> left, T right)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		NDArray<bool> a = new NDArray<bool>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] > right;
		//		return a;
		//	}
		//	public static INDArray<bool> GreaterThan<T>(T left, INDArray<T> right)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//		=> GreaterThan(right, left);

		//	public static void GreaterThan<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] > right[i];
		//	}
		//	public static void GreaterThan<T>(RefNDArray<T> left, T right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] > right;
		//	}
		//	public static void GreaterThan<T>(T left, RefNDArray<T> right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//		=> GreaterThan(right, left, output);

		//	public static INDArray<bool> LessThan<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<bool> a = new NDArray<bool>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] < right[i];
		//		return a;
		//	}
		//	public static INDArray<bool> LessThan<T>(INDArray<T> left, T right)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		NDArray<bool> a = new NDArray<bool>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] < right;
		//		return a;
		//	}
		//	public static INDArray<bool> LessThan<T>(T left, INDArray<T> right)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//		=> LessThan(right, left);

		//	public static void LessThan<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] < right[i];
		//	}
		//	public static void LessThan<T>(RefNDArray<T> left, T right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] < right;
		//	}
		//	public static void LessThan<T>(T left, RefNDArray<T> right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//		=> LessThan(right, left, output);

		//	public static INDArray<bool> GreaterThanOrEqual<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<bool> a = new NDArray<bool>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] > right[i];
		//		return a;
		//	}
		//	public static INDArray<bool> GreaterThanOrEqual<T>(INDArray<T> left, T right)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		NDArray<bool> a = new NDArray<bool>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] >= right;
		//		return a;
		//	}
		//	public static INDArray<bool> GreaterThanOrEqual<T>(T left, INDArray<T> right)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//		=> GreaterThanOrEqual(right, left);

		//	public static void GreaterThanOrEqual<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] >= right[i];
		//	}
		//	public static void GreaterThanOrEqual<T>(RefNDArray<T> left, T right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] >= right;
		//	}
		//	public static void GreaterThanOrEqual<T>(T left, RefNDArray<T> right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//		=> GreaterThanOrEqual(right, left, output);

		//	public static INDArray<bool> LessThanOrEqual<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<bool> a = new NDArray<bool>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] <= right[i];
		//		return a;
		//	}
		//	public static INDArray<bool> LessThanOrEqual<T>(INDArray<T> left, T right)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		NDArray<bool> a = new NDArray<bool>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] <= right;
		//		return a;
		//	}
		//	public static INDArray<bool> LessThanOrEqual<T>(T left, INDArray<T> right)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//		=> LessThanOrEqual(right, left);

		//	public static void LessThanOrEqual<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] <= right[i];
		//	}
		//	public static void LessThanOrEqual<T>(RefNDArray<T> left, T right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] <= right;
		//	}
		//	public static void LessThanOrEqual<T>(T left, RefNDArray<T> right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//		=> LessThanOrEqual(right, left, output);

		//	public static INDArray<bool> Equal<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, IEqualityOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<bool> a = new NDArray<bool>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] == right[i];
		//		return a;
		//	}
		//	public static INDArray<bool> Equal<T>(INDArray<T> left, T right)
		//		where T : unmanaged, IEqualityOperators<T, T>
		//	{
		//		NDArray<bool> a = new NDArray<bool>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] == right;
		//		return a;
		//	}
		//	public static INDArray<bool> Equal<T>(T left, INDArray<T> right)
		//		where T : unmanaged, IEqualityOperators<T, T>
		//		=> Equal(right, left);

		//	public static void Equal<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<bool> output)
		//		where T : unmanaged, IEqualityOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] == right[i];
		//	}
		//	public static void Equal<T>(RefNDArray<T> left, T right, RefNDArray<bool> output)
		//		where T : unmanaged, IEqualityOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] == right;
		//	}
		//	public static void Equal<T>(T left, RefNDArray<T> right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//		=> Equal(right, left, output);

		//	public static INDArray<bool> NotEqual<T>(INDArray<T> left, INDArray<T> right)
		//		where T : unmanaged, IEqualityOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		NDArray<bool> a = new NDArray<bool>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] != right[i];
		//		return a;
		//	}
		//	public static INDArray<bool> NotEqual<T>(INDArray<T> left, T right)
		//		where T : unmanaged, IEqualityOperators<T, T>
		//	{
		//		NDArray<bool> a = new NDArray<bool>(left.Shape);
		//		for (int i = 0; i < left.Length; i++)
		//			a[i] = left[i] != right;
		//		return a;
		//	}
		//	public static INDArray<bool> NotEqual<T>(T left, INDArray<T> right)
		//		where T : unmanaged, IEqualityOperators<T, T>
		//		=> NotEqual(right, left);

		//	public static void NotEqual<T>(RefNDArray<T> left, RefNDArray<T> right, RefNDArray<bool> output)
		//		where T : unmanaged, IEqualityOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, right.Shape);
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] != right[i];
		//	}
		//	public static void NotEqual<T>(RefNDArray<T> left, T right, RefNDArray<bool> output)
		//		where T : unmanaged, IEqualityOperators<T, T>
		//	{
		//		ThrowIfShapesUnequal(left.Shape, output.Shape);
		//		for (int i = 0; i < left.Length; ++i)
		//			output[i] = left[i] != right;
		//	}
		//	public static void NotEqual<T>(T left, RefNDArray<T> right, RefNDArray<bool> output)
		//		where T : unmanaged, IComparisonOperators<T, T>
		//		=> NotEqual(right, left, output);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ThrowIfShapesUnequal(IShape a, IShape b) { if (a.Equals(b)) throw new ArgumentException($"Incompatible shapes {a} and {b}"); }
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ThrowIfShapesUnequal(RefShape a, RefShape b) { if (a.Equals(b)) throw new ArgumentException($"Incompatible shapes {a.ToString()} and {b.ToString()}"); }
	}
}
