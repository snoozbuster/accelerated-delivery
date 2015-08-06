using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Downloader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if(args[0] != "n00b")
            {
                Console.Error.WriteLine("Update failed; game can still be launched but stability is not assured.");
                Application.Exit();
            }

            string[] args2 = new string[args.Length - 1];
            for(int i = 0; i < args2.Length; i++)
                args2[i] = args[i + 1];

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Downloader(args2));
        }
    }
}
