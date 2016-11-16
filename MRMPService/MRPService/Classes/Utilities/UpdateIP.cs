using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRPService.Classes.Utilities
{
    class UpdateIP
    {
        public void setIP(string IPAddress, string SubnetMask, string Gateway)
        {

            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();


            foreach (ManagementObject objMO in objMOC)
            {

                if ((bool)objMO["IPEnabled"])
                {
                    try
                    {
                        ManagementBaseObject objNewIP = null;
                        ManagementBaseObject objSetIP = null;
                        ManagementBaseObject objNewGate = null;


                        objNewIP = objMO.GetMethodParameters("EnableStatic");
                        objNewGate = objMO.GetMethodParameters("SetGateways");



                        //Set DefaultGateway
                        objNewGate["DefaultIPGateway"] = new string[] { Gateway };
                        objNewGate["GatewayCostMetric"] = new int[] { 1 };


                        //Set IPAddress and Subnet Mask
                        objNewIP["IPAddress"] = new string[] { IPAddress };
                        objNewIP["SubnetMask"] = new string[] { SubnetMask };

                        objSetIP = objMO.InvokeMethod("EnableStatic", objNewIP, null);
                        objSetIP = objMO.InvokeMethod("SetGateways", objNewGate, null);



                        Console.WriteLine(
                           "Updated IPAddress, SubnetMask and Default Gateway!");



                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

        }
    }
}
