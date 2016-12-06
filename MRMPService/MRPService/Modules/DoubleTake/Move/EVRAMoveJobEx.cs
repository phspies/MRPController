using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MRMPService.Modules.DoubleTake
{
    //class EVRAMoveJobEx : FullServerBase, IExample
    //{
    //    public class EVRAMoveJobOptions : DefaultOptions
    //    {
    //        [Option("esxHost", Required = false, HelpText = "ESX host name or IP. If you are using vCenter, specify your vCenter. Only specify an ESX host if you are using ESX standalone.")]
    //        public string ESXHost { get; set; }

    //        [Option("esxUsername", Required = false, HelpText = "ESX user name.")]
    //        public string ESXUser { get; set; }

    //        [Option("esxPassword", Required = false, HelpText = "ESX password.")]
    //        public string ESXPswd { get; set; }
    //    }

    //    private EVRAMoveJobOptions evraMoveOptions = new EVRAMoveJobOptions();

    //    public EVRAMoveJobEx()
    //        : base("VraMove") 
    //    {
    //        Options = evraMoveOptions;
    //    }

    //    public async Task Execute(string[] args)
    //    {
    //        if (CommandLineHelper.Parser.ParseArguments(args, Options))
    //        {
    //            SimpleLog.Log("Creating EVRA job.");

    //            if (String.IsNullOrEmpty(evraMoveOptions.ESXHost) || String.IsNullOrEmpty(evraMoveOptions.ESXUser) || String.IsNullOrEmpty(evraMoveOptions.ESXPswd))
    //            {
    //                throw new System.MissingFieldException("ESX host name/IP or credentials information missing. Exiting...");
    //            }

    //            // note: The job must be created on the TARGET machine
    //            var connection = await ManagementService.GetConnectionAsync(Options.Target);
    //            jobApi = new JobsApi(connection);

    //            // Create the workload. Workloads are created on the SOURCE
    //            WorkloadModel workload = await CreateWorkload();

    //            // Create the job's credentials
    //            JobCredentialsModel jobCredentials = CreateJobCredentials();

    //            jobCredentials.OtherServers = new Dictionary<string, ServiceConnectionModel>()
    //            {
    //                {"TargetVimServer", new ServiceConnectionModel()
    //                    {
    //                        Credential = new CredentialModel() 
    //                        { 
    //                            Domain = Utility.GetDomain(evraMoveOptions.ESXUser), 
    //                            Password = evraMoveOptions.ESXPswd, 
    //                            UserName = Utility.GetLogin(evraMoveOptions.ESXUser)
    //                        },
    //                        Host = evraMoveOptions.ESXHost,
    //                        Port = 443 
    //                    }
    //                }
    //            };

    //            SimpleLog.Log("Job credentials model: {0}", jobCredentials);

    //            // Create the job options
    //            CreateOptionsModel createOptions = await GetJobOptions(workload, jobCredentials);

    //            // Verify the options are good and update the CreateOptions with the possibly fixed values
    //            createOptions.JobOptions = await VerifyAndFixJobOptions(jobCredentials, createOptions.JobOptions);

    //            // Create the job
    //            Guid jobId = await CreateJob(createOptions, Options.JobName);

    //            // Delete the no-longer needed workload
    //            // note: workloads can be re-used for jobs created using the same source
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
