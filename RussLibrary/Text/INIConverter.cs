using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Globalization;

namespace RussLibrary.Text
{

    /// <summary>
    /// Works like XmlConverter--loads same way, but only merges to existing file (or saves new).  Will not destroy existing file, but
    /// only merges data from objects with [INIConversionAttribute].
    /// 
    /// Only works for simple objects (top-level only.  Does not iterate down a tree like XMLconverter does.)
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INI")]
    public static class INIConverter
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(INIConverter));
        //
        //Standard INI file syntax:

        //";" at beginning of line is a comment.
        //";name=" is a commented-out parameter that is using default values.
        //"; " should be ignored for processing.
        //parameter=value.  
        //both parameter and value are strings.
        //

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ipath"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IN")]
        public static object ToObject(string INIpath, object value)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        
            if (value != null)
            {
                INIContainer container = new INIContainer(INIpath);
            
                foreach (PropertyInfo prop in value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    foreach (System.Attribute attr in prop.GetCustomAttributes(true))
                    {
                        INIConversionAttribute nodeAttribute = attr as INIConversionAttribute;
                        if (nodeAttribute != null)
                        {
                            if (container.Values.ContainsKey(nodeAttribute.INIParameterName))
                            {
                                INIKeyValueItem item = container.Values[nodeAttribute.INIParameterName];
                                if (!item.UseDefault)
                                {
                                    if (prop.PropertyType == typeof(bool))
                                    {

                                        if (item.Value == "1")
                                        {
                                            prop.SetValue(value, true, null);
                                        }
                                        else if (item.Value == "0")
                                        {
                                            prop.SetValue(value, false, null);
                                        }
                                        else
                                        {
                                            bool b = false;
                                            if (bool.TryParse(item.Value, out b))
                                            {
                                                prop.SetValue(value, b, null);
                                            }
                                        }
                                    }
                                    else if (prop.PropertyType == typeof(byte))
                                    {
                                        byte b = 0;
                                        if (byte.TryParse(item.Value, out b))
                                        {
                                            prop.SetValue(value, b, null);
                                        }
                                    }
                                    else if (prop.PropertyType == typeof(short))
                                    {
                                        short b = 0;
                                        if (short.TryParse(item.Value, out b))
                                        {
                                            prop.SetValue(value, b, null);
                                        }
                                    }
                                    else if (prop.PropertyType == typeof(int))
                                    {
                                        int b = 0;
                                        if (int.TryParse(item.Value, out b))
                                        {
                                            prop.SetValue(value, b, null);
                                        }
                                    }
                                    else if (prop.PropertyType == typeof(long))
                                    {
                                        long b = 0;
                                        if (long.TryParse(item.Value, out b))
                                        {
                                            prop.SetValue(value, b, null);
                                        }
                                    }
                                    else if (prop.PropertyType == typeof(double))
                                    {
                                        double b = 0;
                                        if (double.TryParse(item.Value, out b))
                                        {
                                            prop.SetValue(value, b, null);
                                        }
                                    }
                                    else if (prop.PropertyType == typeof(decimal))
                                    {
                                        decimal b = 0;
                                        if (decimal.TryParse(item.Value, out b))
                                        {
                                            prop.SetValue(value, b, null);
                                        }
                                    }
                                    else
                                    {
                                        prop.SetValue(value, item.Value, null);

                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return value;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "T"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ipath"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IN"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "T")]
        public static object ToObject(string INIpath, Type T, params object[] constructorParameters)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            object retVal = null;
            if (T != null)
            {
    
                retVal = ToObject(INIpath, T.GetInstance(constructorParameters));
               
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IN"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INI"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ipath")]
        public static void ToINI(object value, string INIpath)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            INIContainer container = new INIContainer(INIpath);
            if (value != null)
            {


                foreach (PropertyInfo prop in value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {


                    foreach (System.Attribute attr in prop.GetCustomAttributes(true))
                    {
                        INIConversionAttribute nodeAttribute = attr as INIConversionAttribute;
                        if (nodeAttribute != null)
                        {
                         
                            object propObject = prop.GetValue(value, null);
                            
                            
                          
                            if (propObject != null)
                            {
                                string val = propObject.ToString();
                                if (prop.PropertyType == typeof(bool))
                                {
                                 
                                    val = (bool)propObject ? "1" : "0";
                                }
                               
                                INIKeyValueItem item = new INIKeyValueItem(nodeAttribute.INIParameterName, val, false);
                                container.UpdateEntry(item);
                            }
                            else
                            {

                                if (container.Values.ContainsKey(nodeAttribute.INIParameterName))
                                {
                                    container.Values[nodeAttribute.INIParameterName].UseDefault = true;
                                }
                                else
                                {
                                    INIKeyValueItem item = new INIKeyValueItem(nodeAttribute.INIParameterName, string.Empty, true);
                                    container.UpdateEntry(item);
                                }
                            }
                            
                        }
                    }
                }
                container.SaveFile(INIpath);
            }


            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            
        }


        //

    }
}
