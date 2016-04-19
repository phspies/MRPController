using System;
using System.IO;

namespace MRMPService.MRMPService.Log
{
    public class Logger
    {
        public enum Severity { Info, Debug, Error, Warn };
        public static void log(String message, Severity _severity)
        {
            message = message.Replace("\n", String.Empty);
            message = message.Replace("\r", String.Empty);
            DateTime datet = DateTime.Now;
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
                try
                {
                    StreamWriter sw = File.AppendText(logfilename);
                    sw.WriteLine(datet.ToString("yyyy/MM/dd hh:mm:ss") + " " + _severity + " > " + message);
                    sw.Flush();
                    sw.Close();
                    break;
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
