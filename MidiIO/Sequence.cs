using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiIO
{
    public class Sequence
    {
        public MidiHeader header;
        public List<Track> tracks;

        public Sequence(MidiHeader header, List<Track> tracks)
        {
            this.header = header;
            this.tracks = tracks;
        }
    }
}
