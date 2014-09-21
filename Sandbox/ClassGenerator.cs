using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace Sandbox
{

    public class ClassGenerator
    {

        //conversionrootattribute should be like [XmlConversionRoot(\"{0}\")]
        public ClassGenerator(string nameSpace, string className, bool generateDependencyProperties, string conversionRootAttribute, string conversionPropertyAttribute)
        {
            ClassName = className;
            GenerateDependencyProperties = generateDependencyProperties;
            ConversionRootAttribute = "\t" + conversionRootAttribute;

            if (!string.IsNullOrEmpty(conversionRootAttribute) && !conversionRootAttribute.Contains("{0}"))
            {
                throw new InvalidOperationException("ConversionRootAttribute must have \"{0}\" in it.");
            }
            ConversionPropertyAttribute = "\t\t" + conversionPropertyAttribute;
            if (!string.IsNullOrEmpty(conversionPropertyAttribute) && !conversionPropertyAttribute.Contains("{0}"))
            {
                throw new InvalidOperationException("ConversionPropertyAttribute must have \"{0}\" in it.");
            }

        }
        string ClassName { get; set; }
        string ConversionRootAttribute { get; set; }
        string ConversionPropertyAttribute{get;set;}
        bool GenerateDependencyProperties { get; set; }
        string NameSpace { get; set; }
        const string ClassPrefix1 =
          "using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Windows;\r\nusing System.Collections.ObjectModel;\r\n"
          + "using System.Text;\r\nusing System.Reflection;\r\nusing System.Xml;\r\nusing RussLibrary.Xml;\r\nusing RussLibrary;\r\n"
          + "namespace xx~~xx\r\n"
          + "{\r\n";
         
        const string ClassPrefixFormat2 = "\tpublic class {0} : {1}\r\n\t";

        const string ClassPrefix3 = "{\r\n";

          
        public string GetClassPrefix(string conversionRootParameter, string inheritedClass)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ClassPrefix1.Replace("xx~~xx", NameSpace));

            sb.AppendFormat(ClassPrefixFormat2, ClassName, inheritedClass);
            sb.AppendLine();
            if (!string.IsNullOrEmpty(ConversionRootAttribute))
            {
                sb.AppendFormat(conversionRootParameter, conversionRootParameter);
                sb.AppendLine();
            }
            sb.AppendLine(ClassPrefix3);
            
            return sb.ToString();
        }

        public string GetDependencyProperty(string propertyName, string propertyType, string conversionPropertyParameter)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendFormat(DependencyPropertyFormat1, ClassName, propertyName, propertyType);

            if (!string.IsNullOrEmpty(ConversionPropertyAttribute))
            {
                sb.AppendFormat(ConversionPropertyAttribute, conversionPropertyParameter);
                sb.AppendLine();
            }

            sb.AppendFormat(DependencyPropertyFormat1a, propertyName, propertyType);

            sb.Append(DependencyProperty2);
            sb.AppendFormat(DependencyPropertyFormat3, propertyName, propertyType);
            sb.Append(DependencyProperty4);
            sb.AppendFormat(DependencyPropertyFormat5, propertyName);
            sb.Append(DependencyProperty6);
            

            return sb.ToString();
        }

        const string DependencyPropertyFormat1 =
            "\t\tpublic static readonly DependencyProperty {1}Property = \r\n"
           + "\t\t\tDependencyProperty.Register(\"{1}\", typeof({2}),\r\n"
           + "\t\t\ttypeof({0}));\r\n";

        const string DependencyPropertyFormat1a = "\t\tpublic {1} {0}\r\n";

        const string DependencyProperty2 =
           "\t\t{\r\n"
           + "\t\t\tget\r\n"
           + "\t\t\t{\r\n"
           + "\t\t\t\treturn (";

        const string DependencyPropertyFormat3 = "{1})this.UIThreadGetValue({0}Property);\r\n";

        const string DependencyProperty4 =
           "\t\t\t}\r\n"
           + "\t\t\tset\r\n"
           + "\t\t\t{\r\n";
        const string DependencyPropertyFormat5 = "\t\t\t\tthis.UIThreadSetValue({0}Property, value);\r\n";

        const string DependencyProperty6 = "\t\t\t}\r\n"
           + "\t\t}\r\n\r\n";


        public string GetProperty(string propertyName, string propertyType, string conversionPropertyParameter)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(ConversionPropertyAttribute))
            {
                sb.AppendFormat(ConversionPropertyAttribute, conversionPropertyParameter);
                sb.AppendLine();
            }
            sb.AppendFormat(PropertyFormat1, propertyName, propertyType);
            sb.Append(Property2);
            return sb.ToString();
            
        }

        public string GetClassSuffix()
        {
            return ClassSuffix;
        }
        const string ClassSuffix = "\t}\r\n}";
      

        const string PropertyFormat1 = "\t\tpublic {1} {0} ";
        const string Property2 = "{ get; set; }\r\n\r\n";

        
    }
}
