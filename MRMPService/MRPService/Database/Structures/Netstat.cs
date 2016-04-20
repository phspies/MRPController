using System.ComponentModel.DataAnnotations;

namespace MRMPService.LocalDatabase
{

    public class Netstat
    {
        [Key, StringLength(50)]
        public string id { get; set; }
        [StringLength(50)]
        public string workload_id { get; set; }
        [StringLength(50)]
        public string proto { get; set; }
        [StringLength(50)]
        public string source_ip { get; set; }
        [StringLength(50)]
        public string target_ip { get; set; }
        public int source_port { get; set; }
        public int target_port { get; set; }
        [StringLength(20)]
        public string state { get; set; }
        public int pid { get; set; }
        [StringLength(255)]
        public string process { get; set; }
    }

}
