using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTExamples
{
    //public class LinuxFullServerFailoverJobEx : FullServerBase, IExample
    //{
    //    public class LinuxFullServerJobOptions : DefaultOptions
    //    {
    //        [Option(Required = false, HelpText = "The reserved IP address for the source server.")]
    //        public string SourceReservedIp { get; set; }

    //        [Option(Required = false, HelpText = "The reserved IP address for the target server.")]
    //        public string TargetReservedIp { get; set; }

    //    }

    //    private LinuxFullServerJobOptions lfsjOptions = new LinuxFullServerJobOptions();

    //    public LinuxFullServerFailoverJobEx()
    //        : base("LinuxFullServerFailover")
    //    {
    //        Options = lfsjOptions;
    //    }

    //    public async Task Execute(string[] args)
    //    {
    //        if (CommandLineHelper.Parser.ParseArguments(args, Options))
    //        {
    //            SimpleLog.Log("Creating Linux Full Server Failover job.");

    //            // !!! IMPORTANT !!!
    //            // For Linux Full Server Failover, when the job fails over, the source and target swap identities, and the
    //            // original source goes offline. Meaning that after failover, the 'jobApi' needs to know which machine 
    //            // to connect to.  This is most easily solved by using the targets reserved IP (which remains after failover)
    //            // if no reserved IP is given, then later we will have to re-create the jobApi
    //            string targetId = !String.IsNullOrEmpty(lfsjOptions.TargetReservedIp)
    //                ? lfsjOptions.TargetReservedIp
    //                : Options.Target;

    //            var targetConnection = await ManagementService.GetConnectionAsync(targetId);
    //            jobApi = new JobsApi(targetConnection);

    //            WorkloadModel workload = await CreateWorkload();

    //            JobCredentialsModel jobCredentials = CreateJobCredentials();

    //            // Create the job options
    //            CreateOptionsModel createOptions = await GetJobOptions(workload, jobCredentials);

    //            // Make any desired changes to the job options
    //            {
    //                SetupBackupConnection(createOptions, lfsjOptions.SourceReservedIp, lfsjOptions.TargetReservedIp);
    //            }

    //            // Verify the options are good and update the CreateOptions with the possibly fixed values
    //            createOptions.JobOptions = await VerifyAndFixJobOptions(jobCredentials, createOptions.JobOptions);

    //            // Create the job
    //            Guid jobId = await CreateJob(createOptions, lfsjOptions.JobName);

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

    //                    // !!! IMPORTANT !!!
    //                    // SEE note above about source/target identity
    //                    if (String.IsNullOrEmpty(lfsjOptions.TargetReservedIp))
    //                    {
    //                        // no reserve IP was given for the original target, so we have to re-create the
    //                        // jobApi but now talking to the 'source'.
    //                        var sourceConnection = await ManagementService.GetConnectionAsync(Options.Source);
    //                        jobApi = new JobsApi(sourceConnection);
    //                    }

    //                    SimpleLog.Log("Waiting for failover to complete.");
    //                    await WaitForJobStatus(jobId, s => s.CanReverse);

    //                    if (!Options.DoNotReverseJob)
    //                    {
    //                        if (string.IsNullOrEmpty(lfsjOptions.TargetReservedIp))
    //                        {
    //                            SimpleLog.Log("Reverse requested, but no reserved IPs have been configured.  Reverse is impossible without reserve ip addresses on both source and target.");
    //                        }
    //                        else
    //                        {
    //                            ConsoleColor originalColor = Console.ForegroundColor;
    //                            try
    //                            {
    //                                Console.WriteLine(Environment.NewLine);
    //                                Console.ForegroundColor = ConsoleColor.Red;
    //                                Console.WriteLine("Press any key after you turn the old source machine back on.");
    //                                Console.ReadKey();
    //                            }
    //                            finally
    //                            {
    //                                Console.ForegroundColor = originalColor;
    //                            }

    //                            await ReverseJob(jobId);

    //                            SimpleLog.Log("Waiting for job to re-enter healthy state; signifying reverse is complete.");
    //                            await WaitForJobStatus(jobId, s => s.Health == Health.Ok);
    //                        }
    //                    }
    //                }
    //            }

    //            if (!Options.DoNotDeleteJob)
    //            {
    //                await DeleteJob(jobId);
    //            }
    //        }
    //    }
    //}
}
