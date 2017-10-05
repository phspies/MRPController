using DoubleTake.Web.Models;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPDoubleTake;
using MRMPService.Scheduler.DTPollerCollection;
using MRMPService.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MRMPService.Modules.DoubleTake.Common
{
    partial class ModuleCommon
    {
        public static void StopJob(MRPTaskType _task, MRMPWorkloadBaseType _target_workload, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {
            _task.progress(String.Format("Stopping job {0} on {1}", _managementobject.moname, _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 10));

            string _contactable_ip = _target_workload.GetContactibleIP(true);
            JobInfoModel _dt_job;
            using (Doubletake _dt = new Doubletake(null, _target_workload))
            {
                _dt_job = _dt.job().GetJob(Guid.Parse(_managementobject.moid));
                if (_dt_job.Status.CanStop)
                {
                    _dt.job().StopJob(Guid.Parse(_managementobject.moid));
                }
                else
                {
                    throw new Exception(String.Format("Job cannot be stopped. current state is {0} : {1}", _dt_job.Status.HighLevelState.ToString(), _dt_job.Status.LowLevelState));
                }
                int _retries = 5;
                while (true)
                {
                    _dt_job = _dt.job().GetJob(Guid.Parse(_managementobject.moid));
                    _task.progress(String.Format("Waiting for job to stop {0} : {1}", _managementobject.moname, _dt_job.Status.HighLevelState), ReportProgress.Progress(_start_progress, _end_progress, _retries + 21));
                    if (_dt_job.Status.HighLevelState != HighLevelState.Stopped)
                    {
                        if (_retries-- == 0)
                        {
                            throw new Exception(String.Format("Job was stopped but failed to transition {0} : {1}", _dt_job.Status.HighLevelState.ToString(), _dt_job.Status.LowLevelState));
                        }
                        Thread.Sleep(new TimeSpan(0, 0, 5));
                    }
                    else
                    {
                        _task.progress(String.Format("Job stopped successfully {0} on {1} : {2}", _managementobject.moname, _target_workload.hostname, _dt_job.Status.HighLevelState), ReportProgress.Progress(_start_progress, _end_progress, 30));
                        break;
                    }
                }
            }
            _task.progress(String.Format("Updating job status with portal"), ReportProgress.Progress(_start_progress, _end_progress, 40));
            Task.Delay(new TimeSpan(0, 0, 10));
            DTJobPoller.PollerDo(_managementobject);
            _task.successcomplete(String.Format("Job stopped successfully {0} on {1} : {2}", _managementobject.moname, _target_workload.hostname, _dt_job.Status.HighLevelState));
        }
    }
}

