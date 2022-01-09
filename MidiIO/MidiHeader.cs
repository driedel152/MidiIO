namespace MidiIO
{
    internal class MidiHeader
    {
        public MidiFormat format;
        public Division division;
        public byte[] ignoredData;

        public MidiHeader(MidiFormat format, Division division, byte[] ignoredData = null)
        {
            this.format = format;
            this.division = division;
            this.ignoredData = ignoredData;
        }
    }
}