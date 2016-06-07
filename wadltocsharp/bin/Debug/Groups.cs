namespace MRMPService.RP4VM
{

public void renameGroup_Method(long groupId,restString_object restString)
{
	endpoint = '/groups/{groupId}/name';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put(restString_object);
}


public void removeGroup_Method(long groupId)
{
	endpoint = '/groups/{groupId}';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return delete();
}


public void createBookmark_Method(createBookmarkParams_object createBookmarkParams)
{
	endpoint = '/groups/bookmarks';
	mediatype='*/*'
	return post(createBookmarkParams_object);
}


public void markGroupVolumesAsClean_Method(long groupId)
{
	endpoint = '/groups/{groupId}/volumes/mark_as_clean';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put();
}


public consistencyGroupSettingsSet getAllGroupsSettings_Method()
{
	endpoint = '/groups/settings';
	mediatype='application/json'
	get<consistencyGroupSettingsSet>();
}


public consistencyGroupSettingsSet getSpecificGroupsSettings_Method(consistencyGroupUIDSet_object consistencyGroupUIDSet)
{
	endpoint = '/groups/specific/settings';
	mediatype='application/json'
	post<consistencyGroupSettingsSet>(consistencyGroupUIDSet_object);
}


public consistencyGroupStateSet getAllGroupsState_Method()
{
	endpoint = '/groups/state';
	mediatype='application/json'
	get<consistencyGroupStateSet>();
}


public consistencyGroupStateSet getSpecificGroupsState_Method(consistencyGroupUIDSet_object consistencyGroupUIDSet)
{
	endpoint = '/groups/specific/state';
	mediatype='application/json'
	post<consistencyGroupStateSet>(consistencyGroupUIDSet_object);
}


public consistencyGroupState getGroupState_Method(long groupId)
{
	endpoint = '/groups/{groupId}/state';
	url.replace('{groupId}', groupId);
	mediatype='application/json'
	get<consistencyGroupState>();
}


public consistencyGroupStatisticsSet getAllGroupsStatistics_Method()
{
	endpoint = '/groups/statistics';
	mediatype='application/json'
	get<consistencyGroupStatisticsSet>();
}


public consistencyGroupStatisticsSet getSpecificGroupsStatistics_Method(consistencyGroupUIDSet_object consistencyGroupUIDSet)
{
	endpoint = '/groups/specific/statistics';
	mediatype='application/json'
	post<consistencyGroupStatisticsSet>(consistencyGroupUIDSet_object);
}


public consistencyGroupStatistics getGroupStatistics_Method(long groupId)
{
	endpoint = '/groups/{groupId}/statistics';
	url.replace('{groupId}', groupId);
	mediatype='application/json'
	get<consistencyGroupStatistics>();
}


public consistencyGroupSnapshots getGroupSnapshots_Method(long groupId,long start_time,long end_time,string name)
{
	endpoint = '/groups/{groupId}/snapshots';
	url.replace('{groupId}', groupId);
	url.replace('{start_time}', start_time);
	url.replace('{end_time}', end_time);
	url.replace('{name}', name);
	mediatype='application/json'
	get<consistencyGroupSnapshots>();
}


public void pauseGroupTransfer_Method(long groupId)
{
	endpoint = '/groups/{groupId}/pause_transfer';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put();
}


public void resumeProduction_Method(long groupId,boolean startTransfer)
{
	endpoint = '/groups/{groupId}/resume_production';
	url.replace('{groupId}', groupId);
	url.replace('{startTransfer}', startTransfer);
	mediatype='*/*'
	return put();
}


public void markGroupVolumesAsDirty_Method(long groupId,markGroupVolumesAsDirtyParams_object markGroupVolumesAsDirtyParams)
{
	endpoint = '/groups/{groupId}/volumes/mark_as_dirty';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put(markGroupVolumesAsDirtyParams_object);
}


public void startGroupTransfer_Method(long groupId)
{
	endpoint = '/groups/{groupId}/start_transfer';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put();
}


public transactionID verifyConsistencyGroupState_Method(long groupId,long timeout_in_seconds,verifyConsistencyGroupStateParam_object verifyConsistencyGroupStateParam)
{
	endpoint = '/groups/{groupId}/state/verify';
	url.replace('{groupId}', groupId);
	url.replace('{timeout_in_seconds}', timeout_in_seconds);
	mediatype='application/json'
	post<transactionID>(verifyConsistencyGroupStateParam_object);
}


public replicationSetSettingsSet getAllGroupReplicationSets_Method(long groupId,string replicationSetName)
{
	endpoint = '/groups/{groupId}/replication_sets';
	url.replace('{groupId}', groupId);
	url.replace('{replicationSetName}', replicationSetName);
	mediatype='application/json'
	get<replicationSetSettingsSet>();
}


public void removeReplicationSet_Method(long groupId,long replicationSetId)
{
	endpoint = '/groups/{groupId}/replication_sets/{replicationSetId}';
	url.replace('{groupId}', groupId);
	url.replace('{replicationSetId}', replicationSetId);
	mediatype='*/*'
	return delete();
}


public restString getGroupReplicationSetName_Method(long groupId,long replicationSetId)
{
	endpoint = '/groups/{groupId}/replication_sets/{replicationSetId}/name';
	url.replace('{groupId}', groupId);
	url.replace('{replicationSetId}', replicationSetId);
	mediatype='application/json'
	get<restString>();
}


public consistencyGroupSettings getGroupSettings_Method(long groupId)
{
	endpoint = '/groups/{groupId}/settings';
	url.replace('{groupId}', groupId);
	mediatype='application/json'
	get<consistencyGroupSettings>();
}


public void suspendConsistencyGroup_Method(long groupId)
{
	endpoint = '/groups/{groupId}/suspend';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put();
}


public void resizeReplicationSet_Method(long groupId,long replicationSetId,resizeReplicationSetParam_object resizeReplicationSetParam)
{
	endpoint = '/groups/{groupId}/replication_sets/{replicationSetId}/size';
	url.replace('{groupId}', groupId);
	url.replace('{replicationSetId}', replicationSetId);
	mediatype='*/*'
	return put(resizeReplicationSetParam_object);
}


public recoveryActivitiesSet getAllRecoveryActivities_Method()
{
	endpoint = '/groups/recovery_activities';
	mediatype='application/json'
	get<recoveryActivitiesSet>();
}


public consistencyGroupsTransferStateSet getGroupsTransferState_Method()
{
	endpoint = '/groups/transfer_state';
	mediatype='application/json'
	get<consistencyGroupsTransferStateSet>();
}


public void setReplicationSetsSettings_Method(long groupId,replicationSetSettingsChangesParamSet_object replicationSetSettingsChangesParamSet)
{
	endpoint = '/groups/{groupId}/replication_sets/settings';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put(replicationSetSettingsChangesParamSet_object);
}


public void addDistributedConsistencyGroupWithDefaultPolicy_Method(distributedConsistencyGroupWithDefaultPolicyParams_object distributedConsistencyGroupWithDefaultPolicyParams)
{
	endpoint = '/groups/distributed';
	mediatype='*/*'
	return post(distributedConsistencyGroupWithDefaultPolicyParams_object);
}


public consistencyGroupVolumesStateSet getSpecificGroupsVolumeStates_Method(consistencyGroupUIDSet_object consistencyGroupUIDSet)
{
	endpoint = '/groups/specific/volumes/state';
	mediatype='application/json'
	post<consistencyGroupVolumesStateSet>(consistencyGroupUIDSet_object);
}


public consistencyGroupCopySnapshots getRecoveryGroupCopySnapshots_Method(long groupId)
{
	endpoint = '/groups/{groupId}/recovery_copy/snapshots';
	url.replace('{groupId}', groupId);
	mediatype='application/json'
	get<consistencyGroupCopySnapshots>();
}


public consistencyGroupCopySettingsSet getAllGroupCopies_Method(long groupId)
{
	endpoint = '/groups/{groupId}/copies/settings';
	url.replace('{groupId}', groupId);
	mediatype='application/json'
	get<consistencyGroupCopySettingsSet>();
}


public void setGroupPolicy_Method(long groupId,consistencyGroupPolicy_object consistencyGroupPolicy)
{
	endpoint = '/groups/{groupId}/policy';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put(consistencyGroupPolicy_object);
}


public void setLinkPolicy_Method(long groupId,setConsistencyGroupLinkPolicyParams_object setConsistencyGroupLinkPolicyParams)
{
	endpoint = '/groups/{groupId}/set_link_policy';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put(setConsistencyGroupLinkPolicyParams_object);
}


public void removeVmsFromCG_Method(long groupId,vmUIDSet_object vmUIDSet)
{
	endpoint = '/groups/{groupId}/virtual_machines';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return delete(vmUIDSet_object);
}


public consistencyGroupUID replicateVms_Method(replicateVmsParam_object replicateVmsParam)
{
	endpoint = '/groups/virtual_machines/replicate';
	mediatype='application/json'
	post<consistencyGroupUID>(replicateVmsParam_object);
}


public void changeGroupPowerUpSequence_Method(long groupId,restLong_object restLong)
{
	endpoint = '/groups/{groupId}/virtual_machines/group_powerup_sequence';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put(restLong_object);
}


public void editVMsReplicationParams_Method(long groupId,virtualDisksReplicationPolicyParamSet_object virtualDisksReplicationPolicyParamSet)
{
	endpoint = '/groups/{groupId}/virtual_machines/replication_params';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put(virtualDisksReplicationPolicyParamSet_object);
}


public void editVMsHardwareReplicationParams_Method(long groupId,virtualHardwareReplicationPolicyParamSet_object virtualHardwareReplicationPolicyParamSet)
{
	endpoint = '/groups/{groupId}/virtual_machines/hardware_replication_params';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put(virtualHardwareReplicationPolicyParamSet_object);
}


public consistencyGroupReportContextInfoSet getConsistencyGroupReportsContexts_Method(long groupId)
{
	endpoint = '/groups/{groupId}/reports';
	url.replace('{groupId}', groupId);
	mediatype='application/json'
	get<consistencyGroupReportContextInfoSet>();
}


public void deleteConsistencyGroupReports_Method(long groupId,long reportId)
{
	endpoint = '/groups/{groupId}/reports/{reportId}/delete';
	url.replace('{groupId}', groupId);
	url.replace('{reportId}', reportId);
	mediatype='*/*'
	return delete();
}


public consistencyGroupInformationSet getAllConsistencyGroupsInformation_Method()
{
	endpoint = '/groups/information';
	mediatype='application/json'
	get<consistencyGroupInformationSet>();
}


public consistencyGroupInformation getConsistencyGroupInformation_Method(long groupId)
{
	endpoint = '/groups/{groupId}/information';
	url.replace('{groupId}', groupId);
	mediatype='application/json'
	get<consistencyGroupInformation>();
}


public void changeVmsPowerUpSequence_Method(long groupId,vmPowerUpSequenceParamSet_object vmPowerUpSequenceParamSet)
{
	endpoint = '/groups/{groupId}/virtual_machines/powerup_sequence';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put(vmPowerUpSequenceParamSet_object);
}


public void setVMsReplicationSetsPolicies_Method(long groupId,fullVmReplicationSetPoliciesSet_object fullVmReplicationSetPoliciesSet)
{
	endpoint = '/groups/{groupId}/vm_replication_sets_policy';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put(fullVmReplicationSetPoliciesSet_object);
}


public consistencyGroupVolumesStateSet getAllGroupsVolumesState_Method()
{
	endpoint = '/groups/volumes/state';
	mediatype='application/json'
	get<consistencyGroupVolumesStateSet>();
}


public consistencyGroupVolumesState getGroupVolumesState_Method(long groupId)
{
	endpoint = '/groups/{groupId}/volumes/state';
	url.replace('{groupId}', groupId);
	mediatype='application/json'
	get<consistencyGroupVolumesState>();
}


public consistencyGroupUID addDefaultGroup_Method(addConsistencyGroupWithDefaultPolicyParams_object addConsistencyGroupWithDefaultPolicyParams)
{
	endpoint = '/groups/default_group';
	mediatype='application/json'
	post<consistencyGroupUID>(addConsistencyGroupWithDefaultPolicyParams_object);
}


public void addGroupAndCopies_Method(fullConsistencyGroupPolicy_object fullConsistencyGroupPolicy)
{
	endpoint = '/groups/group_and_copies';
	mediatype='*/*'
	return post(fullConsistencyGroupPolicy_object);
}


public replicationSetSettings getGroupReplicationSet_Method(long groupId,long replicationSetId)
{
	endpoint = '/groups/{groupId}/replication_sets/{replicationSetId}/settings';
	url.replace('{groupId}', groupId);
	url.replace('{replicationSetId}', replicationSetId);
	mediatype='application/json'
	get<replicationSetSettings>();
}


public void addLink_Method(long groupId,addConsistencyGroupLinkParams_object addConsistencyGroupLinkParams)
{
	endpoint = '/groups/{groupId}/links';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return post(addConsistencyGroupLinkParams_object);
}


public void addcopy_Method(long groupId,consistencyGroupCopySettingsParam_object consistencyGroupCopySettingsParam)
{
	endpoint = '/groups/{groupId}/copies';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return post(consistencyGroupCopySettingsParam_object);
}


public void disableGroup_Method(long groupId)
{
	endpoint = '/groups/{groupId}/disable';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put();
}


public void enableGroup_Method(long groupId,boolean startTransfer)
{
	endpoint = '/groups/{groupId}/enable';
	url.replace('{groupId}', groupId);
	url.replace('{startTransfer}', startTransfer);
	mediatype='*/*'
	return put();
}


public void removePassiveLink_Method(long groupId,removePassiveConsistencyGroupLinkParams_object removePassiveConsistencyGroupLinkParams)
{
	endpoint = '/groups/{groupId}/remove_passive_links';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put(removePassiveConsistencyGroupLinkParams_object);
}


public void resumeGroup_Method(long groupId)
{
	endpoint = '/groups/{groupId}/resume';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put();
}


public void setFullGroupPolicy_Method(long groupId,fullConsistencyGroupPolicy_object fullConsistencyGroupPolicy)
{
	endpoint = '/groups/{groupId}/full_policy';
	url.replace('{groupId}', groupId);
	mediatype='*/*'
	return put(fullConsistencyGroupPolicy_object);
}


public void disableCopy_Method(long groupId,long clusterId,int copyId)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/disable';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return put();
}


