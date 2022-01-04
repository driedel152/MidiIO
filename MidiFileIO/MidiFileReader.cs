using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MidiFileIO
{
    // Reference: https://www.personal.kent.edu/~sbirch/Music_Production/MP-II/MIDI/midi_file_format.htm

    public class MidiFileReader
    {
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
            string chunkType = ReadRawToAsciiString(4);
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
                int deltaTime = ReadVariableLengthRawToInt();
                MidiEvent midiEvent = ReadMidiEvent();
                midiEvent.deltaTime = deltaTime;
                midiEvents.Add(midiEvent);
            }

            if(index != endIndex)
            {
                Console.WriteLine("Not sure if that should happen...");
            }

            return new Track(midiEvents.ToArray());
        }

        private MidiEvent ReadMidiEvent()
        {
            byte statusByte = raw[index++];
            int length;
            switch (statusByte)
            {
                // Sysex Events
                case 0xF0: // TODO: differentiate between these 
                    length = ReadVariableLengthRawToInt();
                    byte[] sysexData = ReadRawToByteArr(length);
                    return new SysexEvent(false, sysexData);
                case 0xF7:
                    length = ReadVariableLengthRawToInt();
                    byte[] sysexEscapeData = ReadRawToByteArr(length);
                    return new SysexEvent(true, sysexEscapeData);
                // Meta Events
                case 0xFF:
                    byte type = raw[index++];
                    length = ReadVariableLengthRawToInt();
                    switch (type)
                    {
                        case 0x00:
                            Debug.Assert(length == 2);
                            int sequenceNumber = ReadRawToInt(2);
                            return new SequenceNumberEvent(sequenceNumber);
                        case 0x01:
                            byte[] text = ReadRawToByteArr(length); // Can be not ASCII
                            return new TextEvent(text);
                        case 0x02:
                            string copyright = ReadRawToAsciiString(length);
                            return new CopyrightNoticeEvent(copyright);
                        case 0x03:
                            string trackName = ReadRawToAsciiString(length);
                            return new TrackNameEvent(trackName);
                        case 0x04:
                            string instrumentName = ReadRawToAsciiString(length);
                            return new InstrumentNameEvent(instrumentName);
                        case 0x05:
                            string lyric = ReadRawToAsciiString(length);
                            return new LyricEvent(lyric);
                        case 0x06:
                            string marker = ReadRawToAsciiString(length);
                            return new MarkerEvent(marker);
                        case 0x07:
                            string cuePoint = ReadRawToAsciiString(length);
                            return new CuePointEvent(cuePoint);
                        case 0x20:
                            Debug.Assert(length == 1);
                            int channelPrefix = ReadRawToInt(1);
                            return new MidiChannelPrefixEvent(channelPrefix);
                        case 0x2F:
                            Debug.Assert(length == 0);
                            return new EndOfTrackEvent();
                        case 0x51:
                            Debug.Assert(length == 3);
                            int tempo = ReadRawToInt(3);
                            return new SetTempoEvent(tempo);
                        case 0x54:
                            Debug.Assert(length == 5);
                            int hours = ReadRawToInt(1);
                            int minutes = ReadRawToInt(1);
                            int seconds = ReadRawToInt(1);
                            int frames = ReadRawToInt(1);
                            int fractionalFrame = ReadRawToInt(1);
                            return new SmtpeOffsetEvent(hours, minutes, seconds, frames, fractionalFrame);
                        case 0x58:
                            Debug.Assert(length == 4);
                            int numerator = ReadRawToInt(1);
                            int denominator = ReadRawToInt(1);
                            int clocksPerMetronomeTick = ReadRawToInt(1);
                            int thirtySecondNotesPerTwentyFourClocks = ReadRawToInt(1); // 8 is standard
                            return new TimeSignatureEvent(numerator, denominator, clocksPerMetronomeTick, thirtySecondNotesPerTwentyFourClocks);
                        case 0x59:
                            Debug.Assert(length == 2);
                            int sharpsFlats = ReadRawToInt(1);
                            int majorMinor = ReadRawToInt(1); // TODO: make these enums
                            return new KeySignatureEvent(sharpsFlats, majorMinor);
                        case 0x7F:
                            byte[] sequencerData = ReadRawToByteArr(length); // TODO: read Manufacturer's ID
                            return new SequencerSpecificEvent(sequencerData);
                        default:
                            byte[] data = ReadRawToByteArr(length);
                            return new UnknownMetaEvent(data);

                    }
                // Channel Events
                default:
                    int statusNibble = statusByte >> 4;
                    int channel = statusByte & 0x0F;
                    switch (statusNibble)
                    {
                        // Mode Events
                        case 0xB:
                            switch (raw[index])
                            {
                                case 0x78:
                                    Debug.Assert(raw[index + 1] == 0x00);
                                    index += 2;
                                    return new AllSoundOffEvent(channel);
                                case 0x79:
                                    Debug.Assert(raw[index + 1] == 0x00);
                                    index += 2;
                                    return new ResetAllControllersEvent(channel);
                                case 0x7A:
                                    Debug.Assert(raw[index + 1] == 0x00 || raw[index + 1] == 0x7F);
                                    index += 2;
                                    bool connect = raw[index + 1] == 0x7F;
                                    return new LocalControlEvent(channel, connect);
                                case 0x7B:
                                    Debug.Assert(raw[index + 1] == 0x00);
                                    index += 2;
                                    return new AllNotesOffEvent(channel);
                                case 0x7C:
                                    Debug.Assert(raw[index + 1] == 0x00);
                                    index += 2;
                                    return new OmniModeOffEvent(channel);
                                case 0x7D:
                                    Debug.Assert(raw[index + 1] == 0x00);
                                    index += 2;
                                    return new OmniModeOnEvent(channel);
                                case 0x7E:
                                    index++;
                                    int numChannels = ReadRawToInt(1);
                                    return new MonoModeOnEvent(channel, numChannels);
                                case 0x7F:
                                    Debug.Assert(raw[index + 1] == 0x00);
                                    index += 2;
                                    return new PolyModeOnEvent(channel);
                        // Voice Events
                                default:
                                    int controllerNumber = ReadRawToInt(1);
                                    int controllerValue = ReadRawToInt(1);
                                    return new ControllerChangeEvent(channel, controllerNumber, controllerValue);
                            }
                        case 0x8:
                            int keyOff = ReadRawToInt(1);
                            int velocityOff = ReadRawToInt(1);
                            return new NoteOffEvent(channel, keyOff, velocityOff);
                        case 0x9:
                            int keyOn = ReadRawToInt(1);
                            int velocityOn = ReadRawToInt(1);
                            return new NoteOnEvent(channel, keyOn, velocityOn);
                        case 0xA:
                            int polyKey = ReadRawToInt(1);
                            int keyPressure = ReadRawToInt(1);
                            return new PolyphonicKeyPressureEvent(channel, polyKey, keyPressure);
                        case 0xC:
                            ProgramName programName = (ProgramName)ReadRawToInt(1);
                            return new ProgramChangeEvent(channel, programName);
                        case 0xD:
                            int channelPressure = ReadRawToInt(1);
                            return new ChannelKeyPressureEvent(channel, channelPressure);
                        case 0xE:
                            int lsb = ReadRawToInt(1);
                            int msb = ReadRawToInt(1);
                            return new PitchBendEvent(channel, lsb, msb);
                        default:
                            throw new InvalidDataException();
                    }
            }
        }

        private MidiFileHeader ReadHeaderChunk()
        {
            string chunkType = ReadRawToAsciiString(4);
            if (chunkType != "MThd")
            {
                throw new InvalidDataException("Invalid header");
            }
            int length = ReadChunkLength();
            return ReadHeaderData(length);
        }

        private string ReadRawToAsciiString(int length)
        {
            string chunkType = System.Text.Encoding.ASCII.GetString(raw, index, length);
            index += length;
            return chunkType;
        }

        private int ReadChunkLength()
        {
            int length = ReadRawToInt(4);
            return length;
        }

        private MidiFileHeader ReadHeaderData(int length)
        {
            MidiFileFormat format = (MidiFileFormat)ReadRawToInt(2);
            length -= 2;

            int trackCount = ReadRawToInt(2);
            length -= 2;

            int divisionRaw = ReadRawToInt(2);
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

            byte[] ignoredData = ReadRawToByteArr(length);
            return new MidiFileHeader(format, trackCount, division, ignoredData);
        }

        private byte[] ReadRawToByteArr(int length)
        {
            byte[] data = new byte[length];
            Array.Copy(raw, index, data, 0, length);
            index += length;
            return data;
        }

        private int ReadRawToInt(int length)
        {
            int value = 0;
            while(length > 0)
            {
                value |= raw[index++] << (--length * 8);
            }

            return value;
        }

        private int ReadVariableLengthRawToInt()
        {
            int value = 0;
            bool lastByte = false;
            while (!lastByte)
            {
                value <<= 7;
                value |= raw[index] & 0x7F;
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
