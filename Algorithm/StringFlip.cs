//三步翻转字符串 C#代码实现
//简单原理 字符串XY 当X`T Y`T各部分翻转 整体（X`T Y`T）`T 得到结果字符串 YX

using System;

namespace Algo
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "I am a Student.";
            string[] strArr = str.Split(new char[] { ' '});
            string res = "";
            foreach (string s in strArr)
            {
                //逐个翻转
                res = res + " "+Swtich(s.ToCharArray(), 0, s.Length - 1);
            }
            //整体翻转
            res = Swtich(res.Trim().ToCharArray(), 0, res.Trim().Length - 1);
            Console.WriteLine(res);
            Console.Read();
        }

        public static string Swtich(char[] carr, int start,int end)
        {
            while (start < end)
            {
                char c = carr[start];
                carr[start] = carr[end];
                carr[end] = c;
                start++;
                end--;
            }
            string s = new string(carr);
            return s;
        }
    }
}
