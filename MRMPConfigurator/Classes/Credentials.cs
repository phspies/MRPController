using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MRMPConfigurator.Classes;
using MRMPConfigurator.Classes.Common;
using MRMPConfigurator.MRMPWCFService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Windows.Data;

namespace MRMPConfigurator
{
    public partial class MainWindow : MetroWindow
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
            WCFResultType _result = new WCFResultType();
            try
            {
                _result.result = channel.ListCredentials();
                _result.status = true;
            }
            catch (TimeoutException ex)
            {
                _result.status = false;
                _result.message = String.Format("Service has timed out : {0}", ex.Message);
            }
            catch (FaultException<UnexpectedServiceFault> ex)
            {
                _result.message = String.Format("Service error occurred: {0} {1}", ex.Message, System.Environment.NewLine);
                _result.message = String.Format("service message: {0} {1}", ex.Detail.ErrorMessage, System.Environment.NewLine);
                _result.message = String.Format("source: {0} {1}", ex.Detail.Source, System.Environment.NewLine);
                _result.message = String.Format("target: {0} {1}", ex.Detail.Target, System.Environment.NewLine);
                _result.message = String.Format("stack trace: {0} {1}", ex.Detail.StackTrace, System.Environment.NewLine);
                _result.status = false;
            }
            catch (FaultException ex)
            {
                _result.status = false;
                _result.message = String.Format("Service error occurred: {0}", ex.Message);
            }

            catch (CommunicationException ex)
            {
                _result.status = false;
                _result.message = String.Format("Communications error occurred: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = String.Format("Error occurred: {0}", ex.Message);
            }
            e.Result = _result;
        }
        private async void load_credentiallist_worker_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            WCFResultType _result = (WCFResultType)e.Result;
            if (_result.status)
            {
                _credential_list = (List<Credential>)_result.result;
                lvCredentials.ItemsSource = _credential_list;
            }
            else
            {
                await this.ShowMessageAsync("Error while contacting MRMP Service", _result.message);
            }
            credentials_progress_indicator.Visibility = System.Windows.Visibility.Collapsed;
            credentials_progress_message.Visibility = System.Windows.Visibility.Collapsed;

            //Refresh all credential combo's on all workload objects in view
            ((ObjectDataProvider)FindResource("workload_credentials_static")).Refresh();
        }
    }
}
