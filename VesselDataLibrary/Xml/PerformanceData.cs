using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.Xml;
using RussLibrary;
using System.Windows;
using RussLibrary.WPF;
using ArtemisModLoader;
using System.Xml;
namespace VesselDataLibrary.Xml
{
    [XmlConversionRoot("performance")]
    public class PerformanceData : ChangeDependencyObject, IXmlStorage
    {
        public PerformanceData()
        {
            Storage = new List<XmlNode>();
        }
        //<performance turnrate="0.004" topspeed="0.6" />
        public static readonly DependencyProperty TurnRateProperty =
            DependencyProperty.Register(DataStrings.TurnRate, typeof(double),
            typeof(PerformanceData));
        [XmlConversion("turnrate")]
        public double TurnRate
        {
            get
            {
                return (double)this.UIThreadGetValue(TurnRateProperty);
            }
            set
            {
                this.UIThreadSetValue(TurnRateProperty, value);
            }
        }


        public static readonly DependencyProperty TopSpeedProperty =
           DependencyProperty.Register(DataStrings.TopSpeed, typeof(double),
           typeof(PerformanceData));
        [XmlConversion("topspeed")]
        public double TopSpeed
        {
            get
            {
                return (double)this.UIThreadGetValue(TopSpeedProperty);
            }
            set
            {
                this.UIThreadSetValue(TopSpeedProperty, value);
            }
        }


        protected override void ProcessValidation()
        {
            if (TurnRate > 1)
            {
                //warn
                base.ValidationCollection.AddValidation(DataStrings.TurnRate, ValidationValue.IsWarnState,
                  AMLResources.Properties.Resources.TurnRateFast);
            }
            else if (TurnRate < 0)
            {
                //error
                base.ValidationCollection.AddValidation(DataStrings.TurnRate, ValidationValue.IsError,
                  AMLResources.Properties.Resources.TurnRateNegative);
            }
            else if (TurnRate == 0)
            {
                //warn
                base.ValidationCollection.AddValidation(DataStrings.TurnRate, ValidationValue.IsWarnState,
                 AMLResources.Properties.Resources.TurnRateZero);
            }
            if (TopSpeed > 2)
            {
                //warn
                base.ValidationCollection.AddValidation(DataStrings.TopSpeed, ValidationValue.IsWarnState,
               AMLResources.Properties.Resources.TopSpeedFast);
            }
            else if (TopSpeed < 0)
            {
                //error
                base.ValidationCollection.AddValidation(DataStrings.TopSpeed, ValidationValue.IsError,
                  AMLResources.Properties.Resources.TopSpeedNegative);
            }
            else if (TopSpeed == 0)
            {
                //warn
                base.ValidationCollection.AddValidation(DataStrings.TopSpeed, ValidationValue.IsWarnState,
               AMLResources.Properties.Resources.TopSpeedZero);
            }

        }

        public IList<System.Xml.XmlNode> Storage { get; private set; }
    }
}
