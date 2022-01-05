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

        public override byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)(isEscape ? 0xF7 : 0xF0));
            bytes.AddRange(BinaryUtils.IntToVariableByteArr(data.Length));
            bytes.AddRange(data);
            return bytes.ToArray();
        }
    }
}