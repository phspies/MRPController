using MRMPService.API.Types.API;
using System;
using System.Net;

namespace MRMPService.API
{
    class PortalCredential : Core
    {
        public PortalCredential(MRP_ApiClient _MRP) : base(_MRP) {
        }
        public MRP_ApiClient MRP = new MRP_ApiClient();

        public MRPCredentialListType listcredentials()
        {
            endpoint = "/credentials/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return post<MRPCredentialListType>(worker);
        }
    }
}


