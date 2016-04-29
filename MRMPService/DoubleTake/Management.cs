using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MRMPService.DoubleTake
{
    class Management : Core
    {
        public Management(Doubletake doubletake) : base(doubletake) { }

        public bool CheckLicense()
        {
            bool license_status = false;
            foreach (ManagementConnection _server_connection in new[] { _source_connection, _target_connection })
            {
                if (_server_connection != null)
                {
                    ActivationStatusApi status = new ActivationStatusApi(_server_connection);
                    ActivationStatusModel _information = status.GetActivationStatusAsync().Result.Content;
                    if (_information.IsNodeLocked)
                    {
                        license_status = true;
                    }
                    else
                    {
                        license_status = _information.IsValid;
                    }
                    if (!license_status) { return false; }
                }
            }
            return license_status;
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
