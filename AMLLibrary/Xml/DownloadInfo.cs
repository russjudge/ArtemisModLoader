using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RussLibrary;
using RussLibrary.Xml;
using RussLibrary.WPF;
namespace ArtemisModLoader.Xml
{
    [XmlConversionRoot("Download")]
    public class DownloadInfo : ChangeDependencyObject
    {


        public static readonly DependencyProperty SourceProperty =
          DependencyProperty.Register("Source", typeof(string),
          typeof(DownloadInfo), new UIPropertyMetadata(OnItemChanged));
         [XmlConversion("Source")]
        public string Source
        {
            get
            {
                return (string)this.UIThreadGetValue(SourceProperty);

            }
            set
            {
                this.UIThreadSetValue(SourceProperty, value);

            }
        }

        public static readonly DependencyProperty ForVersionProperty =
         DependencyProperty.Register("ForVersion", typeof(string),
         typeof(DownloadInfo), new UIPropertyMetadata(OnItemChanged));
        
        public string ForVersion
        {
            get
            {
                return (string)this.UIThreadGetValue(ForVersionProperty);

            }
            private set
            {
                this.UIThreadSetValue(ForVersionProperty, value);

            }
        }

        public static readonly DependencyProperty WebpageProperty =
         DependencyProperty.Register("Webpage", typeof(string),
         typeof(DownloadInfo), new UIPropertyMetadata(OnItemChanged));
        [XmlConversion("Webpage")]
        public string Webpage
        {
            get
            {
                return (string)this.UIThreadGetValue(WebpageProperty);

            }
            set
            {
                this.UIThreadSetValue(WebpageProperty, value);

            }
        }


        protected override void ProcessValidation()
        {
            if (string.IsNullOrEmpty(this.Source))
            {
                base.ValidationCollection.AddValidation(DataStrings.Source,
                    ValidationValue.IsWarnState,
                    AMLResources.Properties.Resources.DownloadSourceValidation);
            }
            if (string.IsNullOrEmpty(this.Webpage))
            {
                base.ValidationCollection.AddValidation(DataStrings.Webpage,
                    ValidationValue.IsWarnState,
                    AMLResources.Properties.Resources.DownloadWebpageValidation);
            }
        }
    }
}
