using DoubleTake.Web.Models;
using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;
using MRMPService.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace MRMPService.MRMPDoubleTake
{
    public class SetOptions
    {
        public static CreateOptionsModel set_job_options(string _task_id, MRPWorkloadType _source_workload, MRPWorkloadType _target_workload, MRPProtectiongroupType _protectiongroup, CreateOptionsModel jobInfo, float _start_progress, float _end_progress)
        {

            //disable backup network connection
            jobInfo.JobOptions.FullServerFailoverOptions = new FullServerFailoverOptionsModel() { CreateBackupConnection = false };

            String _job_type = null;
            if (jobInfo.JobType == DT_JobTypes.HA_Full_Failover)
            {
                _job_type = "Availability";
                jobInfo.JobOptions.FullServerFailoverOptions.ShutdownSourceServer = (bool)_protectiongroup.recoverypolicy.shutdown_source;
            }
            else if (jobInfo.JobType == DT_JobTypes.Move_Server_Migration)
            {
                _job_type = "Move";
                jobInfo.JobOptions.FullServerFailoverOptions.ShutdownSourceServer = (bool)_protectiongroup.recoverypolicy.shutdown_source;
            }
            else if (jobInfo.JobType == DT_JobTypes.DR_Full_Protection)
            {
                _job_type = "Disaster Recovery";
                jobInfo.JobOptions.ImageRecoveryOptions.ShutdownSourceServer = (bool)_protectiongroup.recoverypolicy.shutdown_source;
                List<ImageVhdInfoModel> vhd = new List<ImageVhdInfoModel>();
                int i = 0;
                foreach (dynamic volume in _source_workload.workloadvolumes_attributes)
                {
                    String _repositorypath = _protectiongroup.recoverypolicy.repositorypath;
                    String _servicestack = _protectiongroup.service;
                    String _original_id = _source_workload.id;
                    String _volume = volume.driveletter;
                    Int16 _disksize = volume.disksize;
                    Char _shortvolume = _volume[0];
                    String _filename = _original_id + "_" + _shortvolume + ".vhdx";
                    string absfilename = Path.Combine(_repositorypath, _servicestack.ToLower().Replace(" ", "_"), _original_id, _filename);
                    vhd.Add(new ImageVhdInfoModel() { FormatType = "ntfs", VolumeLetter = _shortvolume.ToString(), UseExistingVhd = false, FilePath = absfilename, SizeInMB = (_disksize * 1024) });
                    using (MRMP_ApiClient _mrp_api = new MRMP_ApiClient())
                    {
                        _mrp_api.task().progress(_task_id, String.Format("Volume {0} being synced to {1} on repository server", _shortvolume.ToString(), absfilename), ReportProgress.Progress(_start_progress, _end_progress, 51 + i));
                    }
                    i += 1;
                }
                jobInfo.JobOptions.ImageProtectionOptions.VhdInfo = vhd.ToArray();
                jobInfo.JobOptions.ImageProtectionOptions.ImageName = (String)_target_workload.id;
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

            //set change ports
            jobInfo.JobOptions.SystemStateOptions.ApplyPorts = _protectiongroup.recoverypolicy.change_target_ports;

            // level = 2 and algorithm = 31 Compression is enabled at high level
            if ((bool)_protectiongroup.recoverypolicy.enablecompression)
            {
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.CompressionLevel.Level = 1;
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.CompressionLevel.Algorithm = 21;
            }
            jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.MirrorParameters.ComparisonCriteria = MirrorComparisonCriteria.Newer;

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
                _snapshot.MaxNumberOfSnapshots = _protectiongroup.recoverypolicy.snapshotcount;
                _snapshot.StartTime = new DateTime();
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.SnapshotSchedule = _snapshot;
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
            return jobInfo;

        }
    }
}
