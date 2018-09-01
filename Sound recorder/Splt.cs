using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Sound_recorder
{
    public class Splt
    {
        public List<List<string>> doSplit(string toSplt)
        {
            List<string> preans;
            List<List<string>> ans = new List<List<string>>();
            preans = toSplt.Split(';').ToList();
            for (int i = 0; i < preans.Count - 1; i++)
            {
                ans.Add(preans);
                ans[i] = preans[i].Split(':').ToList();
            }

            return ans;
        }

        public List<string> doPreSplit(string toSplt)
        {
            List<string> ans;
            ans = toSplt.Split(';').ToList();
            for (int i = 0; i < ans.Count - 1; i++)
            {
                ans[i] = ans[i] + ";";
            }

            return ans;
        }

        public string undoSplt(List<List<string>> users)
        {
            string ans = null;

            ans = users[0][0] + ":";
            ans = ans + users[0][1] + ":";
            ans = ans + users[0][2] + ";";

            for (int i = 1; i < users.Count - 1; i++)
            {
                ans = ans + users[i][0] + ":";
                ans = ans + users[i][1] + ":";
                ans = ans + users[i][2] + ";";
            }

            return ans;
        }

        public List<string> oneSplit(string toSplt)
        {
            List<string> temp = toSplt.Split(';').ToList();
            toSplt = temp[0];
            List<string> ans = toSplt.Split(':').ToList();
            return ans;
        }

        private void use()
        {
            List<List<string>> ts = doSplit("asd:ss:hhhhhhhhhhhhh;ww:jjjj;");

            for (int i = 0; i < ts.Count; i++)
            {
                string ot = null;
                for (int j = 0; j < ts[i].Count; j++)
                {
                    ot += ts[i][j] + " ";
                }

                MessageBox.Show(ot);
            }
        }
    }
}