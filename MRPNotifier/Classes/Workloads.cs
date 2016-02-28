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
                workloadloader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(load_workloadlist_worker_complete);
                workloadloader.RunWorkerAsync();
            }
        }
        public void load_workloadlist_worker(object sender, DoWorkEventArgs e)
        {
            List<Platform> _workload_list = new List<Platform>();
            e.Result = channel.ListPlatforms();
        }
        private void load_workloadlist_worker_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            _platform_list = (List<Platform>)e.Result;
            lvPlatforms.ItemsSource = _workload_list;
            workloads_progress_indicator.Visibility = System.Windows.Visibility.Collapsed;
            workloads_progress_message.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
