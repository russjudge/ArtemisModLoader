using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RussLibrary.Xml;
using RussLibrary.WPF;
using System.Xml;
using System.Windows;
using RussLibrary;
namespace DMXCommander.Xml
{
    [XmlConversionRoot("timeblock")]
    public class TimeBlock : ChangeDependencyObject, IXmlStorage
    {

        public TimeBlock()
        {
            Storage = new List<XmlNode>();
            Values = new SetValueCollection();
            Values.ObjectChanged += Values_ObjectChanged;
            
        }

        void Values_ObjectChanged(object sender, EventArgs e)
        {
            SetChanged();
        }


        public static readonly DependencyProperty MillisecondsProperty =
            DependencyProperty.Register("Milliseconds", typeof(int),
            typeof(TimeBlock), new PropertyMetadata(OnItemChanged));
        [XmlConversion("mseconds")]
        public int Milliseconds
        {
            get
            {
                return (int)this.UIThreadGetValue(MillisecondsProperty);

            }
            set
            {
                this.UIThreadSetValue(MillisecondsProperty, value);

            }
        }


        public static readonly DependencyProperty ValuesProperty =
          DependencyProperty.Register("Values", typeof(SetValueCollection),
          typeof(TimeBlock), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("setvalue")]
        public SetValueCollection Values
        {
            get
            {
                return (SetValueCollection)this.UIThreadGetValue(ValuesProperty);

            }
            set
            {
                this.UIThreadSetValue(ValuesProperty, value);

            }
        }


        protected override void ProcessValidation()
        {
          
        }

        public IList<System.Xml.XmlNode> Storage { get; private set; }

    }
}
