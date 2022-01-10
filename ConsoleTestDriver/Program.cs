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
            Console.WriteLine("Track count: " + sequence.Tracks.Count);
            foreach (Track t in sequence.Tracks)
            {
                foreach(MidiEvent e in t.GetEvents())
                {
                    if (e is TimeSignatureEvent)
                    {
                        TimeSignatureEvent timeSignatureEvent = (TimeSignatureEvent)e;
                        Console.WriteLine($"Time Signature: {timeSignatureEvent.numerator}/{Math.Pow(2, timeSignatureEvent.denominator)}, " +
                            $"{timeSignatureEvent.clocksPerTick} clocks per tick, " +
                            $"{timeSignatureEvent.thirtySecondsPerTick} 32nd/24-clocks");
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
            Sequence sequence = new Sequence(new DivisionPPQN(96));
            sequence.AddTrack(new Track());
            sequence.Tracks[0].AddEvent(new SetTempoEvent(500000), 0f);
            sequence.Tracks[0].AddEvent(new NoteOnEvent(0, 60, 40), 0f);
            sequence.Tracks[0].AddEvent(new NoteOffEvent(0, 60, 40), 0.5f);
            sequence.Tracks[0].AddEvent(new NoteOnEvent(0, 62, 40), 0.5f);
            sequence.Tracks[0].AddEvent(new NoteOffEvent(0, 62, 40), 1f);
            sequence.Tracks[0].AddEvent(new NoteOnEvent(0, 63, 40), 1f);
            sequence.Tracks[0].AddEvent(new NoteOffEvent(0, 63, 40), 1.5f);
            sequence.Tracks[0].AddEvent(new NoteOnEvent(0, 65, 40), 1.5f);
            sequence.Tracks[0].AddEvent(new NoteOffEvent(0, 65, 40), 2f);
            sequence.Tracks[0].AddEvent(new NoteOnEvent(0, 62, 40), 2f);
            sequence.Tracks[0].AddEvent(new NoteOffEvent(0, 62, 40), 3f);
            sequence.Tracks[0].AddEvent(new NoteOnEvent(0, 58, 40), 3f);
            sequence.Tracks[0].AddEvent(new NoteOffEvent(0, 58, 40), 3.5f);
            sequence.Tracks[0].AddEvent(new NoteOnEvent(0, 60, 40), 3.5f);
            sequence.Tracks[0].AddEvent(new NoteOffEvent(0, 60, 40), 4.5f);
            sequence.Tracks[0].AddEvent(new NoteOnEvent(0, 63, 40), 3.5f);
            sequence.Tracks[0].AddEvent(new NoteOffEvent(0, 63, 40), 4.5f);
            foreach (MidiEvent e in sequence.Tracks[0].GetEvents(4.5f))
            {
                e.AbsoluteTime += sequence.division.BeatsToAbsolute(5);
            }

            return sequence;
        }
    }
}
