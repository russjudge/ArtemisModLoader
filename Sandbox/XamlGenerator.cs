using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.IO;
using System.Diagnostics;

namespace Sandbox
{

    public static class XamlGenerator
    {

         static string GetLabelContent(string propertyName)
        {
            StringBuilder label = new StringBuilder();
            label.Append(propertyName[0]);
            for (int i = 1; i < propertyName.Length; i++)
            {
                if (propertyName[i].CompareTo('A') >= 0 && propertyName[i].CompareTo('Z') <= 0)
                {
                    label.Append(" ");
                }
                label.Append(propertyName[i]);
            }
            return label.ToString();

        }
        public static void ProcessFileForGrid(string path)
        {
            int row = 0;
             StringBuilder sb = new StringBuilder();
                
                using (StreamReader sr = new StreamReader(path))
                {
                    //public double NetworkPort
                    string sLine = string.Empty;
                    while (sLine != null)
                    {
                        sLine = sr.ReadLine();
                        if (sLine != null)
                        {
                            string wrkLine = sLine.Replace("\t", string.Empty).Trim();
                            if (wrkLine.StartsWith("public "))
                            {

                                //count spaces.  More than 2, skip
                                int spaceC = 0;
                                for (int i = 0; i < wrkLine.Length; i++)
                                {
                                    if (wrkLine[i] == ' ')
                                    {
                                        spaceC++;
                                    }
                                }
                                if (spaceC == 2)
                                {

                                    string wrk2 = wrkLine.Substring(wrkLine.IndexOf(' ')).Trim();
                                    int i = wrk2.IndexOf(' ');
                                    if (i > -1)
                                    {
                                        string type = wrk2.Substring(0, i).Trim();
                                        string propertyName = wrk2.Substring(i).Trim();


                                        sb.AppendLine(XamlGenerator.GetGridSegment(GetLabelContent(propertyName), propertyName, type, row++, 0));
                                    }
                                }
                            }
                        }
                    }
                }
                StringBuilder ut = new StringBuilder();
                ut.AppendLine("<ScrollViewer VerticalScrollBarVisibility=\"Auto\">");
                ut.AppendLine("<Grid>");
                ut.AppendLine("<Grid.ColumnDefinitions>");
                ut.AppendLine("<ColumnDefinition />");
                ut.AppendLine("<ColumnDefinition />");
                ut.AppendLine("</Grid.ColumnDefinitions>");
                ut.AppendLine("<Grid.RowDefinitions>");
                for (int i = 0; i <= row; i++)
                {
                    ut.AppendLine("<RowDefinition />");
                }
                ut.AppendLine("</Grid.RowDefinitions>");
                ut.AppendLine(sb.ToString());
                ut.AppendLine("</Grid>");
                ut.AppendLine("</ScrollViewer>");
                string XamlFile = System.IO.Path.GetTempFileName();
                using (StreamWriter sw = new StreamWriter(XamlFile))
                {
                    sw.WriteLine(ut.ToString());
                }
                Process.Start("Notepad.exe", string.Format("\"{0}\"", XamlFile));
        }
        public static string GetStackPanelSegment(string LabelContent, string BindingPropertyName, string type)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<StackPanel Orientation=\"Horizontal\">");



            sb.AppendLine(GetLabelSegment(LabelContent, string.Empty));

            if (type == "string")
            {

                sb.AppendLine(GetTextBoxSegment(BindingPropertyName, string.Empty));
            }
            else if (type == "double" || type == "int")
            {
                sb.AppendLine(GetDecimalBoxSegment(BindingPropertyName, string.Empty));

            }
            else if (type == "bool")
            {
                sb.AppendLine(GetCheckBoxSegment(BindingPropertyName, string.Empty));

            }


            sb.AppendLine("</StackPanel>");

            return sb.ToString();
        }

        public static string GetGridSegment(string LabelContent, string BindingPropertyName, string type, int gridRow, int initialGridColumn)
        {
            StringBuilder sb = new StringBuilder();
            string GridRow = string.Format("Grid.Row=\"{0}\" ", gridRow.ToString());

            string GridCol1 = string.Format("Grid.Column=\"{0}\" ", initialGridColumn.ToString());
            string GridCol2 = string.Format("Grid.Column=\"{0}\" ", (initialGridColumn + 1).ToString());
            sb.AppendLine(GetLabelSegment(LabelContent, GridRow + GridCol1));

            if (type == "string")
            {

                sb.AppendLine(GetTextBoxSegment(BindingPropertyName, GridRow + GridCol2));
            }
            else if (type == "double" || type == "int")
            {
                sb.AppendLine(GetDecimalBoxSegment(BindingPropertyName, GridRow + GridCol2));

            }
            else if (type == "bool")
            {
                sb.AppendLine(GetCheckBoxSegment(BindingPropertyName, GridRow + GridCol2));

            }

            return sb.ToString();
        }
        public static string GetLabelSegment(string LabelContent, string Modifier)
        {
            return string.Format("<Label {1} Content=\"{0}\" FontWeight=\"Bold\" VerticalAlignment=\"Center\" />\r\n", LabelContent, Modifier);
        }
        public static string GetTextBoxSegment(string BindingPropertyName, string Modifier)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<ctl:ValidationTextBox {0} MinWidth=\"70\" HorizontalAlignment=\"Stretch\" ", Modifier);
            sb.Append("Text=\"{Binding Path=");

            sb.AppendFormat("{0}, Mode=TwoWay, ElementName=uc", BindingPropertyName);
            sb.Append("}\"");
            sb.AppendLine(" VerticalAlignment=\"Center\" />\r\n");
            return sb.ToString();
        }
        public static string GetDecimalBoxSegment(string BindingPropertyName, string Modifier)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<ctl:DecimalBox {0} MinWidth=\"70\" HorizontalAlignment=\"Stretch\" ", Modifier);
            sb.Append("Value=\"{Binding Path=");

            sb.AppendFormat("{0}, Mode=TwoWay, ElementName=uc", BindingPropertyName);
            sb.AppendLine("}\" VerticalAlignment=\"Center\" />\r\n");
            return sb.ToString();
        }
        public static string GetCheckBoxSegment(string BindingPropertyName, string Modifier)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<CheckBox {0} ", Modifier);
            sb.Append("IsChecked=\"{Binding Path=");

            sb.AppendFormat("{0}, Mode=TwoWay, ElementName=uc", BindingPropertyName);
            sb.AppendLine("}\" VerticalAlignment=\"Center\" />\r\n");
            return sb.ToString();
        }
    }
}
