using DoubleTake.Web.Models;
using MRMPService.DTPollerCollection;
using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPDoubleTake;
using MRMPService.Utilities;
using System;
using System.Threading;

namespace MRMPService.Tasks.DoubleTake
{
    partial class ModuleCommon
    {
        public static void PauseJob(string _task_id, MRPWorkloadType _target_workload, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {
            using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
            {

                _mrp_portal.task().progress(_task_id, String.Format("Pausing job {0} on {1}", _managementobject.moname, _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 10));

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
                    _dt_job = _dt.job().GetJob((Guid)_managementobject.moid).Result;
                    if (_dt_job.Status.CanPause)
                    {
                        _dt.job().PauseJob((Guid)_managementobject.moid).Wait();
                    }
                    else
                    {
                        throw new Exception(String.Format("Job cannot be paused. Current state is {0} : {1}", _dt_job.Status.HighLevelState.ToString(), _dt_job.Status.LowLevelState));
                    }

                }
                _mrp_portal.task().progress(_task_id, String.Format("Updating job status with portal"), ReportProgress.Progress(_start_progress, _end_progress, 40));
                Thread.Sleep(new TimeSpan(0, 0, 10));
                DTJobPoller.PollerDo(_managementobject);
                _mrp_portal.task().successcomplete(_task_id, String.Format("Job paused successfully {0} on {1} : {2}", _managementobject.moname, _target_workload.hostname, _dt_job.Status.HighLevelState));
            }
        }
    }
}
