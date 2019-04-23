using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace HashBreaker
{
    public class Utils
    {

        public Utils() {
            throw new Exception("Cannot create instance of Utils.cs");
        }

        public static string reverse(string str) {
            return new string(str.Reverse().ToArray());
        }

        public static string addIndexes(int at, string plain, string chain) {
            StringBuilder stringBuilder = new StringBuilder(plain);
            for (int i = at; i < stringBuilder.Length; i += (at + chain.Length))
                stringBuilder.Insert(i, chain);
            return stringBuilder.ToString();
        }

        private static Random random = new Random();
        public static string randomStr(int length) {
            string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
