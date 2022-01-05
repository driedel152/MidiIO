using MidiFileIO;
using System;

namespace ConsoleTestDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            MidiFileReader reader;
            MidiFile midiFile;
            MidiFileWriter writer;

            for (int i = 0; i < 1000; i++)
            {
                reader = new MidiFileReader("MyMidi.mid");
                midiFile = reader.ReadMidiFile();

                writer = new MidiFileWriter(midiFile, "MyOtherMidi.mid");
                writer.WriteMidiFile();
            }

            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
        }
    }
}
