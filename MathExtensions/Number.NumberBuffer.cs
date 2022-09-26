using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions;
internal static partial class Number
{
    internal const int UInt256NumberBufferLength = 79 + 1;

    internal unsafe ref struct NumberBuffer
    {
        public int DigitsCount;
        public int Scale;
        public bool IsNegative;
        public bool HasNonZeroTail;
        public NumberBufferKind Kind;
        public Span<byte> Digits;

        public NumberBuffer(NumberBufferKind kind, byte* digits, int digitsLength) : this(kind, new Span<byte>(digits, digitsLength))
        {
            Debug.Assert(digits != null);
        }

        /// <summary>Initializes the NumberBuffer.</summary>
        /// <param name="kind">The kind of the buffer.</param>
        /// <param name="digits">The digits scratch space. The referenced memory must not be moveable, e.g. stack memory, pinned array, etc.</param>
        public NumberBuffer(NumberBufferKind kind, Span<byte> digits)
        {
            Debug.Assert(!digits.IsEmpty);

            DigitsCount = 0;
            Scale = 0;
            IsNegative = false;
            HasNonZeroTail = false;
            Kind = kind;
            Digits = digits;
#if DEBUG
                Digits.Fill(0xCC);
#endif
            Digits[0] = (byte)'\0';
            CheckConsistency();
        }

#pragma warning disable CA1822
        [Conditional("DEBUG")]
        public void CheckConsistency()
        {
#if DEBUG
                Debug.Assert((Kind == NumberBufferKind.Integer) || (Kind == NumberBufferKind.Decimal) || (Kind == NumberBufferKind.FloatingPoint));
                Debug.Assert(Digits[0] != '0', "Leading zeros should never be stored in a Number");

                int numDigits;
                for (numDigits = 0; numDigits < Digits.Length; numDigits++)
                {
                    byte digit = Digits[numDigits];

                    if (digit == 0)
                    {
                        break;
                    }

                    Debug.Assert(char.IsAsciiDigit((char)digit), $"Unexpected character found in Number: {digit}");
                }

                Debug.Assert(numDigits == DigitsCount, "Null terminator found in unexpected location in Number");
                Debug.Assert(numDigits < Digits.Length, "Null terminator not found in Number");
#endif // DEBUG
        }
#pragma warning restore CA1822

        public byte* GetDigitsPointer()
        {
            // This is safe to do since we are a ref struct
            return (byte*)(Unsafe.AsPointer(ref Digits[0]));
        }

        //
        // Code coverage note: This only exists so that Number displays nicely in the VS watch window. So yes, I know it works.
        //
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('[');
            sb.Append('"');

            for (int i = 0; i < Digits.Length; i++)
            {
                byte digit = Digits[i];

                if (digit == 0)
                {
                    break;
                }

                sb.Append((char)(digit));
            }

            sb.Append('"');
            sb.Append(", Length = ").Append(DigitsCount);
            sb.Append(", Scale = ").Append(Scale);
            sb.Append(", IsNegative = ").Append(IsNegative);
            sb.Append(", HasNonZeroTail = ").Append(HasNonZeroTail);
            sb.Append(", Kind = ").Append(Kind);
            sb.Append(']');

            return sb.ToString();
        }
    }

    internal enum NumberBufferKind : byte
    {
        Unknown = 0,
        Integer = 1,
        Decimal = 2,
        FloatingPoint = 3,
    }
}
