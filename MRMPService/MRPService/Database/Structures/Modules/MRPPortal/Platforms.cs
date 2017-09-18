using MRMPService.Modules.MRMPPortal.Contracts;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal
{
    public class PortalPlatform : Core
    {
        public PortalPlatform(MRMPApiClient _MRP) : base(_MRP) {
        }
        public MRPPlatformListType list(MRPPlatformFilterPagedType _paged_filter_settings)
        {
            endpoint = "/platforms/list_paged_filtered.json";
            return post<MRPPlatformListType>(_paged_filter_settings) as MRPPlatformListType;
        }

        public MRPPlatformType get_by_id(string _platform_id)
        {
            endpoint = "/platforms/get.json";
            MRPPlatformGETType worker = new MRPPlatformGETType()
            {
                platform_id = _platform_id
            };
            return post<MRPPlatformType>(worker) as MRPPlatformType;
        }

        public ResultType create(MRPPlatformType _platform)
        {
            MRPPlatformsCRUDType platform = new MRPPlatformsCRUDType()
            {
                platform = _platform
            };

            endpoint = "/platforms/create.json";
            return post<ResultType>(platform);
        }
        public ResultType update(MRPPlatformType _platform)
        {
            MRPPlatformsCRUDType platform = new MRPPlatformsCRUDType()
            {
                platform = _platform
            };

            endpoint = "/platforms/update.json";
            return put<ResultType>(platform);
        }
    }
}


