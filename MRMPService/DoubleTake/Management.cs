using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
                    //check source first
                    ActivationCodeModel _source_premium_code = GetLicenses(_source_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "Availability") && x.Attributes.Any(z => z.Name == "Move"));
                    if (_source_premium_code != null)
                    {
                        if (_source_premium_code.Attributes.Any(x => x.Name == "NodeLockedValid"))
                        {
                            //This is a premium migration code and valid
                            //test if nodelock if date expired
                            if (_source_premium_code.IsExpired)
                            {
                                //notify use that current key will not work and that a new key would need to be generated
                            }
                            else
                            {
                                license_status = true;
                            }
                        }
                        else
                        {
                            //key has to be activated - using powershell code - do we have to use powershell?
                        }
                    }
                    //check target server
                    ActivationCodeModel _target_premium_code = GetLicenses(_target_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "TargetOnly") && x.Attributes.Any(z => z.Name == "Move"));
                    if (_target_premium_code == null)
                    {
                        //install target license - no activation required              
                    }
                    else
                    {
                        license_status = true;
                    }
                    license_status = true;
                    break;


                case "FullServerFailover":
                    //check source first
                    ActivationCodeModel _source_ha_protectcode = GetLicenses(_source_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "Availability") && x.Attributes.Any(z => z.Name == "LF_SPLA") && x.Attributes.Any(z => z.Name == "LF_SOURCE"));
                    if (_source_ha_protectcode == null)
                    {
                        InstallLicense(null, "1pqa-0naq-tt1n-wcfk-40ug-xqx6");
                        InstallLicense(null, "hv9e-6fg1-mqgv-d7e5-07ee-026v");
                        InstallLicense(null, "uqgn-mudg-t89m-7uqc-efpu-y2ee");
                        InstallLicense(null, "v8nr-b928-b546-5qr7-n3k9-v7g1");

                    }
                    else
                    {
                        license_status = true;
                    }
                    //check target server
                    ActivationCodeModel _target_ha_protectcode = GetLicenses(_source_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "Availability") && x.Attributes.Any(z => z.Name == "LF_SPLA") && x.Attributes.Any(z => z.Name == "LF_TARGET"));
                    if (_target_ha_protectcode == null)
                    {
                        SetDTMUCode(35, "dtmu_dev");
                    }
                    else
                    {
                        license_status = true;
                    }
                    break;
                case "FullServerImageProtection":
                case "DataOnlyImageProtection":
                    //check source first
                    ActivationCodeModel _source_dr_recovercode = GetLicenses(_source_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "DR") && x.Attributes.Any(z => z.Name == "LF_SPLA") && x.Attributes.Any(z => z.Name == "LF_SOURCE") && x.Attributes.Any(z => z.Name == "Agent"));
                    if (_source_dr_recovercode == null)
                    {
                        InstallLicense(null, "6fby-ef1f-bhpt-g904-bac9-6t9h");
                    }
                    //check target server
                    ActivationCodeModel _target_dr_recovercode = GetLicenses(_source_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "DR") && x.Attributes.Any(z => z.Name == "LF_SPLA") && x.Attributes.Any(z => z.Name == "LF_TARGET") && x.Attributes.Any(z => z.Name == "Repository"));
                    if (_target_dr_recovercode == null)
                    {
                        SetDTMUCode(35, "dtmu_dev");
                    }
                    break;
                case "FullServerImageRecovery":
                case "DataOnlyImageRecovery":
                    //check source repository
                    ActivationCodeModel _source_recovery_dr_recovercode = GetLicenses(_source_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "DR") && x.Attributes.Any(z => z.Name == "LF_SPLA") && x.Attributes.Any(z => z.Name == "LF_TARGET") && x.Attributes.Any(z => z.Name == "Repository"));
                    if (_source_recovery_dr_recovercode == null)
                    {
                        SetDTMUCode(35, "dtmu_dev");

                    }
                    //check target server
                    ActivationCodeModel _target_recovery_dr_recovercode = GetLicenses(_source_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "DR") && x.Attributes.Any(z => z.Name == "LF_SPLA") && x.Attributes.Any(z => z.Name == "LF_TARGET") && x.Attributes.Any(z => z.Name == "RecoveryTarget"));
                    if (_target_recovery_dr_recovercode == null)
                    {
                        InstallLicense(null, "47y8-heb4-1dkd-hb37-rmed-ry33");

                    }
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

        public List<DtmuProductModel> SetDTMUCode(int _product_code, string _customer_id)
        {
            DtmuApi api = new DtmuApi(_target_connection);

            DtmuConfigurationModel model = new DtmuConfigurationModel();
            model.ProductCode = _product_code;
            model.ProviderName = _customer_id;
            model.UserName = "phillip.spies@dimensiondata.com";

            ApiResponse response = api.SetConfigurationAsync(model).Result;

            response.EnsureSuccessStatusCode();

            ApiResponse<IEnumerable<DtmuProductModel>> confirm_response = api.GetProductsAsync().Result;

            response.EnsureSuccessStatusCode();

            return confirm_response.Content.ToList();
        }
    }
}
