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
        FileStream file;
        bool useRunningStatus;

        public MidiFileWriter(Sequence sequence, string path)
        {
            this.sequence = sequence;
            file = new FileStream(path, FileMode.Create);
        }

        public void WriteMidiFile(bool useRunningStatus = true)
        {
            this.useRunningStatus = useRunningStatus;
            WriteHeaderChunk(sequence);

            for (int i = 0; i < sequence.tracks.Count; i++)
            {
                WriteTrackChunk(sequence.tracks[i]);
            }
            file.Close();
        }

        private void WriteHeaderChunk(Sequence sequence)
        {
            // Chunk Type
            file.Write(Encoding.ASCII.GetBytes("MThd"));
            // Length
            file.Write(BinaryUtils.IntToByteArr(6, 4).ToArray());
            // Data
            file.Write(BinaryUtils.IntToByteArr((int)sequence.header.format, 2).ToArray());
            file.Write(BinaryUtils.IntToByteArr(sequence.tracks.Count, 2).ToArray());
            byte[] divisionBytes;
            if (sequence.header.division is DivisionPPQN) // TODO: Enforce maximum values
            {
                DivisionPPQN division = (DivisionPPQN)sequence.header.division;
                divisionBytes = BinaryUtils.IntToByteArr(division.pulsesPerQuarterNote, 2).ToArray();
                divisionBytes[0] &= 0x7F;
            }
            else
            {
                DivisionFrameBased division = (DivisionFrameBased)sequence.header.division;
                divisionBytes = new byte[2] { (byte)(0x80 | division.framesPerSecond), (byte)division.deltaTimePerFrame };
            }
            file.Write(divisionBytes);
        }

        private void WriteTrackChunk(Track track)
        {
            file.Write(Encoding.ASCII.GetBytes("MTrk"));

            byte[] trackData = TrackToByteArr(track);
            file.Write(BinaryUtils.IntToByteArr(trackData.Length, 4).ToArray());
            file.Write(trackData);
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
