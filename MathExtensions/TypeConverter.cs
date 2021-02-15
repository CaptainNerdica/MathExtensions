using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MathExtensions
{
	public static class TypeConverter<TInput, TOutput>
	{
		private static readonly Func<TInput, TOutput> _converter;
		static TypeConverter()
		{
			ParameterExpression p1 = Expression.Parameter(typeof(TInput));
			Expression c = Expression.ConvertChecked(p1, typeof(TOutput));
			_converter = Expression.Lambda<Func<TInput, TOutput>>(c, p1).Compile();
		}

		public static TOutput Convert(TInput value) => _converter(value);
	}
}
