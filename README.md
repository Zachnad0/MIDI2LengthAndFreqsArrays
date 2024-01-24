# MIDI To Note Length, Delay, and Frequency In A Relevantly Applicable Format
## Or just MIDI2LengthAndFreqsArrays.

### Description
This is simply a quite useful, albeit niche, program written by yours truly that translates MIDI files into usable array data.
Designed originally specifically for making arduino buzzer tone tunes *much* more easily than manual transcription, this simple software outputs three clearly labeled arrays in each text file, for each track, for each present MIDI file.

### Usage Instructions
0) Download and extract zip file or publish the release build of source.
0) Run *the* executable once.
0) Copy and paste your midi files (*.mid, *.midi) into the newly created `./MIDI2LengthAndFreqsArrays_IO/` folder located at the exe.
0) Run the executable again. (Hit any key to close at end)
0) Check the `./MIDI2LengthAndFreqsArrays_IO/` folder and observe your new text files.

Note that the program deletes all (*.txt) files within the IO folder upon execution, then regenerates based on the current MIDI files.


### Output Format
Plain text files (.txt) of names formatted to include the track and origin MIDI file name.
Example contents of output file:
> `Note Frequency (Hz): {[doubles]}`
> `Note Length (ms): {[more doubles]}`
> `Pre-Note Delay (ms): {[doubles again]}`

### Basic Usage Example
Here is some C++ Arduino code for playing the output on a buzzer:

```
long TestSeqAlphaFreqs[] PROGMEM = {[insert note frequencies]};
long TestSeqAlphaLengths[] PROGMEM = {[insert note lengths]};
long TestSeqAlphaPreNoteDelay[] PROGMEM = {[insert pre-note delays]};
// No idea why I used longs but usually uint16_t should be fine
```
```
void setup()
{
  // BUZZER BOOTUP SONG TEST
  noTone(BZZR_TEST_PIN);
  long prevNoteSustain = 0;
  for (int n = 0; n < TestSeqAlphaTotalLength; n++)
  {
    uint32_t
      frequency = pgm_read_word_near(&(TestSeqAlphaFreqs[n])),
      sustainLength = pgm_read_word_near(&(TestSeqAlphaLengths[n])),
      preDelay = pgm_read_word_near(&(TestSeqAlphaPreNoteDelay[n]));
  
    delay(preDelay + prevNoteSustain);
    tone(BZZR_TEST_PIN, frequency, max(sustainLength, 8));
    prevNoteSustain = sustainLength;
  }
}
```

Multiple tracks would require multiple buzzers, thus the loop code would have to be modified a bit for that.