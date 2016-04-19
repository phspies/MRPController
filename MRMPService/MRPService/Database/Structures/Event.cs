using System;
using System.ComponentModel.DataAnnotations;

namespace MRMPService.LocalDatabase
{

    public class Event
    {
        [Key, StringLength(50)]
        public string id { get; set; }
        [StringLength(30)]
        public string status { get; set; }
        public Nullable<int> severity { get; set; }
        [StringLength(50)]
        public string component { get; set; }
        [StringLength(50)]
        public string summary { get; set; }
        public Nullable<System.DateTime> timestamp { get; set; }
        [StringLength(50)]
        public string entity { get; set; }
    }
}
