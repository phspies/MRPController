

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VMTypes
{

    public class vCenterServers : Core
    {

        public vCenterServers(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public restStringSet getSupportedPluginVersions_Method()
        {
            endpoint = "/vcenter_servers/available_plugin_versions";
            mediatype = "application/json";
            return get<restStringSet>();
        }


        public clusterVCenterServerViewSet getVCenterServersViewFromAllClusters_Method(bool? shouldRescan = null)
        {
            endpoint = "/vcenter_servers/view";
            if (shouldRescan != null) { url_params.Add(new KeyValuePair<string, string>("shouldRescan", shouldRescan.ToString())); }

            mediatype = "application/json";
            return get<clusterVCenterServerViewSet>();
        }


        public VCenterServersViewSettings getVCenterSettings_Method()
        {
            endpoint = "/vcenter_servers/settings";
            mediatype = "application/json";
            return get<VCenterServersViewSettings>();
        }


        public virtualCenterProtectedVmCountStateSet getProtectedVmCount_Method()
        {
            endpoint = "/vcenter_servers/state";
            mediatype = "application/json";
            return get<virtualCenterProtectedVmCountStateSet>();
        }


        public void changePluginVersion_Method(string virtualCenterUID, restString restString_object)
        {
            endpoint = "/vcenter_servers/{virtualCenterUID}/plugin_version";
            endpoint.Replace("{virtualCenterUID}", virtualCenterUID.ToString());
            mediatype = "*/*";
            put(restString_object);
        }


        public clusterVCenterServersSet getVCenterServersFromAllClusters_Method()
        {
            endpoint = "/vcenter_servers";
            mediatype = "application/json";
            return get<clusterVCenterServersSet>();
        }



    }
}
