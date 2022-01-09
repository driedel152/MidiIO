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
        MidiFile midiFile;
        FileStream file;
        bool useRunningStatus;

        public MidiFileWriter(MidiFile midiFile, string path)
        {
            this.midiFile = midiFile;
            file = new FileStream(path, FileMode.Create);
        }

        public void WriteMidiFile(bool useRunningStatus = true)
        {
            this.useRunningStatus = useRunningStatus;
            WriteHeaderChunk(midiFile);

            for (int i = 0; i < midiFile.tracks.Length; i++)
            {
                WriteTrackChunk(midiFile.tracks[i]);
            }
            file.Close();
        }

        private void WriteHeaderChunk(MidiFile midiFile)
        {
            // Chunk Type
            file.Write(Encoding.ASCII.GetBytes("MThd"));
            // Length
            file.Write(BinaryUtils.IntToByteArr(6, 4).ToArray());
            // Data
            file.Write(BinaryUtils.IntToByteArr((int)midiFile.header.format, 2).ToArray());
            file.Write(BinaryUtils.IntToByteArr(midiFile.tracks.Length, 2).ToArray());
            byte[] divisionBytes;
            if (midiFile.header.division is DivisionPPQN) // TODO: Enforce maximum values
            {
                DivisionPPQN division = (DivisionPPQN)midiFile.header.division;
                divisionBytes = BinaryUtils.IntToByteArr(division.pulsesPerQuarterNote, 2).ToArray();
                divisionBytes[0] &= 0x7F;
            }
            else
            {
                DivisionFrameBased division = (DivisionFrameBased)midiFile.header.division;
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
            foreach (MidiEvent e in track.events)
            {
                data.AddRange(BinaryUtils.IntToVariableByteArr(e.deltaTime));
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
            }

            // Required event
            if(!(track.events[track.events.Count - 1] is EndOfTrackEvent))
            {
                data.Add(0x00);
                data.AddRange(EndOfTrackEvent.BYTES);
            }

            return data.ToArray();
        }
    }
}
