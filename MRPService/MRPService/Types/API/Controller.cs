namespace MRPService.API.Types.API
{
    class MRPControllerType
    {
        public string controller_id
        {
            get
            {
                return Global.agent_id;
            }
        }
        public string controller_hostname { get; set; }
        public string controller_version { get; set; }
        public string controller_ipaddress { get; set; }
    }

    public class MRPControllerConfirmType
    {
        public Controller worker { get; set; }
    }

    public class Controller
    {
        public string organization_id { get; set; }
        public string message { get; set; }
    }


}
