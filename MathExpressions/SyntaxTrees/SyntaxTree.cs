using MathExpressions.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressions.SyntaxTrees
{
	public class SyntaxTree
	{
		public SyntaxNode Root { get; }
		public SyntaxNode this[int i]
		{
			get => Root[i];
			set => Root[i] = value;
		}

		public SyntaxTree()
		{
			Root = new SyntaxNode();
		}
		public SyntaxTree(SyntaxNode rootNode)
		{
			Root = new SyntaxNode(rootNode);
		}
		public SyntaxTree(IEnumerable<Token> tokens)
		{
			Root = new SyntaxNode();
			foreach(Token token in tokens)
				Root.Add(new SyntaxNode(token));
		}
	}
}
