using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.WPF;
using RussLibrary.Text;
using System.Windows;
using RussLibrary;
using System.Collections.ObjectModel;
using System.IO;
namespace ArtemisModLoader.Text
{
    public class AudioConfiguration : ChangeDependencyObject
    {
        //list of files
        //Silent on start up ?
        //
        public void ResetAudioServer()
        {
            RussLibraryAudio.AudioServer.Current.Clear();
            foreach (string file in AudioCollection)
            {
                RussLibraryAudio.AudioServer.Current.Enqueue(file);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void LoadDefault()
        {
          
            DirectoryInfo artDir = new DirectoryInfo(System.IO.Path.Combine(Locations.ArtemisCopyPath, "dat"));
            try
            {
                foreach (FileInfo f in artDir.GetFiles("*.ogg"))
                {
                    if (!f.Name.StartsWith("silence", StringComparison.OrdinalIgnoreCase))
                    {
                        AudioCollection.Add(f.FullName);
                        SetAudioList();
                    }
                }
            }
            catch 
            {

            }
        }

        protected override void ProcessValidation()
        {
          
        }

        public static readonly DependencyProperty StartupMusicProperty =
          DependencyProperty.Register("StartupMusic", typeof(bool),
          typeof(AudioConfiguration), new PropertyMetadata(true, OnItemChanged));
        [INIConversion("StartupMusic")]
        public bool StartupMusic
        {
            get
            {
                return (bool)this.UIThreadGetValue(StartupMusicProperty);
            }
            set
            {
                this.UIThreadSetValue(StartupMusicProperty, value);
            }
        }



        bool IsUpdating = false;
        static void OnAudioListChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AudioConfiguration me = sender as AudioConfiguration;
            if (me != null)
            {
                if (!me.IsUpdating)
                {
                    me.SetAudioCollection();
                }
                OnItemChanged(sender, e);
            }
        }
        public static readonly DependencyProperty AudioListProperty =
            DependencyProperty.Register("AudioList", typeof(string),
            typeof(AudioConfiguration), new PropertyMetadata(OnAudioListChanged));
        [INIConversion("AudioList")]
        public string AudioList
        {
            get
            {
                return (string)this.UIThreadGetValue(AudioListProperty);
            }
            set
            {
                this.UIThreadSetValue(AudioListProperty, value);
            }
        }

        public void SetAudioList()
        {
            IsUpdating = true;
            AudioList = string.Join(";", AudioCollection.ToArray<string>());
            IsUpdating = false;
        }
        public void SetAudioCollection()
        {
            IsUpdating = true;
            if (!string.IsNullOrEmpty(AudioList))
            {

                AudioCollection = new ObservableCollection<string>(AudioList.Split(';'));
            }
            else
            {
                AudioCollection = new ObservableCollection<string>();
            }
            IsUpdating = false;
        }
        static void OnAudioCollectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AudioConfiguration me = sender as AudioConfiguration;
            if (me != null)
            {
                if (!me.IsUpdating)
                {
                    me.SetAudioList();
                }
                OnItemChanged(sender, e);
            }
        }
        public static readonly DependencyProperty AudioCollectionProperty =
           DependencyProperty.Register("AudioCollection", typeof(ObservableCollection<string>),
           typeof(AudioConfiguration), new PropertyMetadata(OnAudioCollectionChanged));

        public ObservableCollection<string> AudioCollection
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(AudioCollectionProperty);
            }
            private set
            {
                this.UIThreadSetValue(AudioCollectionProperty, value);
            }
        }



    }
}
