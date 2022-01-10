using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiIO
{
    public class Sequence
    {
        public Division division;
        private List<Track> tracks;
        public ReadOnlyCollection<Track> Tracks
        {
            get => tracks.AsReadOnly();
        }

        public Sequence(Division division)
        {
            this.division = division;
            tracks = new List<Track>();
        }

        public void AddTrack(Track track)
        {
            if (track.parentSequence != null)
                throw new InvalidOperationException("The Track is already owned by another Sequence!");
            track.parentSequence = this;
            tracks.Add(track);
        }

        public void RemoveTrack(Track track)
        {
            track.parentSequence = null;
            tracks.Remove(track);
        }

        public void RemoveTrackAt(int i)
        {
            tracks[i].parentSequence = null;
            tracks.RemoveAt(i);
        }
    }
}
