using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Xml;
using RussLibrary.Xml;
using System.Collections.ObjectModel;
using System.IO;

namespace Sandbox
{

    public class XmlClassGenerator
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(XmlClassGenerator));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

       
        List<string> ClassesCreated = null;
        public string TargetDirectoryPath { get; set; }
        public bool CreateDependencyObjects { get; set; }
        string DependencyObjectString = string.Empty;
        public void GenerateClasses(string xmlDocumentPath, string targetDirectoryPath, bool makeDependencyObject)
        {
            ClassesCreated = new List<string>();
            TargetDirectoryPath = targetDirectoryPath;
            CreateDependencyObjects = makeDependencyObject;
            if (makeDependencyObject)
            {
                DependencyObjectString = " : DependencyObject";
            }
            else
            {
                DependencyObjectString = string.Empty;
            }
            XmlDocument doc = XmlConverter.LoadXmlFile(xmlDocumentPath);
            
            //Each Node is a class, each Attribute is a property.
            ProcessNode(doc.DocumentElement);
        }
        void ProcessNode(XmlNode node)
        {

            //produces one class, to be saved.

            StringBuilder sb = new StringBuilder();
            string className = GetClassName(node.Name);
            bool addedHere = false;
            bool PossibleCollection = false;
            if (!ClassesCreated.Contains(className))
            {
                ClassesCreated.Add(className);
                addedHere = true;
                if (node.NextSibling != null)
                {
                    PossibleCollection = (node.NextSibling.Name == node.Name);
                }
            }
            ClassGenerator cg = new ClassGenerator("~~~~~~",  className,  true, "[XmlConversionRoot(\"{0}\")]", "[XmlConversion(\"{0}\")]");
            sb.Append(cg.GetClassPrefix(node.Name, DependencyObjectString));
           


            string PropertyName = null;
            string TypeName = null;

            if (node.Attributes != null)
            {
               
                foreach (XmlAttribute attrib in node.Attributes)
                {
                    PropertyName = GetClassName(attrib.Name);
                    TypeName = "string";
                    //if (PossibleCollection)
                    //{
                    //    if (CreateDependencyObjects)
                    //    {
                    //        TypeName = "ObservableCollection<" + PropertyName + ">";
                    //    }
                    //    else
                    //    {
                    //        TypeName = "List<" + PropertyName + ">";
                    //    }
                    //    PropertyName = PropertyName + "Group";
                        
                    //}
                    if (CreateDependencyObjects)
                    {

                        sb.AppendLine(cg.GetDependencyProperty(PropertyName, TypeName, attrib.Name));

                    }
                    else
                    {
                        sb.AppendLine(cg.GetProperty(PropertyName, TypeName, attrib.Name));
                    }
                }
            }
            if (node.ChildNodes != null)
            {
                string LastNodeName = string.Empty;
              
                foreach (XmlNode nd in node.ChildNodes)
                {
                    if (nd.Name != LastNodeName)
                    {
                        bool isCollection = false;
                        if (nd.NextSibling != null)
                        {
                            isCollection = (nd.NextSibling.Name == nd.Name);
                        }
                        TypeName = GetClassName(nd.Name);
                        PropertyName = TypeName + "Object";
                        if (isCollection)
                        {
                            PropertyName = TypeName + "Group";
                            if (CreateDependencyObjects)
                            {
                                TypeName = "ObservableCollection<" + TypeName + ">";
                            }
                            else
                            {
                                TypeName = "List<" + TypeName + ">";
                            }


                        }
                        if (CreateDependencyObjects)
                        {
                            sb.AppendLine(cg.GetDependencyProperty(PropertyName, TypeName, nd.Name));
                           

                        }
                        else
                        {
                            sb.AppendLine(cg.GetProperty(PropertyName, TypeName, nd.Name));
                            
                        }
                        ProcessNode(nd);
                        LastNodeName = nd.Name;
                    }
                }
            }
            sb.AppendLine(cg.GetClassSuffix());
            if (addedHere)
            {
                if (File.Exists(className + ".cs"))
                {
                    File.Delete(className + ".cs");
                }
                using (StreamWriter sw = new StreamWriter(Path.Combine(TargetDirectoryPath, className + ".cs")))
                {
                    sw.WriteLine(sb.ToString());
                }
            }
        }
        
        public static string GetClassName(string nodeName)
        {
            
            string wrk = nodeName.Substring(1);
            
            while (wrk.Contains("_"))
            {
                string wrk2 = string.Empty;
                int i = wrk.IndexOf('_');
                int j = i + 1;
                int k = j + 1;
                if (j < wrk.Length)
                {
                    if (k < wrk.Length)
                    {
                        wrk2 = wrk.Substring(k);
                    }
                    wrk = wrk.Substring(0, i) + wrk[j].ToString().ToUpperInvariant() + wrk2;
                }
            }

            return nodeName[0].ToString().ToUpperInvariant() + wrk;
        }
        //const string DependencyPropertyFormat = 
        //    "\t\t[XmlConversion(\"{2}\")]\r\n"
        //    + "\t\tpublic static readonly DependencyProperty {1}Property = \r\n"
        //    + "\t\t\tDependencyProperty.Register(\"{1}\", typeof({3}),\r\n"
        //    + "\t\t\ttypeof({0}));\r\n"
        //    + "\t\tpublic {3} {1}\r\n"
        //    + "\t\t{\r\n"
        //    + "\t\t\tget\r\n"
        //    + "\t\t\t{\r\n"
        //    + "\t\t\t\treturn ({3})this.UIThreadGetValue({1}Property);\r\n"
        //    + "\t\t\t}\r\n"
        //    + "\t\t\tset\r\n"
        //    + "\t\t\t{\r\n"
        //    + "\t\t\t\tthis.UIThreadSetValue({1}Property, value);"
        //    + "\t\t\t}\r\n"
        //    + "\t\t}\r\n\r\n";
        //const string DependencyPropertyFormat1 =
        //   "\t\t[XmlConversion(\"{2}\")]\r\n"
        //   + "\t\tpublic static readonly DependencyProperty {1}Property = \r\n"
        //   + "\t\t\tDependencyProperty.Register(\"{1}\", typeof({3}),\r\n"
        //   + "\t\t\ttypeof({0}));\r\n"
        //   + "\t\tpublic {3} {1}\r\n";
        //const string DependencyProperty2 =
        //   "\t\t{\r\n"
        //   + "\t\t\tget\r\n"
        //   + "\t\t\t{\r\n"
        //   + "\t\t\t\treturn (";
    
        //const string DependencyPropertyFormat3 = "{1})this.UIThreadGetValue({0}Property);\r\n";

        //const string DependencyProperty4 =
        //   "\t\t\t}\r\n"
        //   + "\t\t\tset\r\n"
        //   + "\t\t\t{\r\n";
        //const string DependencyPropertyFormat5 = "\t\t\t\tthis.UIThreadSetValue({0}Property, value);";

        //const string DependencyProperty6 = "\t\t\t}\r\n"
        //   + "\t\t}\r\n\r\n";


        //const string PropertyFormat = "\t\tpublic {1} {0} { get; set; }\r\n\r\n";

        //const string PropertyFormat1 = "\t\tpublic {1} {0} ";
        //const string Property2 = "{ get; set; }\r\n\r\n";
        

        //const string ClassPrefixFormat = 
        //    "using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Collections.ObjectModel;\r\n"
        //    + "using System.Text;\r\nusing System.Reflection;\r\nusing System.Xml;\r\nusing RussLibrary.Xml;\r\nusing RussLibrary;\r\n"
        //    + "namespace xxx\r\n"
        //    + "{\r\n"
        //    + "\t[XmlConversionRoot(\"{1}\")]\r\n"
        //    + "\tpublic class {0} {2}\r\n\t{\r\n";


        //const string ClassSuffix = "\t}\r\n}";


    }
}
