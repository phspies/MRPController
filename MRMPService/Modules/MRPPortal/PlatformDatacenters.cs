using MRMPService.Modules.MRMPPortal.Contracts;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal
{
    public class PortalPlatformDatacenter : Core
    {
        public PortalPlatformDatacenter(MRMPApiClient _MRP) : base(_MRP) {
        }
         
        public MRPPlatformdatacenterListType list(MRPPlatformType _platform)
        {
            endpoint = "/platformdatacenters/list.json";
            MRPPlatformGETType _platform_filter = new MRPPlatformGETType();
            _platform_filter.platform_id = _platform.id;
            return post<MRPPlatformdatacenterListType>(_platform_filter);
        }

        public ResultType create(MRPPlatformdatacenterType _platform_datacenter)
        {
            MRPPlatformdatacenterCRUDType datacenter = new MRPPlatformdatacenterCRUDType()
            {
                platformdatacenter = _platform_datacenter
            };

            endpoint = "/platformdatacenters/create.json";
            return post<ResultType>(datacenter);
        }
        public ResultType update(MRPPlatformdatacenterType _platform_datacenter)
        {
            MRPPlatformdatacenterCRUDType datacenter = new MRPPlatformdatacenterCRUDType()
            {
                platformdatacenter = _platform_datacenter
            };

            endpoint = "/platformdatacenters/update.json";
            return put<ResultType>(datacenter);
        }
    }
}


