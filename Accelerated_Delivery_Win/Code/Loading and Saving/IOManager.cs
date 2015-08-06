using System;
using System.IO;
using System.Xml.Serialization;
using System.Security;
using System.Net;
using Microsoft.Win32;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using Accelerated_Delivery_Win;
using System.Threading;
using System.Windows.Forms;

namespace Accelerated_Delivery_Win
{
    public class IOManager : IInputManager
    {
        public event Action OnSaveChanged;
        public event Action<int> OnSaveDeleted;

        public bool IsSaving { get; private set; }

        //public delegate void DeleteDelegate(int slot);

        public bool SaveLoaded { get; private set; }
        private SaveData SaveGame1;
        private SaveData SaveGame2;
        private SaveData SaveGame3;
        private int currentSave = -1;
        public SaveData CurrentSave 
        { 
            get 
            {
                switch(currentSave)
                {
                    case 0: return SaveGame1;
                    case 1: return SaveGame2;
                    case 2: return SaveGame3;
                    default: throw new ArgumentException("No such save game exists.");
                }
            }
        }
        public WindowsOptions CurrentSaveWindowsOptions
        {
            get
            {
                return CurrentSave.Options;
            }
        }
        public XboxOptions CurrentSaveXboxOptions
        {
            get
            {
                return CurrentSave.Xoptions;
            }
        }
        private XmlSerializer saveDataSerializer;

#if XBOX360
        private IAsyncResult result;
        private StorageContainer container;
        private StorageDevice device;
        private bool initializing;
#endif
        private Stream currentStream;
        private Stream backupStream;
        private readonly string filename;
        private readonly string backupName;
        private readonly string username;
        private readonly byte[] key;
        private SaveFile file;
        public bool SuccessfulLoad { get; private set; }
        public bool FullScreen { get { return file.Fullscreen; } set { file.Fullscreen = value; } }

        public int CurrentSaveNumber { get { return currentSave + 1; } }

        private object lockingObject = new object();