public void enableCopy_Method(long groupId,long clusterId,int copyId,boolean startTransfer)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	url.replace('{startTransfer}', startTransfer);
	mediatype='*/*'
	return put();
}


public void consolidateSnapshot_Method(long groupId,long clusterId,int copyId,consolidateSnapshotsParams_object consolidateSnapshotsParams)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/consolidate';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return put(consolidateSnapshotsParams_object);
}


public void enableImageAccessWithGeneralParams_Method(long groupId,long clusterId,int copyId,restImageAccessGeneralParameters_object restImageAccessGeneralParameters)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable_image_access_with_general_params';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return put(restImageAccessGeneralParameters_object);
}


public void markAsClean_Method(long groupId,long clusterId,int copyId)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/volumes/mark_as_clean';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return put();
}


public void markAsDirty_Method(long groupId,long clusterId,int copyId,markGroupCopyVolumesAsDirtyParams_object markGroupCopyVolumesAsDirtyParams)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/volumes/mark_as_dirty';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return put(markGroupCopyVolumesAsDirtyParams_object);
}


public void pauseTransfer_Method(long groupId,long clusterId,int copyId)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/pause_transfer';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return put();
}


public restString getGroupCopyName_Method(long groupId,long clusterId,int copyId)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/name';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='application/json'
	get<restString>();
}


