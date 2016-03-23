using DoubleTake.Web.Models;
using MRPService.MRPService.Types.API;
using MRPService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.DoubleTake
{
    public class SetOptions
    {
        public static CreateOptionsModel set_job_options(MRPTaskType payload, CreateOptionsModel jobInfo)
        {
            MRPTaskWorkloadType _source_workload = payload.submitpayload.source;
            MRPTaskWorkloadType _target_workload = payload.submitpayload.target;
            MRPTaskRecoverypolicyType _recovery_policy = payload.submitpayload.servicestack.recoverypolicy;
            MRPTaskServicestackType _service_stack = payload.submitpayload.servicestack;


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
            jobInfo.JobOptions.CoreConnectionOptions.TargetAddress = Connection.FindConnection(_target_workload.iplist, true);


            return jobInfo;

        }
    }
}
