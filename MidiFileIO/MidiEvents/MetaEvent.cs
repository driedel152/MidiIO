using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiIO
{
    public abstract class MetaEvent : MidiEvent { }

    public class SequenceNumberEvent : MetaEvent
    {
        public int sequenceNumber;

        public SequenceNumberEvent(int sequenceNumber)
        {
            this.sequenceNumber = sequenceNumber;
        }

        public override IEnumerable<byte> ToBytes()
        {
            yield return 0xFF;
            yield return 0x00;
            yield return 0x02;
            foreach(byte b in BinaryUtils.IntToByteArr(sequenceNumber, 2))
                yield return b;
        }
    }

    public class TextEvent : MetaEvent
    {
        public byte[] text;

        public TextEvent(byte[] text)
        {
            this.text = text;
        }

        public override IEnumerable<byte> ToBytes()
        {
            yield return 0xFF;
            yield return 0x01;
            foreach (byte b in BinaryUtils.IntToVariableByteArr(text.Length))
                yield return b;
            foreach (byte b in text)
                yield return b;
        }
    }

    public class CopyrightNoticeEvent : MetaEvent
    {
        public string text;

        public CopyrightNoticeEvent(string text)
        {
            this.text = text;
        }

        public override IEnumerable<byte> ToBytes()
        {
            yield return 0xFF;
            yield return 0x02;
            foreach (byte b in BinaryUtils.IntToVariableByteArr(text.Length))
                yield return b;
            foreach (byte b in Encoding.ASCII.GetBytes(text))
                yield return b;
        }
    }

    public class TrackNameEvent : MetaEvent
    {
        public string trackName;

        public TrackNameEvent(string trackName)
        {
            this.trackName = trackName;
        }

        public override IEnumerable<byte> ToBytes()
        {
            yield return 0xFF;
            yield return 0x03;
            foreach (byte b in BinaryUtils.IntToVariableByteArr(trackName.Length))
                yield return b;
            foreach (byte b in Encoding.ASCII.GetBytes(trackName))
                yield return b;
        }
    }

    public class InstrumentNameEvent : MetaEvent
    {
        public string instrumentName;

        public InstrumentNameEvent(string instrumentName)
        {
            this.instrumentName = instrumentName;
        }

        public override IEnumerable<byte> ToBytes()
        {
            yield return 0xFF;
            yield return 0x04;
            foreach (byte b in BinaryUtils.IntToVariableByteArr(instrumentName.Length))
                yield return b;
            foreach (byte b in Encoding.ASCII.GetBytes(instrumentName))
                yield return b;
        }
    }

    public class LyricEvent : MetaEvent
    {
        public string lyric;

        public LyricEvent(string lyric)
        {
            this.lyric = lyric;
        }

        public override IEnumerable<byte> ToBytes()
        {
            yield return 0xFF;
            yield return 0x05;
            foreach (byte b in BinaryUtils.IntToVariableByteArr(lyric.Length))
                yield return b;
            foreach (byte b in Encoding.ASCII.GetBytes(lyric))
                yield return b;
        }
    }

    public class MarkerEvent : MetaEvent
    {
        public string marker;

        public MarkerEvent(string marker)
        {
            this.marker = marker;
        }

        public override IEnumerable<byte> ToBytes()
        {
            yield return 0xFF;
            yield return 0x06;
            foreach (byte b in BinaryUtils.IntToVariableByteArr(marker.Length))
                yield return b;
            foreach (byte b in Encoding.ASCII.GetBytes(marker))
                yield return b;
        }
    }

    public class CuePointEvent : MetaEvent
    {
        public string cuePoint;

        public CuePointEvent(string cuePoint)
        {
            this.cuePoint = cuePoint;
        }

        public override IEnumerable<byte> ToBytes()
        {
            yield return 0xFF;
            yield return 0x07;
            foreach (byte b in BinaryUtils.IntToVariableByteArr(cuePoint.Length))
                yield return b;
            foreach (byte b in Encoding.ASCII.GetBytes(cuePoint))
                yield return b;
        }
    }

    public class MidiChannelPrefixEvent : MetaEvent
    {
        public int channel;

        public MidiChannelPrefixEvent(int channel)
        {
            this.channel = channel;
        }

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { 0xFF, 0x20, 0x01, (byte)channel };
        }
    }

    public class EndOfTrackEvent : MetaEvent
    {
        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { 0xFF, 0x2F, 0x00 };
        }
    }

    public class SetTempoEvent : MetaEvent
    {
        public int tempo;

        public SetTempoEvent(int tempo)
        {
            this.tempo = tempo;
        }

        public override IEnumerable<byte> ToBytes()
        {
            yield return 0xFF;
            yield return 0x51;
            yield return 0x03;
            foreach (byte b in BinaryUtils.IntToByteArr(tempo, 3))
                yield return b;
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

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { 0xFF, 0x54, 0x05, (byte)hours, (byte)minutes, (byte)seconds, (byte)frames, (byte)fractionalFrame };
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

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { 0xFF, 0x58, 0x04, (byte)numerator, (byte)denominator, (byte)clocksPerMetronomeTick, (byte)thirtySecondNotesPerTwentyFourClocks };
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

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { 0xFF, 0x59, 0x02, (byte)sharpsFlats, (byte)majorMinor };
        }
    }

    public class SequencerSpecificEvent : MetaEvent
    {
        public byte[] sequencerData;

        public SequencerSpecificEvent(byte[] sequencerData)
        {
            this.sequencerData = sequencerData;
        }

        public override IEnumerable<byte> ToBytes()
        {
            yield return 0xFF;
            yield return 0x7F;
            foreach (byte b in BinaryUtils.IntToVariableByteArr(sequencerData.Length))
                yield return b;
            foreach (byte b in sequencerData)
                yield return b;
        }
    }

    public class UnknownMetaEvent : MetaEvent
    {
        public byte type;
        public byte[] unknownData;

        public UnknownMetaEvent(byte type, byte[] unknownData)
        {
            this.type = type;
            this.unknownData = unknownData;
        }

        public override IEnumerable<byte> ToBytes()
        {
            yield return 0xFF;
            yield return type;
            foreach (byte b in BinaryUtils.IntToVariableByteArr(unknownData.Length))
                yield return b;
            foreach (byte b in unknownData)
                yield return b;
        }
    }
}
