using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPDoubleTake
{
    class Workload : Core
    {
        new WorkloadsApi workloadApi;
        public Workload(Doubletake doubletake) : base(doubletake)
        {
            workloadApi = new WorkloadsApi(_source_connection);
        }

        public WorkloadModel CreateWorkload(String _job_type)
        {
            var workloadResult = workloadApi.CreateWorkloadAsync(_job_type).Result;
            workloadResult.EnsureSuccessStatusCode();

            return workloadResult.Content;
        }
        public WorkloadModel CreateWorkloadDRRecovery(Guid _image_id)
        {
            var workloadResult = workloadApi.CreateWorkloadAsync(DT_JobTypes.DR_Full_Recovery, _image_id, Guid.Empty).Result;
            workloadResult.EnsureSuccessStatusCode();
            return workloadResult.Content;
        }
        public WorkloadModel CreateWorkloadDRRecovery(Guid _image_id, Guid _snap_id)
        {
            var workloadResult = workloadApi.CreateWorkloadAsync(DT_JobTypes.DR_Full_Recovery, _image_id, _snap_id).Result;
            workloadResult.EnsureSuccessStatusCode();
            return workloadResult.Content;
        }

        public WorkloadModel GetWorkload(Guid Id)
        {
            var workloadUpdateCall = workloadApi.GetWorkloadAsync(Id).Result;
            workloadUpdateCall.EnsureSuccessStatusCode();
            return workloadUpdateCall.Content;
        }

        public void DeleteWorkload(WorkloadModel workload)
        {
            ApiResponse deletedModel = workloadApi.DeleteAsync(workload.Id).Result;
            deletedModel.EnsureSuccessStatusCode();
        }

        public WorkloadModel AddPhysicalRules(WorkloadModel workload, IEnumerable<PhysicalRuleModel> rules)
        {
            // While this works as well, 'workload.PhysicalRules = rules.ToArray();'
            // it is not recommended.  Calling the AddPhysicalRuleAsync method will help to 
            // 'fix' incomplete rules, or provide error feedback when given bad data

            foreach (PhysicalRuleModel rule in rules)
            {
                var result = workloadApi.AddPhysicalRuleAsync(workload.Id, rule).Result;
                result.EnsureSuccessStatusCode();
            }

            return GetWorkload(workload.Id);
        }
        public IEnumerable<FileItemModel> GetWorkloadFiles(WorkloadModel workload)
        {
            ApiResponse<IEnumerable<FileItemModel>> files = workloadApi.GetFilesAsync(workload.Id).Result;

            return files.Content;
        }

    }
}
