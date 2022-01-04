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
            foreach (MidiEvent e in midiFile.tracks[0].events)
            {
                if (e is UnknownMetaEvent)
                {                    
                    Console.WriteLine(e);
                }
            }

            MidiFileWriter writer = new MidiFileWriter(midiFile, "MyOtherMidi.mid");
            writer.WriteMidiFile();
        }
    }
}
