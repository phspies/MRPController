using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MRMPService.DoubleTake
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
                    Credential = new CredentialModel() { Domain = _source_credentials.domain, Password = _source_credentials.password, UserName = _source_credentials.username },
                    Host = _source_address
                },

                TargetServer = new ServiceConnectionModel()
                {
                    Credential = new CredentialModel() { Domain = _target_credentials.domain, Password = _target_credentials.password, UserName = _target_credentials.username },
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
                var stepsToFix = result.Content.Steps.Where(s => s.Status == VerificationStatus.Error).ToList();
                if (stepsToFix.Any())
                {
                    var fixResponse = jobApi.FixRecommendedJobOptionsAsync(_job_type, jobCredentials, jobOptions, stepsToFix).Result;
                    fixResponse.EnsureSuccessStatusCode();
                    var _fix_result = jobApi.VerifyJobOptionsAsync(_job_type, jobCredentials, fixResponse.Content.JobOptions, new Progress<VerificationStatusModel>()).Result;
                    _fix_result.EnsureSuccessStatusCode();

                    if (_fix_result.Content.Steps.Any(s => s.Status == VerificationStatus.Error))
                    {
                        _errors = result.Content.Steps.Where(s => s.Status == VerificationStatus.Error).ToList();
                        break;
                    }
   
                    jobOptions = fixResponse.Content.JobOptions;
                    _job_errorfree = true;
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

        async public Task DeleteJob(Guid jobId)
        {
            var result = await jobApi.DeleteJobAsync(jobId);
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
