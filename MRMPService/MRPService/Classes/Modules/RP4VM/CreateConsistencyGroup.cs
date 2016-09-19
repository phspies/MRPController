using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;
using MRMPService.RP4VM;
using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.Tasks.RP4VM
{
    class CreateConsistencyGroup
    {
        public static void CreateServerMigrationJob(string _task_id, List<MRPWorkloadType> _source_workloads, MRPPlatformType _platform, MRPProtectiongroupType _protectiongroup, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)

        {
            using (MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                RP4VM_ApiClient _rp4vm = new RP4VM_ApiClient(_platform.rp4vm_url, _platform.credential.username, _platform.credential.encrypted_password);

                ReplicateVmsParam _replicate_object = new ReplicateVmsParam()
                {
                    cgName = _protectiongroup.service,
                    productionCopy = new GlobalCopyUID()
                    {
                        clusterUID = new ClusterUID() { id = 23452345234 },
                        copyUID = 0
                    },
                    vmReplicationSets = new List<VMReplicationSetParam>() {
                        new VMReplicationSetParam() {
                            replicationSetVms = new List<ReplicatedVMParams>() {
                                new ReplicatedVMParams() {
                                    copyUID = new GlobalCopyUID() { clusterUID = new ClusterUID() { id = 1341234123 } },
                                    vmParam = new SourceVmParam() {
                                        clusterUID = new ClusterUID() {
                                            id = 341234
                                        },
                                        vmUID = new VmUID() {
                                            uuid = "virtual server uuid",
                                            virtualCenterUID = new VirtualCenterUID() { uuid = "virtual center uuid" }
                                        }
                                    },
                                },
                            },
                            virtualHardwareReplicationPolicy = new VirtualHardwareReplicationPolicy() { hwChangesPolicy = hardwareChangesPolicy.REPLICATE_HW_CHANGES, provisionPolicy = diskProvisionPolicy.SAME_AS_SOURCE },
                            virtualDisksReplicationPolicy = new VirtualDisksReplicationPolicy() {
                            autoReplicateNewVirtualDisks = true
                            }
                        },
                        new VMReplicationSetParam() {
                            replicationSetVms = new List<ReplicatedVMParams>() {
                                new ReplicatedVMParams() {
                                    copyUID = new GlobalCopyUID() { clusterUID = new ClusterUID() { id = 1341234123 } },
                                    vmParam = new SourceVmParam() {
                                        clusterUID = new ClusterUID() {
                                            id = 341234
                                        },
                                        vmUID = new VmUID() {
                                            uuid = "virtual server uuid",
                                            virtualCenterUID = new VirtualCenterUID() { uuid = "virtual center uuid" }
                                        }
                                    },
                                },
                            },
                            virtualHardwareReplicationPolicy = new VirtualHardwareReplicationPolicy() { hwChangesPolicy = hardwareChangesPolicy.REPLICATE_HW_CHANGES, provisionPolicy = diskProvisionPolicy.SAME_AS_SOURCE },
                            virtualDisksReplicationPolicy = new VirtualDisksReplicationPolicy() {
                            autoReplicateNewVirtualDisks = true
                            }
                        },
                    },
                    links = new List<FullConsistencyGroupLinkPolicy>() {
                        new FullConsistencyGroupLinkPolicy() {
                            linkUID = new ConsistencyGroupLinkUID() {
                                firstCopy =new GlobalCopyUID() {
                                    clusterUID = new ClusterUID() {
                                        id = 3245234563456
                                    }
                                },
                                 secondCopy = new GlobalCopyUID()
                                 {
                                     clusterUID = new ClusterUID()
                                     {
                                         id = 4523545234
                                     }
                                 }
                            },
                            linkPolicy = new ConsistencyGroupLinkPolicy() {
                                 protectionPolicy = new LinkProtectionPolicy()
                                 {
                                    protectionType = protectionMode.ASYNCHRONOUS,
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
                    },
                    copies = new List<ConsistencyGroupCopyParam>() {
                     new ConsistencyGroupCopyParam()
                     {
                          copyUID = new GlobalCopyUID()
                          {
                               clusterUID = new ClusterUID()
                               {
                                    id = 8798798
                               }
                          },
                           copyName = "copy-name....",
                            volumeCreationParams = new List<BaseVolumeParams>()
                            {
                              new VolumeCreationParams()
                              {
                                   volumeSize = new VolumeSize() { sizeInBytes = 09809809 },
                                   arrayUid = new ArrayUID() {
                                       id = 0970998798,
                                       clusterUID = new ClusterUID() {
                                           id = 8798798798
                                       }
                                   },
                                    poolUid = new ResourcePoolUID()
                                    {
                                        uuid = 9080980980,
                                        arrayUid = new ArrayUID() {
                                            clusterUID = new ClusterUID() {
                                                id = 09809809
                                            }
                                        }
                                    },
                                    resourcePoolType = arrayResourcePoolType.VC_DATASTORE


                              },
                              new VolumeCreationParams()
                              {
                                   volumeSize = new VolumeSize() { sizeInBytes = 09809809 },
                                   arrayUid = new ArrayUID() {
                                       id = 0970998798,
                                       clusterUID = new ClusterUID() {
                                           id = 8798798798
                                       }
                                   },
                                    poolUid = new ResourcePoolUID()
                                    {
                                        uuid = 9080980980,
                                        arrayUid = new ArrayUID() {
                                            clusterUID = new ClusterUID() {
                                                id = 09809809
                                            }
                                        }
                                    },
                                    resourcePoolType = arrayResourcePoolType.VC_DATASTORE


                              }
                            }
                     }
                     }

                };
                ConsistencyGroupUID _cg = _rp4vm.groups().replicateVms_Method(_replicate_object);
            }
        }
    }
}
