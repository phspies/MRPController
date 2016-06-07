using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VM
{
    public class GroupSets : Core
    {

        public GroupSets(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public restString getGroupSetName_Method(long groupSetId)
        {
            endpoint = "/group_sets/{groupSetId}/name";
            endpoint.Replace("{groupSetId}", groupSetId.ToString());
            mediatype = "application/json";
            return get<restString>();
        }


        public void setBookmarkFrequency_Method(long groupSetId, long? frequencyInMicroSeconds = null)
        {
            endpoint = "/group_sets/{groupSetId}/bookmark_frequency";
            endpoint.Replace("{groupSetId}", groupSetId.ToString());
            if (frequencyInMicroSeconds != null) { url_params.Add(new KeyValuePair<string, string>("frequencyInMicroSeconds", frequencyInMicroSeconds.ToString())); }

            mediatype = "*/*";
            put();
        }


        public void removeGroupSet_Method(long groupSetId)
        {
            endpoint = "/group_sets/{groupSetId}";
            endpoint.Replace("{groupSetId}", groupSetId.ToString());
            mediatype = "*/*";
            delete();
        }


        public void setGroupSetSettings_Method(long groupSetId, ConsistencyGroupSetSettings consistencyGroupSetSettings_object)
        {
            endpoint = "/group_sets/{groupSetId}/settings";
            endpoint.Replace("{groupSetId}", groupSetId.ToString());
            mediatype = "*/*";
            put(consistencyGroupSetSettings_object);
        }


        public ConsistencyGroupSetProblems getIllegalEnableLatestImageAccessForGroupSetSubsetProblems_Method(long clusterId, enableLatestImageAccessForGroupSetSubsetParams enableLatestImageAccessForGroupSetSubsetParams_object)
        {
            endpoint = "/group_sets/clusters/{clusterId}/image_access/latest/problems";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(enableLatestImageAccessForGroupSetSubsetParams_object);
        }


        public void enableImageAccessForGroupSetSubset_Method(long clusterId, enableImageAccessForGroupSetsSubsetParams enableImageAccessForGroupSetsSubsetParams_object)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/image_access/enable";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put(enableImageAccessForGroupSetsSubsetParams_object);
        }


        public ConsistencyGroupSetProblems getIllegalEnableImageAccessGroupSetProblems_Method(long clusterId, enableImageAccessForGroupSetsSubsetParams enableImageAccessForGroupSetsSubsetParams_object)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/image_access/enable/problems";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(enableImageAccessForGroupSetsSubsetParams_object);
        }


        public void enableImageAccessForGroupSetSubsetWithGeneralParameters_Method(long clusterId, enableImageAccessForGroupSetSubsetWithGeneralParametersParams enableImageAccessForGroupSetSubsetWithGeneralParametersParams_object)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/image_access/enable_with_general_params";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put(enableImageAccessForGroupSetSubsetWithGeneralParametersParams_object);
        }


        public ConsistencyGroupSetProblems getIllegalEnableImageAccessGroupSetProblemsWithGeneralParameters_Method(long clusterId, enableImageAccessForGroupSetSubsetWithGeneralParametersParams enableImageAccessForGroupSetSubsetWithGeneralParametersParams_object)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/image_access/enable_with_general_params/problems";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(enableImageAccessForGroupSetSubsetWithGeneralParametersParams_object);
        }


        public void disableImageAccessForGroupSetSubset_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object, bool? startTransfer = null)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/image_access/disable";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "*/*";
            put(ConsistencyGroupSetSubset_object);
        }


        public ConsistencyGroupSetProblems getIllegalDisableImageAccessGroupSetProblems_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object, bool? startTransfer = null)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/image_access/disable/problems";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(ConsistencyGroupSetSubset_object);
        }


        public void enableDirectAccessForGroupSetSubset_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/direct_access/enable";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put(ConsistencyGroupSetSubset_object);
        }


        public ConsistencyGroupSetProblems getIllegalEnableDirectAccessForGroupSetProblems_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/direct_access/enable/problems";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(ConsistencyGroupSetSubset_object);
        }


        public void failoverGroupSetSubset_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object, bool? startTransfer = null)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/failover";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "*/*";
            put(ConsistencyGroupSetSubset_object);
        }


        public ConsistencyGroupSetProblems getIllegalFailoverGroupSetProblems_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object, bool? startTransfer = null)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/failover/problems";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(ConsistencyGroupSetSubset_object);
        }


        public void recoverProductionForGroupSetSubset_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object, bool? startTransfer = null)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterID}/recover_production";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "*/*";
            put(ConsistencyGroupSetSubset_object);
        }


        public ConsistencyGroupSetProblems getIllegalRecoverProductionForGroupSetProblems_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object, bool? startTransfer = null)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/recover_production/problems";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(ConsistencyGroupSetSubset_object);
        }


        public void rollToImageForGroupSetSubset_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/roll_to_image";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put(ConsistencyGroupSetSubset_object);
        }


        public ConsistencyGroupSetProblems getIllegalRollToImageForGroupSetProblems_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/roll_to_image/problems";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(ConsistencyGroupSetSubset_object);
        }


        public void createBookmarkForGroupSetSubset_Method(createBookmarkForGroupSetSubSetParams createBookmarkForGroupSetSubSetParams_object)
        {
            endpoint = "/group_sets/subsets/snapshots";
            mediatype = "*/*";
            post(createBookmarkForGroupSetSubSetParams_object);
        }


        public void resumeProductionForGroupSetSubset_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object, bool? startTransfer = null)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/resume_production";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "*/*";
            put(ConsistencyGroupSetSubset_object);
        }


        public ConsistencyGroupSetProblems getIllegalResumeProductionForGroupSetProblems_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object, bool? startTransfer = null)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/resume_production/problems";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(ConsistencyGroupSetSubset_object);
        }


        public void setProductionForGroupSetSubset_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object, bool? startTransfer = null)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/set_production";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "*/*";
            put(ConsistencyGroupSetSubset_object);
        }


        public ConsistencyGroupSetProblems getIllegalSetProductionForGroupSetProblems_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object, bool? startTransfer = null)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/set_production/problems";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(ConsistencyGroupSetSubset_object);
        }


        public void undoWritesForGroupSetSubset_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/undo_writes";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "*/*";
            put(ConsistencyGroupSetSubset_object);
        }


        public ConsistencyGroupSetProblems getIllegalUndoWritesForGroupSetProblems_Method(long clusterId, ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object)
        {
            endpoint = "/group_sets/subsets/clusters/{clusterId}/undo_writes/problems";
            endpoint.Replace("{clusterId}", clusterId.ToString());
            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(ConsistencyGroupSetSubset_object);
        }


        public void disableGroupsForGroupSetSubset_Method(ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object)
        {
            endpoint = "/group_sets/subsets/disable";
            mediatype = "*/*";
            put(ConsistencyGroupSetSubset_object);
        }


        public ConsistencyGroupSetProblems getIllegalDisableGroupsForGroupSetProblems_Method(ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object)
        {
            endpoint = "/group_sets/subsets/disable/problems";
            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(ConsistencyGroupSetSubset_object);
        }


        public void enableGroupsForGroupSetSubset_Method(ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object, bool? startTransfer = null)
        {
            endpoint = "/group_sets/subsets/enable";
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "*/*";
            put(ConsistencyGroupSetSubset_object);
        }


        public ConsistencyGroupSetProblems getIllegalEnableGroupsForGroupSetProblems_Method(ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object, bool? startTransfer = null)
        {
            endpoint = "/group_sets/subsets/enable/problems";
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(ConsistencyGroupSetSubset_object);
        }


        public void pauseTransferForGroupSetSubset_Method(ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object)
        {
            endpoint = "/group_sets/subsets/pause_transfer";
            mediatype = "*/*";
            put(ConsistencyGroupSetSubset_object);
        }


        public ConsistencyGroupSetProblems getIllegalPauseTransferForGroupSetProblems_Method(ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object)
        {
            endpoint = "/group_sets/subsets/pause_transfer/problems";
            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(ConsistencyGroupSetSubset_object);
        }


        public void startTransferForGroupSetSubset_Method(ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object)
        {
            endpoint = "/group_sets/subsets/start_transfer";
            mediatype = "*/*";
            put(ConsistencyGroupSetSubset_object);
        }


        public ConsistencyGroupSetProblems getIllegalStartTransferForGroupSetProblems_Method(ConsistencyGroupSetSubset ConsistencyGroupSetSubset_object)
        {
            endpoint = "/group_sets/subsets/start_transfer/problems";
            mediatype = "application/json";
            return post<ConsistencyGroupSetProblems>(ConsistencyGroupSetSubset_object);
        }


        public ConsistencyGroupSetUID addGroupSet_Method(ConsistencyGroupSetSettings consistencyGroupSetSettings_object)
        {
            endpoint = "/group_sets";
            mediatype = "application/json";
            return post<ConsistencyGroupSetUID>(consistencyGroupSetSettings_object);
        }



    }
}
