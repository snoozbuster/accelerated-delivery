using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.IO;

namespace Accelerated_Delivery_Win
{
    public class BoxCutter
    {
#if DEBUG || TRACE
        private FileStream file;
        private bool sortLogs;
        private bool overwriteLogs;
        private Dictionary<object, List<string>> sortedBuffer;
        private StreamWriter writer;
        private Default d;
        private FatalException f;
        private NonFatalException n;
#endif

        /// <summary>
        /// Initializes a new BoxCutter for debugging.
        /// </summary>
        /// <param name="sortType">True if you want the logs written to be sorted by writer, false if you want them to be written
        /// chronologically. If true, the buffer has to be flushed when the game closes; if it closes unexpectedly the file may
        /// be empty. Also, it keeps everything written in memory; this could become massive fast.</param>
        /// <param name="overwriteOldLogs">True if you want the log to save to debug.log, false if you want the date and time appended
        /// to the filename.</param>
        public BoxCutter(bool sortLogs, bool overwriteOldLogs)
        {
#if DEBUG || TRACE
            string path;
            this.sortLogs = sortLogs;
            overwriteLogs = overwriteOldLogs;
            d = new Default();
            n = new NonFatalException();
            f = new FatalException();
            if(overwriteOldLogs)
            {
                path = Program.SavePath + "debug.log";
                if(File.Exists(path))
                    file = File.Open(path, FileMode.Truncate);
                else
                    file = File.Open(path, FileMode.CreateNew);
            }
            else
            {
                IEnumerable<string> filenames = Directory.EnumerateFiles(Program.SavePath, "debug*.log");
                int debuglogs = filenames.Count<string>();
                if(debuglogs > 9)
                {
                    int logstodelete = debuglogs - 9;
                    for(int i = 0; i < logstodelete; i++)
                        try { File.Delete(filenames.ElementAt(i)); }
                        catch { }
                }
                path = Program.SavePath + "debug[" + DateTime.Now.ToShortDateString().Replace(@"/", @"-").Replace(@"\", @"-") + "][" + DateTime.Now.ToLongTimeString().Replace(@":", @".") + "].log";
                file = File.Open(path, FileMode.Create);
            }
            writer = new StreamWriter(file);
            if(sortLogs)
            {
                sortedBuffer = new Dictionary<object, List<string>>();
                sortedBuffer.Add(d, new List<string>());
                sortedBuffer.Add(n, new List<string>());
                sortedBuffer.Add(f, new List<string>());
            }
#endif
        }

        /// <summary>
        /// Writes text to a log.
        /// </summary>
        /// <param name="writer">The object that is writing the text. Use the keyword "this." Null is acceptable, it
        /// will write under the object "Default Channel." Objects that are INameable will use their name instead of
        /// object.ToString().</param>
        /// <param name="text">The text to write to the log.</param>
        public void WriteToLog(object caller, string text)
        {
#if DEBUG || TRACE
            string formattedText;
            if(!sortLogs)
            {
                if(caller != null)
                    formattedText = GetLogHeader(caller) + " " + text;
                else
                    formattedText = GetLogHeader(d) + " " + text;
            }
            else
                formattedText = GetLogHeader() + " " + text;
            if(file != null)
            {
                if(sortLogs)
                {
                    if(caller != null)
                    {
                        if(!sortedBuffer.Keys.Contains(caller))
                            sortedBuffer.Add(caller, new List<string>());
                        sortedBuffer[caller].Add(formattedText);
                    }
                    else
                        sortedBuffer[d].Add(formattedText);
                }
                else
                {
                    writer.WriteLine(formattedText);
                    writer.Flush();
                    file.Flush();
                }
            }
#endif
        }

        public void WriteExceptionToLog(Exception ex, bool fatal)
        {
#if DEBUG || TRACE
            INameable i;
            if(fatal)
                i = f;
            else
                i = n;
            string formattedText;
            if(!sortLogs)
                formattedText = GetLogHeader(f) + " Details: " + ex.Message + " Stack Trace:\n" + ex.StackTrace;
            else
                formattedText = GetLogHeader() + " " + ex.Message + " Stack Trace:\n" + ex.StackTrace;
            if(file != null)
            {
                if(sortLogs)
                {
                    sortedBuffer.Add(f, new List<string>());
                    sortedBuffer[f].Add(formattedText);
                }
                else
                {
                    writer.WriteLine(formattedText);
                    writer.Flush();
                    file.Flush();
                }
            }
#endif
        }

        public void Close()
        {
#if DEBUG || TRACE
            if(file != null)
            {
                if(sortLogs)
                {
                    foreach(object obj in sortedBuffer.Keys)
                    {
                        if(sortedBuffer[obj].Count != 0)
                        {
                            if(obj is INameable)
                                writer.WriteLine((obj as INameable).Name + ":");
                            else
                                writer.WriteLine(obj.ToString().Replace("Accelerated_Delivery_Win.", "") + ":");
                            foreach(string s in sortedBuffer[obj])
                                writer.WriteLine("    " + s);
                            writer.Flush();
                        } 
                    }
                }
                file.Flush();
                file.Close();
            }
#endif
        }

#if DEBUG || TRACE
        private string GetLogHeader(object obj)
        {
            return "[" + GetTimeWithMilliseconds() + "]" + "(" + obj.ToString().Replace("Accelerated_Delivery_Win.", "") + ")";
        }
        private string GetLogHeader()
        {
            return "[" + GetTimeWithMilliseconds() + "]";
        }
        private string GetLogHeader(INameable obj)
        {
            return "[" + GetTimeWithMilliseconds() + "]" + "(" + obj.Name + ")";
        }
        private string GetTimeWithMilliseconds()
        {
            return DateTime.Now.ToLongTimeString().Replace(" AM", ":" + GetFormattedMilliseconds() + " AM").Replace(" PM", ":" + GetFormattedMilliseconds() + " PM");
        }
        private string GetFormattedMilliseconds()
        {
            string temp = DateTime.Now.Millisecond.ToString();
            int ms = DateTime.Now.Millisecond;
            if(ms < 10)
                temp = "000" + temp;
            else if(ms < 100)
                temp = "00" + temp;
            else if(ms < 1000)
                temp = "0" + temp;
            return temp; 
        }

        private class Default : INameable
        {
            public string Name { get { return name; } }
            private string name;
            public Default()
            {
                name = "Default Channel";
            }
        }
        private class FatalException : INameable
        {
            public string Name { get { return name; } }
            private string name;
            public FatalException()
            {
                name = "FATAL EXCEPTION";
            }
        }
        private class NonFatalException : INameable
        {
            public string Name { get { return name; } }
            private string name;
            public NonFatalException()
            {
                name = "EXCEPTIONS";
            }
        }
#endif

        public interface INameable
        {
            string Name
            {
                get;
            }
        }
    }
}
