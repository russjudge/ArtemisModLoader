using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Xml;

namespace RussLibrary
{

    public class XmlToClassGenerator : XmlBase
    {

        //TODO: this class needs finished, planned usage for vesselData.xml
       // static readonly ILog _log = LogManager.GetLogger(typeof(XmlToClassGenerator));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Class")]
        public XmlToClassGenerator(string xmlFile, string ClassName) : base(xmlFile)
        {
            data = new StringBuilder();
            data.AppendLine("using System;\r\nusing System.Collections;\r\nusing System.Collections.Generic;\r\nusing System.Reflection;");
            data.AppendLine("using System.Windows;\r\nusing System.Xml;\r\nnamespaceRussLibrary\r\n{\r\n");
            data.AppendFormat("\tpublic class {0} : XmlBase\r\n", ClassName);
            data.AppendFormat("\r\n\r\n\t\tpublic {0}(XmlNode node) : base(node)\r\n", ClassName);
            data.AppendLine("{ }\r\n");
            Class = ClassName;
            Process(WorkDocument.DocumentElement);
            data.AppendLine("\t}\r\n}");
        }
        string Class;
        StringBuilder data = null;
        void Process(XmlNode node)
        {
           
            foreach (XmlAttribute attrib in node.Attributes)
            {
                //Determine type by best guess.
                string typename = string.Empty;
                bool bl;
                byte b;
                short s;
                int i;
                long l;
                double dbl;
                decimal d;
                if (bool.TryParse(attrib.Value, out  bl))
                {
                    typename="bool";
                }
                else if (byte.TryParse(attrib.Value, out b))
                {
                    typename = "byte";
                }
                else if (short.TryParse(attrib.Value, out s))
                {
                    typename = "short";
                }
                else if (int.TryParse(attrib.Value, out i))
                {
                    typename = "int";
                }
                else if (long.TryParse(attrib.Value, out l))
                {
                    typename = "long";
                }
                else if (double.TryParse(attrib.Value, out dbl))
                {
                    typename = "double";
                }
                else if (decimal.TryParse(attrib.Value, out d))
                {
                    typename = "decimal";
                }
                else
                {
                    typename = "string";
                }
                data.AppendFormat("\t\tpublic static readonly DependencyProperty {0}Property =\r\n\t\t\t"
                    + "DependencyProperty.Register(\"{0}\", typeof({1}),\r\n\t\t\t"
                    + "typeof({2}), new UIPropertyMetadata(OnItemChanged));\r\n\r\n", attrib.Name, typename, Class);
                data.AppendFormat("\t\tpublic {1} {2}\r\n", typename, attrib.Name);
                data.AppendLine("{\r\n\t\t\tget\r\n\t\t\t{");
                data.AppendFormat("\t\t\t\treturn ({0}this.GetValue({1}Property);", typename, attrib.Name);
                data.AppendLine("\t\t\t}\r\n\t\t\tset\r\n\t\t\t{");
                data.AppendFormat("\t\t\t\tthis.SetValue({0}Property, value);\r\n", attrib.Name);
                data.AppendLine("\t\t\t}\r\n\t\t}\r\n");
            }

            //<node attrib="value">
            //  <child />
            //  <child />
            //</node>

            //resulting class:
            // class node :XmlBase
            // {
            //    string attrib{get;set;}
            //    Collection<XmlBase> child
            // }
            if (node.ChildNodes != null)
            {
                string working;
                foreach (XmlNode nd in node.ChildNodes)
                {
                    //if node has no child nodes and only attributes, then is okay
                    // if node as all child nodes and no attributes, then this property is collection<xmlbase>.
                    //if node as child nodes and attributes, then is XmlBase, with a collection property in it.


                    Process(nd);
                }
            }
        }
        
        //Attribute:



        //public int Sequence
        //{
        //    get
        //    {
        //        return (int)this.UIThreadGetValue(SequenceProperty);

        //    }
        //    private set
        //    {
        //        this.UIThreadSetValue(SequenceProperty, value);

        //    }
        //}
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
