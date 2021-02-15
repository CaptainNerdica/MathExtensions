using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressions.Lexing
{
	internal class TokenEnumerable : IEnumerable<Token>
	{
		public string Text { get; }
		public Lexer Lexer { get; }
		public bool SuppressErrors { get; }
		public TokenEnumerable(Lexer lexer, string text, bool suppressErrors)
		{
			Lexer = lexer;
			Text = text;
			SuppressErrors = suppressErrors;
		}

		IEnumerator<Token> IEnumerable<Token>.GetEnumerator() => new Enumerator(this);
		IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);


		public struct Enumerator : IEnumerator<Token>
		{
			private readonly TokenEnumerable _tokenList;
			private byte _state;
			private int _currentPos;
			private Token? _current;
			public Enumerator(TokenEnumerable tokenList)
			{
				_state = 0;
				_tokenList = tokenList;
				_currentPos = 0;
				_current = default;
			}

			public Token Current => _current ?? throw new InvalidOperationException("Enumeration has either not started or has already finished.");

			object? IEnumerator.Current
			{
				get
				{
					if (_currentPos <= 0 || _currentPos >= _tokenList.Text.Length)
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					return Current;
				}
			}

			void IDisposable.Dispose() => GC.SuppressFinalize(this);
			bool IEnumerator.MoveNext()
			{
				if (_state == 2)
					return false;
				if (_state == 0)
				{
					_current = new Token(string.Empty, TokenType.Start, -1);
					_state++;
					return true;
				}
				string input = _tokenList.Text[_currentPos..];
				if (input.Length == 0)
				{
					_currentPos = input.Length;
					_current = new Token(string.Empty, TokenType.End, _currentPos);
					_state++;
					return true;
				}
				Token? output = default;
				int length = 0;
				foreach (ITokenizer tokenizer in _tokenList.Lexer.Tokenizers)
				{
					output = tokenizer.NextToken(input, _currentPos, out length);
					if (output != null)
						break;
				}
				if (output == null)
				{
					if (_tokenList.SuppressErrors)
						output = MoveNextRare(out length);
					else
						throw new Exception($"Unrecognized token at position {_currentPos}");
				}
				_current = output;
				_currentPos += length;
				return _current != null;
			}
			private Token? MoveNextRare(out int length)
			{
				length = 0;
				string input = _tokenList.Text[(_currentPos + length)..];
				Token? output = null;
				while (output == null && input.Length != 0)
				{
					length++;
					input = _tokenList.Text[(_currentPos + length)..];
					foreach (ITokenizer tokenizer in _tokenList.Lexer.Tokenizers)
					{
						output = tokenizer.NextToken(input, _currentPos + length, out _);
						if (output != null)
							break;
					}
				}
				return new Token(_tokenList.Text.Substring(_currentPos, length), TokenType.Unkown, _currentPos);
			}

			void IEnumerator.Reset()
			{
				_currentPos = 0;
				_current = null;
			}
		}
	}
}
