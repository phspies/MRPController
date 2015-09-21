using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using PInvoke;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Xml;
using System.Threading;

namespace ConsoleUSNJournalProject
{
    class Program
    {
        private static DriveInfo _driveInfo;
        private static IntPtr _changeJournalRootHandle;
        private static uint _volumeSerialNumber;
        private static ulong _rootFrn = 0;
        private static Win32Api.USN_JOURNAL_DATA _currentUsnState = new Win32Api.USN_JOURNAL_DATA();
        private static Win32Api.USN_JOURNAL_DATA _previousUsnState = new Win32Api.USN_JOURNAL_DATA();
        private static uint _previousVolSer;

        private static bool _bFoldersModified = false;
        private static string _mftFoldersFileName = "_Folders.xml";
        private static string _mftFoldersFullPath = Path.Combine(Directory.GetCurrentDirectory(), _mftFoldersFileName);
        private static string _usnJournalStateFileName = "_UsnJournalState.xml";
        private static string _usnJournalStateFullPath = Path.Combine(Directory.GetCurrentDirectory(), _usnJournalStateFileName);


        private static Dictionary<UInt64, FileNameAndParentFrn> _folders = new Dictionary<ulong, FileNameAndParentFrn>();

        static void Main(string[] args)
        {
            //
            // set the _driveInfo to the C-drive
            //
            _driveInfo = new DriveInfo("C");

            try
            {
                InitializeChangeJournal();

                Console.WriteLine();
                Console.WriteLine("Total Directories found: {0}", _folders.Count);

                Console.WriteLine();
                Console.WriteLine("Journal state:");
                Console.WriteLine("    Journal ID: {0}", _previousUsnState.UsnJournalID);
                Console.WriteLine("   MaximumSize: {0}", _previousUsnState.MaximumSize);
                Console.WriteLine("        MaxUsn: {0}", _previousUsnState.MaxUsn);
                Console.WriteLine("      FirstUsn: {0}", _previousUsnState.FirstUsn);
                Console.WriteLine("       NextUsn:", _previousUsnState.NextUsn);

                Console.WriteLine();
                Console.WriteLine("Do something that causes the files/folders on C-Drive to change, then hit enter");
                Console.ReadKey();
                Win32Api.GetLastErrorEnum lastWin32Error = GatherUsnJournalChanges();
                Console.WriteLine();
                Console.WriteLine("Return value from GatherUsnJournalChanges: {0}", lastWin32Error.ToString());

            }
            catch (Exception excptn)
            {
                DisplayException(excptn);
            }
            Console.Write("Press any key to exit!");
            Console.Read();
        }

        private static Win32Api.GetLastErrorEnum CreateUsnJournal()
        {
            Console.WriteLine("CreateUsnJournal() function entered for drive '{0}'", _driveInfo.Name);

            // This function creates a journal on the volume. If a journal already
            // exists this function will adjust the MaximumSize and AllocationDelta
            // parameters of the journal

            Win32Api.GetLastErrorEnum lastWin32Error = Win32Api.GetLastErrorEnum.ERROR_SUCCESS;

            UInt64 MaximumSize = 0x10000000;
            UInt64 AllocationDelta = 0x100000;
            UInt32 cb;

            Win32Api.CREATE_USN_JOURNAL_DATA cujd = new Win32Api.CREATE_USN_JOURNAL_DATA();
            cujd.MaximumSize = MaximumSize;
            cujd.AllocationDelta = AllocationDelta;

            int sizeCujd = Marshal.SizeOf(cujd);
            IntPtr cujdBuffer = Marshal.AllocHGlobal(sizeCujd);
            Win32Api.ZeroMemory(cujdBuffer, sizeCujd);
            Marshal.StructureToPtr(cujd, cujdBuffer, true);
            //
            // -1 is an invalid handle and _changeJournalRootHandle is initialized to IntPtr.Zero
            //
            if (_changeJournalRootHandle.ToInt32() == 0 || _changeJournalRootHandle.ToInt32() == Win32Api.INVALID_HANDLE_VALUE)
            {
                GetRootHandle();
            }

            bool fOk = Win32Api.DeviceIoControl(
                _changeJournalRootHandle,
                Win32Api.FSCTL_CREATE_USN_JOURNAL,
                cujdBuffer,
                sizeCujd,
                IntPtr.Zero,
                0,
                out cb,
                IntPtr.Zero);
            if (!fOk)
            {
                lastWin32Error = (Win32Api.GetLastErrorEnum)Marshal.GetLastWin32Error();
            }
            Marshal.FreeHGlobal(cujdBuffer);
            return lastWin32Error;
        }

