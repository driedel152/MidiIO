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
            MidiFileReader reader = new MidiFileReader("Movie_Themes_-_Back_to_the_Future.mid");
            MidiFile midiFile = reader.ReadMidiFile();

            Console.WriteLine("Format: " + midiFile.header.format);
            foreach(Track t in midiFile.tracks)
            {
                foreach(MidiEvent e in t.events)
                {
                    if(e is UnknownMetaEvent)
                    {
                        Console.WriteLine("Unknown MetaEvent of type " + ((UnknownMetaEvent)e).type);
                    }
                }
            }

            MidiFileWriter writer = new MidiFileWriter(midiFile, "MyOtherMidi.mid");
            writer.WriteMidiFile(true);

            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
        }
    }
}
