using System.Windows;
using RussLibrary.Xml;
using RussLibrary;


namespace VesselDataLibrary
{
    [XmlConversionRoot("internal_data")]
    public class InternalData : DependencyObject
    {
        // <internal_data file="dat/artemis.snt" />

        public static readonly DependencyProperty FileProperty =
            DependencyProperty.Register("File", typeof(string),
            typeof(InternalData));
        [XmlConversion("file")]
        public string File
        {
            get
            {
                return (string)this.UIThreadGetValue(FileProperty);

            }
            set
            {
                this.UIThreadSetValue(FileProperty, value);

            }
        }
    }
}
