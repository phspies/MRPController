using System.Collections.Generic;

namespace MRPService.CaaS
{

    class TemplateObject : Core
    {
        public TemplateObject(DimensionData _dimensiondata) : base(_dimensiondata) { }
        public OsImagesType platformtemplates(List<Option> options=null)
        {
            orgendpoint("/image/osImage");
            urloptions = options;
            OsImagesType softwarelabels = get<OsImagesType>(null, true) as OsImagesType;
            return softwarelabels;
        }
        public CustomerImagesType customertemplates(List<Option> options = null)
        {
            orgendpoint("/image/customerImage");
            urloptions = options;
            CustomerImagesType customerworkloadimages = get<CustomerImagesType>(null, true) as CustomerImagesType;
            return customerworkloadimages;
        }
    }
}
