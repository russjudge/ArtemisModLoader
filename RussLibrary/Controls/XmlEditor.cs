using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RussLibrary.Helpers;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Folding;
using System.Windows.Threading;
using System.Globalization;

namespace RussLibrary.Controls
{
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
    public class XmlEditor : Control
    {
        public static RoutedCommand SaveCommand = new RoutedCommand();


        static XmlEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XmlEditor), new FrameworkPropertyMetadata(typeof(XmlEditor)));
            LoadCommands();
        }
        void DoSave(object sender, ExecutedRoutedEventArgs e)
        {
        }
        static void LoadCommands()
        {
            SaveCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
        }
        public XmlEditor()
        {
            Content = new TextEditor();
         
            



            
            this.CommandBindings.Add(new CommandBinding(SaveCommand, new ExecutedRoutedEventHandler(DoSave)));

            Content.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
            Content.Visibility = System.Windows.Visibility.Visible;
            Content.Document.TextChanged += new EventHandler(Document_TextChanged);
            
            Content.Options.ShowTabs = true;
            foldingManager = FoldingManager.Install(Content.TextArea);
            foldingStrategy = new XmlFoldingStrategy();
            foldingStrategy.UpdateFoldings(foldingManager, Content.Document);
          

            
        }

        void Document_TextChanged(object sender, EventArgs e)
        {
            if (Content != null && Content.Document != null)
            {
                Text = Content.Document.Text;
            }
        }
        static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            XmlEditor me = sender as XmlEditor;
            if (me != null && me.Content != null && me.Content.Document != null)
            {
                me.Content.Document.Text = me.Text;
            }
        }
        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string),
        typeof(XmlEditor));

        public string Text
        {
            get
            {
                return (string)this.UIThreadGetValue(TextProperty);
            }
            private set
            {
                this.UIThreadSetValue(TextProperty, value);
            }
        }
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

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Subscribe();

            foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(5);
            foldingUpdateTimer.Tick += new EventHandler(foldingUpdateTimer_Tick);
            foldingUpdateTimer.Start();

        }
        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                foldingStrategy.UpdateFoldings(foldingManager, Content.Document);
            }
        }
        private void Subscribe()
        {

            // in the constructor:
            Content.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            Content.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            Content.TextArea.KeyDown += new KeyEventHandler(TextArea_KeyDown);
            Content.TextArea.KeyUp += new KeyEventHandler(TextArea_KeyUp);
            //Content.TextArea.
        }

        void TextArea_KeyUp(object sender, KeyEventArgs e)
        {
           
        }

        void TextArea_KeyDown(object sender, KeyEventArgs e)
        {
           
        }
        #region Fields
    
        CompletionWindow completionWindow;
        FoldingManager foldingManager = null;
        XmlFoldingStrategy foldingStrategy = null;
        DispatcherTimer foldingUpdateTimer = null;

        #endregion

        //TODO: correct mechanism for determining nesting level.  Must be based on current offset, so
        // calculation must start from beginning of doc and work forward.
        int nestingLevel = -1;
        protected virtual void OnTextEntered(TextCompositionEventArgs e)
        {
            if (e != null)
            {
                if (e.Text == "<")
                {
                    // Open code completion after the user has pressed dot:
                    completionWindow = new CompletionWindow(Content.TextArea);
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                    //data.Add(new MyCompletionData("Item1"));
                    //data.Add(new MyCompletionData("Item2"));
                    //data.Add(new MyCompletionData("Item3"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate
                    {
                        completionWindow = null;
                    };
                }
                else if (e.Text == ">")
                {

                }
                else if (e.Text == "\"")
                {
                }
                if (e.Text.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    //Add code completion for last node--text entered must be "</".  overrides above.
                }
            }
        }
        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            OnTextEntered(e);
        }
        protected virtual void OnTextEntering(TextCompositionEventArgs e)
        {
            if (e != null)
            {
                if (e.Text.Length > 0 && completionWindow != null)
                {
                    if (!char.IsLetterOrDigit(e.Text[0]))
                    {
                        // Whenever a non-letter is typed while the completion window is open,
                        // insert the currently selected element.
                        completionWindow.CompletionList.RequestInsertion(e);
                    }

                }
                if (e.Text.Length > 0)
                {
                    //put end tag, if needed.

                    if (e.Text.EndsWith(">", StringComparison.OrdinalIgnoreCase))
                    {
                        //What about cut-paste?
                        e.Handled = InsertClosingTag();
                        if (e.Handled)
                        {
                            nestingLevel++;
                            int start = XmlHelper.GetOffset(Content.Text, Content.CaretOffset, "<");
                            if (nestingLevel > 0)
                            {
                                Content.Document.Insert(Content.CaretOffset+2, "".PadRight(nestingLevel, '\t'));
                                Content.Document.Insert(start, "\r\n".PadRight(nestingLevel+2, '\t'));
                            }

                        }
                    }

                }
                //Setting e.Handled to true prevents e.Text from being added.
                // Do not set e.Handled=true.
                // We still want to insert the character that was typed.
            }
        }
        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            OnTextEntering(e);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "RussLibrary.Helpers.XmlHelper.IsCompletedNode(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "ICSharpCode.AvalonEdit.Document.TextDocument.Insert(System.Int32,System.String)")]
        protected bool InsertClosingTag()
        {
            //TODO: add tabs.  Move whole node to new line if needed.
            bool retVal = false;
            string Node = XmlHelper.GetLastNode(Content.TextArea.Document.Text, Content.CaretOffset) + ">";
            // example return: <node

            if (!string.IsNullOrEmpty(Node) && !XmlHelper.IsCompletedNode(Node))
            {
                int i = Content.CaretOffset + 1;
                string EndTag = string.Format(CultureInfo.InvariantCulture, ">\r\n</{0}>", XmlHelper.GetNodeName(Node));
                Content.Document.Insert(Content.CaretOffset, EndTag);
                Content.CaretOffset = i;

                retVal = true;
            }
            return retVal;

        }
       
    }
}

