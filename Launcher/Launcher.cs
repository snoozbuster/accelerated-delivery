using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Threading;

namespace Launcher
{
    public partial class Launcher : Form
    {
        bool previouslyLoggedIn;
        int seconds = 5;
        string regUser, regPass;

        protected int majorNumber = 0;
        protected int minorNumber = 0;
        protected int majorUpdatedTo = 0;
        protected int minorUpdatedTo = 0;
        protected string versionNumber { get { return majorNumber + "." + minorNumber; } }

        StringBuilder error;
        StringBuilder output;

        Process proc;

        public Launcher()
        {
            InitializeComponent();
            this.Visible = false;
            RegistryKey k1 = null, k2 = null;
            try
            {
                using(k1 = Registry.CurrentUser.OpenSubKey("Software", true))
                    using(k2 = k1.CreateSubKey("Accelerated Delivery"))
                    {
                        regUser = (string)k2.GetValue("username");
                        if(regUser != null)
                            username.Text = regUser;
                        regPass = (string)k2.GetValue("password", "");
                        if(regPass != null)
                            password.Text = regPass;
                        if(regUser != null && regPass != null)
                            previouslyLoggedIn = true;
                    }
            }
            catch { }
            try
            {
                using(k1 = Registry.CurrentUser.OpenSubKey("Software", true))
                    using(k2 = k1.CreateSubKey("Accelerated Delivery"))
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
            }
            catch
            {
                info.Text = "Could not read the version number to attempt to update. Stop messing with the registry.";
                info.Visible = true;
                login.Enabled = true;
                majorNumber = minorNumber = 0;
            }
        }

        private void login_Click(object sender, EventArgs e)
        {
            login.Enabled = false;
            info.Visible = true;
            info.Text = "Logging in...";

            Regex r = new Regex("^[a-zA-Z0-9]*$");
            if(r.IsMatch(username.Text) && r.IsMatch(password.Text))
            {
                try
                {
                    //if(username.Text == "test" && password.Text == "test")
                    //{
                    //    StartGame();
                    //    return;
                    //}

                    string message = "An unexpected error occurred. Please try again later.";
                    using(WebClient client = new WebClient())
                        message = Encoding.UTF8.GetString(client.UploadValues("http://www.accelerateddeliverygame.com/assets/ll5.php", new NameValueCollection() { { "u", username.Text }, { "p", password.Text } }));

                    //WebRequest request = WebRequest.Create("http://www.accelerateddeliverygame.com/assets/ll5.php");
                    //string data = "u=" + username.Text + "&p=" + password.Text;
                    //request.Method = WebRequestMethods.Http.Post;
                    //byte[] bytes = Encoding.ASCII.GetBytes(data);
                    //request.ContentLength = bytes.Length;
                    //request.ContentType = "application/x-www-form-urlencoded";

                    //using(Stream writer = request.GetRequestStream())
                    //{
                    //    writer.Write(bytes, 0, bytes.Length);
                    //    writer.Flush();
                    //}

                    //message = "An unexpected error occurred. Please try again later.";
                    //using(WebResponse re = request.GetResponse())
                    //    using(StreamReader response = new StreamReader(re.GetResponseStream()))
                    //        message = response.ReadToEnd();
                    
                    if(message != "success")
                    {
                        login.Enabled = true;
                        info.Visible = true;
#if DEBUG
                        info.Text = message;
#else
                        if(!previouslyLoggedIn || message == "Username or password were invalid.")
                            info.Text = "An error occurred connecting to the server. Please try again.";
                        else if(username.Text == regUser && password.Text == regPass)
                        {
                            timer.Start();
                            info.Text = "An error occurred connecting to the server. Starting in offline mode in " + seconds + " seconds.";
                        }
#endif
                        return;
                    }
                    
                    StartGame();
                }
                catch
                {
                    login.Enabled = true;
                    info.Visible = true;
                    if(!previouslyLoggedIn)
                        info.Text = "An error occurred connecting to the server. Please try again.";
                    else if(username.Text == regUser && password.Text == regPass)
                    {
                        timer.Start();
                        info.Text = "An error occurred connecting to the server. Starting in offline mode in " + seconds + " seconds.";
                    }
                }
            }
            else
            {
                login.Enabled = true;
                info.Visible = true;
                info.Text = "Invalid username or password.";
            }
        }

        private void register_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.accelerateddeliverygame.com");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            seconds--;
            if(seconds != 0)
                info.Text = "An error occurred connecting to the server. Starting in offline mode in " + seconds + " seconds.";
            else
            {
                timer.Stop();
                StartGame();
            }
        }

