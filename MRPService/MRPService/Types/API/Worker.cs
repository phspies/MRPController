namespace MRPService.API.Types.API
{
    class MRPWorkerType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public string worker_version { get; set; }
        public string worker_ipaddress { get; set; }
    }

    public class MRPWorkerRegisterType
    {
        public Worker worker { get; set; }
    }

    public class Worker
    {
        public string organization_id { get; set; }
        public string message { get; set; }
    }


}
