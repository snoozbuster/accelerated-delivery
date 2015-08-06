using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;

namespace Downloader
{
    public partial class Downloader : Form
    {
        protected string[] allVersions;
        protected int versionDownloading = 0;
        protected WebClient c;
        protected Queue<string> fileListQueue = new Queue<string>();
        protected Queue<string> individualFileQueue = new Queue<string>();
        protected Queue<string> fileTargetQueue = new Queue<string>();

        protected int majorNumber, minorNumber;

        public Downloader(string[] allVersions)
        {
            InitializeComponent();
            this.Visible = false;
            if(allVersions[0] == "l")
            {
                bool finished = false;
                //try
                //{
                Process.GetProcessById(int.Parse(allVersions[1])).Kill();
                bool done = false;
                while(!done)
                {
                    bool found = false;
                    foreach(Process p in Process.GetProcesses())
                        if(p.Id == int.Parse(allVersions[1]))
                        {
                            Thread.Sleep(1000);
                            found = true;
                        }
                    done = !found;
                }
                //}
                //catch(ArgumentException e) // 
                //{
                //if(!e.Message.Contains("Process with an Id of")) // this is the message we're looking for, it means our PID is gone.
                //    throw e;
                string path = "";
                try
                {
                    for(int i = 2; i < allVersions.Length; i++)
                        path += allVersions[i] + " ";

                    File.Delete(path);
                    File.Copy("lnew.exe", path);
                    File.Delete("lnew.exe");
                    finished = true;
                }
                catch(Exception e) { MessageBox.Show(e.Message + "\n" + e.StackTrace); }

                if(finished)
                    System.Diagnostics.Process.Start(path);
                Application.Exit();
                //}
            }
            else if(allVersions[0] != "n00b")
                Application.Exit();

            this.allVersions = new string[allVersions.Length - 1];
            for(int i = 0; i < this.allVersions.Length; i++)
                this.allVersions[i] = allVersions[i + 1];
            c = new WebClient();

            using(RegistryKey k1 = Registry.CurrentUser.OpenSubKey("Software", true)) // acquire version info
                using(RegistryKey k2 = k1.CreateSubKey("Accelerated Delivery"))
                {
                    var major = k2.GetValue("MajorNumber");
                    if(major == null)
                    {
                        k2.SetValue("MajorNumber", 1);
                        majorNumber = 1;
                    }
                    else
                        majorNumber = (int)major;
                    var minor = k2.GetValue("MinorNumber");
                    if(minor == null)
                    {
                        k2.SetValue("MinorNumber", 0);
                        minorNumber = 0;
                    }
                    else
                        minorNumber = (int)minor;
                }

            foreach(string s in allVersions.Reverse())
            {
                int major, minor;
                major = int.Parse(s.Substring(0, 1));
                minor = int.Parse(s.Substring(2, 1));
                if(major > majorNumber || (major == majorNumber && minor > minorNumber))
                    fileListQueue.Enqueue("http://www.accelerateddeliverygame.com/assets/" + s + "/list.txt");
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            try
            {
                downloadFileList();
            }
            catch
            {
                Console.Error.WriteLine("Update failed; game can still be launched but stability is not assured.");
                c.Dispose();
                Application.Exit();
            }
            base.OnLoad(e);
        }

        private void downloadFileList()
        {
            if(fileListQueue.Any())
            {
                c.DownloadFileCompleted += downloadFileListCompleted;

                var url = fileListQueue.Dequeue();

                c.DownloadFileAsync(new Uri(url), "list.txt");
                return;
            }

            if(File.Exists("lnew.exe"))
                Console.Write("new launcher");
            else
                Console.Write("success");

            c.Dispose();
            Application.Exit();
        }

        private void downloadFileListCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if(e.Cancelled || e.Error != null)
            {
                Console.Error.WriteLine("Update failed; game can still be launched but stability is not assured.");
                c.Dispose();
                Application.Exit();
            }

            c.DownloadFileCompleted -= downloadFileListCompleted;
            bool error = false;
            try
            {
                StreamReader fileList = new StreamReader("list.txt");
                string data = fileList.ReadToEnd();
                fileList.Close();
                string[] lines = data.Split('\n');

                foreach(string s in lines)
                {
                    string[] temp = s.Split(' ');
                    individualFileQueue.Enqueue("http://www.accelerateddeliverygame.com/assets/" + allVersions[versionDownloading] + "/" + temp[0]);
                    fileTargetQueue.Enqueue("./" + temp[1]);
                }

                c.DownloadFileCompleted += individualDownloadCompleted;
                downloadIndividualFile();
            }
            catch
            {
                Console.Error.WriteLine("Update failed; game can still be launched but stability is not assured.");
                error = true;
            }
            finally
            {
                try { File.Delete("list.txt"); }
                catch { }
                finally { if(error) { c.Dispose(); Application.Exit(); } }
            }
        }

        private void downloadIndividualFile()
        {
            if(individualFileQueue.Any())
            {
                string url = individualFileQueue.Dequeue();
                string target = fileTargetQueue.Dequeue();

                // location_on_server_relative_to_path location_on_target_relative_to_executable_location
                // if "location_on_server" is "delete", delete local file defined by "location_on_local_machine"
                if(url == "delete")
                    File.Delete(target);
                else
                    c.DownloadFileAsync(new Uri(url), target);
                return;
            }

            c.DownloadFileCompleted -= individualDownloadCompleted;
            downloadFileList();
        }

        private void individualDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if(e.Cancelled || e.Error != null)
            {
                Console.Error.WriteLine("Update failed; game can still be launched but stability is not assured.");
                c.Dispose();
                Application.Exit();
            }
            downloadIndividualFile();
        }
    }
}
