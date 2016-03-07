using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MRPNotifier.Converters
{
    public class os_short : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string osedition = value.ToString();
            if (osedition.Contains("WIN"))
            {
                return "windows";
            }
            else if (osedition.Contains("CENTOS"))
            {
                return "centos";
            }
            else if (osedition.Contains("REDHAT"))
            {
                return "redhat";
            }
            else if (osedition.Contains("UBUNTU"))
            {
                return "ubuntu";
            }
            else if (osedition.Contains("SUSE"))
            {
                return "suse";
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}