using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiFileIO
{
    static class BinaryUtils
    {
        public static string ReadRawToAsciiString(byte[] raw, ref int index, int length)
        {
            string chunkType = Encoding.ASCII.GetString(raw, index, length);
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

        public static byte[] IntToVariableByteArr(int integer)
        {
            if (integer > Math.Pow(2, 28)) throw new OverflowException("Integer value too large");

            if (integer >= Math.Pow(2, 21))
            {
                return new byte[4]
                {
                    (byte)(0x80 | (integer >> 21)),
                    (byte)(0x80 | (integer >> 14)),
                    (byte)(0x80 | (integer >> 7)),
                    (byte)(0x7F & integer)
                };
            }
            else if (integer >= Math.Pow(2, 14))
            {
                return new byte[3]
                {
                    (byte)(0x80 | (integer >> 14)),
                    (byte)(0x80 | (integer >> 7)),
                    (byte)(0x7F & integer)
                };
            }
            else if (integer >= Math.Pow(2, 7))
            {
                return new byte[2]
                {
                    (byte)(0x80 | (integer >> 7)),
                    (byte)(0x7F & integer)
                };
            }
            else
            {
                return new byte[1]
                {
                    (byte)(0x7F & integer)
                };
            }
        }

        public static byte[] IntToByteArr(int value, int length)
        {
            byte[] bytes = new byte[length];
            int index = 0;
            int shiftIndex = length;
            while (index < length)
            {
                bytes[index] = (byte)(value >> (--shiftIndex * 8));
                index++;
            }
            return bytes;
        }
    }
}
