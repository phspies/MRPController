using MRMPConfigurator.MRMPWCFService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MRMPConfigurator.Converters
{
    public class platform_description : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MRMPWCFServiceClient channel = new MRMPWCFServiceClient();
            return channel.ListPlatforms().FirstOrDefault(x => x.id == (String)value).description;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}