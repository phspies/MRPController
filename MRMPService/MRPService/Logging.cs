using MRMPService.LocalDatabase;
using System;
using System.IO;
using System.Threading;

namespace MRMPService.MRMPService.Log
{
    public class Logger
    {
        public enum Severity { Info, Debug, Error, Warn, Fatal };
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

        public static void log(String message, Severity _severity)
        {
            message = message.Replace("\n", String.Empty);
            message = message.Replace("\r", String.Empty);
            DateTime datet = DateTime.UtcNow;
            string loglocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            String filePath = "MRMP_Log_" + datet.ToString("yyyy_MM_dd") + ".log";
            string logfilename = Path.Combine(loglocation, "Log", filePath);
            if (!File.Exists(logfilename))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logfilename));
                FileStream files = File.Create(logfilename);
                files.Close();
            }
            while (true)
            {
                _readWriteLock.EnterWriteLock();
                try
                {
                    // Append text to the file
                    using (StreamWriter sw = File.AppendText(logfilename))
                    {
                        sw.WriteLine(datet.ToString("yyyy/MM/dd hh:mm:ss") + " " + _severity + " > " + message);
                        sw.Close();
                        if (_severity != Severity.Info)
                        {
                            using (ManagerEventSet _event = new ManagerEventSet())
                            {
                                ManagerEvent _new_event = new ManagerEvent();
                                _new_event.message = message;
                                _new_event.timestamp = DateTime.UtcNow;
                                _event.ModelRepository.Insert(_new_event);
                            }
                        }

                    }
                    break;
                }
                catch (Exception )
                {
                }
                finally
                {
                    // Release lock
                    _readWriteLock.ExitWriteLock();
                }
            }
        }
    }
}
