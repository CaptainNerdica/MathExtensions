using MathExpressions.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressions
{
	internal static class Extensions
	{
		public static IEnumerable<Token> TrimWhiteSpace(this IEnumerable<Token> tokens) => tokens.Where(t => t.Type != TokenType.WhiteSpace);
		public static bool IsValid(this IEnumerable<Token> tokens) => !tokens.Any(t => t.Type == TokenType.Unkown);
		public static string RebuildInput(this IEnumerable<Token> tokens)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Token t in tokens)
				sb.Append(t.Text);
			return sb.ToString();
		}
	}
}
