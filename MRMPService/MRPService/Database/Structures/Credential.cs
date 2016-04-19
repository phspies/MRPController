using System.ComponentModel.DataAnnotations;

namespace MRMPService.LocalDatabase
{

    public class Credential
    {
        [Key, StringLength(50)]
        public string id { get; set; }
        [StringLength(100)]
        public string description { get; set; }
        [StringLength(30)]
        public string username { get; set; }
        [StringLength(30)]
        public string password { get; set; }
        [StringLength(30)]
        public string domain { get; set; }
        public int credential_type { get; set; }
        [StringLength(30)]
        public string human_type { get; set; }
        [StringLength(50)]
        public string hash_value { get; set; }
        public bool deleted { get; set; }
    }
}
