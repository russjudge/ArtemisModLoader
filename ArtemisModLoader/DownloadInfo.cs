using System.Windows;
using System.Xml;
using RussLibrary;
using System;
namespace ArtemisModLoader
{
    
    public class DownloadInfo : XmlBase
    {
        public DownloadInfo(XmlNode node) : base(node) { }
        public DownloadInfo() : base() { }

        public static readonly DependencyProperty SourceProperty =
          DependencyProperty.Register("Source", typeof(string),
          typeof(DownloadInfo), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
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
        [XmlBaseAttribute]
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
        [XmlBaseAttribute]
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
        

     

    }
}
