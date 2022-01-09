namespace MidiIO
{
    public class MidiHeader
    {
        public MidiFormat format;
        internal int trackCount; // Used for reading, not writing
        public Division division;
        public byte[] ignoredData;

        public MidiHeader(MidiFormat format, int trackCount, Division division, byte[] ignoredData = null)
        {
            this.format = format;
            this.trackCount = trackCount;
            this.division = division;
            this.ignoredData = ignoredData;
        }
    }
}