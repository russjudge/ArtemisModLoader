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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "DMX"),
    XmlConversionRoot("DMX_CONTROL")]
    public class DMXCommandFile : ChangeDependencyObject, IXmlStorage
    {
        public DMXCommandFile()
        {
            Storage = new List<XmlNode>();
            Events = new EventCollection();
            Events.ObjectChanged += Events_ObjectChanged;
        }
        

        public override void BeginInitialization()
        {
            base.BeginInitialization();
            Events.BeginInitialization();
            
        }
        public override void EndInitialization()
        {
            Events.EndInitialization();
            
            base.EndInitialization();
        }
        public override void AcceptChanges()
        {
            
            Events.AcceptChanges();
            

            base.AcceptChanges();
        }
        public override void RejectChanges()
        {
            Events.RejectChanges();
            
            base.RejectChanges();
        }
        void Events_ObjectChanged(object sender, EventArgs e)
        {
            SetChanged();
        }
        public static readonly DependencyProperty VersionProperty =
            DependencyProperty.Register("Version", typeof(double),
            typeof(DMXCommandFile), new PropertyMetadata(2.0D, OnItemChanged));

        [XmlConversion("version")]
        public double Version
        {
            get
            {
                return (double)this.UIThreadGetValue(VersionProperty);
            }
            set
            {
                this.UIThreadSetValue(VersionProperty, value);
            }
        }
        public static readonly DependencyProperty EventsProperty =
            DependencyProperty.Register("Events", typeof(EventCollection),
            typeof(DMXCommandFile), new PropertyMetadata(OnItemChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("event")]
        public EventCollection Events
        {
            get
            {
                return (EventCollection)this.UIThreadGetValue(EventsProperty);
            }
            set
            {
                this.UIThreadSetValue(EventsProperty, value);
            }
        }
        protected override void ProcessValidation()
        {
           
        }
        public IList<XmlNode> Storage { get; private set; }
    }
}
