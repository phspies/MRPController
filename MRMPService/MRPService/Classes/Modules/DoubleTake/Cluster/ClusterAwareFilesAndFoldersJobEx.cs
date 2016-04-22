using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTExamples
{
    //class ClusterAwareFilesAndFoldersJobEx : JobBase, IExample
    //{
    //    public class ClusterFNFJobOptions : DefaultOptions
    //    {
    //        [OptionList("targetAddresses", Required = false, HelpText = "Provide target IPs to update the source DNS entries with the corresponding target IP addresses.", Separator = ',')]
    //        public IList<string> TargetAddresses { get; set; }

    //        [Option("workloadItem", Required = false, HelpText = "Provide workload item type: \"Roles/Groups\" (DR only, failover not supported) or \"File Servers\" (Failover supported). Default workload item type is \"File Servers\".", DefaultValue ="File Servers")]
    //        public string WorkloadItem { get; set; }

    //        [OptionList("protectionPath", Required = false, HelpText = "File Servers or Roles/Group to be protected.", Separator = ',')]
    //        public IList<string> ProtectionPath { get; set; }

    //    }

    //    private ClusterFNFJobOptions clusterOptions = new ClusterFNFJobOptions();

    //    public ClusterAwareFilesAndFoldersJobEx() : base("ClusterAwareFilesAndFolders")
    //    {
    //        Options = clusterOptions;
    //    }

    //    public async Task Execute(string[] args)
    //    {
    //        if (CommandLineHelper.Parser.ParseArguments(args, Options))
    //        {
    //            SimpleLog.Log("Creating ClusterAwareFilesAndFolders job.");

    //            if (clusterOptions.TargetAddresses.Count == 0)
    //            {
    //                throw new System.MissingFieldException("Target IPs to update the source DNS entries with the corresponding target IP addresses missing. Exiting...");
    //            }

    //            // note: The job must be created on the TARGET machine
    //            var connection = await ManagementService.GetConnectionAsync(Options.Target);
    //            jobApi = new JobsApi(connection);

    //            // Create the workload. Workloads are created on the SOURCE
    //            WorkloadModel workload = await CreateWorkload();


    //            // Create the job's credentials
    //            JobCredentialsModel jobCredentials = CreateJobCredentials();

    //            // Create the job options
    //            CreateOptionsModel createOptions = await GetJobOptions(workload, jobCredentials);

    //            if(clusterOptions.WorkloadItem == "File Servers")
    //            {
    //                createOptions = UpdateDnsOptions(createOptions);
    //            }

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

    //                if (Options.FailoverJob)
    //                {
    //                    var recommendedFailoverOptions = await GetFailoverOptions(jobId);

    //                    await jobApi.FailoverJobAsync(jobId, recommendedFailoverOptions.FailoverOptions);
    //                }
    //            }

    //            if (!Options.DoNotDeleteJob)
    //            {
    //                await DeleteJob(jobId);
    //            }
    //        }

    //    }

    //    private CreateOptionsModel UpdateDnsOptions(CreateOptionsModel createOptions)
    //    {
    //        int i = 0;
    //        foreach (String targetAddress in clusterOptions.TargetAddresses)
    //        {
    //            createOptions.JobOptions.DnsOptions.Domains[0].AddressMappings[i].TargetIP = targetAddress;
    //            i++;
    //        }

    //        createOptions.JobOptions.DnsOptions.Domains[0].Credentials.Domain = Options.UserName.Substring(0, Options.UserName.LastIndexOf('\\'));
    //        createOptions.JobOptions.DnsOptions.Domains[0].Credentials.Password = Options.Password;
    //        createOptions.JobOptions.DnsOptions.Domains[0].Credentials.UserName = Options.UserName.Substring(Options.UserName.LastIndexOf('\\') + 1);

    //        return createOptions;
    //    }

    //    protected override async Task<WorkloadModel> CreateWorkload()
    //    {
    //        WorkloadModel workload = await base.CreateWorkload();

    //        ApiResponse<IEnumerable<LogicalItemModel>> workloadResponse = await workloadApi.GetLogicalItemsAsync(workload.Id);
    //        LogicalItemModel item = null;

    //        item = workloadResponse.Content.First(c => c.ItemType.Contains(clusterOptions.WorkloadItem));

    //        string path;
    //        foreach (var protectionPath in clusterOptions.ProtectionPath)
    //        {
    //            path = item.Path + protectionPath;
    //            //Provide the file servers to be protected. 
    //            await workloadApi.SelectLogicalItemAsync(workload.Id, path);
    //        }

    //        SimpleLog.Log("Selecting repset(s).");

    //       SimpleLog.Log("Workload updated and sent to server.");
    //        return await GetWorkload(workload.Id);
    //    }
    //}
}
