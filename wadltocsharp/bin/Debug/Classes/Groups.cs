using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Groups : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public void renameGroup_Method(long groupId,restString restString_object)
{
	endpoint = "/groups/{groupId}/name";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put(restString_object);
}


public void removeGroup_Method(long groupId)
{
	endpoint = "/groups/{groupId}";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	delete();
}


public void createBookmark_Method(createBookmarkParams createBookmarkParams_object)
{
	endpoint = "/groups/bookmarks";
	mediatype="*/*";
	post(createBookmarkParams_object);
}


public void markGroupVolumesAsClean_Method(long groupId)
{
	endpoint = "/groups/{groupId}/volumes/mark_as_clean";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put();
}


public ConsistencyGroupSettingsSet getAllGroupsSettings_Method()
{
	endpoint = "/groups/settings";
	mediatype="application/json";
	return get<ConsistencyGroupSettingsSet>();
}


public ConsistencyGroupSettingsSet getSpecificGroupsSettings_Method(consistencyGroupUIDSet consistencyGroupUIDSet_object)
{
	endpoint = "/groups/specific/settings";
	mediatype="application/json";
	return post<ConsistencyGroupSettingsSet>(consistencyGroupUIDSet_object);
}


public ConsistencyGroupStateSet getAllGroupsState_Method()
{
	endpoint = "/groups/state";
	mediatype="application/json";
	return get<ConsistencyGroupStateSet>();
}


public ConsistencyGroupStateSet getSpecificGroupsState_Method(consistencyGroupUIDSet consistencyGroupUIDSet_object)
{
	endpoint = "/groups/specific/state";
	mediatype="application/json";
	return post<ConsistencyGroupStateSet>(consistencyGroupUIDSet_object);
}


public ConsistencyGroupState getGroupState_Method(long groupId)
{
	endpoint = "/groups/{groupId}/state";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="application/json";
	return get<ConsistencyGroupState>();
}


public ConsistencyGroupStatisticsSet getAllGroupsStatistics_Method()
{
	endpoint = "/groups/statistics";
	mediatype="application/json";
	return get<ConsistencyGroupStatisticsSet>();
}


public ConsistencyGroupStatisticsSet getSpecificGroupsStatistics_Method(consistencyGroupUIDSet consistencyGroupUIDSet_object)
{
	endpoint = "/groups/specific/statistics";
	mediatype="application/json";
	return post<ConsistencyGroupStatisticsSet>(consistencyGroupUIDSet_object);
}


public ConsistencyGroupStatistics getGroupStatistics_Method(long groupId)
{
	endpoint = "/groups/{groupId}/statistics";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="application/json";
	return get<ConsistencyGroupStatistics>();
}


public ConsistencyGroupSnapshots getGroupSnapshots_Method(long groupId,long? start_time=null,long? end_time=null,string name=null)
{
	endpoint = "/groups/{groupId}/snapshots";
	endpoint.Replace("{groupId}",groupId.ToString());
if (start_time != null) { url_params.Add(new KeyValuePair<string, string>("start_time", start_time.ToString()));}
if (end_time != null) { url_params.Add(new KeyValuePair<string, string>("end_time", end_time.ToString()));}
if (name != null) { url_params.Add(new KeyValuePair<string, string>("name", name.ToString()));}

	mediatype="application/json";
	return get<ConsistencyGroupSnapshots>();
}


public void pauseGroupTransfer_Method(long groupId)
{
	endpoint = "/groups/{groupId}/pause_transfer";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put();
}


public void resumeProduction_Method(long groupId,bool? startTransfer=null)
{
	endpoint = "/groups/{groupId}/resume_production";
	endpoint.Replace("{groupId}",groupId.ToString());
if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString()));}

	mediatype="*/*";
	put();
}


public void markGroupVolumesAsDirty_Method(long groupId,markGroupVolumesAsDirtyParams markGroupVolumesAsDirtyParams_object)
{
	endpoint = "/groups/{groupId}/volumes/mark_as_dirty";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put(markGroupVolumesAsDirtyParams_object);
}


