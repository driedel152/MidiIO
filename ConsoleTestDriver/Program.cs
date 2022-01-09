using MidiIO;
using System;
using System.Collections.Generic;

namespace ConsoleTestDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            MidiFileReader reader = new MidiFileReader("Test_-_test1.mid");
            Sequence sequence = reader.ReadMidiFile();

            Console.WriteLine("Format: " + reader.fileFormat);
            Console.WriteLine("Division: " + ((DivisionPPQN)sequence.division).pulsesPerQuarterNote);
            Console.WriteLine("Track count: " + sequence.tracks.Count);
            foreach (Track t in sequence.tracks)
            {
                foreach(MidiEvent e in t.GetEvents())
                {
                    if (e is TimeSignatureEvent)
                    {
                        TimeSignatureEvent timeSignatureEvent = (TimeSignatureEvent)e;
                        Console.WriteLine($"Time Signature: {timeSignatureEvent.numerator}/{Math.Pow(2, timeSignatureEvent.denominator)}, " +
                            $"{timeSignatureEvent.clocksPerMetronomeTick} clocks per tick, " +
                            $"{timeSignatureEvent.thirtySecondNotesPerTwentyFourClocks} 32nd/24-clocks");
                    }
                    if (e is UnknownMetaEvent)
                    {
                        Console.WriteLine("Unknown MetaEvent of type " + ((UnknownMetaEvent)e).type);
                    }
                }
            }

            MidiFileWriter writer = new MidiFileWriter(sequence, "MyOtherMidi.mid");
            writer.WriteMidiFile(reader.fileFormat);
            
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
        }

        public static Sequence CreateSequence()
        {
            Track track = new Track();
            track.AddEvent(0, new SetTempoEvent(500000));
            track.AddEvent(0, new NoteOnEvent(0, 60, 40));
            track.AddEvent(1, new NoteOffEvent(0, 60, 40));
            track.AddEvent(1, new NoteOnEvent(0, 62, 40));
            track.AddEvent(2, new NoteOffEvent(0, 62, 40));
            track.AddEvent(2, new NoteOnEvent(0, 63, 40));
            track.AddEvent(3, new NoteOffEvent(0, 63, 40));
            track.AddEvent(3, new NoteOnEvent(0, 65, 40));
            track.AddEvent(4, new NoteOffEvent(0, 65, 40));
            track.AddEvent(4, new NoteOnEvent(0, 62, 40));
            track.AddEvent(6, new NoteOffEvent(0, 62, 40));
            track.AddEvent(6, new NoteOnEvent(0, 58, 40));
            track.AddEvent(7, new NoteOffEvent(0, 58, 40));
            track.AddEvent(7, new NoteOnEvent(0, 60, 40));
            track.AddEvent(9, new NoteOffEvent(0, 60, 40));
            track.AddEvent(7, new NoteOnEvent(0, 63, 40));
            track.AddEvent(9, new NoteOffEvent(0, 63, 40));
            foreach (MidiEvent e in track.GetEvents(9))
            {
                e.AbsoluteTime += 10;
            }

            return new Sequence(new List<Track>(new Track[] { track }), new DivisionPPQN(2));
        }
    }
}
