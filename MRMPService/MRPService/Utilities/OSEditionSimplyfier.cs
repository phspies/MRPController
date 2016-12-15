using System;
using System.Linq;

namespace MRMPService.Utilities
{
    class OSEditionSimplyfier
    {
        public static string Simplyfier(string _longname)
        {
            string _simple_osedition = "";
            if (_longname.Contains("Windows"))
            {
                _simple_osedition += "WIN";
                if (_longname.Contains("2003"))
                {
                    _simple_osedition += "2003";
                }
                else if (_longname.Contains("2008"))
                {
                    _simple_osedition += "2008";
                }
                else if (_longname.Contains("2012"))
                {
                    _simple_osedition += "2012";
                }
                else if (_longname.Contains(" 7 "))
                {
                    _simple_osedition += "7";
                }
                if (_longname.Contains("R2"))
                {
                    _simple_osedition += "R2";
                }
                if (_longname.Contains("Standard"))
                {
                    _simple_osedition += "S";
                }
                if (_longname.Contains("Enterprise"))
                {
                    _simple_osedition += "E";
                }
                else if (_longname.Contains("Datacenter"))
                {
                    _simple_osedition += "DC";
                }

                if (_longname.Contains("64"))
                {
                    _simple_osedition += "/64";
                }
                else
                {
                    _simple_osedition += "/32";
                }
            }
            else if (_longname.Contains("SUSE"))
            {
                _simple_osedition += "SUSE";
                var _version = String.Join("", _longname.Where(c => Char.IsDigit(c) || c == '.'));
                if (_version.Contains("."))
                {
                    _simple_osedition += _version.Split('.')[0];
                }
                else
                {
                    _simple_osedition += _version;
                }
                if (_longname.Contains("64"))
                {
                    _simple_osedition = _simple_osedition.Replace("64", "");
                    _simple_osedition += "/64";
                }
                else if (_longname.Contains("32"))
                {
                    _simple_osedition = _simple_osedition.Replace("32", "");
                    _simple_osedition += "/32";
                }
            }
            else if (_longname.Contains("Red Hat"))
            {
                _simple_osedition += "REDHAT";
                var _version = String.Join("", _longname.Where(c => Char.IsDigit(c) || c == '.'));
                if (_version.Contains("."))
                {
                    _simple_osedition += _version.Split('.')[0];
                }
                else
                {
                    _simple_osedition += _version;
                }

                if (_longname.Contains("64"))
                {
                    _simple_osedition = _simple_osedition.Replace("64", "");
                    _simple_osedition += "/64";
                }
                else if (_longname.Contains("32"))
                {
                    _simple_osedition = _simple_osedition.Replace("32", "");
                    _simple_osedition += "/32";
                }
            }
            else if (_longname.Contains("CentOS"))
            {
                _simple_osedition += "CENTOS";
                var _version = String.Join("", _longname.Where(c => Char.IsDigit(c) || c == '.'));
                if (_version.Contains("."))
                {
                    _simple_osedition += _version.Split('.')[0];
                }
                else
                {
                    _simple_osedition += _version;
                }
                if (_longname.Contains("64"))
                {
                    _simple_osedition = _simple_osedition.Replace("64", "");
                    _simple_osedition += "/64";
                }
                else if (_longname.Contains("32"))
                {
                    _simple_osedition = _simple_osedition.Replace("32", "");
                    _simple_osedition += "/32";
                }
            }
            else if (_longname.Contains("Ubuntu"))
            {
                _simple_osedition += "UBUNTU";
                var _version = String.Join("", _longname.Where(c => Char.IsDigit(c) || c == '.'));
                if (_version.Contains("."))
                {
                    _simple_osedition += _version.Split('.')[0];
                }
                else
                {
                    _simple_osedition += _version;
                }
                if (_longname.Contains("64"))
                {
                    _simple_osedition = _simple_osedition.Replace("64", "");
                    _simple_osedition += "/64";
                }
                else if (_longname.Contains("32"))
                {
                    _simple_osedition = _simple_osedition.Replace("32", "");
                    _simple_osedition += "/32";
                }
            }
            else
            {
                _simple_osedition = "Unknown";
            }

            return _simple_osedition;

        }
    }
}
