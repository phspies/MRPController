using MRMPService.Modules.MRMPPortal.Contracts;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal
{
    public class MRPOrganization : Core
    {
        public MRPOrganization(MRMPApiClient _MRP) : base(_MRP) { }

        public MRPOrganizationType get()
        {
            endpoint = "/organization/get.json";
            return post<MRPOrganizationType>(null);
        }
        public ResultType update(MRPOrganizationCRUDType _organization)
        {
            endpoint = "/organization/update.json";
            return put<ResultType>(_organization);
        }

    }
}


