using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using RussLibrary;
using System.Xml;
using log4net;
using System.Reflection;
using SharpCompress.Reader;
using System;
using System.Text;
namespace ArtemisModLoader
{
    //Reads the MOD_*.xml file and loads the definition.
    
    public class ModConfiguration : XmlBase
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ModConfiguration));
        public ModConfiguration() : base() 
        {
            Download = new DownloadInfo();
        }


        public ModConfiguration(XmlNode node) : base(node) { }

        public ModConfiguration(string definitionPath)
            : base(definitionPath)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (string.IsNullOrEmpty(ID))
            {
                FileInfo f = new FileInfo(definitionPath);
                ID = f.Name;
                AcceptChanges();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public string PackagePath { get; set; }

        public void DeactivateSubMod()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            List<FileMap> files = new List<FileMap>();
            foreach (FileMap m in ActiveFiles)
            {
                if (m.ForSubMod)
                {
                    files.Add(m);
                }
            }
            foreach (FileMap m in files)
            {
                Locations.DeleteFile(m.Target);
                ActiveFiles.Remove(m);
            }
            foreach (SubMod sm in SubMods)
            {
                if (sm.IsActive)
                {
                    sm.IsActive = false;
                    break;
                }
            }
            ActiveSubMod = null;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Sub")]
        public void ActivateSubMod(string SubModTitle)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (string.IsNullOrEmpty(InstalledPath))
            {
                SetInstalledPath();
            }
            //Must first make sure the Mod itself is activated.
            string targetRootPath = Locations.ArtemisCopyPath;
            List<KeyValuePair<string, string>> targetList = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrEmpty(ActiveSubMod) && ActiveSubMod != SubModTitle)
            {
                DeactivateSubMod();
            }
            if (ActiveSubMod != SubModTitle)
            {


                foreach (SubMod sub in SubMods)
                {
                    if (sub.Title == SubModTitle)
                    {
                        foreach (FileMap m in sub.Files)
                        {
                            // <FileMap Source="Helm UI Mod\New UI Image Files\*" Target="dat"/>

                            if (m.Source.Contains("*") || m.Source.Contains("?"))
                            {
                                //more than one file, wildcarded.

                                string sourceFle = m.Source;

                                int i = sourceFle.LastIndexOf('\\');
                                sourceFle = sourceFle.Substring(0, i);
                                targetList.AddRange(Locations.CopyFiles(
                                    new DirectoryInfo(Path.Combine(InstalledPath, sourceFle)),
                                    Path.Combine(targetRootPath, m.Target)));

                            }
                            else
                            {

                                FileInfo src = new FileInfo(Path.Combine(InstalledPath, m.Source));
                                string targ = Path.Combine(Locations.ArtemisCopyPath, m.Target);
                                Locations.DeleteFile(targ);
                                src.CopyTo(targ);
                                targetList.Add(new KeyValuePair<string,string>(src.FullName, targ));
                            }
                        }
                        ActiveSubMod = sub.Title;
                        sub.IsActive = true;
                        break;
                    }

                }
            }

            foreach (KeyValuePair<string, string> targ in targetList)
            {

                ActiveFiles.Add(
                    new FileMap(targ.Key, targ.Value, true));

            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public void ActivateMod()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }   
            string targetRootPath = Locations.ArtemisCopyPath;
            List<KeyValuePair<string, string>> targetList = new List<KeyValuePair<string, string>>();
            if (string.IsNullOrEmpty(InstalledPath))
            {
                SetInstalledPath();
            }
            targetList.AddRange(Locations.CopyFiles(new DirectoryInfo(InstalledPath), targetRootPath));
            foreach (FileMap m in BaseFiles)
            {
                if (m.Source.Contains("*") || m.Source.Contains("?"))
                {
                    //more than one file, wildcarded.
                    string sourceFle = m.Source;

                    int i = sourceFle.LastIndexOf('\\');
                    sourceFle = sourceFle.Substring(0, i);
                    targetList.AddRange(Locations.CopyFiles(
                        new DirectoryInfo(Path.Combine(InstalledPath, sourceFle)), 
                        Path.Combine(targetRootPath, m.Target)));

                }
                else
                {

                    FileInfo src = new FileInfo(Path.Combine(InstalledPath, m.Source));
                    string targ = Path.Combine(Locations.ArtemisCopyPath, m.Target);
                    Locations.DeleteFile(targ);
                    src.CopyTo(targ);
                    targetList.Add(new KeyValuePair<string,string>(src.FullName, targ));
                }

            }
            ActiveFiles = new ObservableCollection<FileMap>();
            foreach (KeyValuePair<string, string> targ in targetList)
            {
                
               ActiveFiles.Add(new FileMap(targ.Key, targ.Value));
               
            }
            IsActive = true;
            if (this.SubMods != null && this.SubMods.Count > 0)
            {
                this.ActivateSubMod(SubMods[0].Title);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        private void SetInstalledPath()
        {
            InstalledPath = Path.Combine(Locations.InstalledModsPath,
                this.ID.Replace("\\", string.Empty).Replace("/", string.Empty).Replace(":", string.Empty).Replace("*", string.Empty).Replace("?", string.Empty).Replace("\"", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Replace("|", string.Empty));
        }
        public void InstallMod(string sourceRootPath)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            SetInstalledPath();

            Locations.CreatePath(InstalledPath);

            Locations.CopyFiles(new DirectoryInfo(sourceRootPath), InstalledPath);
            //Now update file that stores list of installed mods, storing "targetRootPath" location.
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        }

        /// <summary>
        /// Unpackages the specified zip file. Puts the package into the InstalledMod directory.
        ///   The Mod is now ready to be activated.
        /// </summary>
        /// <param name="zipFile">The zip file.</param>
        /// <param name="mod">The mod.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Unpackaging"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unpackage"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public string Unpackage(string zipFile)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string retVal = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ID))
                {
                    SetInstalledPath();
                    retVal = InstalledPath;
                    if (!Directory.Exists(retVal))
                    {
                        Locations.CreatePath(retVal);
                    }
                    //retVal will be path to where files were unzipped.
                    // Idea:  if can control, have it unzip straight to target.

                    using (Stream stream = File.OpenRead(zipFile))
                    {
                        IReader reader = ReaderFactory.Open(stream);
                        while (reader.MoveToNextEntry())
                        {
                            if (!reader.Entry.IsDirectory)
                            {

                                string target = Path.Combine(retVal, reader.Entry.FilePath);
                                Locations.DeleteFile(target);
                                if (_log.IsInfoEnabled)
                                {
                                    _log.InfoFormat("Unpackaging {0}, writing to {1}", reader.Entry.FilePath, target);
                                }

                                //Unpackaging "{0}", writing to "{1}".

                                ModManagement.DoMessage(string.Format(System.Globalization.CultureInfo.CurrentCulture, AMLResources.Properties.Resources.Unpackaging, reader.Entry.FilePath, target));
                                    
                                reader.WriteEntryToDirectory(retVal,
                                    SharpCompress.Common.ExtractOptions.ExtractFullPath |
                                    SharpCompress.Common.ExtractOptions.Overwrite);

                                FileInfo f = new FileInfo(target);
                                f.LastWriteTime = reader.Entry.LastModifiedTime.Value;
                              
                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Error unpackaging:", ex);
                }
                retVal = string.Empty;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
      
       
       
        /// <summary>
        /// Deactivates the mod.
        /// </summary>
        /// <param name="rootPath">The root path. (Location where mod is activated at).</param>
        public void DeactivateMod()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            //After deactivating mod, parent caller needs to then go through all remaining mods (including stock) and ensure
            // all files exist.
            foreach (FileMap m in ActiveFiles)
            {

                Locations.DeleteFile(m.Target);
            }
            ActiveFiles.Clear();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public bool UninstallMod()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = true;
            //First make sure that it is not active.  Must b
            if (string.IsNullOrEmpty(InstalledPath))
            {
                SetInstalledPath();
            }
            if (ActiveModConfigurations.Instance.Configurations.Contains(this))
            {
                retVal = false;
            }
            else
            {
                Locations.DeleteAllFiles(InstalledPath);
               
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }




        [XmlBase]
        public string Uninstall { get; private set; }

        [XmlBase]
        public string InstalledPath { get; private set; }


        public static readonly DependencyProperty DependsOnProperty =
           DependencyProperty.Register("DependsOn", typeof(string),
           typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));

        [XmlBase]
        public string DependsOn
        {
            get
            {
                return (string)this.UIThreadGetValue(DependsOnProperty);

            }
            set
            {
                this.UIThreadSetValue(DependsOnProperty, value);

            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public static readonly DependencyProperty IDProperty =
            DependencyProperty.Register("ID", typeof(string),
            typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));

        [XmlBase]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public string ID
        {
            get
            {
                return (string)this.UIThreadGetValue(IDProperty);

            }
            set
            {
                this.UIThreadSetValue(IDProperty, value);

            }
        }


        public static readonly DependencyProperty AuthorProperty =
            DependencyProperty.Register("Author", typeof(string),
            typeof(ModConfiguration));
        [XmlBase]
        public string Author
        {
            get
            {
                return (string)this.UIThreadGetValue(AuthorProperty);

            }
            set
            {
                this.UIThreadSetValue(AuthorProperty, value);

            }
        }



        public static readonly DependencyProperty SupportsInvasionModeProperty =
            DependencyProperty.Register("SupportsInvasionMode", typeof(bool),
            typeof(ModConfiguration), new PropertyMetadata(true));
        [XmlBase]
        public bool SupportsInvasionMode
        {
            get
            {
                return (bool)this.UIThreadGetValue(SupportsInvasionModeProperty);

            }
            set
            {
                this.UIThreadSetValue(SupportsInvasionModeProperty, value);

            }
        }



        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool),
            typeof(ModConfiguration));

        public bool IsActive
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsActiveProperty);

            }
            set
            {
                this.UIThreadSetValue(IsActiveProperty, value);

            }
        }




        public static readonly DependencyProperty SequenceProperty =
        DependencyProperty.Register("Sequence", typeof(int),
        typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));
        [XmlBase]
        public int Sequence
        {
            get
            {
                return (int)this.UIThreadGetValue(SequenceProperty);

            }
            set
            {
                this.UIThreadSetValue(SequenceProperty, value);

            }
        }

        public static readonly DependencyProperty ActiveSubModProperty =
         DependencyProperty.Register("ActiveSubMod", typeof(string),
         typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));
        [XmlBase]
        public string ActiveSubMod
        {
            get
            {
                return (string)this.UIThreadGetValue(ActiveSubModProperty);

            }
            set
            {
                this.UIThreadSetValue(ActiveSubModProperty, value);

            }
        }

      


        public static readonly DependencyProperty DownloadProperty =
            DependencyProperty.Register("Download", typeof(DownloadInfo),
            typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));
        [XmlBase]
        public DownloadInfo Download
        {
            get
            {
                return (DownloadInfo)this.UIThreadGetValue(DownloadProperty);

            }
            private set
            {
                this.UIThreadSetValue(DownloadProperty, value);

            }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string),
            typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));
        [XmlBase]
        public string Description
        {
            get
            {
                return (string)this.UIThreadGetValue(DescriptionProperty);

            }
            private set
            {
                this.UIThreadSetValue(DescriptionProperty, value);

            }
        }


        public static readonly DependencyProperty ActiveFilesProperty =
         DependencyProperty.Register("ActiveFiles", typeof(ObservableCollection<FileMap>),
         typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));

        /// <summary>
        /// Gets the active files.  Is the Fully qualified list of all files that are activated.
        /// </summary>
        [XmlBase]
        public ObservableCollection<FileMap> ActiveFiles
        {
            get
            {
                return (ObservableCollection<FileMap>)this.UIThreadGetValue(ActiveFilesProperty);

            }
            private set
            {
                this.UIThreadSetValue(ActiveFilesProperty, value);

            }
        }

        public static readonly DependencyProperty BaseFilesProperty =
           DependencyProperty.Register("BaseFiles", typeof(ObservableCollection<FileMap>),
           typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));
        [XmlBase]
        public ObservableCollection<FileMap> BaseFiles
        {
            get
            {
                return (ObservableCollection<FileMap>)this.UIThreadGetValue(BaseFilesProperty);

            }
            private set
            {
                this.UIThreadSetValue(BaseFilesProperty, value);

            }
        }


        public static readonly DependencyProperty LastVerifiedArtemisVersionProperty =
            DependencyProperty.Register("LastVerifiedArtemisVersion", typeof(string),
            typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));
        [XmlBase]
        public string LastVerifiedArtemisVersion
        {
            get
            {
                return (string)this.UIThreadGetValue(LastVerifiedArtemisVersionProperty);

            }
            private set
            {
                this.UIThreadSetValue(LastVerifiedArtemisVersionProperty, value);

            }
        }


        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
            typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));
        [XmlBase]
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



       

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mods")]
        public static readonly DependencyProperty SubModsProperty =
            DependencyProperty.Register("SubMods", typeof(ObservableCollection<SubMod>),
            typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));
        [XmlBase]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mods")]
        public ObservableCollection<SubMod> SubMods
        {
            get
            {
                return (ObservableCollection<SubMod>)this.UIThreadGetValue(SubModsProperty);

            }
            private set
            {
                this.UIThreadSetValue(SubModsProperty, value);

            }
        }




    }
}

