﻿namespace CloudMoveyWorkerService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CloudMoveyWorkerServiceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.eventLogInstaller1 = new System.Diagnostics.EventLogInstaller();
            // 
            // CloudMoveyWorkerServiceInstaller1
            // 
            this.CloudMoveyWorkerServiceInstaller1.Description = "CloudMovey client agent for task execution";
            this.CloudMoveyWorkerServiceInstaller1.DisplayName = "CloudMovey Worker Service";
            this.CloudMoveyWorkerServiceInstaller1.ServiceName = "CloudMovey Worker Service";
            this.CloudMoveyWorkerServiceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.CloudMoveyWorkerServiceInstaller1.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.CloudMoveyWorkerServiceInstaller1_AfterInstall);
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // eventLogInstaller1
            // 
            this.eventLogInstaller1.CategoryCount = 0;
            this.eventLogInstaller1.CategoryResourceFile = null;
            this.eventLogInstaller1.Log = "Application";
            this.eventLogInstaller1.MessageResourceFile = null;
            this.eventLogInstaller1.ParameterResourceFile = null;
            this.eventLogInstaller1.Source = "CloudMovey Worker Service";
            this.eventLogInstaller1.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.eventLogInstaller1_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.CloudMoveyWorkerServiceInstaller1,
            this.serviceProcessInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceInstaller CloudMoveyWorkerServiceInstaller1;
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.Diagnostics.EventLogInstaller eventLogInstaller1;
    }
}