        private void StartGame()
        {
            info.Text = "Launching...";
            info.Visible = true;

            RegistryKey k = Registry.CurrentUser.CreateSubKey("inf_data_AD", RegistryKeyPermissionCheck.ReadWriteSubTree);
            k.SetValue("potato", 1, RegistryValueKind.DWord);
            k.Close();
            k = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey sub = k.CreateSubKey("Accelerated Delivery");
            sub.SetValue("username", username.Text);
            sub.SetValue("password", password.Text);
            sub.Close();
            k.Close();
//            string temp = "../Accelerated_Delivery_Win/bin/x86/" + 
//#if DEBUG
//            "Debug"
//#else
//            "Release"
//#endif
//            + "/Accelerated_Delivery_Win.exe";
            if(File.Exists("Accelerated_Delivery_Win.exe"))
                Process.Start("Accelerated_Delivery_Win.exe", "-taco");
            //else if(File.Exists(temp))
            //    Process.Start(temp, "-taco");
            else
            {
                info.Visible = true;
                info.Text = "Could not find the main executable.";
                login.Enabled = true;
                return;
            }
            Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            // check for XNA 4.0
            try
            {
                RegistryKey k = Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey(".NETFramework").OpenSubKey("policy").OpenSubKey("v4.0");
                k.Dispose();
            }
            catch
            {
                MessageBox.Show(this, "You probably don't have the XNA Framework 4.0 Refresh installed. How this happened, we're not sure, but please install it. Thanks!", "Missing XNA 4.0", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            if(File.Exists("dnew.exe"))
            {
                File.Copy("dnew.exe", "downloader.exe", true);
                File.Delete("dnew.exe");
            }
            if(File.Exists("lnew.exe"))
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.FileName = "downloader.exe";
                processInfo.Verb = "runas";
                //string path = Application.ExecutablePath.Replace('\\', '/');
                processInfo.Arguments = "n00b " + "l " + Process.GetCurrentProcess().Id + " " + Application.ExecutablePath;
                //processInfo.WorkingDirectory = "./";
                processInfo.UseShellExecute = true;
                //processInfo.RedirectStandardError = true;
                //processInfo.RedirectStandardOutput = true;
                //processInfo.CreateNoWindow = true;

                try
                {
                    proc = new Process();
                    //proc.EnableRaisingEvents = true;
                    proc.StartInfo = processInfo;
                    //proc.ErrorDataReceived += (o, e2) => error.Append(e2.Data);
                    //proc.OutputDataReceived += (o, e2) => output.Append(e2.Data);
                    proc.Start();
                    //proc.BeginErrorReadLine();
                    //proc.BeginOutputReadLine();
                    //proc.Exited += downloaderExited2;               
                }
                catch
                {
                    info.Text = "Administrative priviledges are required to finish. Please try again by restarting the launcher.";
                    info.Visible = true;
                    login.Enabled = true;
                }
            }
            this.Visible = true;
            if(!(majorNumber == 0 && minorNumber == 0))
            {
                try
                {
                    string message = "An unexpected error occurred while updating. Please try again later.";
                    using(WebClient client = new WebClient())
                        message = Encoding.UTF8.GetString(client.UploadValues("http://www.accelerateddeliverygame.com/assets/version.php", new NameValueCollection() { { "major", majorNumber.ToString() }, { "minor", minorNumber.ToString() } }));

                    //WebRequest request = WebRequest.Create("http://www.accelerateddeliverygame.com/assets/version.php");
                    //string data = "u=" + username.Text + "&p=" + password.Text;
                    //request.Method = WebRequestMethods.Http.Post;
                    //byte[] bytes = Encoding.ASCII.GetBytes(data);
                    //request.ContentLength = bytes.Length;
                    //request.ContentType = "application/x-www-form-urlencoded";

                    //using(Stream writer = request.GetRequestStream())
                    //{
                    //    writer.Write(bytes, 0, bytes.Length);
                    //    writer.Flush();
                    //}

                    if(message != "fully updated")
                    {
                        message = message.Replace('.', '_');

                        string[] lines = message.Split('\n');
                        List<string> temp = lines.ToList();
                        temp.RemoveAt(temp.Count - 1);
                        lines = temp.ToArray();
                        string[] current_comp = lines[lines.Length - 1].Split('_');
                        majorUpdatedTo = int.Parse(current_comp[0]);
                        minorUpdatedTo = int.Parse(current_comp[1]);

                        login.Enabled = false;
                        info.Text = "Update detected; downloading.";
                        info.Visible = true;

                        Thread.Sleep(1000);

                        error = new StringBuilder();
                        output = new StringBuilder();

                        ProcessStartInfo processInfo = new ProcessStartInfo();
                        processInfo.Verb = "runas";
                        processInfo.FileName = "Downloader.exe";
                        processInfo.Arguments = "n00b " + message;
                        processInfo.WorkingDirectory = "./";
                        processInfo.UseShellExecute = false;
                        processInfo.RedirectStandardError = true;
                        processInfo.RedirectStandardOutput = true;
                        //processInfo.CreateNoWindow = true;

                        try
                        {
                            proc = new Process();
                            proc.EnableRaisingEvents = true;
                            proc.StartInfo = processInfo;
                            proc.ErrorDataReceived += (o, e2) => error.Append(e2.Data);
                            proc.OutputDataReceived += (o, e2) => output.Append(e2.Data);
                            proc.Start();
                            proc.BeginErrorReadLine();
                            proc.BeginOutputReadLine();
                            proc.Exited += downloaderExited;
                        }
                        catch
                        {
                            info.Text = "Administrative priviledges are required to update. Please try again by restarting the launcher.";
                            info.Visible = true;
                            login.Enabled = true;
                        }
                    }
                }
                catch
                { }
            }

            base.OnLoad(e);
        }

        private void downloaderExited2(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void downloaderExited(object sender, EventArgs e)
        {
            info.Invoke(new Action(delegate { info.Visible = true; }));
            if(output.ToString() == "success" || output.ToString() == "new launcher")
            {
                if(output.ToString() == "new launcher")
                { 
                    MessageBox.Show(this, "The launcher has been updated and will now restart.", "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                    Application.Restart();
                }
                else
                    info.Invoke(new Action(delegate { info.Text = "Update successful!"; }));
                RegistryKey k1, k2;
                using(k1 = Registry.CurrentUser.OpenSubKey("Software", true))
                using(k2 = k1.CreateSubKey("Accelerated Delivery"))
                {
                    k2.SetValue("MajorNumber", majorUpdatedTo);
                    k2.SetValue("MinorNumber", minorUpdatedTo);
                }
            }
            else
                info.Invoke(new Action(delegate { info.Text = output.ToString(); }));

            login.Invoke(new Action(delegate { login.Enabled = true; }));
            if(proc != null)
                proc.Dispose();
        }
    }
}
