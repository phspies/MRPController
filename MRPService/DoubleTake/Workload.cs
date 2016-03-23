using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.DoubleTake
{
    class Workload : Core
    {
        public Workload(Doubletake doubletake) : base(doubletake) { }

        async public Task<WorkloadModel> CreateWorkload(String _job_type)
        {
            var connection = await ManagementService.GetConnectionAsync(_source_address);
            workloadApi = new WorkloadsApi(connection);

            var workloadResult = await workloadApi.CreateWorkloadAsync(_job_type);
            workloadResult.EnsureSuccessStatusCode();

            return workloadResult.Content;
        }

        async public Task<WorkloadModel> GetWorkload(Guid Id)
        {
            var workloadUpdateCall = await workloadApi.GetWorkloadAsync(Id);
            workloadUpdateCall.EnsureSuccessStatusCode();
            return workloadUpdateCall.Content;
        }

        async public Task DeleteWorkload(WorkloadModel workload)
        {
            ApiResponse deletedModel = await workloadApi.DeleteAsync(workload.Id);
            deletedModel.EnsureSuccessStatusCode();
        }

        async public Task<WorkloadModel> AddPhysicalRules(WorkloadModel workload, IEnumerable<PhysicalRuleModel> rules)
        {
            // While this works as well, 'workload.PhysicalRules = rules.ToArray();'
            // it is not recommended.  Calling the AddPhysicalRuleAsync method will help to 
            // 'fix' incomplete rules, or provide error feedback when given bad data

            foreach (PhysicalRuleModel rule in rules)
            {
                var result = await workloadApi.AddPhysicalRuleAsync(workload.Id, rule);
                result.EnsureSuccessStatusCode();
            }

            return await GetWorkload(workload.Id);
        }
        async public Task<IEnumerable<FileItemModel>> GetWorkloadFiles(WorkloadModel workload)
        {
            ApiResponse<IEnumerable<FileItemModel>> files = await workloadApi.GetFilesAsync(workload.Id);

            return files.Content;
        }

    }
}
