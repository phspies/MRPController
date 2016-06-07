using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VM
{
    public class Clusters : Core
    {
        public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public restString getClusterName_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/name";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<restString>();
        }


        public void createClariionRaidGroups_Method(long clusterId, restInteger restInteger_object)
        {
            endpoint = "/clusters/{clusterId}/clariion_raid_groups";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            post(restInteger_object);
        }


        public UserVolumeSettings getUserVolumeSettings_Method(long volumeId, long clusterId)
        {
            endpoint = "/clusters/{clusterId}/volumes/{volumeId}/user_settings";
            endpoint.Replace("{volumeId}", volumeId.ToString());
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<UserVolumeSettings>();
        }


        public volumePage getClusterSANVolumes_Method(long clusterId, bool? refreshView = null, string product = null, long? min_volume_size = null, long? max_volume_size = null, string direction = null, string sort_by = null, int? page = null, int? pagesize = null, string vendor = null)
        {
            endpoint = "/clusters/{clusterId}/volumes";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (refreshView != null) { url_params.Add(new KeyValuePair<string, string>("refreshView", refreshView.ToString())); }
            if (product != null) { url_params.Add(new KeyValuePair<string, string>("product", product.ToString())); }
            if (min_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("min_volume_size", min_volume_size.ToString())); }
            if (max_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("max_volume_size", max_volume_size.ToString())); }
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }
            if (vendor != null) { url_params.Add(new KeyValuePair<string, string>("vendor", vendor.ToString())); }

            mediatype = "application/json";
            return get<volumePage>();
        }


        public void rescanSANSplittersInCluster_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/san_splitters/rescan";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put();
        }


        public volumePage getAvailableVolumes_Method(long clusterId, bool? refreshView = null, string vendor = null, string product = null, int? page = null, int? pagesize = null, long? min_volume_size = null, long? max_volume_size = null, string direction = null, string sort_by = null)
        {
            endpoint = "/clusters/{clusterId}/volumes/available";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (refreshView != null) { url_params.Add(new KeyValuePair<string, string>("refreshView", refreshView.ToString())); }
            if (vendor != null) { url_params.Add(new KeyValuePair<string, string>("vendor", vendor.ToString())); }
            if (product != null) { url_params.Add(new KeyValuePair<string, string>("product", product.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }
            if (min_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("min_volume_size", min_volume_size.ToString())); }
            if (max_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("max_volume_size", max_volume_size.ToString())); }
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }

            mediatype = "application/json";
            return get<volumePage>();
        }


        public void detachPhoenixCluster_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/phoenix_clusters";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            delete();
        }


        public void acquireClusterManagementIP_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/acquire_management_ip";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put();
        }


        public void releaseClusterManagementIP_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/release_management_ip";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put();
        }


        public void startClusterMaintenance_Method(long clusterId, restClusterMaintenanceMode restClusterMaintenanceMode_object)
        {
            endpoint = "/clusters/{clusterId}/maintenance";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put(restClusterMaintenanceMode_object);
        }


        public void finishClusterMaintenance_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/maintenance/finish";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put();
        }


        public void startRPAMaintenance_Method(long clusterId, int rpaNum)
        {
            endpoint = "/clusters/{clusterId}/rpas/{rpaNum}/maintenance/start";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{rpaNum}", rpaNum.ToString());
            mediatype = "*/*";
            put();
        }


        public void finishRPAMaintenance_Method(long clusterId, int rpaNum)
        {
            endpoint = "/clusters/{clusterId}/rpas/{rpaNum}/maintenance/finish";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{rpaNum}", rpaNum.ToString());
            mediatype = "*/*";
            put();
        }


        public ClariionVolumesContext getClariionVolumesContext_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/volumes/clariion/context";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<ClariionVolumesContext>();
        }


        public ClusterSANVolumesContext getClusterSANVolumesContext_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/context";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<ClusterSANVolumesContext>();
        }


        public SymmetrixArrayList getAvailableSymmetrixArrays_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/arrays/symmetrix/available";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<SymmetrixArrayList>();
        }


        public void setIRThrottlingPolicy_Method(long clusterId, ArrayIRThrottlingPolicy arrayIRThrottlingPolicy_object)
        {
            endpoint = "/clusters/{clusterId}/ir_throttling_policy";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put(arrayIRThrottlingPolicy_object);
        }


        public VolumeInformationAndPaths getVolumeInformationAndPaths_Method(long volumeId, long clusterId)
        {
            endpoint = "/clusters/{clusterId}/volumes/{volumeId}";
            endpoint.Replace("{volumeId}", volumeId.ToString());
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<VolumeInformationAndPaths>();
        }


        public CertificateInformation getVCCertificateInformation_Method(long clusterId, IPPort ipPort_object)
        {
            endpoint = "/clusters/{clusterId}/vc_certificate_information";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<CertificateInformation>(ipPort_object);
        }


        public ClusterStatistics getClusterStatistics_Method(string clusterId)
        {
            endpoint = "/clusters/{clusterId}/statistics";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<ClusterStatistics>();
        }


        public splitterUIDSet getAvailableSplittersToAttachToVolume_Method(long clusterId, string deviceId)
        {
            endpoint = "/clusters/{clusterId}/volumes/{deviceId}/available_splitters";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{deviceId}", deviceId.ToString());
            mediatype = "application/json";
            return get<splitterUIDSet>();
        }


        public splitterUIDSet getSplittersWithUnattachedVolumes_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/splitters_with_unattached_volumes";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<splitterUIDSet>();
        }


        public void rescanSANSplittersInAllClusters_Method()
        {
            endpoint = "/clusters/san_splitters/rescan";
            mediatype = "*/*";
            put();
        }


        public VolumeInformation getVolumeInformation_Method(long volumeId, long clusterId)
        {
            endpoint = "/clusters/{clusterId}/volumes/{volumeId}/information";
            endpoint.Replace("{volumeId}", volumeId.ToString());
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<VolumeInformation>();
        }


        public volumePathSet getVolumePaths_Method(long volumeId, long clusterId)
        {
            endpoint = "/clusters/{clusterId}/volumes/{volumeId}/paths";
            endpoint.Replace("{volumeId}", volumeId.ToString());
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<volumePathSet>();
        }


        public TransactionID verifyRPAState_Method(long clusterId, int rpaNum, VerifyRPAStateParam verifyRPAStateParam_object, long? timeout_in_seconds = null)
        {
            endpoint = "/clusters/{clusterId}/rpas/{rpaNum}/state/verify";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{rpaNum}", rpaNum.ToString());
            if (timeout_in_seconds != null) { url_params.Add(new KeyValuePair<string, string>("timeout_in_seconds", timeout_in_seconds.ToString())); }

            mediatype = "application/json";
            return post<TransactionID>(verifyRPAStateParam_object);
        }


        public void resolveSettings_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/settings/resolve";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put();
        }


        public restStringSet getAvailablePhoenixClustersForCluster_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/phoenix_clusters/available";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<restStringSet>();
        }


        public rpaStatisticsSet getAllRPAStatisticsFromCluster_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/rpas/statistics";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<rpaStatisticsSet>();
        }


        public RPAStatistics getRPAStatistics_Method(int rpaID, long clusterId)
        {
            endpoint = "/clusters/{clusterId}/rpas/{rpaID}/statistics";
            endpoint.Replace("{rpaID}", rpaID.ToString());
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<RPAStatistics>();
        }


        public void rescanVolumesInAllClusters_Method(bool? checkExistingVolumesAsWell = null)
        {
            endpoint = "/clusters/rescan_all";
            if (checkExistingVolumesAsWell != null) { url_params.Add(new KeyValuePair<string, string>("checkExistingVolumesAsWell", checkExistingVolumesAsWell.ToString())); }

            mediatype = "*/*";
            put();
        }


        public void rescanVolumesInCluster_Method(long clusterId, bool? checkExistingVolumesAsWell = null)
        {
            endpoint = "/clusters/{clusterId}/rescan";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (checkExistingVolumesAsWell != null) { url_params.Add(new KeyValuePair<string, string>("checkExistingVolumesAsWell", checkExistingVolumesAsWell.ToString())); }

            mediatype = "*/*";
            put();
        }


        public volumePage getAvailableClariionVolumes_Method(long clusterId, bool? refreshView = null, string vendor = null, string product = null, long? min_volume_size = null, long? max_volume_size = null, int? page = null, int? pagesize = null, string direction = null, string sort_by = null)
        {
            endpoint = "/clusters/{clusterId}/volumes/available/clariion";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (refreshView != null) { url_params.Add(new KeyValuePair<string, string>("refreshView", refreshView.ToString())); }
            if (vendor != null) { url_params.Add(new KeyValuePair<string, string>("vendor", vendor.ToString())); }
            if (product != null) { url_params.Add(new KeyValuePair<string, string>("product", product.ToString())); }
            if (min_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("min_volume_size", min_volume_size.ToString())); }
            if (max_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("max_volume_size", max_volume_size.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }

            mediatype = "application/json";
            return get<volumePage>();
        }


        public volumePage getClariionVolumes_Method(long clusterId, bool? refreshView = null, int? page = null, int? pagesize = null, string vendor = null, string product = null, long? min_volume_size = null, long? max_volume_size = null, string direction = null, string sort_by = null)
        {
            endpoint = "/clusters/{clusterId}/volumes/clariion";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (refreshView != null) { url_params.Add(new KeyValuePair<string, string>("refreshView", refreshView.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }
            if (vendor != null) { url_params.Add(new KeyValuePair<string, string>("vendor", vendor.ToString())); }
            if (product != null) { url_params.Add(new KeyValuePair<string, string>("product", product.ToString())); }
            if (min_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("min_volume_size", min_volume_size.ToString())); }
            if (max_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("max_volume_size", max_volume_size.ToString())); }
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }

            mediatype = "application/json";
            return get<volumePage>();
        }


        public ClusterSplittersState getAllSplittersStateFromCluster_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/splitters/state";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<ClusterSplittersState>();
        }


        public SplitterState getSingleSplitterStateFromCluster_Method(long splitterId, long clusterId)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}/state";
            endpoint.Replace("{splitterId}", splitterId.ToString());
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<SplitterState>();
        }


        public ClusterRPAsState getAllRPAsStateFromCluster_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/rpas/state";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<ClusterRPAsState>();
        }


        public RPAState getSingleRPAStateFromCluster_Method(long clusterId, int rpaId)
        {
            endpoint = "/clusters/{clusterId}/rpas/{rpaId}/state";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{rpaId}", rpaId.ToString());
            mediatype = "application/json";
            return get<RPAState>();
        }


        public RepositoryVolumeState getRepositoryVolumeStateFromCluster_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/repository_volume/state";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<RepositoryVolumeState>();
        }


        public ClusterSettings getClusterSettings_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/settings";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<ClusterSettings>();
        }


        public vCenterServerFiltersSet getAllVCenterServersFilters_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers_filters";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<vCenterServerFiltersSet>();
        }


        public volumePage getVirtualVolumes_Method(long clusterId, bool? refreshView = null, int? page = null, int? pagesize = null, string vendor = null, string product = null, long? min_volume_size = null, long? max_volume_size = null, string direction = null, string sort_by = null)
        {
            endpoint = "/clusters/{clusterId}/volumes/virtual";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (refreshView != null) { url_params.Add(new KeyValuePair<string, string>("refreshView", refreshView.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }
            if (vendor != null) { url_params.Add(new KeyValuePair<string, string>("vendor", vendor.ToString())); }
            if (product != null) { url_params.Add(new KeyValuePair<string, string>("product", product.ToString())); }
            if (min_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("min_volume_size", min_volume_size.ToString())); }
            if (max_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("max_volume_size", max_volume_size.ToString())); }
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }

            mediatype = "application/json";
            return get<volumePage>();
        }


        public volumePage getVplexVolumes_Method(long clusterId, bool? refreshView = null, int? page = null, int? pagesize = null, string vendor = null, string product = null, long? min_volume_size = null, long? max_volume_size = null, string direction = null, string sort_by = null)
        {
            endpoint = "/clusters/{clusterId}/volumes/vplex";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (refreshView != null) { url_params.Add(new KeyValuePair<string, string>("refreshView", refreshView.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }
            if (vendor != null) { url_params.Add(new KeyValuePair<string, string>("vendor", vendor.ToString())); }
            if (product != null) { url_params.Add(new KeyValuePair<string, string>("product", product.ToString())); }
            if (min_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("min_volume_size", min_volume_size.ToString())); }
            if (max_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("max_volume_size", max_volume_size.ToString())); }
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }

            mediatype = "application/json";
            return get<volumePage>();
        }


        public volumePage getSymmetrixVolumes_Method(long clusterId, bool? refreshView = null, int? page = null, int? pagesize = null, string vendor = null, string product = null, long? min_volume_size = null, long? max_volume_size = null, string direction = null, string sort_by = null)
        {
            endpoint = "/clusters/{clusterId}/volumes/symmetrix";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (refreshView != null) { url_params.Add(new KeyValuePair<string, string>("refreshView", refreshView.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }
            if (vendor != null) { url_params.Add(new KeyValuePair<string, string>("vendor", vendor.ToString())); }
            if (product != null) { url_params.Add(new KeyValuePair<string, string>("product", product.ToString())); }
            if (min_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("min_volume_size", min_volume_size.ToString())); }
            if (max_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("max_volume_size", max_volume_size.ToString())); }
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }

            mediatype = "application/json";
            return get<volumePage>();
        }


        public volumePage getAvailableVirtualVolumes_Method(long clusterId, bool? refreshView = null, int? page = null, int? pagesize = null, string vendor = null, string product = null, long? min_volume_size = null, long? max_volume_size = null, string direction = null, string sort_by = null)
        {
            endpoint = "/clusters/{clusterId}/volumes/available/virtual";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (refreshView != null) { url_params.Add(new KeyValuePair<string, string>("refreshView", refreshView.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }
            if (vendor != null) { url_params.Add(new KeyValuePair<string, string>("vendor", vendor.ToString())); }
            if (product != null) { url_params.Add(new KeyValuePair<string, string>("product", product.ToString())); }
            if (min_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("min_volume_size", min_volume_size.ToString())); }
            if (max_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("max_volume_size", max_volume_size.ToString())); }
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }

            mediatype = "application/json";
            return get<volumePage>();
        }


        public volumePage getAvailableVplexVolumes_Method(long clusterId, bool? refreshView = null, int? page = null, int? pagesize = null, string vendor = null, string product = null, long? min_volume_size = null, long? max_volume_size = null, string direction = null, string sort_by = null)
        {
            endpoint = "/clusters/{clusterId}/volumes/available/vplex";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (refreshView != null) { url_params.Add(new KeyValuePair<string, string>("refreshView", refreshView.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }
            if (vendor != null) { url_params.Add(new KeyValuePair<string, string>("vendor", vendor.ToString())); }
            if (product != null) { url_params.Add(new KeyValuePair<string, string>("product", product.ToString())); }
            if (min_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("min_volume_size", min_volume_size.ToString())); }
            if (max_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("max_volume_size", max_volume_size.ToString())); }
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }

            mediatype = "application/json";
            return get<volumePage>();
        }


        public volumePage getAvailableSymmetrixVolumes_Method(long clusterId, bool? refreshView = null, int? page = null, int? pagesize = null, string vendor = null, string product = null, long? min_volume_size = null, long? max_volume_size = null, string direction = null, string sort_by = null)
        {
            endpoint = "/clusters/{clusterId}/volumes/available/symmetrix";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (refreshView != null) { url_params.Add(new KeyValuePair<string, string>("refreshView", refreshView.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }
            if (vendor != null) { url_params.Add(new KeyValuePair<string, string>("vendor", vendor.ToString())); }
            if (product != null) { url_params.Add(new KeyValuePair<string, string>("product", product.ToString())); }
            if (min_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("min_volume_size", min_volume_size.ToString())); }
            if (max_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("max_volume_size", max_volume_size.ToString())); }
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }

            mediatype = "application/json";
            return get<volumePage>();
        }


        public volumePage getAvailableVolumesInfo_Method(bool? refreshView = null, string vendor = null, string product = null, int? page = null, int? pagesize = null, long? min_volume_size = null, long? max_volume_size = null, string direction = null, string sort_by = null)
        {
            endpoint = "/clusters/volumes/available";
            if (refreshView != null) { url_params.Add(new KeyValuePair<string, string>("refreshView", refreshView.ToString())); }
            if (vendor != null) { url_params.Add(new KeyValuePair<string, string>("vendor", vendor.ToString())); }
            if (product != null) { url_params.Add(new KeyValuePair<string, string>("product", product.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }
            if (min_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("min_volume_size", min_volume_size.ToString())); }
            if (max_volume_size != null) { url_params.Add(new KeyValuePair<string, string>("max_volume_size", max_volume_size.ToString())); }
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }

            mediatype = "application/json";
            return get<volumePage>();
        }


        public volumeInformationSet getVolumeInformationFromAllClusters_Method()
        {
            endpoint = "/clusters/volumes/information";
            mediatype = "application/json";
            return get<volumeInformationSet>();
        }


        public vmEntitiesInformationSet getClusterSpecificVMsEntitiesInformation_Method(long clusterId, vmUIDSet vmUIDSet_object)
        {
            endpoint = "/clusters/{clusterId}/virtual_machines/info/by_uids";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<vmEntitiesInformationSet>(vmUIDSet_object);
        }


        public vmStateSet getClusterSpecificVMsState_Method(long clusterId, vmUIDSet vmUIDSet_object)
        {
            endpoint = "/clusters/{clusterId}/virtual_machines/state/by_uids";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<vmStateSet>(vmUIDSet_object);
        }


        public ClusterVirtualInfrastructuresState getVirtualInfrastructuresStateFromCluster_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/virtual_infrastructures/state";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<ClusterVirtualInfrastructuresState>();
        }


        public clusterSupportedAMPAndArrayTypesSet getSupportedArrayManagementProviderAndArrayTypesForAllClusters_Method()
        {
            endpoint = "/clusters/supported_array_management_provider_and_array_types";
            mediatype = "application/json";
            return get<clusterSupportedAMPAndArrayTypesSet>();
        }


        public SupportedArrayManagementProviderAndArrayTypes getSupportedArrayManagementProviderAndArrayTypes_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/supported_array_management_provider_and_array_types";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<SupportedArrayManagementProviderAndArrayTypes>();
        }


        public clusterArrayManagementProvidersStateSet getArrayManagementProviderStateFromAllClusters_Method()
        {
            endpoint = "/clusters/array_management_providers/state";
            mediatype = "application/json";
            return get<clusterArrayManagementProvidersStateSet>();
        }


        public void setClusterAutoRegistrationInformation_Method(long clusterId, ClusterAutoRegistrationInformation clusterAutoRegistrationInformation_object)
        {
            endpoint = "/clusters/{clusterId}/auto_registration_information";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put(clusterAutoRegistrationInformation_object);
        }


        public ClusterVirtualInfraConfiguration getClusterVirtualInfraConfiguration_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/virtual_infra_configuration";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<ClusterVirtualInfraConfiguration>();
        }


        public VmReplicationCandidateClusters getVMsReplicationCandidateClusters_Method(long clusterId, existingVMParamsSet existingVMParamsSet_object)
        {
            endpoint = "/clusters/{clusterId}/replication_candidate_clusters";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<VmReplicationCandidateClusters>(existingVMParamsSet_object);
        }


        public ArrayManagementProviderSettings getArrayManagementProviderSettings_Method(long clusterId, long arrayManagementProviderId)
        {
            endpoint = "/clusters/{clusterId}/array_management_providers/{arrayManagementProviderId}/settings";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{arrayManagementProviderId}", arrayManagementProviderId.ToString());
            mediatype = "application/json";
            return get<ArrayManagementProviderSettings>();
        }



        public ClusterSplittersSettings getAllSplittersSettingsFromCluster_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/splitters/settings";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<ClusterSplittersSettings>();
        }


        public ClusterAvailableSplitters getAvailableSplittersSettingsFromCluster_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/splitters/available";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<ClusterAvailableSplitters>();
        }


        public restString getSplitterName_Method(long clusterId, long splitterId)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}/name";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{splitterId}", splitterId.ToString());
            mediatype = "application/json";
            return get<restString>();
        }


        public TransactionID verifySplitterState_Method(long clusterId, long splitterId, VerifySplitterStateParam verifySplitterStateParam_object, long? timeout_in_seconds = null)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}/state/verify";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{splitterId}", splitterId.ToString());
            if (timeout_in_seconds != null) { url_params.Add(new KeyValuePair<string, string>("timeout_in_seconds", timeout_in_seconds.ToString())); }

            mediatype = "application/json";
            return post<TransactionID>(verifySplitterStateParam_object);
        }


        public void attachVolumesToSplitter_Method(long clusterId, long splitterId, setVolumeParamSet setVolumeParamSet_object)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}/volumes/attach_volumes";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{splitterId}", splitterId.ToString());
            mediatype = "*/*";
            post(setVolumeParamSet_object);
        }


        public void attachVolumeToSplitter_Method(long clusterId, long splitterId, SetVolumeParam setVolumeParam_object)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}/volumes";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{splitterId}", splitterId.ToString());
            mediatype = "*/*";
            post(setVolumeParam_object);
        }


        public deviceUIDSet getAvailableVolumesToAttachToSplitter_Method(long clusterId, long splitterId, bool? filterUnseenVolumes = null)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}/available_volumes";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{splitterId}", splitterId.ToString());
            if (filterUnseenVolumes != null) { url_params.Add(new KeyValuePair<string, string>("filterUnseenVolumes", filterUnseenVolumes.ToString())); }

            mediatype = "application/json";
            return get<deviceUIDSet>();
        }


        public void setAttachedVolumesForSplitter_Method(long clusterId, long splitterId, setAttachedVolumesForSplitterParams setAttachedVolumesForSplitterParams_object)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}/volumes/attached";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{splitterId}", splitterId.ToString());
            mediatype = "*/*";
            post(setAttachedVolumesForSplitterParams_object);
        }


        public ClusterSplittersSANView getSplittersSANViewFromCluster_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/splitters/san_view";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<ClusterSplittersSANView>();
        }


        public void setSplitterAutoRegisterRPAsInitiatorsPolicy_Method(long clusterId, long splitterId, restBoolean restBoolean_object)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}/auto_register_rpas_initiator_policy";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{splitterId}", splitterId.ToString());
            mediatype = "*/*";
            put(restBoolean_object);
        }


        public void rescanSplittersVolumesConnectionsInCluster_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/splitters/volume_connections/rescan";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put();
        }


        public void rescanSplitterVolumesConnections_Method(long clusterId, long splitterId)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}/volume_connections/rescan";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{splitterId}", splitterId.ToString());
            mediatype = "*/*";
            put();
        }


        public void detachVolumeFromSplitter_Method(long clusterId, long splitterId, long volumeId)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}/volumes/{volumeId}";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{splitterId}", splitterId.ToString());
            endpoint.Replace("{volumeId}", volumeId.ToString());
            mediatype = "*/*";
            delete();
        }


        public SplitterSANView getSplitterSANView_Method(long clusterId, long splitterId)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}/san_view";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{splitterId}", splitterId.ToString());
            mediatype = "application/json";
            return get<SplitterSANView>();
        }


        public SplitterSettings getSplitterSettings_Method(long clusterId, long splitterId)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}/settings";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{splitterId}", splitterId.ToString());
            mediatype = "application/json";
            return get<SplitterSettings>();
        }


        public SplitterState getSplitterState_Method(long clusterId, long splitterId)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}/state";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{splitterId}", splitterId.ToString());
            mediatype = "application/json";
            return get<SplitterState>();
        }


        public void removeSplitter_Method(long clusterId, long splitterId)
        {
            endpoint = "/clusters/{clusterId}/splitters/{splitterId}";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{splitterId}", splitterId.ToString());
            mediatype = "*/*";
            delete();
        }


        public void addSplitter_Method(long clusterId, SplitterInfo splitterInfo_object)
        {
            endpoint = "/clusters/{clusterId}/splitters";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            post(splitterInfo_object);
        }


        public AvailableArrayResourcePools getAvailableArrayResourcePoolsFromCluster_Method(long clusterId, long arrayId, bool? use_cache = null, bool? filter_managed = null)
        {
            endpoint = "/clusters/{clusterId}/arrays/{arrayId}/resource_pools/available";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{arrayId}", arrayId.ToString());
            if (use_cache != null) { url_params.Add(new KeyValuePair<string, string>("use_cache", use_cache.ToString())); }
            if (filter_managed != null) { url_params.Add(new KeyValuePair<string, string>("filter_managed", filter_managed.ToString())); }

            mediatype = "application/json";
            return get<AvailableArrayResourcePools>();
        }


        public SymmetrixGateKeepersInfo getSymmetrixGateKeepersInfo_Method(long clusterId, string arrayId)
        {
            endpoint = "/clusters/{clusterId}/arrays/symmetrix/{arrayId}/gate_keepers_info";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{arrayId}", arrayId.ToString());
            mediatype = "application/json";
            return get<SymmetrixGateKeepersInfo>();
        }


        public SymmetrixHostIDsInfo getAccessIdsForSymmetrix_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/arrays/symmetrix/host_ids";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<SymmetrixHostIDsInfo>();
        }


        public ArrayIRThrottlingPolicy getIRThrottlingPolicy_Method(long clusterId, string arraySerial)
        {
            endpoint = "/clusters/{clusterId}/arrays/{arraySerial}/ir_throttling_policy";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{arraySerial}", arraySerial.ToString());
            mediatype = "application/json";
            return get<ArrayIRThrottlingPolicy>();
        }


        public ArrayState getArrayState_Method(long clusterId, long arrayId)
        {
            endpoint = "/clusters/{clusterId}/arrays/{arrayId}/state";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{arrayId}", arrayId.ToString());
            mediatype = "application/json";
            return get<ArrayState>();
        }


        public CertificateInformation getArrayCertificateInformation_Method(long clusterId, getArrayCertificateInformationParams getArrayCertificateInformationParams_object)
        {
            endpoint = "/clusters/{clusterId}/arrays/certificate_information";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<CertificateInformation>(getArrayCertificateInformationParams_object);
        }


        public void removeArray_Method(long clusterId, long arrayId)
        {
            endpoint = "/clusters/{clusterId}/arrays/{arrayId}";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{arrayId}", arrayId.ToString());
            mediatype = "*/*";
            delete();
        }


        public restBoolean checkArrayConnectivity_Method(long clusterId, checkArrayConnectivityParams checkArrayConnectivityParams_object)
        {
            endpoint = "/clusters/{clusterId}/arrays/check_connectivity";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<restBoolean>(checkArrayConnectivityParams_object);
        }


        public restString getArrayName_Method(long clusterId, long arrayId)
        {
            endpoint = "/clusters/{clusterId}/arrays/{arrayId}/name";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{arrayId}", arrayId.ToString());
            mediatype = "application/json";
            return get<restString>();
        }


        public void setArraySettings_Method(long clusterId, SetArraySettingsParams setArraySettingsParams_object)
        {
            endpoint = "/clusters/{clusterId}/arrays/{arrayId}/settings";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put(setArraySettingsParams_object);
        }


        public CertificateInformation getVPLEXCertificateInformation_Method(long clusterId, restString restString_object)
        {
            endpoint = "/clusters/{clusterId}/arrays/vplex_certificate_information";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<CertificateInformation>(restString_object);
        }


        public ArrayCapabilities getArrayCapabilitiesByType_Method(long clusterId, restArrayType restArrayType_object)
        {
            endpoint = "/clusters/{clusterId}/arrays/capabilities";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<ArrayCapabilities>(restArrayType_object);
        }


        public ArrayCapabilities getArrayCapabilitiesByID_Method(long clusterId, long arrayId)
        {
            endpoint = "/clusters/{clusterId}/arrays/{arrayId}/capabilities";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{arrayId}", arrayId.ToString());
            mediatype = "application/json";
            return get<ArrayCapabilities>();
        }


        public ClusterAvailableArrays getAvailableArraysFromCluster_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/arrays/available";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<ClusterAvailableArrays>();
        }


        public void unregisterArrayResourcePools_Method(long clusterId, resourcePoolUIDSet resourcePoolUIDSet_object)
        {
            endpoint = "/clusters/{clusterId}/arrays/resource_pools";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            delete(resourcePoolUIDSet_object);
        }


        public DefaultArrayResourcePool getDefaultArrayResourcePool_Method(long clusterId, DefaultArrayResourcePoolParams defaultArrayResourcePoolParams_object)
        {
            endpoint = "/clusters/{clusterId}/arrays/resource_pools/default";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<DefaultArrayResourcePool>(defaultArrayResourcePoolParams_object);
        }



        public restHostConnectivityStatus checkExternalHostConnectivity_Method(long clusterId, ExternalHostParams externalHostParams_object)
        {
            endpoint = "/clusters/{clusterId}/external_hosts/check_connectivity";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<restHostConnectivityStatus>(externalHostParams_object);
        }


        public restHostConnectivityStatus checkExistingExternalHostConnectivity_Method(long clusterId, long hostId)
        {
            endpoint = "/clusters/{clusterId}/external_hosts/{hostId}/check_connectivity";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{hostId}", hostId.ToString());
            mediatype = "application/json";
            return post<restHostConnectivityStatus>();
        }


        public void setExternalHostSettings_Method(long clusterId, long hostId, ExternalHostSettings externalHostSettings_object)
        {
            endpoint = "/clusters/{clusterId}/external_hosts/{hostId}/settings";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{hostId}", hostId.ToString());
            mediatype = "*/*";
            put(externalHostSettings_object);
        }


        public void removeExternalHost_Method(long clusterId, long hostId)
        {
            endpoint = "/clusters/{clusterId}/external_hosts/{hostId}";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{hostId}", hostId.ToString());
            mediatype = "*/*";
            delete();
        }


        public void addExternalHost_Method(long clusterId, ExternalHostParams externalHostParams_object)
        {
            endpoint = "/clusters/{clusterId}/external_hosts";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            post(externalHostParams_object);
        }


        public virtualNetworkConfigurationSet getVMAvailableNetworks_Method(long clusterId, string vmUID, string vcUID)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/{vcUID}/virtual_machines/{vmUID}/available_networks";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{vmUID}", vmUID.ToString());
            endpoint.Replace("{vcUID}", vcUID.ToString());
            mediatype = "application/json";
            return get<virtualNetworkConfigurationSet>();
        }


        public virtualNetworkConfigurationSet getVMsAvailableNetworks_Method(long clusterId, string vcUID, vmUIDSet vmUIDSet_object)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/{vcUID}/virtual_machines/available_networks";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{vcUID}", vcUID.ToString());
            mediatype = "application/json";
            return post<virtualNetworkConfigurationSet>(vmUIDSet_object);
        }


        public VmReplicationCandidateClusters getVMReplicationCandidateClusters_Method(long clusterId, string vmUID, string vcUID)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/{vcUID}/virtual_machines/{vmUID}/replication_candidate_clusters";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{vmUID}", vmUID.ToString());
            endpoint.Replace("{vcUID}", vcUID.ToString());
            mediatype = "application/json";
            return get<VmReplicationCandidateClusters>();
        }


        public TargetVmCandidates getTargetVMCandidates_Method(long clusterId, string vmUID, string vcUID, long? targetClusterUID = null, string targetVirtualCenterUID = null, string targetDatacenterUID = null, string targetEsxClusterUID = null)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/{vcUID}/virtual_machines/{vmUID}/target_vm_candidates";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{vmUID}", vmUID.ToString());
            endpoint.Replace("{vcUID}", vcUID.ToString());
            if (targetClusterUID != null) { url_params.Add(new KeyValuePair<string, string>("targetClusterUID", targetClusterUID.ToString())); }
            if (targetVirtualCenterUID != null) { url_params.Add(new KeyValuePair<string, string>("targetVirtualCenterUID", targetVirtualCenterUID.ToString())); }
            if (targetDatacenterUID != null) { url_params.Add(new KeyValuePair<string, string>("targetDatacenterUID", targetDatacenterUID.ToString())); }
            if (targetEsxClusterUID != null) { url_params.Add(new KeyValuePair<string, string>("targetEsxClusterUID", targetEsxClusterUID.ToString())); }

            mediatype = "application/json";
            return get<TargetVmCandidates>();
        }


        public void removeAllFilters_Method(long clusterId, string serverIP)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/{serverIP}/remove_all_filters";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{serverIP}", serverIP.ToString());
            mediatype = "*/*";
            put();
        }


        public VCenterServerFilters getVCenterServerFilters_Method(long clusterId, string serverIP)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/{serverIP}/vcenter_servers_filters";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{serverIP}", serverIP.ToString());
            mediatype = "application/json";
            return get<VCenterServerFilters>();
        }


        public void removeVCenterServerFilter_Method(long clusterId, string serverIP, restVCenterServerFilter restVCenterServerFilter_object)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/{serverIP}/remove_vcenter_server_filter";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{serverIP}", serverIP.ToString());
            mediatype = "*/*";
            put(restVCenterServerFilter_object);
        }


        public void setVCenterServerFilters_Method(long clusterId, string serverIP, VCenterServerFilters vCenterServerFilters_object)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/{serverIP}/vcenter_server_filters";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{serverIP}", serverIP.ToString());
            mediatype = "*/*";
            put(vCenterServerFilters_object);
        }


        public VCenterServerViewContext getVCenterServerViewContext_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/context";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<VCenterServerViewContext>();
        }


        public VCenterServerView getVCenterServer_Method(long clusterId, string serverIP, bool? shouldRescan = null)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/{serverIP}/vcenter_view";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{serverIP}", serverIP.ToString());
            if (shouldRescan != null) { url_params.Add(new KeyValuePair<string, string>("shouldRescan", shouldRescan.ToString())); }

            mediatype = "application/json";
            return get<VCenterServerView>();
        }


        public VCenterServer getVCenterServer_Method(long clusterId, string serverIP)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/{serverIP}";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{serverIP}", serverIP.ToString());
            mediatype = "application/json";
            return get<VCenterServer>();
        }


        public ClusterVCenterServerView getVCenterServersViewFromCluster_Method(long clusterId, bool? shouldRescan = null)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/view";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (shouldRescan != null) { url_params.Add(new KeyValuePair<string, string>("shouldRescan", shouldRescan.ToString())); }

            mediatype = "application/json";
            return get<ClusterVCenterServerView>();
        }


        public void setVCenterServerFilterForUnknownLuns_Method(long clusterId, string serverIP, bool? shouldFilterUnknownLuns = null)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/{serverIP}/filter_for_unknown_luns";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{serverIP}", serverIP.ToString());
            if (shouldFilterUnknownLuns != null) { url_params.Add(new KeyValuePair<string, string>("shouldFilterUnknownLuns", shouldFilterUnknownLuns.ToString())); }

            mediatype = "*/*";
            post();
        }


        public vmEntitiesInformationSet getAvailableVMsForReplication_Method(long clusterId, string esxClusterUID, string dcUID, string vcUID)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers/{vcUID}/{dcUID}/{esxClusterUID}/available_vms_for_replication";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            endpoint.Replace("{esxClusterUID}", esxClusterUID.ToString());
            endpoint.Replace("{dcUID}", dcUID.ToString());
            endpoint.Replace("{vcUID}", vcUID.ToString());
            mediatype = "application/json";
            return get<vmEntitiesInformationSet>();
        }


        public ClusterVCenterServers getAllVCenterServers_Method(long clusterId)
        {
            endpoint = "/clusters/{clusterId}/vcenter_servers";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return get<ClusterVCenterServers>();
        }


        public RecoverPointClustersInformation getRecoverPointClustersInformation_Method()
        {
            endpoint = "/clusters";
            mediatype = "application/json";
            return get<RecoverPointClustersInformation>();
        }
    }
}
