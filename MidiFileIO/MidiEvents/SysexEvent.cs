namespace MidiFileIO
{
    public class SysexEvent : MidiEvent
    {
        public bool isEscape;
        public byte[] data;

        public SysexEvent(bool isEscape, byte[] data)
        {
            this.isEscape = isEscape;
            this.data = data;
        }
    }
}