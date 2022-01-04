using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiFileIO
{
    public abstract class ChannelEvent : MidiEvent 
    {
        public int channel;
    }

    public abstract class ChannelModeEvent : ChannelEvent { }

    public class AllNotesOffEvent : ChannelModeEvent
    {
        public AllNotesOffEvent(int channel)
        {
            this.channel = channel;
        }
    }

    public class AllSoundOffEvent : ChannelModeEvent
    {
        public AllSoundOffEvent(int channel)
        {
            this.channel = channel;
        }
    }

    public class LocalControlEvent : ChannelModeEvent
    {
        public bool connect;

        public LocalControlEvent(int channel, bool connect)
        {
            this.channel = channel;
            this.connect = connect;
        }
    }

    public class MonoModeOnEvent : ChannelModeEvent
    {
        public int numChannels;

        public MonoModeOnEvent(int channel, int numChannels)
        {
            this.channel = channel;
            this.numChannels = numChannels;
        }
    }

    public class OmniModeOffEvent : ChannelModeEvent
    {
        public OmniModeOffEvent(int channel)
        {
            this.channel = channel;
        }
    }

    public class OmniModeOnEvent : ChannelModeEvent
    {
        public OmniModeOnEvent(int channel)
        {
            this.channel = channel;
        }
    }

    public class PolyModeOnEvent : ChannelModeEvent
    {
        public PolyModeOnEvent(int channel)
        {
            this.channel = channel;
        }
    }

    public class ResetAllControllersEvent : ChannelModeEvent
    {
        public ResetAllControllersEvent(int channel)
        {
            this.channel = channel;
        }
    }

    public abstract class ChannelVoiceEvent : ChannelEvent { }

    public class ChannelKeyPressureEvent : ChannelVoiceEvent
    {
        public int channelPressure;

        public ChannelKeyPressureEvent(int channel, int channelPressure)
        {
            this.channel = channel;
            this.channelPressure = channelPressure;
        }
    }

    public class ControllerChangeEvent : ChannelVoiceEvent
    {
        public int controllerNumber;
        public int controllerValue;

        public ControllerChangeEvent(int channel, int controllerNumber, int controllerValue)
        {
            this.channel = channel;
            this.controllerNumber = controllerNumber;
            this.controllerValue = controllerValue;
        }
    }

    public class NoteOffEvent : ChannelVoiceEvent
    {
        public int keyOff;
        public int velocityOff;

        public NoteOffEvent(int channel, int keyOff, int velocityOff)
        {
            this.channel = channel;
            this.keyOff = keyOff;
            this.velocityOff = velocityOff;
        }
    }

    public class NoteOnEvent : ChannelVoiceEvent
    {
        public int keyOn;
        public int velocityOn;

        public NoteOnEvent(int channel, int keyOn, int velocityOn)
        {
            this.channel = channel;
            this.keyOn = keyOn;
            this.velocityOn = velocityOn;
        }
    }

    public class PitchBendEvent : ChannelVoiceEvent
    {
        public int lsb;
        public int msb;

        public PitchBendEvent(int channel, int lsb, int msb)
        {
            this.channel = channel;
            this.lsb = lsb;
            this.msb = msb;
        }
    }

    public class PolyphonicKeyPressureEvent : ChannelVoiceEvent
    {
        public int polyKey;
        public int keyPressure;

        public PolyphonicKeyPressureEvent(int channel, int polyKey, int keyPressure)
        {
            this.channel = channel;
            this.polyKey = polyKey;
            this.keyPressure = keyPressure;
        }
    }

    public class ProgramChangeEvent : ChannelVoiceEvent
    {
        public ProgramName programName;

        public ProgramChangeEvent(int channel, ProgramName programName)
        {
            this.channel = channel;
            this.programName = programName;
        }
    }
}
