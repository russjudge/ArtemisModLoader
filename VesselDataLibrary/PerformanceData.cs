using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.Xml;
using RussLibrary;
using System.Windows;
namespace VesselDataLibrary
{
    [XmlConversionRoot("performance")]
    public class PerformanceData : DependencyObject
    {
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

    }
}
