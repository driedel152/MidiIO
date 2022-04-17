using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MidiIO
{
    // Reference: https://www.personal.kent.edu/~sbirch/Music_Production/MP-II/MIDI/midi_file_format.htm

    public class MidiFileReader
    {
        public MidiFormat fileFormat;

        readonly byte[] raw;
        int index;
        byte runningStatus;
        int trackCount;
        int absoluteTime;

        /// <summary>
        /// Initializes a new MidiFileReader for the file at the given path.
        /// </summary>
        /// <param name="path"></param>
        public MidiFileReader(string path)
        {
            raw = File.ReadAllBytes(path);
        }

        public Sequence ReadMidiFile()
        {
            index = 0;
            MidiHeader header = ReadHeaderChunk();
            fileFormat = header.format;

            Sequence sequence = new Sequence(header.division);
            for(int i=0; i<trackCount; i++)
            {
                sequence.AddTrack(ReadTrackChunk());
            }
            Debug.Assert(index == raw.Length);

            return sequence;
        }

        private Track ReadTrackChunk()
        {
            absoluteTime = 0;
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
            Track track = new Track();

            bool endsWithEndOfTrack = false;
            int endIndex = index + length;
            while(index < endIndex)
            {
                int deltaTime = BinaryUtils.ReadVariableLengthRawToInt(raw, ref index);
                absoluteTime += deltaTime;
                MidiEvent midiEvent = ReadMidiEvent();
                track.AddEventAbsolute(midiEvent, absoluteTime);
                endsWithEndOfTrack = midiEvent is EndOfTrackEvent;
            }

            Debug.Assert(endsWithEndOfTrack);
            Debug.Assert(index == endIndex);

            return track;
        }

        private MidiEvent ReadMidiEvent()
        {
            MidiEvent midiEvent = MidiEvent.Parse(raw, ref index, ref runningStatus);
            return midiEvent;
        }

        private MidiHeader ReadHeaderChunk()
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

        private MidiHeader ReadHeaderData(int length)
        {
            MidiFormat format = (MidiFormat)BinaryUtils.ReadRawToInt(raw, ref index, 2);
            length -= 2;

            trackCount = BinaryUtils.ReadRawToInt(raw, ref index, 2);
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
            return new MidiHeader(format, division, ignoredData);
        }
    }
}
