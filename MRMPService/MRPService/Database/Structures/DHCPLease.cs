using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace MRMPService.LocalDatabase
{
    public class DHCPLease
    {
        [JsonProperty("Id")]
        [Key]
        public int Id { get; set; }

        [JsonProperty("workload_id")]
        [StringLength(50)]
        [Index]
        public string workload_id { get; set; }

        [JsonProperty("hostname")]
        [StringLength(255)]
        [Index]
        public string hostname { get; set; }

        [JsonProperty("macaddress")]
        [StringLength(45)]
        [Index]
        public string macaddress { get; set; }

        [JsonProperty("dnsdomains")]
        [StringLength(128)]
        public string dnsdomains { get; set; }

        [JsonProperty("dnsservers")]
        [StringLength(128)]
        public string dnsservers { get; set; }

        [JsonProperty("ipv4address")]
        [StringLength(45)]
        public string ipv4address { get; set; }

        [JsonProperty("ipv4mask")]
        [StringLength(45)]
        public string ipv4mask { get; set; }

        [JsonProperty("ipv4gateway")]
        [StringLength(45)]
        public String ipv4gateway { get; set; }


        [JsonProperty("ipv6address")]
        [StringLength(128)]
        public string ipv6address { get; set; }

        [JsonProperty("ipv6mask")]
        [StringLength(128)]
        public string ipv6mask { get; set; }

        [JsonProperty("ipv6gateway")]
        [StringLength(128)]
        public String ipv6gateway { get; set; }



        [JsonProperty("issued")]
        public bool issued { get; set; }
        [JsonProperty("issued_at")]
        public DateTime issued_at { get; set; }

    }
}
