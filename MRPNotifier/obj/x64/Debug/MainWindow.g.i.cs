﻿#pragma checksum "..\..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A2C2C70E342FAEAF1BF94552AC0A4053"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MRPNotifier;
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
using System.Windows.Interactivity;
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


namespace MRPNotifier {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 12 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MRPNotifier.MainWindow Window;
        
        #line default
        #line hidden
        
        
        #line 345 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl tabControl;
        
        #line default
        #line hidden
        
        
        #line 347 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid ApplicationGroupGrid;
        
        #line default
        #line hidden
        
        
        #line 348 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox workload_filter_toggleswitch;
        
        #line default
        #line hidden
        
        
        #line 349 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox workload_search;
        
        #line default
        #line hidden
        
        
        #line 350 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvWorkloads;
        
        #line default
        #line hidden
        
        
        #line 392 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label2;
        
        #line default
        #line hidden
        
        
        #line 393 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label current_group;
        
        #line default
        #line hidden
        
        
        #line 395 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label;
        
        #line default
        #line hidden
        
        
        #line 399 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid PlatformGrid;
        
        #line default
        #line hidden
        
        
        #line 401 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvPlatforms;
        
        #line default
        #line hidden
        
        
        #line 403 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridView platform_grid;
        
        #line default
        #line hidden
        
        
        #line 445 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MahApps.Metro.Controls.ProgressRing progress_indicator;
        
        #line default
        #line hidden
        
        
        #line 463 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label progress_message;
        
        #line default
        #line hidden
        
        
        #line 467 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid CredentialGrid;
        
        #line default
        #line hidden
        
        
        #line 469 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvCredentials;
        
        #line default
        #line hidden
        
        
        #line 526 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvTasks;
        
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
            System.Uri resourceLocater = new System.Uri("/MRPNotifier;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MainWindow.xaml"
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
            this.Window = ((MRPNotifier.MainWindow)(target));
            
            #line 13 "..\..\..\MainWindow.xaml"
            this.Window.ContentRendered += new System.EventHandler(this.Window_ContentRendered);
            
            #line default
            #line hidden
            
            #line 16 "..\..\..\MainWindow.xaml"
            this.Window.Loaded += new System.Windows.RoutedEventHandler(this.OnWindowLoaded);
            
            #line default
            #line hidden
            
            #line 17 "..\..\..\MainWindow.xaml"
            this.Window.Closing += new System.ComponentModel.CancelEventHandler(this.OnClose);
            
            #line default
            #line hidden
            
            #line 17 "..\..\..\MainWindow.xaml"
            this.Window.StateChanged += new System.EventHandler(this.OnStateChanged);
            
            #line default
            #line hidden
            
            #line 17 "..\..\..\MainWindow.xaml"
            this.Window.IsVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(this.OnIsVisibleChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 315 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.copy_guid_button_clicked);
            
            #line default
            #line hidden
            return;
            case 3:
            this.tabControl = ((System.Windows.Controls.TabControl)(target));
            return;
            case 4:
            this.ApplicationGroupGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.workload_filter_toggleswitch = ((System.Windows.Controls.CheckBox)(target));
            
            #line 348 "..\..\..\MainWindow.xaml"
            this.workload_filter_toggleswitch.Checked += new System.Windows.RoutedEventHandler(this.workload_filter_toggle);
            
            #line default
            #line hidden
            
            #line 348 "..\..\..\MainWindow.xaml"
            this.workload_filter_toggleswitch.Unchecked += new System.Windows.RoutedEventHandler(this.workload_filter_toggle);
            
            #line default
            #line hidden
            return;
            case 6:
            this.workload_search = ((System.Windows.Controls.TextBox)(target));
            
            #line 349 "..\..\..\MainWindow.xaml"
            this.workload_search.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.workload_search_filter);
            
            #line default
            #line hidden
            return;
            case 7:
            this.lvWorkloads = ((System.Windows.Controls.ListView)(target));
            
            #line 350 "..\..\..\MainWindow.xaml"
            this.lvWorkloads.SizeChanged += new System.Windows.SizeChangedEventHandler(this.lvWorkloads_SizeChanged);
            
            #line default
            #line hidden
            return;
            case 10:
            this.label2 = ((System.Windows.Controls.Label)(target));
            return;
            case 11:
            this.current_group = ((System.Windows.Controls.Label)(target));
            return;
            case 12:
            this.label = ((System.Windows.Controls.Label)(target));
            return;
            case 13:
            this.PlatformGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 14:
            this.lvPlatforms = ((System.Windows.Controls.ListView)(target));
            
            #line 401 "..\..\..\MainWindow.xaml"
            this.lvPlatforms.SizeChanged += new System.Windows.SizeChangedEventHandler(this.lvPlatforms_SizeChanged);
            
            #line default
            #line hidden
            return;
            case 15:
            this.platform_grid = ((System.Windows.Controls.GridView)(target));
            return;
            case 19:
            this.progress_indicator = ((MahApps.Metro.Controls.ProgressRing)(target));
            return;
            case 20:
            
            #line 447 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.refresh_platforms_button_clicked);
            
            #line default
            #line hidden
            return;
            case 21:
            
            #line 455 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.add_platforms_button_clicked);
            
            #line default
            #line hidden
            return;
            case 22:
            this.progress_message = ((System.Windows.Controls.Label)(target));
            return;
            case 23:
            this.CredentialGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 24:
            this.lvCredentials = ((System.Windows.Controls.ListView)(target));
            
            #line 469 "..\..\..\MainWindow.xaml"
            this.lvCredentials.SizeChanged += new System.Windows.SizeChangedEventHandler(this.lvCredentials_SizeChanged);
            
            #line default
            #line hidden
            return;
            case 27:
            
            #line 506 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.refresh_credentials_button_clicked);
            
            #line default
            #line hidden
            return;
            case 28:
            
            #line 514 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.add_credentials_button_clicked);
            
            #line default
            #line hidden
            return;
            case 29:
            this.lvTasks = ((System.Windows.Controls.ListView)(target));
            
            #line 526 "..\..\..\MainWindow.xaml"
            this.lvTasks.SizeChanged += new System.Windows.SizeChangedEventHandler(this.lvTasks_SizeChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 8:
            
            #line 371 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.delete_workload_button);
            
            #line default
            #line hidden
            break;
            case 9:
            
            #line 378 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.refresh_workload_button);
            
            #line default
            #line hidden
            break;
            case 16:
            
            #line 417 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.update_platform_button);
            
            #line default
            #line hidden
            break;
            case 17:
            
            #line 424 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.delete_platform_button);
            
            #line default
            #line hidden
            break;
            case 18:
            
            #line 431 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.refresh_platform_button);
            
            #line default
            #line hidden
            break;
            case 25:
            
            #line 485 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.update_credential_button);
            
            #line default
            #line hidden
            break;
            case 26:
            
            #line 492 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.delete_credential_button);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

