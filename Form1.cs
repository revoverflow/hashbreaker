using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Threading;

namespace HashBreaker
{

    public partial class HashBreaker : Form
    {
        public Thread t;

        public HashBreaker()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                MessageBox.Show("Please enter a valid hash.");
                return;
            }

            switch(comboBox1.SelectedItem)
            {
                case "MD5":
                    decryptMD5();
                    break;
                case "SHA-1":
                    MessageBox.Show("SHA-1 support will be added in the next update.");
                    break;
                case "AES":
                    MessageBox.Show("AES support will be added in the next update.");
                    break;
                case "BASE64":
                    MessageBox.Show("BASE64 support will be added in the next update.");
                    break;
                default:
                    MessageBox.Show("Please select a valid option for the hash type");
                    break;
            }
        }

        public void decryptMD5()
        {
            using (MD5 md5Hash = MD5.Create())
            {
                string hash = textBox1.Text;
                t = new Thread(() => BreakMD5(hash));
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 100;
                button2.Visible = true;
                t.Start();
            }
        }

        public void BreakMD5(String target)
        {
            String chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            char[] charsArray = chars.ToCharArray();

            using (MD5 md5Hash = MD5.Create())
            {
                int dwStartTime = System.Environment.TickCount;
                int charindex = 0;

                for (int length = 1; length <= 10; ++length)
                {
                    setLoadingLabel("Length : " + length);
                    StringBuilder Sb = new StringBuilder(new String('a', length));

                    while (true)
                    {
                        String value = Sb.ToString();
                        String hashedval = createMD5Hash(md5Hash, value);
                        int delay = System.Environment.TickCount - dwStartTime;

                        if (hashedval.Equals(target))
                        {
                            setButtonState(false);
                            stopLoading();
                            setLoadingLabel("Hash decrypted in " + delay + "ms : \"" + value + "\"");
                            MessageBox.Show("The hash has been decrypted in " + delay + " miliseconds !\nValue : \"" + value + "\"");
                            Thread.CurrentThread.Abort();
                        }

                        if (value.All(item => item == '~'))
                            break;

                        for (int i = length - 1; i >= 0; i--)
                        {
                            if (Sb[i] != '~')
                            {
                                Sb[i] = (Char)charsArray[chars.LastIndexOf(Sb[i]) + 1];
                                break;
                            }
                            else
                                Sb[i] = (Char)charsArray[0];
                        }
                    }
                }
            }
        }

        void setLoadingLabel(String tested)
        {
            label3.BeginInvoke(new MethodInvoker(() =>
            {
                label3.Text = tested;
            }));
        }

        void setButtonState(bool state)
        {
            button2.BeginInvoke(new MethodInvoker(() =>
            {
                button2.Visible = state;
            }));
        }

        void stopLoading()
        {
            progressBar1.BeginInvoke(new MethodInvoker(() =>
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
            }));
        }

        static string createMD5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            t.Abort();
            progressBar1.Style = ProgressBarStyle.Blocks;
            label3.Text = "Operation aborted.";
            button2.Visible = false;
        }
    }
}
