using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.Controls;
using ICSharpCode.AvalonEdit.CodeCompletion;
using RussLibrary;
using RussLibrary.Helpers;
using System.IO;
using MissionStudio.Helpers;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;

namespace MissionStudio
{
    public class ScriptControl : XmlEditor
    {
        public static void AddButtonRowContent(UIElement control)
        {
            EditorWindow.AddButtonContent(control);
        }
        public static void SetArtemisInstallPath(string path)
        {
            Locations.ArtemisInstallPath = path;
        }
        public static void Show()
        {
            ScriptControl sc = null;
            
            try
            {
                sc = new ScriptControl();
                ScriptletManager sm = new ScriptletManager();
                sm.Editor = sc;
                EditorWindow.Show("Mission Studio", null, sc, sm, ImageHelper.ConvertBitmapToBitmapSource(Properties.Resources._1360035698_run));
                sc = null;
            }
            finally
            {
                if (sc != null)
                {
                    sc.Dispose();
                }
            }

        }
        public static void Show(string file)
        {
            ScriptControl sc = null;
            try
            {
                sc = new ScriptControl();
                ScriptletManager sm = new ScriptletManager();
                sm.Editor = sc;
                EditorWindow.Show("Mission Studio", file, sc, sm, ImageHelper.ConvertBitmapToBitmapSource(Properties.Resources._1360035698_run));
                sc = null;
            }
            finally
            {
                if (sc != null)
                {
                    sc.Dispose();
                }
            }
        }
      
        static readonly string TemplatePath = System.IO.Path.Combine(Locations.DataPath, "MissionData", "Templates");
        static readonly string ProjectTemplate = System.IO.Path.Combine(TemplatePath, "NewProjectTemplate.xml");

        private ScriptControl()
        {
            InitialDirectory = Locations.MissionPath;
            InitializeProjectTemplate();
            this.Content.Document.Text = LoadTemplate(ProjectTemplate);
            
            int i = 0;
            if (!string.IsNullOrEmpty(Content.Document.Text))
            {
                i = Content.Document.Text.IndexOf("<start>", StringComparison.OrdinalIgnoreCase);
                
            }
            if (i > -1)
            {
                this.Content.CaretOffset = i + 7;
            }
            LoadContextMenu();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(ScriptControl_DragEnter);
            this.DragOver += new DragEventHandler(ScriptControl_DragOver);
            this.Drop += new DragEventHandler(ScriptControl_Drop);
            this.Dispatcher.BeginInvoke(new Action(InitializeFocus), System.Windows.Threading.DispatcherPriority.Loaded);

        }

