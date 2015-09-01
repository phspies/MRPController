using CloudMoveyNotifier.CloudMoveyWCF;
using CloudMoveyNotifier.Models;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CloudMoveyNotifier.Forms
{
    /// <summary>
    /// Interaction logic for credentialform.xaml
    /// </summary>
    public partial class CredentialForm : MetroWindow
    {
        public Credential _record = null;
        public CredentialForm(Credential __record, int __action)
        {
            _record = __record;
            InitializeComponent();
            credential_type.ItemsSource = (new CredentialTypes()).CredentialTypeList;
            DataContext = _record;

            switch (__action)
            {
                case 0:
                    this.Title = "Add Credential";
                    break;
                case 1:
                    this.Title = "Update Credential";
                    break;
            }
        }

        private void accept_credential_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void cancel_credential_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
