namespace MRPService
{
    partial class MRPSvc
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
            this.MRPLog1 = new System.Diagnostics.EventLog();
            this.serviceController1 = new System.ServiceProcess.ServiceController();
            ((System.ComponentModel.ISupportInitialize)(this.MRPLog1)).BeginInit();
            // 
            // MRPLog1
            // 
            this.MRPLog1.Log = "Application";
            this.MRPLog1.Source = "CloudMRP Worker Service";
            this.MRPLog1.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(this.eventLog1_EntryWritten);
            // 
            // serviceController1
            // 
            this.serviceController1.ServiceName = "CloudMRPSvc";
            // 
            // MRPSvc
            // 
            this.ServiceName = "CloudMRP Worker Service";
            ((System.ComponentModel.ISupportInitialize)(this.MRPLog1)).EndInit();

        }

        #endregion

        private System.ServiceProcess.ServiceController serviceController1;
        public System.Diagnostics.EventLog MRPLog1;
    }
}
