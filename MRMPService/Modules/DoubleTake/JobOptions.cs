﻿using DoubleTake.Web.Models;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MRMPService.MRMPDoubleTake
{
    public class SetOptions
    {
        public static CreateOptionsModel set_job_options(MRPTaskType _task, MRMPWorkloadBaseType _source_workload, MRMPWorkloadBaseType _target_workload, MRPProtectiongroupType _protectiongroup, CreateOptionsModel jobInfo, float _start_progress, float _end_progress, MRPManagementobjectType _managementobject = null, MRPManagementobjectSnapshotType _recovery_snapshot = null)
        {
            //set replication path if source and target has replication nics defined
            MRPWorkloadInterfaceType _target_rep_nic = _target_workload?.workloadinterfaces?.FirstOrDefault(x => x.replication_interface == true);
            if (_target_rep_nic != null)
            {
                if ((new string[] { "ipv4", "ipv6" }).Any(x => x == _target_rep_nic.replication_interface_iptype))
                {
                    var _target_address = _target_rep_nic.replication_interface_iptype == "ipv4" ? _target_rep_nic.ipaddress : _target_rep_nic.ipv6address;
                    _task.progress(String.Format("Replication route on target workload set to {0}.", _target_address), ReportProgress.Progress(_start_progress, _end_progress, 10));
                    jobInfo.JobOptions.CoreConnectionOptions.TargetAddress = _target_address;
                }
                else
                {
                    _task.progress(String.Format("No replication protocol set on target workload. Using default route."), ReportProgress.Progress(_start_progress, _end_progress, 10));
                }
            }
            else
            {
                _task.progress(String.Format("No replication interface set on target workload. Using default route."), ReportProgress.Progress(_start_progress, _end_progress, 10));
            }
            String _job_type = null;
            if (jobInfo.JobType == DT_JobTypes.HA_Full_Failover || jobInfo.JobType == DT_JobTypes.HA_Linux_FullFailover)
            {
                _job_type = "Availability";
                jobInfo.JobOptions.FullServerFailoverOptions.ShutdownSourceServer = (bool)_protectiongroup.recoverypolicy.shutdown_source;

                //disable backup network connection
                jobInfo.JobOptions.FullServerFailoverOptions = new FullServerFailoverOptionsModel() { CreateBackupConnection = false };
                //set change ports
                jobInfo.JobOptions.SystemStateOptions.ApplyPorts = _protectiongroup.recoverypolicy.change_target_ports;
            }
            else if (jobInfo.JobType == DT_JobTypes.Move_Server_Migration)
            {
                _job_type = "Move";
                jobInfo.JobOptions.FullServerFailoverOptions.ShutdownSourceServer = (bool)_protectiongroup.recoverypolicy.shutdown_source;
                //set change ports
                jobInfo.JobOptions.SystemStateOptions.ApplyPorts = _protectiongroup.recoverypolicy.change_target_ports;
            }
            else if (jobInfo.JobType == DT_JobTypes.DR_Full_Protection)
            {
                _job_type = "DR Protection";
                //jobInfo.JobOptions.ImageRecoveryOptions.ShutdownSourceServer = (bool)_protectiongroup.recoverypolicy.shutdown_source;

                //set change ports
                jobInfo.JobOptions.SystemStateOptions.ApplyPorts = _protectiongroup.recoverypolicy.change_target_ports;

                //disable backup network connection
                jobInfo.JobOptions.FullServerFailoverOptions = new FullServerFailoverOptionsModel() { CreateBackupConnection = false };

                //if (!_target_workload.workloadvolumes_attributes.Exists(x => x.driveletter == _protectiongroup.recoverypolicy.repositorypath[0].ToString()))
                //{
                //    using (MRMP_ApiClient _mrp_api = new MRMP_ApiClient())
                //    {
                //        _mrp_api.task().progress(_task_id, String.Format("Warning: {0} volume not found on the repository server", _protectiongroup.recoverypolicy.repositorypath), ReportProgress.Progress(_start_progress, _end_progress, 10));
                //    }
                // }


                List<ImageVhdInfoModel> vhd = new List<ImageVhdInfoModel>();
                int i = 0;
                foreach (MRPWorkloadVolumeType volume in _source_workload.workloadvolumes)
                {
                    String _repositorypath = _protectiongroup.recoverypolicy.repositorypath;
                    long _disksize = (long)volume.volumesize;
                    Char _shortvolume = volume.driveletter[0];
                    String _filename = _source_workload.id + "_" + _shortvolume + ".vhdx";
                    string absfilename = Path.Combine(_repositorypath, _protectiongroup.id, _source_workload.id, _filename);
                    vhd.Add(new ImageVhdInfoModel() { FormatType = "ntfs", VolumeLetter = _shortvolume.ToString(), UseExistingVhd = false, FilePath = absfilename, SizeInMB = (_disksize * 1024) });

                    _task.progress(String.Format("Volume {0} being synced to {1} on repository server", _shortvolume.ToString(), absfilename), ReportProgress.Progress(_start_progress, _end_progress, 51 + i));
                    i += 1;
                }
                jobInfo.JobOptions.ImageProtectionOptions.VhdInfo = vhd.ToArray();
                jobInfo.JobOptions.ImageProtectionOptions.ImageName = String.Format("dr_dormant_{0}_image", _source_workload.hostname.ToLower());
            }

            else if (jobInfo.JobType == DT_JobTypes.DR_Full_Recovery)
            {
                _job_type = "DR Recovery";

                jobInfo.JobOptions.ImageRecoveryOptions.ShutdownSourceServer = false;
            }
            jobInfo.JobOptions.Name = String.Format("MRMP [{0}] {1} to {2}", _job_type, _source_workload.hostname, _target_workload.hostname);

            //Set snapshot ID when recovering from a snapshot
            //jobInfo.JobOptions.ImageRecoveryOptions.SnapshotSetId

            //set ssm staging folder if present
            //if (!String.IsNullOrEmpty(_protectiongroup.recoverypolicy.windows_staging_path) || !String.IsNullOrEmpty(_protectiongroup.recoverypolicy.linux_staging_path))
            //{
            //    jobInfo.JobOptions.SystemStateOptions.AlternateVolumeStaging = true;
            //    jobInfo.JobOptions.SystemStateOptions.StagingFolder = _target_workload.ostype.ToLower() == "windows" ? _protectiongroup.recoverypolicy.windows_staging_path : _protectiongroup.recoverypolicy.linux_staging_path;
            //}

            //set retain network settings option
            jobInfo.JobOptions.SystemStateOptions.IsWanFailover = (bool)_protectiongroup.recoverypolicy.retain_network_configuration;



            // level = 2 and algorithm = 31 Compression is enabled at high level
            if ((bool)_protectiongroup.recoverypolicy.enablecompression)
            {
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.CompressionLevel.Level = 1;
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.CompressionLevel.Algorithm = 21;
            }
            jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.MirrorParameters.ComparisonCriteria = MirrorComparisonCriteria.Checksum;

            if (!jobInfo.JobType.Contains("Recovery"))
            {
                if ((bool)_protectiongroup.recoverypolicy.enablesnapshots)
                {
                    TimeSpan _snapshot_timespan = new TimeSpan();
                    switch (_protectiongroup.recoverypolicy.snapshotinterval)
                    {
                        case "Hours":
                            _snapshot_timespan = new TimeSpan((int)_protectiongroup.recoverypolicy.snapshotincrement, 0, 0);
                            break;
                        case "Minutes":
                            _snapshot_timespan = new TimeSpan(0, (int)_protectiongroup.recoverypolicy.snapshotincrement, 0);
                            break;

                    }
                    SnapshotScheduleModel _snapshot = new SnapshotScheduleModel();
                    _snapshot.Interval = _snapshot_timespan;

                    _snapshot.IsEnabled = true;
                    _snapshot.MaxNumberOfSnapshots = _protectiongroup.recoverypolicy.snapshotmaxcount == null ? 0 : (int)_protectiongroup.recoverypolicy.snapshotmaxcount;
                    if (_protectiongroup.recoverypolicy.snapshotstarttimestamp != null)
                    {
                        _snapshot.StartTime = (DateTime)_protectiongroup.recoverypolicy.snapshotstarttimestamp;
                    }
                    jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.SnapshotSchedule = _snapshot;
                }
            }
            else
            {
                if (_protectiongroup.recoverypolicy.enablebandwidthlimit)
                {
                    BandwidthOptionsModel bwOptions = new BandwidthOptionsModel()
                    {
                        Mode = BandwidthScheduleMode.Fixed,
                        Limit = (long)_protectiongroup.recoverypolicy.bandwidthlimit
                    };

                    jobInfo.JobOptions.BandwidthOptions = bwOptions;

                }
            }

            //set dns credentials with model to the DnsOptions
            if ((bool)_protectiongroup.recoverypolicy.dns_set_dns)
            {
                DnsOptionsModel _dns = new DnsOptionsModel();
                List<DnsServerDetailModel> _dns_servers = new List<DnsServerDetailModel>();
                if (_protectiongroup.recoverypolicy.dns_servers.Contains(","))
                {
                    foreach (string _dns_server in _protectiongroup.recoverypolicy.dns_servers.Split(','))
                    {
                        System.Net.IPAddress ipAddress = null;
                        if (System.Net.IPAddress.TryParse(_dns_server, out ipAddress))
                        {
                            _dns_servers.Add(new DnsServerDetailModel() { Address = _dns_server });
                        }
                        else
                        {
                            throw new Exception(String.Format("Error in setting DNS server {0}", _dns_server));
                        }
                    }
                }
                else
                {
                    System.Net.IPAddress ipAddress = null;
                    if (System.Net.IPAddress.TryParse(_protectiongroup.recoverypolicy.dns_servers, out ipAddress))
                    {
                        _dns_servers.Add(new DnsServerDetailModel() { Address = _protectiongroup.recoverypolicy.dns_servers });
                    }
                    else
                    {
                        throw new Exception(String.Format("Error in setting DNS server {0}", _protectiongroup.recoverypolicy.dns_servers));
                    }
                }
                _dns.Domains[0] = new DnsDomainDetailsModel()
                {
                    DnsServers = _dns_servers.ToArray(),
                    Credentials = new CredentialModel() { Domain = _protectiongroup.recoverypolicy.dns_user_domain, Password = _protectiongroup.recoverypolicy.dns_password, UserName = _protectiongroup.recoverypolicy.dns_username },
                    DomainName = _protectiongroup.recoverypolicy.dns_domain,
                    ShouldUpdateTtl = (bool)_protectiongroup.recoverypolicy.dns_should_update_ttl,
                    TtlValue = (int)_protectiongroup.recoverypolicy.dns_ttl
                };
                jobInfo.JobOptions.DnsOptions = _dns;
            }

            jobInfo.JobOptions.MonitoringOptions = new MonitoringOptionsModel() { ServiceMonitoringEnabled = false };
            jobInfo.JobOptions.FailoverMonitoringEnabled = false;

            return jobInfo;

        }
    }
}
