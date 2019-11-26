using System.Linq;

namespace Генератор_баз
{
    public static class Funcs
    {
        public static string delNumbers(string s)
        {
            string res = "";
            foreach (char c in s)
                if (!inRange(48, 57, c))
                    res += c;
            return res;
        }

        public static string delChars(string s)
        {
            string res = "";
            foreach (char c in s)
                if (inRange(48, 57, c))
                    res += c;
            return res;
        }

        public static string delSpecChars(string s)
        {
            string res = "";
            foreach (char c in s)
                if (inRange(33, 47, c) || inRange(58, 64, c) || inRange(91, 96, c) || inRange(123, 126, c)) continue;
            else
                res += c;
            return res;
        }

        private static bool inRange(int min, int max, int value) => value >= min && value <= max;

        public static string toUp(string s) => s.ToUpper();

        public static string toLower(string s) => s.ToLower();

        public static string reverse(string s) => string.Join("", s.ToCharArray().Reverse());
    }
}
