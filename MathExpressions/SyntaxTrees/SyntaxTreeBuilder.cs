using MathExpressions.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressions.SyntaxTrees
{
	public class SyntaxTreeBuilder
	{
		public HashSet<string> Symbols { get; }
		public SyntaxTree SyntaxTree { get; }

		public SyntaxTreeBuilder()
		{
			Symbols = new HashSet<string>();
			SyntaxTree = new SyntaxTree();
		}
		public SyntaxTreeBuilder(IEnumerable<Token> tokens)
		{
			Symbols = new HashSet<string>(tokens.Where(t => t.Type == TokenType.Indentifier).Select(t => t.Text));
			SyntaxTree = new SyntaxTree(tokens);
		}
	}
}
