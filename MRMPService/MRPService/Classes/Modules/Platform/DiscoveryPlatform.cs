using DD.CBU.Compute.Api.Client;
using MRMPService.API.Types.API;
using MRMPService.MRMPService.Types.API;
using System;
using System.Net;

namespace MRMPService.Tasks.DiscoveryPlatform
{
    public class DiscoveryPlatform
    {
        public static void DiscoveryPlatformDo(MRPTaskType payload)
        {
            using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
            {
                _mrp_api.task().progress(payload, String.Format("Starting platform discovering process"), 5);
            }
            MRPPlatformType _platform = payload.submitpayload.platform;
            MRPCredentialType _platform_credentail = _platform.credential;
            switch (_platform.platformtype)
            {
                case "dimension_data":
                    using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                    {
                        _mrp_api.task().progress(payload, String.Format("Retrieving datacenters from {0} for type MCP", _platform.url), 10);
                    }
                    try
                    {
                        ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform_credentail.username, _platform_credentail.password));
                        CaaS.Login().Wait();
                    }
                    catch (Exception ex)
                    {

                    }
                    break;
                case "hyperv":
                    break;
                case "vmware":
                    break;
            }
        }
    }
}
