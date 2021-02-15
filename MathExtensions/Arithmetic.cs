using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public static class Arithmetic<T1, T2, TResult>
	{
		private static Func<T1, T2, TResult>? _add;
		private static Func<T1, T2, TResult>? _sub;
		private static Func<T1, T2, TResult>? _mul;
		private static Func<T1, T2, TResult>? _div;

		public static Func<T1, T2, TResult> BuildExpression(Func<Expression, Expression, Expression> op)
		{
			ParameterExpression param1 = Expression.Parameter(typeof(T1));
			ParameterExpression param2 = Expression.Parameter(typeof(T2));
			Expression body;
			try
			{
				body = Expression.ConvertChecked(op(param1, param1), typeof(TResult));
			}
			catch (InvalidOperationException)
			{
				try
				{
					body = Expression.ConvertChecked(op(param1, Expression.ConvertChecked(param2, typeof(T1))), typeof(TResult));
				}
				catch (InvalidOperationException)
				{
					body = Expression.ConvertChecked(op(Expression.ConvertChecked(param1, typeof(T2)), param2), typeof(TResult));
				}
			}
			return Expression.Lambda<Func<T1, T2, TResult>>(body, param1, param2).Compile();
		}

		private static TResult BuildEvalAdd(T1 left, T2 right) => (_add = BuildExpression(Expression.Add))(left, right);
		public static TResult Add(T1 left, T2 right) => _add is null ? BuildEvalAdd(left, right) : _add(left, right);

		private static TResult BuildEvalSub(T1 left, T2 right) => (_sub = BuildExpression(Expression.Subtract))(left, right);
		public static TResult Subtract(T1 left, T2 right) => _sub is null ? BuildEvalSub(left, right) : _sub(left, right);

		private static TResult BuildEvalMul(T1 left, T2 right) => (_mul = BuildExpression(Expression.Multiply))(left, right);
		public static TResult Multiply(T1 left, T2 right) => _mul is null ? BuildEvalMul(left, right) : _mul(left, right);

		private static TResult BuildEvalDiv(T1 left, T2 right) => (_div = BuildExpression(Expression.Divide))(left, right);
		public static TResult Divide(T1 left, T2 right) => _div is null ? BuildEvalDiv(left, right) : _div(left, right);
	}
}
