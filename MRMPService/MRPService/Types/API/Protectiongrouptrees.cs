namespace MRMPService.MRMPService.Types.API
{

    public class MRPProtectiongrouptreeCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPProtectiongrouptreeType protectiongrouptree { get; set; }
    }
    public class MRPProtectiongrouptreeType
    {
        public string id { get; set; }
        public string dt_job_id { get; set; }
    }
}
