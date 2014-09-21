using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using RussLibrary.WPF;
using RussLibrary.Xml;
using System.Xml;
using System.Windows;
using RussLibrary;
namespace VesselDataLibrary.Xml
{

    public class Taunt : ChangeDependencyObject, IXmlStorage
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(Taunt));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        public Taunt()
        {
            Storage = new List<XmlNode>();
        }
        public static readonly DependencyProperty ImmunityProperty =
            DependencyProperty.Register("Immunity", typeof(string),
            typeof(Taunt), new PropertyMetadata(OnItemChanged));
        [XmlConversion("immunity")]
        public string Immunity
        {
            get
            {
                return (string)this.UIThreadGetValue(ImmunityProperty);

            }
            set
            {
                this.UIThreadSetValue(ImmunityProperty, value);

            }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
            typeof(Taunt), new PropertyMetadata(OnItemChanged));
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
        #region IXmlStorage Members
        


        public IList<System.Xml.XmlNode> Storage { get; private set; }
        #endregion

        protected override void ProcessValidation()
        {
            
        }
    }
}
