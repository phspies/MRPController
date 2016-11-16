using MRMPService.MRMPAPI.Contracts;
using System;

namespace MRMPService.MRMPAPI
{
    class MRPOrganization : Core
    {
        public MRPOrganization(MRMP_ApiClient _MRP) : base(_MRP) { }

        public MRMP_ApiClient MRP = new MRMP_ApiClient();

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


