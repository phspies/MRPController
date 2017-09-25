using System;
using System.ComponentModel.DataAnnotations;

namespace MRMPService.LocalDatabase
{
    public class DHCPLease
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string workload_id { get; set; }
        [MaxLength(255)]
        public string hostname { get; set; }
        [MaxLength(45)]
        public string macaddress { get; set; }
        [MaxLength(128)]
        public string dnsdomains { get; set; }
        [MaxLength(128)]
        public string dnsservers { get; set; }
        [MaxLength(45)]
        public string ipv4address { get; set; }
        [MaxLength(45)]
        public string ipv4mask { get; set; }
        [MaxLength(45)]
        public String ipv4gateway { get; set; }
        [MaxLength(128)]
        public string ipv6address { get; set; }
        [MaxLength(128)]
        public string ipv6mask { get; set; }
        [MaxLength(128)]
        public String ipv6gateway { get; set; }
        public bool issued { get; set; }
        public DateTime issued_at { get; set; }

    }
}
