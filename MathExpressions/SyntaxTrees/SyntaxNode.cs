using MathExpressions.Lexing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MathExpressions.SyntaxTrees
{
	public enum SyntaxNodeType
	{
		Root,
		Token,
		Identifier,
		Literal,
		Group,
		Assignment,
		Add,
		Subtract,
		Multiply,
		Divide,
		Function
	}
	public class SyntaxNode : IList<SyntaxNode>
	{
		public SyntaxNodeType NodeType { get; }
		public SyntaxNode? Parent { get; protected set; }
		public Token? Token { get; }
		public IList<SyntaxNode> Children { get; } = new List<SyntaxNode>();

		public int Count => Children.Count;

		public bool IsReadOnly => Children.IsReadOnly;

		public virtual SyntaxNode this[int i]
		{
			get => Children[i];
			set => Children[i] = value;
		}

		public SyntaxNode()
		{
			NodeType = SyntaxNodeType.Root;
		}
		public SyntaxNode(SyntaxNode root)
		{
			Parent = null;
			NodeType = SyntaxNodeType.Root;
			Children = root.Children;
		}
		public SyntaxNode(SyntaxNode parent, SyntaxNodeType type)
		{
			Parent = parent;
			Parent.Add(this);
			NodeType = type;
		}
		public SyntaxNode(SyntaxNode parent, SyntaxNodeType type, int index)
		{
			Parent = parent;
			Parent.Insert(index, this);
			NodeType = type;
		}
		public SyntaxNode(Token token)
		{
			NodeType = SyntaxNodeType.Token;
			Token = token;
		}
		public SyntaxNode(SyntaxNode parent, Token token)
		{
			Parent = parent;
			Parent.Add(this);
			NodeType = SyntaxNodeType.Token;
			Token = token;
		}
		public SyntaxNode(SyntaxNode parent, Token token, int index)
		{
			Parent = parent;
			Parent.Insert(index, parent);
			NodeType = SyntaxNodeType.Token;
			Token = token;
		}

		public void InsertAbove(SyntaxNode newParent)
		{
			Parent?.Add(newParent);
			Parent?.Remove(this);
			newParent.Add(this);
		}

		public void Add(SyntaxNode node)
		{
			Children.Add(node);
			node.Parent = this;
		}
		public void Insert(int index, SyntaxNode node)
		{
			Children.Insert(index, node);
			node.Parent = this;
		}
		public int IndexOf(SyntaxNode node) => Children.IndexOf(node);
		public bool Remove(SyntaxNode node)
		{
			if (Children.Remove(node))
			{
				node.Parent = null;
				return true;
			}
			return false;
		}
		public void RemoveAt(int index)
		{
			Children[index].Parent = null;
			Children.RemoveAt(index);
		}
		public void Clear() => Children.Clear();
		public bool Contains(SyntaxNode node) => Children.Contains(node);
		public void CopyTo(SyntaxNode[] array, int arrayIndex) => Children.CopyTo(array, arrayIndex);
		public IEnumerator<SyntaxNode> GetEnumerator() => Children.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Children).GetEnumerator();
	}
}