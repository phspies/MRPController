using DD.CBU.Compute.Api.Client;
using DD.CBU.Compute.Api.Contracts.Network20;
using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MRMPService.Tasks.DiscoveryPlatform
{
    public class DatacenterDiscovery
    {
        public static void DatacenterDiscoveryDo(MRPTaskType payload)
        {
            using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                _mrp_api.task().progress(payload, String.Format("Starting datacenter discovering process"), 5);
            }
            MRPPlatformType _platform = payload.submitpayload.platform;
            MRPCredentialType _platform_credentail = _platform.credential;
            switch (_platform.platformtype)
            {
                case "dimension_data":
                    ComputeApiClient CaaS = null;
                    MRPPlatformdatacenterListType _mrmp_datacenters = null;
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        try
                        {
                            _mrp_api.task().progress(payload, String.Format("Retrieving datacenters from {0} for type MCP", _platform.url), 10);
                            _mrmp_datacenters = _mrp_api.platformdatacenter().list(_platform);
                            _mrp_api.task().progress(payload, String.Format("Retrieving datacenters from platform for {0}", _platform.platform), 10);

                            CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform_credentail.username, _platform_credentail.password));
                            CaaS.Login().Wait();
                        }
                        catch (Exception ex)
                        {
                            _mrp_api.task().progress(payload, String.Format("Error Connecting to MCP {0}", ex.Message), 15);
                            _mrp_api.task().failcomplete(payload, ex.ToString());
                            return;
                        }



                        List<DatacenterType> _mcp_datacenters = CaaS.Infrastructure.GetDataCenters().Result.ToList();
                        if (_mcp_datacenters != null)
                        {
                            _mrp_api.task().progress(payload, String.Format("Found {0} datacenters", _mcp_datacenters.Count), 15);
                            foreach (DatacenterType _dc in _mcp_datacenters)
                            {
                                MRPPlatformdatacenterType _platform_datacenter = new MRPPlatformdatacenterType();
                                if (_mrmp_datacenters.platformdatacenters.Exists(x => x.moid == _dc.id))
                                {
                                    _platform_datacenter.id = _mrmp_datacenters.platformdatacenters.FirstOrDefault(x => x.moid == _dc.id).id;
                                }

                                _platform_datacenter.moid = _dc.id;
                                _platform_datacenter.diskspeeds = JsonConvert.SerializeObject(_dc.hypervisor.diskSpeed).Replace("\\", "");
                                _platform_datacenter.cpuspeeds = JsonConvert.SerializeObject(_dc.hypervisor.cpuSpeed).Replace("\\", "");
                                _platform_datacenter.vpn_url = _dc.vpnUrl;
                                _platform_datacenter.city = _dc.city;
                                _platform_datacenter.country = _dc.country;
                                _platform_datacenter.displayname = _dc.displayName;
                                _platform_datacenter.platform_id = _platform.id;

                                if (_mrmp_datacenters.platformdatacenters.Exists(x => x.moid == _dc.id))
                                {
                                    _mrp_api.platformdatacenter().update(_platform_datacenter);
                                }
                                else
                                {
                                    _mrp_api.platformdatacenter().create(_platform_datacenter);
                                }
                            }
                        }
                        else
                        {
                            _mrp_api.task().progress(payload, String.Format("Something went wrong, null based mcp server list"), 15);
                            _mrp_api.task().failcomplete(payload, String.Format("Something went wrong, null based mcp server list"));
                            return;
                        }
                        _mrp_api.task().progress(payload, String.Format("Successfully created/updated {0} datacenters", _mcp_datacenters.Count), 20);
                        _mrp_api.task().successcomplete(payload, String.Format("Successfully created/updated {0} datacenters", _mcp_datacenters.Count));
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
