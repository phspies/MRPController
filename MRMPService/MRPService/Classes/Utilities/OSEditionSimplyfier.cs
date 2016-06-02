namespace MRMPService.Utilities
{
    class OSEditionSimplyfier
    {
        public static string Simplyfier(string _longname)
        {
            string _simple_osedition="";
            if (_longname.Contains("Windows"))
            {
                _simple_osedition += "WIN";

                if (_longname.Contains("2008"))
                {
                    _simple_osedition += "2008";
                }
                else if (_longname.Contains("2012"))
                {
                    _simple_osedition += "2012";
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
                _simple_osedition += _longname.Split(' ')[3];
                if (_longname.Contains("64"))
                {
                    _simple_osedition += "/64";
                }
                else if (_longname.Contains("32"))
                {
                    _simple_osedition += "/32";
                }
            }
            else if (_longname.Contains("Red Hat"))
            {
                _simple_osedition += "REDHAT";
                _simple_osedition += _longname.Split(' ')[4];
                if (_longname.Contains("64"))
                {
                    _simple_osedition += "/64";
                }
                else if (_longname.Contains("32"))
                {
                    _simple_osedition += "/32";
                }
            }
            else if (_longname.Contains("Centos"))
            {
                _simple_osedition += "CENTOS";
                _simple_osedition += _longname.Split(' ')[1];
                if (_longname.Contains("64"))
                {
                    _simple_osedition += "/64";
                }
                else if (_longname.Contains("32"))
                {
                    _simple_osedition += "/32";
                }
            }
            else if (_longname.Contains("Ubuntu"))
            {
                _simple_osedition += "UBUNTU";
                _simple_osedition += "14";
                if (_longname.Contains("64"))
                {
                    _simple_osedition += "/64";
                }
                else if (_longname.Contains("32"))
                {
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
