using MRPService.API.Types.API;
using System;
using System.Net;

namespace MRPService.API
{
    class PortalCredential : Core
    {
        public PortalCredential(MRP_ApiClient _MRP) : base(_MRP) {
        }
        public MRP_ApiClient MRP = new MRP_ApiClient();

        public MRPCredentialListType listcredentials()
        {
            endpoint = "/api/v1/credentials/list.json";
            MRPCommandControllerType worker = new MRPCommandControllerType();
            return (MRPCredentialListType)post<MRPCredentialListType>(worker);
        }

        public MRPCredentialType createcredential(MRPCredentialCRUDType _credential)
        {
            MRPCredentialsCRUDType platform = new MRPCredentialsCRUDType()
            {
                credential = _credential
            };
            endpoint = "/api/v1/credentials/create.json";
            return (MRPCredentialType)post<MRPCredentialType>(platform);
        }
        public MRPCredentialType updatecredential(MRPCredentialCRUDType _credential)
        {
            MRPCredentialsCRUDType credential = new MRPCredentialsCRUDType()
            {
                credential = _credential
            };
            endpoint = "/api/v1/credentials/update.json";
            return (MRPCredentialType)put<MRPCredentialType>(credential);
        }

    }
}


