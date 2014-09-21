using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

namespace RussLibrary
{

    public static class ExtensionMethods
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ExtensionMethods));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static object ListBoxItemContent(this MouseButtonEventArgs me)
        {
            object retVal = null;
            if (me != null)
            {
                ItemsControl lb = me.Source as ItemsControl;
                //ListBox lb = me.Source as ListBox;
                if (lb != null)
                {
                    retVal = lb.ListBoxItemContent(me.GetPosition(lb));
                }
            }
            return retVal;
        }
        //public static object ListBoxItemContent(this ListBox me, Point Position)
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static object ListBoxItemContent(this ItemsControl me, Point position)
        {

            if (me != null)
            {
                UIElement elem = (UIElement)me.InputHitTest(position);
                while (elem != null)
                {
                    ContentControl itm = elem as ContentControl;
                    //ListBoxItem itm = elem as ListBoxItem;

                    if (itm != null)
                    {
                        return itm.Content;

                    }
                    elem = VisualTreeHelper.GetParent(elem) as UIElement;
                }
            }
            return null;
        }



        private static void SetWinSize(object state)
        {
            Window win = state as Window;
            if (win != null)
            {
               win.Dispatcher.BeginInvoke(new Action<Window>(SetWinSize), DispatcherPriority.Loaded, win);
            }
        }
        private static void SetWinSize(Window win)
        {
             if (win != null)
            {
                if (double.IsNaN(win.MaxHeight) || double.IsInfinity(win.MaxHeight) || win.MaxHeight > System.Windows.SystemParameters.FullPrimaryScreenHeight )
                {
                    win.MaxHeight = System.Windows.SystemParameters.FullPrimaryScreenHeight;
                }
                if (double.IsNaN(win.MaxWidth) || double.IsInfinity(win.MaxWidth) || win.MaxWidth > System.Windows.SystemParameters.FullPrimaryScreenWidth )
                {
                    win.MaxWidth = System.Windows.SystemParameters.FullPrimaryScreenWidth ;
                    
                }
                if (win.Top < 0)
                {
                    win.Top = 0;
                }
                if (win.Left < 0)
                {
                    win.Left = 0;
                }
            }
        }
        public static void SetMaxSize(this Window win)
        {
            if (win != null)
            {
               System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(SetWinSize), win);
            }
        }
        public static bool ImplementsIList(this Type me)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            if (me != null)
            {
                Type[] fT = me.FindInterfaces(TypeFilter.Equals, typeof(IList));
                retVal = (fT.Length > 0);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }

        public static bool ImplementsIList(this PropertyInfo me)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            if (me != null)
            {
                retVal = me.PropertyType.ImplementsIList();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parms")]
        public static object GetInstance(this Type me, params object[] constructorParms)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            object retVal = null;
            if (me.InheritsOrIs(typeof(Dispatcher))
                && Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            {
                retVal = Application.Current.Dispatcher.Invoke(new Func<Type, object[], object>(GetInstance), me, constructorParms);
            }
            else
            {

                
                List<Type> constructorSignature = new List<Type>();
                if (constructorParms != null)
                {
                    foreach (object p in constructorParms)
                    {
                        constructorSignature.Add(p.GetType());
                    }
                }
                if (me != null)
                {
                    ConstructorInfo constructor = me.GetConstructor(constructorSignature.ToArray());
                    if (constructor == null)
                    {
                        throw new ArgumentException("Constructor not found for type " + me.ToString());
                    }
                    if (me.InheritsOrIs(typeof(Dispatcher)) || me.GetType() == typeof(DispatcherObject))
                    {
                        if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
                        {
                            retVal = Application.Current.Dispatcher.Invoke(
                                new Func<object[], object>(constructor.Invoke), constructorParms);
                        }
                    }
                    else
                    {
                        retVal = constructor.Invoke(constructorParms);
                    }
                }
                if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            }
            return retVal;
        }
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static void UIThreadAddToCollection<T>(this DispatcherObject me, IList<T> collection, T value)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (me != null)
            {
                if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
                {
                    //If trying to execute while closing, will crash.  Try..catch is to prevent that.
                    try
                    {
                        Application.Current.Dispatcher.Invoke(new Action<T>(collection.Add), value);
                    }
                    catch (ArgumentException) { }
                }
                else
                {
                    if (collection != null)
                    {
                        collection.Add(value);
                    }
                }
            }
            else
            {
                if (collection != null)
                {
                    collection.Add(value);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static bool UIThreadRemoveFromCollection<T>(this DispatcherObject me, IList<T> collection, T value)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            if (me != null)
            {
                if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
                {
                    //If trying to execute while closing, will crash.  Try..catch is to prevent that.
                    try
                    {
                        retVal = (bool)Application.Current.Dispatcher.Invoke(new Func<T, bool>(collection.Remove), value);
                    }
                    catch (ArgumentException) { }
                }
                else
                {
                    if (collection != null)
                    {
                        retVal = collection.Remove(value);
                    }
                }
            }
            else
            {
                if (collection != null)
                {
                    retVal = collection.Remove(value);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }


        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static void UIThreadClearCollection<T>(this DispatcherObject me, IList<T> collection)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (me != null)
            {
                if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
                {
                    Application.Current.Dispatcher.Invoke(new Action(collection.Clear));
                }
                else
                {
                    if (collection != null)
                    {
                        collection.Clear();
                    }
                }
            }
            else
            {
                if (collection != null)
                {
                    collection.Clear();
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static object UIThreadGetValue(this DependencyObject me, DependencyProperty dp)
        {
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            object retVal = null;
            if (me != null)
            {
                if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
                {
                    retVal = Application.Current.Dispatcher.Invoke(
                        new Func<DependencyProperty, object>(me.GetValue), dp);

                }
                else
                {
                    retVal =  me.GetValue(dp);
                }
            }
            else
            {
                retVal = null;
            }
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;

        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static void UIThreadSetValue(this DependencyObject me, DependencyProperty dp, object value)
        {
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (me != null)
            {

                if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
                {
                    if (!Application.Current.Dispatcher.HasShutdownFinished 
                        && !Application.Current.Dispatcher.HasShutdownStarted)
                    {
                        Application.Current.Dispatcher.Invoke(
                            new Action<DependencyProperty, object>(me.SetValue), dp, value);
                    }
                }
                else
                {
                    //if (_log.IsInfoEnabled && dp != null)
                    //{

                    //    _log.InfoFormat("Setting {0}.{1} to value {2}", me.GetType().ToString(), dp.GetType().ToString(), value);

                    //}
                    me.SetValue(dp, value);
                }
                
            }
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        /// Copies the properties. objects do not necessarily need be related.
        /// Properties are copied by name and types must match, although a property on the target object 
        /// with the same name can be derived from the type of the property on the source object.
        /// </summary>
        /// <param name="me">Me.</param>
        /// <param name="source">The source.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static void CopyProperties(this object me, object source)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (source != null && me != null)
            {
                if ((me.GetType().InheritsOrIs(typeof(Dispatcher)) || source.GetType().InheritsOrIs(typeof(Dispatcher)))
                    && Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
                {
                    Application.Current.Dispatcher.Invoke(new Action<object, object>(CopyProperties), me, source);
                }
                else
                {
                    Dictionary<string, PropertyInfo> TargetProperties = new Dictionary<string, PropertyInfo>();
                    foreach (PropertyInfo prop in me.GetType().GetProperties())
                    {
                        TargetProperties.Add(prop.Name, prop);
                    }
                     IList lstSrc = source as IList;
                     if (lstSrc != null)
                     {
                         IList targ = me as IList;
                         if (targ != null)
                         {
                             foreach (object obj in lstSrc)
                             {
                                 targ.Add(obj);
                             }
                         }

                     }
                     else
                     {
                         PropertyInfo[] properties = source.GetType().GetProperties();
                         for (int i = 0; i < properties.Length; i++)
                         {
                             
                             TryToSetTarget(source, properties[i], me, TargetProperties);
                            
                         }
                     }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static void TryToSetTarget(object source, PropertyInfo prop, object me, Dictionary<string, PropertyInfo> TargetProperties)
        {
            if (prop.CanWrite && prop.CanRead)
            {
                if (TargetProperties.ContainsKey(prop.Name))
                {
                    if (TargetProperties[prop.Name].GetType() == prop.GetType() || TargetProperties[prop.Name].GetType() == prop.GetType().BaseType || TargetProperties[prop.Name].GetType().BaseType == prop.GetType().BaseType)
                    {
                        try
                        {

                            TargetProperties[prop.Name].SetValue(me, prop.GetValue(source, null), null);

                        }
                        catch (Exception ex)
                        {
                            LogWarnings(ex, source, prop, me);
                        }
                    }
                }
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static void LogWarnings(Exception ex, object source, PropertyInfo prop, object me)
        {
            if (_log.IsWarnEnabled)
            {
                _log.Warn("--------------!!!!   Unable to set property  !!!! ----------", ex);
                try
                {
                    if (source != null)
                    {
                        _log.WarnFormat("Source object type: {0}", source.GetType().ToString());
                    }
                }
                catch { }
                try
                {
                    if (me != null)
                    {
                        _log.WarnFormat("Target object type: {0}", me.GetType().ToString());
                    }
                }
                catch { }
                try
                {
                    if (prop != null)
                    {
                        _log.WarnFormat("Property attempting to set: {0}", prop.Name);
                    }
                }
                catch { }
                try
                {
                    if (source != null)
                    {
                        _log.WarnFormat("Value setting to: {0}", prop.GetValue(source, null), null);
                    }
                }
                catch { }

            }
        }
        public static bool InheritsOrIs(this Type baseType, Type matchingType)
        {
            bool retVal = false;
            do
            {
                if (baseType != null)
                {
                    retVal = (baseType == matchingType || baseType.IsSubclassOf(matchingType));
                    baseType = baseType.BaseType;
                }
            } while (!retVal && baseType != null);
            return retVal;
        }
        public static Type GetElementType(this IList me)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Type retVal = null;
            if (me != null)
            {
                Type seqType = me.GetType();
                Type ienum = FindIEnumerable(seqType);
                if (ienum == null)
                {
                    retVal = seqType;
                }
                else
                {
                    retVal = ienum.GetGenericArguments()[0];
                }
            }
            else
            {
                retVal = null;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        private static Type FindIEnumerable(Type seqType)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Type retVal = null;
            if (seqType == null || seqType == typeof(string))
            {
                retVal = null;
            }
            else if (seqType.IsArray)
            {
                retVal = typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            }
            else if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        retVal = ienum;
                        break;
                    }
                }
            }
            else
            {
                Type[] ifaces = seqType.GetInterfaces();
                if (ifaces != null && ifaces.Length > 0)
                {
                    foreach (Type iface in ifaces)
                    {
                        Type ienum = FindIEnumerable(iface);
                        if (ienum != null)
                        {
                            retVal = ienum;
                            break;
                        }
                    }
                }
                if (seqType.BaseType != null && seqType.BaseType != typeof(object))
                {
                    retVal = FindIEnumerable(seqType.BaseType);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Re")]
        public static void ReCenter(this Window me)
        {
            if (me != null)
            {
                if (me.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
                {
                    me.Dispatcher.Invoke(new Action<Window>(ReCenter), me);
                }
                else
                {
                    Window win = Window.GetWindow(me);
                    if (win != null)
                    {
                        if (win.ActualHeight > SystemParameters.WorkArea.Height - 24)
                        {
                            win.Height = SystemParameters.WorkArea.Height - 24;
                        }
                        if (win.ActualWidth > SystemParameters.WorkArea.Width - 24)
                        {
                            win.Width = SystemParameters.WorkArea.Width - 24;
                        }

                        win.Top = SystemParameters.WorkArea.Top + (SystemParameters.WorkArea.Height / 2) - (win.ActualHeight / 2);
                        win.Left = SystemParameters.WorkArea.Left + (SystemParameters.WorkArea.Width / 2) - (win.ActualWidth / 2);
                    }
                }
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "RussLibrary.NativeMethods.RemoveMenu(System.IntPtr,System.Int32,System.Int32)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "RussLibrary.NativeMethods.DrawMenuBar(System.IntPtr)")]
        public static void RemoveClose(this Window me)
        {
            if (me != null)
            {
                WindowInteropHelper helper = new WindowInteropHelper(me);
                IntPtr windowHandle = helper.Handle; //Get the handle of this window

                IntPtr hmenu = NativeMethods.GetSystemMenu(windowHandle, 0);
                int cnt = NativeMethods.GetMenuItemCount(hmenu);
                //remove the button
                NativeMethods.RemoveMenu(hmenu, cnt - 1, NativeMethods.MF_DISABLED | NativeMethods.MF_BYPOSITION);
                //remove the extra menu line
                NativeMethods.RemoveMenu(hmenu, cnt - 2, NativeMethods.MF_DISABLED | NativeMethods.MF_BYPOSITION);
                NativeMethods.DrawMenuBar(windowHandle);


                if (me.WindowStyle == WindowStyle.SingleBorderWindow)
                {
                    me.WindowStyle = WindowStyle.ToolWindow;
                }
            }
        }
    }
}
