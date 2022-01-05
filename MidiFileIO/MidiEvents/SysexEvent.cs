using System.Collections.Generic;

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

        public override IEnumerable<byte> ToBytes()
        {
            yield return (byte)(isEscape ? 0xF7 : 0xF0);
            foreach (byte b in BinaryUtils.IntToVariableByteArr(data.Length))
                yield return b;
            foreach (byte b in data)
                yield return b;
        }
    }
}