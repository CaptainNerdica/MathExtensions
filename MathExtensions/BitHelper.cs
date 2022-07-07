using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int AsInt32(this bool value) => *(byte*)&value;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint AsUInt32(this bool value) => *(byte*)&value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong AddWithCarry(ulong left, ulong right, out ulong carry)
	{
		ulong o = left + right;
		carry = (o < left).AsUInt32();
		return o;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong Max(ulong x, ulong y)
	{
		uint b = ~((x > y).AsUInt32() - 1);
		return b & x + (~b & y);
	}

	public static ulong MaxVectorized(ulong x, ulong y)
	{
		Vector128<ulong> vx = Vector128.CreateScalar(x);
		Vector128<ulong> vy = Vector128.CreateScalar(y);
		return Vector128.Max(vx, vy).ToScalar();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint BitExtract(uint value, int start, int length)
	{
		int rs = sizeof(int) * 8 - length;
		return (value << (rs - start)) >> rs;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong BitExtract(ulong value, int start, int length)
	{
		int rs = sizeof(ulong) * 8 - length;
		return (value << (rs - start)) >> rs;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T BitExtract<T>(T value, int start, int length) where T : unmanaged, IBinaryInteger<T>
	{
		int rs = sizeof(T) * 8 - length;
		return (value << (rs - start)) >>> rs;
	}
}
