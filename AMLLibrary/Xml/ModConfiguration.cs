using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;
using log4net;
using RussLibrary;
using RussLibrary.Helpers;
using RussLibrary.WPF;
using RussLibrary.Xml;
using SharpCompress.Reader;

namespace ArtemisModLoader.Xml
{
    [XmlConversionRoot("ModConfiguration"),
    XmlComment("Packaged by Artemis Mod Loader\r\nDownload the latest version of Artemis Mod Loader from http://px2owffng8.embed.tal.ki/20121226/artemis-mod-loader-2176572/?viewmark=1")]
    public class ModConfiguration : ChangeDependencyObject
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ModConfiguration));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ModConfiguration()
        {
           


            Initialize();
        }

        void DependsOn_ObjectChanged(object sender, EventArgs e)
        {
            this.SetChanged();
        }
        public override void AcceptChanges()
        {
            if (DependsOn != null)
            {
                DependsOn.AcceptChanges();
                
            }
            Download.AcceptChanges();
            if (BaseFiles != null)
            {
                BaseFiles.AcceptChanges();
            }
            if (ActiveFiles != null)
            {
                ActiveFiles.AcceptChanges();
            }
            if (SubMods != null)
            {
                SubMods.AcceptChanges();
            }
            base.AcceptChanges();
        }
        public override void BeginInitialization()
        {
            base.BeginInitialization();
            if (DependsOn != null)
            {
                DependsOn.BeginInitialization();

            }
            if (Download != null)
            {
                Download.BeginInitialization();
            }
            if (BaseFiles != null)
            {
                BaseFiles.BeginInitialization();
            }
            if (ActiveFiles != null)
            {
                ActiveFiles.BeginInitialization();
            }
            if (SubMods != null)
            {
                SubMods.BeginInitialization();
            }
        }
        public override void EndInitialization()
        {
            Download.EndInitialization();
            if (BaseFiles != null)
            {
                BaseFiles.EndInitialization();
            }
            if (ActiveFiles != null)
            {
                ActiveFiles.EndInitialization();
            }
            if (SubMods != null)
            {
                SubMods.EndInitialization();
            }
            if (DependsOn != null)
            {
                DependsOn.EndInitialization();
                
            }
            base.EndInitialization();
        }
        public override void RejectChanges()
        {
            DependsOn.RejectChanges();
            
            Download.RejectChanges();
            BaseFiles.RejectChanges();
            ActiveFiles.RejectChanges();
            SubMods.RejectChanges();
            base.RejectChanges();
        }
        void Initialize()
        {
           
            Download = new DownloadInfo();
            BaseFiles = new FileGroup();
            ActiveFiles = new FileGroup();
            SubMods = new SubModGroup();
            DependsOn = new StringItemCollection();

            DependsOn.ObjectChanged += new EventHandler(DependsOn_ObjectChanged);

            Download.ObjectChanged += new EventHandler(DependsOn_ObjectChanged);

            BaseFiles.ObjectChanged += new EventHandler(DependsOn_ObjectChanged);


            SubMods.ObjectChanged += new EventHandler(DependsOn_ObjectChanged);

            

        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ModConfiguration(string definitionFile)
        {
            Initialize();

            ModConfiguration config = XmlConverter.ToObject(definitionFile, typeof(ModConfiguration)) as ModConfiguration;
            if (config != null)
            {
                this.CopyProperties(config);
            }

        }
        /// <summary>
        /// Gets or sets the package path.  This is the Zip file source.
        /// </summary>
        /// <value>
        /// The package path.
        /// </value>
        [XmlConversion("Package")]
        public string PackagePath { get; set; }

        public void DeactivateSubMod()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            List<FileMap> files = new List<FileMap>();
            foreach (FileMap m in ActiveFiles.Files)
            {
                if (m.ForSubMod)
                {
                    files.Add(m);
                }
            }
            foreach (FileMap m in files)
            {
                FileHelper.DeleteFile(m.Target);
                ActiveFiles.Files.Remove(m);
            }
            foreach (SubMod sm in SubMods.SubMods)
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
            List<DictionaryEntry> targetList = new List<DictionaryEntry>();
            if (!string.IsNullOrEmpty(ActiveSubMod) && ActiveSubMod != SubModTitle)
            {
                DeactivateSubMod();
            }
            if (ActiveSubMod != SubModTitle)
            {


                foreach (SubMod sub in SubMods.SubMods)
                {
                    if (sub.Title == SubModTitle)
                    {
                        foreach (FileMap m in sub.Files.Files)
                        {
                            // <FileMap Source="Helm UI Mod\New UI Image Files\*" Target="dat"/>

                            if (m.Source.Contains("*") || m.Source.Contains("?"))
                            {
                                //more than one file, wildcarded.

                                string sourceFle = m.Source;

                                int i = sourceFle.LastIndexOf('\\');
                                sourceFle = sourceFle.Substring(0, i);
                                targetList.AddRange(FileHelper.CopyFiles(
                                    new DirectoryInfo(Path.Combine(InstalledPath, sourceFle)),
                                    Path.Combine(targetRootPath, m.Target)));
                                if (!string.IsNullOrEmpty(FileHelper.CopyFilesProblemMessage))
                                {
                                    ModManagement.SetStandbyBack();
                                    Locations.MessageBoxShow(FileHelper.CopyFilesProblemMessage, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                    FileHelper.CopyFilesProblemMessage = null;
                                }

                            }
                            else
                            {

                                string src = Path.Combine(InstalledPath, m.Source);
                                string targ = Path.Combine(Locations.ArtemisCopyPath, m.Target);
                                
                                
                                FileHelper.Copy(src, targ);
                                targetList.Add(new DictionaryEntry(src, targ));
                            }
                        }
                        ActiveSubMod = sub.Title;
                        sub.IsActive = true;
                        break;
                    }

                }
            }

            foreach (DictionaryEntry targ in targetList)
            {
                FileMap fm = Application.Current.Dispatcher.Invoke(new Func<string, string, bool, FileMap>(GetNewFileMap), targ.Key.ToString(), targ.Value.ToString(), true) as FileMap;
                if (fm != null)
                {
                    this.UIThreadAddToCollection<FileMap>(ActiveFiles.Files, fm);
                }
                //ActiveFiles.Files.Add(
                //    new FileMap(targ.Key.ToString(), targ.Value.ToString(), true));

            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        FileInfo[] GetFilesFromBaseFiles(FileMapCollection basefiles)
        {
            List<FileInfo> retVal = new List<FileInfo>();
            foreach (FileMap m in basefiles)
            {
                if (m.Source.Contains("*") || m.Source.Contains("?"))
                {
                    //more than one file, wildcarded.
                    string sourceFle = m.Source;

                    int i = sourceFle.LastIndexOf('\\');
                    if (i >= 0)
                    {
                        sourceFle = sourceFle.Substring(0, i);
                    }
                    string filePath = Path.Combine(InstalledPath, sourceFle);
                    if (Directory.Exists(filePath))
                    {
                        retVal.AddRange(new DirectoryInfo(filePath).GetFiles("*.*", SearchOption.AllDirectories));
                    }
                    else
                    {
                        Locations.MessageBoxShow(
                            string.Format("MOD Definition is INCORRECT.\r\nPath \"{0}\" was NOT FOUND in MOD package.", filePath),
                            MessageBoxButton.OK, MessageBoxImage.Hand);
                    }
                }
                else
                {

                    string src = Path.Combine(InstalledPath, m.Source);
                    retVal.Add(new FileInfo(src));
                    
                }

            }
            return retVal.ToArray();
        }
        public void ActivateMod()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string targetRootPath = Locations.ArtemisCopyPath;
            List<DictionaryEntry> targetList = new List<DictionaryEntry>();
            
            FileInfo[] baseFileList = GetFilesFromBaseFiles(BaseFiles.Files);

            if (string.IsNullOrEmpty(InstalledPath))
            {
                SetInstalledPath();
            }
            targetList.AddRange(FileHelper.CopyFiles(new DirectoryInfo(InstalledPath), targetRootPath, baseFileList));
            if (!string.IsNullOrEmpty(FileHelper.CopyFilesProblemMessage))
            {
                ModManagement.SetStandbyBack();
                Locations.MessageBoxShow(FileHelper.CopyFilesProblemMessage, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                FileHelper.CopyFilesProblemMessage = null;
            }
            foreach (FileMap m in BaseFiles.Files)
            {
                if (m.Source.Contains("*") || m.Source.Contains("?"))
                {
                    //more than one file, wildcarded.
                    string sourceFle = m.Source;

                    int i = sourceFle.LastIndexOf('\\');
                    if (i >= 0)
                    {
                        sourceFle = sourceFle.Substring(0, i);
                    }
                    targetList.AddRange(FileHelper.CopyFiles(
                        new DirectoryInfo(Path.Combine(InstalledPath, sourceFle)),
                        Path.Combine(targetRootPath, m.Target)));
                    if (!string.IsNullOrEmpty(FileHelper.CopyFilesProblemMessage))
                    {
                        ModManagement.SetStandbyBack();
                        Locations.MessageBoxShow(FileHelper.CopyFilesProblemMessage, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        FileHelper.CopyFilesProblemMessage = null;
                    }
                }
                else
                {

                    string src = Path.Combine(InstalledPath, m.Source);
                    string targ = Path.Combine(Locations.ArtemisCopyPath, m.Target);
                    FileHelper.Copy(src, targ);
                    targetList.Add(new DictionaryEntry(src, targ));
                }

            }
            ActiveFiles = Application.Current.Dispatcher.Invoke(new Func<FileGroup>(GetNewFileGroup)) as FileGroup;
            if (ActiveFiles != null)
            {
                foreach (DictionaryEntry targ in targetList)
                {
                    FileMap fm = Application.Current.Dispatcher.Invoke(new Func<string, string, bool, FileMap>(GetNewFileMap), targ.Key.ToString(), targ.Value.ToString(), false) as FileMap;
                    if (fm != null)
                    {
                        ActiveFiles.UIThreadAddToCollection<FileMap>(ActiveFiles.Files, fm);
                    }
                    //ActiveFiles.Files.Add(new FileMap(targ.Key.ToString(), targ.Value.ToString()));

                }
            }
            
            if (this.SubMods != null && this.SubMods.SubMods.Count > 0)
            {

                this.ActivateSubMod(SubMods.SubMods[0].Title);
            }
            ModManagement.SetStandbyBack();
            IsActive = true;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static FileMap GetNewFileMap(string key, string value, bool forSubMod)
        {
            return new FileMap(key, value, forSubMod);
        }
        static FileGroup GetNewFileGroup()
        {
            return new FileGroup();
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

            FileHelper.CopyFiles(new DirectoryInfo(sourceRootPath), InstalledPath);
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
                        FileHelper.CreatePath(retVal);
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
                                FileHelper.DeleteFile(target);
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
            foreach (FileMap m in ActiveFiles.Files)
            {

                FileHelper.DeleteFile(m.Target);
            }
            ActiveFiles.Files.Clear();
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
            if (ActiveModConfigurations.Current.Configurations.Configurations.Contains(this))
            {
                retVal = false;
            }
            else
            {
                FileHelper.DeleteAllFiles(InstalledPath);

            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }




        [XmlConversion("Uninstall")]
        public string Uninstall { get; private set; }

        
     
        public static readonly DependencyProperty InstalledPathProperty =
           DependencyProperty.Register("InstalledPath", typeof(string),
           typeof(ModConfiguration), new PropertyMetadata(OnItemChanged));


        [XmlConversion("InstalledPath")]
        public string InstalledPath
        {
            get
            {
                return (string)this.UIThreadGetValue(InstalledPathProperty);

            }
            set
            {
                this.UIThreadSetValue(InstalledPathProperty, value);

            }
        }

        public static readonly DependencyProperty DependsOnProperty =
           DependencyProperty.Register("DependsOn", typeof(StringItemCollection),
           typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("DependsOn")]
        public StringItemCollection DependsOn
        {
            get
            {
                return (StringItemCollection)this.UIThreadGetValue(DependsOnProperty);

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

        [XmlConversion("ID")]
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
            typeof(ModConfiguration), new PropertyMetadata(OnItemChanged));
        [XmlConversion("Author")]
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
            typeof(ModConfiguration), new PropertyMetadata(true, OnItemChanged));
        [XmlConversion("SupportsInvasionMode")]
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



        public static readonly DependencyProperty DefinitionVersionProperty =
            DependencyProperty.Register("DefinitionVersion", typeof(decimal),
            typeof(ModConfiguration), new PropertyMetadata(0M, OnItemChanged));
        [XmlConversion("DefinitionVersion")]
        public decimal DefinitionVersion
        {
            get
            {
                return (decimal)this.UIThreadGetValue(DefinitionVersionProperty);

            }
            set
            {
                this.UIThreadSetValue(DefinitionVersionProperty, value);

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
        typeof(ModConfiguration));
        [XmlConversion("Sequence")]
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
         typeof(ModConfiguration));
        [XmlConversion("ActiveSubMod")]
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
        [XmlConversion("Download")]
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
        [XmlConversion("Description")]
        public string Description
        {
            get
            {
                return (string)this.UIThreadGetValue(DescriptionProperty);

            }
            set
            {
                this.UIThreadSetValue(DescriptionProperty, value);

            }
        }


        public static readonly DependencyProperty ActiveFilesProperty =
         DependencyProperty.Register("ActiveFiles", typeof(FileGroup),
         typeof(ModConfiguration));

        /// <summary>
        /// Gets the active files.  Is the Fully qualified list of all files that are activated.
        /// </summary>
        [XmlConversion("ActiveFiles")]
        public FileGroup ActiveFiles
        {
            get
            {
                return (FileGroup)this.UIThreadGetValue(ActiveFilesProperty);

            }
            set
            {
                this.UIThreadSetValue(ActiveFilesProperty, value);

            }
        }

        public static readonly DependencyProperty BaseFilesProperty =
           DependencyProperty.Register("BaseFiles", typeof(FileGroup),
           typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));
        [XmlConversion("BaseFiles")]
        public FileGroup BaseFiles
        {
            get
            {
                return (FileGroup)this.UIThreadGetValue(BaseFilesProperty);

            }
            set
            {
                this.UIThreadSetValue(BaseFilesProperty, value);

            }
        }


        public static readonly DependencyProperty LastVerifiedArtemisVersionProperty =
            DependencyProperty.Register("LastVerifiedArtemisVersion", typeof(string),
            typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));
        [XmlConversion("LastVerifiedArtemisVersion")]
        public string LastVerifiedArtemisVersion
        {
            get
            {
                return (string)this.UIThreadGetValue(LastVerifiedArtemisVersionProperty);

            }
            set
            {
                this.UIThreadSetValue(LastVerifiedArtemisVersionProperty, value);

            }
        }


        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
            typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));
        [XmlConversion("Title")]
        public string Title
        {
            get
            {
                return (string)this.UIThreadGetValue(TitleProperty);

            }
            set
            {
                this.UIThreadSetValue(TitleProperty, value);

            }
        }





        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mods")]
        public static readonly DependencyProperty SubModsProperty =
            DependencyProperty.Register("SubMods", typeof(SubModGroup),
            typeof(ModConfiguration), new UIPropertyMetadata(OnItemChanged));
        [XmlConversion("SubMods")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mods")]
        public SubModGroup SubMods
        {
            get
            {
                return (SubModGroup)this.UIThreadGetValue(SubModsProperty);

            }
            set
            {
                this.UIThreadSetValue(SubModsProperty, value);

            }
        }

        public void Save(string file)
        {
            XmlDocument doc = XmlConverter.ToXmlDocument(this, true);
            doc.Save(file);
            this.AcceptChanges();
        }

        public bool IsChanged
        {
            get
            {
                return Changed || Download.Changed || SubMods.Changed || BaseFiles.Changed;
            }
        }
        protected override void ProcessValidation()
        {
            
            if (string.IsNullOrEmpty(this.ID))
            {
                base.ValidationCollection.AddValidation(DataStrings.ID,
                        ValidationValue.IsError,
                        string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.IsRequired, AMLResources.Properties.Resources.ID));
            }
            if (string.IsNullOrEmpty(this.Title))
            {
                base.ValidationCollection.AddValidation(DataStrings.Title,
                    ValidationValue.IsError,
                    string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.IsRequired, AMLResources.Properties.Resources.TitleText));
            }
            if (string.IsNullOrEmpty(this.Description))
            {
                base.ValidationCollection.AddValidation(
                    DataStrings.Description, ValidationValue.IsWarnState, AMLResources.Properties.Resources.DescriptionValidation);
            }
            if (string.IsNullOrEmpty(this.Author))
            {
                base.ValidationCollection.AddValidation(DataStrings.Author,
                    ValidationValue.IsWarnState, AMLResources.Properties.Resources.AuthorValidation);
            }
            if (this.DependsOn != null)
            {
                bool found = false;
                foreach (ModConfiguration config in InstalledModConfigurations.Current.Configurations.Configurations)
                {
                    if (found)
                    {
                        break;
                    }
                    else
                    {
                        foreach (StringItem item in this.DependsOn)
                        {
                            if (config.ID == item.Text)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }
                if (!found)
                {
                    base.ValidationCollection.AddValidation(DataStrings.DependsOn,
                        ValidationValue.IsWarnState, AMLResources.Properties.Resources.DependsOnValidation);
                }
            }
            
        }
    }
}
