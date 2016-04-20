using MRMPConfigurator.MRMPWCFService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MRMPConfigurator.Converters
{
    public class platform : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MRPWCFServiceClient channel = new MRPWCFServiceClient();
            return channel.ListPlatforms().FirstOrDefault(x => x.id == (String)value).human_vendor;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}