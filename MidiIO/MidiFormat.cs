using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiIO
{
    public enum MidiFormat
    {
        /// <summary>
        /// The MIDI format that consists of a header-chunk and a single track-chunk.
        /// The single track chunk will contain all the note and tempo information.
        /// </summary>
        SingleTrack = 0,
        /// <summary>
        /// The MIDI format that consists of a header-chunk and one or more track-chunks, 
        /// with all tracks being played simultaneously. The first track in this format 
        /// is known as the 'Tempo Map'.
        /// </summary>
        SimultaneousTracks = 1,
        /// <summary>
        /// The MIDI format that consists of a header-chunk and one or more track-chunks, 
        /// where each track represents an independant sequence. 
        /// </summary>
        IndependentTracks = 2,
    }
}