        private static Win32Api.GetLastErrorEnum DeleteUsnJournal()
        {
            //
            // This function deletes a usn journal on the volume. 
            //

            Console.WriteLine("DeleteUsnJournal() function entered for drive '{0}'", _driveInfo.Name);

            Win32Api.GetLastErrorEnum lastWin32Error = Win32Api.GetLastErrorEnum.ERROR_SUCCESS;

            UInt32 cb;

            Win32Api.DELETE_USN_JOURNAL_DATA dujd = new Win32Api.DELETE_USN_JOURNAL_DATA();
            dujd.UsnJournalID = _currentUsnState.UsnJournalID;
            dujd.DeleteFlags = (UInt32)Win32Api.UsnJournalDeleteFlags.USN_DELETE_FLAG_DELETE;

            int sizeDujd = Marshal.SizeOf(dujd);
            IntPtr dujdBuffer = Marshal.AllocHGlobal(sizeDujd);
            Win32Api.ZeroMemory(dujdBuffer, sizeDujd);
            Marshal.StructureToPtr(dujd, dujdBuffer, true);
            //
            // -1 is an invalid handle and _changeJournalRootHandle is initialized to IntPtr.Zero
            //
            if (_changeJournalRootHandle.ToInt32() == 0 || _changeJournalRootHandle.ToInt32() == Win32Api.INVALID_HANDLE_VALUE)
            {
                GetRootHandle();
            }

            bool fOk = Win32Api.DeviceIoControl(
                _changeJournalRootHandle,
                Win32Api.FSCTL_DELETE_USN_JOURNAL,
                dujdBuffer,
                sizeDujd,
                IntPtr.Zero,
                0,
                out cb,
                IntPtr.Zero);
            if (!fOk)
            {
                lastWin32Error = (Win32Api.GetLastErrorEnum)Marshal.GetLastWin32Error();
            }
            Marshal.FreeHGlobal(dujdBuffer);
            return lastWin32Error;
        }

        private static Win32Api.GetLastErrorEnum QueryUsnJournal()
        {
            Console.WriteLine("QueryUsnJournal() function entered for drive '{0}'", _driveInfo.Name);
            //
            // This function queries the usn journal on the volume. 
            //
            Win32Api.GetLastErrorEnum lastWin32Error = Win32Api.GetLastErrorEnum.ERROR_SUCCESS;

            Win32Api.USN_JOURNAL_DATA ujd = new Win32Api.USN_JOURNAL_DATA();
            int sizeUjd = Marshal.SizeOf(ujd);
            UInt32 cb;

            if (_changeJournalRootHandle.ToInt32() == 0)
            {
                GetRootHandle();
            }
            bool fOk = Win32Api.DeviceIoControl(
                _changeJournalRootHandle,
                Win32Api.FSCTL_QUERY_USN_JOURNAL,
                IntPtr.Zero,
                0,
                out ujd,
                sizeUjd,
                out cb,
                IntPtr.Zero);
            if (fOk)
            {
                _currentUsnState = ujd;
                Console.WriteLine("  Current usn journal state");
                Console.WriteLine("       Vol Ser: '{0}'", _volumeSerialNumber);
                Console.WriteLine("    Journal ID: '{0}'", _currentUsnState.UsnJournalID);
                Console.WriteLine("     First USN: '{0}'", _currentUsnState.FirstUsn);
                Console.WriteLine("      Next USN: '{0}'", _currentUsnState.NextUsn);
                Console.WriteLine("       Max USN: '{0}'", _currentUsnState.MaxUsn);
            }
            else
            {
                lastWin32Error = (Win32Api.GetLastErrorEnum)Marshal.GetLastWin32Error();
            }
            return lastWin32Error;
        }

        private static bool IsUsnJournalValid()
        {
            Console.WriteLine("IsUsnJournalValid() entered for drive '{0}'", _driveInfo.Name);

            bool retval = true;
            //
            // is the JournalID from the previous state == JournalID from current state?
            //
            if (_previousUsnState.UsnJournalID == _currentUsnState.UsnJournalID)
            {
                //
                // is the next usn to process still available
                //
                if (_previousUsnState.NextUsn > _currentUsnState.FirstUsn && _previousUsnState.NextUsn < _currentUsnState.NextUsn)
                {
                    retval = true;
                }
                else
                {
                    retval = false;
                }
            }
            else
            {
                retval = false;
            }
            return retval;
        }

