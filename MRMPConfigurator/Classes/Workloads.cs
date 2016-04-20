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
            WCFResultType _result = new WCFResultType();
            try
            {
                _result.result = channel.ListWorkloads();
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
        private async void load_workloadlist_worker_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            WCFResultType _result = (WCFResultType)e.Result;
            if (_result.status)
            {
                _workload_list = (List<Workload>)_result.result;
                lvWorkloads.ItemsSource = _workload_list;
            }
            else
            {
                await this.ShowMessageAsync("Error while contacting MRMP Service", _result.message);
            }
            workloads_progress_indicator.Visibility = System.Windows.Visibility.Collapsed;
            workloads_progress_message.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
