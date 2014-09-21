using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.ComponentModel;

namespace RussLibrary.Helpers
{

    public class WatermarkService : Freezable
    {

        /// <summary>
        /// Watermark Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
           "Watermark",
           typeof(string),
           typeof(WatermarkService),
           new FrameworkPropertyMetadata((string)null, new PropertyChangedCallback(OnWatermarkChanged)));

        public string Watermark
        {
            get { return (string)this.UIThreadGetValue(WatermarkProperty); }
            set { this.UIThreadSetValue(WatermarkProperty, value); }
        }
        #region Private Fields

        /// <summary>
        /// Dictionary of ItemsControls
        /// </summary>
        private static readonly Dictionary<object, ItemsControl> itemsControls = new Dictionary<object, ItemsControl>();

        #endregion




        /// <summary>
        /// Gets the Watermark property.  This dependency property indicates the watermark for the control.
        /// </summary>
        /// <param name="value"><see cref="DependencyObject"/> to get the property from</param>
        /// <returns>The value of the Watermark property</returns>
        public static string GetWatermark(DependencyObject value)
        {
            string retval = null;
            if (value != null)
            {
                retval = (string)value.UIThreadGetValue(WatermarkProperty);
            }
            return retval;
        }

        /// <summary>
        /// Sets the Watermark property.  This dependency property indicates the watermark for the control.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/> to set the property on</param>
        /// <param name="value">value of the property</param>
        public static void SetWatermark(DependencyObject sender, string value)
        {
            if (sender != null)
            {
                sender.UIThreadSetValue(WatermarkProperty, value);
            }
        }

        /// <summary>
        /// Handles changes to the Watermark property.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/> that fired the event</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == null)
            {
                Control control = d as Control;
                control.Loaded += Control_Loaded;

                ComboBox cb = d as ComboBox;
                TextBox tb = d as TextBox;
                ItemsControl ic = d as ItemsControl;


                if (cb != null || tb != null)
                {
                    control.GotKeyboardFocus += Control_GotKeyboardFocus;
                    control.LostKeyboardFocus += Control_Loaded;
                }
                if (tb != null)
                {
                    tb.TextChanged += new TextChangedEventHandler(tb_TextChanged);
                }
                if (cb != null)
                {

                    cb.SelectionChanged += new SelectionChangedEventHandler(cb_SelectionChanged);
                }
                if (ic != null && cb == null)
                {

                    // for Items property  
                    ic.ItemContainerGenerator.ItemsChanged += ItemsChanged;
                    itemsControls.Add(ic.ItemContainerGenerator, ic);

                    // for ItemsSource property  
                    DependencyPropertyDescriptor prop = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, ic.GetType());
                    prop.AddValueChanged(ic, ItemsSourceChanged);
                }
            }
        }

        static void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            Control c = sender as Control;
            if (ShouldShowWatermark(c))
            {
                ShowWatermark(c);
            }
            else
            {
                RemoveWatermark(c);
            }
        }

        static void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Control c = sender as Control;
            if (ShouldShowWatermark(c))
            {
                ShowWatermark(c);
            }
            else
            {
                RemoveWatermark(c);
            }
        }

        #region Event Handlers

        /// <summary>
        /// Handle the GotFocus event on the control
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs"/> that contains the event data.</param>
        private static void Control_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            Control c = (Control)sender;
            if (ShouldShowWatermark(c))
            {
                RemoveWatermark(c);
            }
        }
        /// <summary>
        /// Handle the Loaded and LostFocus event on the control
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs"/> that contains the event data.</param>
        private static void Control_Loaded(object sender, RoutedEventArgs e)
        {
            Control c = (Control)sender;

            if (ShouldShowWatermark(c))
            {
                ShowWatermark(c);
            }
        }

        /// <summary>
        /// Event handler for the items source changed event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private static void ItemsSourceChanged(object sender, EventArgs e)
        {
            ItemsControl c = (ItemsControl)sender;
            if (c.ItemsSource != null)
            {
                if (ShouldShowWatermark(c))
                {
                    ShowWatermark(c);
                }
                else
                {
                    RemoveWatermark(c);
                }
            }
            else
            {
                ShowWatermark(c);
            }
        }

        /// <summary>
        /// Event handler for the items changed event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ItemsChangedEventArgs"/> that contains the event data.</param>
        private static void ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            ItemsControl control;
            if (itemsControls.TryGetValue(sender, out control))
            {
                if (ShouldShowWatermark(control))
                {
                    ShowWatermark(control);
                }
                else
                {
                    RemoveWatermark(control);
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Remove the watermark from the specified element
        /// </summary>
        /// <param name="control">Element to remove the watermark from</param>
        private static void RemoveWatermark(UIElement control)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);

            // layer could be null if control is no longer in the visual tree
            if (layer != null)
            {
                Adorner[] adorners = layer.GetAdorners(control);
                if (adorners == null)
                {
                    return;
                }

                foreach (Adorner adorner in adorners)
                {
                    if (adorner is WatermarkAdorner)
                    {
                        adorner.Visibility = Visibility.Hidden;
                        layer.Remove(adorner);
                    }
                }
            }
        }

        /// <summary>
        /// Show the watermark on the specified control
        /// </summary>
        /// <param name="control">Control to show the watermark on</param>
        private static void ShowWatermark(Control control)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);

            // layer could be null if control is no longer in the visual tree
            if (layer != null)
            {
                bool AlreadyAdded = false;
                Adorner[] adorners = layer.GetAdorners(control);
                if (adorners != null)
                {
                    foreach (Adorner a in adorners)
                    {
                        if (a is WatermarkAdorner)
                        {
                            AlreadyAdded = true;
                            break;
                        }
                    }
                }
                if (!AlreadyAdded)
                {
                    layer.Add(new WatermarkAdorner(control, GetWatermark(control)));
                }
            }
        }

        /// <summary>
        /// Indicates whether or not the watermark should be shown on the specified control
        /// </summary>
        /// <param name="c"><see cref="Control"/> to test</param>
        /// <returns>true if the watermark should be shown; false otherwise</returns>
        private static bool ShouldShowWatermark(Control c)
        {
            ComboBox cb = c as ComboBox;
            TextBox tb = c as TextBox;
            ItemsControl ic = c as ItemsControl;


            if (cb != null)
            {
                return (string.IsNullOrEmpty(cb.Text) && cb.SelectedItem == null);
            }
            else if (tb != null)
            {
                return string.IsNullOrEmpty(tb.Text);
            }
            else if (ic != null)
            {
                return ic.Items.Count == 0;
            }
            else
            {
                return false;
            }
        }

        #endregion

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