public void startGroupTransfer_Method(long groupId)
{
	endpoint = "/groups/{groupId}/start_transfer";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put();
}


public TransactionID verifyConsistencyGroupState_Method(long groupId,verifyConsistencyGroupStateParam verifyConsistencyGroupStateParam_object,long? timeout_in_seconds=null)
{
	endpoint = "/groups/{groupId}/state/verify";
	endpoint.Replace("{groupId}",groupId.ToString());
if (timeout_in_seconds != null) { url_params.Add(new KeyValuePair<string, string>("timeout_in_seconds", timeout_in_seconds.ToString()));}

	mediatype="application/json";
	return post<TransactionID>(verifyConsistencyGroupStateParam_object);
}


public ReplicationSetSettingsSet getAllGroupReplicationSets_Method(long groupId)
{
	endpoint = "/groups/{groupId}/replication_sets";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="application/json";
	return get<ReplicationSetSettingsSet>();
}


public void removeReplicationSet_Method(long groupId,long replicationSetId)
{
	endpoint = "/groups/{groupId}/replication_sets/{replicationSetId}";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{replicationSetId}",replicationSetId.ToString());
	mediatype="*/*";
	delete();
}


public RestString getGroupReplicationSetName_Method(long groupId,long replicationSetId)
{
	endpoint = "/groups/{groupId}/replication_sets/{replicationSetId}/name";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{replicationSetId}",replicationSetId.ToString());
	mediatype="application/json";
	return get<RestString>();
}


public ConsistencyGroupSettings getGroupSettings_Method(long groupId)
{
	endpoint = "/groups/{groupId}/settings";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="application/json";
	return get<ConsistencyGroupSettings>();
}


public void suspendConsistencyGroup_Method(long groupId)
{
	endpoint = "/groups/{groupId}/suspend";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put();
}


public void resizeReplicationSet_Method(long groupId,long replicationSetId,resizeReplicationSetParam resizeReplicationSetParam_object)
{
	endpoint = "/groups/{groupId}/replication_sets/{replicationSetId}/size";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{replicationSetId}",replicationSetId.ToString());
	mediatype="*/*";
	put(resizeReplicationSetParam_object);
}


public RecoveryActivitiesSet getAllRecoveryActivities_Method()
{
	endpoint = "/groups/recovery_activities";
	mediatype="application/json";
	return get<RecoveryActivitiesSet>();
}


public ConsistencyGroupsTransferStateSet getGroupsTransferState_Method()
{
	endpoint = "/groups/transfer_state";
	mediatype="application/json";
	return get<ConsistencyGroupsTransferStateSet>();
}


public void setReplicationSetsSettings_Method(long groupId,replicationSetSettingsChangesParamSet replicationSetSettingsChangesParamSet_object)
{
	endpoint = "/groups/{groupId}/replication_sets/settings";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put(replicationSetSettingsChangesParamSet_object);
}


public void addDistributedConsistencyGroupWithDefaultPolicy_Method(distributedConsistencyGroupWithDefaultPolicyParams distributedConsistencyGroupWithDefaultPolicyParams_object)
{
	endpoint = "/groups/distributed";
	mediatype="*/*";
	post(distributedConsistencyGroupWithDefaultPolicyParams_object);
}


public ConsistencyGroupVolumesStateSet getSpecificGroupsVolumeStates_Method(consistencyGroupUIDSet consistencyGroupUIDSet_object)
{
	endpoint = "/groups/specific/volumes/state";
	mediatype="application/json";
	return post<ConsistencyGroupVolumesStateSet>(consistencyGroupUIDSet_object);
}


public ConsistencyGroupCopySnapshots getRecoveryGroupCopySnapshots_Method(long groupId)
{
	endpoint = "/groups/{groupId}/recovery_copy/snapshots";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="application/json";
	return get<ConsistencyGroupCopySnapshots>();
}


