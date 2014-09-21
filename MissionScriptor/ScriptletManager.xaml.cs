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
using MissionStudio.Helpers;
using System.IO;
using System.Collections.ObjectModel;
using RussLibrary;
using RussLibrary.Helpers;
using Microsoft.Win32;
namespace MissionStudio
{
    /// <summary>
    /// Interaction logic for ScriptletManager.xaml
    /// </summary>
    public partial class ScriptletManager : UserControl
    {
        public ScriptControl Editor { get; set; }
        //void LoadCommandBindings()
        //{

        //    CommandBinding cmd = new CommandBinding(EditingCommands.ToggleInsert, new ExecutedRoutedEventHandler(DoInsert));
        //    cmd.CanExecute += new CanExecuteRoutedEventHandler(cmd_CanExecute);

        //    this.CommandBindings.Add(cmd);


        //    cmd = new CommandBinding(EditingCommands.CorrectSpellingError, new ExecutedRoutedEventHandler(DoEdit));
        //    cmd.CanExecute += new CanExecuteRoutedEventHandler(cmd_CanExecute);

        //    this.CommandBindings.Add(cmd);
      


        //    cmd = new CommandBinding(EditingCommands.Delete, new ExecutedRoutedEventHandler(DoDelete));
        //    cmd.CanExecute += new CanExecuteRoutedEventHandler(cmd_CanExecute);

        //    this.CommandBindings.Add(cmd);





        //    cmd = new CommandBinding(EditingCommands.ToggleUnderline, new ExecutedRoutedEventHandler(DoRename));
        //    cmd.CanExecute += new CanExecuteRoutedEventHandler(cmd_CanExecute);

        //    this.CommandBindings.Add(cmd);
        //}
      

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            if (mnu != null)
            {
                ScriptletItem item = mnu.CommandParameter as ScriptletItem;
                Editor.InsertFile(item.Filename);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            if (mnu != null)
            {
                ScriptletItem item = mnu.CommandParameter as ScriptletItem;
                if (item != null)
                {
                    ScriptControl.Show(item.Filename);
                }
            }
        }
        
        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            if (mnu != null)
            {
                ScriptletItem item = mnu.CommandParameter as ScriptletItem;
                item.EnableEdit = true;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            if (mnu != null)
            {
                ScriptletItem item = mnu.CommandParameter as ScriptletItem;
                File.Delete(item.Filename);
                Scripts.Remove(item);
            }
        }

        public ScriptletManager()
        {
            InitializeComponent();
            this.Dispatcher.BeginInvoke(new Action(LoadScriptletList), System.Windows.Threading.DispatcherPriority.Loaded);
              
        }
        void LoadScriptletList()
        {
            Scripts = new ObservableCollection<ScriptletItem>();
            foreach (FileInfo f in new DirectoryInfo(Locations.MissionPath).GetFiles())
            {
                
                Scripts.Add(new ScriptletItem(f));
            }
        }
        static void OnSelectedScriptChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ScriptletItem item = e.OldValue as ScriptletItem;
            if (item != null)
            {
                item.FirstClick = false;
                item.EnableEdit = false;
            }
        }

        public static readonly DependencyProperty SelectedScriptProperty =
        DependencyProperty.Register("SelectedScript", typeof(ScriptletItem),
        typeof(ScriptletManager), new PropertyMetadata(OnSelectedScriptChanged));

        public ScriptletItem SelectedScript
        {
            get
            {
                return (ScriptletItem)this.UIThreadGetValue(SelectedScriptProperty);
            }
            private set
            {
                this.UIThreadSetValue(SelectedScriptProperty, value);
            }
        }
     

        public static readonly DependencyProperty ScriptsProperty =
        DependencyProperty.Register("Scripts", typeof(ObservableCollection<ScriptletItem>),
        typeof(ScriptletManager));

        public ObservableCollection<ScriptletItem> Scripts
        {
            get
            {
                return (ObservableCollection<ScriptletItem>)this.UIThreadGetValue(ScriptsProperty);
            }
            private set
            {
                this.UIThreadSetValue(ScriptsProperty, value);
            }
        }
     
        private void Scriptlet_MouseDown(object sender, MouseButtonEventArgs e)
        {
            

                startPoint = e.GetPosition(null);
            
        
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            //SetDragging(sender as UIElement);

            
        }
        Point startPoint = new Point();
        bool IsDragging = false;
        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;
            