        private static void InitializeChangeJournal()
        {
            Console.WriteLine("----------\n  ReInitializing USN Journal for volume '{0}'", _driveInfo.Name);
            DateTime start = DateTime.Now;

            bool bSuccess = false;
            Win32Api.GetLastErrorEnum lastWin32Error = Win32Api.GetLastErrorEnum.ERROR_SUCCESS;
            _folders.Clear();

            FileNameAndParentFrn fnapfrn = GetRootFrnEntry(_driveInfo.Name);

            if (fnapfrn != null)
            {
                _folders.Add(fnapfrn.Frn, fnapfrn);
            }
            while (true)
            {
                lastWin32Error = CreateUsnJournal();
                if (lastWin32Error == Win32Api.GetLastErrorEnum.ERROR_JOURNAL_DELETE_IN_PROGRESS)
                {
                    Console.WriteLine("CreateUsnJournal() {0}", lastWin32Error.ToString());
                    Thread.Sleep(100);
                    continue;
                }
                else if (lastWin32Error != Win32Api.GetLastErrorEnum.ERROR_SUCCESS)
                {
                    Console.WriteLine("CreateUsnJournal() encountered an error - {0}", lastWin32Error.ToString());
                    bSuccess = false;
                    break;
                }
                else
                {
                    bSuccess = true;
                    break;
                }
            }
            if (bSuccess)
            {
                lastWin32Error = QueryUsnJournal();
                if (lastWin32Error == Win32Api.GetLastErrorEnum.ERROR_SUCCESS)
                {
                    GatherNtfsVolumeDirectories();
                    //
                    // write folders out to folder file
                    //
                    UpdateUsnJournalInfo();
                }
                else
                {
                    Console.WriteLine("QueryUsnJournal() encountered an error! {0}", lastWin32Error.ToString());
                }
            }
            TimeSpan ts = DateTime.Now - start;
            Console.WriteLine("  Time to ReInitialize USN Journal: {0} secs.\n----------", ts.TotalSeconds);
        }

        private static FileNameAndParentFrn GetRootFrnEntry(string path)
        {
            Console.WriteLine("GetRootFrnEntry() function entered for drive '{0}'", path);

            FileNameAndParentFrn fnapfrn = null;
            string pathRoot = string.Concat("\\\\.\\", path);
            pathRoot = string.Concat(pathRoot, Path.DirectorySeparatorChar);
            IntPtr hRoot = Win32Api.CreateFile(pathRoot,
                0,
                Win32Api.FILE_SHARE_READ | Win32Api.FILE_SHARE_WRITE,
                IntPtr.Zero,
                Win32Api.OPEN_EXISTING,
                Win32Api.FILE_FLAG_BACKUP_SEMANTICS,
                IntPtr.Zero);

            if (hRoot.ToInt32() != Win32Api.INVALID_HANDLE_VALUE)
            {
                Win32Api.BY_HANDLE_FILE_INFORMATION fi = new Win32Api.BY_HANDLE_FILE_INFORMATION();
                bool bRtn = Win32Api.GetFileInformationByHandle(hRoot, out fi);
                if (bRtn)
                {
                    UInt64 fileIndexHigh = (UInt64)fi.FileIndexHigh;
                    UInt64 indexRoot = (fileIndexHigh << 32) | fi.FileIndexLow;
                    _volumeSerialNumber = fi.VolumeSerialNumber;
                    fnapfrn = new FileNameAndParentFrn(_volumeSerialNumber, _driveInfo.Name, indexRoot, 0);
                    Console.WriteLine("  Path root name '{0}' vol ser '{1}' frn '{2}', parent frn '{3}'",
                        fnapfrn.Name, fnapfrn.VolSer, _rootFrn.ToString("x"), fnapfrn.ParentFrn.ToString("x"));
                }
                else
                {
                    throw new IOException("GetFileInformationbyHandle() returned invalid handle",
                        new Win32Exception(Marshal.GetLastWin32Error()));
                }
                Win32Api.CloseHandle(hRoot);
            }
            else
            {
                throw new IOException("Unable to get root frn entry", new Win32Exception(Marshal.GetLastWin32Error()));
            }
            return fnapfrn;
        }

        public static void UpdateUsnJournalInfo()
        {
            WriteUsnJournalState();
            _previousUsnState = _currentUsnState;
            if (_bFoldersModified)
            {
                WriteFoldersFile();
            }
        }

