using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

// Code developed by InfernoDevelopment.com

namespace CloudMoveyWorkerService.CloudMoveyWorkerService.Log
{
    public class Logger
    {
        public enum Severity { Info, Debug, Error, Warn};
        public static void log(String message, Severity _severity)
        {
            DateTime datet = DateTime.Now;
            string loglocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            String filePath = "CMW_Log_" + datet.ToString("yyyy_MM_dd") + ".log";
            string logfilename = Path.Combine(loglocation, "Log", filePath);

            if (!File.Exists(logfilename))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logfilename));
                FileStream files = File.Create(logfilename);
                files.Close();
            }
            try
            {
                StreamWriter sw = File.AppendText(logfilename);
                sw.WriteLine(datet.ToString("yyyy/MM/dd hh:mm:ss") + " " + _severity + " > " + message);
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                Global.event_log.WriteEntry(e.Message.ToString(), EventLogEntryType.Error);
            }
        }
    }
}
