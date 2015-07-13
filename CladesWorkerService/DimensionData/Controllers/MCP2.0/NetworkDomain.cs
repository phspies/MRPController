using CloudMoveyWorkerService.CaaS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CaaS2
{
    class MCP2NetworkDomainObject : Core
    {
        public MCP2NetworkDomainObject(DimensionData _dimensiondata) : base(_dimensiondata) { }

        public NetworkDomainListType networkdomainlist()
        {
            orgendpoint2("/network/networkDomain");
            NetworkDomainListType networks = get<NetworkDomainListType>(null, true) as NetworkDomainListType;
            return networks;
        }
        public ResponseType deploynetworkdomain(DeployNetworkDomainType _network)
        {

            orgendpoint2("/network/deployNetworkDomain");
            ResponseType response = post<ResponseType>(_network, false) as ResponseType;
            return response;
        }

        public ResponseType editdomain(EditNetworkDomainType _network)
        {

            orgendpoint2("/network/editNetworkDomain");
            ResponseType response = post<ResponseType>(_network, false) as ResponseType;
            return response;
        }
        public ResponseType editdomain(NetworkDomainType _network)
        {
            orgendpoint2("/network/deleteNetworkDomain");
            ResponseType response = post<ResponseType>(_network, false) as ResponseType;
            return response;
        }
    }
}
