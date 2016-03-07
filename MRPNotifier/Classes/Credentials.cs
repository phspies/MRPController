using MahApps.Metro.Controls;
using MRPNotifier.MRPWCFService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;

namespace MRPNotifier
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        private List<Credential> _credential_list = new List<Credential>();
        BackgroundWorker credentialbgworker = new BackgroundWorker();

        public void load_credentiallist()
        {
            if (!credentialbgworker.IsBusy)
            {
                credentials_progress_indicator.Visibility = System.Windows.Visibility.Visible;
                credentials_progress_message.Visibility = System.Windows.Visibility.Visible;
                credentials_progress_indicator.IsActive = true;
                credentials_progress_message.Content = "Refreshing credential list";
            
                credentialbgworker.WorkerReportsProgress = true;
                credentialbgworker.WorkerSupportsCancellation = true;
                credentialbgworker.DoWork += new DoWorkEventHandler(load_credentiallist_worker);
                credentialbgworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(load_credentiallist_worker_complete);
                credentialbgworker.RunWorkerAsync();
            }
        }
        public void load_credentiallist_worker(object sender, DoWorkEventArgs e)
        {
            List<Credential> _credential_list = new List<Credential>();
            e.Result = channel.ListCredentials();
        }
        private void load_credentiallist_worker_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            _credential_list = (List<Credential>)e.Result;
            lvCredentials.ItemsSource = _credential_list;
            credentials_progress_indicator.Visibility = System.Windows.Visibility.Collapsed;
            credentials_progress_message.Visibility = System.Windows.Visibility.Collapsed;
            ((ObjectDataProvider)FindResource("workload_credentials_static")).Refresh();
        }
    }
}
