
using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VM
{
    public class RepositoryVolumes : Core
    {

        public RepositoryVolumes(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public repositoryVolumeStateSet getRepositoryVolumeStateFromAllClusters_Method()
        {
            endpoint = "/repository_volumes/state";
            mediatype = "application/json";
            return get<repositoryVolumeStateSet>();
        }
    }
}
