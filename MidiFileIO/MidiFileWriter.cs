using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiFileIO
{
    public class MidiFileWriter
    {
        MidiFile midiFile;
        FileStream file;

        public MidiFileWriter(MidiFile midiFile, string path)
        {
            this.midiFile = midiFile;
            file = new FileStream(path, FileMode.Create);
        }

        public void WriteMidiFile()
        {
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
            file.Write(BinaryUtils.IntToByteArr(6, 4));
            // Data
            file.Write(BinaryUtils.IntToByteArr((int)midiFile.header.format, 2));
            file.Write(BinaryUtils.IntToByteArr(midiFile.tracks.Length, 2));
            byte[] divisionBytes;
            if (midiFile.header.division is DivisionPPQN) // TODO: Enforce maximum values
            {
                DivisionPPQN division = (DivisionPPQN)midiFile.header.division;
                divisionBytes = BinaryUtils.IntToByteArr(division.pulsesPerQuarterNote, 2);
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
            file.Write(BinaryUtils.IntToByteArr(trackData.Length, 4));
            file.Write(trackData);
        }

        private byte[] TrackToByteArr(Track track)
        {
            List<byte> data = new List<byte>();
            foreach (MidiEvent e in track.events)
            {
                data.AddRange(BinaryUtils.IntToVariableByteArr(e.deltaTime));
                data.AddRange(e.ToByteArray());
            }
            return data.ToArray();
        }
    }
}
