using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using MRMPConfigurator.MRMPWCFService;
using System.Windows.Input;
using System.IO;

namespace MRMPConfigurator.Forms
{
    /// <summary>
    /// Interaction logic for Platformform.xaml
    /// </summary>
    public partial class ImportWorkloadsForm : MetroWindow
    {
        public List<Workload> _imported_workloads = new List<Workload>();
        public ImportWorkloadsForm()
        {
            initialize_form();
        }
        private void initialize_form()
        {
            InitializeComponent();
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
                        int success, failed;
                        success = failed = 0;
                        foreach (string csvrow in csv)
                        {
                            String[] fields = csvrow.Split(','); // csv delimiter
                            if (fields.Length > 1)
                            {
                                success++;
                                _imported_workloads.Add(new Workload() { hostname = fields[0], iplist = fields[1] });
                            }
                            else
                            {
                                failed++;
                            }
                        }
                        message.Text = String.Format("{0} workloads found", success);
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
