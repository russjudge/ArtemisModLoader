using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using RussLibrary.Helpers;
using RussLibrary.WPF;
using System.Collections.ObjectModel;

namespace RussLibrary.Controls
{
    /// <summary>
    /// Interaction logic for FileSelectionControl.xaml
    /// </summary>
    public partial class FileSelectionControl : UserControl
    {
        public FileSelectionControl()
        {
            Validation = new ValidationObject();
            InitializeComponent();
        }

        public static readonly DependencyProperty CheckFileExistsProperty =
            DependencyProperty.Register("CheckFileExists", typeof(bool),
            typeof(FileSelectionControl), new PropertyMetadata(true));

        public bool CheckFileExists
        {
            get
            {
                return (bool)this.UIThreadGetValue(CheckFileExistsProperty);

            }
            set
            {
                this.UIThreadSetValue(CheckFileExistsProperty, value);

            }
        }







        public static readonly DependencyProperty FullNameProperty =
            DependencyProperty.Register("FullName", typeof(string),
            typeof(FileSelectionControl), new PropertyMetadata(OnFullnameChange));

        public string FullName
        {
            get
            {
                return (string)this.UIThreadGetValue(FullNameProperty);

            }
            set
            {
                this.UIThreadSetValue(FullNameProperty, value);

            }
        }
        static void OnFullnameChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FileSelectionControl me = sender as FileSelectionControl;
            if (me != null)
            {
                if (!me.IsUpdating && !string.IsNullOrEmpty(me.Prefix) && !string.IsNullOrEmpty(me.FullName))
                {
                    me.IsUpdating = true;
                   
                    string wrkFile = me.FullName;
                    if (!me.FullName.StartsWith(me.Prefix, StringComparison.OrdinalIgnoreCase))
                    {


                        //First, is it in the Stock folder. if not, is it in the Active artemis copy?
                        //  if it is in active artemis copy, then tag "DependsOn".  All this must be done elsewhere.
                        RoutedEventArgs e1 = new RoutedEventArgs(InvalidFilePathEvent, wrkFile);
                        me.RaiseEvent(e1);
                        if (!e1.Handled) //if handled, then user canceled operation, so restoring to old value required.
                        {
                            //if e.Handled == true, then all is done, do nothing more.
                            // if not handled, then e.Source should have the new path with the Prefix match.
                            wrkFile = e1.Source as string;
                            if (!wrkFile.StartsWith(me.Prefix, StringComparison.OrdinalIgnoreCase)) // still have problem, but may be okay.
                            {
                                me.FullName = e.OldValue as string;
                            }
                        }
                        else
                        {
                            me.FullName = e.OldValue as string;

                        }
                    }
                   




                    //possibility that FullName not within Prefix.  What should FileDisplay be in that case?
                    if (me.FullName.StartsWith(me.Prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        string str = me.FullName.Substring(me.Prefix.Length+ 1);
                        me.FileDisplay = str.Replace('\\','/');
                    }
                    else
                    {
                        //Need a way to pass alternate Prefix in event of using dependent mod.
                        if (!string.IsNullOrEmpty(me.AlternatePrefix))
                        {
                            if (me.FullName.StartsWith(me.AlternatePrefix, StringComparison.OrdinalIgnoreCase))
                            {
                                me.FileDisplay = me.FullName.Substring(me.AlternatePrefix.Length + 1).Replace('\\', '/');
                            }
                            else
                            {
                                me.FileDisplay = me.FullName.Replace('\\', '/');
                            }
                        }
                        else
                        {
                            me.FileDisplay = me.FullName.Replace('\\', '/');
                        }
                    }
                    me.ProcessValidation();
                    me.IsUpdating = false;
                }
            }
        }
        static void OnPrefixChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FileSelectionControl me = sender as FileSelectionControl;
            if (me != null)
            {
                if (!me.IsUpdating)
                {
                    me.IsUpdating = true;
                    if (!string.IsNullOrEmpty(me.Prefix) && string.IsNullOrEmpty(me.FullName) && !string.IsNullOrEmpty(me.FileDisplay))
                    {

                        me.FullName = Path.Combine(me.Prefix, me.FileDisplay);
                    }
                    me.ProcessValidation();
                    me.IsUpdating = false;
                }
            }
        }
        public static readonly DependencyProperty PrefixProperty =
            DependencyProperty.Register("Prefix", typeof(string),
            typeof(FileSelectionControl), new PropertyMetadata(OnPrefixChange));

