using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.Xml;
using RussLibrary;
using System.Windows;
using RussLibrary.WPF;
using System.Xml;
namespace VesselDataLibrary.Xml
{
    [XmlConversionRoot("performance")]
    public class PerformanceData : ChangeDependencyObject, IXmlStorage
    {
        public PerformanceData()
        {
            Storage = new List<XmlNode>();
            Efficiency = 1;
        }

        public static readonly DependencyProperty EfficiencyProperty =
            DependencyProperty.Register("Efficiency", typeof(double),
            typeof(PerformanceData));
        [XmlConversion("efficiency")]
        public double Efficiency
        {
            get
            {
                return (double)this.UIThreadGetValue(EfficiencyProperty);
            }
            set
            {
                this.UIThreadSetValue(EfficiencyProperty, value);
            }
        }

        //<performance turnrate="0.004" topspeed="0.6" />
        public static readonly DependencyProperty TurnRateProperty =
            DependencyProperty.Register("TurnRate", typeof(double),
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
           DependencyProperty.Register("TopSpeed", typeof(double),
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
                base.ValidationCollection.AddValidation("TurnRate", ValidationValue.IsWarnState,
                  Properties.Resources.TurnRateFast);
            }
            else if (TurnRate < 0)
            {
                //error
                base.ValidationCollection.AddValidation("TurnRate", ValidationValue.IsError,
                  Properties.Resources.TurnRateNegative);
            }
            else if (TurnRate == 0)
            {
                //warn
                base.ValidationCollection.AddValidation(DataStrings.TurnRate, ValidationValue.IsWarnState,
                 Properties.Resources.TurnRateZero);
            }
            if (TopSpeed > 2)
            {
                //warn
                base.ValidationCollection.AddValidation(DataStrings.TopSpeed, ValidationValue.IsWarnState,
                Properties.Resources.TopSpeedFast);
            }
            else if (TopSpeed < 0)
            {
                //error
                base.ValidationCollection.AddValidation(DataStrings.TopSpeed, ValidationValue.IsError,
                  Properties.Resources.TopSpeedNegative);
            }
            else if (TopSpeed == 0)
            {
                //warn
                base.ValidationCollection.AddValidation(DataStrings.TopSpeed, ValidationValue.IsWarnState,
                Properties.Resources.TopSpeedZero);
            }

        }

        public IList<System.Xml.XmlNode> Storage { get; private set; }
    }
}