public void startCopyTransfer_Method(long groupId,long clusterId,int copyId)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/start_transfer';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return put();
}


public void undoWrites_Method(long groupId,long clusterId,int copyId)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/undo_writes';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return put();
}


public void setBookmarksSettings_Method(long groupId,long clusterId,int copyId,long snapshotId,snapshotBookmarkingSettingsInfo_object snapshotBookmarkingSettingsInfo)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/{snapshotId}/bookmark_settings';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	url.replace('{snapshotId}', snapshotId);
	mediatype='*/*'
	return put(snapshotBookmarkingSettingsInfo_object);
}


public void setConsistencyGroupTopology_Method(long groupId,long clusterId,int copyId,boolean start_transfer,consistencyGroupTopologyParams_object consistencyGroupTopologyParams)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/topology';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	url.replace('{start_transfer}', start_transfer);
	mediatype='*/*'
	return put(consistencyGroupTopologyParams_object);
}


public void failover_Method(long groupId,long clusterId,int copyId,boolean startTransfer)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/failover';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	url.replace('{startTransfer}', startTransfer);
	mediatype='*/*'
	return put();
}


public void abortSnapshotsConsolidation_Method(long groupId,long clusterId,int copyId)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/abort_consolidation';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return put();
}


public void disableImageAccess_Method(long groupId,long clusterId,int copyId,boolean startTransfer)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/disable_image_access';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	url.replace('{startTransfer}', startTransfer);
	mediatype='*/*'
	return put();
}


