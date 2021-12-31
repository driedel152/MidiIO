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
        public TrackEvent[] events;

        public Track(int[] deltaTimes, TrackEvent[] events)
        {
            this.deltaTimes = deltaTimes;
            this.events = events;
        }
    }
}
