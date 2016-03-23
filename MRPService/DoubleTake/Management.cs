using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Linq;
using System.Net.Http;
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
                    ActivationStatusApi status = new ActivationStatusApi(connection);
                    ActivationStatusModel _information = status.GetActivationStatusAsync().Result.Content;
                    license_status = _information.IsValid;
                    if (!license_status) { return false; }
                }
            }
            return license_status;
        }

        public ProductInfoModel GetProductInfo()
        {
            ProductInfoModel _information = null;
            foreach (string _address in new[] { _source_address, _target_address })
            {
                if (_address != null)
                {
                    ManagementConnection connection = ManagementService.GetConnectionAsync(_address).Result;
                    ProductInfoApi _product = new ProductInfoApi(connection);
                    _information = _product.GetProductInfoAsync().Result.Content;
                    break;
                }
            }
            return _information;

        }

    }
}
