using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System.Threading.Tasks;

namespace MRPService.DoubleTake
{
    class Management : Core
    {
        public Management(Doubletake doubletake) : base(doubletake) { }

        public bool CheckLicense()
        {
            bool license_status = false;
            foreach (string _address in new[] { _source_address, _target_address })
            {
                if (_address != null)
                {
                    ManagementConnection connection = ManagementService.GetConnectionAsync(_address).Result;

                    //WorkloadsApi _workload = new WorkloadsApi(connection);
                    //List<WorkloadTypeModel> _workloadtypes = _workload.GetWorkloadTypesAsync().Result.Content.ToList();  
                    ////_information.Codes.Any(x => x.Attributes.Any(y => y.Name == "Error"));

                    ActivationStatusApi status = new ActivationStatusApi(connection);
                    ActivationStatusModel _information = status.GetActivationStatusAsync().Result.Content;

                    license_status = _information.IsValid;
                    if (!license_status) { return false; }
                }
            }
            return license_status;
        }

        public async Task<ProductInfoModel> GetProductInfo()
        {
            ProductInfoModel _information = null;
            foreach (string _address in new[] { _source_address, _target_address })
            {
                if (_address != null)
                {
                    ManagementConnection connection = await ManagementService.GetConnectionAsync(_address);
                    ProductInfoApi _product = new ProductInfoApi(connection);
                    _information = (await _product.GetProductInfoAsync()).Content;
                    break;
                }
            }
            return _information;

        }

    }
}
