using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Xml;
using log4net;

namespace RussLibrary
{
    [Obsolete("Use XmlConverter instead.")]
    public class XmlBase : DependencyObject
    {
        
        protected XmlBase() { }

        public string MyClass
        {
            get
            {
                string tp = this.GetType().ToString();
                return tp.Substring(tp.LastIndexOf('.') + 1);

            }
        }
        static readonly ILog _log = LogManager.GetLogger(typeof(XmlBase));

        protected XmlBase(XmlNode node)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (node != null)
            {
                LoadNodeAttributesToObjectProperties(node);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public void Save()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (string.IsNullOrEmpty(workFile))
            {
                throw new InvalidOperationException("Class was not created with a file--cannot save directly.");
            }
            else
            {
                Save(workFile);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        string workFile = null;
        public void Save(string file)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            WorkDocument = new XmlDocument();
            WorkDocument.AppendChild(WorkDocument.CreateNode(XmlNodeType.XmlDeclaration, "", ""));
            WorkDocument.AppendChild(GetXmlNode(WorkDocument));

            WorkDocument.Save(file);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        }
   
        protected XmlDocument WorkDocument { get; private set; }
        protected XmlBase(string path)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (!string.IsNullOrEmpty(path))
            {
                WorkDocument = new System.Xml.XmlDocument();
                if (System.IO.File.Exists(path))
                {
                    WorkDocument.Load(path);
                    LoadNodeAttributesToObjectProperties(WorkDocument.DocumentElement);
                }
                else
                {
                    //Make sure all Collections are created.
                    foreach (PropertyInfo prop in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {

                        Type[] fT = prop.PropertyType.FindInterfaces(TypeFilter.Equals, typeof(IList));
                        if (fT.Length > 0)
                        {
                            prop.SetValue(this, prop.PropertyType.GetInstance(), null);

                        }

                    }
                }
                workFile = path;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "elem")]
        public static XmlElement GetXmlNode(XmlElement elem, XmlBase item, XmlDocument workDocument)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            XmlAttribute attrib = null;
            //Now need to append attributes.
            if (elem != null && item != null && workDocument != null)
            {
                foreach (PropertyInfo prop in item.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public))
                {
                    
                    bool CanProcess = false;
                    foreach (System.Attribute attr in prop.GetCustomAttributes(true))
                    {
                        if (attr is XmlBaseAttribute)
                        {
                            CanProcess = true;
                            break;
                        }

                    }
                    if (CanProcess)
                    {
                        if (prop.PropertyType.BaseType == typeof(XmlBase))
                        {
                            XmlBase wrk = (XmlBase)prop.GetValue(item, null);
                            if (wrk != null)
                            {

                                elem.AppendChild(GetXmlNode(workDocument.CreateElement(prop.Name), wrk, workDocument));
                            }
                        }
                        else
                        {
                            if (prop.ImplementsIList())
                            {
                                IList propList = (IList)prop.GetValue(item, null);

                                if (propList != null && propList.Count > 0)
                                {
                                    XmlElement elemCollection = workDocument.CreateElement(prop.Name);
                                    elem.AppendChild(elemCollection);
                                    foreach (XmlBase xx in propList)
                                    {
                                        if (xx != null)
                                        {
                                            elemCollection.AppendChild(GetXmlNode(workDocument.CreateElement(xx.MyClass), xx, workDocument));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                attrib = workDocument.CreateAttribute(prop.Name);
                                object val = prop.GetValue(item, null);
                                if (val != null)
                                {
                                    attrib.Value = val.ToString();

                                    elem.Attributes.Append(attrib);
                                }

                            }
                        }
                    }
                }
                if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            }
            return elem;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public XmlElement GetXmlNode(XmlDocument workDocument)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            XmlElement elem = null;
            if (workDocument != null)
            {
                elem = GetXmlNode(workDocument.CreateElement("", MyClass, ""), this, workDocument);
                

            }

      
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return elem;
        }
        
        //static object GetInstance(Type t, params object[] constructorParms)
        //{
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //    object retVal = null;
        //    List<Type> constructorSignature = new List<Type>();
        //    foreach (object p in constructorParms)
        //    {
        //        constructorSignature.Add(p.GetType());
        //    }

        //    if (t != null)
        //    {
        //        ConstructorInfo constructor = t.GetConstructor(constructorSignature.ToArray());
        //        if (constructor == null)
        //        {
        //            throw new ArgumentException("Constructor not found for type " + t.ToString());
        //        }
        //        retVal = constructor.Invoke(constructorParms);
        //    }
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        //    return retVal;
        //}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "XmlBase")]
        static void NodeToCollection(XmlNode nd, IList propertyItem)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (nd.ChildNodes != null && nd.ChildNodes.Count > 0)
            {


                Type ElementType = propertyItem.GetElementType();
                if (ElementType.BaseType == typeof(XmlBase))
                {
                    foreach (XmlNode childNode in nd.ChildNodes)
                    {
                        if (ElementType.Name == childNode.Name)
                        {
                            AddToCollection(ElementType.GetInstance(childNode), propertyItem);

                        }

                    }
                }
                else
                {
                    throw new InvalidOperationException("Only types of XmlBase supported in collections.");
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static void AddToCollection(object Instance, IList propertyItem)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            MethodInfo[] methodItems = propertyItem.GetType().GetMethods();
            foreach (MethodInfo m in methodItems)
            {
                if (m.Name == "Add")
                {
                    object[] signature = { Instance };
                    m.Invoke(propertyItem, signature);

                    break;
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        void ProcessAttributes(XmlAttributeCollection Attributes, Dictionary<string, PropertyInfo> properties)
        {
            if (Attributes != null)
            {
                foreach (XmlAttribute item in Attributes)
                {

                    //Only valid types for attributes are:
                    // strings
                    // bool
                    // decimals
                    //     (basically only imutable or instances.  No reference objects).
                    if (properties.ContainsKey(item.Name))
                    {
                        if (properties[item.Name].PropertyType == typeof(bool))
                        {
                            bool b = false;
                            if (bool.TryParse(item.Value, out b))
                            {
                                properties[item.Name].SetValue(this, b, null);
                            }
                        }
                        else if (properties[item.Name].PropertyType == typeof(byte))
                        {
                            byte b = 0;
                            if (byte.TryParse(item.Value, out b))
                            {
                                properties[item.Name].SetValue(this, b, null);
                            }
                        }
                        else if (properties[item.Name].PropertyType == typeof(short))
                        {
                            short b = 0;
                            if (short.TryParse(item.Value, out b))
                            {
                                properties[item.Name].SetValue(this, b, null);
                            }
                        }
                        else if (properties[item.Name].PropertyType == typeof(int))
                        {
                            int b = 0;
                            if (int.TryParse(item.Value, out b))
                            {
                                properties[item.Name].SetValue(this, b, null);
                            }
                        }
                        else if (properties[item.Name].PropertyType == typeof(long))
                        {
                            long b = 0;
                            if (long.TryParse(item.Value, out b))
                            {
                                properties[item.Name].SetValue(this, b, null);
                            }
                        }
                        else if (properties[item.Name].PropertyType == typeof(double))
                        {
                            double b = 0;
                            if (double.TryParse(item.Value, out b))
                            {
                                properties[item.Name].SetValue(this, b, null);
                            }
                        }
                        else if (properties[item.Name].PropertyType == typeof(decimal))
                        {
                            decimal b = 0;
                            if (decimal.TryParse(item.Value, out b))
                            {
                                properties[item.Name].SetValue(this, b, null);
                            }
                        }
                        else
                        {

                            properties[item.Name].SetValue(this, item.Value, null);
                        }
                    }

                }
            }
        }
        void ProcessNodeChildren(XmlNodeList children, Dictionary<string, PropertyInfo> properties)
        {
            if (children != null)
            {
                foreach (XmlNode nd in children)
                {

                    //thinking--nd.Name = SubMods.  matching Property of "this" is Collection<SubMod> SubMods.

                    //Node names will match reference object properties.
                    if (properties.ContainsKey(nd.Name))
                    {
                        //SubMod
                        PropertyInfo prop = properties[nd.Name];
                        


                        if (prop.PropertyType.BaseType == typeof(XmlBase))
                        {
                            var Instance = prop.PropertyType.GetInstance(nd);

                            prop.SetValue(this, Instance, null);
                        }
                        else if (prop.ImplementsIList())
                        {

                            NodeToCollection(nd, (IList)prop.GetValue(this, null));

                        }
                        else
                        {
                            prop.SetValue(this, prop.PropertyType.GetInstance(), null);
                        }
                    }
                }
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "XmlBase"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "XmlNode")]
        protected void LoadNodeAttributesToObjectProperties(XmlNode node)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            //Validate node is correct
            string TypeBase = this.GetType().ToString();
            TypeBase = TypeBase.Substring(TypeBase.LastIndexOf('.') + 1);
          

            Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo prop in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                 bool CanProcess = false;
                    foreach (System.Attribute attr in prop.GetCustomAttributes(true))
                    {
                        if (attr is XmlBaseAttribute)
                        {
                            CanProcess = true;
                            break;
                        }

                    }
                    if (CanProcess)
                    {
                        properties.Add(prop.Name, prop);
                        
                        if (prop.ImplementsIList())
                        {
                            prop.SetValue(this, prop.PropertyType.GetInstance(), null);

                        }
                    }

            }
            //node.Name must = name of base class.
            if (node != null)
            {
                ProcessNodeChildren(node.ChildNodes, properties);

                ProcessAttributes(node.Attributes, properties);
               
            }
            AcceptChanges();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        protected static void OnItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            XmlBase me = sender as XmlBase;
            if (me != null)
            {
                if (e.NewValue != e.OldValue)
                {
                    me.Changed = true;
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public static readonly DependencyProperty ChangedProperty =
          DependencyProperty.Register("Changed", typeof(bool),
          typeof(XmlBase));

        public bool Changed
        {
            get
            {
                return (bool)this.UIThreadGetValue(ChangedProperty);

            }
            private set
            {
                this.UIThreadSetValue(ChangedProperty, value);

            }
        }

        public XmlBase Original { get; set; }

        public void AcceptChanges()
        {
            Original = this.Copy();
            Changed = false;
        }
        public void RejectChanges()
        {
            if (Original != null)
            {
                this.CopyProperties(Original);
                Changed = false;
            }
        }
       
       
        public XmlBase Copy()
        {
            XmlBase retVal = new XmlBase();
            retVal.CopyProperties(this);
           
            return retVal;
        }
       
    }
}
