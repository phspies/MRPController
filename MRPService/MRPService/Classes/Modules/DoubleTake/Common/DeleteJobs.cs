using DoubleTake.Core.Contract;
using DoubleTake.Jobs.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MRPService.DoubleTake
{
    class DeleteJobs
    {
        public static void Delete(IJobManager iJobMgr, JobInfo _delete_job)
        {
            DeleteOptions _delete_options = new DeleteOptions();
            _delete_options.DeleteReplica = true;
            _delete_options.DiscardTargetQueue = true;
            ImageDeleteInfo _delete_info = new ImageDeleteInfo();
            _delete_info.VhdDeleteAction = VhdDeleteActionType.DeleteAll;
            _delete_info.DeleteImage = true;
            _delete_options.ImageOptions = _delete_info;
            iJobMgr.Stop(_delete_job.Id);
            iJobMgr.Delete(_delete_job.Id, _delete_options);
            try
            {
                while (true)
                {
                    iJobMgr.GetJob(_delete_job.Id);
                    Thread.Sleep(2000);
                }
            }
            catch (Exception) { }
        }
    }
}
