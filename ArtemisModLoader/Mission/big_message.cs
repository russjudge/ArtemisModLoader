using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using RussLibrary;
using System.Xml;
using System.Windows;
using System.IO;

namespace ArtemisModLoader.Mission
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "message"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "big")]
    public class big_message : DependencyObject
    {
     
        public big_message(string missionFile) : base()
        {
            string data = null;
            using (StreamReader sr = new StreamReader(missionFile))
            {
                data = sr.ReadToEnd();

            }
            int strt = data.IndexOf("<big_message", StringComparison.OrdinalIgnoreCase);
            int end = 0;
            if (strt >= 0)
            {
                end = data.IndexOf("/>", strt, StringComparison.OrdinalIgnoreCase);
                string line = data.Substring(strt, end - strt);


                strt = line.IndexOf("title", StringComparison.OrdinalIgnoreCase) + 5;
                while (line[++strt] != '\"' && strt < line.Length - 1) ;
                end = strt++;
                while (line[++end] != '\"' && end < line.Length - 1) ;
                if (end > strt && strt > -1)
                {
                    title = line.Substring(strt, end - strt);
                }

                strt = line.IndexOf("subtitle1", StringComparison.OrdinalIgnoreCase) + 5;
                while (line[++strt] != '\"' && strt < line.Length - 1) ;
                end = strt++;
                while (line[++end] != '\"' && end < line.Length - 1) ;
                if (end > strt && strt > -1)
                {
                    subtitle1 = line.Substring(strt, end - strt);
                }
            }
            else
            {
                title = new FileInfo(missionFile).Name.Substring(5);
                title = title.Substring(0, title.Length - 4);

            }
            MissionPath = missionFile;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Filename")]
        public static readonly DependencyProperty MissionFilenameProperty =
            DependencyProperty.Register("MissionFilename", typeof(string),
            typeof(big_message));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Filename")]
        public string MissionFilename
        {
            get
            {
                return (string)this.UIThreadGetValue(MissionFilenameProperty);

            }
            private set
            {
                this.UIThreadSetValue(MissionFilenameProperty, value);

            }
        }
        static void OnMissionPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            big_message me = sender as big_message;
            if (me != null)
            {
                if (!string.IsNullOrEmpty(me.MissionPath))
                {
                    me.MissionFilename = new FileInfo(me.MissionPath).Name;
                }
            }
        }
        public static readonly DependencyProperty MissionPathProperty =
            DependencyProperty.Register("MissionPath", typeof(string),
            typeof(big_message), new PropertyMetadata(OnMissionPathChanged));
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "title")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        
        public static readonly DependencyProperty titleProperty =
            DependencyProperty.Register("title", typeof(string),
            typeof(big_message));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "title")]
        public string title
        {
            get
            {
                return (string)this.UIThreadGetValue(titleProperty);

            }
            private set
            {
                this.UIThreadSetValue(titleProperty, value);

            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "subtitle")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static readonly DependencyProperty subtitle1Property =
          DependencyProperty.Register("subtitle1", typeof(string),
          typeof(big_message));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "subtitle")]
        public string subtitle1
        {
            get
            {
                return (string)this.UIThreadGetValue(subtitle1Property);

            }
            private set
            {
                this.UIThreadSetValue(subtitle1Property, value);

            }
        }
    }
}