public void enableImageAccessWithParams_Method(long groupId,long clusterId,int copyId,enableImageAccessWithParams_object enableImageAccessWithParams)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable_image_access_with_params';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return put(enableImageAccessWithParams_object);
}


public void enableImageAccess_Method(long groupId,long clusterId,int copyId,enableImageAccessParams_object enableImageAccessParams)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable_image_access';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return put(enableImageAccessParams_object);
}


public consistencyGroupCopySnapshots getGroupCopySnapshots_Method(long groupId,long clusterId,int copyId,string start_time,string end_time,string name)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	url.replace('{start_time}', start_time);
	url.replace('{end_time}', end_time);
	url.replace('{name}', name);
	mediatype='application/json'
	get<consistencyGroupCopySnapshots>();
}


public void recoverProduction_Method(long groupId,long clusterId,int copyId,boolean startTransfer)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/recover_production';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	url.replace('{startTransfer}', startTransfer);
	mediatype='*/*'
	return put();
}


public void enableDirectAccess_Method(long groupId,long clusterId,int copyId)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/enable_direct_access';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return put();
}


public void forceLongInitialization_Method(long groupId,long clusterId,int copyId)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/force_long_initialization';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return put();
}


public transactionID verifyGroupCopySnapshots_Method(long groupId,long clusterId,int copyId,long timeout_in_seconds,verifyGroupCopySnapshotsParam_object verifyGroupCopySnapshotsParam)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/snapshots/verify';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	url.replace('{timeout_in_seconds}', timeout_in_seconds);
	mediatype='application/json'
	post<transactionID>(verifyGroupCopySnapshotsParam_object);
}


