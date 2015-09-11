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
    /// Interaction logic for failovergroupform.xaml
    /// </summary>
    public partial class FailovergroupForm : MetroWindow
    {
        public Failovergroup _record = null;
        public FailovergroupForm(Failovergroup __record, int __action)
        {
            _record = __record;
            InitializeComponent();
            failovergroup_grouptype.ItemsSource = (new Failovergroups()).FailovergroupList;
            DataContext = _record;

            switch (__action)
            {
                case 0:
                    this.Title = "Add Failovergroup";
                    break;
                case 1:
                    this.Title = "Update Failovergroup";
                    break;
            }
        }

        private void accept_failovergroup_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void cancel_failovergroup_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
 
    }
}
