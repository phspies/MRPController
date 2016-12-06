using System.Collections.Generic;

namespace MRMPService.Modules.MCP
{
    class TimeZones
    {
        List<TimeZone> _zones;

        public List<TimeZone> Zones
        {
            get
            {
                _zones = new List<TimeZone>();
                _zones.Add(new TimeZone()
                {
                    index = 000,
                    name = "Dateline Standard Time",
                    timezone = "(GMT-12:00) International Date Line West"
                });
                _zones.Add(new TimeZone()
                {
                    index = 001,
                    name = "Samoa Standard Time",
                    timezone = "(GMT-11:00) Midway Island, Samoa"
                });
                _zones.Add(new TimeZone()
                {
                    index = 002,
                    name = "Hawaiian Standard Time",
                    timezone = "(GMT-10:00) Hawaii"
                });
                _zones.Add(new TimeZone()
                {
                    index = 003,
                    name = "Alaskan Standard Time",
                    timezone = "(GMT-09:00) Alaska"
                });
                _zones.Add(new TimeZone()
                {
                    index = 004,
                    name = "Pacific Standard Time",
                    timezone = "(GMT-08:00) Pacific Time (US and Canada); Tijuana"
                });
                _zones.Add(new TimeZone()
                {
                    index = 010,
                    name = "Mountain Standard Time",
                    timezone = "(GMT-07:00) Mountain Time (US and Canada)"
                });
                _zones.Add(new TimeZone()
                {
                    index = 013,
                    name = "Mexico Standard Time 2",
                    timezone = "(GMT-07:00) Chihuahua, La Paz, Mazatlan"
                });
                _zones.Add(new TimeZone()
                {
                    index = 015,
                    name = "U.S. Mountain Standard Time",
                    timezone = "(GMT-07:00) Arizona"
                });
                _zones.Add(new TimeZone()
                {
                    index = 020,
                    name = "Central Standard Time",
                    timezone = "(GMT-06:00) Central Time (US and Canada"
                });
                _zones.Add(new TimeZone()
                {
                    index = 025,
                    name = "Canada Central Standard Time",
                    timezone = "(GMT-06:00) Saskatchewan"
                });
                _zones.Add(new TimeZone()
                {
                    index = 030,
                    name = "Mexico Standard Time",
                    timezone = "(GMT-06:00) Guadalajara, Mexico City, Monterrey"
                });
                _zones.Add(new TimeZone()
                {
                    index = 033,
                    name = "Central America Standard Time",
                    timezone = "(GMT-06:00) Central America"
                });
                _zones.Add(new TimeZone()
                {
                    index = 035,
                    name = "Eastern Standard Time",
                    timezone = "(GMT-05:00) Eastern Time (US and Canada)"
                });
                _zones.Add(new TimeZone()
                {
                    index = 040,
                    name = "U.S. Eastern Standard Time",
                    timezone = "(GMT-05:00) Indiana (East)"
                });
                _zones.Add(new TimeZone()
                {
                    index = 045,
                    name = "S.A. Pacific Standard Time",
                    timezone = "(GMT-05:00) Bogota, Lima, Quito"
                });
                _zones.Add(new TimeZone()
                {
                    index = 050,
                    name = "Atlantic Standard Time",
                    timezone = "(GMT-04:00) Atlantic Time (Canada)"
                });
                _zones.Add(new TimeZone()
                {
                    index = 055,
                    name = "S.A. Western Standard Time",
                    timezone = "(GMT-04:00) Caracas, La Paz"
                });
                _zones.Add(new TimeZone()
                {
                    index = 056,
                    name = "Pacific S.A. Standard Time",
                    timezone = "(GMT-04:00) Santiago"
                });
                _zones.Add(new TimeZone()
                {
                    index = 060,
                    name = "Newfoundland and Labrador Standard Time",
                    timezone = "(GMT-03:30) Newfoundland and Labrador"
                });
                _zones.Add(new TimeZone()
                {
                    index = 065,
                    name = "E. South America Standard Time",
                    timezone = "(GMT-03:00) Brasilia"
                });
                _zones.Add(new TimeZone()
                {
                    index = 070,
                    name = "S.A. Eastern Standard Time",
                    timezone = "(GMT-03:00) Buenos Aires, Georgetown"
                });
                _zones.Add(new TimeZone()
                {
                    index = 073,
                    name = "Greenland Standard Time",
                    timezone = "(GMT-03:00) Greenland"
                });
                _zones.Add(new TimeZone()
                {
                    index = 075,
                    name = "Mid-Atlantic Standard Time",
                    timezone = "(GMT-02:00) Mid-Atlantic"
                });
                _zones.Add(new TimeZone()
                {
                    index = 080,
                    name = "Azores Standard Time",
                    timezone = "(GMT-01:00) Azores"
                });
                _zones.Add(new TimeZone()
                {
                    index = 083,
                    name = "Cape Verde Standard Time",
                    timezone = "(GMT-01:00) Cape Verde Islands"
                });
                _zones.Add(new TimeZone()
                {
                    index = 085,
                    name = "GMT Standard Time",
                    timezone = "(GMT) Greenwich Mean Time: Dublin, Edinburgh, Lisbon, London"
                });
                _zones.Add(new TimeZone()
                {
                    index = 090,
                    name = "Greenwich Standard Time",
                    timezone = "(GMT) Casablanca, Monrovia"
                });
                _zones.Add(new TimeZone()
                {
                    index = 095,
                    name = "Central Europe Standard Time",
                    timezone = "(GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague"
                });
                _zones.Add(new TimeZone()
                {
                    index = 100,
                    name = "Central European Standard Time",
                    timezone = "(GMT+01:00) Sarajevo, Skopje, Warsaw, Zagreb"
                });
                _zones.Add(new TimeZone()
                {
                    index = 105,
                    name = "Romance Standard Time",
                    timezone = "(GMT+01:00) Brussels, Copenhagen, Madrid, Paris"
                });
                _zones.Add(new TimeZone()
                {
                    index = 110,
                    name = "W. Europe Standard Time",
                    timezone = "(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna"
                });
                _zones.Add(new TimeZone()
                {
                    index = 113,
                    name = "W. Central Africa Standard Time",
                    timezone = "(GMT+01:00) West Central Africa"
                });
                _zones.Add(new TimeZone()
                {
                    index = 115,
                    name = "E. Europe Standard Time",
                    timezone = "(GMT+02:00) Bucharest"
                });
                _zones.Add(new TimeZone()
                {
                    index = 120,
                    name = "Egypt Standard Time",
                    timezone = "(GMT+02:00) Cairo"
                });
                _zones.Add(new TimeZone()
                {
                    index = 125,
                    name = "FLE Standard Time",
                    timezone = "(GMT+02:00) Helsinki, Kiev, Riga, Sofia, Tallinn, Vilnius"
                });
                _zones.Add(new TimeZone()
                {
                    index = 130,
                    name = "GTB Standard Time",
                    timezone = "(GMT+02:00) Athens, Istanbul, Minsk"
                });
                _zones.Add(new TimeZone()
                {
                    index = 135,
                    name = "Israel Standard Time",
                    timezone = "(GMT+02:00) Jerusalem"
                });
                _zones.Add(new TimeZone()
                {
                    index = 140,
                    name = "South Africa Standard Time",
                    timezone = "(GMT+02:00) Harare, Pretoria"
                });
                _zones.Add(new TimeZone()
                {
                    index = 145,
                    name = "Russian Standard Time",
                    timezone = "(GMT+03:00) Moscow, St. Petersburg, Volgograd"
                });
                _zones.Add(new TimeZone()
                {
                    index = 150,
                    name = "Arab Standard Time",
                    timezone = "(GMT+03:00) Kuwait, Riyadh"
                });
                _zones.Add(new TimeZone()
                {
                    index = 155,
                    name = "E. Africa Standard Time",
                    timezone = "(GMT+03:00) Nairobi"
                });
                _zones.Add(new TimeZone()
                {
                    index = 158,
                    name = "Arabic Standard Time",
                    timezone = "(GMT+03:00) Baghdad"
                });
                _zones.Add(new TimeZone()
                {
                    index = 160,
                    name = "Iran Standard Time",
                    timezone = "(GMT+03:30) Tehran"
                });
                _zones.Add(new TimeZone()
                {
                    index = 165,
                    name = "Arabian Standard Time",
                    timezone = "(GMT+04:00) Abu Dhabi, Muscat"
                });
                _zones.Add(new TimeZone()
                {
                    index = 170,
                    name = "Caucasus Standard Time",
                    timezone = "(GMT+04:00) Baku, Tbilisi, Yerevan"
                });
                _zones.Add(new TimeZone()
                {
                    index = 175,
                    name = "Transitional Islamic State of Afghanistan Standard Time",
                    timezone = "(GMT+04:30) Kabul"
                });
                _zones.Add(new TimeZone()
                {
                    index = 180,
                    name = "Ekaterinburg Standard Time",
                    timezone = "(GMT+05:00) Ekaterinburg"
                });
                _zones.Add(new TimeZone()
                {
                    index = 185,
                    name = "West Asia Standard Time",
                    timezone = "(GMT+05:00) Islamabad, Karachi, Tashkent"
                });
                _zones.Add(new TimeZone()
                {
                    index = 190,
                    name = "India Standard Time",
                    timezone = "(GMT+05:30) Chennai, Kolkata, Mumbai, New Delhi"
                });
                _zones.Add(new TimeZone()
                {
                    index = 193,
                    name = "Nepal Standard Time",
                    timezone = "(GMT+05:45) Kathmandu"
                });
                _zones.Add(new TimeZone()
                {
                    index = 195,
                    name = "Central Asia Standard Time",
                    timezone = "(GMT+06:00) Astana, Dhaka"
                });
                _zones.Add(new TimeZone()
                {
                    index = 200,
                    name = "Sri Lanka Standard Time",
                    timezone = "(GMT+06:00) Sri Jayawardenepura"
                });
                _zones.Add(new TimeZone()
                {
                    index = 201,
                    name = "N. Central Asia Standard Time",
                    timezone = "(GMT+06:00) Almaty, Novosibirsk"
                });
                _zones.Add(new TimeZone()
                {
                    index = 203,
                    name = "Myanmar Standard Time",
                    timezone = "(GMT+06:30) Yangon Rangoon"
                });
                _zones.Add(new TimeZone()
                {
                    index = 206,
                    name = "S.E. Asia Standard Time",
                    timezone = "(GMT+07:00) Bangkok, Hanoi, Jakarta"
                });
                _zones.Add(new TimeZone()
                {
                    index = 207,
                    name = "North Asia Standard Time",
                    timezone = "(GMT+07:00) Krasnoyarsk"
                });
                _zones.Add(new TimeZone()
                {
                    index = 210,
                    name = "China Standard Time",
                    timezone = "(GMT+08:00) Beijing, Chongqing, Hong Kong SAR, Urumqi"
                });
                _zones.Add(new TimeZone()
                {
                    index = 215,
                    name = "Singapore Standard Time",
                    timezone = "(GMT+08:00) Kuala Lumpur, Singapore"
                });
                _zones.Add(new TimeZone()
                {
                    index = 220,
                    name = "Taipei Standard Time",
                    timezone = "(GMT+08:00) Taipei"
                });
                _zones.Add(new TimeZone()
                {
                    index = 225,
                    name = "W. Australia Standard Time",
                    timezone = "(GMT+08:00) Perth"
                });
                _zones.Add(new TimeZone()
                {
                    index = 227,
                    name = "North Asia East Standard Time",
                    timezone = "(GMT+08:00) Irkutsk, Ulaanbaatar"
                });
                _zones.Add(new TimeZone()
                {
                    index = 230,
                    name = "Korea Standard Time",
                    timezone = "(GMT+09:00) Seoul"
                });
                _zones.Add(new TimeZone()
                {
                    index = 235,
                    name = "Tokyo Standard Time",
                    timezone = "(GMT+09:00) Osaka, Sapporo, Tokyo"
                });
                _zones.Add(new TimeZone()
                {
                    index = 240,
                    name = "Yakutsk Standard Time",
                    timezone = "(GMT+09:00) Yakutsk"
                });
                _zones.Add(new TimeZone()
                {
                    index = 245,
                    name = "A.U.S. Central Standard Time",
                    timezone = "(GMT+09:30) Darwin"
                });
                _zones.Add(new TimeZone()
                {
                    index = 250,
                    name = "Cen. Australia Standard Time",
                    timezone = "(GMT+09:30) Adelaide"
                });
                _zones.Add(new TimeZone()
                {
                    index = 255,
                    name = "A.U.S. Eastern Standard Time",
                    timezone = "(GMT+10:00) Canberra, Melbourne, Sydney"
                });
                _zones.Add(new TimeZone()
                {
                    index = 260,
                    name = "E. Australia Standard Time",
                    timezone = "(GMT+10:00) Brisbane"
                });
                _zones.Add(new TimeZone()
                {
                    index = 265,
                    name = "Tasmania Standard Time",
                    timezone = "(GMT+10:00) Hobart"
                });
                _zones.Add(new TimeZone()
                {
                    index = 270,
                    name = "Vladivostok Standard Time",
                    timezone = "(GMT+10:00) Vladivostok"
                });
                _zones.Add(new TimeZone()
                {
                    index = 275,
                    name = "West Pacific Standard Time",
                    timezone = "(GMT+10:00) Guam, Port Moresby"
                });
                _zones.Add(new TimeZone()
                {
                    index = 280,
                    name = "Central Pacific Standard Time",
                    timezone = "(GMT+11:00) Magadan, Solomon Islands, New Caledonia"
                });
                _zones.Add(new TimeZone()
                {
                    index = 285,
                    name = "Fiji Islands Standard Time",
                    timezone = "(GMT+12:00) Fiji Islands, Kamchatka, Marshall Islands"
                });
                _zones.Add(new TimeZone()
                {
                    index = 290,
                    name = "New Zealand Standard Time",
                    timezone = "(GMT+12:00) Auckland, Wellington"
                });
                _zones.Add(new TimeZone()
                {
                    index = 300,
                    name = "Tonga Standard Time",
                    timezone = "(GMT+13:00) Nuku'alofa"
                });

                return _zones;
            }
        }
    }
    public class TimeZone
    {
        public int index { get; set; }
        public string name { get; set; }
        public string timezone { get; set; }
    }
}