public ConsistencyGroupCopySettingsSet getAllGroupCopies_Method(long groupId)
{
	endpoint = "/groups/{groupId}/copies/settings";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="application/json";
	return get<ConsistencyGroupCopySettingsSet>();
}


public void setGroupPolicy_Method(long groupId,consistencyGroupPolicy consistencyGroupPolicy_object)
{
	endpoint = "/groups/{groupId}/policy";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put(consistencyGroupPolicy_object);
}


public void setLinkPolicy_Method(long groupId,setConsistencyGroupLinkPolicyParams setConsistencyGroupLinkPolicyParams_object)
{
	endpoint = "/groups/{groupId}/set_link_policy";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put(setConsistencyGroupLinkPolicyParams_object);
}


public void removeVmsFromCG_Method(long groupId,vmUIDSet vmUIDSet_object)
{
	endpoint = "/groups/{groupId}/virtual_machines";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	delete(vmUIDSet_object);
}


public ConsistencyGroupUID replicateVms_Method(replicateVmsParam replicateVmsParam_object)
{
	endpoint = "/groups/virtual_machines/replicate";
	mediatype="application/json";
	return post<ConsistencyGroupUID>(replicateVmsParam_object);
}


public void changeGroupPowerUpSequence_Method(long groupId,restLong restLong_object)
{
	endpoint = "/groups/{groupId}/virtual_machines/group_powerup_sequence";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put(restLong_object);
}


public void editVMsReplicationParams_Method(long groupId,virtualDisksReplicationPolicyParamSet virtualDisksReplicationPolicyParamSet_object)
{
	endpoint = "/groups/{groupId}/virtual_machines/replication_params";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put(virtualDisksReplicationPolicyParamSet_object);
}


public void editVMsHardwareReplicationParams_Method(long groupId,virtualHardwareReplicationPolicyParamSet virtualHardwareReplicationPolicyParamSet_object)
{
	endpoint = "/groups/{groupId}/virtual_machines/hardware_replication_params";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put(virtualHardwareReplicationPolicyParamSet_object);
}


public ConsistencyGroupReportContextInfoSet getConsistencyGroupReportsContexts_Method(long groupId)
{
	endpoint = "/groups/{groupId}/reports";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="application/json";
	return get<ConsistencyGroupReportContextInfoSet>();
}


public void deleteConsistencyGroupReports_Method(long groupId,long reportId)
{
	endpoint = "/groups/{groupId}/reports/{reportId}/delete";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{reportId}",reportId.ToString());
	mediatype="*/*";
	delete();
}


public ConsistencyGroupInformationSet getAllConsistencyGroupsInformation_Method()
{
	endpoint = "/groups/information";
	mediatype="application/json";
	return get<ConsistencyGroupInformationSet>();
}


public ConsistencyGroupInformation getConsistencyGroupInformation_Method(long groupId)
{
	endpoint = "/groups/{groupId}/information";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="application/json";
	return get<ConsistencyGroupInformation>();
}


public void changeVmsPowerUpSequence_Method(long groupId,vmPowerUpSequenceParamSet vmPowerUpSequenceParamSet_object)
{
	endpoint = "/groups/{groupId}/virtual_machines/powerup_sequence";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put(vmPowerUpSequenceParamSet_object);
}


public void setVMsReplicationSetsPolicies_Method(long groupId,fullVmReplicationSetPoliciesSet fullVmReplicationSetPoliciesSet_object)
{
	endpoint = "/groups/{groupId}/vm_replication_sets_policy";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put(fullVmReplicationSetPoliciesSet_object);
}


public ConsistencyGroupVolumesStateSet getAllGroupsVolumesState_Method()
{
	endpoint = "/groups/volumes/state";
	mediatype="application/json";
	return get<ConsistencyGroupVolumesStateSet>();
}


