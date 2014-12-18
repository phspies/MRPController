namespace CladesWorkerService
{
    partial class CladesWorkerSvc
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
            this.CladesWorkerLog1 = new System.Diagnostics.EventLog();
            this.serviceController1 = new System.ServiceProcess.ServiceController();
            ((System.ComponentModel.ISupportInitialize)(this.CladesWorkerLog1)).BeginInit();
            // 
            // CladesWorkerLog1
            // 
            this.CladesWorkerLog1.Log = "Application";
            this.CladesWorkerLog1.Source = "Clades Worker Service";
            this.CladesWorkerLog1.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(this.eventLog1_EntryWritten);
            // 
            // serviceController1
            // 
            this.serviceController1.ServiceName = "DoubleTakeProxySvc";
            // 
            // CladesWorkerSvc
            // 
            this.ServiceName = "Double-Take JSON Service";
            ((System.ComponentModel.ISupportInitialize)(this.CladesWorkerLog1)).EndInit();

        }

        #endregion

        private System.ServiceProcess.ServiceController serviceController1;
        public System.Diagnostics.EventLog CladesWorkerLog1;
    }
}
