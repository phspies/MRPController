using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class Program
    {
        const int FILE_ATTRIBUTE_SYSTEM = 0x4;
        const int FILE_FLAG_SEQUENTIAL_SCAN = 0x8;

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(string fileName, [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess, [MarshalAs(UnmanagedType.U4)] FileShare fileShare, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, int flags, IntPtr template);

        static List<string> hash_table = new List<String>();

        static void Main(string[] args)
        {
            using (SafeFileHandle device = CreateFile(@"\\.\C:", FileAccess.Read, FileShare.Write | FileShare.Read | FileShare.Delete, IntPtr.Zero, FileMode.Open, FILE_ATTRIBUTE_SYSTEM | FILE_FLAG_SEQUENTIAL_SCAN, IntPtr.Zero))
            {
                if (device.IsInvalid)
                {
                    throw new IOException("Unable to access drive. Win32 Error Code " + Marshal.GetLastWin32Error());
                }

                FileStream src = new FileStream(device, FileAccess.Read);

                int _blockSize = 16 * 1024;

                int lastPos = 0;
                int _amountBlocks = 0;
                src.Position = 0;
                long unique, duplicate;
                unique = duplicate = 0;
                while (true)
                {
                    byte[] block = new byte[_blockSize];
                    try
                    {
                        src.Read(block, lastPos, _blockSize);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                    }
                    //encypt byte[]
                    String base64 = null;
                    byte[] _encypted = null;
                    string hash = null;

                    try
                    {
                        hash = HashToString(block);
                        if (hash_table.Contains(hash))
                        {
                            duplicate += 1;
                        }
                        else
                        {
                            unique += 1;
                            hash_table.Add(hash);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    Console.WriteLine(String.Format("Total Hashes: {0} | Duplicate Data: {1} | Add unique data: {2}", hash_table.Count, duplicate, unique));

                }
            }
        }

        private static string GetHashFile(System.IO.FileStream fileStream)
        {
            RIPEMD160 myRIPEMD160 = RIPEMD160Managed.Create();
            StringBuilder sb = new StringBuilder();
            byte[] hashValue;
            hashValue = myRIPEMD160.ComputeHash(fileStream);
            foreach (byte by in hashValue)
            {
                sb.Append(by.ToString("x2"));
            }
            //combine file and system id to ensure file hash is global unique to this system and file
            String combined = sb.ToString();

            return GetHash(combined);
        }
        private static string GetHash(String data)
        {
            byte[] _data = Convert.FromBase64String(data);
            SHA1 sha = new SHA1CryptoServiceProvider();
            StringBuilder sb = new StringBuilder();
            _data = sha.ComputeHash(_data);
            foreach (byte by in _data)
            {
                sb.Append(by.ToString("x2"));
            }
            return sb.ToString();
        }
        static string HashToString(byte[] bytes)
        {
            byte[] hashData = SHA1.Create().ComputeHash(bytes); // SHA1 or MD5
            string hashText = "";
            string hexValue = "";
            foreach (byte b in hashData)
            {
                hexValue = b.ToString("X").ToLower(); // Lowercase for compatibility on case-sensitive systems
                hashText += (hexValue.Length == 1 ? "0" : "") + hexValue;
            }

            return hashText;
        }
    }
}