using CladesWorkerService.CaaS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CladesWorkerService.CaaS
{
    class MCP2VLANObject : Core
    {
        public MCP2VLANObject(DimensionData _dimensiondata) : base(_dimensiondata) { }

        public VlanListType listvlan(List<Option> options = null)
        {
            orgendpoint2("/network/vlan");
            urloptions = options;
            VlanListType vlans = get<VlanListType>(null, true) as VlanListType;
            return vlans;
        }

        public VlanType getvlan(String vlan_id)
        {
            orgendpoint2(String.Format("/network/vlan/{0}", vlan_id));
            VlanType vlan = get<VlanType>(null, true) as VlanType;
            return vlan;
        }

        //NewNetworkWithLocation
        public ResponseType deployvlan(NewNetworkWithLocation _network)
        {
            orgendpoint2("/network/deployVlan");
            ResponseType status = post<ResponseType>(_network, false) as ResponseType;
            return status;
        }

        public ResponseType deletevlan(string vlan_id)
        {
            orgendpoint2("/network/deleteVlan");
            dynamic vlanobject = new System.Dynamic.ExpandoObject();
            vlanobject.id = vlan_id;
            ResponseType status = post<ResponseType>(vlanobject, false) as ResponseType;
            return status;
        }
        public ResponseType editvlan(EditVlanType _vlan)
        {
            orgendpoint2("/network/editVlan");
            ResponseType status = post<ResponseType>(_vlan, false) as ResponseType;
            return status;
        }
    }
}
