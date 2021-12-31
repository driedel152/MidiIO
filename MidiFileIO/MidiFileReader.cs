using System;
using System.Collections.Generic;
using System.IO;

namespace MidiFileIO
{
    // Reference: https://www.personal.kent.edu/~sbirch/Music_Production/MP-II/MIDI/midi_file_format.htm

    public class MidiFileReader
    {
        const int CHUNK_TYPE_SIZE = 4;
        const int LENGTH_SIZE = 4;
        const int FORMAT_SIZE = 2;
        const int TRACK_COUNT_SIZE = 2;
        const int DIVISION_SIZE = 2;

        readonly byte[] raw;
        int index;

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
            string chunkType = ReadChunkType();
            if (chunkType != "MTrk")
            {
                throw new InvalidDataException("Invalid track");
            }
            int length = ReadChunkLength();
            return ReadTrackData(length);
        }

        private Track ReadTrackData(int length)
        {
            List<int> deltaTimes = new List<int>();
            List<TrackEvent> trackEvents = new List<TrackEvent>();

            int endIndex = index + length;
            while(index < endIndex)
            {
                int deltaTime = ReadVariableLengthByteArrToInt();
                TrackEvent trackEvent = ReadTrackEvent();
                deltaTimes.Add(deltaTime);
                trackEvents.Add(trackEvent);
            }

            if(index != endIndex)
            {
                Console.WriteLine("Not sure if that should happen...");
            }

            return new Track(deltaTimes.ToArray(), trackEvents.ToArray());
        }

        private TrackEvent ReadTrackEvent()
        {
            throw new NotImplementedException();
        }

        private MidiFileHeader ReadHeaderChunk()
        {
            string chunkType = ReadChunkType();
            if (chunkType != "MThd")
            {
                throw new InvalidDataException("Invalid header");
            }
            int length = ReadChunkLength();
            return ReadHeaderData(length);
        }

        private string ReadChunkType()
        {
            string chunkType = System.Text.Encoding.ASCII.GetString(raw, index, CHUNK_TYPE_SIZE);
            index += CHUNK_TYPE_SIZE;
            return chunkType;
        }

        private int ReadChunkLength()
        {
            int length = ReadByteArrToInt(LENGTH_SIZE);
            return length;
        }

        private MidiFileHeader ReadHeaderData(int length)
        {
            MidiFileFormat format = (MidiFileFormat)ReadByteArrToInt(FORMAT_SIZE);
            length -= FORMAT_SIZE;

            int trackCount = ReadByteArrToInt(TRACK_COUNT_SIZE);
            length -= TRACK_COUNT_SIZE;

            int divisionRaw = ReadByteArrToInt(DIVISION_SIZE);
            length -= DIVISION_SIZE;
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

            byte[] ignoredData = new byte[length];
            Array.Copy(raw, index, ignoredData, 0, length);
            return new MidiFileHeader(format, trackCount, division, ignoredData);
        }

        private int ReadByteArrToInt(int length)
        {
            int value = 0;
            while(length > 0)
            {
                value |= raw[index++] << (--length * 8);
            }

            return value;
        }

        private int ReadVariableLengthByteArrToInt()
        {
            int value = 0;
            bool lastByte = false;
            while (!lastByte)
            {
                value <<= 7;
                value |= raw[index] & 0xEF;
                if(raw[index] >> 7 == 0)
                {
                    lastByte = true;
                }
                index++;
            }
            return value;
        }
    }
}
