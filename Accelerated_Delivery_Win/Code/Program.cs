using System;
using System.IO;
using Accelerated_Delivery_Win;

namespace Accelerated_Delivery_Win
{
    static class Program
    {
        public static BaseGame Game { get; private set; }
        public static string SavePath { get; private set; }
        public static BoxCutter Cutter { get; private set; }

        public static bool Apprehensive { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length > 0 && args[0] == "-taco")
                Apprehensive = true;
#if !DEBUG
            try
            {
#endif
#if WINDOWS
                SavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Accelerated Delivery\\";
                if(!Directory.Exists(SavePath))
                    Directory.CreateDirectory(SavePath);
#elif XBOX
                SavePath = "";
#endif
                Cutter = new BoxCutter(false, false, SavePath);
                
                using(Game = new BaseGame())
                    Game.Run();
#if !DEBUG
            }
            catch(Exception ex)
            {
                using(CrashDebugGame game = new CrashDebugGame(ex, Cutter))
                    game.Run();
            }
#endif
            Cutter.Close(); 
        }
    }
}