public ConsistencyGroupVolumesState getGroupVolumesState_Method(long groupId)
{
	endpoint = "/groups/{groupId}/volumes/state";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="application/json";
	return get<ConsistencyGroupVolumesState>();
}


public ConsistencyGroupUID addDefaultGroup_Method(addConsistencyGroupWithDefaultPolicyParams addConsistencyGroupWithDefaultPolicyParams_object)
{
	endpoint = "/groups/default_group";
	mediatype="application/json";
	return post<ConsistencyGroupUID>(addConsistencyGroupWithDefaultPolicyParams_object);
}


public void addGroupAndCopies_Method(fullConsistencyGroupPolicy fullConsistencyGroupPolicy_object)
{
	endpoint = "/groups/group_and_copies";
	mediatype="*/*";
	post(fullConsistencyGroupPolicy_object);
}


public ReplicationSetSettings getGroupReplicationSet_Method(long groupId,long replicationSetId)
{
	endpoint = "/groups/{groupId}/replication_sets/{replicationSetId}/settings";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{replicationSetId}",replicationSetId.ToString());
	mediatype="application/json";
	return get<ReplicationSetSettings>();
}


public void addLink_Method(long groupId,addConsistencyGroupLinkParams addConsistencyGroupLinkParams_object)
{
	endpoint = "/groups/{groupId}/links";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	post(addConsistencyGroupLinkParams_object);
}


public void addcopy_Method(long groupId,consistencyGroupCopySettingsParam consistencyGroupCopySettingsParam_object)
{
	endpoint = "/groups/{groupId}/copies";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	post(consistencyGroupCopySettingsParam_object);
}


public void disableGroup_Method(long groupId)
{
	endpoint = "/groups/{groupId}/disable";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put();
}


public void enableGroup_Method(long groupId,bool? startTransfer=null)
{
	endpoint = "/groups/{groupId}/enable";
	endpoint.Replace("{groupId}",groupId.ToString());
if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString()));}

	mediatype="*/*";
	put();
}


public void removePassiveLink_Method(long groupId,removePassiveConsistencyGroupLinkParams removePassiveConsistencyGroupLinkParams_object)
{
	endpoint = "/groups/{groupId}/remove_passive_links";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put(removePassiveConsistencyGroupLinkParams_object);
}


public void resumeGroup_Method(long groupId)
{
	endpoint = "/groups/{groupId}/resume";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put();
}


public void setFullGroupPolicy_Method(long groupId,fullConsistencyGroupPolicy fullConsistencyGroupPolicy_object)
{
	endpoint = "/groups/{groupId}/full_policy";
	endpoint.Replace("{groupId}",groupId.ToString());
	mediatype="*/*";
	put(fullConsistencyGroupPolicy_object);
}


public void disableCopy_Method(long groupId,long clusterId,int copyId)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/disable";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put();
}


public void enableCopy_Method(long groupId,long clusterId,int copyId,bool? startTransfer=null)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString()));}

	mediatype="*/*";
	put();
}


public void consolidateSnapshot_Method(long groupId,long clusterId,int copyId,consolidateSnapshotsParams consolidateSnapshotsParams_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/consolidate";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put(consolidateSnapshotsParams_object);
}


public void enableImageAccessWithGeneralParams_Method(long groupId,long clusterId,int copyId,restImageAccessGeneralParameters restImageAccessGeneralParameters_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable_image_access_with_general_params";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put(restImageAccessGeneralParameters_object);
}


public void markAsClean_Method(long groupId,long clusterId,int copyId)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/volumes/mark_as_clean";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put();
}


public void markAsDirty_Method(long groupId,long clusterId,int copyId,markGroupCopyVolumesAsDirtyParams markGroupCopyVolumesAsDirtyParams_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/volumes/mark_as_dirty";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put(markGroupCopyVolumesAsDirtyParams_object);
}


public void pauseTransfer_Method(long groupId,long clusterId,int copyId)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/pause_transfer";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put();
}


public RestString getGroupCopyName_Method(long groupId,long clusterId,int copyId)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/name";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="application/json";
	return get<RestString>();
}


