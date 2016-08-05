using DoubleTake.Web.Models;
using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPDoubleTake;
using MRMPService.MRMPService.Types.API;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace MRMPService.Tasks.DoubleTake
{
    partial class Migration
    {
        public static void FailoverServerMigration(string _task_id, MRPWorkloadType _source_workload, MRPWorkloadType _target_workload, MRPProtectiongroupType _protectiongroup, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {
            using (MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {


                using (Doubletake _dt = new Doubletake(_source_workload, _target_workload))
                {
                    _mrp_api.task().progress(_task_id, "Verifying license status on both source and target workloads", 2);
                    if (!_dt.management().CheckLicense(DT_JobTypes.Move_Server_Migration, _protectiongroup.organization_id))
                    {
                        _mrp_api.task().progress(_task_id, String.Format("Invalid license detected on workloads."), 3);
                        _mrp_api.task().progress(_task_id, String.Format("Attempting to configure license in deployment policy."), 4);
                        if (!_dt.management().CheckLicense(DT_JobTypes.Move_Server_Migration, _protectiongroup.organization_id))
                        {
                            _mrp_api.task().progress(_task_id, String.Format("Invalid license detected on workloads after trying to fix license"), 5);
                        }
                    }
                    else
                    {
                        _mrp_api.task().progress(_task_id, String.Format("License valid on workloads."), 3);
                    }

                    _mrp_api.task().progress(_task_id, "Initiating job move operation", 10);
                    FailoverOptionsModel _options = new FailoverOptionsModel();

                    _options.FailoverType = FailoverType.FullServer;
                    _options.FailoverMode = FailoverMode.Live;
                    _options.FailoverDataAction = FailoverDataAction.Apply;

                    ActivityStatusModel _status = _dt.job().FailoverJob((Guid)_managementobject.moid, _options);

                    _mrp_api.task().progress(_task_id, "Setting source workload to disabled", 20);
                    using (MRMP_ApiClient _api = new MRMP_ApiClient())
                    {
                        _target_workload.enabled = false;
                        _api.workload().updateworkload(_target_workload);

                        _source_workload.enabled = true;
                        _source_workload.iplist = _target_workload.iplist;
                        _source_workload.platform_id = _target_workload.platform_id;
                        _api.workload().updateworkload(_source_workload);
                    }
                    _mrp_api.task().progress(_task_id, "Move process started", 30);
                    JobInfoModel jobinfo = _dt.job().GetJob(Guid.Parse(_managementobject.moid.ToString())).Result;
                    int _wait_times = 1;
                    while (jobinfo.Status.HighLevelState != HighLevelState.FailingOver)
                    {
                        _mrp_api.task().progress(_task_id, "Waiting for job to start the failover", 30);
                        Thread.Sleep(TimeSpan.FromSeconds(2));
                        _wait_times++;
                        jobinfo = _dt.job().GetJob(Guid.Parse(_managementobject.moid.ToString())).Result;
                    }

                    while (jobinfo.Status.HighLevelState == HighLevelState.FailingOver)
                    {
                        int percentcomplete = jobinfo.Statistics.FullServerJobDetails.CutoverDetails.PercentComplete;

                        String progress = String.Format("{0}% complete", percentcomplete);
                        _mrp_api.task().progress(_task_id, progress, (((double)percentcomplete / 100) * 60) + 30);

                        Thread.Sleep(TimeSpan.FromSeconds(10));
                        DateTime timeoutTime = DateTime.UtcNow.AddMinutes(15);
                        while (true)
                        {
                            if (DateTime.UtcNow > timeoutTime)
                            {
                                _mrp_api.task().progress(_task_id, String.Format("Timeout waiting for target workload {0} to become available", _target_workload.hostname), 94);
                                _mrp_api.task().successcomplete(_task_id, JsonConvert.SerializeObject(jobinfo));
                                return;
                            }
                            try
                            {
                                jobinfo = _dt.job().GetJob(Guid.Parse(_managementobject.moid.ToString())).Result;
                                break;
                            }
                            catch (Exception ex)
                            {
                                _mrp_api.task().progress(_task_id, String.Format("Waiting for target workload {0} to become available", _target_workload.hostname), 93);
                                Thread.Sleep(TimeSpan.FromSeconds(30));
                            }
                        }
                    }
                    if (jobinfo.Status.HighLevelState == HighLevelState.FailedOver)
                    {
                        _mrp_api.task().progress(_task_id, String.Format("Successfully moved {0} to {1}", _source_workload.hostname, _target_workload.hostname), 95);
                        _mrp_api.task().successcomplete(_task_id, JsonConvert.SerializeObject(jobinfo));
                    }
                    else if (jobinfo.Status.HighLevelState == HighLevelState.FailoverFailed)
                    {
                        _mrp_api.task().progress(_task_id, String.Format("Error moving {0} to {1}", _source_workload.hostname, _target_workload.hostname), 95);
                        _mrp_api.task().failcomplete(_task_id, JsonConvert.SerializeObject(jobinfo));
                    }
                    else if (jobinfo.Status.HighLevelState == HighLevelState.TargetInfoNotAvailable)
                    {
                        _mrp_api.task().progress(_task_id, String.Format("Cannot contact {0}", _target_workload.hostname), 95);
                        _mrp_api.task().failcomplete(_task_id, JsonConvert.SerializeObject(jobinfo));
                    }
                }


            }
        }
    }
}
//public class MoveServerMigrationJobEx : FullServerBase, IExample
//{
//    public MoveServerMigrationJobEx() : base("MoveServerMigration") { }

//    public async Task Execute(string[] args)
//    {
//        if (CommandLineHelper.Parser.ParseArguments(args, Options))
//        {
//            SimpleLog.Log("Creating Move Server Migration Failover job.");

//            // note: The job must be created on the TARGET machine
//            var connection = await ManagementService.GetConnectionAsync(Options.Target);
//            jobApi = new JobsApi(connection);

//            WorkloadModel workload = await CreateWorkload();

//            JobCredentialsModel jobCredentials = CreateJobCredentials();

//            // Create the job options
//            CreateOptionsModel createOptions = await GetJobOptions(workload, jobCredentials);

//            // Verify the options are good and update the CreateOptions with the possibly fixed values
//            createOptions.JobOptions = await VerifyAndFixJobOptions(jobCredentials, createOptions.JobOptions);

//            // Create the job
//            Guid jobId = await CreateJob(createOptions, Options.JobName);

//            await DeleteWorkload(workload);

//            if (Options.StartJob)
//            {
//                await StartJob(jobId);

//                if (Options.FailoverJob)
//                {
//                    SimpleLog.Log("Waiting for the job to reach a state where it is ready to failover.");
//                    await WaitForJobStatus(jobId, s => s.CanFailover && s.Health == Health.Ok);

//                    RecommendedFailoverOptionsModel recommendedFailoverOptions = await GetFailoverOptions(jobId);

//                    SimpleLog.Log("Test Failover {0} supported.", recommendedFailoverOptions.IsTestFailoverSupported ? "is" : "is NOT");

//                    await FailoverJob(jobId, recommendedFailoverOptions.FailoverOptions);
//                }
//            }

//            if (!Options.DoNotDeleteJob)
//            {
//                await DeleteJob(jobId);
//            }
//        }
//    }
//}
