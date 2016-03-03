using MRPService.VMWare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMware.Vim;

namespace VMWare
{
    class Program
    {
        static void Main(string[] args)
        {
            ApiClient VMWare = new ApiClient("https://192.168.0.152/sdk", @"lab\administrator", "Qwerty1234");
            Console.WriteLine("Datacenters -----------------");
            foreach (Datacenter dc in VMWare.datacenter().DatacenterList())
            {
                Console.WriteLine(String.Format("DC: {0} {1}", dc.Name, dc.MoRef.Value));
                Console.WriteLine("\n\tDatastores -----------------\n");
                foreach (Datastore ds in VMWare.datastore().DatastoreList(dc))
                {
                    Console.WriteLine(String.Format("\t\tDS: {0} {1}", ds.Name, ds.MoRef.Value));
                }
                Console.WriteLine("\n\tVirtual Machines -----------------\n");
                foreach (VirtualMachine vm in VMWare.workload().GetWorkloads(dc))
                {
                    Console.WriteLine(String.Format("\t\t{0} {1}", vm.Name, vm.MoRef.Value));
                }
                Console.WriteLine("\n\tNetworks -----------------\n");
                foreach (Network net in VMWare.networks().GetPortGroups(dc))
                {
                    Console.WriteLine(String.Format("\t\t{0} {1}", net.Name, net.MoRef.Value));
                }

            }




            Console.ReadKey();

        }
    }
}
