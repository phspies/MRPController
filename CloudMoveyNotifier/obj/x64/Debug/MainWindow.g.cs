﻿#pragma checksum "..\..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B4CB6469BB577394109EEDE5005D4DD9"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CloudMoveyNotifier;
using CloudMoveyNotifier.Models;
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


namespace CloudMoveyNotifier {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 12 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CloudMoveyNotifier.MainWindow Window;
        
        #line default
        #line hidden
        
        
        #line 361 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl tabControl;
        
        #line default
        #line hidden
        
        
        #line 363 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid ApplicationGroupGrid;
        
        #line default
        #line hidden
        
        
        #line 364 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox workload_filter_toggleswitch;
        
        #line default
        #line hidden
        
        
        #line 365 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox workload_search;
        
        #line default
        #line hidden
        
        
        #line 367 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView Failovergroup_treeview;
        
        #line default
        #line hidden
        
        
        #line 410 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvWorkloads;
        
        #line default
        #line hidden
        
        
        #line 452 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label2;
        
        #line default
        #line hidden
        
        
        #line 454 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label;
        
        #line default
        #line hidden
        
        
        #line 458 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid PlatformGrid;
        
        #line default
        #line hidden
        
        
        #line 460 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvPlatforms;
        
        #line default
        #line hidden
        
        
        #line 462 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridView platform_grid;
        
        #line default
        #line hidden
        
        
        #line 504 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MahApps.Metro.Controls.ProgressRing progress_indicator;
        
        #line default
        #line hidden
        
        
        #line 522 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label progress_message;
        
        #line default
        #line hidden
        
        
        #line 526 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid CredentialGrid;
        
        #line default
        #line hidden
        
        
        #line 528 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvCredentials;
        
        #line default
        #line hidden
        
        
        #line 585 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvEvents;
        
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
            System.Uri resourceLocater = new System.Uri("/CloudMoveyNotifier;component/mainwindow.xaml", System.UriKind.Relative);
            
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
            this.Window = ((CloudMoveyNotifier.MainWindow)(target));
            
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
            case 3:
            
            #line 205 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.Failovergroup_add_click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 206 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.Failovergroup_update_click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 207 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.Failovergroup_destroy_click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 209 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.Failovergroup_moveup_click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 210 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.Failovergroup_movedown_click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 331 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.copy_guid_button_clicked);
            
            #line default
            #line hidden
            return;
            case 9:
            this.tabControl = ((System.Windows.Controls.TabControl)(target));
            return;
            case 10:
            this.ApplicationGroupGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 11:
            this.workload_filter_toggleswitch = ((System.Windows.Controls.CheckBox)(target));
            
            #line 364 "..\..\..\MainWindow.xaml"
            this.workload_filter_toggleswitch.Checked += new System.Windows.RoutedEventHandler(this.workload_filter_toggle);
            
            #line default
            #line hidden
            
            #line 364 "..\..\..\MainWindow.xaml"
            this.workload_filter_toggleswitch.Unchecked += new System.Windows.RoutedEventHandler(this.workload_filter_toggle);
            
            #line default
            #line hidden
            return;
            case 12:
            this.workload_search = ((System.Windows.Controls.TextBox)(target));
            
