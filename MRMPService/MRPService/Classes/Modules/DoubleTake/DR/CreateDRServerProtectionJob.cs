using DoubleTake.Web.Models;
using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Types.API;
using MRMPService.DoubleTake;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using MRMPService.MRPService.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MRMPService.Tasks.DoubleTake
{
    partial class DisasterRecovery
    {
        public static void CreateDRServerProtectionJob(MRPTaskType payload)
        {
            using (MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                try
                {
                    MRPWorkloadType _source_workload = payload.submitpayload.source;
                    MRPWorkloadType _target_workload = payload.submitpayload.target;
                    MRPRecoverypolicyType _recovery_policy = payload.submitpayload.servicestack.recoverypolicy;
                    MRPServicestackType _service_stack = payload.submitpayload.servicestack;
                    MRPStacktreeType _stacktree = payload.submitpayload.stacktree;
                    using (Doubletake _dt = new Doubletake(_source_workload, _target_workload))
                    {
                        _mrp_api.task().progress(payload, "Verifying license status on both source and target workloads", 2);
                        if (!_dt.management().CheckLicense(DT_JobTypes.DR_Full_Protection))
                        {
                            _mrp_api.task().progress(payload, String.Format("Invalid license detected on workloads."), 3);
                            _mrp_api.task().progress(payload, String.Format("Attempting to configure license in deployment policy."), 4);
                            _dt.management().InstallLicense(_target_workload.deploymentpolicy.source_activation_code, _source_workload.deploymentpolicy.target_activation_code);
                            if (!_dt.management().CheckLicense(DT_JobTypes.DR_Full_Protection))
                            {
                                _mrp_api.task().progress(payload, String.Format("Invalid license detected on workloads after trying to fix license"), 5);
                            }
                        }
                        else
                        {
                            _mrp_api.task().progress(payload, String.Format("License valid on workloads."), 3);
                        }

                        List<JobInfoModel> _jobs = _dt.job().GetJobs().Result;

                        _mrp_api.task().progress(payload, "Looking for DR Server jobs on target workload", 11);
                        int _count = 1;
                        //This is a migration server migration job, so we need to remove all jobs from target server
                        foreach (JobInfoModel _delete_job in _jobs.Where(x => x.JobType == DT_JobTypes.DR_Full_Protection))
                        {
                            _mrp_api.task().progress(payload, String.Format("{0} - Deleting existing DR Server job between {1} and {2}", _count, _source_workload.hostname, _target_workload.hostname), _count + 15);
                            _dt.job().DeleteJob(_delete_job.Id).Wait();
                            _count += 1;
                        }

                        var workloadId = Guid.Empty;
                        WorkloadModel wkld = (WorkloadModel)null;

                        workloadId = (_dt.workload().CreateWorkload(DT_JobTypes.DR_Full_Protection).Result).Id;
                        wkld = _dt.workload().GetWorkload(workloadId).Result;


                        JobCredentialsModel jobCreds = _dt.job().CreateJobCredentials();

                        _mrp_api.task().progress(payload, "Fetching recommended job options", 20);

                        CreateOptionsModel jobInfo = _dt.job().GetJobOptions(
                            wkld,
                            jobCreds,
                            DT_JobTypes.DR_Full_Protection).Result;

                        _mrp_api.task().progress(payload, "Setting job options", 50);
                        jobInfo = SetOptions.set_job_options(payload, jobInfo);

                        _mrp_api.task().progress(payload, "Verifying job options and settings", 60);

                        JobOptionsModel _job_model = new JobOptionsModel();
                        var _fix_result = _dt.job().VerifyAndFixJobOptions(jobCreds, jobInfo.JobOptions, DT_JobTypes.DR_Full_Protection);
                        if (_fix_result.Item1)
                        {
                            _job_model = _fix_result.Item2;
                        }
                        else
                        {
                            int _fix_count = 0;
                            foreach (var _failed_item in _fix_result.Item3)
                            {
                                _mrp_api.task().progress(payload, String.Format("Error creating job: {0} : {1}", _failed_item.TitleKey, _failed_item.MessageKey), _fix_count + 61);

                                _fix_count++;
                            }
                            Logger.log(String.Format("Job Model Information {0}", JsonConvert.SerializeObject(_job_model)), Logger.Severity.Error);

                            throw new System.ArgumentException(string.Format("Cannot create job"));

                        }
                        _mrp_api.task().progress(payload, "Creating new job", 65);
                        Guid jobId = _dt.job().CreateJob((new CreateOptionsModel
                        {
                            JobOptions = _job_model,
                            JobCredentials = jobCreds,
                            JobType = DT_JobTypes.DR_Full_Protection
                        })).Result;

                        _mrp_api.task().progress(payload, String.Format("Job created. Starting job id {0}", jobId), 66);
                        _dt.job().StartJob(jobId).Wait();

                        _mrp_api.task().progress(payload, String.Format("Registering job {0} with portal", jobId), 67);
                        _mrp_api.job().createjob(new MRPJobType()
                        {
                            dt_job_id = jobId,
                            job_type = DT_JobTypes.DR_Full_Protection,
                            target_workload_id = _target_workload.id,
                            source_workload_id = _source_workload.id,
                            servicestack_id = _service_stack.id
                        });
                        _mrp_api.task().progress(payload, String.Format("Updating stacktree"), 70);
                        _mrp_api.stacktree().update(new MRPStacktreeType()
                        {
                            id = _stacktree.id,
                            dt_job_id = jobId.ToString()
                        });

                        _mrp_api.task().progress(payload, "Waiting for sync process to start", 71);

                        JobInfoModel jobinfo = _dt.job().GetJob(jobId).Result;
                        while (jobinfo.Statistics.CoreConnectionDetails.MirrorState != MirrorState.Mirror)
                        {
                            Thread.Sleep(new TimeSpan(0, 0, 30));
                            jobinfo = _dt.job().GetJob(jobId).Result;
                        }

                        _mrp_api.task().progress(payload, String.Format("Sync process started at {0}", jobinfo.Statistics.CoreConnectionDetails.StartTime), 75);

                        _mrp_api.task().progress(payload, String.Format("Successfully created disaster recover protection job between {0} to {1}", _source_workload.hostname, _target_workload.hostname), 95);
                        _mrp_api.task().successcomplete(payload, JsonConvert.SerializeObject(jobinfo));
                    }
                }
                catch (Exception e)
                {
                    Logger.log(String.Format("Error creating disaster recover protection job: {0}", e.ToString()), Logger.Severity.Error);
                    _mrp_api.task().failcomplete(payload, String.Format("Create disaster recover protection process failed: {0}", e.Message));
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
