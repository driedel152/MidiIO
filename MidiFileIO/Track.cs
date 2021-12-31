using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiFileIO
{
    public class Track
    {
        public int[] deltaTimes;
        public MidiEvent[] events;

        public Track(int[] deltaTimes, MidiEvent[] events)
        {
            this.deltaTimes = deltaTimes;
            this.events = events;
        }
    }
}
