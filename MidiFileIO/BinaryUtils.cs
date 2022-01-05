using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiFileIO
{
    static class BinaryUtils
    {
        public static string ReadRawToAsciiString(byte[] raw, ref int index, int length)
        {
            string chunkType = System.Text.Encoding.ASCII.GetString(raw, index, length);
            index += length;
            return chunkType;
        }

        public static byte[] ReadRawToByteArr(byte[] raw, ref int index, int length)
        {
            byte[] data = new byte[length];
            Array.Copy(raw, index, data, 0, length);
            index += length;
            return data;
        }

        public static int ReadRawToInt(byte[] raw, ref int index, int length)
        {
            int value = 0;
            while (length > 0)
            {
                value |= raw[index++] << (--length * 8);
            }

            return value;
        }

        public static int ReadVariableLengthRawToInt(byte[] raw, ref int index)
        {
            int value = 0;
            bool lastByte = false;
            while (!lastByte)
            {
                value <<= 7;
                value |= raw[index] & 0x7F;
                if (raw[index] >> 7 == 0)
                {
                    lastByte = true;
                }
                index++;
            }
            return value;
        }

        public static IEnumerable<byte> IntToVariableByteArr(int integer)
        {
            if (integer > Math.Pow(2, 28)) throw new OverflowException("Integer value too large");

            if (integer >= Math.Pow(2, 21))
            {
                yield return (byte)(0x80 | (integer >> 21));
            }
            if (integer >= Math.Pow(2, 14))
            {
                yield return (byte)(0x80 | (integer >> 14));
            }
            if (integer >= Math.Pow(2, 7))
            {
                yield return (byte)(0x80 | (integer >> 7));
            }
            yield return (byte)(0x7F & integer);
        }

        public static IEnumerable<byte> IntToByteArr(int value, int length)
        {
            byte[] bytes = new byte[length];
            int index = 0;
            int shiftIndex = length;
            while (index < length)
            {
                yield return (byte)(value >> (--shiftIndex * 8));
                index++;
            }
        }
    }
}
