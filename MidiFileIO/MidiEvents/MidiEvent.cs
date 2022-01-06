using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MidiFileIO
{
    public abstract class MidiEvent 
    {
        public int deltaTime = 0;
        
        public abstract IEnumerable<byte> ToBytes();

        public static MidiEvent Parse(byte[] raw, ref int index, ref byte runningStatus)
        {
            byte statusByte = raw[index] < 0x80 ? runningStatus : raw[index++];
            runningStatus = statusByte;

            int length;
            switch (statusByte)
            {
                // Sysex Events TODO: Add System Common Messages
                case 0xF0:
                    length = BinaryUtils.ReadVariableLengthRawToInt(raw, ref index);
                    byte[] sysexData = BinaryUtils.ReadRawToByteArr(raw, ref index, length);
                    return new SysexEvent(false, sysexData);
                case 0xF7:
                    length = BinaryUtils.ReadVariableLengthRawToInt(raw, ref index);
                    byte[] sysexEscapeData = BinaryUtils.ReadRawToByteArr(raw, ref index, length);
                    return new SysexEvent(true, sysexEscapeData);
                // Meta Events
                case 0xFF:
                    byte type = raw[index++];
                    length = BinaryUtils.ReadVariableLengthRawToInt(raw, ref index);
                    switch (type)
                    {
                        case 0x00:
                            Debug.Assert(length == 2);
                            int sequenceNumber = BinaryUtils.ReadRawToInt(raw, ref index, 2);
                            return new SequenceNumberEvent(sequenceNumber);
                        case 0x01:
                            byte[] text = BinaryUtils.ReadRawToByteArr(raw, ref index, length); // Can be not ASCII
                            return new TextEvent(text);
                        case 0x02:
                            string copyright = BinaryUtils.ReadRawToAsciiString(raw, ref index, length);
                            return new CopyrightNoticeEvent(copyright);
                        case 0x03:
                            string trackName = BinaryUtils.ReadRawToAsciiString(raw, ref index, length);
                            return new TrackNameEvent(trackName);
                        case 0x04:
                            string instrumentName = BinaryUtils.ReadRawToAsciiString(raw, ref index, length);
                            return new InstrumentNameEvent(instrumentName);
                        case 0x05:
                            string lyric = BinaryUtils.ReadRawToAsciiString(raw, ref index, length);
                            return new LyricEvent(lyric);
                        case 0x06:
                            string marker = BinaryUtils.ReadRawToAsciiString(raw, ref index, length);
                            return new MarkerEvent(marker);
                        case 0x07:
                            string cuePoint = BinaryUtils.ReadRawToAsciiString(raw, ref index, length);
                            return new CuePointEvent(cuePoint);
                        case 0x20:
                            Debug.Assert(length == 1);
                            int channelPrefix = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            return new MidiChannelPrefixEvent(channelPrefix);
                        case 0x2F:
                            Debug.Assert(length == 0);
                            return new EndOfTrackEvent();
                        case 0x51:
                            Debug.Assert(length == 3);
                            int tempo = BinaryUtils.ReadRawToInt(raw, ref index, 3);
                            return new SetTempoEvent(tempo);
                        case 0x54:
                            Debug.Assert(length == 5);
                            int hours = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            int minutes = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            int seconds = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            int frames = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            int fractionalFrame = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            return new SmtpeOffsetEvent(hours, minutes, seconds, frames, fractionalFrame);
                        case 0x58:
                            Debug.Assert(length == 4);
                            int numerator = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            int denominator = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            int clocksPerMetronomeTick = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            int thirtySecondNotesPerTwentyFourClocks = BinaryUtils.ReadRawToInt(raw, ref index, 1); // 8 is standard
                            return new TimeSignatureEvent(numerator, denominator, clocksPerMetronomeTick, thirtySecondNotesPerTwentyFourClocks);
                        case 0x59:
                            Debug.Assert(length == 2);
                            int sharpsFlats = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            int majorMinor = BinaryUtils.ReadRawToInt(raw, ref index, 1); // TODO: make these enums
                            return new KeySignatureEvent(sharpsFlats, majorMinor);
                        case 0x7F:
                            byte[] sequencerData = BinaryUtils.ReadRawToByteArr(raw, ref index, length); // TODO: read Manufacturer's ID
                            return new SequencerSpecificEvent(sequencerData);
                        default:
                            throw new InvalidDataException();

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
                                    int numChannels = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                                    return new MonoModeOnEvent(channel, numChannels);
                                case 0x7F:
                                    Debug.Assert(raw[index + 1] == 0x00);
                                    index += 2;
                                    return new PolyModeOnEvent(channel);
                                // Voice Events
                                default:
                                    int controllerNumber = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                                    int controllerValue = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                                    return new ControllerChangeEvent(channel, controllerNumber, controllerValue);
                            }
                        case 0x8:
                            int keyOff = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            int velocityOff = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            return new NoteOffEvent(channel, keyOff, velocityOff);
                        case 0x9:
                            int keyOn = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            int velocityOn = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            return new NoteOnEvent(channel, keyOn, velocityOn);
                        case 0xA:
                            int polyKey = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            int keyPressure = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            return new PolyphonicKeyPressureEvent(channel, polyKey, keyPressure);
                        case 0xC:
                            ProgramName programName = (ProgramName)BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            return new ProgramChangeEvent(channel, programName);
                        case 0xD:
                            int channelPressure = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            return new ChannelKeyPressureEvent(channel, channelPressure);
                        case 0xE:
                            int lsb = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            int msb = BinaryUtils.ReadRawToInt(raw, ref index, 1);
                            return new PitchBendEvent(channel, lsb, msb);
                        default:
                            throw new InvalidDataException();
                    }
            }
        }
    }
}