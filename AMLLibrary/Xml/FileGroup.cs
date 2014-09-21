using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.WPF;
using RussLibrary;
using RussLibrary.Xml;
using System.Windows;
using System.Collections.ObjectModel;

namespace ArtemisModLoader.Xml
{
    
    public class FileGroup : ChangeDependencyObject
    {
        public FileGroup()
        {
            Files = new FileMapCollection();
            Files.ObjectChanged += new EventHandler(Files_ObjectChanged);
        }

        void Files_ObjectChanged(object sender, EventArgs e)
        {
            this.SetChanged();
        }
        public override void AcceptChanges()
        {
            if (Files != null)
            {
                Files.AcceptChanges();
            }
            base.AcceptChanges();
        }
        public override void RejectChanges()
        {
            if (Files != null)
            {
                Files.RejectChanges();
            }
            base.RejectChanges();
        }
        public override void BeginInitialization()
        {
            
            base.BeginInitialization();
            if (Files != null)
            {
               Files.BeginInitialization();
                
            }
        }
        public override void EndInitialization()
        {
            if (Files != null)
            {
                Files.EndInitialization();
            }
            base.EndInitialization();
        }
        public static readonly DependencyProperty FilesProperty =
            DependencyProperty.Register("Files", typeof(FileMapCollection),
            typeof(FileGroup));
        [XmlConversion("FileMap")]
        public FileMapCollection Files
        {
            get
            {
                return (FileMapCollection)this.UIThreadGetValue(FilesProperty);

            }
            private set
            {
                this.UIThreadSetValue(FilesProperty, value);

            }
        }



        protected override void ProcessValidation()
        {
            
        }
    }
}
