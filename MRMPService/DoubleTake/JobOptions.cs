using DoubleTake.Web.Models;
using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;
using MRMPService.MRPService.Types.API;
using MRMPService.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace MRMPService.DoubleTake
{
    public class SetOptions
    {
        public static CreateOptionsModel set_job_options(MRPTaskType payload, CreateOptionsModel jobInfo)
        {
            MRPWorkloadType _source_workload = payload.submitpayload.source;
            MRPWorkloadType _target_workload = payload.submitpayload.target;
            MRPRecoverypolicyType _recovery_policy = payload.submitpayload.protectiongroup.recoverypolicy;
            MRPProtectiongroupType _service_stack = payload.submitpayload.protectiongroup;

            //disable backup network connection
            jobInfo.JobOptions.FullServerFailoverOptions = new FullServerFailoverOptionsModel() { CreateBackupConnection = false };

            String _job_type = null;
            if (jobInfo.JobType == DT_JobTypes.HA_Full_Failover)
            {
                _job_type = "Availability";
                jobInfo.JobOptions.FullServerFailoverOptions.ShutdownSourceServer = (bool)_recovery_policy.shutdown_source;
            }
            else if (jobInfo.JobType == DT_JobTypes.Move_Server_Migration)
            {
                _job_type = "Move";
                jobInfo.JobOptions.FullServerFailoverOptions.ShutdownSourceServer = (bool)_recovery_policy.shutdown_source;
            }
            else if (jobInfo.JobType == DT_JobTypes.DR_Full_Protection)
            {
                _job_type = "Disaster Recovery";
                jobInfo.JobOptions.ImageRecoveryOptions.ShutdownSourceServer = (bool)_recovery_policy.shutdown_source;
                List<ImageVhdInfoModel> vhd = new List<ImageVhdInfoModel>();
                int i = 0;
                foreach (dynamic volume in payload.submitpayload.source.workloadvolumes_attributes)
                {
                    String _repositorypath = payload.submitpayload.protectiongroup.recoverypolicy.repositorypath;
                    String _servicestack = payload.submitpayload.protectiongroup.service;
                    String _original_id = payload.submitpayload.original.id;
                    String _volume = volume.driveletter;
                    Int16 _disksize = volume.disksize;
                    Char _shortvolume = _volume[0];
                    String _filename = _original_id + "_" + _shortvolume + ".vhdx";
                    string absfilename = Path.Combine(_repositorypath, _servicestack.ToLower().Replace(" ", "_"), _original_id, _filename);
                    vhd.Add(new ImageVhdInfoModel() { FormatType = "ntfs", VolumeLetter = _shortvolume.ToString(), UseExistingVhd = false, FilePath = absfilename, SizeInMB = (_disksize * 1024) });
                    using (MRMP_ApiClient _mrp_api = new MRMP_ApiClient())
                    {
                        _mrp_api.task().progress(payload, String.Format("Volume {0} being synced to {1} on repository server", _shortvolume.ToString(), absfilename), 51 + i);
                    }
                    i += 1;
                }
                jobInfo.JobOptions.ImageProtectionOptions.VhdInfo = vhd.ToArray();
                jobInfo.JobOptions.ImageProtectionOptions.ImageName = (String)payload.target_id;
            }

            jobInfo.JobOptions.Name = String.Format("MRMP [{0}] {1} to {2}", _job_type, payload.submitpayload.source.hostname, payload.submitpayload.target.hostname);

            jobInfo.JobOptions.SystemStateOptions.IsWanFailover = (bool)_recovery_policy.retain_network_configuration;
            jobInfo.JobOptions.SystemStateOptions.ApplyPorts = _recovery_policy.change_target_ports;

            // level = 2 and algorithm = 31 Compression is enabled at high level
            if ((bool)_recovery_policy.enablecompression)
            {
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.CompressionLevel.Level = 1;
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.CompressionLevel.Algorithm = 21;
            }
            jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.MirrorParameters.ComparisonCriteria = MirrorComparisonCriteria.Newer;

            if ((bool)_recovery_policy.enablesnapshots)
            {
                TimeSpan _snapshot_timespan = new TimeSpan();
                switch (_recovery_policy.snapshotinterval)
                {
                    case "Hours":
                        _snapshot_timespan = new TimeSpan((int)_recovery_policy.snapshotincrement, 0, 0);
                        break;
                    case "Minutes":
                        _snapshot_timespan = new TimeSpan(0, (int)_recovery_policy.snapshotincrement, 0);
                        break;

                }
                SnapshotScheduleModel _snapshot = new SnapshotScheduleModel();
                _snapshot.Interval = _snapshot_timespan;
                _snapshot.IsEnabled = true;
                _snapshot.MaxNumberOfSnapshots = _recovery_policy.snapshotcount;
                _snapshot.StartTime = new DateTime();
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.SnapshotSchedule = _snapshot;
            }

            //set dns credentials with model to the DnsOptions
            if ((bool)_recovery_policy.dns_set_dns)
            {
                DnsOptionsModel _dns = new DnsOptionsModel();
                List<DnsServerDetailModel> _dns_servers = new List<DnsServerDetailModel>();
                if (_recovery_policy.dns_servers.Contains(","))
                {
                    foreach (string _dns_server in _recovery_policy.dns_servers.Split(','))
                    {
                        System.Net.IPAddress ipAddress = null;
                        if (System.Net.IPAddress.TryParse(_dns_server, out ipAddress))
                        {
                            _dns_servers.Add(new DnsServerDetailModel() { Address = _dns_server });
                        }
                        else
                        {
                            using (MRMP_ApiClient _mrp_api = new MRMP_ApiClient())
                            {
                                _mrp_api.task().progress(payload, String.Format("Error in setting DNS server {0}",_dns_server), 51);
                            }
                        }
                    }
                }
                else
                {
                    System.Net.IPAddress ipAddress = null;
                    if (System.Net.IPAddress.TryParse(_recovery_policy.dns_servers, out ipAddress))
                    {
                        _dns_servers.Add(new DnsServerDetailModel() { Address = _recovery_policy.dns_servers });
                    }
                    else
                    {
                        using (MRMP_ApiClient _mrp_api = new MRMP_ApiClient())
                        {
                            _mrp_api.task().progress(payload, String.Format("Error in setting DNS server {0}", _recovery_policy.dns_servers), 51);
                        }
                    }
                }


                _dns.Domains[0] = new DnsDomainDetailsModel()
                {
                    DnsServers = _dns_servers.ToArray(),
                    Credentials = new CredentialModel() { Domain = _recovery_policy.dns_user_domain, Password = _recovery_policy.dns_password, UserName = _recovery_policy.dns_username },
                    DomainName = _recovery_policy.dns_domain,
                    ShouldUpdateTtl = (bool)_recovery_policy.dns_should_update_ttl,
                    TtlValue = (int)_recovery_policy.dns_ttl
                };
                jobInfo.JobOptions.DnsOptions = _dns;
            }
            return jobInfo;

        }
    }
}
