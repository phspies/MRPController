using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPPlatformtemplatesCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPPlatformtemplateType platformtemplate { get; set; }
    }
    public class MRPPlatformtemplateListType
    {
        public List<MRPPlatformtemplateType> platformtemplates { get; set; }
    }
    public class MRPPlatformtemplateType
        {
        public string id { get; set; }
        public string organization_id { get; set; }
        public string platform_id { get; set; }
        public string platform_moid { get; set; }
        public string image_moid { get; set; }
        public string image_name { get; set; }
        public string image_description { get; set; }
        public string image_type { get; set; }
        public string os_id { get; set; }
        public string os_displayname { get; set; }
        public string os_type { get; set; }
    }

}