        public IOManager()
        {
#if XBOX360
            initializing = true;

            try
            {
                result = StorageDevice.BeginShowSelector(null, null);
                result.AsyncWaitHandle.WaitOne();
                device = StorageDevice.EndShowSelector(result);
                result.AsyncWaitHandle.Close();
            }
            catch
            { }

            try
            {
                result = device.BeginOpenContainer("Accelerated Delivery", null, null);
                result.AsyncWaitHandle.WaitOne();
                container = device.EndOpenContainer(result);
                result.AsyncWaitHandle.Close();
            }
            catch 
            { }
#elif WINDOWS
            #region Save Downloading
            try
            {
                RegistryKey k1 = null, k2 = null;
                bool uploadFailed = false;
                string password = null;
#if INDIECITY
                password = "0x32614baff";
                username = "0x005bef124";
                filename = Program.SavePath + "IndieCity.sav";
                backupName = Program.SavePath + "IndieCity.bak";
#else
                using (k1 = Registry.CurrentUser.OpenSubKey("Software", true))
                    using(k2 = k1.CreateSubKey("Accelerated Delivery"))
                    {
                        username = (string)k2.GetValue("username");
                        password = (string)k2.GetValue("password");
#endif
#if !INDIECITY
                        filename = Program.SavePath + username + ".sav";
                        backupName = Program.SavePath + username + ".bak";
                        try { uploadFailed = bool.Parse((string)k2.GetValue("hydroxide", false)); }
                        catch { uploadFailed = false; }
                    }
#endif

                HashAlgorithm a = new SHA256Managed();
                byte[] n = Encoding.UTF8.GetBytes(username);
                byte[] salt = a.ComputeHash(n);
                byte[] ps = Encoding.UTF8.GetBytes(password);
                byte[] pre = new byte[salt.Length + ps.Length];
                ps.CopyTo(pre, 0);
                salt.CopyTo(pre, ps.Length);
                key = a.ComputeHash(pre);

#if !INDIECITY
                if(!uploadFailed)
                {
                    //SecureString p = new SecureString();
                    //p.AppendChar('p'); p.AppendChar('s'); p.AppendChar('q'); p.AppendChar('1'); p.AppendChar('Z'); p.AppendChar('f');
                    //p.AppendChar('J'); p.AppendChar('i'); p.AppendChar('I');
                    //p.MakeReadOnly();

                    //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(@"ftp://accelerateddeliverygame.com/assets/saves/" + username + "/ADsave.sav");
                    //request.Method = WebRequestMethods.Ftp.DownloadFile;
                    //request.Credentials = new NetworkCredential("adsave@twobuttoncrew.com", p);

                    //FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                    WebClient c = new WebClient();
                    byte[] contents = c.UploadValues("http://www.accelerateddeliverygame.com/assets/download_save.php",
                        new System.Collections.Specialized.NameValueCollection() { { "u", username }, { "p", password } });

                    //Stream responseStream = response.GetResponseStream();
                    //StreamReader reader = new StreamReader(responseStream);

                    StreamWriter write;// = new StreamWriter(File.Open(backupName, FileMode.Create, FileAccess.Write));
                    //byte[] contents;
                    //List<byte> bytes = new List<byte>();
                    //while(true)
                    //{
                    //    // read byte-by-byte
                    //    int data = reader.BaseStream.ReadByte();
                    //    if(data == -1)
                    //        break;
                    //    bytes.Add((byte)data);
                    //}
                    //contents = bytes.ToArray(); // convert to array
                    //reader.BaseStream.Read(contents, 0, (int)contents.Length); // reads file into array
                    //write.BaseStream.Write(contents, 0, (int)bytes); // writes file to backup
                    //write.Close();
                    write = new StreamWriter(File.Open(filename, FileMode.Create, FileAccess.Write)); // opens primary
                    write.BaseStream.Write(contents, 0, (int)contents.Length); // writes file to primary
                    write.Close();
                    //reader.Close();
                    //responseStream.Close();
                }
#endif
            }
            catch(Exception e) { Program.Cutter.WriteExceptionToLog(e, false); }
            #endregion
#endif
            saveDataSerializer = new XmlSerializer(typeof(SaveFile));
            //saveDataSerializer.UnknownAttribute += new XmlAttributeEventHandler(doPoop);
            //saveDataSerializer.UnknownElement += new t(doPoop);
            //saveDataSerializer.UnknownNode += new XmlNodeEventHandler(doPoop);
            //saveDataSerializer.UnreferencedObject += new UnreferencedObjectEventHandler(doPoop);

            file = new SaveFile();
            OnSaveChanged += delegate { Input.SetOptions(CurrentSaveWindowsOptions, CurrentSaveXboxOptions); };

            try { Load(); SuccessfulLoad = true; }
            catch
            {
                SuccessfulLoad = false;
#if WINDOWS
                New();
#endif
            }
            finally { if(CurrentSaveWindowsOptions != null && CurrentSaveXboxOptions != null) Input.SetOptions(CurrentSaveWindowsOptions, CurrentSaveXboxOptions); else throw new Exception("Options are null, crash imminent."); } 
#if XBOX360
            initializing = false;
#endif
        }

        //void doPoop(object sender, UnreferencedObjectEventArgs e)
        //{
        //    Console.WriteLine(e);
        //}

        //void doPoop(object sender, XmlNodeEventArgs e)
        //{
        //    Console.WriteLine(e);
        //}

        //void doPoop(object sender, XmlElementEventArgs e)
        //{
        //    Console.WriteLine(e);
        //}

        //void doPoop(object sender, XmlAttributeEventArgs e)
        //{
        //    Console.WriteLine(e);
        //}

        ~IOManager()
        {
            if(currentStream != null)
                currentStream.Close();
            if(backupStream != null)
                backupStream.Close();
        }