public void addJournalVolume_Method(long groupId,long clusterId,int copyId,deviceUID_object deviceUID)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/journal_volumes';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return post(deviceUID_object);
}


public void removeJournalVolume_Method(long groupId,long clusterId,int copyId,string deviceId)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/journal_volumes/{deviceId}';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	url.replace('{deviceId}', deviceId);
	mediatype='*/*'
	return delete();
}


public void addUserVolume_Method(long groupId,long clusterId,int copyId,addUserVolumeParams_object addUserVolumeParams)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/user_volumes';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	mediatype='*/*'
	return post(addUserVolumeParams_object);
}


public void removeUserVolume_Method(long groupId,long clusterId,int copyId,string deviceId,replicationSetUID_object replicationSetUID)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/user_volumes/{deviceId}';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	url.replace('{deviceId}', deviceId);
	mediatype='*/*'
	return delete(replicationSetUID_object);
}


public void addAndAttachUserVolume_Method(long groupId,long clusterId,int copyId,long replicationSetId,deviceUID_object deviceUID)
{
	endpoint = '/groups/{groupId}/clusters/{clusterId}/copies/{copyId}/replication_sets/{replicationSetId}/volumes';
	url.replace('{groupId}', groupId);
	url.replace('{clusterId}', clusterId);
	url.replace('{copyId}', copyId);
	url.replace('{replicationSetId}', replicationSetId);
	mediatype='*/*'
	return post(deviceUID_object);