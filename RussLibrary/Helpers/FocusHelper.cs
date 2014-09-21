using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows;
using System.Threading;
using System.Windows.Input;
using RussLibrary;
using System.Windows.Controls;
namespace RussLibrary.Helpers
{

    public static class FocusHelper
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(FocusHelper));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        public static void Focus(UIElement element)
        {
            //Focus in a callback to run on another thread, ensuring the main UI thread is initialized by the
            //time focus is set
            ThreadPool.QueueUserWorkItem(delegate(Object foo)
            {

                UIElement elem = (UIElement)foo;
                element.Dispatcher.BeginInvoke(
                    (Action)(delegate()
                    {
                        elem.Focus();
                        Keyboard.Focus(elem);
                    }));

            }, element);
        }
        public static DependencyObject MoveFocus(UIElement element, FocusNavigationDirection focusDirection)
        {
            DependencyObject o;
            UIElement TestObject = element;
            do
            {
                o = TestObject.PredictFocus(focusDirection);

                if (o == TestObject)
                {
                    o = Window.GetWindow(element);
                }
                
                if (o != null)
                {

                    TestObject = o as UIElement;
                    o = TestObject;
                }
            } while (o != null);
            return o;
        }
        public static void MoveFocus(UIElement element)
        {


            //this choice moves first to the right, then up parents.
            //user does not have access to this control, so move focus to the next accessible control
            //      (to handle user Tabbing through controls).


            DependencyObject o;
            o = MoveFocus(element, FocusNavigationDirection.Down);
            if (o == null)
            {

                o = MoveFocus(element, FocusNavigationDirection.Right);

                if (o == null)
                {
                    o = MoveFocus(element, FocusNavigationDirection.Up);
                    if (o == null)
                    {
                        o = MoveFocus(element, FocusNavigationDirection.Left);
                    }
                }
            }



            UIElement elem = o as UIElement;
            if (elem != null)
            {

                elem.Focus();
                


            }
        }

        public static readonly DependencyProperty ActionFocusProperty =
           DependencyProperty.RegisterAttached("ActionFocus",
           typeof(bool), typeof(FocusHelper), new FrameworkPropertyMetadata(OnActionFocusChanged));

        public static void SetActionFocus(DependencyObject element, bool value)
        {
            element.UIThreadSetValue(ActionFocusProperty, value);
        }
        public static bool GetActionFocus(DependencyObject element)
        {
            return (bool)element.UIThreadGetValue(ActionFocusProperty);
        }
        private static void OnActionFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            bool DoAction = (bool)e.NewValue;
            TextBox t = sender as TextBox;
            UIElement elem = sender as UIElement;
            if (t != null)
            {
                if (DoAction)
                {
                    t.SelectAll();
                    
                }

            }
            if (elem != null)
            {
                FocusHelper.Focus(elem);
            }
        }
    }
}
