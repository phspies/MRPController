using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace CladesWorkerService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void DoubleTakeProxyInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void eventLogInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
