using CloudMoveyWorkerService.CaaS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CaaS2
{
    class VLANObject : Core
    {
        public VLANObject(DimensionData _dimensiondata) : base(_dimensiondata) { }

        public VlanListType list(List<Option> options = null)
        {
            orgendpoint("/network/vlan");
            urloptions = options;
            VlanListType vlans = get<VlanListType>(null, true) as VlanListType;
            return vlans;
        }

        public VlanType getvlan(String vlan_id)
        {
            orgendpoint(String.Format("/network/vlan/{0}", vlan_id));
            VlanType vlan = get<VlanType>(null, true) as VlanType;
            return vlan;
        }

        //NewNetworkWithLocation
        public ResponseType deployvlan(DeployVlanType _network)
        {
            orgendpoint("/network/deployVlan");
            ResponseType status = post<ResponseType>(_network, false) as ResponseType;
            return status;
        }

        public ResponseType deletevlan(DeleteVlanType _vlan)
        {
            orgendpoint("/network/deleteVlan");
            ResponseType status = post<ResponseType>(_vlan, false) as ResponseType;
            return status;
        }
        public ResponseType editvlan(EditVlanType _vlan)
        {
            orgendpoint("/network/editVlan");
            ResponseType status = post<ResponseType>(_vlan, false) as ResponseType;
            return status;
        }
    }
}
