namespace MRMPService.MRMPService.Types.API
{

    public class MRPStacktreeCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPStacktreeType stacktree { get; set; }
    }
    public class MRPStacktreeType
    {
        public string id { get; set; }
        public string dt_job_id { get; set; }
    }
}
