using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiIO
{
    public class Sequence
    {
        public MidiFormat format;
        public Division division;
        public List<Track> tracks;

        public Sequence(List<Track> tracks, Division division, MidiFormat format)
        {
            this.tracks = tracks;
            this.division = division;
            this.format = format;
        }
    }
}
