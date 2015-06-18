using CladesWorkerService.CaaS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CladesWorkerService.CaaS
{
    class NetworkObject : Core
    {
        public NetworkObject(DimensionData _dimensiondata) : base(_dimensiondata) { }

        public NetworkWithLocations networklist()
        {
            orgendpoint("/networkWithLocation");
            NetworkWithLocations networks = get<NetworkWithLocations>(null, true) as NetworkWithLocations;
            return networks;
        }

        //id}/networkWithLocation/{locationid}

        public NetworkWithLocations networklist(String location_id)
        {
            orgendpoint(String.Format("/networkWithLocation/{0}",location_id));
            NetworkWithLocations networks = get<NetworkWithLocations>(null, true) as NetworkWithLocations;
            return networks;
        }

        public Network networkget(String network_id)
        {
            orgendpoint(String.Format("/network/{0}", network_id));
            Network network = get<Network>(null, true) as Network;
            return network;
        }

        //NewNetworkWithLocation
        public Status networkcreate(String name, String location, String description = null)
        {
            NewNetworkWithLocation create = new NewNetworkWithLocation();
            create.name = name;
            create.location = location;
            create.description = description;
            orgendpoint("/networkWithLocation");
            Status status = post<Status>(create, false) as Status;
            return status;
        }

        public Status networkdelete(String network_id)
        {
            orgendpoint(String.Format("/network/{0}?delete", network_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        public Status networkmodify(String network_id, String name=null, String description=null)
        {
            ModifyNetwork modify = new ModifyNetwork();
            modify.name = name;
            modify.description = description;
            orgendpoint(String.Format("/network/{0}", network_id));
            Status status = post<Status>(modify, true) as Status;
            return status;
        }
    }
}
