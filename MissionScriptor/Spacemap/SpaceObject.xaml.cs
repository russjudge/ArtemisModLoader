using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RussLibrary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace MissionStudio.Spacemap
{
    /// <summary>
    /// Interaction logic for SpaceObject.xaml
    /// </summary>
    public partial class SpaceObject : UserControl, IMappable
    {
        public SpaceObject()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double),
            typeof(SpaceObject), new PropertyMetadata(24D));

     
        public double ImageWidth
        {
            get
            {
                return (double)this.UIThreadGetValue(ImageWidthProperty);

            }
            set
            {
                this.UIThreadSetValue(ImageWidthProperty, value);

            }
        }


        public void XmlCommandUpdate(string tag)
        {
            //Go through each attribute and update Attributes.
            //example tag = <command attrib="x" attrib2="y" >
            List<string> attributes = new List<string>();
            //string.split will work, but need to remove spaces from within quote.
            if (!string.IsNullOrEmpty(tag))
            {
                int point = tag.IndexOf(" ");
                int i = -1;
                int j = -1;
                do
                {

                    i = tag.IndexOf("\"", point);
                    if (i > -1)
                    {
                        j = tag.IndexOf("\"", i + 1);
                        string attr = tag.Substring(point, i - point).Trim();
                        string val = tag.Substring(i + 1, j - i);
                        foreach (PropertyItem item in Attributes)
                        {
                            if (item.PropertyName == attr)
                            {
                                if (item.Value != val)
                                {
                                    item.Value = val;
                                }
                            }
                        }
                        point = tag.IndexOf(" ", j);
                    }
                } while (i > -1);
            }
        }


        public static readonly DependencyProperty AttributesProperty =
            DependencyProperty.Register("Attributes", typeof(ObservableCollection<PropertyItem>),
            typeof(SpaceObject));

        public ObservableCollection<PropertyItem> Attributes
        {
            get
            {
                return (ObservableCollection<PropertyItem>)this.UIThreadGetValue(AttributesProperty);

            }
            set
            {
                this.UIThreadSetValue(AttributesProperty, value);

            }
        }



        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double),
            typeof(SpaceObject), new PropertyMetadata(OnXChanged));

        /// <summary>
        /// Gets or sets the X, (x & width)
        /// </summary>
        /// <value>
        /// The X.
        /// </value>
        public double X
        {
            get
            {
                return (double)this.UIThreadGetValue(XProperty);

            }
            set
            {
                this.UIThreadSetValue(XProperty, value);

            }
        }


        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double),
            typeof(SpaceObject), new PropertyMetadata(OnYChanged));
        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>
        /// The Y.
        /// </value>
        public double Y
        {
            get
            {
                return (double)this.UIThreadGetValue(YProperty);

            }
            set
            {
                this.UIThreadSetValue(YProperty, value);

            }
        }
        
        static void OnXChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SpaceObject me = sender as SpaceObject;
            if (me != null)
            {
                string val = null;
                if (!double.IsNaN(me.X))
                {
                    val = me.X.ToString();
                }
                me.UpdatePropertyItem("x", val);
                me.UpdatePropertyItem("startX", val);
                me.UpdateUnnamedObjectAdorner();
            }
        }


        static void OnYChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SpaceObject me = sender as SpaceObject;
            if (me != null)
            {
                string val = null;
                if (!double.IsNaN(me.Y))
                {
                    val = me.Y.ToString();
                }
                me.UpdatePropertyItem("y", val);
                me.UpdatePropertyItem("startY", val);
                me.UpdateUnnamedObjectAdorner();
            }
        }

        static void OnZChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SpaceObject me = sender as SpaceObject;
            if (me != null)
            {
                string val = null;
                if (!double.IsNaN(me.Z))
                {
                    val = me.Z.ToString();
                }
                me.UpdatePropertyItem("z", val);
                me.UpdatePropertyItem("startZ", val);
                me.UpdateUnnamedObjectAdorner();
            }
        }
        public static readonly DependencyProperty ZProperty =
            DependencyProperty.Register("Z", typeof(double),
            typeof(SpaceObject), new PropertyMetadata(OnZChanged));
        /// <summary>
        /// Gets or sets the Z. (Height & top)
        /// </summary>
        /// <value>
        /// The Z.
        /// </value>
        public double Z
        {
            get
            {
                return (double)this.UIThreadGetValue(ZProperty);

            }
            set
            {
                this.UIThreadSetValue(ZProperty, value);

            }
        }
        void UpdatePropertyItem(string name, string value)
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                foreach (PropertyItem item in Attributes)
                {
                    if (item.PropertyName == name)
                    {
                        if (item.Value != value)
                        {
                            item.Value = value;
                        }
                        break;
                    }
                }
                IsUpdating = false;
            }
        }
        static void OnObjectNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SpaceObject me = sender as SpaceObject;
            if (me != null)
            {
                me.UpdatePropertyItem("name", me.ObjectName);
            }
        }
        public static readonly DependencyProperty ObjectNameProperty =
          DependencyProperty.Register("ObjectName", typeof(string),
          typeof(SpaceObject), new PropertyMetadata(OnObjectNameChanged));
        public string ObjectName
        {
            get
            {
                return (string)this.UIThreadGetValue(ObjectNameProperty);

            }
            set
            {
                this.UIThreadSetValue(ObjectNameProperty, value);

            }
        }
        static void OnObjectTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SpaceObject me = sender as SpaceObject;
            if (me != null)
            {
                me.Attributes = new ObservableCollection<PropertyItem>();
                List<PropertyItem> items = new List<PropertyItem>(PropertyItem.GetCommandProperties("create", me.ObjectType));
                foreach (PropertyItem item in items)
                {
                    item.ValueChanged += new RoutedEventHandler(me.OnValueChanged);
                    me.Attributes.Add(item);
                }
                if (me.ObjectType == SpaceObjectType.Asteroids || me.ObjectType == SpaceObjectType.Mines
                    || me.ObjectType == SpaceObjectType.Nebulas)
                {

                    me.UpdateUnnamedObjectAdorner();
                }
            }
        }
        void SetUnnamedObjectAdornerProperties(UnnamedObjectAdorner adorner)
        {
            foreach (PropertyInfo prop in adorner.GetType().GetProperties())
            {
                foreach (PropertyItem item in this.Attributes)
                {
                    if (item.PropertyName == prop.Name)
                    {
                        //item.Value;
                        if (prop.PropertyType == typeof(int))
                        {
                            int i = 0;
                            int.TryParse(item.Value, out i);
                            prop.SetValue(adorner, i, null);
                        }
                        else
                        {
                            prop.SetValue(adorner, item.Value, null);
                        }
                        break;
                    }
                }
            }
        }
        void UpdateUnnamedObjectAdorner()
        {
            AdornerLayer layr = AdornerLayer.GetAdornerLayer(this);
            bool FoundAdorner = false;
            if (layr != null)
            {
                Adorner[] adorners = layr.GetAdorners(this);
                if (adorners != null)
                {
                    foreach (Adorner adr in adorners)
                    {
                        UnnamedObjectAdorner uoa = adr as UnnamedObjectAdorner;
                        if (uoa != null)
                        {
                            //update all properties from attributes.
                            FoundAdorner = true;
                            SetUnnamedObjectAdornerProperties(uoa);
                           
                        }
                    }
                    if (!FoundAdorner)
                    {
                        UnnamedObjectAdorner adorner = new UnnamedObjectAdorner(this);
                        SetUnnamedObjectAdornerProperties(adorner);
                        layr.Add(adorner);
                    }
                }
            }
        }
        bool IsUpdating = false;
        void OnValueChanged(object sender, RoutedEventArgs e)
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                PropertyItem prop = sender as PropertyItem;
                if (prop != null)
                {
                    double val = 0;
                    bool changed = false;
                    if (!double.TryParse(prop.Value, out val))
                    {
                        val = double.NaN;
                    }

                    switch (prop.PropertyName)
                    {
                        case "startX":
                        case "x":
                            if (X != val)
                            {
                                X = val;
                                changed = true;
                            }
                            break;
                        case "startY":
                        case "y":
                            if (Y != val)
                            {
                                Y = val;
                                changed = true;
                            }
                            break;
                        case "startZ":
                        case "z":
                            if (Z != val)
                            {
                                Z = val;
                                changed = true;
                            }
                            break;
                        case "name":
                            if (ObjectName != prop.Value)
                            {
                                ObjectName = prop.Value;
                            }
                            break;

                    }
                    if (changed)
                    {
                        if (LocationChanged != null)
                        {
                            LocationChanged(this, EventArgs.Empty);
                        }
                    }
                    UpdateUnnamedObjectAdorner();
                }
                IsUpdating = false;
            }
        }
        public event EventHandler LocationChanged;
        public static readonly DependencyProperty ObjectTypeProperty =
          DependencyProperty.Register("ObjectType", typeof(SpaceObjectType),
          typeof(SpaceObject), new PropertyMetadata(OnObjectTypeChanged));
        public SpaceObjectType ObjectType
        {
            get
            {
                return (SpaceObjectType)this.UIThreadGetValue(ObjectTypeProperty);

            }
            set
            {
                this.UIThreadSetValue(ObjectTypeProperty, value);

            }
        }



        System.Collections.IList IMappable.Attributes
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

    }
}
