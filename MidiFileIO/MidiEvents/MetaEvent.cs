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
    }

    public class CuePointEvent : MetaEvent
    {
        public string cuePoint;

        public CuePointEvent(string cuePoint)
        {
            this.cuePoint = cuePoint;
        }
    }

    public class EndOfTrackEvent : MetaEvent { }

    public class InstrumentNameEvent : MetaEvent
    {
        public string instrumentName;

        public InstrumentNameEvent(string instrumentName)
        {
            this.instrumentName = instrumentName;
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
    }

    public class LyricEvent : MetaEvent
    {
        public string lyric;

        public LyricEvent(string lyric)
        {
            this.lyric = lyric;
        }
    }

    public class MarkerEvent : MetaEvent
    {
        public string marker;

        public MarkerEvent(string marker)
        {
            this.marker = marker;
        }
    }

    public class MidiChannelPrefixEvent : MetaEvent
    {
        public int channel;

        public MidiChannelPrefixEvent(int channel)
        {
            this.channel = channel;
        }
    }

    public class SequenceNumberEvent : MetaEvent
    {
        public int sequenceNumber;

        public SequenceNumberEvent(int sequenceNumber)
        {
            this.sequenceNumber = sequenceNumber;
        }
    }

    public class SequencerSpecificEvent : MetaEvent
    {
        public byte[] sequencerData;

        public SequencerSpecificEvent(byte[] sequencerData)
        {
            this.sequencerData = sequencerData;
        }
    }

    public class SetTempoEvent : MetaEvent
    {
        public int tempo;

        public SetTempoEvent(int tempo)
        {
            this.tempo = tempo;
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
    }

    public class TextEvent : MetaEvent
    {
        public byte[] text;

        public TextEvent(byte[] text)
        {
            this.text = text;
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
    }

    public class TrackNameEvent : MetaEvent
    {
        public string trackName;

        public TrackNameEvent(string trackName)
        {
            this.trackName = trackName;
        }
    }

    public class UnknownMetaEvent : MetaEvent
    {
        public byte[] data;

        public UnknownMetaEvent(byte[] data)
        {
            this.data = data;
        }
    }
}
