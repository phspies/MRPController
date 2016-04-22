using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTExamples
{
    //class FullServerImageRecoveryJobEx : DRRecoveryBase, IExample
    //{

    //    public FullServerImageRecoveryJobEx()
    //        : base("FullServerImageRecovery")
    //    { }

    //    public async Task Execute(string[] args)
    //    {
    //        if (CommandLineHelper.Parser.ParseArguments(args, Options))
    //        {
    //            SimpleLog.Log("Creating DR Full Server Image Recovery Job job.");

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
    //            }

    //            if (!Options.DoNotDeleteJob)
    //            {
    //                await DeleteJob(jobId);
    //            }
    //        }
    //    }

    //}
}
