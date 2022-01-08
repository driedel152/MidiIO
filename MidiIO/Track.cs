using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiIO
{
    public class Track
    {
        public MidiEvent[] events;

        public Track(MidiEvent[] events)
        {
            this.events = events;
        }
    }
}
