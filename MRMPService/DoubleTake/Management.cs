using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MRMPService.DoubleTake
{
    class Management : Core
    {
        public Management(Doubletake doubletake) : base(doubletake) { }

        public bool CheckLicense(string _job_type)
        {
            bool license_status = false;


            switch (_job_type)
            {
                case "MoveServerMigration":
                    if (GetLicenses(_source_connection).Any(x => x.ProductName.Contains("Source")))
                    { license_status = true; }
                    if (GetLicenses(_target_connection).Any(x => x.ProductName.Contains("Target")))
                    { license_status = true; }
                    break;
            }
            return license_status;
        }
        static private List<ActivationCodeModel> GetLicenses(ManagementConnection _connection)
        {
            ActivationStatusApi status = new ActivationStatusApi(_connection);
            List<ActivationCodeModel> _information = status.GetActivationStatusAsync().Result.Content.Codes.ToList();
            return _information;
        }
        public void InstallLicense(string _target_license_key, string _source_license_key = null)
        {
            if (_source_license_key != null)
            {
                ActivationParametersApi _source_activation = new ActivationParametersApi(_source_connection);
                _source_activation.ApplyActivationParametersAsync(new ActivationParametersModel() { ActivationCodes = new String[] { _source_license_key } });

                ActivationStatusModel _activation_status = (_source_activation.SetActivationParametersAsync(new ActivationParametersModel() { ActivationCodes = new String[] { _source_license_key } }).Result).Content;
                foreach (ActivationCodeModel _type in _activation_status.Codes)
                {

                }
            }

            ActivationParametersApi _target_activation = new ActivationParametersApi(_target_connection);
            _target_activation.SetActivationParametersAsync(new ActivationParametersModel() { ActivationCodes = new String[] { _target_license_key } });
            _target_activation.ApplyActivationParametersAsync(new ActivationParametersModel() { ActivationCodes = new String[] { _target_license_key } });
        }

        public async Task<ProductInfoModel> GetProductInfo()
        {
            ProductInfoModel _information = null;
            foreach (string _address in new[] { _source_address, _target_address })
            {
                if (_address != null)
                {
                    ProductInfoApi _product = new ProductInfoApi(_target_connection);
                    _information = (await _product.GetProductInfoAsync()).Content;
                    break;
                }
            }
            return _information;

        }

    }
}
