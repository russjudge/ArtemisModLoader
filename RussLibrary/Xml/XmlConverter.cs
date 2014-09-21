using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using log4net;
using System.Reflection;
using System.Collections;
using RussLibrary;
using RussLibrary.WPF;
using System.Windows.Threading;
using System.Windows;

namespace RussLibrary.Xml
{
    public static class XmlConverter
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(XmlConverter));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static XmlDocument LoadXmlFile(string path)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            XmlDocument XmlDataFile = new XmlDocument();
            XmlDataFile.PreserveWhitespace = true;
            string xmlData = null;

            try
            {
                xmlData = XmlSanitizer.GetXmlString(path);
                XmlDataFile.LoadXml(xmlData);
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                {
                    _log.Error("Error loading XmlDocument: " + path, ex);
                }
                XmlDataFile = null;
      
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return XmlDataFile;
            
        }
        
        static void BuildXmlDocument(XmlDocument doc, string name, object value, XmlNode currentNode)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }

          

            foreach (System.Attribute attr in value.GetType().GetCustomAttributes(true))
            {
                  XmlCommentAttribute nodeAttribute = attr as XmlCommentAttribute;
                  if (nodeAttribute != null && !string.IsNullOrEmpty(nodeAttribute.Comment))
                  {
                      XmlComment cm = doc.CreateComment(nodeAttribute.Comment);
                      currentNode.AppendChild(cm);
                  }
            }
            
                //now need to identify value type.  If is value type, then create attribute to add to current node.
                if (value.GetType() == typeof(int)
                    || value.GetType() == typeof(decimal)
                    || value.GetType() == typeof(string)
                    || value.GetType() == typeof(double)
                    || value.GetType() == typeof(short)
                    || value.GetType() == typeof(byte)
                    || value.GetType() == typeof(long)
                    || value.GetType() == typeof(float)
                    || value.GetType() == typeof(bool))
                {
                    XmlAttribute attrib = doc.CreateAttribute(name);

                    if (value != null)
                    {
                        string val = value.ToString();
                        if (!string.IsNullOrEmpty(val))
                        {
                            attrib.Value = value.ToString();

                            currentNode.Attributes.Append(attrib);
                        }
                    }

                }
                else if (value.GetType().ImplementsIList())
                {


                    IList itemList = (IList)value;
                    foreach (object val in itemList)
                    {

                        BuildXmlDocument(doc, name, val, currentNode);
                    }
                }
                else
                {
                    XmlElement newElem = doc.CreateElement(name);
                    //if (ConvertObjectToXml(value, doc, newElem))
                    //{
                        ConvertObjectToXml(value, doc, newElem);
                        currentNode.AppendChild(newElem);
                    //}
                }

            
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        }
        static object ThreadGetValue(this PropertyInfo property, object value)
        {
            object retVal = null;

            if (value.GetType().InheritsOrIs(typeof(Dispatcher)) 
                && Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            {
                retVal = Application.Current.Dispatcher.Invoke(new Func<PropertyInfo, object, object>(ThreadGetValue), property, value);
            }
            else
            {
                retVal =  property.GetValue(value, null);
            }
            return retVal;
        }
        static void ThreadSetValue(this PropertyInfo property, object obj, object value)
        {
            if (value.GetType().InheritsOrIs(typeof(Dispatcher))
                && Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            {
                Application.Current.Dispatcher.Invoke(new Action<PropertyInfo, object, object>(ThreadSetValue), property, obj, value);
            }
            else
            {
                property.SetValue(obj, value, null);
            }
        }
        static bool ConvertObjectToXml(object conversionObject, XmlDocument dataDocument, XmlNode currentNode)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = true;
            if (conversionObject != null)
            {


                foreach (PropertyInfo prop in conversionObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {


                    foreach (System.Attribute attr in prop.GetCustomAttributes(true))
                    {
                        XmlConversionAttribute nodeAttribute = attr as XmlConversionAttribute;
                        if (nodeAttribute != null)
                        {

                            //if the property is an IList, need to go through elements.
                            object propObject = prop.ThreadGetValue(conversionObject);
                            //if (CanInclude(prop.PropertyType, propObject))
                            //{

                            if (propObject != null)
                            {
                                BuildXmlDocument(dataDocument, nodeAttribute.NodeNameMap, propObject, currentNode);
                            }
                            //}
                            //else
                            //{
                            //    retVal = false;
                            //}
                            break;
                        }
                    }
                }
                IXmlStorage storage = conversionObject as IXmlStorage;

                if (storage != null)
                {
                    AddStorageElements(currentNode, storage, dataDocument);
                }



            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        static XmlNode GetNodeFromObject(object value, XmlDocument dataFile)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            XmlNode node = null;
            XmlComment cm = null;
            IXmlStorage storage = value as IXmlStorage;

            if (value != null)
            {
                foreach (Attribute attrib in value.GetType().GetCustomAttributes(true))
                {
                    XmlConversionRootAttribute rootAttrib = attrib as XmlConversionRootAttribute;
                    if (rootAttrib != null)
                    {
                        node = dataFile.CreateElement(rootAttrib.RootNodeName);
                       
                    }
                    XmlCommentAttribute comment = attrib as XmlCommentAttribute;
                    if (comment != null)
                    {
                        cm = dataFile.CreateComment(comment.Comment);

                    }
                }
                if (node != null && cm != null)
                {
                    node.AppendChild(cm);
                }
                if (node != null)
                {

                    AddStorageElements(node, storage, dataFile);
                    
                }

            }
            
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return node;
        }
        static void AddStorageElements(XmlNode parentNode, IXmlStorage storage, XmlDocument dataFile)
        {
            if (storage != null && storage.Storage != null)
            {
                foreach (XmlNode nd in storage.Storage)
                {
                    XmlAttribute aa = nd as XmlAttribute;
                    if (aa == null)
                    {
                      
                        XmlComment comm = nd as XmlComment;
                        if (comm == null)
                        {
                            XmlWhitespace whitespace = nd as XmlWhitespace;
                            if (whitespace == null)
                            {
                                XmlElement newNode = dataFile.CreateElement(nd.Name);
                                newNode.InnerXml = nd.InnerXml;

                                parentNode.AppendChild(newNode);
                            }
                            else
                            {
                                //if (whitespace.Data != "\r\n")
                                //{
                                //    //XmlWhitespace newWhitespace = dataFile.CreateWhitespace(whitespace.Data);
                                //    //parentNode.AppendChild(newWhitespace);
                                //}
                            }
                        }
                        else
                        {
                            if (comm.Value.StartsWith("Created by Artemis Mod Loader", StringComparison.OrdinalIgnoreCase)
                                || comm.Value.StartsWith("Modified by Artemis Mod Loader", StringComparison.OrdinalIgnoreCase))
                            {
                            }
                            else
                            {
                                XmlComment newComm = dataFile.CreateComment(comm.Data);
                                parentNode.AppendChild(newComm);
                            }

                        }
                    }
                    else
                    {
                        XmlAttribute newAttrib = dataFile.CreateAttribute(nd.Name);
                        newAttrib.Value = aa.Value;
                        parentNode.Attributes.Append(newAttrib);
                    }
                }
            }
        }

        static string GetExpectedRootNode(Type baseType)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string retVal = null;
            if (baseType != null)
            {
                foreach (System.Attribute attr in baseType.GetCustomAttributes(true))
                {
                    if (attr.GetType() == typeof(XmlConversionRootAttribute))
                    {
                        retVal = ((XmlConversionRootAttribute)attr).RootNodeName;
                        break;
                    }

                }

            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0} and returning \"{1}\"", MethodBase.GetCurrentMethod().ToString(), retVal); }
            return retVal;
        }

        static Dictionary<string, PropertyInfo> LoadXmlConversionProperties(Type conversionType)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Dictionary<string, PropertyInfo> retVal = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo prop in conversionType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {


                foreach (System.Attribute attr in prop.GetCustomAttributes(true))
                {
                    if (attr.GetType() == typeof(XmlConversionAttribute))
                    {
                        retVal.Add(((XmlConversionAttribute)attr).NodeNameMap, prop);
                        break;
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Datafile")]
        public static void ToObject(XmlDocument xmlDatafile, object baseObject)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (baseObject != null)
            {
                
                if (xmlDatafile != null)
                {

                    XmlElement elem = xmlDatafile.DocumentElement;
                    LoadProperties(elem, baseObject);
                }
            }
            
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            

        }
        public static void ToObject(string xmlPath, object baseObject)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            
            if (!string.IsNullOrEmpty(xmlPath) && System.IO.File.Exists(xmlPath))
            {
                XmlDocument doc = LoadXmlFile(xmlPath);
                if (doc != null)
                {
                    ToObject(doc, baseObject);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            
        }
        /// <summary>
        /// Converts the loaded XmlDataFile to the specified object.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parms")]
        public static object ToObject(XmlDocument xmlDataFile, Type conversionType, params object[] constructorParms)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            object ConversionObject = null;
            if (conversionType != null && xmlDataFile != null)
            {

                ConversionObject = conversionType.GetInstance(constructorParms);
                ToObject(xmlDataFile, ConversionObject);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return ConversionObject;
        }
        public static object ToObject(string xmlPath, Type conversionType, params object[] constructorParms)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            object ConversionObject = null;
            if (!string.IsNullOrEmpty(xmlPath) && System.IO.File.Exists(xmlPath))
            {
                XmlDocument doc = LoadXmlFile(xmlPath);
                if (doc != null)
                {

                    ConversionObject = ToObject(doc, conversionType, constructorParms);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return ConversionObject;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "XmlConversionRootAttribute")]
        static void LoadProperties( XmlNode node, object conversionObject)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string expectedRootNode = GetExpectedRootNode(conversionObject.GetType());
            if (!string.IsNullOrEmpty(expectedRootNode) && node.Name != expectedRootNode )
            {
                throw new XmlConversionException("XmlConversionRootAttribute does not match the root node of the document. Expected: " + expectedRootNode + ", Actual: " + node.Name);
            }

            Dictionary<string, PropertyInfo> ConversionDictionary = LoadXmlConversionProperties(conversionObject.GetType());
            IXmlStorage store = conversionObject as IXmlStorage;
            IList listItem = conversionObject as IList;
            Type elementType = null;
            if (listItem != null)
            {
                elementType = listItem.GetElementType();
            }
            //Node children first, then attributes.
            foreach (XmlNode nd in node.ChildNodes)
            {
                if (listItem != null)
                {
                   
                    object newElement = elementType.GetInstance();


                    LoadProperties(nd, newElement);

                    listItem.Add(newElement);


                }

                if (ConversionDictionary.ContainsKey(nd.Name))
                {
                    PropertyInfo prop = ConversionDictionary[nd.Name];
                    object conv = prop.ThreadGetValue(conversionObject);
                    if (conv == null)
                    {
                        conv = prop.PropertyType.GetInstance();
                        prop.ThreadSetValue(conversionObject, conv);
                    }
                    IList lItem = conv as IList;
                    Type elemType = null;
                    if (lItem != null)
                    {
                        if (nd.Value == null && nd.Attributes.Count == 0 && nd.ChildNodes.Count == 0)
                        {
                        }
                        else
                        {
                            elemType = lItem.GetElementType();

                            object elem = elemType.GetInstance();
                            LoadProperties(nd, elem);

                            lItem.Add(elem);

                        }
                    }
                    else
                    {
                        //?? If is IList, loop here?
                        LoadProperties(nd, conv);
                    }
                }
                else
                {
                   
                    if (store != null && store.Storage !=null)
                    {
                        store.Storage.Add(nd);
                    }
                }
            }
            foreach (XmlAttribute attribute in node.Attributes)
            {
                if (ConversionDictionary.ContainsKey(attribute.Name))
                {
                    PropertyInfo prop = ConversionDictionary[attribute.Name];
                    SetPropertyValue(conversionObject, prop, attribute.Value);
                }
                else if (store != null && store.Storage !=null)
                {
                    store.Storage.Add(attribute);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static void SetPropertyValue(object ObjectItem, PropertyInfo property, string value)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (property.PropertyType == typeof(bool))
            {
                bool b = false;
                if (bool.TryParse(value, out b))
                {
                    property.ThreadSetValue(ObjectItem, b);
                }
            }
            else if (property.PropertyType == typeof(byte))
            {
                byte b = 0;
                if (byte.TryParse(value, out b))
                {
                    property.ThreadSetValue(ObjectItem, b);
                }
            }
            else if (property.PropertyType == typeof(short))
            {
                short b = 0;
                if (value.Contains('.'))
                {
                    value = value.Substring(0, value.IndexOf('.'));
                }
                if (short.TryParse(value, out b))
                {
                    property.ThreadSetValue(ObjectItem, b);
                }
            }
            else if (property.PropertyType == typeof(int))
            {
                int b = 0;
                if (value.Contains('.'))
                {
                    value = value.Substring(0, value.IndexOf('.'));
                }
                if (int.TryParse(value, out b))
                {
                    property.ThreadSetValue(ObjectItem, b);
                }
            }
            else if (property.PropertyType == typeof(long))
            {
                long b = 0;
                if (value.Contains('.'))
                {
                    value = value.Substring(0, value.IndexOf('.'));
                }
                if (long.TryParse(value, out b))
                {
                    property.ThreadSetValue(ObjectItem, b);
                }
            }
            else if (property.PropertyType == typeof(double))
            {
                double b = 0;
                if (double.TryParse(value, out b))
                {
                    property.ThreadSetValue(ObjectItem, b);
                }
            }
            else if (property.PropertyType == typeof(decimal))
            {
                decimal b = 0;
                if (decimal.TryParse(value, out b))
                {
                    property.ThreadSetValue(ObjectItem, b);
                }
            }
            else
            {
                if (property.ImplementsIList())
                {
                    IList l = property.ThreadGetValue(ObjectItem) as IList;

                    if (l == null)
                    {
                        l = property.PropertyType.GetInstance() as IList;
                    }
                    if (l != null)
                    {
                        Type t = l.GetElementType();
                        if (t.InheritsOrIs (typeof(ChangeDependencyObject)))
                        {
                            ChangeDependencyObject newObj = t.GetInstance(value) as ChangeDependencyObject;
                            l.Add(newObj);
                        }
                        else
                        {
                            l.Add(value);
                        }
                    }
                    
                }
                else
                {
                    property.ThreadSetValue(ObjectItem, value);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Endng {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object")]
        public static XmlDocument ToXmlDocument(object conversionObject, bool addXmlDeclaration)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }

            XmlDocument XmlDataFile = new XmlDocument();
            if (addXmlDeclaration)
            {
                XmlDataFile.AppendChild(XmlDataFile.CreateNode(XmlNodeType.XmlDeclaration, "", ""));
            }
            XmlNode node = GetNodeFromObject(conversionObject, XmlDataFile);


            //if (ConvertObjectToXml(conversionObject, XmlDataFile, node))
            //{
            ConvertObjectToXml(conversionObject, XmlDataFile, node);
                XmlDataFile.AppendChild(node);
            //}
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return XmlDataFile;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object")]
        public static XmlDocument ToXmlDocument(object conversionObject)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            XmlDocument doc = ToXmlDocument(conversionObject, false);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return doc;

        }
    }
}
