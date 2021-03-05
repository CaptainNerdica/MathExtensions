using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct QuadUnion
	{
		[FieldOffset(0)]
		internal Quadruple x;
		[FieldOffset(0)]
		internal UInt128 i;
	}
}
