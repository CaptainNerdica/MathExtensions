using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MathExpressions.Lexing
{
	internal class Tokenizer : ITokenizer
	{
		public virtual Regex Regex { get; }
		public virtual TokenType Lexeme { get; }

		public Tokenizer(Regex regex, TokenType lexeme)
		{
			Regex = regex;
			Lexeme = lexeme;
		}

		public virtual Token? NextToken(string input, in int startIndex, out int length)
		{
			Match match = Regex.Match(input);
			length = 0;
			if (match.Success)
			{
				length = match.Length;
				return new Token(match.Value, Lexeme, startIndex);
			}
			else
				return default;
		}
	}
}
