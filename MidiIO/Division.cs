namespace MidiIO
{
    public abstract class Division 
    {
        /// <summary>
        /// Converts number of quarter note beats to the absolute time in a sequence.
        /// </summary>
        /// <param name="beats"></param>
        /// <returns></returns>
        public abstract int BeatsToAbsolute(float beats);
    }

    public class DivisionPPQN : Division
    {
        public int pulsesPerQuarterNote;

        public DivisionPPQN(int pulsesPerQuarterNote)
        {
            this.pulsesPerQuarterNote = pulsesPerQuarterNote;
        }

        public override int BeatsToAbsolute(float beats)
        {
            return (int)(beats * pulsesPerQuarterNote);
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

        public override int BeatsToAbsolute(float beats)
        {
            throw new System.NotImplementedException("Frame based division format is not supported");
        }
    }
}