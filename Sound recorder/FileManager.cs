using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;

namespace Sound_recorder
{
    public class FileManager
    {
        private DateTime dt = DateTime.UtcNow;

        public void showDate()
        {
            dt = DateTime.UtcNow;
            MessageBox.Show(dt.Year + " " + dt.Month + " " + dt.Day + " " + (dt.Hour + 5) + " " + dt.Minute);
        }

        private NewDir nd;

        public void init()
        {
            nd = new NewDir(this);
            nd.Show();

        }

        private void SetCurrentFolder(string name)
        {
            Properties.Settings.Default.Current = name;
        }


        //ИмяФамилия:Включать дату:номер записи;


        public bool isExits(string path)
        {
            return Directory.Exists(path);
        }

        public void replaceRecord()
        {
            string prePathRecord = "tmp.wav";
            string newPathRecord = null;

            Splt sp = new Splt();

            List<string> setting = sp.oneSplit(Properties.Settings.Default.Current);

            newPathRecord = setting[0] + "\\" + setting[0] + setting[2];
            if (setting[1] == "1")
            {
                dt = DateTime.UtcNow;
                newPathRecord = newPathRecord + " " + dt.Year + "_" + dt.Month + "_" + dt.Day + " " + dt.Hour + "." +
                                dt.Minute;

            }

            newPathRecord = newPathRecord + ".wav";

            File.Copy(prePathRecord, newPathRecord);

            setting[2] = Convert.ToString(Convert.ToInt32(setting[2]) + 1);

            string upd = setting[0] + ":" + setting[1] + ":" + setting[2] + ";";
            
            updateUsers(setting);
            Properties.Settings.Default.Current = upd;
        }

        private void updateUsers(List<string> upd)
        {
            Splt sp = new Splt();

            List<List<string>> tmp = sp.doSplit(Properties.Settings.Default.Users);

            for (int i = 0; i < tmp.Count - 1; i++)
            {
                if (tmp[i][0] == upd[0])
                {
                    tmp[i] = upd;
                    break;
                }
            }

            MessageBox.Show(tmp.Count.ToString());

            Properties.Settings.Default.Users = sp.undoSplt(tmp);
        }

        public void CreateNewFolder(string name, bool isDate)
        {
            string setting;
            bool isUnnamed = false;

            if (String.IsNullOrWhiteSpace(name))
            {
                name = "Unnamed" + Properties.Settings.Default.Unnamed.ToString();
                isUnnamed = true;
            }

            setting = name + ":";

            if (isDate)
            {
                dt = DateTime.UtcNow;
                setting = setting + "1" + ":" + "0";
            }
            else
            {
                setting = setting + "0" + ":" + "0";
            }

            setting = setting + ";";

            if (isExits(Environment.CurrentDirectory + "\\" + name))
            {
                MessageBox.Show("Уже существует");
                return;
            }

            SetCurrentFolder(setting);
            Properties.Settings.Default.Users = Properties.Settings.Default.Users + setting;
            MessageBox.Show(Properties.Settings.Default.Users);
            Properties.Settings.Default.Count++;

            if (isUnnamed)
            {
                Properties.Settings.Default.Unnamed++;
            }

            Directory.CreateDirectory(Environment.CurrentDirectory + "\\" + name);

            nd.Close();
            nd.Dispose();
        }
    }
}