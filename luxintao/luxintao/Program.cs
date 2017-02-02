using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace luxintao
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] Args)//加入参数
        {
            string s = "";
            if (Args.Length > 0) s = Args[0];
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new lxt_form1(s));
        }
    }
}
