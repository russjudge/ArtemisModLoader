using System.Windows;
using RussLibrary.Xml;
using RussLibrary;
using RussLibrary.WPF;
using ArtemisModLoader;
using System;
using System.Xml;
using System.Collections.Generic;


namespace VesselDataLibrary.Xml
{
    [XmlConversionRoot("internal_data")]
    public class InternalData : ChangeDependencyObject, IXmlStorage
    {
        public InternalData()
        {

            Storage = new List<XmlNode>();
        }
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

        protected override void ProcessValidation()
        {
            if (string.IsNullOrEmpty(File))
            {
                base.ValidationCollection.AddValidation(DataStrings.File, ValidationValue.IsError,
                   AMLResources.Properties.Resources.HullRaceNameValidation);
            }
            else if (!File.EndsWith(DataStrings.SNTExtension, StringComparison.OrdinalIgnoreCase))
            {
                base.ValidationCollection.AddValidation(DataStrings.File, ValidationValue.IsWarnState,
                        AMLResources.Properties.Resources.InternalDataFileExtensionValidation);
            }

        }

        public System.Collections.Generic.IList<System.Xml.XmlNode> Storage { get; private set; }
    }
}
