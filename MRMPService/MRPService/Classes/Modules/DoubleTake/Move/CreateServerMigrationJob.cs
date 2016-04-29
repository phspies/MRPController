using DoubleTake.Web.Models;
using MRMPService.API;
using MRMPService.API.Types.API;
using MRMPService.DoubleTake;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MRMPService.Tasks.DoubleTake
{
    partial class Migration
    {
        public static void CreateServerMigrationJob(MRPTaskType payload)
        {
            using (MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
            {
                try
                {
                    MRPTaskWorkloadType _source_workload = payload.submitpayload.source;
                    MRPTaskWorkloadType _target_workload = payload.submitpayload.target;
                    MRPTaskRecoverypolicyType _recovery_policy = payload.submitpayload.servicestack.recoverypolicy;
                    MRPTaskServicestackType _service_stack = payload.submitpayload.servicestack;
                    using (Doubletake _dt = new Doubletake(_source_workload.id, _target_workload.id))
                    {
                        _mrp_api.task().progress(payload, "Verifying license status on both source and target workloads", 2);
                        if (!_dt.management().CheckLicense())
                        {
                            _mrp_api.task().progress(payload, String.Format("Invalid license detected on workloads."), 3);
                            _mrp_api.task().progress(payload, String.Format("Attempting to configure license in deployment policy."), 4);
                            _dt.management().InstallLicense(_target_workload.deploymentpolicy.source_activation_code, _source_workload.deploymentpolicy.target_activation_code);
                            if (!_dt.management().CheckLicense())
                            {
                                _mrp_api.task().progress(payload, String.Format("Invalid license detected on workloads after trying to fix license"), 5);
                            }
                        }
                        else
                        {
                            _mrp_api.task().progress(payload, String.Format("License valid on workloads."), 3);
                        }

                        List<JobInfoModel> _jobs = _dt.job().GetJobs().Result;
                        String[] _source_ips = _source_workload.iplist.Split(',');
                        String[] _target_ips = _target_workload.iplist.Split(',');

                        _mrp_api.task().progress(payload, "Deleting current jobs associated to the source and target workloads", 11);
                        int _count = 1;
                        foreach (JobInfoModel _delete_job in _jobs.Where(x => _source_ips.Any(x.SourceServer.Host.Contains) && _target_ips.Any(x.TargetServer.Host.Contains)))
                        {
                            _mrp_api.task().progress(payload, String.Format("Deleting existing Move jobs between {0} and {1}", _source_ips[0], _target_ips[0]), _count + 15);
                            _dt.job().DeleteJob(_delete_job.Id).Wait();
                            _count += 1;
                        }

                        var workloadId = Guid.Empty;
                        WorkloadModel wkld = (WorkloadModel)null;

                        workloadId = (_dt.workload().CreateWorkload(DT_JobTypes.Move_Server_Migration).Result).Id;
                        wkld = _dt.workload().GetWorkload(workloadId).Result;


                        JobCredentialsModel jobCreds = _dt.job().CreateJobCredentials();

                        _mrp_api.task().progress(payload, "Fetching recommended job options", 20);

                        CreateOptionsModel jobInfo = _dt.job().GetJobOptions(
                            wkld,
                            jobCreds,
                            DT_JobTypes.Move_Server_Migration).Result;

                        _mrp_api.task().progress(payload, "Setting job options", 50);
                        jobInfo = SetOptions.set_job_options(payload, jobInfo);

                        _mrp_api.task().progress(payload, "Verifying job options and settings", 55);

                        JobOptionsModel _job_model;
                        var _fix_result = _dt.job().VerifyAndFixJobOptions(jobCreds, jobInfo.JobOptions, DT_JobTypes.HA_Full_Failover);
                        if (_fix_result.Item1)
                        {
                            _job_model = _fix_result.Item2;
                        }
                        else
                        {
                            int _fix_count = 0;
                            foreach (var _failed_item in _fix_result.Item3)
                            {
                                _mrp_api.task().progress(payload, String.Format("Error creating job: {0}", _failed_item.TitleKey), _fix_count + 55);
                                _fix_count++;
                            }
                            throw new System.ArgumentException(string.Format("Cannot create job"));

                        }
                        _mrp_api.task().progress(payload, "Creating new job", 56);
                        Guid jobId = _dt.job().CreateJob((new CreateOptionsModel
                        {
                            JobOptions = jobInfo.JobOptions,
                            JobCredentials = jobCreds,
                            JobType = DT_JobTypes.Move_Server_Migration
                        })).Result;

                        _mrp_api.task().progress(payload, String.Format("Job created. Starting job id {0}", jobId), 57);
                        _dt.job().StartJob(jobId).Wait();

                        _mrp_api.task().progress(payload, String.Format("Registering job {0} with portal", jobId), 60);
                        _mrp_api.job().createjob(new MRPJobType()
                        {
                            dt_job_id = jobId.ToString(),
                            job_type = DT_JobTypes.Move_Server_Migration,
                            target_workload_id = _target_workload.id,
                            source_workload_id = _source_workload.id,
                            servicestack_id = _service_stack.id
                        });

                        _mrp_api.task().progress(payload, "Waiting for sync process to start", 65);

                        JobInfoModel jobinfo = _dt.job().GetJob(jobId).Result;
                        while (jobinfo.Statistics.CoreConnectionDetails.TargetState == TargetStates.Unknown)
                        {
                            Thread.Sleep(5000);
                            jobinfo = _dt.job().GetJob(jobId).Result;
                        }
                        Thread.Sleep(5000);
                        jobinfo = _dt.job().GetJob(jobId).Result;

                        _mrp_api.task().progress(payload, "Sync process started", 70);
                        while (jobinfo.Statistics.CoreConnectionDetails.MirrorState != MirrorState.Idle)
                        {
                            if (jobinfo.Statistics.CoreConnectionDetails.MirrorBytesRemaining != 0)
                            {
                                double totalremaining = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesRemaining) / 1024 / 1024 / 1024;
                                double totalstorage = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent + (double)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSkipped + (double)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesRemaining) / 1024 / 1024 / 1024;
                                double totalcomplete = totalstorage - totalremaining;
                                if ((totalremaining > 0) && (totalremaining > 0))
                                {
                                    double percentage = (((double)totalcomplete / (double)totalstorage) * 20);
                                    String progress = String.Format("{0}GB of {1}GB seeded", Math.Round(totalcomplete, 2), Math.Round(totalstorage, 2));
                                    _mrp_api.task().progress(payload, progress, percentage + 72);
                                }
                            }

                        }
                        _mrp_api.task().progress(payload, String.Format("Successfully synchronized {0} to {1}", _source_workload.hostname, _target_workload.hostname), 95);
                        _mrp_api.task().successcomplete(payload, JsonConvert.SerializeObject(jobinfo));
                    }
                }
                catch (Exception e)
                {
                    Logger.log(String.Format("Error creating availbility sync job: {0}", e.ToString()), Logger.Severity.Error);
                    _mrp_api.task().failcomplete(payload, String.Format("Create sync process failed: {0}", e.Message));
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
