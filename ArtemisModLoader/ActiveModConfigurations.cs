using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using RussLibrary;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;

namespace ArtemisModLoader
{

    internal class ActiveModConfigurations : XmlBase
    {
        public static ActiveModConfigurations Instance
        {
            get;
            private set;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ActiveModConfigurations()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Instance = new ActiveModConfigurations();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static readonly ILog _log = LogManager.GetLogger(typeof(ActiveModConfigurations));
        //
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        private ActiveModConfigurations()
            : base(Locations.ActiveModsFile)
        { }
        public static void ResetActivations()
        {
            Instance = new ActiveModConfigurations();
        }
        public bool IsAlreadyActive(string id)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            foreach (ModConfiguration c in Configurations)
            {
                if (id == c.ID)
                {
                    retVal = true;
                    break;
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        public bool ActivateConfiguration(ModConfiguration config)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = true;
            if (!IsAlreadyActive(config.ID))
            {
                config.ActivateMod();
                config.Sequence = Configurations.Count;
                Configurations.Add(config);

                SaveData();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Mods")]
        public bool DeactivateStock()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = true;
            if (Configurations.Count == 1)
            {
                ModConfiguration config = Configurations[0];
                config.DeactivateMod();

                Configurations.Remove(config);
               
            }
            else
            {

                retVal = false;
                if (Configurations.Count > 1)
                {
                    throw new InvalidOperationException("Cannot deactivate Stock while other Mods are active.");
                }

            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        public bool DeactivateLastConfig()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = true;
            if (Configurations.Count > 1)
            {
               
                ModConfiguration config = Configurations[Configurations.Count - 1];
                 if (_log.IsInfoEnabled)
                {
                    _log.InfoFormat("~~~~~~~~~~~~~ Beginning Deactivation of {0}", config.Title);
                }
                config.DeactivateMod();

                Configurations.Remove(config);
                for (int i = Configurations.Count - 1; i >= 0; i--)
                {
                    foreach (FileMap m in Configurations[i].ActiveFiles)
                    {
                        if (!File.Exists(m.Target))
                        {
                            if (File.Exists(m.Source))
                            {
                                if (_log.IsInfoEnabled)
                                {
                                    _log.InfoFormat("Restoring \"{0}\" from config {1}", m.Source, Configurations[i].Title);
                                }

                                File.Copy(m.Source, m.Target);
                            }
                            else
                            {
                                if (_log.IsWarnEnabled)
                                {
                                    _log.WarnFormat("Source file is missing: \"{0}\"", m.Source); 
                                }
                                //A source file from Mod "{0}" is missing.
                                StringBuilder sb = new StringBuilder();
                                sb.AppendFormat(AMLResources.Properties.Resources.SourceNotFoundPrefix, Configurations[i].Title);
                                sb.AppendLine();
                                sb.AppendLine();
                                sb.AppendLine(AMLResources.Properties.Resources.CorruptedMod);
                                sb.AppendLine();
                                sb.AppendLine(AMLResources.Properties.Resources.ProcessingContinue);
                                sb.AppendLine();
                                sb.AppendFormat(AMLResources.Properties.Resources.MissingFileLabel, m.Source);
                                Locations.MessageBoxShow(sb.ToString(),
                                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                        }
                    }

                    //Processing base files might not be needed if bug with active files fixed.
                    ////////if (Configurations[i].BaseFiles != null)
                    ////////{
                    ////////    foreach (FileMap m in Configurations[i].BaseFiles)
                    ////////    {
                    ////////        string target = Path.Combine(Locations.ArtemisCopyPath, m.Target);
                    ////////        if (m.Source.Contains("*") || m.Source.Contains("?"))
                    ////////        {
                    ////////            //more than one file, wildcarded.
                    ////////            string sourceFle = m.Source;

                    ////////            int i1 = sourceFle.LastIndexOf('\\');
                    ////////            sourceFle = sourceFle.Substring(0, i1);
                                

                    ////////            string fullSrc = Path.Combine(Configurations[i].InstalledPath, sourceFle);
                    ////////            foreach (FileInfo f in new DirectoryInfo(fullSrc).GetFiles("*.*", SearchOption.AllDirectories))
                    ////////            {
                    ////////                string relative = f.DirectoryName.Substring(fullSrc.Length + 1);
                    ////////                if (relative.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
                    ////////                {
                    ////////                    relative = relative.Substring(1);
                    ////////                }
                    ////////                if (!File.Exists(Path.Combine(Locations.ArtemisCopyPath, relative)))
                    ////////                {
                                        
                    ////////                    f.CopyTo(Path.Combine(Locations.ArtemisCopyPath, relative));
                    ////////                }
                                   
                    ////////            }

                    ////////        }
                    ////////        else
                    ////////        {

                                
                    ////////            if (!File.Exists(target))
                    ////////            {
                    ////////                string src = Path.Combine(Configurations[i].InstalledPath, m.Source);
                    ////////                if (File.Exists(src))
                    ////////                {
                    ////////                    File.Copy(src, target);
                    ////////                }
                    ////////                else
                    ////////                {
                    ////////                    if (_log.IsWarnEnabled)
                    ////////                    {
                    ////////                        _log.WarnFormat("Source file is missing: \"{0}\"", src);
                    ////////                    }
                    ////////                    Locations.MessageBoxShow("A source file from Mod \""
                    ////////                        + Configurations[i].Title
                    ////////                        + "\" is missing.\r\n\r\nThis could mean the mod is corrupted.\r\n\r\n"
                    ////////                        + "Processing will continue, but you may want to deactivate this mod and re-install.\r\n\r\n"
                    ////////                        + " Missing file: \"" + src + "\"",
                    ////////                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    ////////                }
                    ////////            }
                                
                    ////////        }
                    ////////    }
                    ////////}
                }

                //Do Save.
                SaveData();
                
            }
            else
            {
                retVal = false;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        void SaveData()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Save();
            ObservableCollection<ModConfiguration> hold = Configurations;
            Configurations = null;
            Configurations = hold;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public static readonly DependencyProperty ConfigurationsProperty =
            DependencyProperty.Register("Configurations", typeof(ObservableCollection<ModConfiguration>),
            typeof(ActiveModConfigurations), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        public ObservableCollection<ModConfiguration> Configurations
        {
            get
            {
                return (ObservableCollection<ModConfiguration>)this.UIThreadGetValue(ConfigurationsProperty);

            }
            private set
            {
                this.UIThreadSetValue(ConfigurationsProperty, value);

            }
        }
    }
}
