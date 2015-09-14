using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMware.Vim;

namespace CloudMoveyWorkerService.VirtualCenter
{
    class Workloads : Core
    {
        public Workloads(VirtualCenter _virtualcenter) : base(_virtualcenter) {}
        public bool WorkloadList()
        {
            IList<EntityViewBase> vmList = _vmwarecontext.FindEntityViews(typeof(VirtualMachine), null, null, null);
            // Power off the virtual machines.
            foreach (VirtualMachine vm in vmList)
            {
                // Refresh the state of each view.
                vm.UpdateViewData();
                if (vm.Runtime.PowerState == VirtualMachinePowerState.poweredOn)
                {
                    vm.PowerOffVM();
                    Console.WriteLine("Stopped virtual machine: {0}", vm.Name);
                }
                else
                {
                    Console.WriteLine("Virtual machine {0} power state is: {1}", vm.Name, vm.Runtime.PowerState);
                }
            }
            return true;
        }
    }

}
