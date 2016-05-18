using MRMPService.MRMPAPI.Types.API;
using System;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class PortalCredential : Core
    {
        public PortalCredential(MRMP_ApiClient _MRP) : base(_MRP) {
        }
        public MRMP_ApiClient MRP = new MRMP_ApiClient();

        public MRPCredentialListType listcredentials()
        {
            endpoint = "/credentials/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return post<MRPCredentialListType>(worker);
        }
    }
}


