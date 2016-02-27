using MahApps.Metro.Controls;
using MRPNotifier.MRPWCFService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace MRPNotifier
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        private List<Platform> _workload_list = new List<Platform>();
        BackgroundWorker workloadloader = new BackgroundWorker();

        public void load_workloadlist()
        {
            if (!workloadloader.IsBusy)
            {
                workloads_progress_indicator.Visibility = System.Windows.Visibility.Visible;
                workloads_progress_message.Visibility = System.Windows.Visibility.Visible;
                workloads_progress_indicator.IsActive = true;
                workloads_progress_message.Content = "Refreshing workload list";
            
                workloadloader.WorkerReportsProgress = true;
                workloadloader.WorkerSupportsCancellation = true;
                workloadloader.DoWork += new DoWorkEventHandler(load_workloadlist_worker);
                workloadloader.ProgressChanged += new ProgressChangedEventHandler(load_workloadlist_worker_changed);
                workloadloader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(load_workloadlist_worker_complete);
            }
        }
        public void load_workloadlist_worker(object sender, DoWorkEventArgs e)
        {
            WorkerState _state = new WorkerState();

            List<Platform> _workload_list = new List<Platform>();
            _workload_list = channel.ListPlatforms();
            int index = 1;
            foreach (var workload in _workload_list)
            {
                int percentage = (index / _workload_list.Count) * 100;
                _state.message = String.Format("Updating {0} [{1} complete]", workload.description, percentage);
                (sender as BackgroundWorker).ReportProgress(percentage, _state);
            }
            e.Result = _workload_list;
        }
        private void load_workloadlist_worker_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            _platform_list = (List<Platform>)e.Result;
            lvPlatforms.ItemsSource = _workload_list;
            workloads_progress_indicator.Visibility = System.Windows.Visibility.Collapsed;
            workloads_progress_message.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void load_workloadlist_worker_changed(object sender, ProgressChangedEventArgs e)
        {
            WorkerState ws = e.UserState as WorkerState;
            Debug.Print(ws.message);
            workloads_progress_message.Content = ws.message;
        }
    }
}
