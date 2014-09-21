using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using RussLibrary.WPF;
using RussLibrary.Xml;
using System.Windows;
using RussLibrary;
using System.IO;
namespace ArtemisModLoader.Xml
{
    [XmlConversionRoot("Mission")]
    public class MissionMeta : ChangeDependencyObject
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
            typeof(MissionMeta));
        [XmlConversion("Title")]
        public string Title
        {
            get
            {
                return (string)this.UIThreadGetValue(TitleProperty);

            }
            private set
            {
                this.UIThreadSetValue(TitleProperty, value);

            }
        }

        public static readonly DependencyProperty AuthorProperty =
           DependencyProperty.Register("Author", typeof(string),
           typeof(MissionMeta));
        [XmlConversion("Author")]
        public string Author
        {
            get
            {
                return (string)this.UIThreadGetValue(AuthorProperty);

            }
            private set
            {
                this.UIThreadSetValue(AuthorProperty, value);

            }
        }


        public static readonly DependencyProperty ModIdProperty =
           DependencyProperty.Register("ModId", typeof(string),
           typeof(MissionMeta));
        [XmlConversion("ModFileName")]
        public string ModId
        {
            get
            {
                return (string)this.UIThreadGetValue(ModIdProperty);

            }
            private set
            {
                this.UIThreadSetValue(ModIdProperty, value);

            }
        }


        public static readonly DependencyProperty InstalledWithModProperty =
           DependencyProperty.Register("InstalledWithMod", typeof(bool),
           typeof(MissionMeta));
        [XmlConversion("InstalledWithMod")]
        public bool InstalledWithMod
        {
            get
            {
                return (bool)this.UIThreadGetValue(InstalledWithModProperty);

            }
            private set
            {
                this.UIThreadSetValue(InstalledWithModProperty, value);

            }
        }


        public static readonly DependencyProperty InstallDateProperty =
           DependencyProperty.Register("InstallDate", typeof(DateTime),
           typeof(MissionMeta));
        [XmlConversion("InstallDate")]
        public DateTime InstallDate
        {
            get
            {
                return (DateTime)this.UIThreadGetValue(InstallDateProperty);

            }
            private set
            {
                this.UIThreadSetValue(InstallDateProperty, value);

            }
        }


        public static readonly DependencyProperty CreationDateProperty =
           DependencyProperty.Register("CreationDate", typeof(DateTime),
           typeof(MissionMeta));
        [XmlConversion("CreationDate")]
        public DateTime CreationDate
        {
            get
            {
                return (DateTime)this.UIThreadGetValue(CreationDateProperty);

            }
            private set
            {
                this.UIThreadSetValue(CreationDateProperty, value);

            }
        }





        public static readonly DependencyProperty DependsOnProperty =
           DependencyProperty.Register("DependsOn", typeof(ObservableCollection<StringItem>),
           typeof(MissionMeta));
        [XmlConversion("DependsOn")]
        public ObservableCollection<StringItem> DependsOn
        {
            get
            {
                return (ObservableCollection<StringItem>)this.UIThreadGetValue(CreationDateProperty);

            }
            private set
            {
                this.UIThreadSetValue(CreationDateProperty, value);

            }
        }

        public static readonly DependencyProperty MissionFileNameProperty =
           DependencyProperty.Register("MissionFilename", typeof(string),
           typeof(MissionMeta));
        [XmlConversion("MissionFilename")]
        public string MissionFileName
        {
            get
            {
                return (string)this.UIThreadGetValue(MissionFileNameProperty);

            }
            private set
            {
                this.UIThreadSetValue(MissionFileNameProperty, value);

            }
        }
        static void OnMissionPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MissionMeta me = sender as MissionMeta;
            if (me != null)
            {
                if (!string.IsNullOrEmpty(me.MissionPath))
                {
                    me.MissionFileName = new FileInfo(me.MissionPath).Name;
                }
            }
        }
        public static readonly DependencyProperty MissionPathProperty =
            DependencyProperty.Register("MissionPath", typeof(string),
            typeof(MissionMeta), new PropertyMetadata(OnMissionPathChanged));
        [XmlConversion("MissionPath")]
        public string MissionPath
        {
            get
            {
                return (string)this.UIThreadGetValue(MissionPathProperty);

            }
            private set
            {
                this.UIThreadSetValue(MissionPathProperty, value);

            }
        }

        protected override void ProcessValidation()
        {
            
        }
    }
}
