using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MathExpressions.Lexing
{
	public class Lexer
	{
		private static readonly IReadOnlyList<ITokenizer> _defaults = new List<ITokenizer>
		{
			new Tokenizer(new Regex(@"^\s+"), TokenType.WhiteSpace),
			new Tokenizer(new Regex(@"^(?:[a-zA-Z_])(?:[a-zA-Z_0-9])*"), TokenType.Indentifier),
			new Tokenizer(new Regex(@"^([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?<!\.)(?:[eE]([+-]?\d+))?"), TokenType.Literal),
			new Tokenizer(new Regex(@"^[\+\-\*\/^=]|(?:<=?|>=?)"), TokenType.Operator),
			new Tokenizer(new Regex(@"^[,.;:]"), TokenType.Separator),
			new Tokenizer(new Regex(@"^[\(\[\{]"), TokenType.OpenParenthesis),
			new Tokenizer(new Regex(@"^[\)\]\}]"), TokenType.ClosedParenthesis)
		};
		private readonly IList<ITokenizer> _tokenizers;
		public IReadOnlyList<ITokenizer> Tokenizers => (IReadOnlyList<ITokenizer>)_tokenizers;

		public Lexer()
		{
			_tokenizers = (IList<ITokenizer>)_defaults;
		}

		public Lexer(IEnumerable<ITokenizer> tokenizers)
		{
			_tokenizers = new List<ITokenizer>(tokenizers);
		}

		public IEnumerable<Token> Tokenize(string text, bool suppressErrors = false) => new TokenEnumerable(this, text, suppressErrors);
		public static IEnumerable<Token> Trim(IEnumerable<Token> tokens) => tokens.Where(t => t.Type != TokenType.WhiteSpace);
		public static bool Valid(IEnumerable<Token> tokens) => !tokens.Where(t => t.Type == TokenType.Unkown).Any();
	}
}