        void ScriptControl_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.StringFormat)
             && !e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.All;
            }
        }

        void ScriptControl_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.StringFormat)
                && !e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.All;
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "insertText")]
        void ScriptControl_Drop(object sender, DragEventArgs e)
        {
            
            /*
            
             * Dropped text could be in format:
             * <start>innerxml</start><event>innerxml</event>
             * 
             * rule is: <start> items merge with <start>, <event> gets on its own at insert point, outside an event.
             * 
             * if all that scriptlet has is <event>, then all inside gets inserted at insert point, without <event> unless outside <event>,<start>
             
            */
            
           if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files != null)
                {
                    foreach (string f in files)
                    {
                        InsertFile(f);
                    }
                    e.Handled = true;
                }
            }
            

            //if (!string.IsNullOrEmpty(insertText))
            //{
            //    Content.Document.Insert(Content.CaretOffset, insertText);
            //}
        }
        public void InsertFile(string file)
        {
            string data = null;
            using (StreamReader sr = new StreamReader(file))
            {
                data = sr.ReadToEnd();
            }
            //e.GetPosition(Content.Document);
            //Need to work iwth mouse position instead of caret.
            InsertData(data);
        }
        public void InsertData(string data)
        {
            string wrk = InsertStartTagData(data);
            InsertEventTagData(wrk);
                        
        }
        void InsertEventTagData(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                //Strip out <mission_data and </mission_data>
                data = RemoveMissionDataTag(data);
                //First, how may event tags are in data?  If more than one, insert all 
                // outside of content's event tags.  If one, insert where at.

                int i = data.IndexOf("<event", StringComparison.OrdinalIgnoreCase);
                if (i >= 0)
                {
                   
                    int j = this.Content.CaretOffset;

                    
                    if (j < Content.Document.Text.Length)
                    {
                        if (IsInEvent(j))
                        {
                            int k = Content.Document.Text.IndexOf("</event>", j, StringComparison.OrdinalIgnoreCase);
                            if (k > -1)
                            {
                                j = k + 8;
                                data = "\r\n\t" + data;
                            }
                        }
                    }
                   
                    Content.Document.Insert(j, data);
                }
                //else
                //{
                //    //Only one or zero events.  If within event tag, remove all events tags from data.
                //    if (IsInEvent(this.Content.CaretOffset))
                //    {
                //        data = RemoveEventTag(data);
                //    }
                //    else
                //    {
                //        //not in event tag--does data have event tag? if not add it in.
                //        if (data.IndexOf("<event", StringComparison.OrdinalIgnoreCase) < 0)
                //        {
                //            while (data.StartsWith("\t", StringComparison.OrdinalIgnoreCase))
                //            {
                //                data = data.Substring(1);
                //            }

                //            data = "\t<event>\r\n\t\t" + data;
                //        }
                //        if (data.IndexOf("</event>", StringComparison.OrdinalIgnoreCase) < 0)
                //        {
                //            data += "\r\n\t</event>\r\n";
                //        }
                //    }

                //    Content.Document.Insert(Content.CaretOffset, data);
                //}
            }
        }
        static string RemoveEventTag(string data)
        {
            string retVal = data;
            int i = data.IndexOf("<event", StringComparison.OrdinalIgnoreCase);
            if (i > -1)
            {
                int j = data.IndexOf(">", i, StringComparison.OrdinalIgnoreCase);
                if (j >= data.Length)
                {
                    retVal = data.Substring(0, i);
                }
                else
                {
                    retVal = data.Substring(0, i) + data.Substring(j + 1);
                }
            }
            i = retVal.IndexOf("</event>", StringComparison.OrdinalIgnoreCase);
            if (i > -1)
            {
                int j = i + 15;
                if (j >= retVal.Length)
                {
                    retVal = retVal.Substring(0, i);
                }
                else
                {
                    retVal = retVal.Substring(0, i) + retVal.Substring(i + j);
                }
            }
            return retVal;
        }
        static string RemoveMissionDataTag(string data)
        {
            string retVal = data;
            int i = data.IndexOf("<mission_data", StringComparison.OrdinalIgnoreCase);
            if (i > -1)
            {
                int j = data.IndexOf(">", i, StringComparison.OrdinalIgnoreCase);
                if (j >= data.Length)
                {
                    retVal = data.Substring(0, i);
                }
                else
                {
                    retVal = data.Substring(0, i) + data.Substring(j + 1);
                }
            }
            i = retVal.IndexOf("</mission_data>", StringComparison.OrdinalIgnoreCase);
            if (i > -1)
            {
                int j = i + 15;
                if (j >= retVal.Length)
                {
                    retVal = retVal.Substring(0, i);
                }
                else
                {
                    retVal = retVal.Substring(0, i) + retVal.Substring(j);
                }
            }
            return retVal;
        }
        static string ExtractStartDataData(string data, int newStart, int endStart)
        {
            string retVal = string.Empty;
            if (!string.IsNullOrEmpty(data) && newStart > -1 && endStart >= newStart)
            {
                if (newStart > 0)
                {
                    retVal = data.Substring(0, newStart);
                }
                retVal += data.Substring(endStart + 1);
            }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "ICSharpCode.AvalonEdit.Document.TextDocument.Insert(System.Int32,System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "missiondata")]
        string InsertStartTagData(string data)
        {
            string retVal = data;
            if (!string.IsNullOrEmpty(data))
            {
                string textInsert = string.Empty;
                //First look for <start> tag:
                int newStart = data.IndexOf("<start", StringComparison.OrdinalIgnoreCase);
                int wrkStart = newStart;
                if (newStart > -1)
                {
                    newStart = data.IndexOf(">", newStart, StringComparison.OrdinalIgnoreCase) + 1;
                    int endStart = data.IndexOf("</start>", newStart, StringComparison.OrdinalIgnoreCase);
                    if (endStart < 0)
                    {
                        endStart = data.Length - 1;
                    }
                    int wrkEnd = endStart + 7;
                    textInsert = data.Substring(newStart, endStart - newStart + 1);
                    retVal = ExtractStartDataData(data, wrkStart, wrkEnd);

                    int pos = Content.Document.Text.IndexOf("</start>", StringComparison.OrdinalIgnoreCase);
                    if (pos < 0)
                    {
                        // + 
                        textInsert = textInsert + "\r\n\t</start>";
                        pos = Content.Document.Text.IndexOf("<event", StringComparison.OrdinalIgnoreCase);
                        if (pos < -1)
                        {
                            pos = Content.Document.Text.IndexOf("<start>", StringComparison.OrdinalIgnoreCase);
                            if (pos >= 0)
                            {
                                pos += 7;
                            }
                            else
                            {
                                //No start.

                                textInsert = "\t<start>\r\n" + textInsert;
                                pos = Content.Document.Text.IndexOf("<mission_data", StringComparison.OrdinalIgnoreCase);
                                if (pos < 0)
                                {
                                    textInsert = "<mission_data version=\"1.69\">" + textInsert;
                                    pos = 0;
                                }
                                else
                                {
                                    int pos2 = Content.Document.Text.IndexOf(">", pos, StringComparison.OrdinalIgnoreCase);
                                    if (pos2 >= 0)
                                    {
                                        pos2++;
                                    }
                                    else
                                    {
                                        pos2 = pos + 14;
                                        textInsert = "\r\n" + textInsert;
                                    }

                                }
                            }
                        }
                    }

                    Content.Document.Insert(pos, textInsert);
                }
            }
            return retVal;
        }
        void InitializeFocus()
        {
            this.Content.Focus();
            FocusManager.SetFocusedElement(this, Content);
        }
        void LoadContextMenu()
        {
            var mnu = new ScriptContextMenu();
            this.ContextMenu =mnu as ContextMenu;
        }
        static string LoadTemplate(string file)
        {
            string retVal = null;
            if (!string.IsNullOrEmpty(file) && System.IO.File.Exists(file))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    retVal = sr.ReadToEnd();
                }
            }
            return retVal;
        }
        static void InitializeProjectTemplate()
        {
            FileHelper.CreatePath(TemplatePath);
            if (!File.Exists(ProjectTemplate))
            {
                using (StreamWriter sw = new StreamWriter(ProjectTemplate))
                {
                    sw.WriteLine(Properties.Resources.NewProjectTemplate);
                }
            }
        }
       
        
