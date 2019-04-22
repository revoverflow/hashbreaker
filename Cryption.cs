using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace HashBreaker
{
    public class Cryption
    {

        public delegate void Callback(params object[] objects);

        public Thread[] threads;

        private int hashLength;

        public string type, method;

        private MD5 md5;
        private SHA1 sha1;

        public Cryption(int maxThreads = 1, int hashLength = 10,string type = "MD5", string method = "bruteforce") {
            if (maxThreads < 1) throw new Exception("MaxThreads can't be under 1!");

            this.threads = new Thread[maxThreads];

            this.hashLength = hashLength;

            this.type = type;
            this.method = method;

            this.md5 = MD5.Create();
            this.sha1 = SHA1.Create();
        }

        public string toBase64(string str) {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(str));
        }

        public string fromBase64(string str) {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }

        public bool checkHash(string hash) {
            hash = hash.Trim();
            if (string.IsNullOrWhiteSpace(hash)) return false;
            string pattern = "";
            if (type.Equals("MD5")) pattern = "^[a-fA-F0-9]{32}$";
            else if (type.Equals("SHA-1")) pattern = "^[a-fA-F0-9]{40}$";
            else if (type.Equals("BASE64")) return (hash.Length % 4 == 0) && Regex.IsMatch(hash, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
            return Regex.IsMatch(hash, pattern, RegexOptions.Compiled);
        }

        public string encrypt(string plain, Callback before, Callback after) {
            before();
            byte[] data = null;
            if (type.Equals("MD5")) data = md5.ComputeHash(Encoding.UTF8.GetBytes(plain));
            else if (type.Equals("SHA-1")) data = sha1.ComputeHash(Encoding.UTF8.GetBytes(plain));
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                stringBuilder.Append(data[i].ToString("x2"));
            after();
            return stringBuilder.ToString();
        }

        public void decrypt(string hash, Callback[] callbacks) {
            if (callbacks.Length < 3) throw new Exception("We need 4 callbacks in decrypt method!");
            callbacks[0]();
            this.threads[0] = new Thread(() => {
                string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
                int startTime = Environment.TickCount;
                for (int length = 1; length <= this.hashLength; ++length) {
                    callbacks[1](length);
                    StringBuilder stringBuilder = new StringBuilder(new string('a', length));
                    while (true) {
                        string value = stringBuilder.ToString();
                        string hashedVal = encrypt(value, x => { }, x => { });
                        if (hashedVal.Equals(hash)) {
                            int delay = Environment.TickCount - startTime;
                            callbacks[2](delay, value);
                            abortThread(0);
                            callbacks[3](value);
                        }
                        if (value.All(item => item == '~'))
                            break;
                        for (int i = length - 1; i >= 0; --i) {
                            if (stringBuilder[i] != '~') {
                                stringBuilder[i] = chars.ToCharArray()[chars.LastIndexOf(stringBuilder[i]) + 1];
                                break;
                            } else stringBuilder[i] = chars.ToCharArray()[0];
                        }
                    }
                }
            });
            this.threads[0].Start();
        }

        public bool abortThread(int thread) {
            if (this.threads[thread].IsAlive)
                this.threads[thread].Abort();
            return this.threads[thread].IsAlive;
        }

    }
}
