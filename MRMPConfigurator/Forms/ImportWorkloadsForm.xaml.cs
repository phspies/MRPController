using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using MRMPConfigurator.MRMPWCFService;
using System.Windows.Input;
using System.IO;
using System.Linq;

namespace MRMPConfigurator.Forms
{
    /// <summary>
    /// Interaction logic for Platformform.xaml
    /// </summary>
    public partial class ImportWorkloadsForm : MetroWindow
    {
        public List<Workload> _imported_workloads = new List<Workload>();
        public List<Credential> _credentials;
        public ImportWorkloadsForm(List<Credential> __credentials)
        {
            _credentials = __credentials.FindAll(x => x.credential_type == 1);
            InitializeComponent();
            initialize_form();
        }
        private void initialize_form()
        {
            workload_credential.ItemsSource = _credentials;
        }

        private void select_import_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    try
                    {
                        _imported_workloads = new List<Workload>();
                        var file = fileDialog.FileName;
                        filename.Text = file;
                        String[] csv = File.ReadAllLines(file);
                        foreach (string csvrow in csv)
                        {
                            String[] fields = csvrow.Split(','); // csv delimiter
                            if (fields.Count() > 1)
                            {
                                List<String> _ips = new List<String>(fields);
                                _imported_workloads.Add(new Workload()
                                {
                                    hostname = _ips.First(),
                                    iplist = String.Join(",", _ips.GetRange(1, _ips.Count() - 1))
                                });

                            }
                            message.Text = String.Format("{0} workloads found", _imported_workloads.Count());
                        }
                    }
                    catch (Exception ex)
                    {
                        message.Text = ex.Message;
                    }

                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    break;
            }
        }

        private void accept_import_Click(object sender, RoutedEventArgs e)
        {
            foreach (Workload _workload in _imported_workloads)
            {
                _workload.enabled = workload_enabled.IsChecked;
                if (((Credential)workload_credential.SelectedItem) != null)
                {
                    _workload.credential_id = ((Credential)workload_credential.SelectedItem).id;
                }
            }
            this.DialogResult = true;
            Close();
        }

        private void cancel_import_Click(object sender, RoutedEventArgs e)
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
