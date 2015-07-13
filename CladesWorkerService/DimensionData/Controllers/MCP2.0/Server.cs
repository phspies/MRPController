using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.CaaS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CloudMoveyWorkerService.CaaS2
{
    class MCP2ServerObject : Core
    {
        public MCP2ServerObject(DimensionData _dimensiondata) : base(_dimensiondata) { }

        // Paging/Ordering Optional Parameters:
        // [&pageSize=]
        // [&pageNumber=]
        // [&orderBy=]
        //Filter Optional Parameters:
        // [&id=]
        // [&datacenterId=]
        // [&networkDomainId=]
        // [&networkId=]
        // [&vlanId=]
        // [&sourceImageId=]
        // [&deployed=]
        // [&name=]
        // [&createTime=]
        // [&state=]
        // [&started=]
        // [&operatingSystemId=]
        // [&ipv6=]
        // [&privateIpv4=]
        public ServerListType listservers(List<Option> options = null)
        {
            orgendpoint2("/server/server");
            urloptions = options;
            ServerListType servers = get<ServerListType>(null, true) as ServerListType;
            return servers;
        }
        public ResponseType deployserver(DeployServerType _server)
        {
            orgendpoint2("/server/deployServer");
            ResponseType response = post<ResponseType>(_server, false) as ResponseType;
            return response;
        }
        public ServerType getserver(String _server_id)
        {
            orgendpoint2(String.Format("/server/server/{0}",_server_id));
            ServerType server = get<ServerType>(null, true) as ServerType;
            return server;
        }
        public ResponseType deleteserver(DeleteServerType _server)
        {
            orgendpoint2("/server/deleteServer");
            ResponseType response = post<ResponseType>(_server, false) as ResponseType;
            return response;
        }
        public ResponseType serveraddnic(AddNicType _nic)
        {
            orgendpoint2("/server/addNic");
            ResponseType response = post<ResponseType>(_nic, false) as ResponseType;
            return response;
        }
        public ResponseType serverremovenic(RemoveNicType _nic)
        {
            orgendpoint2("/server/removeNic");
            ResponseType response = post<ResponseType>(_nic, false) as ResponseType;
            return response;
        }
        public ResponseType serverIpChange(NotifyNicIpChangeType _ipchange)
        {
            orgendpoint2("/server/notifyNicIpChange");
            ResponseType response = post<ResponseType>(_ipchange, false) as ResponseType;
            return response;
        }
        public ResponseType serverCleanFailedDeploy(CleanServerType _server)
        {
            orgendpoint2("/server/cleanServer");
            ResponseType response = post<ResponseType>(_server, false) as ResponseType;
            return response;
        }
        public ResponseType serverEnableMonitoring(EnableServerMonitoringType _monitor)
        {
            orgendpoint2("/server/enableServerMonitoring");
            _monitor.servicePlan = "ESSENTIALS";
            ResponseType response = post<ResponseType>(_monitor, false) as ResponseType;
            return response;
        }
        public ResponseType serverDisableMonitoring(DisableServerMonitoringType _server)
        {
            orgendpoint2("/server/disableServerMonitoring");
            ResponseType response = post<ResponseType>(_server, false) as ResponseType;
            return response;
        }
    }
}



