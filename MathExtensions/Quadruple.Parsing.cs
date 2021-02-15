using System;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace MathExtensions
{
	internal static unsafe class QuadrupleParsing
	{
		static readonly Regex _numberRegex = new Regex(@"^[+-]?(\d[,_]?)+(\d)?(\.\d*)?([eE][+-]?\d+)?$");
		internal static bool TryParse(ReadOnlySpan<char> input, out Quadruple result)
		{
			result = default;
			NumberFormatInfo info = NumberFormatInfo.InvariantInfo;
			input = input.Trim();
			string text = new string(input);
			if (input == info.NegativeInfinitySymbol)
			{
				result = Quadruple.NegativeInfinity;
				return true;
			}
			if (input == info.PositiveInfinitySymbol)
			{
				result = Quadruple.PositiveInfinity;
				return true;
			}
			if (input == info.NaNSymbol)
			{
				result = Quadruple.NaN;
				return true;
			}
			if (!_numberRegex.IsMatch(text))
				return false;
			result = QuadFromText(text);
			return true;
		}

		private static Quadruple QuadFromText(string input)
		{
			ReadOnlySpan<char> inputSpan = input.AsSpan();
			int exponentIndex = input.IndexOf('e', StringComparison.OrdinalIgnoreCase);
			ReadOnlySpan<char> exponentSpan = inputSpan[(exponentIndex + 1)..];
			int exponent = int.Parse(exponentSpan);

			return default;
		}

		internal static Quadruple Parse(ReadOnlySpan<char> input)
		{
			if (!TryParse(input, out Quadruple result))
				throw new FormatException("Input string was not in a correct format.");
			return result;
		}
	}

	internal readonly ref struct NumberBuffer
	{
		public Span<char> Chars { get; }

		public NumberBuffer(Span<char> chars)
		{
			Chars = chars;
		}
	}
}