        private static void ReadUsnJournalState()
        {
            Console.WriteLine("ReadUsnJournalState() function entered for volume '{0}'", _driveInfo.Name);

            if (File.Exists(_usnJournalStateFullPath))
            {
                FileStream usnJournalStateStream = null;
                XmlTextReader reader = null;

                try
                {
                    usnJournalStateStream = File.Open(_usnJournalStateFullPath, FileMode.Open, FileAccess.Read, FileShare.None);
                    reader = new XmlTextReader(usnJournalStateStream);
                    reader.MoveToElement();
                    while (reader.Read())
                    {
                        if (0 != string.Compare(reader.LocalName, "usnjournalstate", true))
                        {
                            continue;
                        }
                        _previousVolSer = Convert.ToUInt32(reader.GetAttribute("volser"));
                        _volumeSerialNumber = _previousVolSer;
                        _previousUsnState.UsnJournalID = Convert.ToUInt64(reader.GetAttribute("journalid"), 16);
                        _previousUsnState.FirstUsn = Convert.ToInt64(reader.GetAttribute("firstusn"), 16);
                        _previousUsnState.NextUsn = Convert.ToInt64(reader.GetAttribute("nextusn"), 16);
                        _previousUsnState.MaxUsn = Convert.ToInt64(reader.GetAttribute("maxusn"), 16);
                        _previousUsnState.MaximumSize = Convert.ToUInt64(reader.GetAttribute("maxsize"), 16);
                        _previousUsnState.AllocationDelta = Convert.ToUInt64(reader.GetAttribute("allocationdelta"), 16);
                        _previousUsnState.LowestValidUsn = Convert.ToInt64(reader.GetAttribute("lowestvalidusn"), 16);
                        Console.WriteLine("  Previous usn journal state");
                        Console.WriteLine("       Vol Ser: '{0}'", _previousVolSer);
                        Console.WriteLine("    Journal ID: '{0}'", _previousUsnState.UsnJournalID);
                        Console.WriteLine("     First USN: '{0}'", _previousUsnState.FirstUsn);
                        Console.WriteLine("      Next USN: '{0}'", _previousUsnState.NextUsn);
                        Console.WriteLine("       Max USN: '{0}'", _previousUsnState.MaxUsn);
                    }
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    if (usnJournalStateStream != null)
                    {
                        usnJournalStateStream.Close();
                        usnJournalStateStream.Dispose();
                    }
                }
            }
            else
            {
                string msg = string.Format("  Usn Journal state file '{0}' does not exist", _usnJournalStateFullPath);
                Console.WriteLine(msg);
                throw new Exception(msg.TrimStart(' '));
            }
        }

        public static void Serialize()
        {
            //
            // serialize the usn journal state and folder information
            //
            WriteUsnJournalState();
            WriteFoldersFile();
        }

