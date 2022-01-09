using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiIO
{
    public class Track
    {
        public List<MidiEvent> events;

        public Track(List<MidiEvent> events)
        {
            this.events = events;
        }
    }
}