public void startCopyTransfer_Method(long groupId,long clusterId,int copyId)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/start_transfer";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put();
}


public void undoWrites_Method(long groupId,long clusterId,int copyId)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/undo_writes";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put();
}


public void setBookmarksSettings_Method(long groupId,long clusterId,int copyId,long snapshotId,snapshotBookmarkingSettingsInfo snapshotBookmarkingSettingsInfo_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/{snapshotId}/bookmark_settings";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	endpoint.Replace("{snapshotId}",snapshotId.ToString());
	mediatype="*/*";
	put(snapshotBookmarkingSettingsInfo_object);
}


public void setConsistencyGroupTopology_Method(long groupId,long clusterId,int copyId,consistencyGroupTopologyParams consistencyGroupTopologyParams_object,bool? start_transfer=null)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/topology";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
if (start_transfer != null) { url_params.Add(new KeyValuePair<string, string>("start_transfer", start_transfer.ToString()));}

	mediatype="*/*";
	put(consistencyGroupTopologyParams_object);
}


public void failover_Method(long groupId,long clusterId,int copyId,bool? startTransfer=null)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/failover";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString()));}

	mediatype="*/*";
	put();
}


public void abortSnapshotsConsolidation_Method(long groupId,long clusterId,int copyId)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/abort_consolidation";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put();
}


public void disableImageAccess_Method(long groupId,long clusterId,int copyId,bool? startTransfer=null)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/disable_image_access";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString()));}

	mediatype="*/*";
	put();
}


public void enableImageAccessWithParams_Method(long groupId,long clusterId,int copyId,enableImageAccessWithParams enableImageAccessWithParams_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable_image_access_with_params";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put(enableImageAccessWithParams_object);
}


public void enableImageAccess_Method(long groupId,long clusterId,int copyId,enableImageAccessParams enableImageAccessParams_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable_image_access";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put(enableImageAccessParams_object);
}


public ConsistencyGroupCopySnapshots getGroupCopySnapshots_Method(long groupId,long clusterId,int copyId,string start_time=null,string end_time=null,string name=null)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
if (start_time != null) { url_params.Add(new KeyValuePair<string, string>("start_time", start_time.ToString()));}
if (end_time != null) { url_params.Add(new KeyValuePair<string, string>("end_time", end_time.ToString()));}
if (name != null) { url_params.Add(new KeyValuePair<string, string>("name", name.ToString()));}

	mediatype="application/json";
	return get<ConsistencyGroupCopySnapshots>();
}


public void recoverProduction_Method(long groupId,long clusterId,int copyId,bool? startTransfer=null)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/recover_production";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString()));}

	mediatype="*/*";
	put();
}


public void enableDirectAccess_Method(long groupId,long clusterId,int copyId)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable_direct_access";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put();
}


public void forceLongInitialization_Method(long groupId,long clusterId,int copyId)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/force_long_initialization";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put();
}


public TransactionID verifyGroupCopySnapshots_Method(long groupId,long clusterId,int copyId,verifyGroupCopySnapshotsParam verifyGroupCopySnapshotsParam_object,long? timeout_in_seconds=null)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/verify";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
if (timeout_in_seconds != null) { url_params.Add(new KeyValuePair<string, string>("timeout_in_seconds", timeout_in_seconds.ToString()));}

	mediatype="application/json";
	return post<TransactionID>(verifyGroupCopySnapshotsParam_object);
}


public void addJournalVolume_Method(long groupId,long clusterId,int copyId,deviceUID deviceUID_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/journal_volumes";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	post(deviceUID_object);
}


public void removeJournalVolume_Method(long groupId,long clusterId,int copyId,string deviceId)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/journal_volumes/{deviceId}";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	endpoint.Replace("{deviceId}",deviceId.ToString());
	mediatype="*/*";
	delete();
}