        private static void WriteUsnJournalState()
        {
            FileStream usnJournalStateStream = null;
            XmlTextWriter writer = null;
            if (File.Exists(_usnJournalStateFullPath))
            {
                File.Delete(_usnJournalStateFullPath);
            }
            try
            {
                usnJournalStateStream = File.Open(
                    _usnJournalStateFullPath,
                    FileMode.OpenOrCreate,
                    FileAccess.Write,
                    FileShare.None);

                using (writer = new XmlTextWriter(usnJournalStateStream, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartDocument();
                    writer.WriteStartElement("root");
                    writer.WriteStartElement("usnjournalstate");
                    writer.WriteAttributeString("volser", _volumeSerialNumber.ToString());
                    writer.WriteAttributeString("journalid", _currentUsnState.UsnJournalID.ToString("x"));
                    writer.WriteAttributeString("firstusn", _currentUsnState.FirstUsn.ToString("x"));
                    writer.WriteAttributeString("nextusn", _currentUsnState.NextUsn.ToString("x"));
                    writer.WriteAttributeString("maxusn", _currentUsnState.MaxUsn.ToString("x"));
                    writer.WriteAttributeString("maxsize", _currentUsnState.MaximumSize.ToString("x"));
                    writer.WriteAttributeString("allocationdelta", _currentUsnState.AllocationDelta.ToString("x"));
                    writer.WriteAttributeString("lowestvalidusn", _currentUsnState.LowestValidUsn.ToString("x"));
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (XmlException xmlExcptn)
            {
                throw new Exception("Exception caught in WriteFoldersFile().", xmlExcptn);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                if (usnJournalStateStream != null)
                {
                    usnJournalStateStream.Close();
                    usnJournalStateStream.Dispose();
                }
            }
        }

        private static void WriteFoldersFile()
        {
            Console.WriteLine("WriteDirectoriesFile() function entered for drive '{0}'", _driveInfo.Name);

            FileStream foldersStream = null;
            if (_bFoldersModified)
            {
                if (File.Exists(_mftFoldersFullPath))
                {
                    File.Delete(_mftFoldersFullPath);
                }
                try
                {
                    foldersStream = File.Open(
                        _mftFoldersFullPath,
                        FileMode.OpenOrCreate,
                        FileAccess.Write,
                        FileShare.None);

                    using (XmlTextWriter writer = new XmlTextWriter(foldersStream, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartDocument();
                        writer.WriteStartElement("folders");
                        foreach (KeyValuePair<ulong, FileNameAndParentFrn> kvp in _folders)
                        {
                            FileNameAndParentFrn fnapfrn = kvp.Value;
                            writer.WriteStartElement("folder");
                            writer.WriteAttributeString("frn", kvp.Key.ToString("x"));
                            writer.WriteAttributeString("pfrn", fnapfrn.ParentFrn.ToString("x"));
                            writer.WriteAttributeString("nm", fnapfrn.Name);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                }
                catch (XmlException xmlExcptn)
                {
                    throw new Exception("Exception caught in WriteFoldersFile().", xmlExcptn);
                }
                Console.WriteLine("Total folders serialized to disk: {0}", _folders.Count);
                _bFoldersModified = false;
            }
        }

        private static Win32Api.GetLastErrorEnum GatherNtfsVolumeDirectories()
        {
            Console.WriteLine("----------\n  Gathering Ntfs Volume Directories for drive '{0}'", _driveInfo.Name);
            DateTime start = DateTime.Now;

            Win32Api.GetLastErrorEnum lastWin32Error = Win32Api.GetLastErrorEnum.ERROR_SUCCESS;
            //
            // set up MFT_ENUM_DATA structure
            //
            Win32Api.MFT_ENUM_DATA med;
            med.StartFileReferenceNumber = 0;
            med.LowUsn = 0;
            med.HighUsn = _currentUsnState.NextUsn;
            Int32 sizeMftEnumData = Marshal.SizeOf(med);
            IntPtr medBuffer = Marshal.AllocHGlobal(sizeMftEnumData);
            Win32Api.ZeroMemory(medBuffer, sizeMftEnumData);
            Marshal.StructureToPtr(med, medBuffer, true);

            //
            // set up the data buffer which receives the USN_RECORD data
            //
            int pDataSize = sizeof(UInt64) + 10000;
            IntPtr pData = Marshal.AllocHGlobal(pDataSize);
            Win32Api.ZeroMemory(pData, pDataSize);
            uint outBytesReturned = 0;
            Win32Api.USN_RECORD usn = null;

            //
            // Gather up volume's directories
            //
            while (false != Win32Api.DeviceIoControl(
                _changeJournalRootHandle,
                Win32Api.FSCTL_ENUM_USN_DATA,
                medBuffer,
                sizeMftEnumData,
                pData,
                pDataSize,
                out outBytesReturned,
                IntPtr.Zero))
            {
                IntPtr pUsnRecord = new IntPtr(pData.ToInt32() + sizeof(Int64));
                while (outBytesReturned > 60)
                {
                    usn = new Win32Api.USN_RECORD(pUsnRecord);
                    //
                    // check for directory entries
                    //
                    if (0 != (usn.FileAttributes & Win32Api.FILE_ATTRIBUTE_DIRECTORY))
                    {
                        FileNameAndParentFrn directoryEntry =
                            new FileNameAndParentFrn(_volumeSerialNumber, usn.FileName,
                                usn.FileReferenceNumber,
                                usn.ParentFileReferenceNumber);
                        _folders.Add(usn.FileReferenceNumber, directoryEntry);
                        _bFoldersModified = true;
                    }
                    pUsnRecord = new IntPtr(pUsnRecord.ToInt32() + usn.RecordLength);
                    outBytesReturned -= usn.RecordLength;
                }
                Marshal.WriteInt64(medBuffer, Marshal.ReadInt64(pData, 0));
            }
            Marshal.FreeHGlobal(pData);

            Console.WriteLine("  Total Directories gathered from Usn Journal: {0}", _folders.Count);
            TimeSpan elapsedTime = DateTime.Now - start;
            Console.WriteLine("  Elapsed time for reading MFT: {0} secs.\n----------", elapsedTime.TotalSeconds);

            lastWin32Error = (Win32Api.GetLastErrorEnum)Marshal.GetLastWin32Error();
            if (lastWin32Error == Win32Api.GetLastErrorEnum.ERROR_SUCCESS || lastWin32Error == Win32Api.GetLastErrorEnum.ERROR_HANDLE_EOF)
            {
                lastWin32Error = Win32Api.GetLastErrorEnum.ERROR_SUCCESS;
            }
            return lastWin32Error;
        }

        private static Win32Api.GetLastErrorEnum GatherUsnJournalChanges()
        {
            string statusMsg = string.Empty;
            DateTime start = DateTime.Now;
            TimeSpan result;
            bool bReadMore = true;
            //
            // update the usn journal state
            //
            Win32Api.GetLastErrorEnum lastWin32Error = QueryUsnJournal();
            if (lastWin32Error == Win32Api.GetLastErrorEnum.ERROR_SUCCESS)
            {
                //
                // sequentially process the usn journal looking for image file entries
                //
                int pbDataSize = sizeof(UInt64) * 0x4000;
                IntPtr pbData = Marshal.AllocHGlobal(pbDataSize);
                Win32Api.ZeroMemory(pbData, pbDataSize);
                uint outBytesReturned = 0;

                Win32Api.READ_USN_JOURNAL_DATA rujd = new Win32Api.READ_USN_JOURNAL_DATA();
                rujd.StartUsn = _previousUsnState.NextUsn;
                rujd.ReasonMask = Win32Api.USN_REASON_CLOSE |
                    Win32Api.USN_REASON_FILE_DELETE |
                    Win32Api.USN_REASON_RENAME_NEW_NAME |
                    Win32Api.USN_REASON_RENAME_OLD_NAME;
                rujd.ReturnOnlyOnClose = 0;
                rujd.Timeout = 0;
                rujd.bytesToWaitFor = 0;
                rujd.UsnJournalId = _previousUsnState.UsnJournalID;

                int sizeRujd = Marshal.SizeOf(rujd);
                IntPtr rujdBuffer = Marshal.AllocHGlobal(sizeRujd);
                Win32Api.ZeroMemory(rujdBuffer, sizeRujd);
                Marshal.StructureToPtr(rujd, rujdBuffer, true);

                Win32Api.USN_RECORD usn = null;

                //
                // read usn journal entries
                //
                while (bReadMore)
                {
                    bool bRtn = Win32Api.DeviceIoControl(
                        _changeJournalRootHandle,
                        Win32Api.FSCTL_READ_USN_JOURNAL,
                        rujdBuffer,
                        sizeRujd,
                        pbData,
                        pbDataSize,
                        out outBytesReturned,
                        IntPtr.Zero);
                    if (bRtn)
                    {
                        IntPtr pUsnRecord = new IntPtr(pbData.ToInt32() + sizeof(UInt64));
                        while (outBytesReturned > 60)   // while there are at least one entry in the usn journal
                        {
                            usn = new Win32Api.USN_RECORD(pUsnRecord);
                            if (usn.Usn >= _currentUsnState.NextUsn)      // only read until the current usn points beyond the current state's usn
                            {
                                bReadMore = false;
                                break;
                            }
                            if (0 == (usn.FileAttributes & Win32Api.FILE_ATTRIBUTE_DIRECTORY))
                            {
                                //
                                // handle files
                                //
                                HandleFiles(usn);
                            }
                            else
                            {
                                //
                                // handle folders/directories
                                //
                                HandleFolders(usn);
                            }

                            pUsnRecord = new IntPtr(pUsnRecord.ToInt32() + usn.RecordLength);
                            outBytesReturned -= usn.RecordLength;

                        }   // end while (outBytesReturned > 60) closing bracket

                    }   // end else closing bracket for if (bRtn)
                    else
                    {
                        lastWin32Error = (Win32Api.GetLastErrorEnum)Marshal.GetLastWin32Error();
                        Console.WriteLine("Error encountered reading USN Journal! {0}", lastWin32Error);
                        break;
                    }

                    Int64 nextUsn = Marshal.ReadInt64(pbData, 0);
                    if (nextUsn >= _currentUsnState.NextUsn)
                    {
                        break;
                    }
                    Marshal.WriteInt64(rujdBuffer, nextUsn);

                }   // end while (bReadMore) closing bracket

                Marshal.FreeHGlobal(rujdBuffer);
                Marshal.FreeHGlobal(pbData);

                result = DateTime.Now - start;
                Console.WriteLine("  Time to read usn journal entries:{0} seconds", result.TotalSeconds);
            }
            else
            {
                Console.WriteLine("Error encountered in QueryUsnJournal! {0}", lastWin32Error.ToString());
            }
            return lastWin32Error;
        }   // end GatherUsnJournalChanges() closing bracket

        private static void HandleFiles(Win32Api.USN_RECORD usn)
        {
            FileNameAndParentFrn fnapfrn = new FileNameAndParentFrn(_volumeSerialNumber, usn);
            if ((usn.Reason & Win32Api.USN_REASON_FILE_DELETE) != 0 ||
                (usn.Reason & Win32Api.USN_REASON_RENAME_OLD_NAME) != 0)
            {
                //
                // if the reason code is delete or rename old add entry to the deleted image file list 
                //
                HandleDeletes(fnapfrn, usn.Reason);
            }
            else if ((usn.Reason & Win32Api.USN_REASON_RENAME_NEW_NAME) != 0)
            {
                //
                // if the reason code is 'rename new' handle the new files
                //
                HandleAdds(fnapfrn, usn.Reason);
            }
            else if ((usn.Reason & Win32Api.USN_REASON_CLOSE) != 0 &&
                    (usn.Reason & Win32Api.USN_REASON_FILE_CREATE) != 0)
            {
                //
                // if an image file has been copied in, we should see reasons for create and close.
                // Handle as a new image file
                //
                HandleAdds(fnapfrn, usn.Reason);
            }
        }

        private static void HandleFolders(Win32Api.USN_RECORD usn)
        {
            ulong frn = usn.FileReferenceNumber;
            if ((usn.Reason & Win32Api.USN_REASON_FILE_DELETE) != 0 ||
                (usn.Reason & Win32Api.USN_REASON_RENAME_OLD_NAME) != 0)
            {
                _folders.Remove(frn);
                _bFoldersModified = true;
            }
            else if ((usn.Reason & Win32Api.USN_REASON_RENAME_NEW_NAME) != 0 &&
                (usn.Reason & Win32Api.USN_REASON_CLOSE) != 0)
            {
                _folders.Remove(frn);
                FileNameAndParentFrn fnapfrn =
                    new FileNameAndParentFrn(_volumeSerialNumber, usn.FileName,
                        frn, usn.ParentFileReferenceNumber);
                _folders.Add(frn, fnapfrn);
                _bFoldersModified = true;
            }
        }

        private static void HandleAdds(FileNameAndParentFrn fnapfrn, uint reason)
        {
            //
            // get the fully qualified path of the image file
            //
            string folder = GetFullyQualifiedPath(fnapfrn.ParentFrn).ToLower();
            //
            // send the FileNameAndParentFrn object to the image search engine to be added
            // to the image index.
            //
            Console.WriteLine("Add request");
            Console.WriteLine("  Path: {0}\n  Filename: {1}\n   frn:{2}\n  pfrn:{3}",
                        folder, fnapfrn.Name, fnapfrn.Frn, fnapfrn.ParentFrn);

            DisplayReasonCodes(folder, reason);
        }   // end MonitorFile() closing bracket

        private static void HandleDeletes(FileNameAndParentFrn fnapfrn, uint reason)
        {
            //
            // get the fully qualified path of the image file
            //
            string folder = GetFullyQualifiedPath(fnapfrn.ParentFrn).ToLower();
            //
            // send the FileNameAndParentFrn object to the image search engine to be removed 
            // from the image index.
            //
            Console.WriteLine("Delete request");
            Console.WriteLine("  Path: {0}\n  Filename: {1}\n   frn:{2}\n  pfrn:{3}",
                        folder, fnapfrn.Name, fnapfrn.Frn, fnapfrn.ParentFrn);

            DisplayReasonCodes(folder, reason);
        }

        public static string GetFullyQualifiedPath(UInt64 frn)
        {
            string retval = string.Empty; ;
            FileNameAndParentFrn fnFRN = null;
            if (frn >= 0)
            {
                if (_folders.TryGetValue(frn, out fnFRN))
                {
                    retval = fnFRN.Name;
                    while (fnFRN.ParentFrn != 0)
                    {
                        if (_folders.TryGetValue(fnFRN.ParentFrn, out fnFRN))
                        {
                            string name = fnFRN.Name;
                            retval = Path.Combine(name, retval);
                        }
                        else
                        {
                            Console.WriteLine("File or Directory's '{0}' parent frn not found", retval);
                            break;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException("Invalid argument", "frn");
            }
            return retval;
        }

        private static void GetRootHandle()
        {
            Console.WriteLine("GetRootHandle() function entered for drive '{0}'", _driveInfo.Name);

            string vol = string.Concat("\\\\.\\", _driveInfo.Name.TrimEnd('\\'));
            _changeJournalRootHandle = Win32Api.CreateFile(vol,
                 Win32Api.GENERIC_READ | Win32Api.GENERIC_WRITE,
                 Win32Api.FILE_SHARE_READ | Win32Api.FILE_SHARE_WRITE,
                 IntPtr.Zero,
                 Win32Api.OPEN_EXISTING,
                 0,
                 IntPtr.Zero);
            if (_changeJournalRootHandle.ToInt32() == Win32Api.INVALID_HANDLE_VALUE)
            {
                Exception excptn = new Win32Exception(Marshal.GetLastWin32Error());
                throw new IOException("CreateFile() returned invalid handle", excptn);
            }
        }

        private static void DisplayReasonCodes(string fileName, uint reason)
        {
            Console.WriteLine("\n    Reason Code(s)");
            if ((reason & Win32Api.USN_REASON_BASIC_INFO_CHANGE) != 0)
            {
                Console.WriteLine("    USN_REASON_BASIC_INFO_CHANGE");
            }
            if ((reason & Win32Api.USN_REASON_CLOSE) != 0)
            {
                Console.WriteLine("    USN_REASON_CLOSE");
            }
            if ((reason & Win32Api.USN_REASON_COMPRESSION_CHANGE) != 0)
            {
                Console.WriteLine("    USN_REASON_COMPRESSION_CHANGE");
            }
            if ((reason & Win32Api.USN_REASON_DATA_EXTEND) != 0)
            {
                Console.WriteLine("    USN_REASON_DATA_EXTEND");
            }
            if ((reason & Win32Api.USN_REASON_DATA_OVERWRITE) != 0)
            {
                Console.WriteLine("    USN_REASON_DATA_OVERWRITE");
            }
            if ((reason & Win32Api.USN_REASON_DATA_TRUNCATION) != 0)
            {
                Console.WriteLine("    USN_REASON_DATA_TRUNCATION");
            }
            if ((reason & Win32Api.USN_REASON_EA_CHANGE) != 0)
            {
                Console.WriteLine("    USN_REASON_EA_CHANGE");
            }
            if ((reason & Win32Api.USN_REASON_ENCRYPTION_CHANGE) != 0)
            {
                Console.WriteLine("    USN_REASON_ENCRYPTION_CHANGE");
            }
            if ((reason & Win32Api.USN_REASON_FILE_CREATE) != 0)
            {
                Console.WriteLine("    USN_REASON_FILE_CREATE");
            }
            if ((reason & Win32Api.USN_REASON_FILE_DELETE) != 0)
            {
                Console.WriteLine("    USN_REASON_FILE_DELETE");
            }
            if ((reason & Win32Api.USN_REASON_HARD_LINK_CHANGE) != 0)
            {
                Console.WriteLine("    USN_REASON_HARD_LINK_CHANGE");
            }
            if ((reason & Win32Api.USN_REASON_INDEXABLE_CHANGE) != 0)
            {
                Console.WriteLine("    USN_REASON_INDEXABLE_CHANGE");
            }
            if ((reason & Win32Api.USN_REASON_NAMED_DATA_EXTEND) != 0)
            {
                Console.WriteLine("    USN_REASON_NAMED_DATA_EXTEND");
            }
            if ((reason & Win32Api.USN_REASON_NAMED_DATA_OVERWRITE) != 0)
            {
                Console.WriteLine("    USN_REASON_NAMED_DATA_OVERWRITE");
            }
            if ((reason & Win32Api.USN_REASON_NAMED_DATA_TRUNCATION) != 0)
            {
                Console.WriteLine("    USN_REASON_NAMED_DATA_TRUNCATION");
            }
            if ((reason & Win32Api.USN_REASON_OBJECT_ID_CHANGE) != 0)
            {
                Console.WriteLine("    USN_REASON_OBJECT_ID_CHANGE");
            }
            if ((reason & Win32Api.USN_REASON_RENAME_NEW_NAME) != 0)
            {
                Console.WriteLine("    USN_REASON_RENAME_NEW_NAME");
            }
            if ((reason & Win32Api.USN_REASON_RENAME_OLD_NAME) != 0)
            {
                Console.WriteLine("    USN_REASON_RENAME_OLD_NAME");
            }
            if ((reason & Win32Api.USN_REASON_REPARSE_POINT_CHANGE) != 0)
            {
                Console.WriteLine("    USN_REASON_REPARSE_POINT_CHANGE");
            }
            if ((reason & Win32Api.USN_REASON_SECURITY_CHANGE) != 0)
            {
                Console.WriteLine("    USN_REASON_SECURITY_CHANGE");
            }
            if ((reason & Win32Api.USN_REASON_STREAM_CHANGE) != 0)
            {
                Console.WriteLine("    USN_REASON_STREAM_CHANGE");
            }

        }   // DisplayReasonCodes() closing bracket

        private static void DisplayException(Exception excptn)
        {
            string padding = string.Empty;
            while (excptn != null)
            {
                Console.WriteLine("{0}{1}", padding, excptn.Message);
                padding = "   ";
                excptn = excptn.InnerException;
            }
        }
    }
}