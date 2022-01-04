using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiFileIO
{
    public class MidiFileWriter
    {
        MidiFile midiFile;
        FileStream file;

        public MidiFileWriter(MidiFile midiFile, string path)
        {
            this.midiFile = midiFile;
            file = new FileStream(path, FileMode.Create);
        }

        public void WriteMidiFile()
        {
            WriteHeaderChunk(midiFile);

            for (int i = 0; i < midiFile.tracks.Length; i++)
            {
                WriteTrackChunk(midiFile.tracks[i]);
            }
            file.Close();
        }

        private void WriteHeaderChunk(MidiFile midiFile)
        {
            // Chunk Type
            file.Write(Encoding.ASCII.GetBytes("MThd"));
            // Length
            file.Write(IntToByteArr(6, 4));
            // Data
            file.Write(IntToByteArr((int)midiFile.header.format, 2));
            file.Write(IntToByteArr(midiFile.tracks.Length, 2));
            byte[] divisionBytes;
            if(midiFile.header.division is DivisionPPQN) // TODO: Enforce maximum values
            {
                DivisionPPQN division = (DivisionPPQN)midiFile.header.division;
                divisionBytes = IntToByteArr(division.pulsesPerQuarterNote, 2);
                divisionBytes[0] &= 0x7F;
            }
            else
            {
                DivisionFrameBased division = (DivisionFrameBased)midiFile.header.division;
                divisionBytes = new byte[2] { (byte)(0x80 | division.framesPerSecond), (byte)division.deltaTimePerFrame };
            }
            file.Write(divisionBytes);
        }

        private void WriteTrackChunk(Track track)
        {
            file.Write(Encoding.ASCII.GetBytes("MTrk"));

            byte[] trackData = TrackToByteArr(track);
            file.Write(IntToByteArr(trackData.Length, 4));
            file.Write(trackData);
        }

        private byte[] TrackToByteArr(Track track)
        {
            List<byte> data = new List<byte>();
            foreach(MidiEvent e in track.events)
            {
                data.AddRange(MidiEventToByteArr(e));
            }
            return data.ToArray();
        }

        private List<byte> MidiEventToByteArr(MidiEvent e)
        {
            List<byte> data = new List<byte>();
            data.AddRange(IntToVariableByteArr(e.deltaTime));
            if (e is SysexEvent)
            {
                SysexEvent sysexEvent = (SysexEvent)e;
                data.Add((byte)(sysexEvent.isEscape ? 0xF7 : 0xF0));
                data.AddRange(IntToVariableByteArr(sysexEvent.data.Length));
                data.AddRange(sysexEvent.data);
            }
            else if (e is MetaEvent)
            {
                MetaEvent metaEvent = (MetaEvent)e;
                data.Add(0xFF);
                if (metaEvent is SequenceNumberEvent)
                {
                    SequenceNumberEvent sequenceNumberEvent = (SequenceNumberEvent)metaEvent;
                    data.Add(0x00);
                    data.Add(0x02);
                    data.AddRange(IntToByteArr(sequenceNumberEvent.sequenceNumber, 2));
                }
                else if(metaEvent is TextEvent)
                {
                    TextEvent textEvent = (TextEvent)metaEvent;
                    data.Add(0x01);
                    data.AddRange(IntToVariableByteArr(textEvent.text.Length));
                    data.AddRange(textEvent.text);
                }
                else if(metaEvent is CopyrightNoticeEvent)
                {
                    CopyrightNoticeEvent copyrightNoticeEvent = (CopyrightNoticeEvent)metaEvent;
                    data.Add(0x02);
                    data.AddRange(IntToVariableByteArr(copyrightNoticeEvent.text.Length));
                    data.AddRange(Encoding.ASCII.GetBytes(copyrightNoticeEvent.text));
                }
                else if(metaEvent is TrackNameEvent)
                {
                    TrackNameEvent trackNameEvent = (TrackNameEvent)metaEvent;
                    data.Add(0x03);
                    data.AddRange(IntToVariableByteArr(trackNameEvent.trackName.Length));
                    data.AddRange(Encoding.ASCII.GetBytes(trackNameEvent.trackName));
                }
                else if(metaEvent is InstrumentNameEvent)
                {
                    InstrumentNameEvent instrumentNameEvent = (InstrumentNameEvent)metaEvent;
                    data.Add(0x04);
                    data.AddRange(IntToVariableByteArr(instrumentNameEvent.instrumentName.Length));
                    data.AddRange(Encoding.ASCII.GetBytes(instrumentNameEvent.instrumentName));
                }
                else if(metaEvent is LyricEvent)
                {
                    LyricEvent lyricEvent = (LyricEvent)metaEvent;
                    data.Add(0x05);
                    data.AddRange(IntToVariableByteArr(lyricEvent.lyric.Length));
                    data.AddRange(Encoding.ASCII.GetBytes(lyricEvent.lyric));
                }
                else if (metaEvent is MarkerEvent)
                {
                    MarkerEvent markerEvent = (MarkerEvent)metaEvent;
                    data.Add(0x06);
                    data.AddRange(IntToVariableByteArr(markerEvent.marker.Length));
                    data.AddRange(Encoding.ASCII.GetBytes(markerEvent.marker));
                }
                else if (metaEvent is CuePointEvent)
                {
                    CuePointEvent cuePointEvent = (CuePointEvent)metaEvent;
                    data.Add(0x07);
                    data.AddRange(IntToVariableByteArr(cuePointEvent.cuePoint.Length));
                    data.AddRange(Encoding.ASCII.GetBytes(cuePointEvent.cuePoint));
                }
                else if (metaEvent is MidiChannelPrefixEvent)
                {
                    MidiChannelPrefixEvent midiChannelPrefixEvent = (MidiChannelPrefixEvent)metaEvent;
                    data.Add(0x20);
                    data.Add(0x01);
                    data.AddRange(IntToByteArr(midiChannelPrefixEvent.channel, 1));
                }
                else if (metaEvent is EndOfTrackEvent)
                {
                    data.Add(0x2F);
                    data.Add(0x00);
                }
                else if (metaEvent is SetTempoEvent)
                {
                    SetTempoEvent setTempoEvent = (SetTempoEvent)metaEvent;
                    data.Add(0x51);
                    data.Add(0x03);
                    data.AddRange(IntToByteArr(setTempoEvent.tempo, 3));
                }
                else if (metaEvent is SmtpeOffsetEvent)
                {
                    SmtpeOffsetEvent smtpeOffsetEvent = (SmtpeOffsetEvent)metaEvent;
                    data.Add(0x54);
                    data.Add(0x05);
                    data.AddRange(IntToByteArr(smtpeOffsetEvent.hours, 1));
                    data.AddRange(IntToByteArr(smtpeOffsetEvent.minutes, 1));
                    data.AddRange(IntToByteArr(smtpeOffsetEvent.seconds, 1));
                    data.AddRange(IntToByteArr(smtpeOffsetEvent.frames, 1));
                    data.AddRange(IntToByteArr(smtpeOffsetEvent.fractionalFrame, 1));
                }
                else if (metaEvent is TimeSignatureEvent)
                {
                    TimeSignatureEvent timeSignatureEvent = (TimeSignatureEvent)metaEvent;
                    data.Add(0x58);
                    data.Add(0x04);
                    data.AddRange(IntToByteArr(timeSignatureEvent.numerator, 1));
                    data.AddRange(IntToByteArr(timeSignatureEvent.denominator, 1));
                    data.AddRange(IntToByteArr(timeSignatureEvent.clocksPerMetronomeTick, 1));
                    data.AddRange(IntToByteArr(timeSignatureEvent.thirtySecondNotesPerTwentyFourClocks, 1));
                }
                else if (metaEvent is KeySignatureEvent)
                {
                    KeySignatureEvent keySignatureEvent = (KeySignatureEvent)metaEvent;
                    data.Add(0x59);
                    data.Add(0x02);
                    data.AddRange(IntToByteArr(keySignatureEvent.sharpsFlats, 1));
                    data.AddRange(IntToByteArr(keySignatureEvent.majorMinor, 1));
                }
                else if (metaEvent is SequencerSpecificEvent)
                {
                    SequencerSpecificEvent sequencerSpecificEvent = (SequencerSpecificEvent)metaEvent;
                    data.Add(0x7F);
                    data.AddRange(IntToVariableByteArr(sequencerSpecificEvent.sequencerData.Length));
                    data.AddRange(sequencerSpecificEvent.sequencerData);
                }
            }
            else if(e is ChannelEvent)
            {
                ChannelEvent channelEvent = (ChannelEvent)e;
                if(channelEvent is ChannelModeEvent)
                {
                    ChannelModeEvent channelModeEvent = (ChannelModeEvent)channelEvent;
                    data.Add((byte)(0xB0 | channelModeEvent.channel));
                    if(channelModeEvent is AllSoundOffEvent)
                    {
                        data.Add(0x78);
                        data.Add(0x00);
                    }
                    else if (channelModeEvent is ResetAllControllersEvent)
                    {
                        data.Add(0x79);
                        data.Add(0x00);
                    }
                    else if (channelModeEvent is LocalControlEvent)
                    {
                        LocalControlEvent localControlEvent = (LocalControlEvent)channelModeEvent;
                        data.Add(0x7A);
                        data.Add((byte)(localControlEvent.connect ? 0x7F : 0x00));
                    }
                    else if (channelModeEvent is AllNotesOffEvent)
                    {
                        data.Add(0x7B);
                        data.Add(0x00);
                    }
                    else if (channelModeEvent is OmniModeOffEvent)
                    {
                        data.Add(0x7C);
                        data.Add(0x00);
                    }
                    else if (channelModeEvent is OmniModeOnEvent)
                    {
                        data.Add(0x7D);
                        data.Add(0x00);
                    }
                    else if (channelModeEvent is MonoModeOnEvent)
                    {
                        MonoModeOnEvent monoModeOnEvent = (MonoModeOnEvent)channelModeEvent;
                        data.Add(0x7E);
                        data.AddRange(IntToByteArr(monoModeOnEvent.numChannels, 1));
                    }
                    else if (channelModeEvent is PolyModeOnEvent)
                    {
                        data.Add(0x7F);
                        data.Add(0x00);
                    }
                }
                else if(channelEvent is ChannelVoiceEvent)
                {
                    ChannelVoiceEvent channelVoiceEvent = (ChannelVoiceEvent)channelEvent;
                    if(channelVoiceEvent is NoteOffEvent)
                    {
                        NoteOffEvent noteOffEvent = (NoteOffEvent)channelVoiceEvent;
                        data.Add((byte)(0x80 | noteOffEvent.channel));
                        data.AddRange(IntToByteArr(noteOffEvent.keyOff, 1));
                        data.AddRange(IntToByteArr(noteOffEvent.velocityOff, 1));
                    }
                    else if (channelVoiceEvent is NoteOnEvent)
                    {
                        NoteOnEvent noteOnEvent = (NoteOnEvent)channelVoiceEvent;
                        data.Add((byte)(0x90 | noteOnEvent.channel));
                        data.AddRange(IntToByteArr(noteOnEvent.keyOn, 1));
                        data.AddRange(IntToByteArr(noteOnEvent.velocityOn, 1));
                    }
                    else if (channelVoiceEvent is PolyphonicKeyPressureEvent)
                    {
                        PolyphonicKeyPressureEvent polyphonicKeyPressureEvent = (PolyphonicKeyPressureEvent)channelVoiceEvent;
                        data.Add((byte)(0xA0 | polyphonicKeyPressureEvent.channel));
                        data.AddRange(IntToByteArr(polyphonicKeyPressureEvent.polyKey, 1));
                        data.AddRange(IntToByteArr(polyphonicKeyPressureEvent.keyPressure, 1));
                    }
                    else if (channelVoiceEvent is ControllerChangeEvent)
                    {
                        ControllerChangeEvent controllerChangeEvent = (ControllerChangeEvent)channelVoiceEvent;
                        data.Add((byte)(0xB0 | controllerChangeEvent.channel));
                        data.AddRange(IntToByteArr(controllerChangeEvent.controllerNumber, 1));
                        data.AddRange(IntToByteArr(controllerChangeEvent.controllerValue, 1));
                    }
                    else if (channelVoiceEvent is ProgramChangeEvent)
                    {
                        ProgramChangeEvent programChangeEvent = (ProgramChangeEvent)channelVoiceEvent;
                        data.Add((byte)(0xC0 | programChangeEvent.channel));
                        data.AddRange(IntToByteArr((int)programChangeEvent.programName, 1));
                    }
                    else if (channelVoiceEvent is ChannelKeyPressureEvent)
                    {
                        ChannelKeyPressureEvent channelKeyPressureEvent = (ChannelKeyPressureEvent)channelVoiceEvent;
                        data.Add((byte)(0xD0 | channelKeyPressureEvent.channel));
                        data.AddRange(IntToByteArr(channelKeyPressureEvent.channelPressure, 1));
                    }
                    else if (channelVoiceEvent is PitchBendEvent)
                    {
                        PitchBendEvent pitchBendEvent = (PitchBendEvent)channelVoiceEvent;
                        data.Add((byte)(0xE0 | pitchBendEvent.channel));
                        data.AddRange(IntToByteArr(pitchBendEvent.lsb, 1));
                        data.AddRange(IntToByteArr(pitchBendEvent.msb, 1));
                    }
                }
            }
            return data;
        }

        private IEnumerable<byte> IntToVariableByteArr(int integer)
        {
            if (integer > Math.Pow(2, 28)) throw new OverflowException("Integer value too large");

            if (integer >= Math.Pow(2, 21))
            {
                return new byte[4]
                {
                    (byte)(0x80 | (integer >> 21)),
                    (byte)(0x80 | (integer >> 14)),
                    (byte)(0x80 | (integer >> 7)),
                    (byte)(0x7F & integer)
                };
            }
            else if (integer >= Math.Pow(2, 14))
            {
                return new byte[3]
                {
                    (byte)(0x80 | (integer >> 14)),
                    (byte)(0x80 | (integer >> 7)),
                    (byte)(0x7F & integer)
                };
            }
            else if (integer >= Math.Pow(2, 7))
            {
                return new byte[2]
                {
                    (byte)(0x80 | (integer >> 7)),
                    (byte)(0x7F & integer)
                };
            }
            else
            {
                return new byte[1]
                {
                    (byte)(0x7F & integer)
                };
            }
        }

        private byte[] IntToByteArr(int value, int length)
        {
            byte[] bytes = new byte[length];
            int index = 0;
            int shiftIndex = length;
            while (index < length)
            {
                bytes[index] = (byte)(value >> (--shiftIndex * 8));
                index++;
            }
            return bytes;
        }
    }
}
