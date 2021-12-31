using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiFileIO
{
    public class MidiFile
    {
        public MidiFileHeader header;
        public Track[] tracks;

        public MidiFile(MidiFileHeader header, Track[] tracks)
        {
            this.header = header;
            this.tracks = tracks;
        }
    }
}
