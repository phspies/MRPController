//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CloudMoveyWorkerService.CloudMovey.Sqlite.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Workload
    {
        public string id { get; set; }
        public string hostname { get; set; }
        public string platform_id { get; set; }
        public string credential_id { get; set; }
        public string hash_value { get; set; }
        public string failovergroup_id { get; set; }
        public string moid { get; set; }
        public Nullable<bool> enabled { get; set; }
        public Nullable<int> cpu_count { get; set; }
        public Nullable<int> memory_count { get; set; }
        public Nullable<long> storage_count { get; set; }
        public bool credential_ok { get; set; }
        public string application { get; set; }
        public string osedition { get; set; }
        public string ostype { get; set; }
        public Nullable<System.DateTime> last_contact_attempt { get; set; }
        public Nullable<int> last_contact_status { get; set; }
        public string last_contact_message { get; set; }
        public Nullable<int> failed_contact_attempts { get; set; }
    }
}
