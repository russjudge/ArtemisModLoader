using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RussLibrary;
using System.IO;
namespace MissionStudio
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Scriptlet")]
    public class ScriptletItem : DependencyObject
    {
        public ScriptletItem(string file)
        {
            Filename = file;
        }
        public ScriptletItem(FileInfo file)
        {
            if (file != null)
            {
                Filename = file.FullName;
            }
        }
        bool IsUpdating = false;
        static void OnFilenameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

            ScriptletItem me = sender as ScriptletItem;
            if (me != null)
            {
                if (!me.IsUpdating)
                {
                    me.IsUpdating = true;
                    FileInfo f = new FileInfo(me.Filename);
                    if (f.Extension.ToUpperInvariant() == ".XML")
                    {
                        me.DisplayItem = f.Name.Substring(0, f.Name.Length - f.Extension.Length);
                    }
                    else
                    {
                        me.DisplayItem = f.Name;
                    }
                    me.IsUpdating = false;
                }
            }
        }



        public static readonly DependencyProperty EnableEditProperty =
        DependencyProperty.Register("EnableEdit", typeof(bool),
        typeof(ScriptletItem));

        public bool EnableEdit
        {
            get
            {
                return (bool)this.UIThreadGetValue(EnableEditProperty);
            }
            set
            {
                this.UIThreadSetValue(EnableEditProperty, value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Filename")]
        public static readonly DependencyProperty FilenameProperty =
        DependencyProperty.Register("Filename", typeof(string),
        typeof(ScriptletItem), new PropertyMetadata(OnFilenameChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Filename")]
        public string Filename
        {
            get
            {
                return (string)this.UIThreadGetValue(FilenameProperty);
            }
            private set
            {
                this.UIThreadSetValue(FilenameProperty, value);
            }
        }
        public bool FirstClick { get; set; }
        static void OnDisplayItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ScriptletItem me = sender as ScriptletItem;
            string wrk = me.DisplayItem;
            if (!me.DisplayItem.Contains('.'))
            {
                wrk = wrk + ".xml";
            }

            FileInfo source = new FileInfo(me.Filename);
            FileInfo target = new FileInfo(Path.Combine(source.DirectoryName, wrk));
            if (e.OldValue != null)
            {
                source.CopyTo(target.FullName);
            }
            if (!me.IsUpdating)
            {
                me.IsUpdating = true;
                me.Filename = target.FullName;
                me.IsUpdating = false;
            }
            me.EnableEdit = false;
        }
        public static readonly DependencyProperty DisplayItemProperty =
        DependencyProperty.Register("DisplayItem", typeof(string),
        typeof(ScriptletItem), new PropertyMetadata(OnDisplayItemChanged));

        public string DisplayItem
        {
            get
            {
                return (string)this.UIThreadGetValue(DisplayItemProperty);
            }
            private set
            {
                this.UIThreadSetValue(DisplayItemProperty, value);
            }
        }

    }
}
