using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Launcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Screen[] screens = Screen.AllScreens;
            bool taco = true;
            foreach(Screen s in screens)
                if(s.Bounds.Width >= 800 && s.Bounds.Height >= 600)
                {
                    taco = false;
                    break;
                }
            if(taco)
            {
                MessageBox.Show("Your screen resolution is not supported. Resolutions must be at least 800x600.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new Launcher());
        }
    }
}
