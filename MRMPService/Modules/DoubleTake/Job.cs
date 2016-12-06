using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MRMPService.MRMPDoubleTake
{
    class Job : Core
    {
        public Job(Doubletake doubletake) : base(doubletake)
        {
            jobApi = new JobsApi(_target_connection);
        }

        public JobCredentialsModel CreateJobCredentials()
        {
            JobCredentialsModel jobCredentials = new JobCredentialsModel()
            {
                SourceServer = new ServiceConnectionModel()
                {
                    Credential = new CredentialModel() { Domain = _source_credentials.domain, Password = _source_credentials.encrypted_password, UserName = _source_credentials.username },
                    Host = _source_address
                },

                TargetServer = new ServiceConnectionModel()
                {
                    Credential = new CredentialModel() { Domain = _target_credentials.domain, Password = _target_credentials.encrypted_password, UserName = _target_credentials.username },
                    Host = _target_address
                }
            };
            return jobCredentials;
        }

        async public Task<CreateOptionsModel> GetJobOptions(WorkloadModel workload, JobCredentialsModel jobCredentials, String _job_type)
        {
            ApiResponse<RecommendedJobOptionsModel> recommendedOptions = await jobApi.RecommendJobOptionsAsync(_job_type, workload, jobCredentials);
            recommendedOptions.EnsureSuccessStatusCode();

            CreateOptionsModel createOptions = new CreateOptionsModel()
            {
                JobCredentials = jobCredentials,
                JobOptions = recommendedOptions.Content.JobOptions,
                JobType = _job_type
            };
            return createOptions;
        }
        public List<SnapshotEntryModel> GetSnapShots(Guid _job_id)
        {
            List<SnapshotEntryModel> _snapshots = new List<SnapshotEntryModel>();
            var _connections = jobApi.GetConnectionsAsync(_job_id).Result.Content;
            foreach (var _connection in _connections)
            {
                var _connection_snapshots = jobApi.GetSnapshotsAsync(_job_id, _connection.ManagedConnectionId);
                _snapshots.AddRange(_connection_snapshots.Result.Content);
            }
            return _snapshots;
        }
        async public Task<Guid> CreateJob(CreateOptionsModel createOptions, string jobName)
        {
            if (!string.IsNullOrEmpty(jobName))
            {
                createOptions.JobOptions.Name = jobName;
            }

            ApiResponse<Guid> data = await jobApi.CreateJobAsync(createOptions);
            data.EnsureSuccessStatusCode();
            return data.Content;
        }

        // TODO: Verification steps currently contain IDs meant for translation. As things are today, this data is difficult to 
        // present in a meaningful way.  Until this is resolved, just return the jobOptions
        public Tuple<bool, JobOptionsModel, List<VerificationStepModel>> VerifyAndFixJobOptions(JobCredentialsModel jobCredentials, JobOptionsModel jobOptions, String _job_type)
        {
            bool _job_errorfree = false;
            List<VerificationStepModel> _errors = null;
            while (true)
            {
                var result = jobApi.VerifyJobOptionsAsync(_job_type, jobCredentials, jobOptions, new Progress<VerificationStatusModel>()).Result;
                result.EnsureSuccessStatusCode();
                var stepsToFix = result.Content.Steps.ToList();
                if (stepsToFix.Any(s => s.Status == VerificationStatus.Error && s.CanFix == false))
                {
                    _errors = result.Content.Steps.Where(s => s.Status == VerificationStatus.Error && s.CanFix == false).ToList();
                    _job_errorfree = false;
                    break;
                }
                else if (stepsToFix.Any(s => s.Status == VerificationStatus.Error && s.CanFix == true))
                {
                    var fixResponse = jobApi.FixRecommendedJobOptionsAsync(_job_type, jobCredentials, jobOptions, stepsToFix.Where(s => s.Status == VerificationStatus.Error && s.CanFix == true)).Result;
                    fixResponse.EnsureSuccessStatusCode();

                    jobOptions = fixResponse.Content.JobOptions;
                    _job_errorfree = true;
                    break;
                }
                else
                {
                    _job_errorfree = true;
                    break;
                }



            }

            return new Tuple<bool, JobOptionsModel, List<VerificationStepModel>>(_job_errorfree, jobOptions, _errors);
        }

        async public Task<List<JobInfoModel>> GetJobs()
        {
            var result = await jobApi.GetJobsAsync();
            result.EnsureSuccessStatusCode();
            return result.Content.ToList();

        }
        async public Task<JobInfoModel> GetJob(Guid jobId)
        {
            var result = await jobApi.GetJobAsync(jobId);
            result.EnsureSuccessStatusCode();
            return result.Content;
        }

        async public Task StartJob(Guid jobId)
        {
            var result = await jobApi.StartJobAsync(jobId);
            result.EnsureSuccessStatusCode();
        }
        async public Task StopJob(Guid jobId)
        {
            var result = await jobApi.StopJobAsync(jobId);
            result.EnsureSuccessStatusCode();
        }
        async public Task PauseJob(Guid jobId)
        {
            var result = await jobApi.PauseJobAsync(jobId);
            result.EnsureSuccessStatusCode();
        }

        async public Task DeleteJob(Guid jobId)
        {
            var result = await jobApi.DeleteJobAsync(jobId, true, true, true, new Guid(), VhdDeleteActionType.DeleteAll );
            result.EnsureSuccessStatusCode();
        }
        async public Task DeleteSnapshot(Guid jobId, Guid snaphotId, Guid connectionId)
        {
            var result = await jobApi.DeleteSnapshotAsync(jobId, connectionId, snaphotId);
            result.EnsureSuccessStatusCode();
        }
        async public Task DeleteJob_DeleteFiles(Guid jobId)
        {
            var result = await jobApi.DeleteJobAsync(jobId, true, true, true, new Guid(), VhdDeleteActionType.DeleteAll);
            result.EnsureSuccessStatusCode();
        }

        public ActivityStatusModel FailoverJob(Guid jobId, FailoverOptionsModel options)
        {
            var result = jobApi.FailoverJobAsync(jobId, options, new Progress<ActivityStatusModel>()).Result;
            result.EnsureSuccessStatusCode();
            return result.Content;
        }

        async public Task ReverseJob(Guid jobId)
        {
            var result = await jobApi.ReverseJobAsync(jobId, new Progress<ActivityStatusModel>());
            result.EnsureSuccessStatusCode();
        }
        async public Task<RecommendedFailoverOptionsModel> GetFailoverOptions(Guid jobId)
        {
            ApiResponse<RecommendedFailoverOptionsModel> recommendedOptions = await jobApi.RecommendFailoverOptionsAsync(jobId);
            recommendedOptions.EnsureSuccessStatusCode();
            return recommendedOptions.Content;
        }

        async public Task<RecommendedFailbackOptionsModel> GetFailbackOptions(Guid jobId)
        {
            ApiResponse<RecommendedFailbackOptionsModel> recommendedOptions = await jobApi.RecommendFailbackOptionsAsync(jobId);
            recommendedOptions.EnsureSuccessStatusCode();
            return recommendedOptions.Content;
        }

        async public Task<Guid> CreateJob(CreateOptionsModel createOptions)
        {
            ApiResponse<Guid> data = await jobApi.CreateJobAsync(createOptions);
            data.EnsureSuccessStatusCode();
            return data.Content;
        }

        async public Task WaitForJobStatus(Guid jobId, Func<JobStatusModel, bool> actionStatus)
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));

                ApiResponse<JobInfoModel> jobResponse;

                try
                {
                    jobResponse = await jobApi.GetJobAsync(jobId, fields: "status");

                    if (!jobResponse.IsSuccessStatusCode)
                    {
                        continue;
                    }
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerException is HttpRequestException)
                    {
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (HttpRequestException)
                {
                    continue;
                }

                var status = jobResponse.Content.Status;

                if (actionStatus(status) == true)
                {
                    break;
                }
            }
        }
    }
}
