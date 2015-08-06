using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Decryptor
{
    class Program
    {
        private static byte[] k;

        static void Main(string[] args)
        {
            string u, p;

            if(args.Length != 0)
            {
                if(args.Length == 2)
                {
                    u = args[0];
                    p = args[1];
                }
                else
                {
                    Console.WriteLine("Usage: \"Decryptor.exe (username) (password)\" or run without command-line args.");
                    Console.Read();
                    return;
                }
            }
            else
            {
                Console.Write("Enter username used to encrypt: ");
                u = Console.ReadLine();
                if(u[u.Length - 1] == '\n')
                    u = u.Substring(0, u.Length - 1);
                Console.Write("Enter password used to encrypt: ");
                p = Console.ReadLine();
                if(p[p.Length - 1] == '\n')
                    p = p.Substring(0, p.Length - 1);
            }

            HashAlgorithm a = new SHA256Managed();
            byte[] n = Encoding.UTF8.GetBytes(u);
            byte[] salt = a.ComputeHash(n);
            byte[] ps = Encoding.UTF8.GetBytes(p);
            byte[] pre = new byte[ps.Length + salt.Length];
            ps.CopyTo(pre, 0);
            salt.CopyTo(pre, ps.Length);
            k = a.ComputeHash(pre);

            bool doNotRead = false;

            while(true) // loop until exit
            {
                try
                {
                    Console.Write("Press 1 for main file, 2 for backup, 3 for dump, and 4 for exit: ");
                    ConsoleKeyInfo key = new ConsoleKeyInfo();
                    while(key.Key != ConsoleKey.D1 && key.Key != ConsoleKey.D2 && key.Key != ConsoleKey.D3 && key.Key != ConsoleKey.D4)
                        key = Console.ReadKey(true);

                    string savePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Accelerated Delivery\\";
                    Stream file = null;

                    switch(key.Key)
                    {
                        case ConsoleKey.D1:
                            Console.WriteLine("1");
                            file = File.Open(savePath + u + ".sav", FileMode.Open, FileAccess.ReadWrite);
                            break;
                        case ConsoleKey.D2:
                            Console.WriteLine("2");
                            file = File.Open(savePath + u + ".bak", FileMode.Open, FileAccess.ReadWrite);
                            break;
                        case ConsoleKey.D3:
                            Console.WriteLine("3");
                            file = File.Open(savePath + "dump.sav", FileMode.Open, FileAccess.ReadWrite);
                            break;
                        case ConsoleKey.D4:
                            Console.WriteLine("4");
                            doNotRead = true;
                            return;
                    }
                    HandleEncryption(ref file);
                    file.Close();
                    Console.WriteLine("Done.");
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
                finally
                {
                    if(!doNotRead)
                        Console.ReadLine();
                }
            }
        }

        private static void HandleEncryption(ref Stream stream)
        {
            long length = stream.Length;
            if(length > int.MaxValue)
                throw new InvalidDataException("\nThe file is too large to load. Ensure the file size is less than " + int.MaxValue + " bytes.");

            byte[] buffer = new byte[length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(buffer, 0, (int)length);

            for(int i = 0; i < length; i++)
                buffer[i] ^= k[i % k.Length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
