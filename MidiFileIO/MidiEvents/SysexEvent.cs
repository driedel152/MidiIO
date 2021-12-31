namespace MidiFileIO
{
    public class SysexEvent : MidiEvent
    {
        public byte[] data;

        public SysexEvent(byte[] data)
        {
            this.data = data;
        }
    }
}