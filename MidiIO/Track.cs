using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiIO
{
    public class Track
    {
        /// <summary>
        /// The Sequence that contains this Track. This value is assigned when the Track is added to a Sequence.
        /// </summary>
        internal Sequence parentSequence;
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

        public IEnumerable<MidiEvent> GetEventsAbsolute(int absoluteTime)
        {
            if (events.ContainsKey(absoluteTime))
            {
                return events[absoluteTime].ToArray();
            }
            else
            {
                return new MidiEvent[] { };
            }
        }

        public IEnumerable<MidiEvent> GetEvents(float beat)
        {
            return GetEventsAbsolute(parentSequence.division.BeatsToAbsolute(beat));
        }

        public void AddEventAbsolute(MidiEvent midiEvent, int absoluteTime)
        {
            if (!events.ContainsKey(absoluteTime))
            {
                events.Add(absoluteTime, new List<MidiEvent>());
            }

            midiEvent.AbsoluteTime = absoluteTime;
            midiEvent.OnUpdateAbsoluteTime += HandleUpdateAbsoluteTime;
            events[absoluteTime].Add(midiEvent);
        }

        public void AddEvent(MidiEvent midiEvent, float beat)
        {
            if (parentSequence?.division != null)
            {
                AddEventAbsolute(midiEvent, parentSequence.division.BeatsToAbsolute(beat));
            }
            else
            {
                throw new InvalidOperationException("The Track is not owned by a Sequence with a defined Division!");
            }
        }

        private void HandleUpdateAbsoluteTime(MidiEvent midiEvent, int previousValue)
        {
            events[previousValue].Remove(midiEvent);
            AddEventAbsolute(midiEvent, midiEvent.AbsoluteTime);
        }
    }
}
