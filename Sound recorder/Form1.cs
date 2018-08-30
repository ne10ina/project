using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NA = NAudio.Wave;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.Streams;
using CSCore.Win32;


namespace Sound_recorder
{
    public partial class Form1 : Form
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int record(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);

        private Forms fo;
        private Recorder rec;
        private Player player;

        public void getForms(Forms fos)
        {
            fo = fos;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ////record("open new Type waveaudio Alias recsound", "", 0, 0);
            ////record("record recsound", null, 0, 0);
            
            rec.startRec();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ////record("save recsound mic.wav", "", 0, 0);
            ////record("close recsound", "", 0, 0);

            rec.refreshDevices();
        }

        

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != null)
            {
                player.fileName = textBox1.Text;
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            player.startPlaying();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            rec.stopRec();
        }
        


        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            rec.selectedDeviceChanged();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            OnClosing(e);
            rec.stopRec();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rec = new Recorder(fo);
            player = new Player(fo);
        }
    }
}
