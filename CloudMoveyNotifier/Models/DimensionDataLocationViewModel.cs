using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyNotifier.Models
{
    public class DimensionDataLocations
    {
        public DimensionDataLocations()
        {
            LocationList = new ObservableCollection<Location>();
            LocationList.Add(new Location() { Description = "North America (NA)", Url = "https://api-na.dimensiondata.com" });
            LocationList.Add(new Location() { Description = "Europe (EU)", Url = "https://api-eu.dimensiondata.com" });
            LocationList.Add(new Location() { Description = "Africa (AF)", Url = "https://api-mea.dimensiondata.com" });
            LocationList.Add(new Location() { Description = "Australia (AU)", Url = "https://api-au.dimensiondata.com" });
            LocationList.Add(new Location() { Description = "Asia Pacific (AP)", Url = "https://api-ap.dimensiondata.com" });
            LocationList.Add(new Location() { Description = "South America (SA)", Url = "https://api-latam.dimensiondata.com" });
            LocationList.Add(new Location() { Description = "Canada(CA)", Url = "https://api-canada.dimensiondata.com" });
        }
        public ObservableCollection<Location> LocationList { get; set; }
    }

    public class Location
    {
        public string Url { get; set; }
        public string Description { get; set; }
    }
    //class DimensionDataLocationViewModel : INotifyPropertyChanged
    //{
    //    public ObservableCollection<Location> LocationList { get; set; }
    //    public DimensionDataLocationViewModel()
    //    {
    //        LocationList = new ObservableCollection<Location>();
    //        LocationList.Add(new Location() { Description = "North America (NA)", Url = "https://api-na.dimensiondata.com" });
    //        LocationList.Add(new Location() { Description = "Europe (EU)", Url = "https://api-eu.dimensiondata.com" });
    //        LocationList.Add(new Location() { Description = "Africa (AF)", Url = "https://api-mea.dimensiondata.com" });
    //        LocationList.Add(new Location() { Description = "Australia (AU)", Url = "https://api-au.dimensiondata.com" });
    //        LocationList.Add(new Location() { Description = "Asia Pacific (AP)", Url = "https://api-ap.dimensiondata.com" });
    //        LocationList.Add(new Location() { Description = "South America (SA)", Url = "https://api-latam.dimensiondata.com" });
    //        LocationList.Add(new Location() { Description = "Canada(CA)", Url = "https://api-canada.dimensiondata.com" });
    //    }
    //    private Location selectedLocation;
    //    public Location SelectedLocation
    //    {
    //        get { return selectedLocation; }
    //        set
    //        {
    //            selectedLocation = value;
    //            OnPropertyChanged("SelectedLocation");
    //        }
    //    }
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    protected void OnPropertyChanged(string name)
    //    {
    //        PropertyChangedEventHandler handler = PropertyChanged;
    //        if (handler != null)
    //        {
    //            handler(this, new PropertyChangedEventArgs(name));
    //        }
    //    }
    //}

}