public void addUserVolume_Method(long groupId,long clusterId,int copyId,addUserVolumeParams addUserVolumeParams_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/user_volumes";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	post(addUserVolumeParams_object);
}


public void removeUserVolume_Method(long groupId,long clusterId,int copyId,string deviceId,replicationSetUID replicationSetUID_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/user_volumes/{deviceId}";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	endpoint.Replace("{deviceId}",deviceId.ToString());
	mediatype="*/*";
	delete(replicationSetUID_object);
}


public void addAndAttachUserVolume_Method(long groupId,long clusterId,int copyId,long replicationSetId,deviceUID deviceUID_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/replication_sets/{replicationSetId}/volumes";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	endpoint.Replace("{replicationSetId}",replicationSetId.ToString());
	mediatype="*/*";
	post(deviceUID_object);
}


public void setBookmarkConsistencyType_Method(long groupId,long clusterId,int copyId,long snapshotId,restSnapshotConsistencyType restSnapshotConsistencyType_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/{snapshotId}/consistency_type";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	endpoint.Replace("{snapshotId}",snapshotId.ToString());
	mediatype="*/*";
	put(restSnapshotConsistencyType_object);
}


public void setProductionCopy_Method(long groupId,long clusterId,int copyId,bool? startTransfer=null)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/set_production_copy";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
if (startTransfer != null) { url_params.Add(new KeyValuePair<string, string>("startTransfer", startTransfer.ToString()));}

	mediatype="*/*";
	put();
}


public void setBookmarkConsolidationPolicy_Method(long groupId,long clusterId,int copyId,long snapshotId,restBookmarkConsolidationPolicy restBookmarkConsolidationPolicy_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/{snapshotId}/consolidation_policy";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	endpoint.Replace("{snapshotId}",snapshotId.ToString());
	mediatype="*/*";
	put(restBookmarkConsolidationPolicy_object);
}


public void setSignaturesUsageDuringInitialization_Method(long groupId,long clusterId,int copyId,restBoolean restBoolean_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/signature_usage_during_initialization";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put(restBoolean_object);
}


public void moveToImage_Method(long groupId,long clusterId,int copyId,moveToImageParams moveToImageParams_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/move_to_image";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put(moveToImageParams_object);
}


public void rollToImage_Method(long groupId,long clusterId,int copyId)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/roll_to_image";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put();
}


public void setCopyPolicy_Method(long groupId,long clusterId,int copyId,consistencyGroupCopyPolicy consistencyGroupCopyPolicy_object,string name=null)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/policy";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
if (name != null) { url_params.Add(new KeyValuePair<string, string>("name", name.ToString()));}

	mediatype="*/*";
	put(consistencyGroupCopyPolicy_object);
}


public void enableLatestImageAccess_Method(long groupId,long clusterId,int copyId,enableLatestImageAccessParams enableLatestImageAccessParams_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/image_access/latest/enable";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put(enableLatestImageAccessParams_object);
}


public void dismissVmStartUpPrompts_Method(long groupId,long clusterId,int copyId,vmStartUpActionUIDSet vmStartUpActionUIDSet_object)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/vm_startup_prompts/dismiss";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	put(vmStartUpActionUIDSet_object);
}


public ConsistencyGroupCopySettings getGroupCopySettings_Method(long groupId,long clusterId,int copyId)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/settings";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="application/json";
	return get<ConsistencyGroupCopySettings>();
}


public void removeCopy_Method(long groupId,long clusterId,int copyId)
{
	endpoint = "/groups/{groupId}/clusters/{clusterId}/copies/{copyId}";
	endpoint.Replace("{groupId}",groupId.ToString());
	endpoint.Replace("{clusterId}",clusterId.ToString());
	endpoint.Replace("{copyId}",copyId.ToString());
	mediatype="*/*";
	delete();
}


public ConsistencyGroupUIDSet getAllGroupsUIDs_Method()
{
	endpoint = "/groups";
	mediatype="application/json";
	return get<ConsistencyGroupUIDSet>();
}



}
}