            TextBlock t = sender as TextBlock;
            if (t != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed &&
                    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    List<string> files = new List<string>();
                    ScriptletItem f = t.Tag as ScriptletItem;
                    if (f != null)
                    {
                        string data = null;
                        using (StreamReader sr = new StreamReader(f.Filename))
                        {
                            data = sr.ReadToEnd();
                            if (data.Contains("<event"))
                            {
                                data = null;
                            }
                            
                        }
                        f.EnableEdit = false;
                        IsDragging = true;
                        if (!string.IsNullOrEmpty(data))
                        {
                            DragDrop.DoDragDrop(t, new DataObject(DataFormats.StringFormat, data), DragDropEffects.Copy);

                        }
                        else
                        {
                            files.Add(f.Filename);
                            DragDrop.DoDragDrop(t, new DataObject(DataFormats.FileDrop, files.ToArray()), DragDropEffects.Copy);
                            
                        }
                        IsDragging = false;
                    }

                }
            }
               
            
        }

       

        private void Scriptlet_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement elem = sender as FrameworkElement;

            if (elem.Tag == SelectedScript)
            {
                if (SelectedScript.FirstClick)
                {
                    SelectedScript.EnableEdit = true;
                }
                SelectedScript.FirstClick = true;

            }
            else
            {
                SelectedScript.EnableEdit = false;
                SelectedScript.FirstClick = false;
            }
            
        }

        private void Scriptlet_Drop(object sender, DragEventArgs e)
        {
           
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {

                string data = e.Data.GetData(DataFormats.StringFormat) as string;
                if (!string.IsNullOrEmpty(data))
                {
                    int i = 1;
                    string f = System.IO.Path.Combine(Locations.MissionPath, "NewScriptlet" + i.ToString() + ".xml");
                    while (File.Exists(f))
                    {
                        f = System.IO.Path.Combine(Locations.MissionPath, "NewScriptlet" + (++i).ToString() + ".xml");
                    }

                    using (StreamWriter sw = new StreamWriter(f))
                    {
                        sw.Write(data);
                    }
                    Scripts.Add(new ScriptletItem(new FileInfo(f)));
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files != null)
                {
                    
                    foreach (string f in files)
                    {
                        FileInfo fle = new FileInfo(f);
                        if (fle.DirectoryName != Locations.MissionPath)
                        {
                            string target = System.IO.Path.Combine(Locations.MissionPath, fle.Name);
                           
                            FileHelper.Copy(f, target);
                            Scripts.Add(new ScriptletItem(new FileInfo(target)));
                        }
                    }
                }

            }
        }

        private void Scriptlet_DragEnter(object sender, DragEventArgs e)
        {
            if (IsDragging)
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                if (e.Data.GetDataPresent(DataFormats.StringFormat)
                    || e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effects = DragDropEffects.All;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TextBox t = sender as TextBox;
                if (t != null)
                {
                    ScriptletItem item = t.Tag as ScriptletItem;
                    if (item != null)
                    {
                        item.EnableEdit = false;
                    }

                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t != null)
            {
                ScriptletItem item = t.Tag as ScriptletItem;
                if (item != null)
                {
                    item.EnableEdit = false;
                }

            }

        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            if (mnu != null)
            {
                ScriptletItem si = mnu.CommandParameter as ScriptletItem;
                if (si != null)
                {
                    SaveFileDialog diag = new SaveFileDialog();
                    diag.Title = "Select filename to save scriptlet to.";
                    diag.Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*";

                    diag.DefaultExt = ".xml";
                    if (diag.ShowDialog() == true)
                    {
                        FileHelper.Copy(si.Filename, diag.FileName);
                    }

                }
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = "Select file to import";
            diag.Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*";
            diag.DefaultExt = ".xml";
            diag.AddExtension = true;
            if (diag.ShowDialog() == true)
            {
                foreach (string f in diag.FileNames)
                {
                    FileInfo fle = new FileInfo(f);
                    string target = System.IO.Path.Combine(Locations.MissionPath, fle.Name);
                    FileHelper.Copy(fle.FullName, target);
                    ScriptletItem itm = new ScriptletItem(fle);
                    Scripts.Add(itm);
                }
            }

        }

        //void SetDragging(UIElement elem)
        //{
        //    if (elem != null)
        //    {
        //        elem.InitializeDragging(this);
        //        elem.SetDragTypes(typeof(TextBlock), typeof(Border)); //
        //        elem.SetInvalidDragTypes(typeof(ListBox), typeof(System.Windows.Controls.Primitives.ScrollBar));

        //    }
        //}
        

    }
}