        /// <summary>
        /// Creates a new save file.
        /// </summary>
        public void New()
        {
            file.save1 = SaveData.GetEmptyData(1);
            file.save2 = SaveData.GetEmptyData(2);
            file.save3 = SaveData.GetEmptyData(3);

            SaveGame1 = SaveData.GetEmptyData(1);
            SaveGame2 = SaveData.GetEmptyData(2);
            SaveGame3 = SaveData.GetEmptyData(3);

            Program.Cutter.WriteToLog(this, "Attempting to create new save file.");
#if XBOX360
            try
            {
                int? initialResult;
                do
                {
                    initialResult = SimpleMessageBox.ShowMessageBox("No Options Found", "Please select a location to save options.",
                        new string[] { "Okay", "Continue Without Saving" }, 0, MessageBoxIcon.Alert);
                    switch(initialResult)
                    {
                        case -1:
                        case 1: returnOptionsToDefault();
                            return;
                        case 0: StorageDevice.BeginShowSelector((PlayerIndex)Program.Game.MessagePad, storageSelectEnd, null);
                            break;
                    }
                } while(initialResult == null);

                if(!container.FileExists(filename))
                    currentStream = container.CreateFile(filename);
                else
                    currentStream = container.OpenFile(filename, FileMode.Open);
                
                if(!container.FileExists(backupName))
                    backupStream = container.CreateFile(backupName);
                else
                    backupStream = container.OpenFile(backupName, FileMode.Create);

                saveDataSerializer.Serialize(backupStream, file);
                HandleEncryption(ref backupStream);
                backupStream.Seek(0, SeekOrigin.Begin);
                backupStream.CopyTo(currentStream);

                backupStream.Close();
                currentStream.Close();
                container.Dispose();
            }
            catch(Exception ex)
            {
                if(backupStream != null)
                    backupStream.Close();
                if(currentStream != null)
                    currentStream.Close();
                if(container != null)
                    container.Dispose();
                throw ex;
            }
#elif WINDOWS
            try
            {
                if(!File.Exists(filename))
                    currentStream = File.Create(filename);
                else
                    currentStream = File.Open(filename, FileMode.Open);
                if(!File.Exists(backupName))
                    backupStream = File.Create(backupName);
                else
                    backupStream = File.Open(backupName, FileMode.Open);

                saveDataSerializer.Serialize(backupStream, file);
                HandleEncryption(ref backupStream);

                backupStream.Seek(0, SeekOrigin.Begin);
                backupStream.CopyTo(currentStream);

                backupStream.Close();
                currentStream.Close();
            }
            catch(Exception ex)
            {
                if(backupStream != null)
                    backupStream.Close();
                if(currentStream != null)
                    currentStream.Close();

                Program.Cutter.WriteToLog(this, "Creation failed.");
                throw ex;
            }
#endif
            Program.Cutter.WriteToLog(this, "Creation successful.");
            currentSave = 0;
        }

        /// <summary>
        /// Saves the options to a file.
        /// </summary>
        public void Save(bool uploadAsync)
        {
            file.save1 = SaveGame1;
            file.save2 = SaveGame2;
            file.save3 = SaveGame3;
            file.currentSaveGame = currentSave;
            file.Fullscreen = RenderingDevice.GDM.IsFullScreen;

            Program.Cutter.WriteToLog(this, "Attempting to save game data.");
#if XBOX360
            try
            {
                if(!container.FileExists(filename) && !container.FileExists(backupName))
                    return;

                currentStream = container.CreateFile(filename);
                backupStream = container.CreateFile(backupName);

                saveDataSerializer.Serialize(backupStream, file);
                HandleEncryption(ref backupStream);
                
                backupStream.Seek(0, SeekOrigin.Begin);
                backupStream.CopyTo(currentStream);
                
                backupStream.Close();
                currentStream.Close();
                container.Dispose();
            }
            catch(Exception ex)
            { 
                if(backupStream != null)
                    backupStream.Close();
                if(currentStream != null)
                    currentStream.Close();
                if(container != null)
                    container.Dispose();
                throw ex;
            }
#elif WINDOWS
            try
            {
                if(!File.Exists(filename) && !File.Exists(backupName))
                    return;

                backupStream = File.Create(backupName);
                currentStream = File.Create(filename);

                saveDataSerializer.Serialize(backupStream, file);
                HandleEncryption(ref backupStream);

                backupStream.Seek(0, SeekOrigin.Begin);
                int maxRetries = 5;
                while(tryLoad(backupStream))
                {
                    maxRetries--;
                    if(maxRetries == 0)
                        break;
                }

                if(maxRetries != 0)
                {
                    backupStream.CopyTo(currentStream);
                    currentStream.Close();

                    if(uploadAsync)
                    {
                        IsSaving = true;
                        Thread t = new Thread(new ThreadStart(uploadSave));
                        Thread t2 = new Thread(new ThreadStart(delegate { while(t.IsAlive) { } IsSaving = false; })); // never ever join this
                        t2.IsBackground = true;
                        t.Start();
                        t2.Start();
                    }
                    else
                        uploadSave();
                }
                else
                    Program.Cutter.WriteExceptionToLog(new InvalidDataException("Could not save file after five retries."), false);

                backupStream.Close();
                currentStream.Close();
            }
            catch(Exception ex)
            {
                if(backupStream != null)
                    backupStream.Close();
                if(currentStream != null)
                    currentStream.Close();

                Program.Cutter.WriteToLog(this, "Save failed for:\n" + ex.Message);
                throw ex;
            }
#endif
            Program.Cutter.WriteToLog(this, "Save successful.");
        }