        public string Prefix
        {
            get
            {
                return (string)this.UIThreadGetValue(PrefixProperty);

            }
            set
            {
                this.UIThreadSetValue(PrefixProperty, value);

            }
        }

        public static readonly DependencyProperty SearchPrefixesProperty =
         DependencyProperty.Register("SearchPrefixes", typeof(ObservableCollection<string>),
         typeof(FileSelectionControl));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ObservableCollection<string> SearchPrefixes
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(SearchPrefixesProperty);

            }
            set
            {
                this.UIThreadSetValue(SearchPrefixesProperty, value);

            }
        }



        public static readonly DependencyProperty AlternatePrefixProperty =
           DependencyProperty.Register("AlternatePrefix", typeof(string),
           typeof(FileSelectionControl));

        public string AlternatePrefix
        {
            get
            {
                return (string)this.UIThreadGetValue(AlternatePrefixProperty);

            }
            set
            {
                this.UIThreadSetValue(AlternatePrefixProperty, value);

            }
        }
        bool IsUpdating = false;
        static void OnFileDisplayChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FileSelectionControl me = sender as FileSelectionControl;
            if (me != null)
            {
                if (!me.IsUpdating && !string.IsNullOrEmpty(me.Prefix) && !string.IsNullOrEmpty(me.FileDisplay))
                {
                    string wrkFileDisplay = me.FileDisplay.Replace('/', '\\');

                    if (wrkFileDisplay.EndsWith("*", StringComparison.OrdinalIgnoreCase))
                    {
                        me.FullName = null;
                    }
                    else if (wrkFileDisplay.Contains(":"))
                    {
                        me.FullName = wrkFileDisplay;
                    }
                    else if (wrkFileDisplay.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
                    {
                        me.FullName = Path.Combine(me.Prefix, wrkFileDisplay.Substring(1));
                    }
                    else
                    {
                        me.IsUpdating = true;

                        me.FullName = Path.Combine(me.Prefix, wrkFileDisplay);
                        
                        me.IsUpdating = false;
                    }

                    me.ProcessValidation();
                   
                }
                me.RaiseEvent(new RoutedEventArgs(FileChangedEvent, me.FileDisplay));
            }
        }
        public static readonly DependencyProperty FileDisplayProperty =
            DependencyProperty.Register("FileDisplay", typeof(string),
            typeof(FileSelectionControl), new PropertyMetadata(OnFileDisplayChange));

        public string FileDisplay
        {
            get
            {
                return (string)this.UIThreadGetValue(FileDisplayProperty);

            }
            set
            {
                this.UIThreadSetValue(FileDisplayProperty, value);

            }
        }



        public static readonly DependencyProperty FilterProperty =
          DependencyProperty.Register("Filter", typeof(string),
          typeof(FileSelectionControl));

        public string Filter
        {
            get
            {
                return (string)this.UIThreadGetValue(FilterProperty);

            }
            set
            {
                this.UIThreadSetValue(FilterProperty, value);

            }
        }



        public static readonly DependencyProperty ValidExtensionsProperty =
          DependencyProperty.Register("ValidExtensions", typeof(string),
          typeof(FileSelectionControl));

        /// <summary>
        /// Gets or sets the valid extensions.  Must be delimited with ";"
        /// </summary>
        /// <value>
        /// The valid extensions.
        /// </value>
        public string ValidExtensions
        {
            get
            {
                return (string)this.UIThreadGetValue(ValidExtensionsProperty);

            }
            set
            {
                this.UIThreadSetValue(ValidExtensionsProperty, value);

            }
        }



        public static readonly DependencyProperty MustBeImageProperty =
          DependencyProperty.Register("MustBeImage", typeof(bool),
          typeof(FileSelectionControl));

        /// <summary>
        /// Gets or sets the valid extensions.  Must be delimited with ";"
        /// </summary>
        /// <value>
        /// The valid extensions.
        /// </value>
        public bool MustBeImage
        {
            get
            {
                return (bool)this.UIThreadGetValue(MustBeImageProperty);

            }
            set
            {
                this.UIThreadSetValue(MustBeImageProperty, value);

            }
        }


        void ProcessValidation()
        {
            bool skip = false;
            this.Validation = new ValidationObject();
            if (MustBeImage && !string.IsNullOrEmpty(FullName))
            {
                if (!FileHelper.IsImageFile(FullName))
                {
                    
                    Validation = new ValidationObject(string.Empty, ValidationValue.IsWarnState, "Must be an image file.");
                    skip = true;
                }
            }
            if (!skip && !string.IsNullOrEmpty(FullName))
            {
                if (!string.IsNullOrEmpty(ValidExtensions)
                    && (!ValidExtensions.ToUpperInvariant().Contains(new FileInfo(FullName).Extension.ToUpperInvariant() + ";")
                    && ValidExtensions.ToUpperInvariant() != new FileInfo(FullName).Extension.ToUpperInvariant()))
                {
                    Validation = new ValidationObject(string.Empty, ValidationValue.IsWarnState, "Not an expected extension.");
                  
                    skip = true;
                }
            }
            if (!skip && CheckFileExists && !string.IsNullOrEmpty(FullName))
            {

                bool IsInvalid = false;
                if (!File.Exists(FullName))
                {
                    IsInvalid = true;
                    if (!string.IsNullOrEmpty(AlternatePrefix))
                    {
                        IsInvalid = !File.Exists(Path.Combine(AlternatePrefix, new FileInfo(FullName).Name));
                    }
                    if (IsInvalid)
                    {
                        if (SearchPrefixes != null)
                        {
                            foreach (string src in SearchPrefixes)
                            {

                                int adjust = -1;
                                if (!string.IsNullOrEmpty(Prefix))
                                {
                                    adjust = Prefix.Length;
                                }
                                string fle = Path.Combine(src, FullName.Substring(adjust + 1));
                                IsInvalid = !File.Exists(fle);
                                if (!IsInvalid)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                if (IsInvalid)
                {
                    Validation = new ValidationObject(string.Empty, ValidationValue.IsError, "File does not exist");
                    skip = true;
                }
            }
        }

        public static readonly DependencyProperty ValidationProperty =
          DependencyProperty.Register("Validation", typeof(ValidationObject),
          typeof(FileSelectionControl));

        public ValidationObject Validation
        {
            get
            {
                return (ValidationObject)this.UIThreadGetValue(ValidationProperty);

            }
            set
            {
                this.UIThreadSetValue(ValidationProperty, value);

            }
        }





        public static readonly DependencyProperty DialogTitleProperty =
          DependencyProperty.Register("DialogTitle", typeof(string),
          typeof(FileSelectionControl));

        public string DialogTitle
        {
            get
            {
                return (string)this.UIThreadGetValue(DialogTitleProperty);

            }
            set
            {
                this.UIThreadSetValue(DialogTitleProperty, value);

            }
        }
        public static readonly RoutedEvent FileChangedEvent =
         EventManager.RegisterRoutedEvent(
         "FileChanged", RoutingStrategy.Direct,
         typeof(RoutedEventHandler),
         typeof(FileSelectionControl));

        public event RoutedEventHandler FileChanged
        {
            add { AddHandler(FileChangedEvent, value); }
            remove { RemoveHandler(FileChangedEvent, value); }
        }



        public static readonly RoutedEvent InvalidFilePathEvent =
          EventManager.RegisterRoutedEvent(
          "InvalidFilePath", RoutingStrategy.Direct,
          typeof(RoutedEventHandler),
          typeof(FileSelectionControl));

        public event RoutedEventHandler InvalidFilePath
        {
            add { AddHandler(InvalidFilePathEvent, value); }
            remove { RemoveHandler(InvalidFilePathEvent, value); }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.InitialDirectory = Prefix;
            diag.Multiselect = false;
            diag.Filter = Filter;
            diag.CheckFileExists = false;
            diag.Title = DialogTitle;
 
            if (diag.ShowDialog() == true)
            {
                this.FullName = diag.FileName;
                
            }
     
        }
    }
}
