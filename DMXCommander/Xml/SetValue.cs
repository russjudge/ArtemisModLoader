using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RussLibrary.Xml;
using RussLibrary.WPF;
using System.Xml;
using System.Windows;
using RussLibrary;
namespace DMXCommander.Xml
{
    [XmlConversionRoot("setvalue")]
    public class SetValue: ChangeDependencyObject, IXmlStorage
    {

        public SetValue()
        {
            Storage = new List<XmlNode>();
            
        }

        
        public static readonly DependencyProperty ChannelProperty =
            DependencyProperty.Register("Channel", typeof(int),
            typeof(SetValue), new PropertyMetadata(OnItemChanged));
        [XmlConversion("index")]
        public int Channel
        {
            get
            {
                return (int)this.UIThreadGetValue(ChannelProperty);

            }
            set
            {
                this.UIThreadSetValue(ChannelProperty, value);

            }
        }



        public static readonly DependencyProperty ChannelValueProperty =
            DependencyProperty.Register("ChannelValue", typeof(byte),
            typeof(SetValue), new PropertyMetadata(OnItemChanged));
        [XmlConversion("value")]
        public byte ChannelValue
        {
            get
            {
                return (byte)this.UIThreadGetValue(ChannelValueProperty);

            }
            set
            {
                this.UIThreadSetValue(ChannelValueProperty, value);

            }
        }

        static void OnDeltaChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SetValue me = sender as SetValue;
            if (me != null)
            {
                if (!me.DeltaChanging)
                {
                    me.DeltaChanging = true;
                    me.Change = Convert.ToByte((me.Delta / 35) * 1000);
                    me.DeltaChanging = false;
                }
            }
        }
        bool DeltaChanging = false;

        public static readonly DependencyProperty DeltaProperty =
            DependencyProperty.Register("Delta", typeof(decimal),
            typeof(SetValue), new PropertyMetadata(OnDeltaChanged));
        
        public decimal Delta
        {
            get
            {
                return (decimal)this.UIThreadGetValue(DeltaProperty);

            }
            set
            {
                this.UIThreadSetValue(DeltaProperty, value);

            }
        }

        static void OnChangeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SetValue me = sender as SetValue;
            if (me != null)
            {
                if (!me.DeltaChanging)
                {
                    me.DeltaChanging = true;
                    me.Delta = Convert.ToDecimal(me.Change*35) / 1000;
                    me.DeltaChanging = false;
                }
                OnItemChanged(sender, e);
            }
        }
        public static readonly DependencyProperty ChangeProperty =
            DependencyProperty.Register("Change", typeof(short),
            typeof(SetValue), new PropertyMetadata(OnChangeChanged));
        [XmlConversion("change")]
        public short Change
        {
            get
            {
                return (short)this.UIThreadGetValue(ChangeProperty);

            }
            set
            {
                this.UIThreadSetValue(ChangeProperty, value);

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ChannelValue")]
        protected override void ProcessValidation()
        {
            if (Channel < 0 || Channel > 511)
            {
                base.ValidationCollection.AddValidation("Channel", ValidationValue.IsError,
                     "Channel must be in range of 0 - 511");
            }

            if (ChannelValue < 0 || ChannelValue > 255)
            {
                base.ValidationCollection.AddValidation("ChannelValue", ValidationValue.IsError,
                     "ChannelValue must be in range of 0 - 255");
            }
            if (Change < -255 || Change > 255)
            {
                base.ValidationCollection.AddValidation("Change", ValidationValue.IsError,
                     "Change must be in range of -255 to 255");
            }
        }

        public IList<System.Xml.XmlNode> Storage { get; private set; }

    }
}
