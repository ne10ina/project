using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
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
    public class Recorder
    {
        private Forms fo;
        public Recorder(Forms fos)
        {
            fo = fos;
        }
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int record(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);

        private const string CaptureMode = "Capture";
        private MMDevice _selectedDevice;
        private WasapiCapture _soundIn;
        private IWriteable _writer;
        private IWaveSource _finalSource;

        public MMDevice SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                if (value != null)
                {
                    fo.frm1.button1.Enabled = true;
                }
            }
        }

        public void selectedDeviceChanged()
        {
            if (fo.frm1.deviceList.SelectedItems.Count > 0)
            {
                SelectedDevice = (MMDevice)fo.frm1.deviceList.SelectedItems[0].Tag;
            }
            else
            {
                SelectedDevice = null;
            }
        }

        public void startRec()
        {
            StartCapture();
            fo.frm1.button1.Enabled = false;
            fo.frm1.button4.Enabled = true;
            
        }

        public void refreshDevices()
        {
            fo.frm1.deviceList.Items.Clear();

            using (var deviceEnumerator = new MMDeviceEnumerator())
            using (var deviceCollection = deviceEnumerator.EnumAudioEndpoints(
                CaptureMode == "Capture" ? DataFlow.Capture : DataFlow.Render, DeviceState.Active))
            {
                foreach (var device in deviceCollection)
                {
                    var deviceFormat = WaveFormatFromBlob(device.PropertyStore[
                        new PropertyKey(new Guid(0xf19f064d, 0x82c, 0x4e27, 0xbc, 0x73, 0x68, 0x82, 0xa1, 0xbb, 0x8e, 0x4c), 0)].BlobValue);

                    var it = device + " " + deviceFormat.Channels.ToString(CultureInfo.InvariantCulture);

                    var item = new ListViewItem(device.FriendlyName) { Tag = device };
                    item.SubItems.Add(deviceFormat.Channels.ToString(CultureInfo.InvariantCulture));

                    fo.frm1.deviceList.Items.Add(item);
                }
            }
        }

        private void StartCapture()
        {
            if (SelectedDevice == null)
                return;

            if (CaptureMode == "Capture")
                _soundIn = new WasapiCapture();

            _soundIn.Device = SelectedDevice;
            _soundIn.Initialize();

            var soundInSource = new SoundInSource(_soundIn);
            var singleBlockNotificationStream = new SingleBlockNotificationStream(soundInSource.ToSampleSource());
            _finalSource = singleBlockNotificationStream.ToWaveSource();
            _writer = new WaveWriter("tmp.wav", _finalSource.WaveFormat);

            byte[] buffer = new byte[_finalSource.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += (s, e) =>
            {
                int read;
                while ((read = _finalSource.Read(buffer, 0, buffer.Length)) > 0)
                    _writer.Write(buffer, 0, read);
            };

            //singleBlockNotificationStream.SingleBlockRead += SingleBlockNotificationStreamOnSingleBlockRead; // visualization

            _soundIn.Start();
        }

        private static WaveFormat WaveFormatFromBlob(Blob blob)
        {
            if (blob.Length == 40)
                return (WaveFormat)Marshal.PtrToStructure(blob.Data, typeof(WaveFormatExtensible));
            return (WaveFormat)Marshal.PtrToStructure(blob.Data, typeof(WaveFormat));
        }

        public void stopRec()
        {
            StopCapture();
        }

        private void StopCapture()
        {
            if (_soundIn != null)
            {
                _soundIn.Stop();
                _soundIn.Dispose();
                _soundIn = null;
                _finalSource.Dispose();

                if (_writer is IDisposable)
                    ((IDisposable)_writer).Dispose();

                FileManager fm = new FileManager();
                fm.replaceRecord();

                fo.frm1.button4.Enabled = false;
                fo.frm1.button1.Enabled = true;
            }
        }
    }
}