using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace MathExpressions.Lexing
{
	public enum TokenType
	{
		Unkown,
		Start,
		End,
		WhiteSpace,
		Indentifier,
		Literal,
		Operator,
		OpenParenthesis,
		ClosedParenthesis,
		Equals,
		Separator
	}

	public readonly struct Token : IEquatable<Token>
	{
		public string Text { get; }
		public TokenType Type { get; }
		public int Column { get; }
		public int Row { get; }
		public int Length => Text.Length;

		public Token(string text, TokenType type, int index)
		{
			Text = text;
			Type = type;
			Column = index;
			Row = 0;
		}
		public Token(string text, TokenType type, int column, int row)
		{
			Text = text;
			Type = type;
			Column = column;
			Row = row;
		}

		public override string ToString() => string.Format("({0}, '{1}')", Type, Regex.Unescape(Text));
		bool IEquatable<Token>.Equals(Token other) => other.Type == Type && other.Text == Text;
		public override bool Equals(object? obj) => obj is Token t && Equals(t);

		public static bool operator ==(Token left, Token right) => left.Equals(right);

		public static bool operator !=(Token left, Token right) => !left.Equals(right);

		public override int GetHashCode() => HashCode.Combine(Text, Type);
	}
}
