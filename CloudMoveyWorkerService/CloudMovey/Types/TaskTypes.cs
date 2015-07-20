using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudMoveyWorkerService.CloudMovey.Types
{



    public class TaskListObject
    {
        public TaskObject[] Property1 { get; set; }
    }

    public class TaskObject
    {
    public string starttimestamp { get; set; }
    public string endtimestamp { get; set; }
    public string task { get; set; }
    public string status { get; set; }
    public string target_type { get; set; }
    public string target_id { get; set; }
    public string source_id { get; set; }
    public string payload { get; set; }
    }

    //Only used for talk updates
    public class TaskUpdateObject
    {
        public string id { get; set; }
        public string hostname { get; set; }
        public string task_id { get; set; }

        public TaskUpdateAttriubutes attributes { get; set; }
    }
    public class TaskUpdateAttriubutes
    {
        public string returnpayload { set; get; }
        public int status { set; get; }
    }

    public class ProgressTaskUpdateObject
    {
        public string id { get; set; }
        public string hostname { get; set; }
        public string task_id { get; set; }

        public ProgressTaskUpdateAttriubutes attributes { get; set; }
    }
    public class ProgressTaskUpdateAttriubutes
    {
        public string step { set; get; }
        public double percentage { set; get; }
    }

    public class CompleteTaskUpdateObject
    {
        public string id { get; set; }
        public string hostname { get; set; }
        public string task_id { get; set; }

        public CompleteTaskUpdateAttriubutes attributes { get; set; }
    }
    public class CompleteTaskUpdateAttriubutes
    {
        public string step { set; get; }
        public decimal percentage { set; get; }
        public string returnpayload { set; get; }
        public int status { set; get; }
    }
}

