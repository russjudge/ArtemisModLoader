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
    //[XmlConversionRoot("long_desc", true)]
    [XmlConversionRoot("long_desc")]
    public class DescriptionObject : ChangeDependencyObject, IXmlStorage 
    {
        public DescriptionObject()
        {
            Storage = new List<XmlNode>();
        }
      
        //<long_desc text="USFP Cruiser^Standard long patrol vessel of the USFP.^2 forward beams^2 Torpedo tubes^Stores for 2 nukes, 8 homing, 6 mines, 4 ECM." />
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
            typeof(DescriptionObject), new PropertyMetadata(OnTextChanged));
        //[XmlConversion("text", true)]
        [XmlConversion("text")]
        public string Text
        {
            get
            {
                return (string)this.UIThreadGetValue(TextProperty);
            }
            set
            {
                this.UIThreadSetValue(TextProperty, value);
            }
        }
        bool Updating = false;
        static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DescriptionObject me = sender as DescriptionObject;
            if (me != null && !me.Updating)
            {
                me.Updating = true;
                me.TextData = me.Text.Replace(DataStrings.Caret, "\r\n");
                ChangeDependencyObject.OnItemChanged(me, e);
                me.Updating = false;
                
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "VesselDataLibrary.DescriptionObject.set_Text(System.String)")]
        static void OnTextDataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DescriptionObject me = sender as DescriptionObject;
            if (me != null && !me.Updating)
            {
                me.Updating = true;
                me.Text = me.TextData.Replace("\r\n", DataStrings.Caret);
                ChangeDependencyObject.OnItemChanged(me, e);
                me.Updating = false;
            }
        }
        public static readonly DependencyProperty TextDataProperty =
           DependencyProperty.Register("TextData", typeof(string),
           typeof(DescriptionObject), new PropertyMetadata(OnTextDataChanged));

        public string TextData
        {
            get
            {
                return (string)this.UIThreadGetValue(TextDataProperty);
            }
            set
            {
                this.UIThreadSetValue(TextDataProperty, value);
            }
        }




        protected override void ProcessValidation()
        {
            if (string.IsNullOrEmpty(Text))
            {
                base.ValidationCollection.Add(
                  new ValidationObject(DataStrings.Text, ValidationValue.IsError,
                      Properties.Resources.VesselDescriptionValidation));
            }
        }

        public IList<XmlNode> Storage { get; private set; }
    }
}
