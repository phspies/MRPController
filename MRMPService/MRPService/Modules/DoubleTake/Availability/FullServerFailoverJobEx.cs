namespace MRMPService.Modules.DoubleTake.Availability
{
    //public class FullServerFailoverJobEx : FullServerBase, IExample
    //{
    //    public class FullServerJobOptions : DefaultOptions
    //    {
    //        [Option(Required = false, HelpText = "The reserved IP address for the source server.")]
    //        public string SourceReservedIp { get; set; }

    //        [Option(Required = false, HelpText = "The reserved IP address for the target server.")]
    //        public string TargetReservedIp { get; set; }

    //        [OptionList(Required = false, HelpText = "The comma separated list of services to keep running while mirroring.", Separator = ',')]
    //        public IList<string> CriticalServices { get; set; }
    //    }

    //    private FullServerJobOptions fsjOptions = new FullServerJobOptions();

    //    public FullServerFailoverJobEx()
    //        : base("FullServerFailover")
    //    {
    //        Options = fsjOptions;
    //    }

    //    private void ConfigureServicesToKeepRunningDuringMirror(CreateOptionsModel createOptions)
    //    {
    //        SimpleLog.Log("Marking selected services to keep running during mirror.");

    //        if (fsjOptions.CriticalServices != null && fsjOptions.CriticalServices.Count > 0)
    //        {
    //            var selected = createOptions.JobOptions.SystemStateOptions.ServicesToStopOptions
    //               .Where(s => fsjOptions.CriticalServices.Contains(s.ServiceName, StringComparer.OrdinalIgnoreCase));

    //            foreach (var service in selected)
    //            {
    //                SimpleLog.Log("Marking service '{0}' to keep it running during mirror.", service.ServiceName);

    //                service.KeepRunningNonCritical = true;
    //            }
    //        }

    //        SimpleLog.Log("Selected services configured.");
    //    }

    //    public async Task Execute(string[] args)
    //    {
    //        if (CommandLineHelper.Parser.ParseArguments(args, Options))
    //        {
    //            SimpleLog.Log("Creating Full Server Failover job.");

    //            // !!! IMPORTANT !!!
    //            // For Full Server Failover, when the job fails over, the source and target swap identities, and the
    //            // original source goes offline. Meaning that after failover, the 'jobApi' needs to know which machine 
    //            // to connect to.  This is most easily solved by using the targets reserved IP (which remains after failover)
    //            // if no reserved IP is given, then later we will have to re-create the jobApi
    //            string targetId = !String.IsNullOrEmpty(fsjOptions.TargetReservedIp)
    //                ? fsjOptions.TargetReservedIp
    //                : Options.Target;

    //            var targetConnection = await ManagementService.GetConnectionAsync(targetId);
    //            jobApi = new JobsApi(targetConnection);

    //            WorkloadModel workload = await CreateWorkload();

    //            JobCredentialsModel jobCredentials = CreateJobCredentials();

    //            // Create the job options
    //            CreateOptionsModel createOptions = await GetJobOptions(workload, jobCredentials);

    //            // Make any desired changes to the job options
    //            {
    //                SetupBackupConnection(createOptions, fsjOptions.SourceReservedIp, fsjOptions.TargetReservedIp);

    //                ConfigureServicesToKeepRunningDuringMirror(createOptions);
    //            }

    //            // Verify the options are good and update the CreateOptions with the possibly fixed values
    //            createOptions.JobOptions = await VerifyAndFixJobOptions(jobCredentials, createOptions.JobOptions);

    //            // Create the job
    //            Guid jobId = await CreateJob(createOptions, fsjOptions.JobName);

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
    //                    if (String.IsNullOrEmpty(fsjOptions.TargetReservedIp))
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
    //                        if (string.IsNullOrEmpty(fsjOptions.TargetReservedIp))
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
