using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public unsafe static class Arithmetic
	{

		public static TOut Add<T1, T2, TOut>(T1 left, T2 right) => BuildGetFunc(ref BinaryExprStore<T1, T2, TOut>.Add, &Expression.Add)(left, right);
		public static T Add<T>(T left, T right) => Add(left, right);
		public static TOut Subtract<T1, T2, TOut>(T1 left, T2 right) => BuildGetFunc(ref BinaryExprStore<T1, T2, TOut>.Subtract, &Expression.Subtract)(left, right);
		public static T Subtract<T>(T left, T right) => Subtract(left, right);
		public static TOut Multiply<T1, T2, TOut>(T1 left, T2 right) => BuildGetFunc(ref BinaryExprStore<T1, T2, TOut>.Multiply, &Expression.Multiply)(left, right);
		public static T Multiply<T>(T left, T right) => Multiply(left, right);
		public static TOut Divide<T1, T2, TOut>(T1 left, T2 right) => BuildGetFunc(ref BinaryExprStore<T1, T2, TOut>.Divide, &Expression.Divide)(left, right);
		public static T Divide<T>(T left, T right) => Divide(left, right);
		public static TOut Modulo<T1, T2, TOut>(T1 left, T2 right) => BuildGetFunc(ref BinaryExprStore<T1, T2, TOut>.Modulo, &Expression.Modulo)(left, right);
		public static T Modulo<T>(T left, T right) => Modulo(left, right);

		private static Func<T1, T2, TOut> BuildGetFunc<T1, T2, TOut>(ref Func<T1, T2, TOut>? prop, delegate*<Expression, Expression, Expression> op) => prop ??= (Func<T1, T2, TOut>)BuildBinaryExpression(op, typeof(T1), typeof(T2), typeof(TOut));

		private static class BinaryExprStore<T1, T2, TOut>
		{
			public static Func<T1, T2, TOut>? Add;
			public static Func<T1, T2, TOut>? Subtract;
			public static Func<T1, T2, TOut>? Multiply;
			public static Func<T1, T2, TOut>? Divide;
			public static Func<T1, T2, TOut>? Modulo;

			//public static Func<T1, T2, TOut>? And;
			//public static Func<T1, T2, TOut>? Or;
			//public static Func<T1, T2, TOut>? ExcusiveOr;

			//public static Func<T1, T2, TOut>? ShiftLeft;
			//public static Func<T1, T2, TOut>? ShiftRight;
			//public static Func<T1, T2, TOut>? ArithmeticShiftLeft;
			//public static Func<T1, T2, TOut>? ArithmeticShiftRight;
		}

		internal static Delegate BuildBinaryExpression(delegate*<Expression, Expression, Expression> op, Type t1, Type t2, Type tOut)
		{
			ParameterExpression param1 = Expression.Parameter(t1);
			ParameterExpression param2 = Expression.Parameter(t2);
			Expression body;
			try
			{
				body = Expression.ConvertChecked(op(param1, param2), tOut);
			}
			catch (InvalidOperationException)
			{
				try
				{
					body = Expression.ConvertChecked(op(param1, Expression.ConvertChecked(param2, t1)), tOut);
				}
				catch (InvalidOperationException)
				{
					body = Expression.ConvertChecked(op(Expression.ConvertChecked(param1, t2), param2), tOut);
				}
			}
			return Expression.Lambda(body, param1, param2).Compile();
		}
	}
}
