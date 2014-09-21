using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using log4net;
using RussLibrary;
using RussLibrary.Xml;
namespace ArtemisModLoader.Xml
{
    [XmlConversionRoot("ActiveModConfigurations")]
    public class ActiveModConfigurations : DependencyObject
    {
        public static ActiveModConfigurations Current
        {
            get;
            private set;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ActiveModConfigurations()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ResetActivations();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static readonly ILog _log = LogManager.GetLogger(typeof(ActiveModConfigurations));
        private ActiveModConfigurations() 
        {
            
            Configurations = new ConfigurationGroup();
            
        }
       
        public static void ResetActivations()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (_log.IsInfoEnabled) { _log.Info("~~~~~~~~~~~~~~~~~~~~ Starting ResetActivations in ActiveModConfigurations (for performance info) ~~~~~~~~~~~~~~~~~~"); }
            Current = new ActiveModConfigurations();

            XmlConverter.ToObject(Locations.ActiveModsFile, Current);

            if (_log.IsInfoEnabled) { _log.Info("~~~~~~~~~~~~~~~~~~~~ Ending ResetActivations in ActiveModConfigurations (for performance info) ~~~~~~~~~~~~~~~~~~"); }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        public bool IsAlreadyActive(string id)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            foreach (ModConfiguration c in Configurations.Configurations)
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
            if (config != null)
            {
                if (!IsAlreadyActive(config.ID))
                {
                    config.ActivateMod();
                    config.Sequence = Configurations.Configurations.Count;
                    this.UIThreadAddToCollection<ModConfiguration>( Configurations.Configurations, config);

                    SaveData();
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Mods")]
        public bool DeactivateStock()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = true;
            if (Configurations.Configurations.Count == 1)
            {
                ModConfiguration config = Configurations.Configurations[0];
                config.DeactivateMod();

                Configurations.Configurations.Remove(config);

            }
            else
            {

                retVal = false;
                if (Configurations.Configurations.Count > 1)
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
            if (Configurations.Configurations.Count > 1)
            {

                ModConfiguration config = Configurations.Configurations[Configurations.Configurations.Count - 1];
                if (_log.IsInfoEnabled)
                {
                    _log.InfoFormat("~~~~~~~~~~~~~ Beginning Deactivation of {0}", config.Title);
                }
                config.DeactivateMod();
                Application.Current.UIThreadRemoveFromCollection<ModConfiguration>(Configurations.Configurations, config); 

                //Configurations.Configurations.Remove(config);
                for (int i = Configurations.Configurations.Count - 1; i >= 0; i--)
                {
                    foreach (FileMap m in Configurations.Configurations[i].ActiveFiles.Files)
                    {
                        if (!File.Exists(m.Target))
                        {
                            if (File.Exists(m.Source))
                            {
                                if (_log.IsInfoEnabled)
                                {
                                    _log.InfoFormat("Restoring \"{0}\" from config {1}", m.Source, Configurations.Configurations[i].Title);
                                }

                                RussLibrary.Helpers.FileHelper.Copy(m.Source, m.Target);
                            }
                            else
                            {
                                if (_log.IsWarnEnabled)
                                {
                                    _log.WarnFormat("Source file is missing: \"{0}\"", m.Source);
                                }
                                //A source file from Mod "{0}" is missing.
                                StringBuilder sb = new StringBuilder();
                                sb.AppendFormat(AMLResources.Properties.Resources.SourceNotFoundPrefix, Configurations.Configurations[i].Title);
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

                }

                //Do Save.
                SaveData();
                if (_log.IsInfoEnabled)
                {
                    _log.InfoFormat("~~~~~~~~~~~~~ Ending Deactivation of {0}", config.Title);
                }
                Locations.MessageBoxShow(config.Title + " deactivated.", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else
            {
                retVal = false;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        public void SaveData()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            XmlDocument doc = XmlConverter.ToXmlDocument(this, true);
            doc.Save(Locations.ActiveModsFile);

            ConfigurationGroup hold = Configurations;
            Configurations = null;
            Configurations = hold;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public static readonly DependencyProperty ConfigurationsProperty =
            DependencyProperty.Register("Configurations", typeof(ConfigurationGroup),
            typeof(ActiveModConfigurations));
          [XmlConversion("Configurations")]
        public ConfigurationGroup Configurations
        {
            get
            {
                return (ConfigurationGroup)this.UIThreadGetValue(ConfigurationsProperty);

            }
            private set
            {
                this.UIThreadSetValue(ConfigurationsProperty, value);

            }
        }

    }
}
