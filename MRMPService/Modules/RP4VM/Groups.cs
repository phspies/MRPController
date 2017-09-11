
using MRMPService.RP4VMAPI;
using MRMPService.Utilities;
using System.Collections.Generic;

namespace MRMPService.RP4VMTypes
{
    public class Groups : Core
    {
        public Groups(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public void renameGroup_Method(long groupId, restString restString_object)
        {
            endpoint = $"/groups/{groupId}/name";
            mediatype = "*/*";
            put(restString_object);
        }


        public void removeGroup_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}";
            mediatype = "*/*";
            delete();
        }


        public void createBookmark_Method(createBookmarkParams createBookmarkParams_object)
        {
            endpoint = "/groups/bookmarks";
            mediatype = "*/*";
            post(createBookmarkParams_object);
        }


        public void markGroupVolumesAsClean_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/volumes/mark_as_clean";
            mediatype = "*/*";
            put();
        }


        public consistencyGroupSettingsSet getAllGroupsSettings_Method()
        {
            endpoint = "/groups/settings";
            mediatype = "application/json";
            return get<consistencyGroupSettingsSet>();
        }


        public consistencyGroupSettingsSet getSpecificGroupsSettings_Method(consistencyGroupUIDSet ConsistencyGroupUIDSet_object)
        {
            endpoint = "/groups/specific/settings";
            mediatype = "application/json";
            return post<consistencyGroupSettingsSet>(ConsistencyGroupUIDSet_object);
        }


        public consistencyGroupStateSet getAllGroupsState_Method()
        {
            endpoint = "/groups/state";
            mediatype = "application/json";
            return get<consistencyGroupStateSet>();
        }


        public consistencyGroupStateSet getSpecificGroupsState_Method(consistencyGroupUIDSet ConsistencyGroupUIDSet_object)
        {
            endpoint = "/groups/specific/state";
            mediatype = "application/json";
            return post<consistencyGroupStateSet>(ConsistencyGroupUIDSet_object);
        }


        public ConsistencyGroupState getGroupState_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/state";
            mediatype = "application/json";
            return get<ConsistencyGroupState>();
        }


        public consistencyGroupStatisticsSet getAllGroupsStatistics_Method()
        {
            endpoint = "/groups/statistics";
            mediatype = "application/json";
            return get<consistencyGroupStatisticsSet>();
        }


        public consistencyGroupStatisticsSet getSpecificGroupsStatistics_Method(consistencyGroupUIDSet ConsistencyGroupUIDSet_object)
        {
            endpoint = "/groups/specific/statistics";
            mediatype = "application/json";
            return post<consistencyGroupStatisticsSet>(ConsistencyGroupUIDSet_object);
        }


        public ConsistencyGroupStatistics getGroupStatistics_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/statistics";
            mediatype = "application/json";
            return get<ConsistencyGroupStatistics>();
        }


        public ConsistencyGroupSnapshots getGroupSnapshots_Method(long groupId, long? start_time = null, long? end_time = null, string name = null)
        {
            endpoint = $"/groups/{groupId}/snapshots";
            if (start_time != null) { url_params.Add(new KeyValuePair<string, string>("start_time", start_time.ToString())); }
            if (end_time != null) { url_params.Add(new KeyValuePair<string, string>("end_time", end_time.ToString())); }
            if (name != null) { url_params.Add(new KeyValuePair<string, string>("name", name.ToString())); }

            mediatype = "application/json";
            return get<ConsistencyGroupSnapshots>();
        }


        public void pauseGroupTransfer_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/pause_transfer";
            mediatype = "*/*";
            put();
        }


        public void resumeProduction_Method(long groupId, bool? startTransfer = null)
        {
            endpoint = $"/groups/{groupId}/resume_production";
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "*/*";
            put();
        }


        public void markGroupVolumesAsDirty_Method(long groupId, markGroupVolumesAsDirtyParams markGroupVolumesAsDirtyParams_object)
        {
            endpoint = $"/groups/{groupId}/volumes/mark_as_dirty";
            mediatype = "*/*";
            put(markGroupVolumesAsDirtyParams_object);
        }


        public void startGroupTransfer_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/start_transfer";
            mediatype = "*/*";
            put();
        }


        public TransactionID verifyConsistencyGroupState_Method(long groupId, VerifyConsistencyGroupStateParam verifyConsistencyGroupStateParam_object, long? timeout_in_seconds = null)
        {
            endpoint = $"/groups/{groupId}/state/verify";
            if (timeout_in_seconds != null) { url_params.Add(new KeyValuePair<string, string>("timeout_in_seconds", timeout_in_seconds.ToString())); }

            mediatype = "application/json";
            return post<TransactionID>(verifyConsistencyGroupStateParam_object);
        }


        public replicationSetSettingsSet getAllGroupReplicationSets_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/replication_sets";
            mediatype = "application/json";
            return get<replicationSetSettingsSet>();
        }


        public void removeReplicationSet_Method(long groupId, long replicationSetId)
        {
            endpoint = $"/groups/{groupId}/replication_sets/{replicationSetId}";
            mediatype = "*/*";
            delete();
        }


        public restString getGroupReplicationSetName_Method(long groupId, long replicationSetId)
        {
            endpoint = $"/groups/{groupId}/replication_sets/{replicationSetId}/name";
            mediatype = "application/json";
            return get<restString>();
        }


        public ConsistencyGroupSettings getGroupSettings_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/settings";
            mediatype = "application/json";
            return get<ConsistencyGroupSettings>();
        }


        public void suspendConsistencyGroup_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/suspend";
            mediatype = "*/*";
            put();
        }


        public void resizeReplicationSet_Method(long groupId, long replicationSetId, ResizeReplicationSetParam resizeReplicationSetParam_object)
        {
            endpoint = $"/groups/{groupId}/replication_sets/{replicationSetId}/size";
            mediatype = "*/*";
            put(resizeReplicationSetParam_object);
        }


        public recoveryActivitiesSet getAllRecoveryActivities_Method()
        {
            endpoint = "/groups/recovery_activities";
            mediatype = "application/json";
            return get<recoveryActivitiesSet>();
        }


        public consistencyGroupsTransferStateSet getGroupsTransferState_Method()
        {
            endpoint = "/groups/transfer_state";
            mediatype = "application/json";
            return get<consistencyGroupsTransferStateSet>();
        }

        public void setReplicationSetsSettings_Method(long groupId, replicationSetSettingsChangesParamSet replicationSetSettingsChangesParamSet_object)
        {
            endpoint = $"/groups/{groupId}/replication_sets/settings";
            mediatype = "*/*";
            put(replicationSetSettingsChangesParamSet_object);
        }


        public void addDistributedConsistencyGroupWithDefaultPolicy_Method(DistributedConsistencyGroupWithDefaultPolicyParams distributedConsistencyGroupWithDefaultPolicyParams_object)
        {
            endpoint = "/groups/distributed";
            mediatype = "*/*";
            post(distributedConsistencyGroupWithDefaultPolicyParams_object);
        }


        public consistencyGroupVolumesStateSet getSpecificGroupsVolumeStates_Method(consistencyGroupUIDSet ConsistencyGroupUIDSet_object)
        {
            endpoint = "/groups/specific/volumes/state";
            mediatype = "application/json";
            return post<consistencyGroupVolumesStateSet>(ConsistencyGroupUIDSet_object);
        }


        public ConsistencyGroupCopySnapshots getRecoveryGroupCopySnapshots_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/recovery_copy/snapshots";
            mediatype = "application/json";
            return get<ConsistencyGroupCopySnapshots>();
        }


        public consistencyGroupCopySettingsSet getAllGroupCopies_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/copies/settings";
            mediatype = "application/json";
            return get<consistencyGroupCopySettingsSet>();
        }


        public void setGroupPolicy_Method(long groupId, ConsistencyGroupPolicy ConsistencyGroupPolicy_object)
        {
            endpoint = $"/groups/{groupId}/policy";
            mediatype = "*/*";
            put(ConsistencyGroupPolicy_object);
        }


        public void setLinkPolicy_Method(long groupId, setConsistencyGroupLinkPolicyParams setConsistencyGroupLinkPolicyParams_object)
        {
            endpoint = $"/groups/{groupId}/set_link_policy";
            mediatype = "*/*";
            put(setConsistencyGroupLinkPolicyParams_object);
        }


        public void removeVmsFromCG_Method(long groupId, vmUIDSet vmUIDSet_object)
        {
            endpoint = $"/groups/{groupId}/virtual_machines";
            mediatype = "*/*";
            delete(vmUIDSet_object);
        }


        public ConsistencyGroupUID replicateVms_Method(ReplicateVmsParam replicateVmsParam_object)
        {
            endpoint = "/groups/virtual_machines/replicate";
            mediatype = "application/json";
            return post<ConsistencyGroupUID>(replicateVmsParam_object);
        }


        public void changeGroupPowerUpSequence_Method(long groupId, restLong restLong_object)
        {
            endpoint = $"/groups/{groupId}/virtual_machines/group_powerup_sequence";
            mediatype = "*/*";
            put(restLong_object);
        }


        public void editVMsReplicationParams_Method(long groupId, virtualDisksReplicationPolicyParamSet virtualDisksReplicationPolicyParamSet_object)
        {
            endpoint = $"/groups/{groupId}/virtual_machines/replication_params";
            mediatype = "*/*";
            put(virtualDisksReplicationPolicyParamSet_object);
        }


        public void editVMsHardwareReplicationParams_Method(long groupId, virtualHardwareReplicationPolicyParamSet virtualHardwareReplicationPolicyParamSet_object)
        {
            endpoint = $"/groups/{groupId}/virtual_machines/hardware_replication_params";
            mediatype = "*/*";
            put(virtualHardwareReplicationPolicyParamSet_object);
        }


        public consistencyGroupReportContextInfoSet getConsistencyGroupReportsContexts_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/reports";
            mediatype = "application/json";
            return get<consistencyGroupReportContextInfoSet>();
        }


        public void deleteConsistencyGroupReports_Method(long groupId, long reportId)
        {
            endpoint = $"/groups/{groupId}/reports/{reportId}/delete";
            mediatype = "*/*";
            delete();
        }


        public consistencyGroupInformationSet getAllConsistencyGroupsInformation_Method()
        {
            endpoint = "/groups/information";
            mediatype = "application/json";
            return get<consistencyGroupInformationSet>();
        }


        public ConsistencyGroupInformation getConsistencyGroupInformation_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/information";
            mediatype = "application/json";
            return get<ConsistencyGroupInformation>();
        }


        public void changeVmsPowerUpSequence_Method(long groupId, vmPowerUpSequenceParamSet vmPowerUpSequenceParamSet_object)
        {
            endpoint = $"/groups/{groupId}/virtual_machines/powerup_sequence";
            mediatype = "*/*";
            put(vmPowerUpSequenceParamSet_object);
        }


        public void setVMsReplicationSetsPolicies_Method(long groupId, fullVmReplicationSetPoliciesSet fullVmReplicationSetPoliciesSet_object)
        {
            endpoint = $"/groups/{groupId}/vm_replication_sets_policy";
            mediatype = "*/*";
            put(fullVmReplicationSetPoliciesSet_object);
        }


        public consistencyGroupVolumesStateSet getAllGroupsVolumesState_Method()
        {
            endpoint = "/groups/volumes/state";
            mediatype = "application/json";
            return get<consistencyGroupVolumesStateSet>();
        }


        public ConsistencyGroupVolumesState getGroupVolumesState_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/volumes/state";
            mediatype = "application/json";
            return get<ConsistencyGroupVolumesState>();
        }


        public ConsistencyGroupUID addDefaultGroup_Method(addConsistencyGroupWithDefaultPolicyParams addConsistencyGroupWithDefaultPolicyParams_object)
        {
            endpoint = "/groups/default_group";
            mediatype = "application/json";
            return post<ConsistencyGroupUID>(addConsistencyGroupWithDefaultPolicyParams_object);
        }


        public void addGroupAndCopies_Method(FullConsistencyGroupPolicy fullConsistencyGroupPolicy_object)
        {
            endpoint = "/groups/group_and_copies";
            mediatype = "*/*";
            post(fullConsistencyGroupPolicy_object);
        }


        public ReplicationSetSettings getGroupReplicationSet_Method(long groupId, long replicationSetId)
        {
            endpoint = $"/groups/{groupId}/replication_sets/{replicationSetId}/settings";
            mediatype = "application/json";
            return get<ReplicationSetSettings>();
        }


        public void addLink_Method(long groupId, addConsistencyGroupLinkParams addConsistencyGroupLinkParams_object)
        {
            endpoint = $"/groups/{groupId}/links";
            mediatype = "*/*";
            post(addConsistencyGroupLinkParams_object);
        }


        public void addcopy_Method(long groupId, ConsistencyGroupCopySettingsParam ConsistencyGroupCopySettingsParam_object)
        {
            endpoint = $"/groups/{groupId}/copies";
            mediatype = "*/*";
            post(ConsistencyGroupCopySettingsParam_object);
        }


        public void disableGroup_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/disable";
            mediatype = "*/*";
            put();
        }


        public void enableGroup_Method(long groupId, bool? startTransfer = null)
        {
            endpoint = $"/groups/{groupId}/enable";
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "*/*";
            put();
        }


        public void removePassiveLink_Method(long groupId, removePassiveConsistencyGroupLinkParams removePassiveConsistencyGroupLinkParams_object)
        {
            endpoint = $"/groups/{groupId}/remove_passive_links";
            mediatype = "*/*";
            put(removePassiveConsistencyGroupLinkParams_object);
        }


        public void resumeGroup_Method(long groupId)
        {
            endpoint = $"/groups/{groupId}/resume";
            mediatype = "*/*";
            put();
        }


        public void setFullGroupPolicy_Method(long groupId, FullConsistencyGroupPolicy fullConsistencyGroupPolicy_object)
        {
            endpoint = $"/groups/{groupId}/full_policy";
            mediatype = "*/*";
            put(fullConsistencyGroupPolicy_object);
        }


        public void disableCopy_Method(long groupId, long clusterId, int copyId)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/disable";
            mediatype = "*/*";
            put();
        }


        public void enableCopy_Method(long groupId, long clusterId, int copyId, bool? startTransfer = null)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable";
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "*/*";
            put();
        }


        public void consolidateSnapshot_Method(long groupId, long clusterId, int copyId, consolidateSnapshotsParams consolidateSnapshotsParams_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/consolidate";
            mediatype = "*/*";
            put(consolidateSnapshotsParams_object);
        }


        public void enableImageAccessWithGeneralParams_Method(long groupId, long clusterId, int copyId, restImageAccessGeneralParameters restImageAccessGeneralParameters_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable_image_access_with_general_params";
            mediatype = "*/*";
            put(restImageAccessGeneralParameters_object);
        }


        public void markAsClean_Method(long groupId, long clusterId, int copyId)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/volumes/mark_as_clean";
            mediatype = "*/*";
            put();
        }


        public void markAsDirty_Method(long groupId, long clusterId, int copyId, markGroupCopyVolumesAsDirtyParams markGroupCopyVolumesAsDirtyParams_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/volumes/mark_as_dirty";
            mediatype = "*/*";
            put(markGroupCopyVolumesAsDirtyParams_object);
        }


        public void pauseTransfer_Method(long groupId, long clusterId, int copyId)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/pause_transfer";
            mediatype = "*/*";
            put();
        }


        public restString getGroupCopyName_Method(long groupId, long clusterId, int copyId)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/name";
            mediatype = "application/json";
            return get<restString>();
        }


        public void startCopyTransfer_Method(long groupId, long clusterId, int copyId)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/start_transfer";
            mediatype = "*/*";
            put();
        }


        public void undoWrites_Method(long groupId, long clusterId, int copyId)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/undo_writes";
            mediatype = "*/*";
            put();
        }


        public void setBookmarksSettings_Method(long groupId, long clusterId, int copyId, long snapshotId, SnapshotBookmarkingSettingsInfo snapshotBookmarkingSettingsInfo_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/{snapshotId}/bookmark_settings";
            mediatype = "*/*";
            put(snapshotBookmarkingSettingsInfo_object);
        }


        public void setConsistencyGroupTopology_Method(long groupId, long clusterId, int copyId, ConsistencyGroupTopologyParams ConsistencyGroupTopologyParams_object, bool? start_transfer = null)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/topology";
            if (start_transfer != null) { url_params.Add(new KeyValuePair<string, string>("start_transfer", start_transfer.ToString())); }

            mediatype = "*/*";
            put(ConsistencyGroupTopologyParams_object);
        }


        public void failover_Method(long groupId, long clusterId, int copyId, bool? startTransfer = null)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/failover";
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "*/*";
            put();
        }


        public void abortSnapshotsConsolidation_Method(long groupId, long clusterId, int copyId)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/abort_consolidation";
            mediatype = "*/*";
            put();
        }


        public void disableImageAccess_Method(long groupId, long clusterId, int copyId, bool? startTransfer = null)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/disable_image_access";
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "*/*";
            put();
        }


        public void enableImageAccessWithParams_Method(long groupId, long clusterId, int copyId, enableImageAccessWithParams enableImageAccessWithParams_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable_image_access_with_params";
            mediatype = "*/*";
            put(enableImageAccessWithParams_object);
        }


        public void enableImageAccess_Method(long groupId, long clusterId, int copyId, enableImageAccessParams enableImageAccessParams_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable_image_access";
            mediatype = "*/*";
            put(enableImageAccessParams_object);
        }


        public ConsistencyGroupCopySnapshots getGroupCopySnapshots_Method(long groupId, long clusterId, int copyId, string start_time = null, string end_time = null, string name = null)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots";
            if (start_time != null) { url_params.Add(new KeyValuePair<string, string>("start_time", start_time.ToString())); }
            if (end_time != null) { url_params.Add(new KeyValuePair<string, string>("end_time", end_time.ToString())); }
            if (name != null) { url_params.Add(new KeyValuePair<string, string>("name", name.ToString())); }

            mediatype = "application/json";
            return get<ConsistencyGroupCopySnapshots>();
        }


        public void recoverProduction_Method(long groupId, long clusterId, int copyId, bool? startTransfer = null)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/recover_production";
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "*/*";
            put();
        }


        public void enableDirectAccess_Method(long groupId, long clusterId, int copyId)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable_direct_access";
            mediatype = "*/*";
            put();
        }


        public void forceLongInitialization_Method(long groupId, long clusterId, int copyId)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/force_long_initialization";
            mediatype = "*/*";
            put();
        }


        public TransactionID verifyGroupCopySnapshots_Method(long groupId, long clusterId, int copyId, VerifyGroupCopySnapshotsParam verifyGroupCopySnapshotsParam_object, long? timeout_in_seconds = null)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/verify";
            if (timeout_in_seconds != null) { url_params.Add(new KeyValuePair<string, string>("timeout_in_seconds", timeout_in_seconds.ToString())); }

            mediatype = "application/json";
            return post<TransactionID>(verifyGroupCopySnapshotsParam_object);
        }


        public void addJournalVolume_Method(long groupId, long clusterId, int copyId, DeviceUID deviceUID_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/journal_volumes";
            mediatype = "*/*";
            post(deviceUID_object);
        }


        public void removeJournalVolume_Method(long groupId, long clusterId, int copyId, string deviceId)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/journal_volumes/{deviceId.EscapeData()}";
            mediatype = "*/*";
            delete();
        }


        public void addUserVolume_Method(long groupId, long clusterId, int copyId, addUserVolumeParams addUserVolumeParams_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/user_volumes";
            mediatype = "*/*";
            post(addUserVolumeParams_object);
        }


        public void removeUserVolume_Method(long groupId, long clusterId, int copyId, string deviceId, ReplicationSetUID replicationSetUID_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/user_volumes/{deviceId.EscapeData()}";
            mediatype = "*/*";
            delete(replicationSetUID_object);
        }


        public void addAndAttachUserVolume_Method(long groupId, long clusterId, int copyId, long replicationSetId, DeviceUID deviceUID_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/replication_sets/{replicationSetId}/volumes";
            mediatype = "*/*";
            post(deviceUID_object);
        }


        public void setBookmarkConsistencyType_Method(long groupId, long clusterId, int copyId, long snapshotId, restSnapshotConsistencyType restSnapshotConsistencyType_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/{snapshotId}/consistency_type";
            mediatype = "*/*";
            put(restSnapshotConsistencyType_object);
        }


        public void setProductionCopy_Method(long groupId, long clusterId, int copyId, bool? startTransfer = null)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/set_production_copy";
            if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString())); }

            mediatype = "*/*";
            put();
        }


        public void setBookmarkConsolidationPolicy_Method(long groupId, long clusterId, int copyId, long snapshotId, restBookmarkConsolidationPolicy restBookmarkConsolidationPolicy_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/{snapshotId}/consolidation_policy";
            mediatype = "*/*";
            put(restBookmarkConsolidationPolicy_object);
        }


        public void setSignaturesUsageDuringInitialization_Method(long groupId, long clusterId, int copyId, restBoolean restBoolean_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/signature_usage_during_initialization";
            mediatype = "*/*";
            put(restBoolean_object);
        }


        public void moveToImage_Method(long groupId, long clusterId, int copyId, moveToImageParams moveToImageParams_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/move_to_image";
            mediatype = "*/*";
            put(moveToImageParams_object);
        }


        public void rollToImage_Method(long groupId, long clusterId, int copyId)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/roll_to_image";
            mediatype = "*/*";
            put();
        }


        public void setCopyPolicy_Method(long groupId, long clusterId, int copyId, ConsistencyGroupCopyPolicy ConsistencyGroupCopyPolicy_object, string name = null)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/policy";
            if (name != null) { url_params.Add(new KeyValuePair<string, string>("name", name.ToString())); }

            mediatype = "*/*";
            put(ConsistencyGroupCopyPolicy_object);
        }


        public void enableLatestImageAccess_Method(long groupId, long clusterId, int copyId, enableLatestImageAccessParams enableLatestImageAccessParams_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/image_access/latest/enable";
            mediatype = "*/*";
            put(enableLatestImageAccessParams_object);
        }


        public void dismissVmStartUpPrompts_Method(long groupId, long clusterId, int copyId, vmStartUpActionUIDSet vmStartUpActionUIDSet_object)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/vm_startup_prompts/dismiss";
            mediatype = "*/*";
            put(vmStartUpActionUIDSet_object);
        }


        public ConsistencyGroupCopySettings getGroupCopySettings_Method(long groupId, long clusterId, int copyId)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/settings";
            mediatype = "application/json";
            return get<ConsistencyGroupCopySettings>();
        }

        public void removeCopy_Method(long groupId, long clusterId, int copyId)
        {
            endpoint = $"/groups/{groupId}/clusters/{clusterId}/copies/{copyId}";
            mediatype = "*/*";
            delete();
        }

        public consistencyGroupUIDSet getAllGroupsUIDs_Method()
        {
            endpoint = "/groups";
            mediatype = "application/json";
            return get<consistencyGroupUIDSet>();
        }
    }
}
