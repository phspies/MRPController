﻿#pragma checksum "..\..\..\..\Forms\WorkloadForm - Copy.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2B17B701C7B91D07AB94D235044B6F53"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MRPNotifier.Forms;
using MRPNotifier.MRPWCFService;
using MRPNotifier.Models;
using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MRPNotifier.Forms {
    
    
    /// <summary>
    /// WorkloadForm
    /// </summary>
    public partial class WorkloadForm : MahApps.Metro.Controls.MetroWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 32 "..\..\..\..\Forms\WorkloadForm - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grid1;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\..\Forms\WorkloadForm - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox workload_hostname;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\..\Forms\WorkloadForm - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox workload_iplist;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\..\Forms\WorkloadForm - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox workload_credential;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\..\Forms\WorkloadForm - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock message;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\..\Forms\WorkloadForm - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button accept_platform;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\..\Forms\WorkloadForm - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cancel_platform;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MRPNotifier;component/forms/workloadform%20-%20copy.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Forms\WorkloadForm - Copy.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.grid1 = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.workload_hostname = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.workload_iplist = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.workload_credential = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 5:
            this.message = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.accept_platform = ((System.Windows.Controls.Button)(target));
            
            #line 59 "..\..\..\..\Forms\WorkloadForm - Copy.xaml"
            this.accept_platform.Click += new System.Windows.RoutedEventHandler(this.accept_platform_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.cancel_platform = ((System.Windows.Controls.Button)(target));
            
            #line 69 "..\..\..\..\Forms\WorkloadForm - Copy.xaml"
            this.cancel_platform.Click += new System.Windows.RoutedEventHandler(this.cancel_platform_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

