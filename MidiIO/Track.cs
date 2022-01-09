using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiIO
{
    public class Track
    {
        private SortedDictionary<int, List<MidiEvent>> events;

        public Track()
        {
            events = new SortedDictionary<int, List<MidiEvent>>();
        }

        public IEnumerable<MidiEvent> GetEvents()
        {
            List<MidiEvent> allEvents = new List<MidiEvent>();
            foreach(List<MidiEvent> list in events.Values)
            {
                allEvents.AddRange(list);
            }
            return allEvents;
        }

        public IEnumerable<MidiEvent> GetEvents(int absoluteTime)
        {
            return events[absoluteTime].ToArray();
        }

        public void AddEvent(int absoluteTime, MidiEvent midiEvent)
        {
            if (!events.ContainsKey(absoluteTime))
            {
                events.Add(absoluteTime, new List<MidiEvent>());
            }

            midiEvent.AbsoluteTime = absoluteTime;
            midiEvent.OnUpdateAbsoluteTime += HandleUpdateAbsoluteTime;
            events[absoluteTime].Add(midiEvent);
        }

        private void HandleUpdateAbsoluteTime(MidiEvent midiEvent, int previousValue)
        {
            events[previousValue].Remove(midiEvent);
            AddEvent(midiEvent.AbsoluteTime, midiEvent);
        }
    }
}
