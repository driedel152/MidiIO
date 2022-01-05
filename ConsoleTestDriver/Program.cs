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
                // Print instruments
                if (e is ProgramChangeEvent)
                {
                    ProgramChangeEvent programChange = e as ProgramChangeEvent;
                    Console.WriteLine($"Instrument {programChange.channel}: {programChange.programName}");
                    foreach (MidiEvent e0 in midiFile.tracks[0].events)
                    {
                        if (e0 is NoteOnEvent)
                        {
                            NoteOnEvent noteOn = e0 as NoteOnEvent;
                            if (noteOn.channel == programChange.channel)
                            {
                                Console.Write(noteOn.keyOn + " ");
                            }
                        }
                    }
                    Console.WriteLine();
                }

                //// Raise channel 9
                //if (e is NoteOnEvent)
                //{
                //    NoteOnEvent noteOnEvent = e as NoteOnEvent;
                //    if (noteOnEvent.channel == 9)
                //        noteOnEvent.keyOn++;
                //}
                //if (e is NoteOffEvent)
                //{
                //    NoteOffEvent noteOffEvent = e as NoteOffEvent;
                //    if (noteOffEvent.channel == 9)
                //        noteOffEvent.keyOff++;
                //}
            }

            MidiFileWriter writer = new MidiFileWriter(midiFile, "MyOtherMidi.mid");
            writer.WriteMidiFile();
        }
    }
}
