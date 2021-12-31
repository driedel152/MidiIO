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
                if(e is NoteOnEvent)
                {
                    NoteOnEvent noteOn = e as NoteOnEvent;
                    if(noteOn.channel == 1)
                    {
                        Console.WriteLine(noteOn.keyOn);
                    }
                }
            }
        }
    }
}
