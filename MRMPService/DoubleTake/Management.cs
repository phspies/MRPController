using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Management.Automation;
using System.IO;
using MRMPService.MRMPAPI.Types.API;

namespace MRMPService.MRMPDoubleTake
{
    class Management : Core
    {
        public Management(Doubletake doubletake) : base(doubletake) { }

        public bool CheckLicense(string _job_type, string _org_id, string _source_license_key = null, string _target_license_key = null)
        {
            bool _source_license_status = false;
            bool _target_license_status = false;

            switch (_job_type)
            {
                case "MoveServerMigration":
                    while (true)
                    {
                        //check source first
                        List<ActivationCodeModel> _source_licenses = GetLicenses(_source_connection);

                        ActivationCodeModel _source_premium_code = _source_licenses.FirstOrDefault(x => x.Attributes.Any(y => y.Name == "Availability") && x.Attributes.Any(z => z.Name == "Move"));
                        ActivationCodeModel _source_eval_code = _source_licenses.FirstOrDefault(x => x.Attributes.Any(y => y.Name == "Eval") && x.Attributes.Any(z => z.Name == "Move"));

                        //could not find an eval nor a premium code
                        if (_source_premium_code == null && _source_eval_code == null)
                        {
                            InstallLicense(DT_WorkloadType.Source, _source_deployment_policy.source_activation_code);
                            continue;
                        }

                        //we dealing with a eval key
                        else if (_source_eval_code != null && _source_premium_code == null)
                        {
                            if (_source_eval_code.IsValid && _source_eval_code.IsEvaluation)
                            {
                                _source_license_status = true;
                                break;
                            }
                            else if (!_source_eval_code.IsValid && _source_eval_code.IsExpired)
                            {
                                throw new Exception(String.Format("Current evaluation license expired {0}. Please contact vendor for new evaluation license key", _source_premium_code.ExpirationDate));
                            }
                        }

                        //we dealing with a premium code
                        else if (_source_eval_code == null && _source_premium_code != null)
                        {
                            if (_source_premium_code.IsExpired)
                            {
                                throw new Exception(String.Format("Current premium license expired {0}. Please contact vendor for new premium license key", _source_premium_code.ExpirationDate));
                            }
                            else if (!_source_premium_code.IsValid)
                            {
                                bool _success = ActivateLicense(DT_WorkloadType.Source);
                                if (_success)
                                {
                                    continue;
                                }
                            }
                            else if (_source_premium_code.IsValid)
                            {
                                _source_license_status = true;
                                break;
                            }
                        }
                    }

                    //    if (_source_premium_code.Attributes.Any(x => x.Name == "NodeLockedValid"))
                    //    {
                    //        //This is a premium migration code and valid
                    //        //test if nodelock if date expired
                    //        if (_source_premium_code.IsExpired)
                    //        {
                    //            throw new Exception("Current move license key has expired. Please contact vendor for new move license key");
                    //        }
                    //        else
                    //        {
                    //            _source_license_status = true;
                    //        }
                    //    }
                    //}

                    //check target server
                    while (true)
                    {
                        ActivationCodeModel _target_premium_code = GetLicenses(_target_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "TargetOnly") && x.Attributes.Any(z => z.Name == "Move"));
                        if (_target_premium_code == null)
                        {
                            InstallLicense(DT_WorkloadType.Target, _target_deployment_policy.target_activation_code);
                            continue;
                        }
                        else
                        {
                            if (_target_premium_code.IsValid)
                            {
                                _target_license_status = true;
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    break;
                case "FullServerFailover":
                    //check source first
                    ActivationCodeModel _source_ha_protectcode = GetLicenses(_source_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "Availability") && x.Attributes.Any(z => z.Name == "LF_SPLA") && x.Attributes.Any(z => z.Name == "LF_SOURCE"));
                    if (_source_ha_protectcode == null)
                    {
                        InstallLicense(DT_WorkloadType.Source, "1pqa-0naq-tt1n-wcfk-40ug-xqx6");
                        InstallLicense(DT_WorkloadType.Source, "hv9e-6fg1-mqgv-d7e5-07ee-026v");
                        InstallLicense(DT_WorkloadType.Source, "uqgn-mudg-t89m-7uqc-efpu-y2ee");
                        InstallLicense(DT_WorkloadType.Source, "v8nr-b928-b546-5qr7-n3k9-v7g1");
                    }
                    else
                    {
                        if (_source_ha_protectcode.IsValid)
                        {
                            _source_license_status = true;
                        }
                    }
                    //check target server
                    ActivationCodeModel _target_ha_protectcode = GetLicenses(_target_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "Availability") && x.Attributes.Any(z => z.Name == "LF_SPLA") && x.Attributes.Any(z => z.Name == "LF_TARGET"));
                    if (_target_ha_protectcode == null)
                    {
                        SetDTMUCode(35, _org_id, DT_WorkloadType.Target);
                    }
                    else
                    {
                        if (_target_ha_protectcode.IsValid)
                        {
                            _target_license_status = true;
                        }
                    }
                    break;
                case "FullServerImageProtection":
                case "DataOnlyImageProtection":
                    //check source first
                    ActivationCodeModel _source_dr_recovercode = GetLicenses(_source_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "DR") && x.Attributes.Any(z => z.Name == "LF_SPLA") && x.Attributes.Any(z => z.Name == "LF_SOURCE") && x.Attributes.Any(z => z.Name == "Agent"));
                    if (_source_dr_recovercode == null)
                    {
                        InstallLicense(DT_WorkloadType.Source, "6fby-ef1f-bhpt-g904-bac9-6t9h");
                    }
                    else
                    {
                        if (_source_dr_recovercode.IsValid)
                        {
                            _source_license_status = true;
                        }
                    }
                    //check target server
                    ActivationCodeModel _target_dr_recovercode = GetLicenses(_target_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "DR") && x.Attributes.Any(z => z.Name == "LF_SPLA") && x.Attributes.Any(z => z.Name == "LF_TARGET") && x.Attributes.Any(z => z.Name == "Repository"));
                    if (_target_dr_recovercode == null)
                    {
                        SetDTMUCode(111, _org_id, DT_WorkloadType.Target);
                    }
                    else
                    {
                        if (_target_dr_recovercode.IsValid)
                        {
                            _target_license_status = true;
                        }
                    }
                    break;
                case "FullServerImageRecovery":
                case "DataOnlyImageRecovery":
                    //check source repository
                    ActivationCodeModel _source_recovery_dr_recovercode = GetLicenses(_source_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "DR") && x.Attributes.Any(z => z.Name == "LF_SPLA") && x.Attributes.Any(z => z.Name == "LF_TARGET") && x.Attributes.Any(z => z.Name == "Repository"));
                    if (_source_recovery_dr_recovercode == null)
                    {
                        SetDTMUCode(111, _org_id, DT_WorkloadType.Source);
                    }
                    else
                    {
                        if (_source_recovery_dr_recovercode.IsValid)
                        {
                            _source_license_status = true;
                        }
                    }
                    //check target server
                    ActivationCodeModel _target_recovery_dr_recovercode = GetLicenses(_target_connection).FirstOrDefault(x => x.Attributes.Any(y => y.Name == "DR") && x.Attributes.Any(z => z.Name == "LF_SPLA") && x.Attributes.Any(z => z.Name == "LF_TARGET") && x.Attributes.Any(z => z.Name == "RecoveryTarget"));
                    if (_target_recovery_dr_recovercode == null)
                    {
                        InstallLicense(DT_WorkloadType.Target, "47y8-heb4-1dkd-hb37-rmed-ry33");

                    }
                    else
                    {
                        if (_target_recovery_dr_recovercode.IsValid)
                        {
                            _target_license_status = true;
                        }
                    }
                    break;

            }
            return (_source_license_status && _target_license_status) ? true : false;
        }
        static private List<ActivationCodeModel> GetLicenses(ManagementConnection _connection)
        {
            ActivationStatusApi status = new ActivationStatusApi(_connection);
            List<ActivationCodeModel> _information = status.GetActivationStatusAsync().Result.Content.Codes.ToList();
            return _information;
        }
        public void InstallLicense(DT_WorkloadType _workload_type, string _license_key)
        {
            PowerShell ps = PowerShell.Create();
            try
            {
                ImportDoubleTake(ps);
                DoubleTake.PowerShell.Server server = GetServer(ps, _workload_type == DT_WorkloadType.Source ? _source_address : _target_address, _workload_type == DT_WorkloadType.Source ? _source_credentials : _target_credentials);
                ApplyActivationCode(ps, server, _license_key);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error applying license key to workload: {0}", ex.Message));
            }
        }
        public bool ActivateLicense(DT_WorkloadType _workload_type)
        {
            PowerShell ps = PowerShell.Create();
            try
            {
                ImportDoubleTake(ps);
                DoubleTake.PowerShell.Server server = GetServer(ps, _workload_type == DT_WorkloadType.Source ? _source_address : _target_address, _workload_type == DT_WorkloadType.Source ? _source_credentials : _target_credentials);
                PSObject activationStatus = ActivateOnline(ps, server);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
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

        public List<DtmuProductModel> SetDTMUCode(int _product_code, string _customer_id, DT_WorkloadType _workload_type)
        {
            DtmuApi api = new DtmuApi((_workload_type == DT_WorkloadType.Source ? _source_connection : _target_connection));

            DtmuConfigurationModel model = new DtmuConfigurationModel();
            model.ProductCode = _product_code;
            model.ProviderName = "213712";
            model.UserName = _customer_id;

            ApiResponse response = api.SetConfigurationAsync(model).Result;

            response.EnsureSuccessStatusCode();

            ApiResponse<IEnumerable<DtmuProductModel>> confirm_response = api.GetProductsAsync().Result;

            response.EnsureSuccessStatusCode();

            return confirm_response.Content.ToList();
        }
        private static PSObject ActivateOnline(PowerShell ps, DoubleTake.PowerShell.Server server)
        {
            ps.AddCommand("Get-DtOnlineActivationRequest").AddParameter("ServiceHost", server).AddCommand("Request-DtOnlineActivation").AddCommand("Set-DtActivationCode");
            List<PSObject> activationStatus = ps.Invoke().ToList();
            return activationStatus.First();
        }

        private void ApplyActivationCode(PowerShell ps, DoubleTake.PowerShell.Server server, string _license_code)
        {
            ps.AddCommand("Set-DtActivationCode").AddParameter("ServiceHost", server).AddParameter("Code", _license_code);
            var o = ps.Invoke();
            ps.Commands.Clear();
        }

        private DoubleTake.PowerShell.Server GetServer(PowerShell ps, String _address, MRPCredentialType _credentials)
        {
            PowerShell newDtServer = ps.AddCommand("New-DtServer").AddParameter("Name", _address).AddParameter("Username", _credentials.username).AddParameter("Password", _credentials.encrypted_password);
            var output = newDtServer.Invoke().ToList();
            ps.Commands.Clear();
            DoubleTake.PowerShell.Server server = (DoubleTake.PowerShell.Server)output.First().BaseObject;
            return server;
        }

        private static void ImportDoubleTake(PowerShell ps)
        {
            String cmdletModulePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"DoubleTake.PowerShell.dll");
            ps.AddCommand("Import-Module").AddParameter("Name", cmdletModulePath);
            ps.Invoke().ToList();
            ps.Commands.Clear();
        }
    }
}
