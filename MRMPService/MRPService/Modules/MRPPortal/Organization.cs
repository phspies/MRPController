using MRMPService.Modules.MRMPPortal.Contracts;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal
{
    public class MRPOrganization : Core
    {
        public MRPOrganization(MRMPApiClient _MRP) : base(_MRP) { }

        public async Task<MRPOrganizationType> get()
        {
            endpoint = "/organization/get.json";
            return await post<MRPOrganizationType>(null);
        }
        public async Task<ResultType> update(MRPOrganizationCRUDType _organization)
        {
            endpoint = "/organization/update.json";
            return await put<ResultType>(_organization);
        }

    }
}


