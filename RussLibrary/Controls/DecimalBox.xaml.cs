using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace RussLibrary.Controls
{
    /// <summary>
    /// Interaction logic for DecimalBox.xaml
    /// </summary>
    public partial class DecimalBox : UserControl
    {
        public DecimalBox()
        {
            
            InitializeComponent();
            DataObject.AddPastingHandler(this, new DataObjectPastingEventHandler(TextBoxPasting));
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
        protected override void OnPreviewTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e != null)
            {
                if (IsTextAllowed(e.Text))
                {
                    base.OnPreviewTextInput(e);
                }
                else
                {
                    e.Handled = true;
                }
            }
        }


        public static readonly DependencyProperty MaxLengthProperty =
         DependencyProperty.Register("MaxLength", typeof(int),
         typeof(DecimalBox), new UIPropertyMetadata(int.MaxValue));

        public int MaxLength
        {
            get
            {
                return (int)this.UIThreadGetValue(MaxLengthProperty);
            }
            set
            {
                this.UIThreadSetValue(MaxLengthProperty, value);
            }
        }


        public static readonly DependencyProperty MaxNumberProperty =
          DependencyProperty.Register("MaxNumber", typeof(decimal?),
          typeof(DecimalBox), new UIPropertyMetadata(null));

        public decimal? MaxNumber
        {
            get
            {
                return (decimal?)this.UIThreadGetValue(MaxNumberProperty);
            }
            set
            {
                this.UIThreadSetValue(MaxNumberProperty, value);
            }
        }


        public static readonly DependencyProperty DisplayFormatProperty =
            DependencyProperty.Register("DisplayFormat", typeof(string),
            typeof(DecimalBox), new UIPropertyMetadata(new PropertyChangedCallback(OnDisplayFormatChanged)));

        public string DisplayFormat
        {
            get
            {
                return (string)this.UIThreadGetValue(DisplayFormatProperty);
            }
            set
            {
                this.UIThreadSetValue(DisplayFormatProperty, value);
            }
        }

        static void OnDisplayFormatChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DecimalBox me = sender as DecimalBox;
            if (me != null)
            {
                me.SetDisplay();
            }
        }
        void SetDisplay()
        {
            if (!string.IsNullOrEmpty(DisplayFormat))
            {
                DisplayValue = Value.ToString(DisplayFormat, System.Globalization.CultureInfo.CurrentCulture);
            }
            else
            {
                DisplayValue = Value.ToString(System.Globalization.CultureInfo.CurrentCulture);
            }
        }
        static void OnNumberOfDecimalDigitChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DecimalBox me = sender as DecimalBox;
            if (me != null)
            {
                if (string.IsNullOrEmpty(me.DisplayFormat))
                {
                    string wrk = string.Empty;
                    if (me.MaxNumber != null)
                    {
                        wrk = "0".PadLeft(Math.Floor(me.MaxNumber.Value).ToString(System.Globalization.CultureInfo.CurrentCulture).Length, '#');

                    }
                    else
                    {
                        int padLen = me.MaxLength;

                        if (me.NumberOfDecimalDigit != null && me.NumberOfDecimalDigit.Value > 0)
                        {
                            padLen -= me.NumberOfDecimalDigit.Value;
                        }
                        wrk = "0".PadLeft(padLen, '#');

                    }

                    if (me.NumberOfDecimalDigit != null)
                    {
                        if (me.NumberOfDecimalDigit.Value > 0)
                        {
                            wrk += ".".PadRight(me.NumberOfDecimalDigit.Value + 1, '0');
                        }

                    }
                    me.DisplayFormat = wrk;
                }
            }
        }

        static bool ValidateNumberOfDecimalDigits(object value)
        {
            int? val = value as int?;
            return (val == null || val.Value >= 0);

        }
        public static readonly DependencyProperty NumberOfDecimalDigitProperty =
        DependencyProperty.Register("NumberOfDecimalDigit", typeof(int?),
        typeof(DecimalBox), new UIPropertyMetadata(OnNumberOfDecimalDigitChanged),
        new ValidateValueCallback(ValidateNumberOfDecimalDigits));

        public int? NumberOfDecimalDigit
        {
            get
            {
                return (int?)this.UIThreadGetValue(NumberOfDecimalDigitProperty);
            }
            set
            {
                this.UIThreadSetValue(NumberOfDecimalDigitProperty, value);
            }
        }
        public static readonly DependencyProperty MinNumberProperty =
         DependencyProperty.Register("MinNumber", typeof(decimal?),
         typeof(DecimalBox), new UIPropertyMetadata(null));

        public decimal? MinNumber
        {
            get
            {
                return (decimal?)this.UIThreadGetValue(MinNumberProperty);
            }
            set
            {
                this.UIThreadSetValue(MinNumberProperty, value);
            }
        }

        public static readonly DependencyProperty DisplayValueProperty =
           DependencyProperty.Register("DisplayValue", typeof(string),
           typeof(DecimalBox), new UIPropertyMetadata("0"));

        public string DisplayValue
        {
            get
            {
                return (string)this.UIThreadGetValue(DisplayValueProperty);
            }
            set
            {
                this.UIThreadSetValue(DisplayValueProperty, value);
            }
        }

        bool SettingValues = false;
        static void OnWorkValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DecimalBox me = sender as DecimalBox;
            if (me != null)
            {
                if (!me.SettingValues)
                {
                    me.SettingValues = true;
                    decimal wrk = me.Value;
                    if (decimal.TryParse(me.WorkValue, out wrk))
                    {
                        me.Value = wrk;

                    }
                    else
                    {
                        me.WorkValue = me.Value.ToString(System.Globalization.CultureInfo.CurrentCulture);
                        me.txEntryBox.Text = me.Value.ToString(System.Globalization.CultureInfo.CurrentCulture);
                    }
                    me.SettingValues = false;
                }
            }
        }

        internal static readonly DependencyProperty WorkValueProperty =
          DependencyProperty.Register("WorkValue", typeof(string),
          typeof(DecimalBox), new UIPropertyMetadata("0", new PropertyChangedCallback(OnWorkValueChanged)));

        internal string WorkValue
        {
            get
            {
                return (string)this.UIThreadGetValue(WorkValueProperty);
            }
            set
            {
                this.UIThreadSetValue(WorkValueProperty, value);
            }
        }



        static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DecimalBox me = sender as DecimalBox;
            if (me != null)
            {
                if (!me.SettingValues)
                {
                    me.SettingValues = true;
                    me.WorkValue = me.Value.ToString(System.Globalization.CultureInfo.CurrentCulture);
                    me.SettingValues = false;
                }
                me.SetDisplay();
                me.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));

            }
        }


        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(DecimalBox));

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }




        static object OnCoerceValueChanged(DependencyObject sender, object value)
        {
            decimal retVal = (decimal)value;
            DecimalBox me = sender as DecimalBox;
            bool valueModified = false;
            if (me != null)
            {
                if (me.MaxNumber != null)
                {
                    if (retVal > me.MaxNumber.Value)
                    {
                        retVal = me.MaxNumber.Value;
                        valueModified = true;
                    }
                }
                if (me.MinNumber != null)
                {
                    if (retVal < me.MinNumber.Value)
                    {
                        retVal = me.MinNumber.Value;
                        valueModified = true;
                    }
                }
                if (me.NumberOfDecimalDigit != null)
                {
                    string[] wrk = retVal.ToString(System.Globalization.CultureInfo.CurrentCulture).Split('.');
                    int numDigs = 0;
                    if (wrk.Length > 1)
                    {
                        numDigs = wrk[1].Length;
                    }
                    if (numDigs > me.NumberOfDecimalDigit.Value)
                    {
                        retVal = Math.Floor(decimal.Multiply(retVal, Convert.ToDecimal(Math.Pow(10, me.NumberOfDecimalDigit.Value)))) / Convert.ToDecimal(Math.Pow(10, me.NumberOfDecimalDigit.Value));
                        valueModified = true;
                        
                    }

                }
                if (valueModified)
                {


                    me.WorkValue = retVal.ToString(System.Globalization.CultureInfo.CurrentCulture);
                      
                 
                    me.SetDisplay();
                }
            }
            return retVal;
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal),
            typeof(DecimalBox), new UIPropertyMetadata(0M, new PropertyChangedCallback(OnValueChanged),
                new CoerceValueCallback(OnCoerceValueChanged)));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public decimal Value
        {
            get
            {
                return (decimal)this.UIThreadGetValue(ValueProperty);
            }
            set
            {
                this.UIThreadSetValue(ValueProperty, value);
            }
        }
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            bool retVal = !regex.IsMatch(text);
            if (retVal)
            {
                retVal = (text.IndexOf('.') == text.LastIndexOf('.')) && (text.IndexOf('-') < 1) && text.IndexOf('+') < 1;

            }
            return retVal;
        }
        public static readonly DependencyProperty IsEditingProperty =
         DependencyProperty.Register("IsEditing", typeof(bool),
         typeof(DecimalBox), new UIPropertyMetadata(false));

        public bool IsEditing
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsEditingProperty);
            }
            set
            {
                this.UIThreadSetValue(IsEditingProperty, value);
            }
        }

        private void txEntry_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox txEntry = sender as TextBox;
            if (txEntry != null)
            {
                txEntry.SelectAll();
            }
        }


        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            IsEditing = true;
            txEntryBox.Focus();
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            IsEditing = false;
        }
    }
}