//        StringBuilder buildCommand;
//        int caretStart = 0;
       
        bool IsWithinTag(string tag, int offset)
        {
            bool retVal = false;
            int i = Content.Document.Text.LastIndexOf(string.Format(CultureInfo.InvariantCulture, "<{0}", tag), offset, StringComparison.OrdinalIgnoreCase);
            if (i > -1)
            {
                int j = Content.Document.Text.LastIndexOf(string.Format(CultureInfo.InvariantCulture, "</{0}>", tag), offset, StringComparison.OrdinalIgnoreCase);
                if (j < i)
                {
                    retVal = true;
                }
            }
            return retVal;
        }
        bool IsInEvent(int offset)
        {
            bool retVal = IsWithinTag("event", offset);
            if (!retVal)
            {
                retVal = IsWithinTag("start", offset);
            }
            return retVal;
        }
        bool CheckedForValueCompletion = false;
        //static readonly ILog _log = LogManager.GetLogger(typeof(ScriptControl));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "RussLibrary.XmlCompletionData.#ctor(System.String)")]
        protected override void OnTextEntered(System.Windows.Input.TextCompositionEventArgs e)
        {
            base.OnTextEntered(e);
            //Code completion here.
            //"<" entered, command completion offered.
            if (e != null)
            {
                if (e.Text.EndsWith("<", StringComparison.OrdinalIgnoreCase))
                {
                    CheckedForValueCompletion = false;
                    if (IsInEvent(Content.CaretOffset))
                    {
                        List<ICompletionData> data = new List<ICompletionData>();
                        foreach (string cmd in Commands.Current.CommandDictionary.Keys)
                        {
                            data.Add(Commands.Current.CommandDictionary[cmd]);
                        }
                        AddCompletionData(data);
                        
                        //buildCommand = new StringBuilder();
                        //caretStart = Content.CaretOffset;
                    }
                    else
                    {
                        List<ICompletionData> data = new List<ICompletionData>();
                        data.Add(new XmlCompletionData("event"));
                        AddCompletionData(data);
                    }
                }
                else
                {
                    if (completionWindow == null && XmlHelper.IsInTag(Content.Document.Text, Content.CaretOffset))
                    {
                        //int iWrk = 0;
                        ////char prevChar = '~';
                        //if (Content.CaretOffset > 0)
                        //{
                        //    iWrk = Content.CaretOffset;
                        //    //prevChar = Content.Document.Text[iWrk - 1]; 
                        //}
                        
                        
                        if (!XmlHelper.IsWithinQuotes(Content.Document.Text, Content.CaretOffset)) // && caretStart + buildCommand.ToString().Length == Content.CaretOffset)
                        {
                            if (char.IsLetterOrDigit(e.Text[e.Text.Length - 1]) || e.Text[e.Text.Length - 1] == ' ')
                            {
                                //find command and lookup in dictionary.
                                string nd = XmlHelper.GetNodeName(XmlHelper.GetLastNode(Content.Document.Text, Content.CaretOffset));
                                if (Commands.Current.CommandDictionary.ContainsKey(nd))
                                {
                                    CommandElement cmd1 = Commands.Current.CommandDictionary[nd] as CommandElement;

                                    List<ICompletionData> data = new List<ICompletionData>();
                                    foreach (AttributeElement cmd in cmd1.GetSortedAttributes())
                                    {
                                        data.Add(cmd);
                                    }
                                    AddCompletionData(data);
                                    CheckedForValueCompletion = false;
                                }
                            }
                        }
                        else
                        {
                            if (!CheckedForValueCompletion)
                            {
                                if (char.IsLetterOrDigit(e.Text[e.Text.Length - 1]) || e.Text[e.Text.Length - 1] == ' '
                                    || ("-+*/^%.".Contains(e.Text[e.Text.Length -1])))
                                {
                                    NoCodeCompleteCharacters = "-+*/^%.";
                                    DoValueEntering();
                                }
                            }
                            
                        }
                    }

                }
            }
        }

        void DoValueEntering()
        {
            //add value completion list.
            if (!CheckedForValueCompletion)
            {
                //if (char.IsLetterOrDigit(text[text.Length - 1]) || text[text.Length - 1] == ' ')
                //{
                //find attribute (we are within quote)
                string attrib = null;
                int i = Content.Document.Text.LastIndexOf(" ", Content.CaretOffset, StringComparison.OrdinalIgnoreCase);
                if (i > -1 && XmlHelper.IsInTag(Content.Document.Text, i))
                {
                    int j = Content.Document.Text.LastIndexOf("=", Content.CaretOffset, StringComparison.OrdinalIgnoreCase);
                    if (j > i)
                    {
                        attrib = Content.Document.Text.Substring(i + 1, j - i - 1).Trim();
                    }
                }
                string nd = XmlHelper.GetNodeName(XmlHelper.GetLastNode(Content.Document.Text, Content.CaretOffset));

                if (Commands.Current.CommandDictionary.ContainsKey(nd))
                {
                    CommandElement cmd1 = Commands.Current.CommandDictionary[nd] as CommandElement;

                    AttributeElement elem = null;
                    foreach (AttributeElement cmd in cmd1.Attributes)
                    {
                        if (cmd.Text == attrib)
                        {
                            elem = cmd;
                            break;
                        }

                    }

                    if (elem != null)
                    {
                        if (elem.AttributeType == AttributeType.ExpressionValue)
                        {
                            List<string> variables = GetVariables();
                            variables.Sort();
                            List<ICompletionData> data = new List<ICompletionData>();
                            string lastVar = string.Empty;
                            foreach (string v in variables)
                            {
                                if (v != lastVar)
                                {
                                    data.Add(new XmlCompletionData(v));
                                    lastVar = v;
                                }
                            }
                            AddCompletionData(data);
                        }
                        else
                        {
                            //If the attribute starts "name", get list of names from <create.
                            if (elem.Text.StartsWith("name", StringComparison.OrdinalIgnoreCase))
                            {
                                List<string> variables = GetNames();
                                variables.Sort();
                                List<ICompletionData> data = new List<ICompletionData>();
                                string lastVar = string.Empty;
                                foreach (string v in variables)
                                {
                                    if (v != lastVar)
                                    {
                                        data.Add(new XmlCompletionData(v));
                                        lastVar = v;
                                    }
                                }
                                AddCompletionData(data);
                            }
                            else if (elem.Values != null)
                            {
                                List<ICompletionData> data = new List<ICompletionData>();
                                foreach (XmlCompletionData value in elem.Values)
                                {
                                    data.Add(value);
                                }
                                AddCompletionData(data);
                            }
                        }
                    }

                }
                //}
                CheckedForValueCompletion = true;
            }
        }
        List<string> GetVariables()
        {
            List<string> retVal = new List<string>();
            //<set_variable name="s"
            int i = 0;
            do
            {
                i = Content.Document.Text.IndexOf("<set_variable", i, StringComparison.OrdinalIgnoreCase);
                if (i > -1)
                {
                    int j = Content.Document.Text.IndexOf(" name", i, StringComparison.OrdinalIgnoreCase);
                    if (j > -1)
                    {
                        int k = Content.Document.Text.IndexOf("\"", j, StringComparison.OrdinalIgnoreCase);
                        if (k > -1)
                        {
                            int l = Content.Document.Text.IndexOf("\"", k + 1, StringComparison.OrdinalIgnoreCase);
                            string text = Content.Document.Text.Substring(k + 1, l - k - 1);
                            retVal.Add(text);
                            i = l;
                        }
                    }
                }
            } while (i > -1);
            return retVal;
        }
        List<string> GetNames()
        {
            List<string> retVal = new List<string>();
            //<set_variable name="s"
            int i = 0;
            do
            {
                i = Content.Document.Text.IndexOf("<create", i, StringComparison.OrdinalIgnoreCase);
                if (i > -1)
                {
                    int j = Content.Document.Text.IndexOf(" name", i, StringComparison.OrdinalIgnoreCase);
                    if (j > -1)
                    {
                        int k = Content.Document.Text.IndexOf("\"", j, StringComparison.OrdinalIgnoreCase);
                        if (k > -1)
                        {
                            int l = Content.Document.Text.IndexOf("\"", k + 1, StringComparison.OrdinalIgnoreCase);
                            string text = Content.Document.Text.Substring(k + 1, l - k - 1);
                            retVal.Add(text);
                            i = l;
                        }
                    }
                }
            } while (i > -1);
            return retVal;
        }
        protected override void OnTextEntering(System.Windows.Input.TextCompositionEventArgs e)
        {
            base.OnTextEntering(e);
            if (CodeCompleted && e != null && XmlHelper.IsWithinQuotes(Content.Document.Text, Content.CaretOffset))
            {
                int i = Content.Document.Text.IndexOf('\"', Content.CaretOffset);
                if (i > -1)
                {
                    Content.CaretOffset = i + 1;
                }
            }
            if (e != null && XmlHelper.IsWithinQuotes(Content.Document.Text, Content.CaretOffset))
            {
                DoValueEntering();
            }
            else if (XmlHelper.IsInTag(Content.Document.Text, Content.CaretOffset))
            {
                //Need attributes.

            }

        }
    }


}

