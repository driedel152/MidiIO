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
            MidiFileReader reader = new MidiFileReader("ABBA_-_Dancing_Queen.mid");
            Sequence sequence = reader.ReadMidiFile();

            Console.WriteLine("Format: " + sequence.header.format);
            Console.WriteLine("Division: " + ((DivisionPPQN)sequence.header.division).pulsesPerQuarterNote);
            Console.WriteLine("Track count: " + sequence.tracks.Count);
            foreach (Track t in sequence.tracks)
            {
                foreach(MidiEvent e in t.events)
                {
                    if(e is TimeSignatureEvent)
                    {
                        TimeSignatureEvent timeSignatureEvent = (TimeSignatureEvent)e;
                        Console.WriteLine($"Time Signature: {timeSignatureEvent.numerator}/{Math.Pow(2, timeSignatureEvent.denominator)}, " +
                            $"{timeSignatureEvent.clocksPerMetronomeTick} clocks per tick, " +
                            $"{timeSignatureEvent.thirtySecondNotesPerTwentyFourClocks} 32nd/24-clocks");
                    }
                    if(e is UnknownMetaEvent)
                    {
                        Console.WriteLine("Unknown MetaEvent of type " + ((UnknownMetaEvent)e).type);
                    }
                }
            }

            MidiFileWriter writer = new MidiFileWriter(sequence, "MyOtherMidi.mid");
            writer.WriteMidiFile(true);

            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
        }

        public static Sequence CreateSequence()
        {
            MidiEvent e0 = new SetTempoEvent(500000);
            e0.absoluteTime = 0;
            MidiEvent e1 = new NoteOnEvent(0, 60, 40);
            e1.absoluteTime = 0;
            MidiEvent e2 = new NoteOffEvent(0, 60, 40);
            e2.absoluteTime = 192;
            MidiEvent e3 = new NoteOnEvent(0, 62, 40);
            e3.absoluteTime = 192;
            MidiEvent e4 = new NoteOffEvent(0, 62, 40);
            e4.absoluteTime = 192*2;
            MidiEvent[] events = new MidiEvent[] { e0, e1, e2, e3, e4 };
            Track track = new Track(new List<MidiEvent>(events));

            MidiHeader header = new MidiHeader(MidiFormat.SingleTrack, new DivisionPPQN(96));

            return new Sequence(header, new List<Track>(new Track[] { track }));
        }
    }
}
