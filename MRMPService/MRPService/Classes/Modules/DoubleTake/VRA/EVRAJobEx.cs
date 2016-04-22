using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTExamples
{
    //class EVRAJobEx : FullServerBase, IExample
    //{
    //    public class EVRAJobOptions : DefaultOptions
    //    {
    //        [Option("esxHost", Required = false, HelpText = "ESX host name or IP. If you are using vCenter, specify your vCenter. Only specify an ESX host if you are using ESX standalone.")]
    //        public string ESXHost { get; set; }

    //        [Option("esxUsername", Required = false, HelpText = "ESX user name.")]
    //        public string ESXUser { get; set; }

    //        [Option("esxPassword", Required = false, HelpText = "ESX password.")]
    //        public string ESXPswd { get; set; }

    //        [Option("displayName", Required = false, HelpText = "VM display name. This name must be unique within your environment and different from the existing directory location name if you are reusing an existing disk.", DefaultValue = "replicaVM")]
    //        public string ReplicaDisplayName { get; set; }

    //        [Option("datastoreLocation", Required = false, HelpText = "Datastore location - GUID assigned to the datastore. You can find this GUID in your vSphere or VMware web client.")]
    //        public string DatastoreLocation { get; set; }

    //        [Option("existingDisk", Required = false, HelpText = "Existing disk - Specify only if you want to reuse an existing disk.")]
    //        public string PreExistingDisk { get; set; }

    //        [Option("diskType", Required = false, HelpText = "Type of disk. Use Dynamic for ESX thin disks, Fixed for ESX thick disks and Flat Disk for ESX flat disks.", DefaultValue = "Dynamic")]
    //        public string DiskType { get; set; }
    //    }

    //    private EVRAJobOptions evraOptions = new EVRAJobOptions();

    //    public EVRAJobEx() : base("VRA") 
    //    {
    //        Options = evraOptions;
    //    }

    //    public async Task Execute(string[] args)
    //    {
    //        if (CommandLineHelper.Parser.ParseArguments(args, Options))
    //        {
    //            SimpleLog.Log("Creating EVRA job.");

    //            if (String.IsNullOrEmpty(evraOptions.ESXHost) || String.IsNullOrEmpty(evraOptions.ESXUser) || String.IsNullOrEmpty(evraOptions.ESXPswd))
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
    //                            Domain = Utility.GetDomain(evraOptions.ESXUser), 
    //                            Password = evraOptions.ESXPswd, 
    //                            UserName = Utility.GetLogin(evraOptions.ESXUser)
    //                        },
    //                        Host = evraOptions.ESXHost,
    //                        Port = 443 
    //                    }
    //                }
    //            };

    //            SimpleLog.Log("Job credentials model: {0}", jobCredentials);

    //            // Create the job options
    //            CreateOptionsModel createOptions = await GetJobOptions(workload, jobCredentials);

    //            //Update job options
    //            createOptions = ConfigureReplicaDiskParameters(createOptions);

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

    //    private CreateOptionsModel ConfigureReplicaDiskParameters(CreateOptionsModel createOptions)
    //    {
    //        SimpleLog.Log("Marking selected services to keep running during mirror.");

    //        if (String.IsNullOrEmpty(evraOptions.DatastoreLocation))
    //        {
    //            throw new System.MissingFieldException("Datastore location is not provided for the EVRA job. Exiting...");
    //        }

    //        createOptions.JobOptions.VraOptions.ReplicaVmInfo.Path = evraOptions.DatastoreLocation;

    //        if(String.IsNullOrEmpty(evraOptions.ReplicaDisplayName))
    //        {
    //            createOptions.JobOptions.VraOptions.ReplicaVmInfo.DisplayName = evraOptions.ReplicaDisplayName;
    //        }

    //        foreach(var volume in createOptions.JobOptions.VraOptions.Volumes)
    //        {
    //            volume.VirtualDiskPath = evraOptions.DatastoreLocation;
    //            volume.DiskProvisioningType = evraOptions.DiskType;
    //            if(String.IsNullOrEmpty(evraOptions.PreExistingDisk))
    //            {
    //                volume.PreexistingDiskPath = evraOptions.PreExistingDisk;
    //            }
    //        }

    //        SimpleLog.Log("Selected services configured.");

    //        return createOptions;
    //    }
    //}
}
