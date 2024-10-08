﻿using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.Modules.DoubleTake.DR
{
    //class DataOnlyImageProtectionJobEx : JobBase, IExample
    //{
    //    public DataOnlyImageProtectionJobEx() : base("DataOnlyImageProtection") { }

    //    public async Task Execute(string[] args)
    //    {
    //        if (CommandLineHelper.Parser.ParseArguments(args, Options))
    //        {
    //            SimpleLog.Log("Creating DR Data Only Image Protection Job.");

    //            // note: The job must be created on the TARGET machine
    //            var connection = await ManagementService.GetConnectionAsync(Options.Target);
    //            jobApi = new JobsApi(connection);

    //            // Create the workload. Workloads are created on the SOURCE
    //            WorkloadModel workload = await CreateWorkload();

    //            // Add the physical rules (rulesets) to the workload
    //            // note: the local workloadModel must be updated after adding the rules.
    //            // AddPhysicalRules returns the updated workloadModel
    //            workload = await AddPhysicalRules(workload, new PhysicalRuleModel[] 
    //            { 
    //                new PhysicalRuleModel() { Path = @"c:\", Recursion = RecursionMode.Recursive, Inclusion = InclusionMode.Exclude },
    //                new PhysicalRuleModel() { Path = @"c:\Perflogs", Recursion = RecursionMode.Recursive, Inclusion = InclusionMode.Include }
    //            });

    //            // Create the job's credentials
    //            JobCredentialsModel jobCredentials = CreateJobCredentials();

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
