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
using System.IO;
using System.Text.RegularExpressions;

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
            string selected = comboBox1.SelectedItem.ToString();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = textBox2.Text.Trim();
            if (textBox2.Text == "")
            {
                MessageBox.Show("Please enter a valid plain text.");
                return;
            }
            string selected = comboBox6.SelectedItem.ToString();
            switch (selected)
            {
                case "MD5":
                case "SHA-1":
                    MD5 md5Hash = MD5.Create();
                    SHA1 sha1Hash = SHA1.Create();
                    String hashedval = selected.Equals("MD5") ? createHash(md5Hash, textBox2.Text) : createHash(sha1Hash, textBox2.Text);
                    setClipboard(hashedval);
                    MessageBox.Show("The hash has been created !\nValue : \"" + hashedval + "\"\nCopied to the clipboard");
                    break;
                case "BASE64":
                    string crypted = Convert.ToBase64String(Encoding.ASCII.GetBytes(textBox2.Text));
                    setClipboard(crypted);
                    MessageBox.Show("The hash has been created !\nValue : \"" + crypted + "\"\nCopied to the clipboard");
                    break;
                default:
                    MessageBox.Show("Please select a valid option for the hash type");
                    break;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Trim();
            if(textBox1.Text == "")
            {
                MessageBox.Show("Please enter a valid hash.");
                return;
            }
            string selected = comboBox1.SelectedItem.ToString();
            switch (selected)
            {
                case "MD5":
                case "SHA-1":
                    if (!checkHash(selected, textBox1.Text)) {
                        MessageBox.Show("Please enter a valid hash!");
                        return;
                    }
                    disableButton(true);
                    decrypt(selected);
                    break;
                case "BASE64":
                    if (!checkHash(selected, textBox1.Text)) {
                        MessageBox.Show("Please enter a valid hash!");
                        return;
                    }
                    int time = Environment.TickCount;
                    string value = Encoding.UTF8.GetString(Convert.FromBase64String(textBox1.Text));
                    int delay = Environment.TickCount - time;
                    setClipboard(value);
                    setLoadingLabel("Hash decrypted in " + delay + "ms : \"" + value + "\"");
                    MessageBox.Show("The hash has been decrypted in " + delay + " milliseconds !\nValue : \"" + value + "\"\nCopied to the clipboard");
                    break;
                default:
                    MessageBox.Show("Please select a valid option for the hash type");
                    break;
            }
        }

        public void setClipboard(string text) {
            Clipboard.SetText(text);
        }

        public bool checkHash(string type, string hash) {
            hash = hash.Trim();
            if (string.IsNullOrWhiteSpace(hash)) return false;
            string pattern = "";
            if (type.Equals("MD5")) pattern = "^[a-fA-F0-9]{32}$";
            else if (type.Equals("SHA-1")) pattern = "^[a-fA-F0-9]{40}$";
            else if (type.Equals("BASE64")) return (hash.Length % 4 == 0) && Regex.IsMatch(hash, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
            return Regex.IsMatch(hash, pattern, RegexOptions.Compiled);
        }

        public void decrypt(string type)
        {
            string hash = textBox1.Text;
            t = new Thread(() => breakHash(hash, type));
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 100;
            button2.Visible = true;
            t.Start();
        }

        public void breakHash(String target, string type)
        {
            String chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            char[] charsArray = chars.ToCharArray();

            SHA1 sha1Hash = SHA1.Create();
            MD5 md5Hash = MD5.Create();

            int dwStartTime = Environment.TickCount;
            int charindex = 0;

            for (int length = 1; length <= 10; ++length)
            {
                setLoadingLabel("Length : " + length);
                StringBuilder Sb = new StringBuilder(new String('a', length));

                while (true)
                {
                    String value = Sb.ToString();
                    String hashedval = type.Equals("MD5") ? createHash(md5Hash, value) : createHash(sha1Hash, value);
                    int delay = Environment.TickCount - dwStartTime;

                    if (hashedval.Equals(target))
                    {
                        setButtonState(false);
                        stopLoading();
                        setLoadingLabel("Hash decrypted in " + delay + "ms : \"" + value + "\"");
                        MessageBox.Show("The hash has been decrypted in " + delay + " milliseconds !\nValue : \"" + value + "\"\nCopied to the clipboard");
                        disableButton(false);
                        Thread.CurrentThread.Abort();
                        setClipboard(value);
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

        public void disableButton(bool state)
        {
            button1.BeginInvoke(new MethodInvoker(() =>
            {
                button1.Visible = !state;
            }));
        }

        void stopLoading()
        {
            progressBar1.BeginInvoke(new MethodInvoker(() =>
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
            }));
        }

        static string createHash(object hash, string input)
        {
            byte[] data = null;
            if (hash is MD5) data = ((MD5)hash).ComputeHash(Encoding.UTF8.GetBytes(input));
            if (hash is SHA1) data = ((SHA1)hash).ComputeHash(Encoding.UTF8.GetBytes(input));
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
            disableButton(false);
            t.Abort();
            progressBar1.Style = ProgressBarStyle.Blocks;
            label3.Text = "Operation aborted.";
            button2.Visible = false;
        }
    }
}
