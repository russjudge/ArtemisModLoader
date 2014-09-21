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
using System.Windows.Shapes;
using RussLibrary;

namespace RussLibrary.Controls
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        static UIElement ButtonContent = null;
        public static void AddButtonContent(UIElement control)
        {
            ButtonContent = control;
        }
        /// <summary>
        /// Shows the specified editor control. Must inherit XmlEditor.
        /// </summary>
        /// <param name="editorControl">The editor control.</param>
        public static void Show(XmlEditor editorControl)
        {
            Show(null, null, editorControl, null, null);
        }
        /// <summary>
        /// Shows the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static void Show(string title)
        {
            XmlEditor xe = null;
            try
            {
                xe = new XmlEditor();

                Show(title, null, xe, null, null);
                xe = null;
            }
            finally
            {
                if (xe != null)
                {
                    xe.Dispose();
                }
            }
        }
        /// <summary>
        /// Shows the specified title. Must inherit XmlEditor.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="editorControl">The editor control.</param>
        public static void Show(string title, XmlEditor editorControl)
        {
            Show(title, null, editorControl, null, null);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static void Show(string title, string file)
        {
            XmlEditor xe = null;
            try
            {
                xe = new XmlEditor();

                Show(title, file, xe, null, null);
                xe = null;
            }
            finally
            {
                if (xe != null)
                {
                    xe.Dispose();
                }
            }
        }
        public static void Show(string title, string file, XmlEditor editorControl, Control sideElement, ImageSource Icon)
        {

            EditorWindow win = new EditorWindow(editorControl);
            if (Icon != null)
            {
                win.Icon = Icon;
            }
            if (!string.IsNullOrEmpty(title))
            {
                win.Title = title;
            }
            win.SideElement = sideElement;
            if (!string.IsNullOrEmpty(file) && System.IO.File.Exists(file))
            {
                win.Load(file);
            }

            win.AdditionalButtons = ButtonContent;
            ButtonContent = null;
            win.Show();
        }
        private EditorWindow(XmlEditor editorControl)
        {
            if (editorControl == null)
            {
                Editor = new XmlEditor();
            }
            else
            {
                Editor = editorControl;
            }
            LoadCommandBindings();
            InitializeComponent();
        }
        private void Load(string file)
        {
            Editor.Load(file);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetMaxSize();

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
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, new ExecutedRoutedEventHandler(DoClose)));

            //cmd = new CommandBinding(NavigationCommands.GoToPage, new ExecutedRoutedEventHandler(DoLocate));


            cmd = new CommandBinding(ApplicationCommands.SelectAll, new ExecutedRoutedEventHandler(DoSelectAll));
            cmd.CanExecute += new CanExecuteRoutedEventHandler(cmd_CanExecute);
            this.CommandBindings.Add(cmd);

        }
        void DoSelectAll(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Content.SelectAll();
        }
        void cmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        void DoClose(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
        void DoNew(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.DoNew();
        }
        void DoSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.DoSavePromptForFilename();
        }
        void DoReplace(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.DoReplace();
        }
        void DoFind(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.DoFind();
        }

        void DoOpen(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Open();
        }
        void DoSave(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Save();
        }

        void DoFormat(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Format();
        }


        public static readonly DependencyProperty AdditionalButtonsProperty =
        DependencyProperty.Register("AdditionalButtons", typeof(UIElement),
        typeof(EditorWindow));

        public UIElement AdditionalButtons
        {
            get
            {
                return (UIElement)this.UIThreadGetValue(AdditionalButtonsProperty);
            }
            private set
            {
                this.UIThreadSetValue(AdditionalButtonsProperty, value);
            }
        }

        public static readonly DependencyProperty EditorProperty =
        DependencyProperty.Register("Editor", typeof(XmlEditor),
        typeof(EditorWindow));

        public XmlEditor Editor
        {
            get
            {
                return (XmlEditor)this.UIThreadGetValue(EditorProperty);
            }
            private set
            {
                this.UIThreadSetValue(EditorProperty, value);
            }
        }



        public static readonly DependencyProperty SideElementProperty =
        DependencyProperty.Register("SideElement", typeof(Control),
        typeof(EditorWindow));

        public Control SideElement
        {
            get
            {
                return (Control)this.UIThreadGetValue(SideElementProperty);
            }
            private set
            {
                this.UIThreadSetValue(SideElementProperty, value);
            }
        }


        private void uc_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.XMLEditorWindowHeight = this.Height;
            Properties.Settings.Default.XMLEditorWindowWidth = this.Width;
            Properties.Settings.Default.Save();
        }

        private void uc_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Editor.Content.IsModified)
            {
                switch (MessageBox.Show("Do you wish to save your changes?",
                    this.Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        Editor.Save();
                        break;

                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }
    }
}
