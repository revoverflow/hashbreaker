﻿using System;
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

        public Cryption cryption;

        public HashBreaker() {
            InitializeComponent();

            this.cryption = new Cryption();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            this.cryption.type = comboBox1.SelectedItem.ToString();
        }

        private void button3_Click(object sender, EventArgs e) {
            textBox2.Text = textBox2.Text.Trim();
            if (textBox2.Text == "") {
                MessageBox.Show("Please enter a valid plain text!");
                return;
            }
            string selected = comboBox6.SelectedItem.ToString();
            switch (selected) {
                case "MD5":
                case "SHA-1":
                    string hash = this.cryption.encrypt(textBox2.Text, x => { }, x => { });
                    setClipboard(hash);
                    MessageBox.Show("The hash has been created !\nValue : \"" + hash + "\"\nCopied to the clipboard");
                    break;
                case "BASE64":
                    string crypted = this.cryption.toBase64(textBox2.Text);
                    setClipboard(crypted);
                    MessageBox.Show("The hash has been created !\nValue : \"" + crypted + "\"\nCopied to the clipboard");
                    break;
                default:
                    MessageBox.Show("Please select a valid option for the hash type");
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            textBox1.Text = textBox1.Text.Trim();
            if(textBox1.Text == "") {
                MessageBox.Show("Please enter a valid hash!");
                return;
            }
            string selected = comboBox1.SelectedItem.ToString();
            switch (selected) {
                case "MD5":
                case "SHA-1":
                    if (!this.cryption.checkHash(textBox1.Text)) {
                        MessageBox.Show("Please enter a valid hash!");
                        return;
                    }
                    this.cryption.decrypt(textBox1.Text,
                    new Cryption.Callback[] {
                        x => {
                            controlOptions(button1, control => control.Visible = false);
                            controlOptions(button2, control => control.Visible = true);
                            controlOptions(button3, control => control.Visible = false);
                            controlOptions(progressBar1, control => ((ProgressBar)control).Style = ProgressBarStyle.Marquee);
                        },
                        x => controlOptions(label3, control => control.Text = "Length: "+x[0]),
                        x => {
                            int d = (int) x[0];
                            string v = (string) x[1];
                            controlOptions(button1, control => control.Visible = true);
                            controlOptions(button2, control => control.Visible = false);
                            controlOptions(progressBar1, control => ((ProgressBar)control).Style = ProgressBarStyle.Blocks);
                            controlOptions(label3, control => control.Text = "Hash decrypted in " + d + "ms : \"" + v + "\"");
                            MessageBox.Show("The hash has been decrypted in " + d + " milliseconds !\nValue : \"" + v + "\"\nCopied to the clipboard");
                        },
                        x => setClipboard((string) x[0])
                    });
                    break;
                case "BASE64":
                    if (!this.cryption.checkHash(textBox1.Text)) {
                        MessageBox.Show("Please enter a valid hash!");
                        return;
                    }
                    int time = Environment.TickCount;
                    string value = this.cryption.fromBase64(textBox1.Text);
                    int delay = Environment.TickCount - time;
                    setClipboard(value);
                    controlOptions(label3, control => control.Text = "Hash decrypted in " + delay + "ms : \"" + value + "\"");
                    MessageBox.Show("The hash has been decrypted in " + delay + " milliseconds !\nValue : \"" + value + "\"\nCopied to the clipboard");
                    break;
                default:
                    MessageBox.Show("Please select a valid option for the hash type");
                    break;
            }
        }
        
        private void button2_Click(object sender, EventArgs e) {
            controlOptions(button1, control => control.Visible = true);
            controlOptions(button3, control => control.Visible = true);
            this.cryption.abortThread(0);
            progressBar1.Style = ProgressBarStyle.Blocks;
            label3.Text = "Operation aborted.";
            button2.Visible = false;
        }

        public void setClipboard(string text) {
            Clipboard.SetText(text);
        }

        public delegate void ControlCallback(Control control);

        public void controlOptions(Control control, ControlCallback callback) {
            control.BeginInvoke(new MethodInvoker(() => callback(control)));
        }

    }
}
