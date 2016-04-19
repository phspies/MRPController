using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MRMPConfigurator.Converters
{
    public class os_short : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string osedition = value.ToString();
                if (osedition.ToUpper().Contains("WIN"))
                {
                    return "windows";
                }
                else if (osedition.ToUpper().Contains("CENTOS"))
                {
                    return "centos";
                }
                else if (osedition.ToUpper().Contains("REDHAT"))
                {
                    return "redhat";
                }
                else if (osedition.ToUpper().Contains("UBUNTU"))
                {
                    return "ubuntu";
                }
                else if (osedition.ToUpper().Contains("SUSE"))
                {
                    return "suse";
                }
                return "windows";
            }
            else
            {
                return "windows";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}