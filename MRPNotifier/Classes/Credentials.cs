using MahApps.Metro.Controls;
using MRPNotifier.MRPWCFService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPNotifier
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        private List<Credential> _credential_list = new List<Credential>();
        BackgroundWorker credentialloader = new BackgroundWorker();

        public void load_credentiallist()
        {
            if (!credentialloader.IsBusy)
            {
                credentials_progress_indicator.Visibility = System.Windows.Visibility.Visible;
                credentials_progress_message.Visibility = System.Windows.Visibility.Visible;
                credentials_progress_indicator.IsActive = true;
                credentials_progress_message.Content = "Refreshing credential list";
            
                credentialloader.WorkerReportsProgress = true;
                credentialloader.WorkerSupportsCancellation = true;
                credentialloader.DoWork += new DoWorkEventHandler(load_credentiallist_worker);
                credentialloader.ProgressChanged += new ProgressChangedEventHandler(load_credentiallist_worker_changed);
                credentialloader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(load_credentiallist_worker_complete);
            }
        }
        public void load_credentiallist_worker(object sender, DoWorkEventArgs e)
        {
            WorkerState _state = new WorkerState();

            List<Credential> _credential_list = new List<Credential>();
            _credential_list = channel.ListCredentials();
            int index = 1;
            foreach (var credential in _credential_list)
            {
                int percentage = (index / _credential_list.Count) * 100;
                _state.message = String.Format("Updating {0} [{1} complete]", credential.description, percentage);
                (sender as BackgroundWorker).ReportProgress(percentage, _state);
            }
            e.Result = _credential_list;
        }
        private void load_credentiallist_worker_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            _credential_list = (List<Credential>)e.Result;
            lvCredentials.ItemsSource = _credential_list;
            credentials_progress_indicator.Visibility = System.Windows.Visibility.Collapsed;
            credentials_progress_message.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void load_credentiallist_worker_changed(object sender, ProgressChangedEventArgs e)
        {
            WorkerState ws = e.UserState as WorkerState;
            Debug.Print(ws.message);
            credentials_progress_message.Content = ws.message;
        }
    }
}
