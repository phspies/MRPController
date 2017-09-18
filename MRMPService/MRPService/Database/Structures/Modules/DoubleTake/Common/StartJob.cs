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
        public static void StartJob(MRPTaskType _task, MRMPWorkloadBaseType _target_workload, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {
            _task.progress(String.Format("Starting job {0} on {1}", _managementobject.moname, _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 10));
            string _contactable_ip = _target_workload.GetContactibleIP(true);
            JobInfoModel _dt_job;
            using (Doubletake _dt = new Doubletake(null, _target_workload))
            {
                _dt_job = _dt.job().GetJob(Guid.Parse(_managementobject.moid));
                if (_dt_job.Status.CanStart)
                {
                    _dt.job().StartJob(Guid.Parse(_managementobject.moid));
                }
                else
                {
                    throw new Exception(String.Format("Job cannot be started. Current state is {0} : {1}", _dt_job.Status.HighLevelState.ToString(), _dt_job.Status.LowLevelState));
                }
            }
            _task.progress(String.Format("Updating job status with portal"), ReportProgress.Progress(_start_progress, _end_progress, 40));
            Task.Delay(new TimeSpan(0, 0, 10));
            DTJobPoller.PollerDo(_managementobject);
            _task.successcomplete(String.Format("Job Started successfully {0} on {1} : {2}", _managementobject.moname, _target_workload.hostname, _dt_job.Status.HighLevelState));
        }
    }
}