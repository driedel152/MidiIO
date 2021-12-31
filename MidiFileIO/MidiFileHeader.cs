namespace MidiFileIO
{
    public class MidiFileHeader
    {
        public MidiFileFormat format;
        public int trackCount;
        public Division division;
        public byte[] ignoredData;

        public MidiFileHeader(MidiFileFormat format, int trackCount, Division division, byte[] ignoredData)
        {
            this.format = format;
            this.trackCount = trackCount;
            this.division = division;
            this.ignoredData = ignoredData;
        }
    }
}