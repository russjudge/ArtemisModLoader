using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary;
using RussLibrary.Xml;
using RussLibrary.WPF;
using System.Windows;

namespace ArtemisModLoader.Xml
{
   
    public class StringItem : ChangeDependencyObject
    {

        public StringItem()
        { }
        public StringItem(string text)
        {
            Text = text;
        }
        public static readonly DependencyProperty TextProperty =
          DependencyProperty.Register("Text", typeof(string),
          typeof(StringItem), new UIPropertyMetadata(OnItemChanged));
        [XmlConversion("Key")]
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

        protected override void ProcessValidation()
        {
      
        }
    }
}
