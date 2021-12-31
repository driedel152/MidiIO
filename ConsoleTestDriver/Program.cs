using MidiFileIO;
using System;

namespace ConsoleTestDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            MidiFileReader reader = new MidiFileReader("MyMidi.mid");
            MidiFile midiFile = reader.ReadMidiFile();
            Console.WriteLine(midiFile.header.division);
        }
    }
}
