using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MRMPService.Utilities;
using MRMPService.Modules.MRMPPortal.Contracts;
using System.Linq;
using static MRMPService.Modules.OperatingSystemTasks.Windows.ServiceManagement;

namespace MRMPService.Modules.DoubleTake.Common
{
    partial class ModuleCommon
    {
        public static void DeployWindowsDoubleTakePatches(MRPTaskType _task, MRMPWorkloadBaseType _workload, float _start_progress, float _end_progress)
        {
            string _local_path_location = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"Double-Take", @"Windows", @"Patches");
            _task.progress(String.Format("Applying datamover deploying patches to {0}", _workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 5));
            string[] _patches = Directory.GetDirectories(_local_path_location);
            _task.progress($"Found {_patches.Count()} patch(s) on manager", ReportProgress.Progress(_start_progress, _end_progress, 10));
            if (_patches.Length == 0)
            {
                return;
            }
            Thread.Sleep(TimeSpan.FromSeconds(30));
            string[] _dt_services = new string[] { "Double-Take", "CoreManagementService" };
            _task.progress($"Stopping {_dt_services.StringJoin(", ")} services on {_workload.hostname}", ReportProgress.Progress(_start_progress, _end_progress, 13));
            foreach (string _srv in _dt_services)
            {
                int _tries = 0;
                int _connect_tries = 0;
                while (_tries++ < 3)
                {
                    ServiceManagementServiceState _state = ServiceManagementServiceState.Running;
                    while (_connect_tries++ < 3)
                        {
                        try
                        {
                            _state = _workload.WMIMethods.GetServiceState(_srv);
                        }
                        catch (Exception _ex)
                        {
                            Thread.Sleep(new TimeSpan(0, 0, 10));
                            if (_connect_tries > 3)
                            {
                                throw new Exception($"{_srv} not found after 3 tries on #{_workload.hostname} : {_ex.GetBaseException().Message}");
                        }
                            continue;
                        }

                    }
                    if (_state != ServiceManagementServiceState.Stopped)
                    {
                        _workload.WMIMethods.StopService(_srv);
                        _workload.WMIMethods.KillProcess($"{ _srv.Replace("-","")}.exe");
                    }
                    else
                    {
                        break;
                    }
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                    if (_tries > 3)
                    {
                        throw new Exception($"{_srv} did not stop after 3 tries on #{_workload.hostname}");
                    }
                }
            }
            string _dt_reg_key = @"SOFTWARE\NSI Software\Double-Take\CurrentVersion";
            string[] _keys = new string[] { "InstallVersionInfo", "InstallPath" };
            Dictionary<string, object> _returned_values = _workload.WMIMethods.GetRemoteRegistryKeys(_dt_reg_key, _keys);
            if (_returned_values.Count != 0 || !_returned_values.Any(x => String.IsNullOrEmpty((string)x.Value)))
            {
                foreach (var _patch in _patches)
                {
                    _task.progress($"Applying {Path.GetFileName(_patch)} to {_workload.hostname}", ReportProgress.Progress(_start_progress, _end_progress, 15 + Array.IndexOf(_patches, _patch)));
                    string _remote_installed_path = Path.Combine(@"\\", _workload.GetContactibleIP(true), ((string)_returned_values["InstallPath"]).Replace(':', '$'));
                    var _source_path = Directory.Exists(Path.Combine(_patch, "x64")) ? Path.Combine(_patch, "x64") : _patch;
                    _workload.WMIMethods.CopyLocalToRemoteDirectory(_source_path, _remote_installed_path, true, new string[] { "Console" });
                }
            }
            else
            {
                throw new Exception($"Datamover not installed on {_workload.hostname}");
            }
            _task.progress($"Starting {_dt_services.StringJoin(",")} services on {_workload.hostname}", ReportProgress.Progress(_start_progress, _end_progress, 80));
            foreach (string _srv in _dt_services)
            {
                int _tries = 0;
                while (_tries++ < 3)
                {
                    ServiceManagementServiceState _state = _workload.WMIMethods.GetServiceState(_srv);
                    if (_state == ServiceManagementServiceState.Stopped)
                    {
                        _workload.WMIMethods.StartService(_srv);
                    }
                    else if (_state == ServiceManagementServiceState.Running)
                    {
                        break;
                    }
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
            _task.progress($"Completed patch deployment for {_workload.hostname}", ReportProgress.Progress(_start_progress, _end_progress, 90));
        }
    }
}
