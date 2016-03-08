using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMware.Vim;

namespace MRPService.VMWare
{
    public class Core
    {
        public VimClientImpl _vmwarecontext;
        
        public Core(VimApiClient _api_client)
        {
            _vmwarecontext = new VimClientImpl();

            ServiceContent sc = _vmwarecontext.Connect(_api_client.URL);
            UserSession us = _vmwarecontext.Login(_api_client.Username, _api_client.Password);
        }
    }
}


public static List<com.vmware.vim25.VirtualDisk> getVmDiskInfo(com.vmware.utils.VMwareConnection conn, String vmIP)
{
    List<com.vmware.vim25.VirtualDisk> resultList = new ArrayList<com.vmware.vim25.VirtualDisk>();
    com.vmware.vim25.VimPortType vimPort = conn.getVimPort();
    com.vmware.vim25.ServiceContent serviceContent = conn.getServiceContent();
    //  find a virtual machine by IP address (in all datacenters)
    com.vmware.vim25.ManagedObjectReference vmMoRef = vimPort.findByIp(serviceContent.getSearchIndex(), null, vmIP, true);
    if ((vmMoRef == null))
    {
        throw new Exception(("Unable to find the virtual machine: " + vmIP));
    }

    //  retrieve the list of virtual devices associated with the virtual machine
    com.vmware.vim25.ObjectContent vmContent = conn.findObject(vmMoRef, "config.hardware.device");
    if ((vmContent == null))
    {
        throw new Exception("Unable to find the virtual machine device list");
    }

    List<com.vmware.vim25.DynamicProperty> dps = vmContent.getPropSet();
    foreach (com.vmware.vim25.DynamicProperty dp in dps)
    {
        List<com.vmware.vim25.VirtualDevice> deviceList = ((com.vmware.vim25.ArrayOfVirtualDevice)(dp.getVal())).getVirtualDevice();
        //  go through the list of virtual devices to get the virtual disks
        foreach (com.vmware.vim25.VirtualDevice device in deviceList)
        {
            if ((device is com.vmware.vim25.VirtualDisk))
            {
                com.vmware.vim25.VirtualDisk disk = ((com.vmware.vim25.VirtualDisk)(device));
                resultList.add(disk);
            }

        }

    }

    return resultList;
}

public static void main(String[] args)
{
    if ((args.length != 4))
    {
        System.out.println("Wrong number of arguments, must provide four arguments:");
        System.out.println("[1] The server name or IP address");
        System.out.println("[2] The user name to log in as");
        System.out.println("[3] The password to use");
        System.out.println("[4] The virtual machine IP address.");
        System.exit(1);
    }

    //  handle input info
    String serverName = args[0];
    String userName = args[1];
    String password = args[2];
    String vmIP = args[3];
    com.vmware.utils.VMwareConnection conn = null;
    try
    {
        //  Step-1 Create a connection to vCenter, using the name, user, and password
        conn = new com.vmware.utils.VMwareConnection(serverName, userName, password);
        //  Step-2 get the list of virtual disk for a given VM
        List<com.vmware.vim25.VirtualDisk> vmDiskList = GetVMDiskInfo.getVmDiskInfo(conn, vmIP);
        //  Step-3 output the virtual disk info
        GetVMDiskInfo.displayVmDiskInfo(vmIP, vmDiskList);
    }
    catch (Exception e)
    {
        e.printStackTrace();
    }
    finally
    {
        //  close connection to vCenter
        if ((conn != null))
        {
            conn.close();
        }

    }

}

private static void displayVmDiskInfo(String vmIP, List<com.vmware.vim25.VirtualDisk> vmDiskList)
{
    System.out.printf("Here follows the virtual disks that belongs to the Virtual Machine: %s%n", vmIP);
    foreach (com.vmware.vim25.VirtualDisk virtualDisk in vmDiskList)
    {
        com.vmware.vim25.Description deviceInfo = virtualDisk.getDeviceInfo();
        String diskName = deviceInfo.getLabel();
        String capacity = deviceInfo.getSummary();
        System.out.printf("%s - capacity: %s%n", diskName, capacity);
    }

}