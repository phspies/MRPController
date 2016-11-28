using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Types.API;
using MRMPService.RP4VMTypes;
using MRMPService.RP4VMAPI;
using System.Collections.Generic;
using System;

namespace MRMPService.Tasks.RP4VM
{
    class RP4VM
    {
        public static void CreateConsistencyGroup(string _task_id, List<MRPWorkloadType> _source_workloads, MRPProtectiongroupType _protectiongroup, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {
            MRPPlatformType _source_platform = _protectiongroup.recoverypolicy.sourceplatform;
            MRPPlatformType _target_platform = _protectiongroup.recoverypolicy.targetplatform;

            MRPPlatformdatastoreType _source_journal_ds = _protectiongroup.recoverypolicy.source_journal_datastore;
            MRPPlatformdatastoreType _target_journal_ds = _protectiongroup.recoverypolicy.target_journal_datastore;
            MRPPlatformdatastoreType _target_ds = _protectiongroup.recoverypolicy.target_datastore;
            MRPPlatformclusterType _target_cluster = _protectiongroup.recoverypolicy.target_cluster;

            using (MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                RP4VM_ApiClient _rp4vm = new RP4VM_ApiClient(_source_platform.rp4vm_url, _source_platform.credential.username, _source_platform.credential.encrypted_password);

                try
                {
                    ReplicateVmsParam _replicate_object = new ReplicateVmsParam()
                    {
                        cgName = String.Format("{0}_consistency_group", _protectiongroup.group.Replace(" ", "_").ToLower()),
                        productionCopy = new GlobalCopyUID()
                        {
                            clusterUID = new ClusterUID()
                            {
                                id = Int64.Parse(_source_platform.moid),
                            },
                            copyUID = 0
                        },

                    };
                    List<VMReplicationSetParam> _vm_replication_sets = new List<VMReplicationSetParam>();
                    foreach (var _source_vm in _source_workloads)
                    {
                        List<ReplicatedVMParams> _source_target_vms = new List<ReplicatedVMParams>();

                        _source_target_vms.Add(new ReplicatedVMParams()
                        {
                            copyUID = new GlobalCopyUID()
                            {
                                clusterUID = new ClusterUID()
                                {
                                    id = Int64.Parse(_source_platform.moid)
                                }
                            },
                            vmParam = new SourceVmParam()
                            {
                                clusterUID = new ClusterUID()
                                {
                                    id = Int64.Parse(_source_platform.moid)
                                },
                                vmUID = new VmUID()
                                {
                                    uuid = _source_vm.vcenter_uuid,
                                    virtualCenterUID = new VirtualCenterUID() { uuid = _source_platform.parent_platform.vcenter_uuid }
                                }
                            },

                        });
                        _source_target_vms.Add(new ReplicatedVMParams()
                        {
                            copyUID = new GlobalCopyUID()
                            {
                                clusterUID = new ClusterUID()
                                {
                                    id = Int64.Parse(_target_platform.moid)
                                },
                                copyUID = 1
                            },
                            vmParam = new CreateVMParam()
                            {
                                targetVirtualCenterUID = new VirtualCenterUID() { uuid = _target_platform.parent_platform.vcenter_uuid },
                                targetDatastoreUID = new DatastoreUID() { uuid = _target_ds.moid },
                                targetResourcePlacementParam = new CreateTargetVMAutomaticResourcePlacementParam()
                                {
                                    targetResourcePoolUID = new VirtualResourcePoolUUID()
                                    {
                                        uuid = _target_cluster.resourcepool_moid
                                    }
                                },
                            },

                        });

                        VirtualHardwareReplicationPolicy _virtualHardwareReplicationPolicy = new VirtualHardwareReplicationPolicy()
                        {
                            hwChangesPolicy = hardwareChangesPolicy.REPLICATE_HW_CHANGES,
                            provisionPolicy = diskProvisionPolicy.SAME_AS_SOURCE
                        };

                        VirtualDisksReplicationPolicy _virtualDisksReplicationPolicy = new VirtualDisksReplicationPolicy()
                        {
                            autoReplicateNewVirtualDisks = true
                        };
                        _vm_replication_sets.Add(new VMReplicationSetParam()
                        {
                            replicationSetVms = _source_target_vms,
                            virtualDisksReplicationPolicy = _virtualDisksReplicationPolicy,
                            virtualHardwareReplicationPolicy = _virtualHardwareReplicationPolicy
                        });
                    }
                    _replicate_object.vmReplicationSets = _vm_replication_sets;

                    _replicate_object.links = new List<FullConsistencyGroupLinkPolicy>() {
                        new FullConsistencyGroupLinkPolicy() {
                            linkUID = new ConsistencyGroupLinkUID() {
                                firstCopy =new GlobalCopyUID() {
                                    clusterUID = new ClusterUID() {
                                        id = Int64.Parse(_source_platform.moid)
                                    }
                                },
                                 secondCopy = new GlobalCopyUID()
                                 {
                                     clusterUID = new ClusterUID()
                                     {
                                         id = Int64.Parse(_target_platform.moid)
                                     },
                                     copyUID = 1
                                 }
                            },
                            linkPolicy = new ConsistencyGroupLinkPolicy() {
                                 protectionPolicy = new LinkProtectionPolicy()
                                 {
                                    protectionType = _protectiongroup.recoverypolicy.rp4vm_replicationmode == "synchronous" ? protectionMode.SYNCHRONOUS : protectionMode.ASYNCHRONOUS,
                                    syncReplicationLatencyThresholds = new SyncReplicationThreshold()
                                    {
                                        thresholdEnabled = false,
                                        startAsyncReplicationAbove = new Quantity()
                                        {
                                                type = quantityType.MICROSECONDS,
                                                value = 5000
                                        },
                                        resumeSyncReplicationBelow = new Quantity()
                                        {
                                            type = quantityType.MICROSECONDS,
                                            value = 3000
                                        }
                                    },
                                     syncReplicationThroughputThresholds = new SyncReplicationThreshold()
                                     {
                                          thresholdEnabled = false,
                                           resumeSyncReplicationBelow = new Quantity()
                                           {
                                                type = quantityType.KB,
                                                value = 35000
                                           },
                                            startAsyncReplicationAbove = new Quantity()
                                            {
                                                 type = quantityType.KB,
                                                  value = 45000
                                            }

                                     },
                                    rpoPolicy = new RPOPolicy()
                                    {
                                         allowRegulation = false,
                                         minimizationType = rpoMinimizationType.MINIMIZE_LAG,
                                         maximumAllowedLag = new Quantity()
                                         {
                                              type = quantityType.MINUTES,
                                              value = 15
                                         }
                                    },
                                    replicatingOverWAN = true,
                                    compression = wanCompression.MEDIUM,
                                    bandwidthLimit = 0,
                                    measureLagToTargetRPA = true,
                                    deduplication = false,
                                    weight = 1
                                 },
                                 advancedPolicy = new LinkAdvancedPolicy()
                                 {
                                    performLongInitialization = true,
                                    snapshotGranularity = snapshotGranularity.DYNAMIC
                                 },
                            },
                        },
                    };

                    _replicate_object.copies = new List<ConsistencyGroupCopyParam>() {
                     new ConsistencyGroupCopyParam()
                     {
                            copyUID = new GlobalCopyUID()
                            {
                                clusterUID = new ClusterUID()
                                {
                                    id = Int64.Parse(_source_platform.moid)
                                },
                                 copyUID = 0
                            },
                            copyName = String.Format("{0}_primary_copy", _protectiongroup.group.Replace(" ", "_").ToLower()),
                            volumeCreationParams = new ConsistencyGroupCopyVolumeCreationParams() { volumeParams =  new List<BaseVolumeParams>() {

                                new VolumeCreationParams()
                                    {
                                    volumeSize = new VolumeSize() {
                                        sizeInBytes = (long)((long)_protectiongroup.recoverypolicy.rp4vm_source_journal_size * 1024 * 1024 * 1024)
                                    },

                                    arrayUid = new ArrayUID() {
                                        id = (long)_source_journal_ds.rp4vm_arrayid,
                                        clusterUID = new ClusterUID() {
                                            id = (long)_source_journal_ds.rp4vm_clusterid
                                        }
                                    },
                                    poolUid = new ResourcePoolUID()
                                    {
                                        uuid = (long)_source_journal_ds.rp4vm_resourcepoolid,
                                        storageResourcePoolId = _source_journal_ds.moid,
                                        arrayUid = new ArrayUID() {
                                            id = (long)_source_journal_ds.rp4vm_arrayid,
                                            clusterUID = new ClusterUID() {
                                                id = (long)_source_journal_ds.rp4vm_clusterid
                                            }
                                        }
                                    },
                                    resourcePoolType = arrayResourcePoolType.VC_DATASTORE
                                },
                            }

                            }
                        },
                         new ConsistencyGroupCopyParam()
                          {
                          copyUID = new GlobalCopyUID()
                          {
                               clusterUID = new ClusterUID()
                               {
                                    id = Int64.Parse(_target_platform.moid)
                               },
                               copyUID = 1
                          },
                           copyName = String.Format("{0}_secondary_copy", _protectiongroup.group.Replace(" ", "_").ToLower()),
                            volumeCreationParams = new ConsistencyGroupCopyVolumeCreationParams() { volumeParams =  new List<BaseVolumeParams>()
                            {
                                new VolumeCreationParams()
                                    {
                                    volumeSize = new VolumeSize() {
                                        sizeInBytes = (long)((long)_protectiongroup.recoverypolicy.rp4vm_target_journal_size * 1024 * 1024 *1024)
                                    },

                                    arrayUid = new ArrayUID() {
                                        id = (long)_target_journal_ds.rp4vm_arrayid,
                                        clusterUID = new ClusterUID() {
                                            id = (long)_target_journal_ds.rp4vm_clusterid
                                        }
                                    },
                                    poolUid = new ResourcePoolUID()
                                    {
                                        uuid = (long)_target_journal_ds.rp4vm_resourcepoolid,
                                        storageResourcePoolId = _target_journal_ds.moid,
                                        arrayUid = new ArrayUID() {
                                            id = (long)_target_journal_ds.rp4vm_arrayid,
                                            clusterUID = new ClusterUID() {
                                                id = (long)_target_journal_ds.rp4vm_clusterid
                                            }
                                        }
                                    },
                                    resourcePoolType = arrayResourcePoolType.VC_DATASTORE
                                },
                                }
                            }
                        }
                    };
                    ConsistencyGroupUID _cg = _rp4vm.groups().replicateVms_Method(_replicate_object);

                    var _string = Newtonsoft.Json.JsonConvert.SerializeObject(_replicate_object);


                    List<BaseVolumeParams> _test = new List<BaseVolumeParams>() { new VolumeCreationParams() { } };

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetBaseException().Message);
                }

            };
        }
    }
}

