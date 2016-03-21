using DoubleTake.Core.Contract;
using System.Collections.Generic;

namespace MRPService.DoubleTake
{
    public class ManagementService : Core
    {
        public ManagementService(MRP_DoubleTake doubletake) : base(doubletake) { }

        public bool check_license_status()
        {
            IManagementService _source_iMgtSrvFactory = ManagementService(DT_WorkloadType.Source);
            IManagementService _target_iMgtSrvFactory = ManagementService(DT_WorkloadType.Target);

            foreach (IManagementService _mgt_service in new List<IManagementService>() { _source_iMgtSrvFactory, _target_iMgtSrvFactory })
            {
                ActivationStatus _status = _mgt_service.GetProductInfo().ActivationStatus;
                switch (_status.IsValid)
                {
                    case true:
                        return true;
                    case false:
                        return false;
                }
            }
            return false;
        }
    }
}
