using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTExamples
{
    //public class FullServerBase : JobBase
    //{
    //    public FullServerBase(string jobType)
    //        :base(jobType)
    //    { }

    //    protected override async Task<WorkloadModel> CreateWorkload()
    //    {
    //        WorkloadModel workload = await base.CreateWorkload();

    //        ApiResponse<IEnumerable<LogicalItemModel>> workloadResponse = await workloadApi.GetLogicalItemsAsync(workload.Id);
    //        workloadResponse.EnsureSuccessStatusCode();

    //        SimpleLog.Log("Selecting repset(s).");

    //        foreach (var item in workloadResponse.Content)
    //        {
    //            SimpleLog.Log("Adding repset: '{0}'", item.Path);

    //            await workloadApi.SelectLogicalItemAsync(workload.Id, item.Path);
    //        }

    //        SimpleLog.Log("Workload updated and sent to server.");
    //        return await GetWorkload(workload.Id);
    //    }

    //    protected void SetupBackupConnection(CreateOptionsModel createOptions, string sourceReservedIp, string targetReservedIp)
    //    {
    //        SimpleLog.Log("Configuring backup connection (for reverse).");
            

    //        if (String.IsNullOrEmpty(sourceReservedIp))
    //        {
    //            SimpleLog.Log("Reverse disabled.");
    //            createOptions.JobOptions.FullServerFailoverOptions.CreateBackupConnection = false;
    //        }
    //        else
    //        {
    //            SimpleLog.Log("Setting reserved IP addresses {0} <source> and {1} <target>", sourceReservedIp, targetReservedIp);

    //            createOptions.JobOptions.SystemStateOptions.SourceReservedAddress = sourceReservedIp;
    //            createOptions.JobOptions.SystemStateOptions.TargetReservedAddress = targetReservedIp;
    //        }

    //        SimpleLog.Log("Backup connection configured.");
    //    }
    //}
}
