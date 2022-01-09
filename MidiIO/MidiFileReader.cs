using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MidiIO
{
    // Reference: https://www.personal.kent.edu/~sbirch/Music_Production/MP-II/MIDI/midi_file_format.htm

    public class MidiFileReader
    {
        readonly byte[] raw;
        int index;
        byte runningStatus;

        public MidiFileReader(string path)
        {
            raw = File.ReadAllBytes(path);
            index = 0;
        }

        public MidiFile ReadMidiFile()
        {
            MidiFileHeader header = ReadHeaderChunk();

            Track[] tracks = new Track[header.trackCount];
            for(int i=0; i<header.trackCount; i++)
            {
                tracks[i] = ReadTrackChunk();
            }
            return new MidiFile(header, tracks);
        }

        private Track ReadTrackChunk()
        {
            string chunkType = BinaryUtils.ReadRawToAsciiString(raw, ref index, 4);
            if (chunkType != "MTrk")
            {
                throw new InvalidDataException("Invalid track");
            }
            int length = ReadChunkLength();
            return ReadTrackData(length);
        }

        private Track ReadTrackData(int length)
        {
            List<MidiEvent> midiEvents = new List<MidiEvent>();

            int endIndex = index + length;
            while(index < endIndex)
            {
                int deltaTime = BinaryUtils.ReadVariableLengthRawToInt(raw, ref index);
                MidiEvent midiEvent = ReadMidiEvent();
                midiEvent.deltaTime = deltaTime;
                midiEvents.Add(midiEvent);
            }

            Debug.Assert(midiEvents[midiEvents.Count - 1] is EndOfTrackEvent);
            Debug.Assert(index == endIndex);

            return new Track(midiEvents);
        }

        private MidiEvent ReadMidiEvent()
        {
            MidiEvent midiEvent = MidiEvent.Parse(raw, ref index, ref runningStatus);
            return midiEvent;
        }

        private MidiFileHeader ReadHeaderChunk()
        {
            string chunkType = BinaryUtils.ReadRawToAsciiString(raw, ref index, 4);
            if (chunkType != "MThd")
            {
                throw new InvalidDataException("Invalid header");
            }
            int length = ReadChunkLength();
            return ReadHeaderData(length);
        }

        private int ReadChunkLength()
        {
            int length = BinaryUtils.ReadRawToInt(raw, ref index, 4);
            return length;
        }

        private MidiFileHeader ReadHeaderData(int length)
        {
            MidiFileFormat format = (MidiFileFormat)BinaryUtils.ReadRawToInt(raw, ref index, 2);
            length -= 2;

            int trackCount = BinaryUtils.ReadRawToInt(raw, ref index, 2);
            length -= 2;

            int divisionRaw = BinaryUtils.ReadRawToInt(raw, ref index, 2);
            length -= 2;
            Division division;
            if (divisionRaw >> 15 == 0)
            {
                int ticksPerQuarterNote = divisionRaw & 0xEFFF;
                division = new DivisionPPQN(ticksPerQuarterNote);
            }
            else
            {
                int framesPerSecond = (divisionRaw & 0xEF00) >> 8;
                int ticksPerFrame = divisionRaw & 0x00FF;
                division = new DivisionFrameBased(ticksPerFrame, framesPerSecond);
            }

            byte[] ignoredData = BinaryUtils.ReadRawToByteArr(raw, ref index, length);
            return new MidiFileHeader(format, trackCount, division, ignoredData);
        }
    }
}
