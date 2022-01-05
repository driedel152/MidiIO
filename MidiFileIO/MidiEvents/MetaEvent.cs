using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiFileIO
{
    public abstract class MetaEvent : MidiEvent { }

    public class CopyrightNoticeEvent : MetaEvent
    {
        public string text;

        public CopyrightNoticeEvent(string text)
        {
            this.text = text;
        }

        public override byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(0xFF);
            bytes.Add(0x02);
            bytes.AddRange(BinaryUtils.IntToVariableByteArr(text.Length));
            bytes.AddRange(Encoding.ASCII.GetBytes(text));
            return bytes.ToArray();
        }
    }

    public class CuePointEvent : MetaEvent
    {
        public string cuePoint;

        public CuePointEvent(string cuePoint)
        {
            this.cuePoint = cuePoint;
        }

        public override byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(0xFF);
            bytes.Add(0x07);
            bytes.AddRange(BinaryUtils.IntToVariableByteArr(cuePoint.Length));
            bytes.AddRange(Encoding.ASCII.GetBytes(cuePoint));
            return bytes.ToArray();
        }
    }

    public class EndOfTrackEvent : MetaEvent
    {
        public override byte[] ToByteArray()
        {
            return new byte[3] { 0xFF, 0x2F, 0x00 };
        }
    }

    public class InstrumentNameEvent : MetaEvent
    {
        public string instrumentName;

        public InstrumentNameEvent(string instrumentName)
        {
            this.instrumentName = instrumentName;
        }

        public override byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(0xFF);
            bytes.Add(0x04);
            bytes.AddRange(BinaryUtils.IntToVariableByteArr(instrumentName.Length));
            bytes.AddRange(Encoding.ASCII.GetBytes(instrumentName));
            return bytes.ToArray();
        }
    }

    public class KeySignatureEvent : MetaEvent
    {
        public int sharpsFlats;
        public int majorMinor;

        public KeySignatureEvent(int sharpsFlats, int majorMinor)
        {
            this.sharpsFlats = sharpsFlats;
            this.majorMinor = majorMinor;
        }

        public override byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(0xFF);
            bytes.Add(0x59);
            bytes.Add(0x02);
            bytes.AddRange(BinaryUtils.IntToByteArr(sharpsFlats, 1));
            bytes.AddRange(BinaryUtils.IntToByteArr(majorMinor, 1));
            return bytes.ToArray();
        }
    }

    public class LyricEvent : MetaEvent
    {
        public string lyric;

        public LyricEvent(string lyric)
        {
            this.lyric = lyric;
        }

        public override byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(0xFF);
            bytes.Add(0x05);
            bytes.AddRange(BinaryUtils.IntToVariableByteArr(lyric.Length));
            bytes.AddRange(Encoding.ASCII.GetBytes(lyric));
            return bytes.ToArray();
        }
    }

    public class MarkerEvent : MetaEvent
    {
        public string marker;

        public MarkerEvent(string marker)
        {
            this.marker = marker;
        }

        public override byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(0xFF);
            bytes.Add(0x06);
            bytes.AddRange(BinaryUtils.IntToVariableByteArr(marker.Length));
            bytes.AddRange(Encoding.ASCII.GetBytes(marker));
            return bytes.ToArray();
        }
    }

    public class MidiChannelPrefixEvent : MetaEvent
    {
        public int channel;

        public MidiChannelPrefixEvent(int channel)
        {
            this.channel = channel;
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { 0xFF, 0x20, 0x01, BinaryUtils.IntToByteArr(channel, 1)[0] };
        }
    }

    public class SequenceNumberEvent : MetaEvent
    {
        public int sequenceNumber;

        public SequenceNumberEvent(int sequenceNumber)
        {
            this.sequenceNumber = sequenceNumber;
        }

        public override byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(0xFF);
            bytes.Add(0x00);
            bytes.Add(0x02);
            bytes.AddRange(BinaryUtils.IntToByteArr(sequenceNumber, 2));
            return bytes.ToArray();
        }
    }

    public class SequencerSpecificEvent : MetaEvent
    {
        public byte[] sequencerData;

        public SequencerSpecificEvent(byte[] sequencerData)
        {
            this.sequencerData = sequencerData;
        }

        public override byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(0xFF);
            bytes.Add(0x7F);
            bytes.AddRange(BinaryUtils.IntToVariableByteArr(sequencerData.Length));
            bytes.AddRange(sequencerData);
            return bytes.ToArray();
        }
    }

    public class SetTempoEvent : MetaEvent
    {
        public int tempo;

        public SetTempoEvent(int tempo)
        {
            this.tempo = tempo;
        }

        public override byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(0xFF);
            bytes.Add(0x51);
            bytes.Add(0x03);
            bytes.AddRange(BinaryUtils.IntToByteArr(tempo, 3));
            return bytes.ToArray();
        }
    }

    public class SmtpeOffsetEvent : MetaEvent
    {
        public int hours;
        public int minutes;
        public int seconds;
        public int frames;
        public int fractionalFrame;

        public SmtpeOffsetEvent(int hours, int minutes, int seconds, int frames, int fractionalFrame)
        {
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.frames = frames;
            this.fractionalFrame = fractionalFrame;
        }

        public override byte[] ToByteArray()
        {
            return new byte[]
            {
                0xFF, 0x54, 0x05,
                (byte)hours,
                (byte)minutes,
                (byte)seconds,
                (byte)frames,
                (byte)fractionalFrame
            };
        }
    }

    public class TextEvent : MetaEvent
    {
        public byte[] text;

        public TextEvent(byte[] text)
        {
            this.text = text;
        }

        public override byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(0xFF);
            bytes.Add(0x01);
            bytes.AddRange(BinaryUtils.IntToVariableByteArr(text.Length));
            bytes.AddRange(text);
            return bytes.ToArray();
        }
    }

    public class TimeSignatureEvent : MetaEvent
    {
        public int numerator;
        /// <summary>
        /// Expressed as a power of 2.
        /// </summary>
        public int denominator;
        public int clocksPerMetronomeTick;
        public int thirtySecondNotesPerTwentyFourClocks;

        public TimeSignatureEvent(int numerator, int denominator, int clocksPerMetronomeTick, int thirtySecondNotesPerTwentyFourClocks)
        {
            this.numerator = numerator;
            this.denominator = denominator;
            this.clocksPerMetronomeTick = clocksPerMetronomeTick;
            this.thirtySecondNotesPerTwentyFourClocks = thirtySecondNotesPerTwentyFourClocks;
        }

        public override byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(0xFF);
            bytes.Add(0x58);
            bytes.Add(0x04);
            bytes.AddRange(BinaryUtils.IntToByteArr(numerator, 1));
            bytes.AddRange(BinaryUtils.IntToByteArr(denominator, 1));
            bytes.AddRange(BinaryUtils.IntToByteArr(clocksPerMetronomeTick, 1));
            bytes.AddRange(BinaryUtils.IntToByteArr(thirtySecondNotesPerTwentyFourClocks, 1));
            return bytes.ToArray();
        }
    }

    public class TrackNameEvent : MetaEvent
    {
        public string trackName;

        public TrackNameEvent(string trackName)
        {
            this.trackName = trackName;
        }

        public override byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(0xFF);
            bytes.Add(0x03);
            bytes.AddRange(BinaryUtils.IntToVariableByteArr(trackName.Length));
            bytes.AddRange(Encoding.ASCII.GetBytes(trackName));
            return bytes.ToArray();
        }
    }
}
