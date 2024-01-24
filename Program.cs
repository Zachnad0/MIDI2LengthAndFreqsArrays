using Commons.Music.Midi;
using System;
using System.IO;
using System.Linq;

namespace MIDI2LengthAndFreqsArrays
{
	public class Program
	{
		const string
#if DEBUG
			IO_FILE_PATH = "../MIDI2LengthAndFreqsArrays_IO/";
#elif RELEASE
			IO_FILE_PATH = "./MIDI2LengthAndFreqsArrays_IO/";
#endif

		static void Main(string[] args)
		{
			Console.WriteLine("==> Init success...");

			// Remove previous output text files
			Directory.CreateDirectory(IO_FILE_PATH);
			foreach (string fileDir in Directory.GetFiles(IO_FILE_PATH).Where(d => d.ToLower().EndsWith(".txt")))
				File.Delete(fileDir);

			// Go through each MIDI file, and get the channels
			string[] midiFileDirs = Directory.GetFiles(IO_FILE_PATH).Where(d => d.ToLower().EndsWith(".mid") || d.ToLower().EndsWith(".midi")).ToArray();
			foreach (string midiFileDir in midiFileDirs)
			{
				MidiMusic songMusic = MidiMusic.Read(File.OpenRead(midiFileDir));
				string songName = midiFileDir.Split('/').Last().Split('.').First();
				double ticksPerMS = songMusic.GetTotalTicks() / (double)songMusic.GetTotalPlayTimeMilliseconds();

				// Make an output file for each track
				for (int trackN = 0; trackN < songMusic.Tracks.Count; trackN++)
				{
					string outputNoteFreqs = "Note Frequency (Hz): {", outputNoteLengths = "Note Length (ms): {", outputPreNoteDelay = "Pre-Note Delay (ms): {";

					// For each event in track, record based on note open/close type and incl. time
					int msgCount = songMusic.Tracks[trackN].Messages.Count;
					bool isAnyNotes = false;
					for (int noteN = 0; noteN < msgCount; noteN++)
					{
						MidiMessage currMessage = songMusic.Tracks[trackN].Messages[noteN];
						switch (currMessage.Event.EventType)
						{
							case MidiEvent.NoteOn: // Note on: delta is time between note
								isAnyNotes = true;

								outputPreNoteDelay += $"{currMessage.DeltaTime / ticksPerMS},";
								byte midiNoteNum = currMessage.Event.Msb;
								outputNoteFreqs += $"{FreqFromMIDINote(midiNoteNum)},"; // Central byte
								//Console.WriteLine(midiNoteNum);
								break;

							case MidiEvent.NoteOff: // Note off: delta is length of note
								isAnyNotes = true;

								outputNoteLengths += $"{currMessage.DeltaTime / ticksPerMS},";
								break;

							default: continue; // Ignore other types
						}
					}

					// Close output and write to file
					if (isAnyNotes)
					{
						outputNoteFreqs = outputNoteFreqs.TrimEnd(',') + '}';
						outputNoteLengths = outputNoteLengths.TrimEnd(',') + '}';
						outputPreNoteDelay = outputPreNoteDelay.TrimEnd(',') + '}';
						File.WriteAllText(IO_FILE_PATH + $"{songName}-Track{trackN}.txt", outputNoteFreqs + "\n\n" + outputNoteLengths + "\n\n" + outputPreNoteDelay);
						Console.WriteLine($"==> \'{songName}-Track{trackN}.txt\' Generated.");
					}
					else
						Console.WriteLine($"==> {songName}-Track{trackN} contained no basic notes, skipping...");
				}
			}

			// Done
			Console.WriteLine("==> Done.");
			Console.ReadKey();
		}

		private static double FreqFromMIDINote(byte note) => Math.Pow(2, (double)(note - 69) / 12) * 440;
	}
}
