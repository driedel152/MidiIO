using System;
using System.IO;

namespace MidiFileIO
{
    public class MidiFileReader
    {
        byte[] raw;

        public MidiFileReader(string path)
        {
            raw = File.ReadAllBytes(path);
            ReadHeader();
        }

        private void ReadHeader()
        {
            // Chunk Type
            string chunkType = System.Text.Encoding.ASCII.GetString(raw, 0, 4);
            Console.WriteLine(chunkType);

            int i = 4;

            // Length
            int length = ByteArrToInt(raw, 4, 4);
            Console.WriteLine("Length: " + length);

            // Data
            int format = ByteArrToInt(raw, 8, 2);
            Console.WriteLine("Format: " + format);
            int tracks = ByteArrToInt(raw, 10, 2);
            Console.WriteLine("Tracks: " + tracks);
            int division = ByteArrToInt(raw, 12, 2);
            Console.WriteLine(division);
            if(division >> 15 == 0)
            {
                int ticksPerQuarterNote = division & 0xEFFF;
                Console.WriteLine("Ticks/quarter note: " + ticksPerQuarterNote);
            }
            else
            {
                int framesPerSecond = (division & 0xEF00) >> 8;
                Console.WriteLine("Frames/sec: " + framesPerSecond);
                int ticksPerFrame = division & 0x00FF;
                Console.WriteLine("Ticks/frame: " + ticksPerFrame);
            }
        }

        private int ByteArrToInt(byte[] raw, int index, int length)
        {
            int value = 0;
            while(length > 0)
            {
                value |= raw[index++] << (--length * 8);
            }

            return value;
        }
    }
}
