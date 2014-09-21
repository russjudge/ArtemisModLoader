using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RussLibrary;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MissionStudio.Spacemap
{
    public class PropertyItem : UIElement
    {
        public static IList<PropertyItem> GetCommandProperties(string command, SpaceObjectType objectType)
        {
            List<PropertyItem> retVal = new List<PropertyItem>();

            //Get everything except Create and Destroy_near.
            if (command == "create")
            {
                //Convert objectType to enumeration.
               // SpaceObjectType obType = SpaceObjectType.Destroyer;

                retVal = new List<PropertyItem>(GetObjectProperties(objectType));
            }
            else
            {
                //objectType ignored.  Get attributes for command and create list from list of attributes.
                if (Commands.Current.CommandDictionary.ContainsKey(command))
                {
                    foreach (AttributeElement attrib in Commands.Current.CommandDictionary[command].Attributes)
                    {
                        PropertyItem p = new PropertyItem(attrib.Text, null);
                        retVal.Add(p);
                    }
                }
            }

            
            retVal.Sort(new PropertyItemComparer());
            
            return retVal;
        }
        class PropertyItemComparer : IComparer<PropertyItem>
        {

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.CompareTo(System.String)")]
            public int Compare(PropertyItem x, PropertyItem y)
            {
                if (x == null)
                {
                    if (y == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }

                }
                else
                {
                    if (y == null)
                    {
                        return -1;
                    }
                    else
                    {
                        return x.PropertyName.CompareTo(y.PropertyName);
                    }
                }
            }

        }

        public static IList<PropertyItem> GetObjectProperties(SpaceObjectType objectType)
        {
            List<PropertyItem> retVal = new List<PropertyItem>();
            retVal.Add(new PropertyItem("use_gm_position", null));
            retVal.Add(new PropertyItem("type", objectType.ToString()));
            switch (objectType)
            {
                case SpaceObjectType.Anomaly:
                case SpaceObjectType.BlackHole:
                    retVal.AddRange(GetStandardNamedList());
                    break;
                case SpaceObjectType.Monster:
                    retVal.AddRange(GetStandardNamedList());
                    retVal.Add(new PropertyItem("angle", null));
                    break;
                case SpaceObjectType.Enemy:
                case SpaceObjectType.Neutral:
                case SpaceObjectType.Station:
                    retVal.AddRange(GetVesselList());
                    retVal.Add(new PropertyItem("fleetnumber", null));
                    break;
                case SpaceObjectType.Player:
                      retVal.AddRange(GetVesselList());
                    
                    break;
                case SpaceObjectType.Whale:
                    retVal.AddRange(GetWhaleList());
                    break;
                case SpaceObjectType.GenericMesh:
                    retVal.AddRange(GetGenericMeshList());
                    break;
                case SpaceObjectType.Mines:
                case SpaceObjectType.Asteroids:
                case SpaceObjectType.Nebulas:
                    retVal.AddRange(GetUnnamedList());
                    break;
                case SpaceObjectType.Destroyer:
                    retVal.AddRange(PropertyItem.GetCommandProperties("destroy_near", SpaceObjectType.Destroyer));
                    break;
            }
            
            return retVal;
        }
        static IList<PropertyItem> GetGenericMeshList()
        {
            List<PropertyItem> retVal = new List<PropertyItem>(GetStandardNamedList());
            retVal.Add(new PropertyItem("meshFileName", null));
            retVal.Add(new PropertyItem("textureFileName", null));
            retVal.Add(new PropertyItem("hullType", null));
            retVal.Add(new PropertyItem("hullRace", null));
            retVal.Add(new PropertyItem("fakeShieldsFront", null));
            retVal.Add(new PropertyItem("fakeShieldsRear", null));
            retVal.Add(new PropertyItem("hasFakeShldFreq", null));
            retVal.Add(new PropertyItem("ColorRed", null));
            retVal.Add(new PropertyItem("ColorGreen", null));
            retVal.Add(new PropertyItem("ColorBlue", null));

            return retVal;
        }
        static IList<PropertyItem> GetWhaleList()
        {
            List<PropertyItem> retVal = new List<PropertyItem>(GetStandardNamedList());
            retVal.Add(new PropertyItem("podnumber", null));
            return retVal;
        }
        static IList<PropertyItem> GetStandardNamedList()
        {
            List<PropertyItem> retVal = new List<PropertyItem>();
            retVal.Add(new PropertyItem("x", null));
            retVal.Add(new PropertyItem("y", null));
            retVal.Add(new PropertyItem("z", null));
            retVal.Add(new PropertyItem("name", null));
            
            return retVal;
        }
        static IList<PropertyItem> GetVesselList()
        {
            List<PropertyItem> retVal = new List<PropertyItem>(GetStandardNamedList());
            retVal.Add(new PropertyItem("hullID", null));
            retVal.Add(new PropertyItem("hullKeys", null));
            retVal.Add(new PropertyItem("raceKeys", null));
            retVal.Add(new PropertyItem("angle", null));
            
            return retVal;
        }
        static IList<PropertyItem> GetUnnamedList()
        {
            List<PropertyItem> retVal = new List<PropertyItem>();
            retVal.Add(new PropertyItem("count", null));
            retVal.Add(new PropertyItem("radius", null));
            retVal.Add(new PropertyItem("randomRange", null));
            retVal.Add(new PropertyItem("startX", null));
            retVal.Add(new PropertyItem("startY", null));
            retVal.Add(new PropertyItem("startZ", null));
            retVal.Add(new PropertyItem("endX", null));
            retVal.Add(new PropertyItem("endY", null));
            retVal.Add(new PropertyItem("endZ", null));
            retVal.Add(new PropertyItem("randomSeed", null));
            retVal.Add(new PropertyItem("startAngle", null));
            retVal.Add(new PropertyItem("endAngle", null));
            return retVal;
        }
        public PropertyItem(string propertyName, string value)
        {
            PropertyName = propertyName;
            Value = value;
            bool matched = false;
            if (propertyName != "type")
            {
                foreach (CommandElement cmd in Commands.Current.CommandDictionary.Values)
                {

                    foreach (AttributeElement elem in cmd.Attributes)
                    {
                        if (elem.Text == propertyName)
                        {
                            if (elem.Values != null)
                            {
                                ValidChoices = new ObservableCollection<string>();
                                ValidChoices.Add(null);
                                foreach (XmlCompletionData val in elem.Values)
                                {
                                    ValidChoices.Add(val.Text);
                                }
                            }
                            matched = true;
                            break;
                        }
                    }
                    if (matched)
                    {
                        break;
                    }

                }
            }
        }
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string),
            typeof(PropertyItem));
        public string PropertyName
        {
            get
            {
                return (string)this.UIThreadGetValue(PropertyNameProperty);

            }
            set
            {
                this.UIThreadSetValue(PropertyNameProperty, value);

            }
        }
        static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PropertyItem me = sender as PropertyItem;
            if (me != null)
            {
                me.RaiseEvent(new RoutedEventArgs(PropertyItem.ValueChangedEvent));
            }
        }
        public static readonly DependencyProperty ValueProperty =
          DependencyProperty.Register("Value", typeof(string),
          typeof(PropertyItem), new PropertyMetadata(OnValueChanged));
        public string Value
        {
            get
            {
                return (string)this.UIThreadGetValue(ValueProperty);

            }
            set
            {
                this.UIThreadSetValue(ValueProperty, value);

            }
        }

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(PropertyItem));

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public static readonly DependencyProperty ValidChoicesProperty =
         DependencyProperty.Register("ValidChoices", typeof(ObservableCollection<string>),
         typeof(PropertyItem));

        public ObservableCollection<string> ValidChoices
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(ValidChoicesProperty);

            }
            private set
            {
                this.UIThreadSetValue(ValidChoicesProperty, value);

            }
        }

    }
}
