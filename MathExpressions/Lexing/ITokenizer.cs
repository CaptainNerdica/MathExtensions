using System;
using System.Collections.Generic;
using System.Text;

namespace MathExpressions.Lexing
{
	public interface ITokenizer
	{
		public Token? NextToken(string input, in int startIndex, out int length);
	}
}
