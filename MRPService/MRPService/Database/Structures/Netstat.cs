using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.LocalDatabase
{
    
    public class Netstat
    {
        [Key, StringLength(50)]
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
        [StringLength(50)]
        public string process { get; set; }
    }

}