        private void uploadSave()
        {
#if !INDIECITY
            lock(lockingObject)
            {
                #region Save Uploading
                try
                {
                    RegistryKey k1 = null, k2 = null;
                    string username, password;

                    using(k1 = Registry.CurrentUser.OpenSubKey("Software", true))
                        using(k2 = k1.CreateSubKey("Accelerated Delivery"))
                        {
                            username = (string)k2.GetValue("username");
                            password = (string)k2.GetValue("password");
                        }

                    WebClient c = new WebClient();
                    byte[] response = c.UploadFile("http://www.accelerateddeliverygame.com/assets/upload.php?u=" + username + "&p=" + password,
                        filename);

                    string s = Encoding.ASCII.GetString(response);
                    if(s != "success")
                        throw new ApplicationException(s); // the catch handler will handle logging this error

                    Program.Cutter.WriteToLog(this, "Save upload successful.");

                    using(k1 = Registry.CurrentUser.OpenSubKey("Software", true))
                        using(k2 = k1.CreateSubKey("Accelerated Delivery"))
                            k2.DeleteValue("hydroxide", false); // get rid of it since we've uploaded successfully
                }
                catch(Exception e)
                {
                    Program.Cutter.WriteToLog(this, "Warning: Uploading save failed with error: \n" + e.Message);
                    using(RegistryKey k1 = Registry.CurrentUser.OpenSubKey("Software", true))
                    using(RegistryKey k2 = k1.CreateSubKey("Accelerated Delivery"))
                        k2.SetValue("hydroxide", true);
                }
                #endregion
            }
#endif
        }

