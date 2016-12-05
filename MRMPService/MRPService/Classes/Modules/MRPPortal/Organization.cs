using MRMPService.MRMPAPI.Contracts;
using System;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI
{
    public class MRPOrganization : Core
    {
        public MRPOrganization(MRMP_ApiClient _MRP) : base(_MRP) { }

        public MRMP_ApiClient MRP = new MRMP_ApiClient();

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


