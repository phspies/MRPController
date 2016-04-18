using System;
using System.Windows.Data;

namespace MRPNotifier.Converters
{
    public class osedition_convert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string osedition = value.ToString();
                if (osedition.Contains("/"))
                {
                    string major, minor, edition, edition_size;
                    major = minor = edition = edition_size = "";
                    string cpu = osedition.Split('/')[1];
                    if (osedition.Contains("WIN"))
                    {
                        major = "Windows";
                        if (osedition.Contains("2008"))
                        {
                            minor = "2008";
                        }
                        else if (osedition.Contains("2012"))
                        {
                            minor = "2012";
                        }
                        if (osedition.Contains("R2"))
                        {
                            edition = "R2";
                        }
                        if (osedition.Contains("DC"))
                        {
                            edition_size = "Datacenter";
                        }
                        else if (osedition.Contains("E"))
                        {
                            edition_size = "Enterprise";
                        }
                        else if (osedition.Contains("S"))
                        {
                            edition_size = "Standard";
                        }
                    }
                    else if (osedition.Contains("CENTOS"))
                    {
                        major = "Centos";
                        minor = osedition.Split('/')[0].Replace("CENTOS", "");
                    }
                    else if (osedition.Contains("REDHAT"))
                    {
                        major = "Redhat";
                        minor = osedition.Split('/')[0].Replace("REDHAT", "");
                    }
                    else if (osedition.Contains("UBUNTU"))
                    {
                        major = "Ubuntu";
                        minor = osedition.Split('/')[0].Replace("UBUNTU", "");
                    }
                    else if (osedition.Contains("SUSE"))
                    {
                        major = "Suse";
                        minor = osedition.Split('/')[0].Replace("SUSE", "");
                    }
                    string returnval = "";
                    if (major.ToLower() == "windows")
                    {
                        returnval = String.Format("{0} {1} {2} {3} {4}bit Edition", major, minor, edition_size, edition, cpu);
                    }
                    else if (major.ToLower() == "unix")
                    {
                        returnval = String.Format("{0} {1} {2}bit Edition", major, minor, cpu);
                    }
                    return returnval;

                }
                else
                {
                    return "Windows";
                }
            }
            else
            {
                return "Windows";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
