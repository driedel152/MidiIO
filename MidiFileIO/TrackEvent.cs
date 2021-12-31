namespace MidiFileIO
{
    public abstract class TrackEvent
    {
        EventType type;
    }

    public enum EventType
    {
        /// <summary>
        /// Any MIDI Channel message, including Channel Voice and Channel Mode messages.
        /// </summary>
        MidiEvent,
        /// <summary>
        /// System Exclusive messages.
        /// </summary>
        SysexEvent,
        /// <summary>
        /// Meta Events are used for things like track-names, lyrics and cue-points, which 
        /// don't result in MIDI messages being sent, but are still useful components of a 
        /// MIDI file.
        /// </summary>
        MetaEvent,
    }
}