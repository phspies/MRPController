using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Portal.Types.API
{
    public class MoveyPlatformtemplatesCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MoveyPlatformtemplateCRUDType platformtemplate { get; set; }
    }
    public class MoveyPlatformtemplateCRUDType
    {
        public string id { get; set; }
        public string organization_id { get; set; }
        public string platform_moid { get; set; }
        public string image_moid { get; set; }
        public string image_name { get; set; }
        public string image_description { get; set; }
        public string image_type { get; set; }
        public string os_id { get; set; }
        public string os_displayname { get; set; }
        public string os_type { get; set; }
}
    public class MoveyPlatformtemplateListType
    {
        public List<MoveyPlatformtemplateType> platformtemplates { get; set; }
    }
    public class MoveyPlatformtemplateType
        {
        public string id { get; set; }
        public string organization_id { get; set; }
        public string platform_moid { get; set; }
        public string image_moid { get; set; }
        public string image_name { get; set; }
        public string image_description { get; set; }
        public string os_id { get; set; }
        public string os_displayname { get; set; }
        public string os_type { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

}
