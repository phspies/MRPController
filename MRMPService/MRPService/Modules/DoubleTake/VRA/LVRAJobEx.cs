using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.Modules.DoubleTake
{
    //class LvraJobEx : FullServerBase, IExample
    //{            
    //    public class LvraJobOptions : DefaultOptions
    //    {
    //        [Option("esxHost", Required = false, HelpText = "ESX host name or IP. If you are using vCenter, specify your vCenter. Only specify an ESX host if you are using ESX standalone.")]
    //        public string ESXHost { get; set; }

    //        [Option("esxUsername", Required = false, HelpText = "ESX user name.")]
    //        public string ESXUser { get; set; }

    //        [Option("esxPassword", Required = false, HelpText = "ESX password.")]
    //        public string ESXPswd { get; set; }

    //    }

    //    private LvraJobOptions lvraOptions = new LvraJobOptions();

    //    public LvraJobEx()
    //        : base("Lvra")
    //    {
    //        Options = lvraOptions;
    //    }

    //    public async Task Execute(string[] args)
    //    {
    //        if (CommandLineHelper.Parser.ParseArguments(args, Options))
    //        {
    //            SimpleLog.Log("Creating LVRA job.");

    //            // note: The job must be created on the TARGET machine
    //            var connection = await ManagementService.GetConnectionAsync(Options.Target);
    //            jobApi = new JobsApi(connection);

    //            WorkloadModel workload = await CreateWorkload();

    //            JobCredentialsModel jobCredentials = CreateJobCredentials();
    //            jobCredentials.OtherServers = new Dictionary<String, ServiceConnectionModel>()
    //            {
    //                { "VC", new ServiceConnectionModel
    //                    {
    //                       Credential = new CredentialModel
    //                       {
    //                           Domain = Utility.GetDomain(lvraOptions.ESXUser),
    //                           Password = lvraOptions.ESXPswd,
    //                           UserName = Utility.GetLogin(lvraOptions.ESXUser)
    //                       },
    //                       Host = lvraOptions.ESXHost,
    //                       Port = 443
    //                    }
    //               }
    //            };


    //            CreateOptionsModel createOptions = await GetJobOptions(workload, jobCredentials);

    //            Guid jobId = await CreateJob(createOptions);

    //            await DeleteWorkload(workload);

    //            if(lvraOptions.StartJob)
    //            {
    //                await StartJob(jobId);

    //                if (Options.FailoverJob)
    //                {
    //                    SimpleLog.Log("Waiting for the job to reach a state where it is ready to failover.");
    //                    await WaitForJobStatus(jobId, s => s.CanFailover && s.Health == Health.Ok);

    //                    RecommendedFailoverOptionsModel recommendedFailoverOptions = await GetFailoverOptions(jobId);

    //                    SimpleLog.Log("Test Failover {0} supported.", recommendedFailoverOptions.IsTestFailoverSupported ? "is" : "is NOT");

    //                    await FailoverJob(jobId, recommendedFailoverOptions.FailoverOptions);

    //                    SimpleLog.Log("Waiting for failover to complete.");
    //                    await WaitForJobStatus(jobId, s => s.CanReverse);

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
