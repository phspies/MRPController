namespace CloudMoveyWorkerService.Portal.Types.API
{
    class MoveyWorkerType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public string worker_version { get; set; }
        public string worker_ipaddress { get; set; }
    }

    public class MoveyWorkerRegisterType
    {
        public Worker worker { get; set; }
    }

    public class Worker
    {
        public string organization_id { get; set; }
    }


}
