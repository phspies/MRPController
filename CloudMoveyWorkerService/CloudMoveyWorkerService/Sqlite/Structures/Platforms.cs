using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SQLite;

namespace CloudMoveyWorkerService.CloudMovey.Sqlite
{
    public class Platform 
    {
        public int id { get; set; }
        public string moid { get; set; }
        public PLATFORM platform { get; set; }
        public string credential_id { get; set; }
        public int type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string datacenter { get; set; }
        public int max_cores { get; set; }
        public int max_memory { get; set; }
        public int max_network { get; set; }
        public int max_disks { get; set; }
        public int max_disk_size { get; set; }
        public VENDOR vendor { get; set; }
        public string url { get; set; }
    }
    public enum PLATFORM { Source, Target }
    public enum VENDOR { DimensionData, VMWare, HyperV }
}
