using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RussLibrary;
using RussLibrary.Xml;
using RussLibrary.WPF;
using System.Xml;
namespace VesselDataLibrary.Xml
{

    public class VectorObject : ChangeDependencyObject, IXmlStorage
    {
        public VectorObject()
        {
            Storage = new List<XmlNode>();
        }
        //x="-102.14" y="8.35" z="258.74"

  //      <torpedo_tube x="0" y="8.35" z="258.74"/>
  //<torpedo_tube x="0" y="8.35" z="258.74"/>

  //      <engine_port x=" 82.93" y="5" z="-240.89" />
  //<engine_port x="-82.93" y="5" z="-240.89" />
  //<engine_port x="0" y="-9.22" z="-300" />
  //<engine_port x="0" y="29.64" z="-300" />

        protected virtual void OnVectorItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
        protected static void OnVectorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ChangeDependencyObject.OnItemChanged(sender, e);
            VectorObject me = sender as VectorObject;
            if (me != null)
            {
                me.OnVectorItemChanged(me, e);
            }
        }
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double),
            typeof(VectorObject), new PropertyMetadata(OnVectorChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X"), XmlConversion("x")]
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
            typeof(VectorObject), new PropertyMetadata(OnVectorChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y"), XmlConversion("y")]
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



        public static readonly DependencyProperty ZProperty =
            DependencyProperty.Register("Z", typeof(double),
            typeof(VectorObject), new PropertyMetadata(OnVectorChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Z"), XmlConversion("z")]
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


        protected override void ProcessValidation()
        {
            
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<XmlNode> Storage { get; protected set; }
    }
}
