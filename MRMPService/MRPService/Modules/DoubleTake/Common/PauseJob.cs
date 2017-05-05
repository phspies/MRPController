using DoubleTake.Web.Models;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPDoubleTake;
using MRMPService.Scheduler.DTPollerCollection;
using MRMPService.Utilities;
using System;
using System.Threading.Tasks;

namespace MRMPService.Modules.DoubleTake.Common
{
    partial class ModuleCommon
    {
        public static async Task PauseJob(string _task_id, MRPWorkloadType _target_workload, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {

            MRMPServiceBase._mrmp_api.task().progress(_task_id, string.Format("Pausing job {0} on {1}", _managementobject.moname, _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 10));

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
                _dt_job = _dt.job().GetJob(Guid.Parse(_managementobject.moid));
                if (_dt_job.Status.CanPause)
                {
                    _dt.job().PauseJob(Guid.Parse(_managementobject.moid));
                }
                else
                {
                    throw new Exception(String.Format("Job cannot be paused. Current state is {0} : {1}", _dt_job.Status.HighLevelState.ToString(), _dt_job.Status.LowLevelState));
                }

            }
            MRMPServiceBase._mrmp_api.task().progress(_task_id, string.Format("Updating job status with portal"), ReportProgress.Progress(_start_progress, _end_progress, 40));
            await Task.Delay(new TimeSpan(0, 0, 10));
            DTJobPoller.PollerDo(_managementobject);
            MRMPServiceBase._mrmp_api.task().successcomplete(_task_id, String.Format("Job paused successfully {0} on {1} : {2}", _managementobject.moname, _target_workload.hostname, _dt_job.Status.HighLevelState));
        }
    }
}
