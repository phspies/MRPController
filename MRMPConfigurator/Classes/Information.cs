using MRMPNotifier.Classes;
using MRMPNotifier.Classes.Common;
using MRMPNotifier.MRMPWCFService;
using System;
using System.ComponentModel;
using System.ServiceModel;

namespace MRMPNotifier
{
    public partial class MainWindow : Window
    {
        BackgroundWorker informationbgworker = new BackgroundWorker();

        public void load_information()
        {
            if (!informationbgworker.IsBusy)
            {
                //information_progress_indicator.Visibility = System.Windows.Visibility.Visible;
                //information_progress_message.Visibility = System.Windows.Visibility.Visible;
                //information_progress_indicator.IsActive = true;
                //information_progress_message.Content = "Refreshing Manager Information";
            
                informationbgworker.WorkerReportsProgress = true;
                informationbgworker.WorkerSupportsCancellation = true;
                informationbgworker.DoWork += new DoWorkEventHandler(load_informationlist_worker);
                informationbgworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(load_informationlist_worker_complete);
                informationbgworker.RunWorkerAsync();
            }
        }
        public void load_informationlist_worker(object sender, DoWorkEventArgs e)
        {
            WCFResultType _result = new WCFResultType();
            try
            {
                _result.result = channel.CollectionInformation();
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
        private async void load_informationlist_worker_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            WCFResultType _result = (WCFResultType)e.Result;
            if (_result.status)
            {
                _information = (workerInformation)_result.result;
            }
            else
            {

            }
            //information_progress_indicator.Visibility = System.Windows.Visibility.Collapsed;
            //information_progress_message.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
