using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiFileIO
{
    static internal class VariableLengthQuantity
    {
        public static IEnumerable<byte> ToVlqCollection(ulong integer)
        {
            if (integer > Math.Pow(2, 56))
                throw new OverflowException("Integer exceeds max value.");

            var index = 7;
            var significantBitReached = false;
            var mask = 0x7fUL << (index * 7);
            while (index >= 0)
            {
                var buffer = (mask & integer);
                if (buffer > 0 || significantBitReached)
                {
                    significantBitReached = true;
                    buffer >>= index * 7;
                    if (index > 0)
                        buffer |= 0x80;
                    yield return (byte)buffer;
                }
                mask >>= 7;
                index--;
            }
        }


    }
}