            #line 365 "..\..\..\MainWindow.xaml"
            this.workload_search.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.workload_search_filter);
            
            #line default
            #line hidden
            return;
            case 13:
            this.Failovergroup_treeview = ((System.Windows.Controls.TreeView)(target));
            
            #line 367 "..\..\..\MainWindow.xaml"
            this.Failovergroup_treeview.DragOver += new System.Windows.DragEventHandler(this.Tree_DragOver);
            
            #line default
            #line hidden
            
            #line 367 "..\..\..\MainWindow.xaml"
            this.Failovergroup_treeview.Drop += new System.Windows.DragEventHandler(this.Tree_Drop);
            
            #line default
            #line hidden
            
            #line 367 "..\..\..\MainWindow.xaml"
            this.Failovergroup_treeview.DragLeave += new System.Windows.DragEventHandler(this.Tree_DragLeave);
            
            #line default
            #line hidden
            
            #line 367 "..\..\..\MainWindow.xaml"
            this.Failovergroup_treeview.SelectedItemChanged += new System.Windows.RoutedPropertyChangedEventHandler<object>(this.failovergroup_treeview_selectionchanged);
            
            #line default
            #line hidden
            return;
            case 14:
            this.lvWorkloads = ((System.Windows.Controls.ListView)(target));
            
            #line 410 "..\..\..\MainWindow.xaml"
            this.lvWorkloads.SizeChanged += new System.Windows.SizeChangedEventHandler(this.lvWorkloads_SizeChanged);
            
            #line default
            #line hidden
            return;
            case 17:
            this.label2 = ((System.Windows.Controls.Label)(target));
            return;
            case 18:
            this.label = ((System.Windows.Controls.Label)(target));
            return;
            case 19:
            this.PlatformGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 20:
            this.lvPlatforms = ((System.Windows.Controls.ListView)(target));
            
            #line 460 "..\..\..\MainWindow.xaml"
            this.lvPlatforms.SizeChanged += new System.Windows.SizeChangedEventHandler(this.lvPlatforms_SizeChanged);
            
            #line default
            #line hidden
            return;
            case 21:
            this.platform_grid = ((System.Windows.Controls.GridView)(target));
            return;
            case 25:
            this.progress_indicator = ((MahApps.Metro.Controls.ProgressRing)(target));
            return;
            case 26:
            
            #line 506 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.refresh_platforms_button_clicked);
            
            #line default
            #line hidden
            return;
            case 27:
            
            #line 514 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.add_platforms_button_clicked);
            
            #line default
            #line hidden
            return;
            case 28:
            this.progress_message = ((System.Windows.Controls.Label)(target));
            return;
            case 29:
            this.CredentialGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 30:
            this.lvCredentials = ((System.Windows.Controls.ListView)(target));
            
            #line 528 "..\..\..\MainWindow.xaml"
            this.lvCredentials.SizeChanged += new System.Windows.SizeChangedEventHandler(this.lvCredentials_SizeChanged);
            
            #line default
            #line hidden
            return;
            case 33:
            
            #line 565 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.refresh_credentials_button_clicked);
            
            #line default
            #line hidden
            return;
            case 34:
            
            #line 573 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.add_credentials_button_clicked);
            
            #line default
            #line hidden
            return;
            case 35:
            this.lvEvents = ((System.Windows.Controls.ListView)(target));
            
            #line 585 "..\..\..\MainWindow.xaml"
            this.lvEvents.SizeChanged += new System.Windows.SizeChangedEventHandler(this.lvWorkloads_SizeChanged);
            
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
            case 2:
            
            #line 168 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Viewbox)(target)).PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.lvWorkloads_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 168 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Viewbox)(target)).PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.lvWorkloads_MouseMove);
            
            #line default
            #line hidden
            break;
            case 15:
            
            #line 431 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.delete_workload_button);
            
            #line default
            #line hidden
            break;
            case 16:
            
            #line 438 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.refresh_workload_button);
            
            #line default
            #line hidden
            break;
            case 22:
            
            #line 476 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.update_platform_button);
            
            #line default
            #line hidden
            break;
            case 23:
            
            #line 483 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.delete_platform_button);
            
            #line default
            #line hidden
            break;
            case 24:
            
            #line 490 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.refresh_platform_button);
            
            #line default
            #line hidden
            break;
            case 31:
            
            #line 544 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.update_credential_button);
            
            #line default
            #line hidden
            break;
            case 32:
            
            #line 551 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.delete_credential_button);
            
            #line default
            #line hidden
            break;
            case 36:
            
            #line 605 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.delete_workload_button);
            
            #line default
            #line hidden
            break;
            case 37:
            
            #line 612 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.refresh_workload_button);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

