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

        public static void log(String message, Severity _severity, String _subsystem = null)
        {
            message = message.Replace("\n", String.Empty);
            message = message.Replace("\r", String.Empty);
            DateTime datet = DateTime.UtcNow;
            string _log_location = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            String _prefix = _subsystem == null ? "core_engine_" : String.Format("{0}_", _subsystem);
            String _file_path = _prefix + datet.ToString("yyyy_MM_dd") + ".log";
            string _log_filename = Path.Combine(_log_location, "Log", _file_path);
            if (!File.Exists(_log_filename))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_log_filename));
                FileStream files = File.Create(_log_filename);
                files.Close();
            }
            while (true)
            {
                _readWriteLock.EnterWriteLock();
                try
                {
                    // Append text to the file
                    using (StreamWriter sw = File.AppendText(_log_filename))
                    {
                        sw.WriteLine(datet.ToString("yyyy/MM/dd hh:mm:ss") + " " + _severity + " > " + message);
                        sw.Close();
                        if ((int)_severity > 1)
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
                    _readWriteLock.ExitWriteLock();
                }
            }
        }
    }
}
