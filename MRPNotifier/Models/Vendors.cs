using System.Collections.Generic;

namespace MRPNotifier.Models
{
    class Vendors
    {
        public Vendors()
        {
            VendorList = new List<vendordetail>();
            VendorList.Add(new vendordetail() { ID = 0, Vendor = "Dimension Data" });
            VendorList.Add(new vendordetail() { ID = 1, Vendor = "VMWare" });
            VendorList.Add(new vendordetail() { ID = 2, Vendor = "Hyper-V" });
            VendorList.Add(new vendordetail() { ID = 3, Vendor = "Physical" });
        }
        public List<vendordetail> VendorList { get; set; }
    }

    class vendordetail
    {
        public int ID { get; set; }
        public string Vendor { get; set; }
    }
}
