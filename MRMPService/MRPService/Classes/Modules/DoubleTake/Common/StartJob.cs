using DoubleTake.Web.Models;
using MRMPService.DTPollerCollection;
using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPDoubleTake;
using MRMPService.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MRMPService.Tasks.DoubleTake
{
    partial class ModuleCommon
    {
        public static async void StartJob(string _task_id, MRPWorkloadType _target_workload, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {
            await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Starting job {0} on {1}", _managementobject.moname, _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 10));
            string _contactable_ip = null;
            using (Connection _connection = new Connection())
            {
                _contactable_ip = _connection.FindConnection(_target_workload.iplist, true);
            }
            if (_contactable_ip == null)
            {
                throw new Exception(String.Format("Cannot contact workload {0}", _target_workload.hostname));
            }

            JobInfoModel _dt_job;
            using (Doubletake _dt = new Doubletake(null, _target_workload))
            {
                _dt_job = _dt.job().GetJob(Guid.Parse(_managementobject.moid)).Result;
                if (_dt_job.Status.CanStart)
                {
                    _dt.job().StartJob(Guid.Parse(_managementobject.moid)).Wait();
                }
                else
                {
                    throw new Exception(String.Format("Job cannot be started. Current state is {0} : {1}", _dt_job.Status.HighLevelState.ToString(), _dt_job.Status.LowLevelState));
                }

            }
            await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Updating job status with portal"), ReportProgress.Progress(_start_progress, _end_progress, 40));

            await Task.Delay(new TimeSpan(0, 0, 10));

            await DTJobPoller.PollerDo(_managementobject);
            await MRMPServiceBase._mrmp_api.task().successcomplete(_task_id, String.Format("Job Started successfully {0} on {1} : {2}", _managementobject.moname, _target_workload.hostname, _dt_job.Status.HighLevelState));

        }

    }
}
