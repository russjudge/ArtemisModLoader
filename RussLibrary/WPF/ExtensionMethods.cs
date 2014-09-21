using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace RussLibrary.WPF
{

    public static class ExtensionMethods
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(ExtensionMethods));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        public static void AcceptChanges(this IList<ChangeDependencyObject> collection)
        {
            if (collection != null)
            {
                ChangeDependentCollection<ChangeDependencyObject> testItem = collection as ChangeDependentCollection<ChangeDependencyObject>;
                if (testItem != null)
                {
                    testItem.AcceptChanges();
                }
                else
                {
                    foreach (ChangeDependencyObject item in collection)
                    {
                        item.AcceptChanges();
                    }
                }
            }
        }

        public static void RejectChanges(this IList<ChangeDependencyObject> collection)
        {
            if (collection != null)
            {
                ChangeDependentCollection<ChangeDependencyObject> testItem = collection as ChangeDependentCollection<ChangeDependencyObject>;
                if (testItem != null)
                {
                    testItem.RejectChanges();
                }
                else
                {
                    foreach (ChangeDependencyObject item in collection)
                    {
                        item.RejectChanges();
                    }
                }
            }
        }

        public static void BeginInitialization(this IList<ChangeDependencyObject> collection)
        {
            if (collection != null)
            {
                ChangeDependentCollection<ChangeDependencyObject> testItem = collection as ChangeDependentCollection<ChangeDependencyObject>;
                if (testItem != null)
                {
                    testItem.BeginInitialization();
                }
                else
                {

                    foreach (ChangeDependencyObject item in collection)
                    {
                        item.BeginInitialization();
                    }
                }
            }
        }

        public static void EndInitialization(this IList<ChangeDependencyObject> collection)
        {
            if (collection != null)
            {
                ChangeDependentCollection<ChangeDependencyObject> testItem = collection as ChangeDependentCollection<ChangeDependencyObject>;
                if (testItem != null)
                {
                    testItem.EndInitialization();
                }
                else
                {
                    foreach (ChangeDependencyObject item in collection)
                    {
                        item.EndInitialization();
                    }
                }
            }
        }
    }
}
