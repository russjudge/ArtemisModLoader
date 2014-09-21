using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using log4net;
using Microsoft.Win32;
using RussLibrary.Helpers;
using RussLibrary.Windows;

namespace RussLibrary.Controls
{

    //TODO: Add CommandBindings: Ctl-F (find).  Ctl-E (format document) Ctl-H Ctl-G
    //  Add status line indicating position by line and character.
    // Ctl-S ?? Ctl-O?? Ctl-N??
    //Already integrated: Ctl-x Ctl-v Ctl-c
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:RussLibrary.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:RussLibrary.Controls;assembly=RussLibrary.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:XmlEditor/>
    ///
    /// </summary>
    public class XmlEditor : Control, IDisposable
    {
        #region Static Elements
        static readonly ILog _log = LogManager.GetLogger(typeof(XmlEditor));

        static XmlEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XmlEditor), new FrameworkPropertyMetadata(typeof(XmlEditor)));
          
        }
        #endregion

        #region Constructor and Destructor

        public XmlEditor()
        {
            Content = new TextEditor();

            //LoadCommandBindings();

            Content.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
            Content.Visibility = System.Windows.Visibility.Visible;
            Content.ShowLineNumbers = true;
            Content.Options.ShowTabs = true;
        
            Content.TextArea.Caret.PositionChanged += new EventHandler(Caret_PositionChanged);

            foldingManager = FoldingManager.Install(Content.TextArea);
            foldingStrategy = new XmlFoldingStrategy();
            foldingStrategy.UpdateFoldings(foldingManager, Content.Document);

            fsw = new FileSystemWatcher();
            fsw.Changed += new FileSystemEventHandler(fsw_Changed);

            fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.DirectoryName;

        }

       
        void Caret_PositionChanged(object sender, EventArgs e)
        {
            SetTextLocation();
        }



        private void Subscribe()
        {

            // in the constructor:
            Content.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            Content.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            Content.TextArea.KeyDown += new KeyEventHandler(TextArea_KeyDown);
            Content.TextArea.KeyUp += new KeyEventHandler(TextArea_KeyUp);
            Content.TextArea.MouseWheel += new MouseWheelEventHandler(TextArea_MouseWheel);
            Content.MouseWheel += new MouseWheelEventHandler(TextArea_MouseWheel);

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Subscribe();

            foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(5);
            foldingUpdateTimer.Tick += new EventHandler(foldingUpdateTimer_Tick);
            foldingUpdateTimer.Start();

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        ~XmlEditor()
        {
            this.Dispose(false);
            if (fsw != null)
            {
                fsw.Dispose();
            }
        }

        #endregion


        #region FileSystemWatcher
        FileSystemWatcher fsw = null;
        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            if (MessageBox.Show("Something else has changed the vesselData file.\r\n\r\nDo you wish to reload?",
                "Editor",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                this.Dispatcher.Invoke(new Action<string>(this.Load), DataFile);

            }
        }

        void SetWatcher()
        {
            if (!string.IsNullOrEmpty(DataFile))
            {
                FileInfo f = new FileInfo(DataFile);

                fsw.Path = f.DirectoryName;
                fsw.Filter = f.Name;
                fsw.EnableRaisingEvents = true;

            }
            else
            {
                fsw.EnableRaisingEvents = false;
            }
        }


        
        
        #endregion


        #region Properties
        public string InitialDirectory { get; set; }

        public static readonly DependencyProperty ContentProperty =
         DependencyProperty.Register("Content", typeof(TextEditor),
         typeof(XmlEditor));

        public TextEditor Content
        {
            get
            {
                return (TextEditor)this.UIThreadGetValue(ContentProperty);
            }
            private set
            {
                this.UIThreadSetValue(ContentProperty, value);
            }
        }


        public static readonly DependencyProperty DataFileProperty =
           DependencyProperty.Register("DataFile", typeof(string),
           typeof(XmlEditor));

        public string DataFile
        {
            get
            {
                return (string)this.UIThreadGetValue(DataFileProperty);
            }
            private set
            {
                this.UIThreadSetValue(DataFileProperty, value);
            }
        }
        public static readonly DependencyProperty ErrorListProperty =
         DependencyProperty.Register("ErrorList", typeof(ObservableCollection<string>),
         typeof(XmlEditor));

        public ObservableCollection<string> ErrorList
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(ErrorListProperty);
            }
            set
            {
                this.UIThreadSetValue(ErrorListProperty, value);
            }
        }
        static void OnErrorsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            XmlEditor me = sender as XmlEditor;
            if (me != null)
            {
                me.ErrorList = new ObservableCollection<string>();
                string[] err = me.Errors.Split('\r');
                foreach (string eo in err)
                {
                    if (!string.IsNullOrEmpty(eo.Replace("\n", string.Empty).Trim()))
                    {
                        me.ErrorList.Add(eo.Replace("\n", string.Empty).Trim());
                    }
                }
            }
        }
        public static readonly DependencyProperty ErrorsProperty =
          DependencyProperty.Register("Errors", typeof(string),
          typeof(XmlEditor), new PropertyMetadata(OnErrorsChanged));

        public string Errors
        {
            get
            {
                return (string)this.UIThreadGetValue(ErrorsProperty);
            }
            set
            {
                this.UIThreadSetValue(ErrorsProperty, value);
            }
        }


        #endregion


        #region Load and Save
        internal void DoSavePromptForFilename()
        {
            fsw.EnableRaisingEvents = false;
            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = "Xml Editor";
            diag.Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*";
            diag.FileName = DataFile;
            diag.CheckPathExists = true;
            diag.AddExtension = true;
            diag.DefaultExt = ".xml";
            if (!string.IsNullOrEmpty(DataFile))
            {
                diag.InitialDirectory = new FileInfo(DataFile).DirectoryName;
            }
            else
            {
                if (!string.IsNullOrEmpty(InitialDirectory) && !InitialDirectorySet)
                {
                    InitialDirectorySet = true;
                    diag.InitialDirectory = InitialDirectory;
                }
            }
            if (diag.ShowDialog() == true)
            {
                DataFile = diag.FileName;
                Save();
     

            }
        }
        internal bool DoSavePrompt()
        {
            bool retVal = false;
            if (Content != null && Content.IsModified)
            {

                switch (MessageBox.Show("Do you wish to save?", "Data Changed", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        Save();
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        retVal=true;
                        break;
                }

            } 
            return retVal;
        }

        bool InitialDirectorySet = false;
        internal virtual void Open()
        {
            if (!DoSavePrompt())
            {
                fsw.EnableRaisingEvents = false;
                OpenFileDialog diag = new OpenFileDialog();
                diag.Title = "XML Editor";
                diag.Filter = "Xml Files (*.xml)|*.xml|All Files(*.*)|*.*";
                diag.Multiselect = false;
                diag.CheckFileExists = true;
                if (!string.IsNullOrEmpty(InitialDirectory) && !InitialDirectorySet)
                {
                    InitialDirectorySet = true;
                    diag.InitialDirectory = InitialDirectory;
                }

                if (diag.ShowDialog() == true)
                {
                    Load(diag.FileName);
                }
            }

        }
        internal void Load(string path)
        {
            fsw.EnableRaisingEvents = false;
            if (!string.IsNullOrEmpty(path))
            {
                Content.Load(path);
            }
            else
            {
                Content.Document.Text = string.Empty;
            }
            DataFile = path;
            SetWatcher();
            Content.IsModified = false;
        }
        internal void Save()
        {
            if (!string.IsNullOrEmpty(DataFile))
            {
                fsw.EnableRaisingEvents = false;
                Content.Save(DataFile);
                
                Content.IsModified = false;
            }
        }

        #endregion

        #region Find and Replace

        internal void DoFind()
        {
            FindReplaceDialog diag = new FindReplaceDialog();
            diag.EnableReplace = false;
            diag.FindNext += new EventHandler(diag_FindNext);
            diag.Closed += new EventHandler(diagFind_Closed);
            diag.Show();
        }
        void diag_ReplaceAll(object sender, EventArgs e)
        {
            FindReplaceDialog diag = sender as FindReplaceDialog;
            if (diag != null)
            {
     
                Content.Document.Text = Content.Document.Text.Replace(diag.SearchText, diag.ReplaceText); 
               
            }
        }

        void diag_Replace(object sender, EventArgs e)
        {
            FindReplaceDialog diag = sender as FindReplaceDialog;
            if (diag != null)
            {

                //first--is the currently selected text already matched?
                if (Content.SelectedText == diag.SearchText)
                {
                    Content.SelectedText = diag.ReplaceText;
                    diag_FindNext(sender, e);
                }
                else
                {
                    diag_FindNext(sender, e);
                    if (Content.SelectedText == diag.SearchText)
                    {
                        Content.SelectedText = diag.ReplaceText;
                    }
                   
                }
            }
        }
        void diag_FindNext(object sender, EventArgs e)
        {
            FindReplaceDialog diag  = sender as FindReplaceDialog;
            if (diag != null)
            {
                int i = Content.Document.Text.IndexOf(diag.SearchText, Content.CaretOffset);
                if (i < 0)
                {
                    i = Content.Document.Text.IndexOf(diag.SearchText);
                }
                if (i >= 0)
                {
                    Content.CaretOffset = i;
                    Content.Select(Content.CaretOffset, diag.SearchText.Length);

                }
                else
                {
                    MessageBox.Show(diag.SearchText + " not found.", "Find", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        internal void DoReplace()
        {
            FindReplaceDialog diag = new FindReplaceDialog();
            diag.EnableReplace = true;
            diag.FindNext += new EventHandler(diag_FindNext);
            diag.Replace += new EventHandler(diag_Replace);
            diag.ReplaceAll += new EventHandler(diag_ReplaceAll);
            diag.Closed += new EventHandler(diag_Closed);
            diag.Show();
        }
       

        void diag_Closed(object sender, EventArgs e)
        {
            FindReplaceDialog diag = sender as FindReplaceDialog;
            if (diag != null)
            {
                diag.FindNext -= new EventHandler(diag_FindNext);
                diag.Replace -= new EventHandler(diag_Replace);
                diag.ReplaceAll -= new EventHandler(diag_ReplaceAll);
                diag.Closed -= new EventHandler(diag_Closed);
            }
        }
        void diagFind_Closed(object sender, EventArgs e)
        {
            FindReplaceDialog diag = sender as FindReplaceDialog;
            if (diag != null)
            {
                diag.FindNext -= new EventHandler(diag_FindNext);
                
                diag.Closed -= new EventHandler(diag_Closed);
            }
        }
        #endregion
        StandBy standBy = null;
        #region Formatting
        public void Format()
        {
            Errors = string.Empty;


            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            standBy = new StandBy();
            standBy.Show();
            string text = Content.Document.Text;
            System.Threading.ThreadPool.QueueUserWorkItem(Format, text);
        }
        static object formatLock = new object();
        void Format(object state)
        {
            string data = null;
            string err = string.Empty;
            FormattedXmlResult result = FormatXml(state as string, false, false);
            if (result != null)
            {
                if (result.ResultCode <= FormattedXmlResultCode.Warnings)
                {
                    data = result.ResultXml;
                }

                err = result.ErrorMessages;
                this.Dispatcher.BeginInvoke(new Action<string, string>(UpdateFormatted), data, err);
            }
        }
        void UpdateFormatted(string text, string errors)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Content.Document.Text = text;
            }
            Errors = errors;
            standBy.Close();
        }
        public static FormattedXmlResult FormatXml(string xmlData, bool sortAttributes, bool indentAttributes)
        {
            string formattedXml = xmlData;
            FormattedXmlResult retVal = null;
            bool usesDeclaration = false;
            if (xmlData != null)
            {
                usesDeclaration = (xmlData.Trim().StartsWith("<?"));
            }
            lock (formatLock)
            {
                try
                {
                    FileInfo f = new FileInfo(Assembly.GetExecutingAssembly().Location);
                    StringBuilder parameters = new StringBuilder();
                    parameters.Append("--quiet yes --force-output yes --markup yes");
                    parameters.Append(" --output-xml yes --input-xml yes --wrap-attributes yes --add-xml-decl no -raw");
                    if (sortAttributes)
                    {
                        parameters.Append(" --sort-attributes alpha");
                    }
                    //--indent-attributes yes (1 attribute per line).
                    if (indentAttributes)
                    {
                        parameters.Append(" --indent-attributes yes");
                    }
                    string tidy = System.IO.Path.Combine(f.DirectoryName, "tidy.exe");
                    if (File.Exists(tidy))
                    {
                        StringBuilder outData = new StringBuilder();
                        StringBuilder errData = new StringBuilder();

                        using (System.Diagnostics.Process p =
                            ProcessHelper.Start(
                            tidy, parameters.ToString(), xmlData,
                            delegate(string parm)
                            {
                                outData.AppendLine(parm);
                            },
                            delegate(string parm)
                            {
                                errData.AppendLine(parm);
                            }
                            ))
                        {


                            p.WaitForExit();
                            if (p.ExitCode <= 1)
                            {
                               
                                formattedXml = XmlHelper.Beautify(outData.ToString());
                                if (!usesDeclaration)
                                {
                                    if (formattedXml.Trim().StartsWith("<?"))
                                    {
                                        int i = formattedXml.IndexOf("?>");
                                        if (i > -1)
                                        {
                                            int j = formattedXml.IndexOf("<?");
                                            string prepend = string.Empty;
                                            if (j > 0)
                                            {
                                                prepend = formattedXml.Substring(0, j);
                                            }
                                            formattedXml = prepend + formattedXml.Substring(i + 2);
                                            if (formattedXml.StartsWith("\r\n"))
                                            {
                                                formattedXml = formattedXml.Substring(2);
                                            }
                                        }
                                    }
                                }
                            }

                            retVal = new FormattedXmlResult(formattedXml, errData.ToString(), p.ExitCode);
                        }
                    }
                    else
                    {
                        retVal = new FormattedXmlResult(xmlData, "Xml formatting component (tidy.exe) not found.  Formatting cannot be performed.", 2);
                    }
                }
                catch (Exception ex)
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.Warn("Error formatting XML.", ex);
                    }
                    Application.Current.Dispatcher.BeginInvoke(new Action<string>(ShowError), ex.Message);
                }
            }
            return retVal;
        }
        static void ShowError(string message)
        {
            MessageBox.Show("Problem formatting:\r\n\r\n" + message + "\r\n\r\nPlease try again.  If problem persists, please notify russjudge@gmail.com.", "Xml Editor", MessageBoxButton.OK, MessageBoxImage.Hand);
        }

        #endregion

        #region Command Bindings
        void DoSave(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }
        internal void DoNew()
        {
            if (!DoSavePrompt())
            {
                Content.Document.Text = string.Empty;
                Content.IsModified = false;
            }
        }
        void DoNew(object sender, ExecutedRoutedEventArgs e)
        {
            DoNew();
        }
        void DoSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            DoSavePromptForFilename();
        }
        void DoReplace(object sender, ExecutedRoutedEventArgs e)
        {
            DoReplace();
        }
        void DoFind(object sender, ExecutedRoutedEventArgs e)
        {
            DoFind();
        }
        void DoOpen(object sender, ExecutedRoutedEventArgs e)
        {
            Open();
        }

        void DoGoto(object sender, ExecutedRoutedEventArgs e)
        {

        }

        void LoadCommandBindings()
        {

            
                
            this.CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Save, new ExecutedRoutedEventHandler(DoSave)));

            this.CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Open, new ExecutedRoutedEventHandler(DoOpen)));
            
           
            this.CommandBindings.Add(
                new CommandBinding(ApplicationCommands.New, new ExecutedRoutedEventHandler(DoNew)));
            this.CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Find, new ExecutedRoutedEventHandler(DoFind)));
            this.CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Replace, new ExecutedRoutedEventHandler(DoReplace)));
            CommandBinding cmd = new CommandBinding(EditingCommands.AlignCenter, new ExecutedRoutedEventHandler(DoFormat));
            cmd.CanExecute += new CanExecuteRoutedEventHandler(cmd_CanExecute);

            this.CommandBindings.Add(cmd);
            this.CommandBindings.Add(
               new CommandBinding(ApplicationCommands.SaveAs, new ExecutedRoutedEventHandler(DoSaveAs)));

            cmd = new CommandBinding(NavigationCommands.BrowseForward, new ExecutedRoutedEventHandler(DoLocate));
            cmd.CanExecute += new CanExecuteRoutedEventHandler(cmd_CanExecute);

            this.CommandBindings.Add(cmd);
        }

        void cmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void DoFormat(object sender, ExecutedRoutedEventArgs e)
        {
            Format();
        }

        
        void DoLocate(object sender, ExecutedRoutedEventArgs e)
        {
            int line = 0;
            int column = 0;
            string parm = e.Parameter as string;
            if (!string.IsNullOrEmpty(parm))
            {
                //"line xx column yy"
                int i = parm.IndexOf("line ", StringComparison.OrdinalIgnoreCase);
                if (i > -1)
                {
                    int j = parm.IndexOf(" ", i + 5, StringComparison.OrdinalIgnoreCase);
                    if (j > -1)
                    {
                        string k = parm.Substring(i + 5, j - (i + 5));
                        if (int.TryParse(k, out line))
                        {
                            i = parm.IndexOf("column ", StringComparison.OrdinalIgnoreCase);
                            if (i > -1)
                            {
                                j = parm.IndexOf(" ", i + 7, StringComparison.OrdinalIgnoreCase);
                                if (j > -1)
                                {
                                    k = parm.Substring(i + 7, j - (i + 7));
                                    if (int.TryParse(k, out column))
                                    {
                                        Goto(line, column);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
        #endregion
        void Goto(int line, int column)
        {
            Content.CaretOffset = Content.Document.GetOffset(line, column);
            FocusManager.SetFocusedElement(this, Content.TextArea);
            this.Content.Focus();
            Keyboard.Focus(Content.TextArea);
            Content.ScrollToLine(line);
        }




        void SetTextLocation()
        {
            TextLocation location = Content.Document.GetLocation(Content.CaretOffset);

            LineNumber = location.Line;
            Column = location.Column;

        }
        public static readonly DependencyProperty LineNumberProperty =
         DependencyProperty.Register("LineNumber", typeof(int),
         typeof(XmlEditor));

        public int LineNumber
        {
            get
            {
                return (int)this.UIThreadGetValue(LineNumberProperty);
            }
            private set
            {
                this.UIThreadSetValue(LineNumberProperty, value);
            }
        }





        public static readonly DependencyProperty ColumnProperty =
         DependencyProperty.Register("Column", typeof(int),
         typeof(XmlEditor));

        public int Column
        {
            get
            {
                return (int)this.UIThreadGetValue(ColumnProperty);
            }
            private set
            {
                this.UIThreadSetValue(ColumnProperty, value);
            }
        }

        #region Events
        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                foldingStrategy.UpdateFoldings(foldingManager, Content.Document);
            }
        }
       

        void TextArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (controlDown)
            {
                double delta = Convert.ToDouble(e.Delta / Math.Abs(e.Delta));
                double newSize = this.Content.TextArea.FontSize + delta;
                if (newSize > 2 && newSize <= 48)
                {
                    this.Content.TextArea.FontSize = newSize;
                    
                }
               e.Handled = true;
            }

        }

        void TextArea_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
            {
                controlDown = false;
            }
        }
        
        void TextArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                controlDown = true;
            }
        }
        #endregion

        #region Fields

    
        bool controlDown = false;
        protected CompletionWindow completionWindow;
        FoldingManager foldingManager = null;
        XmlFoldingStrategy foldingStrategy = null;
        DispatcherTimer foldingUpdateTimer = null;

        #endregion

        #region Code Completion
        protected void AddCompletionData(IList<ICompletionData> data)
        {
            if (data != null)
            {
                completionWindow = new CompletionWindow(Content.TextArea);
                foreach (ICompletionData d in data)
                {
                    completionWindow.CompletionList.CompletionData.Add(d);
                }
                //IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                //data.Add(new MyCompletionData("Item1"));
                //data.Add(new MyCompletionData("Item2"));
                //data.Add(new MyCompletionData("Item3"));

                completionWindow.SizeToContent = SizeToContent.Width;
                completionWindow.MinWidth = 175;
                //object x = completionWindow.WindowStyle;
                //completionWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }
        }
        int nestingLevel = -1;

        
        protected virtual void OnTextEntered(TextCompositionEventArgs e)
        {
            if (e != null)
            {
                
                if (e.Text.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    
                    //Add code completion for last node--text entered must be "</".  overrides above.

                    if (Content.CaretOffset > 2 && Content.Text[Content.CaretOffset -2] == '<')
                    {
                        int i = Content.Text.LastIndexOf("<", Content.CaretOffset - 3);
                        
                        string node = XmlHelper.GetNodeName(Content.Text.Substring(i));
                        if (!string.IsNullOrEmpty(node))
                        {
                            List<ICompletionData> data = new List<ICompletionData>();
                            data.Add(new XmlCompletionData(node, null, string.Format("</{0}>", node)));
                            AddCompletionData(data);
                        }
                    }
                }
            }
        }
        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            OnTextEntered(e);
        }
        protected bool CodeCompleted = false;
        protected string NoCodeCompleteCharacters = string.Empty;
        protected virtual void OnTextEntering(TextCompositionEventArgs e)
        {
            CodeCompleted = false;
            bool IsDone = false;
            if (e != null)
            {
                if (e.Text.Length > 0 && completionWindow != null)
                {
                    if (completionWindow.CompletionList.ListBox.Items.Count == 0)
                    {
                        completionWindow = null;
                    }
                    if (completionWindow != null)
                    {
                        if (!char.IsLetterOrDigit(e.Text[0])
                            && e.Text[0] != '_' && !NoCodeCompleteCharacters.Contains(e.Text))
                        {
                            // Whenever a non-letter is typed while the completion window is open,
                            // insert the currently selected element.
                            completionWindow.CompletionList.RequestInsertion(e);
                            NoCodeCompleteCharacters = string.Empty;
                            if (e.Text[0] != '>')
                            {
                                IsDone = true;
                            }
                            CodeCompleted = true;
                        }
                    }
                }
                if (e.Text.Length > 0 && !IsDone)
                {
                    //put end tag, if needed.

                    if (e.Text.EndsWith(">", StringComparison.OrdinalIgnoreCase))
                    {
                        //First--are we within a quote?--move out if we are.
                        if (!XmlHelper.IsWithinQuotes(Content.Document.Text, Content.CaretOffset)
                            && XmlHelper.IsAllNode(Content.Document.Text, Content.CaretOffset))
                        {
                            //What about cut-paste?
                            e.Handled = InsertClosingTag();
                            if (e.Handled)
                            {


                            }
                            else
                            {
                                nestingLevel = GetNestingCount() - 1;
                               
                                int start = Content.Document.Text.LastIndexOf("<", Content.CaretOffset);
                                if (nestingLevel > -1)
                                {
                                    if (_log.IsInfoEnabled)
                                    {
                                        _log.InfoFormat("On text entered, '/' entered, caretposition={0}", Content.CaretOffset);
                                    }
                                    //Content.Document.Insert(Content.CaretOffset + 2, "".PadRight(nestingLevel, '\t'));
                              
                                    Content.Document.Insert(start, "\r\n");
                                    Content.Document.Insert(start + 2, "".PadRight(nestingLevel, '\t'));
                                }

                            }
                        }
                    }
                   
                }
                if (e != null && e.Text.Length > 0 && completionWindow == null)
                {
                    if (e.Text.EndsWith("="))
                    {

                        Content.Document.Insert(Content.CaretOffset, "=\"\"");
                        Content.CaretOffset--;
                        e.Handled = true;
                        CodeCompletedWithQuote();
                        CodeCompleted = false;
                    }
                }
                //Setting e.Handled to true prevents e.Text from being added.
                // Do not set e.Handled=true.
                // We still want to insert the character that was typed.
            }
        }
        protected virtual void CodeCompletedWithQuote()
        {

        }
        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            OnTextEntering(e);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "RussLibrary.Helpers.XmlHelper.IsCompletedNode(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "ICSharpCode.AvalonEdit.Document.TextDocument.Insert(System.Int32,System.String)")]
        protected bool InsertClosingTag()
        {
     
            bool retVal = false;
            string Node = XmlHelper.GetLastNode(Content.TextArea.Document.Text, Content.CaretOffset) + ">";
            // example return: <node

            if (!string.IsNullOrEmpty(Node) && !XmlHelper.IsCompletedNode(Node))
            {
                int i = Content.CaretOffset + 1;
                /////
                
                nestingLevel = GetNestingCount() - 1;

                int start = Content.Document.Text.LastIndexOf("<", Content.CaretOffset);
                int j=-1;
                if (start > 0)
                {
                    j = Content.Document.Text.LastIndexOf("<", start - 1);
                }
                /////
                string nodeName= XmlHelper.GetNodeName(Node);
                if (!string.IsNullOrEmpty(nodeName))
                {
                    string EndTag = string.Empty;
                    if (nestingLevel > 0 && j > -1)
                    {
                        EndTag = string.Format(CultureInfo.InvariantCulture, ">\r\n{0}</{1}>",
                            "".PadRight(nestingLevel, '\t'), nodeName);
                    }
                    else
                    {
                        EndTag = string.Format(CultureInfo.InvariantCulture, ">\r\n</{0}>", nodeName);
                    }
                    
                    Content.Document.Insert(Content.CaretOffset, EndTag);
                    Content.CaretOffset = i;
                    if (nestingLevel > 0 && j > -1)
                    {

                        Content.Document.Insert(start, "\r\n");
                        Content.Document.Insert(start + 2, "".PadRight(nestingLevel, '\t'));
                    }

                    retVal = true;
                }
            }
            return retVal;

        }
       
        
        int GetNestingCount()
        {
            return XmlHelper.GetNestingCount(this.Content.Document.Text, Content.CaretOffset);
        }
        #endregion

        public static string GetWordUnderMouse(TextDocument document, TextViewPosition position)
        {
            string wordHovered = string.Empty;

            int line = position.Line;
            int column = position.Column;
            if (document != null)
            {
                int offset = document.GetOffset(line, column);
                if (offset >= document.TextLength)
                    offset--;

                string textAtOffset = document.GetText(offset, 1);

                // Get text backward of the mouse position, until the first space
                while (!string.IsNullOrWhiteSpace(textAtOffset))
                {
                    wordHovered = textAtOffset + wordHovered;

                    offset--;

                    if (offset < 0)
                        break;

                    textAtOffset = document.GetText(offset, 1);
                }

                // Get text forward the mouse position, until the first space
                offset = document.GetOffset(line, column);
                if (offset < document.TextLength - 1)
                {
                    offset++;

                    textAtOffset = document.GetText(offset, 1);

                    while (!string.IsNullOrWhiteSpace(textAtOffset))
                    {
                        wordHovered = wordHovered + textAtOffset;

                        offset++;

                        if (offset >= document.TextLength)
                            break;

                        textAtOffset = document.GetText(offset, 1);
                    }
                }
            }
            return wordHovered;
        }
        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    if (fsw != null)
                    {
                        fsw.Dispose();
                    }
                    isDisposed = true;
                }
            }
        }
        bool isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }
    }
}

