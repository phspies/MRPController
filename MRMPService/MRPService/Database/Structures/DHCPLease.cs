using Newtonsoft.Json;
using System;
using SQLite.CodeFirst;
using System.ComponentModel.DataAnnotations;

namespace MRMPService.LocalDatabase
{
    public class DHCPLease
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("workload_id")]
        [MaxLength(50)]
        public string workload_id { get; set; }

        [JsonProperty("hostname")]
        [MaxLength(255)]
        public string hostname { get; set; }

        [JsonProperty("macaddress")]
        [MaxLength(45)]
        public string macaddress { get; set; }

        [JsonProperty("dnsdomains")]
        [MaxLength(128)]
        public string dnsdomains { get; set; }

        [JsonProperty("dnsservers")]
        [MaxLength(128)]
        public string dnsservers { get; set; }

        [JsonProperty("ipv4address")]
        [MaxLength(45)]
        public string ipv4address { get; set; }

        [JsonProperty("ipv4mask")]
        [MaxLength(45)]
        public string ipv4mask { get; set; }

        [JsonProperty("ipv4gateway")]
        [MaxLength(45)]
        public String ipv4gateway { get; set; }


        [JsonProperty("ipv6address")]
        [MaxLength(128)]
        public string ipv6address { get; set; }

        [JsonProperty("ipv6mask")]
        [MaxLength(128)]
        public string ipv6mask { get; set; }

        [JsonProperty("ipv6gateway")]
        [MaxLength(128)]
        public String ipv6gateway { get; set; }



        [JsonProperty("issued")]
        public bool issued { get; set; }
        [JsonProperty("issued_at")]
        public DateTime issued_at { get; set; }

    }
}
