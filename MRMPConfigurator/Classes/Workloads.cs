using MahApps.Metro.Controls;
using MRMPConfigurator.MRMPWCFService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace MRMPConfigurator
{
    public partial class MainWindow : MetroWindow
    {
        private List<Workload> _workload_list = new List<Workload>();
        BackgroundWorker workloadbgworker = new BackgroundWorker();

        public void load_workloadlist()
        {
            if (!workloadbgworker.IsBusy)
            {
                workloads_progress_indicator.Visibility = System.Windows.Visibility.Visible;
                workloads_progress_message.Visibility = System.Windows.Visibility.Visible;
                workloads_progress_indicator.IsActive = true;
                workloads_progress_message.Content = "Refreshing workload list";
            
                workloadbgworker.WorkerReportsProgress = true;
                workloadbgworker.WorkerSupportsCancellation = true;
                workloadbgworker.DoWork += new DoWorkEventHandler(load_workloadlist_worker);
                workloadbgworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(load_workloadlist_worker_complete);
                workloadbgworker.RunWorkerAsync();
            }
        }
        public void load_workloadlist_worker(object sender, DoWorkEventArgs e)
        {
            List<Workload> _workload_list = new List<Workload>();
            e.Result = channel.ListWorkloads();
        }
        private void load_workloadlist_worker_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            _workload_list = (List<Workload>)e.Result;
            lvWorkloads.ItemsSource = _workload_list;
            workloads_progress_indicator.Visibility = System.Windows.Visibility.Collapsed;
            workloads_progress_message.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
