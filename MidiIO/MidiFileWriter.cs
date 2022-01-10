using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiIO
{
    public class MidiFileWriter
    {
        Sequence sequence;
        MidiFormat format;
        FileStream file;
        bool useRunningStatus;

        public MidiFileWriter(Sequence sequence, string path)
        {
            this.sequence = sequence;
            file = new FileStream(path, FileMode.Create);
        }

        public void WriteMidiFile(MidiFormat format, bool useRunningStatus = true)
        {
            this.useRunningStatus = useRunningStatus;
            this.format = format;
            WriteHeaderChunk(sequence);

            if(format == MidiFormat.SingleTrack)
            {
                // TODO: Merge tracks to a single track
                WriteTrackChunk(sequence.Tracks[0]);
            }
            else
            {
                for (int i = 0; i < sequence.Tracks.Count; i++)
                {
                    WriteTrackChunk(sequence.Tracks[i]);
                }
            }
            file.Close();
        }

        private void WriteHeaderChunk(Sequence sequence)
        {
            // Chunk Type
            file.Write(Encoding.ASCII.GetBytes("MThd"), 0, 4);
            // Length
            file.Write(BinaryUtils.IntToByteArr(6, 4), 0, 4);
            // Data
            file.Write(BinaryUtils.IntToByteArr((int)format, 2), 0, 2);
            file.Write(BinaryUtils.IntToByteArr(sequence.Tracks.Count, 2), 0, 2);
            byte[] divisionBytes;
            if (sequence.division is DivisionPPQN) // TODO: Enforce maximum values
            {
                DivisionPPQN division = (DivisionPPQN)sequence.division;
                divisionBytes = BinaryUtils.IntToByteArr(division.pulsesPerQuarterNote, 2);
                divisionBytes[0] &= 0x7F;
            }
            else
            {
                DivisionFrameBased division = (DivisionFrameBased)sequence.division;
                divisionBytes = new byte[2] { (byte)(0x80 | division.framesPerSecond), (byte)division.deltaTimePerFrame };
            }
            file.Write(divisionBytes, 0, divisionBytes.Length);
        }

        private void WriteTrackChunk(Track track)
        {
            file.Write(Encoding.ASCII.GetBytes("MTrk"), 0, 4);

            byte[] trackData = TrackToByteArr(track);
            file.Write(BinaryUtils.IntToByteArr(trackData.Length, 4), 0, 4);
            file.Write(trackData, 0, trackData.Length);
        }

        private byte[] TrackToByteArr(Track track)
        {
            List<byte> data = new List<byte>();
            byte runningStatus = 0x00;

            bool endsWithEndOfTrack = false;
            int lastEventTime = 0;
            foreach (MidiEvent e in track.GetEvents())
            {
                int deltaTime = e.AbsoluteTime - lastEventTime;
                lastEventTime = e.AbsoluteTime;
                data.AddRange(BinaryUtils.IntToVariableByteArr(deltaTime));
                IEnumerable<byte> bytes = e.ToBytes();
                if (useRunningStatus && e is ChannelEvent)
                {
                    foreach (byte b in bytes)
                    {
                        if (b == runningStatus) continue; // Skip status byte
                        data.Add(b);
                    }
                }
                else
                {
                    data.AddRange(bytes);
                }
                runningStatus = bytes.First();
                endsWithEndOfTrack = e is EndOfTrackEvent;
            }

            // Required event
            if (!endsWithEndOfTrack)
            {
                data.Add(0x00); // delta time
                data.AddRange(EndOfTrackEvent.BYTES);
            }

            return data.ToArray();
        }
    }
}
