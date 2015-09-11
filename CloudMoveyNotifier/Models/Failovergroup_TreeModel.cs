using CloudMoveyNotifier.CloudMoveyWCF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyNotifier.Models
{
        public class Failovergroup_TreeModel : INotifyPropertyChanged
        {
            private string _id;
            public string id
            {
                get { return _id; }
                set { _id = value; OnPropertyChanged("id"); }
            }

            private string _parent_id;
            public string parent_id
            {
                get { return _parent_id; }
                set { _parent_id = value; OnPropertyChanged("parent_id"); }
            }

            private string _group;
            public String group
            {
                get { return _group; }
                set { _group = value; OnPropertyChanged("group"); }
            }

            private int? _group_type;
            public int? group_type
            {
                get { return _group_type; }
                set { _group_type = value; OnPropertyChanged("group_type"); }
            }

            private Failovergroup _parent;
            public Failovergroup parent
            {
                get { return _parent; }
                set { _parent = value; OnPropertyChanged("parent"); }
            }
            public Failovergroup group_object { get; set; }
            private ObservableCollection<Failovergroup_TreeModel> _children;
            public ObservableCollection<Failovergroup_TreeModel> children
            {
                get { return _children; }
                set { _children = value; OnPropertyChanged("children"); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(String propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
