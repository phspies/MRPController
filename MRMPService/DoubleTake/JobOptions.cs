using DoubleTake.Web.Models;
using MRMPService.API.Types.API;
using MRMPService.MRMPService.Types.API;
using MRMPService.MRPService.Types.API;
using MRMPService.Utilities;
using System;
using System.Collections.Generic;

namespace MRMPService.DoubleTake
{
    public class SetOptions
    {
        public static CreateOptionsModel set_job_options(MRPTaskType payload, CreateOptionsModel jobInfo)
        {
            MRPWorkloadType _source_workload = payload.submitpayload.source;
            MRPWorkloadType _target_workload = payload.submitpayload.target;
            MRPRecoverypolicyType _recovery_policy = payload.submitpayload.servicestack.recoverypolicy;
            MRPServicestackType _service_stack = payload.submitpayload.servicestack;

            //disable backup network connection
            jobInfo.JobOptions.FullServerFailoverOptions = new FullServerFailoverOptionsModel() { CreateBackupConnection = false };

            String _job_type = null;
            if (payload.task_type.Contains("ha"))
            {
                _job_type = "Availability";
                jobInfo.JobOptions.FullServerFailoverOptions.ShutdownSourceServer = _recovery_policy.shutdown_source;
            }
            else if (payload.task_type.Contains("move"))
            {
                _job_type = "Move";
                jobInfo.JobOptions.FullServerFailoverOptions.ShutdownSourceServer = _recovery_policy.shutdown_source;
            }
            else if (payload.task_type.Contains("dr"))
            {
                _job_type = "Disaster Recovery";
                jobInfo.JobOptions.ImageRecoveryOptions.ShutdownSourceServer = _recovery_policy.shutdown_source;
            }

            jobInfo.JobOptions.Name = String.Format("MRMP [{0}] {1} to {1}", _job_type, payload.submitpayload.source.hostname, payload.submitpayload.target.hostname);

            jobInfo.JobOptions.SystemStateOptions.IsWanFailover = _recovery_policy.retain_network_configuration;
            jobInfo.JobOptions.SystemStateOptions.ApplyPorts = _recovery_policy.change_target_ports;

            //DNSOptions _dnsOptions = new DNSOptions();
            //DnsDomainDetails _dnsDomainDetails = new DnsDomainDetails();
            //_dnsOptions.Domains = new Array[0](_dnsDomainDetails);
            //jobInfo.JobOptions.DnsOptions = _dnsOptions;
            // level = 2 and algorithm = 31 Compression is enabled at high level
            if (_recovery_policy.enablecompression)
            {
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.CompressionLevel.Level = 1;
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.CompressionLevel.Algorithm = 21;
            }
            jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.MirrorParameters.ComparisonCriteria = MirrorComparisonCriteria.Newer;

            if (_recovery_policy.enablesnapshots)
            {
                TimeSpan _snapshot_timespan = new TimeSpan();
                switch (_recovery_policy.snapshotinterval)
                {
                    case "Hours":
                        _snapshot_timespan = new TimeSpan(_recovery_policy.snapshotincrement, 0, 0);
                        break;
                    case "Minutes":
                        _snapshot_timespan = new TimeSpan(0, _recovery_policy.snapshotincrement, 0);
                        break;

                }
                SnapshotScheduleModel _snapshot = new SnapshotScheduleModel();
                _snapshot.Interval = _snapshot_timespan;
                _snapshot.IsEnabled = true;
                _snapshot.MaxNumberOfSnapshots = _recovery_policy.snapshotcount;
                _snapshot.StartTime = new DateTime();
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.SnapshotSchedule = _snapshot;
            }
            //if (_recovery_policy.enablebandwidthlimit)
            //{
            //    BandwidthSchedule _bandwidth = new BandwidthSchedule();

            //    _bandwidth.Current.
            //    BandwidthSpecification _bandwidth_specification = new BandwidthSpecification();
            //    _bandwidth_specification
            //    _bandwidth.Mode = BandwidthScheduleMode.Fixed;
            //    _bandwidth.Specifications
            //    jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.Schedule.Bandwidth
            //}
            using (Connection _connection = new Connection())
            {
                //jobInfo.JobOptions.CoreConnectionOptions.TargetAddress = _connection.FindConnection(_target_workload.iplist, false);
            }

            //set dns credentials with model to the DnsOptions

            DnsOptionsModel _dns = new DnsOptionsModel();
            _dns.Domains = new DnsDomainDetailsModel[] {
                new DnsDomainDetailsModel() {
                    DnsServers = new DnsServerDetailModel[] { new DnsServerDetailModel() { Address = "" } },
                    Credentials = new CredentialModel() { Domain = "", Password = "", UserName = "" },
                    DomainName = "",
                    ShouldUpdateTtl = true,
                    }
                };
            jobInfo.JobOptions.DnsOptions = _dns;

            return jobInfo;

        }
    }
}
