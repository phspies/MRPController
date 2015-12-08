using CloudMoveyWorkerService.CaaS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CaaS2
{
    class NetworkDomainObject : Core
    {
        public NetworkDomainObject(DimensionData _dimensiondata) : base(_dimensiondata) { }

        public NetworkDomainListType list(List<Option> _options)
        {
            orgendpoint("/network/networkDomain");
            urloptions = _options;
            NetworkDomainListType networks = get<NetworkDomainListType>(null, true) as NetworkDomainListType;
            return networks;
        }
        public ResponseType deploy(DeployNetworkDomainType _network)
        {

            orgendpoint("/network/deployNetworkDomain");
            ResponseType response = post<ResponseType>(_network, false) as ResponseType;
            return response;
        }

        public ResponseType edit(EditNetworkDomainType _network)
        {

            orgendpoint("/network/editNetworkDomain");
            ResponseType response = post<ResponseType>(_network, false) as ResponseType;
            return response;
        }
        public ResponseType edit(NetworkDomainType _network)
        {
            orgendpoint("/network/deleteNetworkDomain");
            ResponseType response = post<ResponseType>(_network, false) as ResponseType;
            return response;
        }
    }
}
