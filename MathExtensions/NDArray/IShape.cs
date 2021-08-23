using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	public interface IShape : IEquatable<IShape>
	{
		public uint this[int index] { get; }
		public int Rank { get; }
		public uint Size { get; }
		public ReadOnlySpan<uint> Span { get; }
	}
}
