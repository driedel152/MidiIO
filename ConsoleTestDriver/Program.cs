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
            foreach (MidiEvent e in midiFile.tracks[0].events)
            {
                if(e is ProgramChangeEvent)
                {
                    ProgramChangeEvent programChange = e as ProgramChangeEvent;
                    Console.WriteLine("Instrument: " + programChange.programName);
                    foreach(MidiEvent e0 in midiFile.tracks[0].events)
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
            }
        }
    }
}