        /// <summary>
        /// Loads the options from the XML.
        /// </summary>
        public void Load()
        {
            Program.Cutter.WriteToLog(this, "Attempting to load game data.");
#if XBOX360
            try
            {
                if(container == null)
                {
                    if(initializing)
                        throw new FileNotFoundException("Set Manager to null and continue along.");
                    int? devSelRes;
                    do
                    {
                        devSelRes = SimpleMessageBox.ShowMessageBox("Save", "No storage device is selected. Please select a device.",
                            new string[] { "Select a Device", "Continue Without Saving" }, 0, MessageBoxIcon.Alert);
                        switch(devSelRes)
                        {
                            case -1: // Fall through
                            case 1: returnOptionsToDefault();
                                return;
                            case 0: StorageDevice.BeginShowSelector(storageSelectEnd, null);
                                break;
                        }
                    } while(devSelRes == null);
                }
              
            try
            {
            FileNotFound:
                if(!container.FileExists(filename))
                {
                    if(!container.FileExists(backupName))
                    {
                        if(initializing)
                            throw new FileNotFoundException("Set Manager to null and continue along.");
                        int? fileResult;
                        do
                        {
                            fileResult = SimpleMessageBox.ShowMessageBox("No File Found", "No options file was found on the storage device. Would you like to create a new file or select a new device?",
                                new string[] { "Create File", "Select New Device", "Exit Without Loading" }, 0, MessageBoxIcon.Alert);
                            switch(fileResult)
                            {
                                case 0: New();
                                    return;
                                case 1: result = StorageDevice.BeginShowSelector(null, null);
                                    result.AsyncWaitHandle.WaitOne();
                                    device = StorageDevice.EndShowSelector(result);
                                    result.AsyncWaitHandle.Close();

                                    result = device.BeginOpenContainer("Accelerated Delivery", null, null);
                                    result.AsyncWaitHandle.WaitOne();
                                    container = device.EndOpenContainer(result);
                                    result.AsyncWaitHandle.Close();
                                    goto FileNotFound;
                                case -1: // Fall through
                                case 2: returnOptionsToDefault();
                                    return;
                            }
                        } while(fileResult == null);
                    }
                }
            
                currentStream = container.OpenFile(filename, FileMode.Open);
                HandleEncryption(ref currentStream);
                file = (SaveFile)saveDataSerializer.Deserialize(currentStream);
                currentStream.Close();
                container.Dispose();
            }
            catch
            {
                try
                {
                    if(!initializing)
                    {
                        int? lalala;
                        do
                        {
                            lalala = SimpleMessageBox.ShowMessageBox("Warning", "The save file was missing or corrupted, but a backup file was found. Loading backup file.", new string[] { "Okay" }, 0, MessageBoxIcon.Alert);
                        } while(lalala == null);
                    }
                    backupStream = container.OpenFile(backupName, FileMode.Open);
                    HandleEncryption(ref backupStream);
                    file = (SaveFile)saveDataSerializer.Deserialize(backupStream);
                    currentStream = container.CreateFile(filename);
                    backupStream.Seek(0, SeekOrigin.Begin);
                    backupStream.CopyTo(currentStream);
                    currentStream.Close();
                    backupStream.Close();
                    container.Dispose();
                }
                catch(Exception ex)
                {
                    if(backupStream != null)
                        backupStream.Close();
                    if(currentStream != null)
                        currentStream.Close();
                    if(container != null)
                        container.Dispose();
                    throw ex;
                }
            }
#elif WINDOWS
            // Attempt to load primary save
            try
            {
                if(!File.Exists(filename))
                {
                    if(!File.Exists(backupName))
                        New();
                }
                currentStream = File.Open(filename, FileMode.Open);
                HandleEncryption(ref currentStream);
                try
                {
                    file = (SaveFile)saveDataSerializer.Deserialize(currentStream);
                }
                catch
                {
                    currentStream.Seek(0, SeekOrigin.Begin);
                    HandleEncryption(ref currentStream);
                    file = (SaveFile)saveDataSerializer.Deserialize(currentStream);
                }
                // at this point loading was successful
                // it will always be safe to copy to backup at this point
                currentStream.Seek(0, SeekOrigin.Begin);
                backupStream = File.Open(backupName, FileMode.Create);
                currentStream.CopyTo(backupStream);
                backupStream.Close();
                //HandleEncryption(ref currentStream); // re-encrypts the file
                currentStream.Close();
                SaveGame1 = file.save1;
                SaveGame2 = file.save2;
                SaveGame3 = file.save3;
                currentSave = file.currentSaveGame;
                SuccessfulLoad = true;
                Program.Cutter.WriteToLog(this, "Load successful.");
            }
            catch(Exception ex)
            {
                Program.Cutter.WriteToLog(this, "An error occurred while loading game data. Attempting to load backup save.");
                Program.Cutter.WriteExceptionToLog(ex, false);
                try
                {
                    FileStream dump = File.Open(Program.SavePath + "dump.sav", FileMode.Create);
                    currentStream.Seek(0, SeekOrigin.Begin);
                    currentStream.CopyTo(dump);
                    dump.Close();
                }
                catch(Exception exc)
                {
                    Program.Cutter.WriteToLog(this, "Could not dump save file.");
                    Program.Cutter.WriteExceptionToLog(exc, false);
                }
                finally
                {
                    // Attempt to load backup save and copy data to current save

                    try
                    {
                        if(File.Exists(backupName))
                        {
                            backupStream = File.Open(backupName, FileMode.Open);
                            HandleEncryption(ref backupStream);
                            file = (SaveFile)saveDataSerializer.Deserialize(backupStream);
                            currentStream = File.Open(filename, FileMode.Create);
                            backupStream.Seek(0, SeekOrigin.Begin);
                            backupStream.CopyTo(currentStream);
                            currentStream.Close();
                            backupStream.Close();
                            SaveGame1 = file.save1;
                            SaveGame2 = file.save2;
                            SaveGame3 = file.save3;
                            currentSave = file.currentSaveGame;
                            SuccessfulLoad = true;
                            Program.Cutter.WriteToLog(this, "Load successful.");
                        }
                        else
                        {
                            Program.Cutter.WriteToLog(this, "Backup save was not there. Load failed.");
                            DialogResult d = MessageBox.Show("The save file is corrupt. Is it okay to create a new save file? This will erase your progress.", "Error!", MessageBoxButtons.YesNo);
                            if(d == DialogResult.Yes)
                                New();
                            else
                                Program.Game.Exit();
                        }
                    }
                    catch
                    {
                        // Primary and backups are faulty, throw error
                        if(backupStream != null)
                            backupStream.Close();
                        if(currentStream != null)
                            currentStream.Close();

                        SuccessfulLoad = false;
                        Program.Cutter.WriteToLog(this, "Primary and backup saves are faulty or missing. Load failed. Error is:");
                        Program.Cutter.WriteExceptionToLog(ex, false);

                        DialogResult d = MessageBox.Show("The save file is corrupt. Is it okay to create a new save file? This will erase your progress.", "Error!", MessageBoxButtons.YesNo);
                        if(d == DialogResult.Yes)
                            New();
                        else
                            Program.Game.Exit();
                    }
                }
            }
#endif
        }

        private bool tryLoad(Stream s)
        {
            try
            {
                HandleEncryption(ref s);
                file = (SaveFile)saveDataSerializer.Deserialize(s);
                HandleEncryption(ref s);
            }
            catch
            {
                s.Seek(0, SeekOrigin.Begin);
                return true;
            }
            finally
            {
                s.Seek(0, SeekOrigin.Begin);
            }
            return false;
        }

        /// <summary>
        /// Encrypts or decrypts a stream.
        /// </summary>
        /// <param name="stream">The Stream to encrypt.</param>
        private void HandleEncryption(ref Stream stream)
        {
            long length = stream.Length;
            if(length > int.MaxValue)
                throw new InvalidDataException("The file is too large to load. Ensure the file size is less than " + int.MaxValue + " bytes.");

            byte[] buffer = new byte[length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(buffer, 0, (int)length);

            for(int i = 0; i < length; i++)
                buffer[i] ^= key[i % key.Length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Creates an empty savegame. Does not switch it to the current savegame.
        /// Warning: Will overwrite any save game in the current slot.
        /// </summary>
        /// <param name="saveGameIndex">Valid values are 1, 2, 3.</param>
        public void CreateNewSaveGame(int saveGameIndex)
        {
            switch(saveGameIndex)
            {
                case 1: SaveGame1 = SaveData.GetEmptyData(saveGameIndex);
                    SaveGame1.BeenCreated = true;
                    break;
                case 2: SaveGame2 = SaveData.GetEmptyData(saveGameIndex);
                    SaveGame2.BeenCreated = true;
                    break;
                case 3: SaveGame3 = SaveData.GetEmptyData(saveGameIndex);
                    SaveGame3.BeenCreated = true;
                    break;
            }
        }

        /// <summary>
        /// Switchs the savegame.
        /// </summary>
        /// <param name="saveGameIndex">Valid values are 1, 2, and 3.</param>
        public void SwitchCurrentSave(int saveGameIndex)
        {
            currentSave = saveGameIndex - 1;
            if(!CurrentSave.BeenCreated)
                CreateNewSaveGame(currentSave + 1);
            SaveLoaded = true;

            OnSaveChanged();
        }

        /// <summary>
        /// Unloads a save, for use with the save selector.
        /// </summary>
        public void Unload()
        {
            SaveLoaded = false;
        }

        /// <summary>
        /// Gets one of the saves. Please don't modify values through this method.
        /// </summary>
        /// <param name="slot">Valid values are 1, 2, 3.</param>
        /// <returns>The save in that slot.</returns>
        public SaveData GetSaveSlot(int slot)
        {
            switch(slot)
            {
                case 1: return SaveGame1;
                case 2: return SaveGame2;
                case 3: return SaveGame3;
                default: throw new ArgumentOutOfRangeException("No such save file.");
            }
        }

        /// <summary>
        /// Deletes a file, permanently. Auto-saves.
        /// </summary>
        /// <param name="i">Valid values are 1, 2, 3.</param>
        public void Delete(int i)
        {
            switch(i)
            {
                case 1: SaveGame1 = SaveData.GetEmptyData(i);
                    break;
                case 2: SaveGame2 = SaveData.GetEmptyData(i);
                    break;
                case 3: SaveGame3 = SaveData.GetEmptyData(i);
                    break;
                default: throw new ArgumentOutOfRangeException("No such save file.");
            }
            OnSaveDeleted(i);
            Save(true);
        }

        private bool ftpDirectoryExists(string directory, SecureString p)
        {
            bool directoryExists;

            var request = (FtpWebRequest)WebRequest.Create(directory);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential("alex@twobuttoncrew.com", p);

            try
            {
                using(request.GetResponse())
                {
                    directoryExists = true;
                }
            }
            catch(WebException)
            {
                directoryExists = false;
            }

            return directoryExists;
        }
    }
}