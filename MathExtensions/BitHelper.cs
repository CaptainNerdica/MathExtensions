using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions;

internal unsafe static class BitHelper
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint GetHigh(ulong* value) => BitConverter.IsLittleEndian ? ((uint*)value)[1] : (uint)(*value >> 32);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint GetLow(ulong* value) => *(uint*)value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort GetHigh(uint* value) => BitConverter.IsLittleEndian ? ((ushort*)value)[1] : (ushort)(*value >> 32);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort GetLow(uint* value) => *(ushort*)value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte GetHigh(ushort* value) => BitConverter.IsLittleEndian ? ((byte*)value)[1] : (byte)(*value >> 8);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte GetLow(ushort* value) => *(byte*)value;
}
