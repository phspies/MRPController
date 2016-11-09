using MRMPService.MRMPAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI;

namespace MRMPService.NetstatCollection
{
    class NetstatUpload
    {
        static public void Upload(List<NetworkFlowType> _workload_netstats, MRPWorkloadType _workload)
        {
            try
            {
                Logger.log("Starting Netflow Upload Thread", Logger.Severity.Debug);
                Stopwatch _sw = Stopwatch.StartNew();

                List<MRPNetworkFlowCRUDType> _netflow_list = new List<MRPNetworkFlowCRUDType>();
                foreach (NetworkFlowType _workload_netstat in _workload_netstats)
                {
                    _netflow_list.Add(new MRPNetworkFlowCRUDType()
                    {

                        protocol = _workload_netstat.protocol,
                        source_address = _workload_netstat.source_address,
                        source_port = _workload_netstat.source_port,
                        target_address = _workload_netstat.target_address,
                        target_port = _workload_netstat.target_port,
                        timestamp = _workload_netstat.timestamp,
                        pid = _workload_netstat.pid,
                        process = _workload_netstat.process,
                    });

                    if (_netflow_list.Count > Global.portal_upload_netflow_page_size)
                    {
                        using (MRMP_ApiClient _mrmp_api = new MRMP_ApiClient())
                        {
                            _mrmp_api.netflow().createnetworkflow(_netflow_list);
                        }
                        _netflow_list.Clear();
                    }
                }
                //upload last remaining records
                if (_netflow_list.Count > 0)
                {
                    using (MRMP_ApiClient _mrmp_api = new MRMP_ApiClient())
                    {
                        _mrmp_api.netflow().createnetworkflow(_netflow_list);
                    }
                }

                _sw.Stop();
                Logger.log(String.Format("Completed Netstat Upload for {0} in {1}", _workload.hostname, TimeSpan.FromMilliseconds(_sw.Elapsed.TotalMilliseconds)), Logger.Severity.Debug);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error uploading Netflow information to portal {0}", ex.ToString()), Logger.Severity.Error);
            }
        }
    }
}
