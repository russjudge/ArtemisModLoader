using log4net;
using RussLibrary.WPF;
using RussLibrary.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using RussLibrary;
namespace VesselDataLibrary.Xml
{
    public class Production : ChangeDependencyObject, IXmlStorage
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(Taunt));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        public Production()
        {
            Storage = new List<XmlNode>();
        }
        public static readonly DependencyProperty CoefficientProperty =
            DependencyProperty.Register("Coefficient", typeof(decimal),
            typeof(Taunt), new PropertyMetadata(OnItemChanged));
        [XmlConversion("coeff")]
        public decimal Coefficient
        {
            get
            {
                return (decimal)this.UIThreadGetValue(CoefficientProperty);

            }
            set
            {
                this.UIThreadSetValue(CoefficientProperty, value);

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
