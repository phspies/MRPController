namespace MRMPService.API.Types.API
{
    class MRPManagerType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public string hostname { get; set; }
        public string version { get; set; }
        public string ipaddress { get; set; }
    }

    public class MRPManagerConfirmType
    {
        public Manager manager { get; set; }
    }

    public class Manager
    {
        public string organization_id { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
    }


}
