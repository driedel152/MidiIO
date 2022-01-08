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
            MidiFile midiFile = CreateMidiFile();

            Console.WriteLine("Format: " + midiFile.header.format);
            Console.WriteLine("Division: " + ((DivisionPPQN)midiFile.header.division).pulsesPerQuarterNote);
            foreach (Track t in midiFile.tracks)
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

        public static MidiFile CreateMidiFile()
        {
            MidiEvent e1 = new NoteOnEvent(0, 60, 40);
            e1.deltaTime = 0;
            MidiEvent e2 = new NoteOffEvent(0, 60, 40);
            e2.deltaTime = 1000;
            MidiEvent e3 = new NoteOnEvent(0, 62, 40);
            e3.deltaTime = 0;
            MidiEvent e4 = new NoteOffEvent(0, 62, 40);
            e4.deltaTime = 5000;
            MidiEvent e5 = new EndOfTrackEvent();
            e5.deltaTime = 0;
            MidiEvent[] events = new MidiEvent[] { e1, e2, e3, e4, e5 };
            Track track = new Track(events);

            MidiFileHeader header = new MidiFileHeader(MidiFileFormat.SingleTrack, 1, new DivisionPPQN(300));

            return new MidiFile(header, new Track[] { track });
        }
    }
}
