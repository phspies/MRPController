using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using MRMPConfigurator.MRMPWCFService;
using System.Windows.Input;

namespace MRMPConfigurator.Forms
{
    /// <summary>
    /// Interaction logic for Platformform.xaml
    /// </summary>
    public partial class WorkloadForm : MetroWindow
    {
        public Workload _record;
        private int _action;
        public List<Credential> _credentials;
        public WorkloadForm(Workload __record, int __action, List<Credential> __credentials)
        {
            _credentials = __credentials.FindAll(x => x.credential_type == 1);
            _record = __record;
            InitializeComponent();
            initialize_form();
            _action = __action;
            switch (__action)
            {
                case 0:
                    this.Title = "Add Workload";
                    break;
                case 1:
                    this.Title = "Update Workload";
                    break;
            }
        }
        private void initialize_form()
        {
            workload_credential.ItemsSource = _credentials;
            this.DataContext = _record;
        }


        private void accept_platform_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void cancel_platform_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public class WaitCursor : IDisposable
        {
            private Cursor _previousCursor;

            public WaitCursor()
            {
                _previousCursor = Mouse.OverrideCursor;

                Mouse.OverrideCursor = Cursors.Wait;
            }

            #region IDisposable Members

            public void Dispose()
            {
                Mouse.OverrideCursor = _previousCursor;
            }

            #endregion
        }
    }
}
