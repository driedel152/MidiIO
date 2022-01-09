using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiIO
{
    public class Sequence
    {
        public Division division;
        public List<Track> tracks;

        public Sequence(List<Track> tracks, Division division)
        {
            this.tracks = tracks;
            this.division = division;
        }
    }
}
