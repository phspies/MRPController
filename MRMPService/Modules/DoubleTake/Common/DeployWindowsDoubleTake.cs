using Microsoft.Win32;
using MRMPService.MRMPService.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;
using MRMPService.Utilities;
using DoubleTake.Web.Models;
using MRMPService.MRMPDoubleTake;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.Exceptions;
using System.Linq;

namespace MRMPService.Modules.DoubleTake.Common
{
    partial class ModuleCommon
    {
        public static void DeployWindowsDoubleTake(MRPTaskType _task, MRMPWorkloadBaseType _source_workload, MRMPWorkloadBaseType _target_workload, float _start_progress, float _end_progress)
        {
            FileVersionInfo localFileVersion;
            string localFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"Double-Take", @"Windows", @"setup.exe");
            try
            {
                localFileVersion = FileVersionInfo.GetVersionInfo(localFilePath);
            }
            catch (Exception ex)
            {
                throw new MRMPDatamoverException(String.Format("Error finding datamover installation file on Manager {0}", ex.GetBaseException().Message));
            }
            int _counter = 0;
            Dictionary<dt_server_type, MRMPWorkloadBaseType> _workloads = new Dictionary<dt_server_type, MRMPWorkloadBaseType>() { { dt_server_type.target, MRMPServiceBase._mrmp_api.workload().get_by_id(_target_workload.id) }, { dt_server_type.source, MRMPServiceBase._mrmp_api.workload().get_by_id(_source_workload.id) } };
            foreach (KeyValuePair<dt_server_type, MRMPWorkloadBaseType> _workload_entry in _workloads)
            {
                bool _install_dm = false;
                switch (_workload_entry.Key)
                {
                    case dt_server_type.target:                       
                        _counter = 1;
                        break;
                    case dt_server_type.source:
                        _counter = 50;
                        break;
                }
                MRMPWorkloadBaseType _working_workload = _workload_entry.Value;

                _task.progress(String.Format("Starting datamover deploying process on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 5));
                string _contactable_ip = _working_workload.GetContactibleIP(true);
                string _dt_reg_key = @"SOFTWARE\NSI Software\Double-Take\CurrentVersion";
                string[] _keys = new string[] { "InstallVersionInfo", "InstallPath" };
                Dictionary<string, object> _returned_values = _working_workload.WMIMethods.GetRemoteRegistryKeys(_dt_reg_key, _keys);
                if (_returned_values.Any(x => x.Value == null))
                {
                    _install_dm = true;
                    _task.progress(String.Format("Datamover not found on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 11));
                }
                else
                {
                    string _dt_installed_version = (string)_returned_values["InstallVersionInfo"];
                    string _dt_installed_path = (string)_returned_values["InstallPath"];
                    _task.progress(String.Format("Datamover Found on {0} : {1} [{2}] ", _working_workload.hostname, _dt_installed_version, _dt_installed_path), ReportProgress.Progress(_start_progress, _end_progress, _counter + 10));
                    if (Versions.Compare(localFileVersion.ProductVersion, (string)_returned_values["InstallVersionInfo"]) > 0)
                    {
                        _install_dm = true;
                        _task.progress(String.Format("Upgrading datamover from {0} to {1} on {2}", _dt_installed_version, localFileVersion.ProductVersion, _working_workload.hostname, _dt_installed_version, _dt_installed_path), ReportProgress.Progress(_start_progress, _end_progress, _counter + 11));
                    }
                    else
                    {
                        _task.progress(String.Format("Testing datamover connectivity on {0} using {1}", _working_workload.hostname, _contactable_ip), ReportProgress.Progress(_start_progress, _end_progress, _counter + 14));
                        ProductVersionModel _installed_dt_version;
                        try
                        {
                            using (Doubletake _dt = new Doubletake(null, _working_workload))
                            {
                                _installed_dt_version = (_dt.management().GetProductInfo()).ManagementServiceVersion;
                                _task.progress(String.Format("Datamover installed and running on {0} with version {1}", _working_workload.hostname, _dt.management().GetProductVersion(_installed_dt_version)), ReportProgress.Progress(_start_progress, _end_progress, _counter + 15));
                            }
                        }
                        catch (Exception ex)
                        {
                            _install_dm = true;
                            _task.progress(String.Format("Datamover installed on {0} but cannot be contacted: {1}", _working_workload.hostname, ex.GetBaseException().Message), ReportProgress.Progress(_start_progress, _end_progress, _counter + 15));
                        }

                    }
                }
                if (_install_dm)
                {
                    _task.progress(String.Format("Copy installation file to {0} on {1})", _working_workload.deploymentpolicy.dt_windows_temppath, _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 20));
                    _working_workload.WMIMethods.CopyLocalToRemoteFile(localFilePath, _working_workload.deploymentpolicy.dt_windows_temppath);
                    _task.progress(String.Format("Complete binaries copy process for {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 21));
                    _task.progress(String.Format("Copy configuration file to {0} on {1}", _working_workload.deploymentpolicy.dt_windows_temppath, _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 25));
                    _working_workload.WMIMethods.CopyStringArrayToRemoteFile(BuildINI.BuildINIFile(_working_workload.deploymentpolicy, _workload_entry.Key).ToArray(), Path.Combine(_working_workload.deploymentpolicy.dt_windows_temppath, "DTSetup.ini"));
                    _task.progress(String.Format("Complete configuration copy process for {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 26));
                    _task.progress(String.Format("Starting installer on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 30));
                    string installCmd = @"cmd.exe /c " + _working_workload.deploymentpolicy.dt_windows_temppath + "\\setup.exe /s /v\"DTSETUPINI=\\\"" + _working_workload.deploymentpolicy.dt_windows_temppath + "\\" + "DTSetup.ini\\\" /qn /l*v+ " + _working_workload.deploymentpolicy.dt_windows_temppath + "\\Repinst.log";
                    _working_workload.WMIMethods.RunCommand(_working_workload.deploymentpolicy.dt_windows_temppath, installCmd);
                    _task.progress(String.Format("Verify DT connectivity on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 40));
                    ProductVersionModel _dt_version;
                    int restries = 0;
                    using (Doubletake _dt = new Doubletake(null, _working_workload))
                    {
                        while (true)
                        {
                            try
                            {
                                _dt_version = _dt.management().GetProductInfo().ManagementServiceVersion;
                                _task.progress(String.Format("Datamover {0} found on {1}", _dt.management().GetProductVersion(_dt_version), _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 45));
                                break;
                            }
                            catch (Exception ex)
                            {
                                if (restries++ >= 3)
                                {
                                    throw new MRMPDatamoverException(String.Format("Cannot determine installed version of datamover on {0} : {1}", _working_workload.hostname, ex.GetBaseException().Message));
                                }
                            }
                            Thread.Sleep(new TimeSpan(0, 0, 10));
                            _task.progress(String.Format("Verify datamover connectivity on {0} [{1}]", _working_workload.hostname, restries), ReportProgress.Progress(_start_progress, _end_progress, _counter + 41 + restries));
                        }
                    }
                }
            }
            _task.progress("Completed datamover deployment", ReportProgress.Progress(_start_progress, _end_progress, _counter + 47));
        }
    }
}
