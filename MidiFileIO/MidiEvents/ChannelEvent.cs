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

    #region ChannelVoiceEvents
    public abstract class ChannelVoiceEvent : ChannelEvent { }

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

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0x80 | channel), (byte)keyOff, (byte)velocityOff };
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

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0x90 | channel), (byte)keyOn, (byte)velocityOn };
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

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0xA0 | channel), (byte)polyKey, (byte)keyPressure };
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

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0xB0 | channel), (byte)controllerNumber, (byte)controllerValue };
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

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0xC0 | channel), (byte)programName };
        }
    }

    public class ChannelKeyPressureEvent : ChannelVoiceEvent
    {
        public int channelPressure;

        public ChannelKeyPressureEvent(int channel, int channelPressure)
        {
            this.channel = channel;
            this.channelPressure = channelPressure;
        }

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0xD0 | channel), (byte)channelPressure };
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

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0xE0 | channel), (byte)lsb, (byte)msb };
        }
    }
    #endregion

    #region ChannelModeEvents
    public abstract class ChannelModeEvent : ChannelEvent { }

    public class AllSoundOffEvent : ChannelModeEvent
    {
        public AllSoundOffEvent(int channel)
        {
            this.channel = channel;
        }

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0xB0 | channel), 0x78, 0x00 };
        }
    }

    public class ResetAllControllersEvent : ChannelModeEvent
    {
        public ResetAllControllersEvent(int channel)
        {
            this.channel = channel;
        }

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0xB0 | channel), 0x79, 0x00 };
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

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0xB0 | channel), 0x7A, (byte)(connect ? 0x7F : 0x00) };
        }
    }

    public class AllNotesOffEvent : ChannelModeEvent
    {
        public AllNotesOffEvent(int channel)
        {
            this.channel = channel;
        }

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0xB0 | channel), 0x7B, 0x00 };
        }
    }

    public class OmniModeOffEvent : ChannelModeEvent
    {
        public OmniModeOffEvent(int channel)
        {
            this.channel = channel;
        }

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0xB0 | channel), 0x7C, 0x00 };
        }
    }

    public class OmniModeOnEvent : ChannelModeEvent
    {
        public OmniModeOnEvent(int channel)
        {
            this.channel = channel;
        }

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0xB0 | channel), 0x7D, 0x00 };
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

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0xB0 | channel), 0x7E, (byte)numChannels };
        }
    }

    public class PolyModeOnEvent : ChannelModeEvent
    {
        public PolyModeOnEvent(int channel)
        {
            this.channel = channel;
        }

        public override IEnumerable<byte> ToBytes()
        {
            return new byte[] { (byte)(0xB0 | channel), 0x7F, 0x00 };
        }
    }
    #endregion

}
