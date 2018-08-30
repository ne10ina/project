using System;
using System.Windows.Forms;
using NAudio;
using NAudio.Wave;

namespace Sound_recorder
{
    public class Player
    {
        public string fileName = "";

        private Forms fo;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        public Player(Forms fos)
        {
            fo = fos;
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            MessageBox.Show("stopped");
            outputDevice.Dispose();
            outputDevice = null;
            audioFile.Dispose();
            audioFile = null;
        }

        public void startPlaying()
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }

            if (audioFile == null)
            {
                try
                {
                    audioFile = new AudioFileReader(fileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Укажите имя файла...");
                    return;
                }
                outputDevice.Init(audioFile);
            }

            outputDevice.Play();
            
        }
    }
}