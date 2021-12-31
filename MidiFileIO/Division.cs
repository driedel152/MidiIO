namespace MidiFileIO
{
    public abstract class Division
    {
        protected DivisionFormat format;
    }

    public class DivisionPPQN : Division
    {
        public int pulsesPerQuarterNote;

        public DivisionPPQN(int pulsesPerQuarterNote)
        {
            format = DivisionFormat.PPQN;
            this.pulsesPerQuarterNote = pulsesPerQuarterNote;
        }
    }

    public class DivisionFrameBased : Division
    {
        public int deltaTimePerFrame;
        public int framesPerSecond;

        public DivisionFrameBased(int deltaTimePerFrame, int framesPerSecond)
        {
            this.deltaTimePerFrame = deltaTimePerFrame;
            this.framesPerSecond = framesPerSecond;
        }
    }

    public enum DivisionFormat
    {
        /// <summary>
        /// Division format that specifies the number of delta-time units in each a quarter-note (Pulses Per Quarter Note).
        /// </summary>
        PPQN = 0,
        /// <summary>
        /// Division format that specifies the delta-time units per SMTPE frame and SMTPE frames per second.
        /// </summary>
        FrameBased = 1,
    }
}