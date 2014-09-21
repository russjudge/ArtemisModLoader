using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Windows;
using log4net;
using ArtemisModLoader.Xml;
using RussLibrary.Helpers;

namespace ArtemisModLoader
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ProjectInstaller));
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static void UpdateProperties(ModConfiguration source, ModConfiguration target)
        {
            try
            {
                if (target.ID == source.ID)
                {



                }
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Exception updating config", ex);
                }
            }
            
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static void UpdateConfig(ModConfiguration installedConfig)
        {

            foreach (System.IO.FileInfo fle in new System.IO.DirectoryInfo(Locations.PredefinedModsPath).GetFiles())
            {
                try
                {
                    ModConfiguration config = new ModConfiguration(fle.FullName);
                    UpdateProperties(config, installedConfig);
                }
                catch (Exception ex)
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.Warn("Exception updating config", ex);
                    }
                }

            }


        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void AfterInstall_Event(object sender, InstallEventArgs e)
        {

            try
            {
                foreach (ModConfiguration config in InstalledModConfigurations.Current.Configurations.Configurations)
                {

                    UpdateConfig(config);

                }
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Exception updating config", ex);
                }
            }
            try
            {
                foreach (ModConfiguration config in ActiveModConfigurations.Current.Configurations.Configurations)
                {

                    UpdateConfig(config);

                }
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Exception updating config", ex);
                }
            }
        }

        private void AfterUninstall_Event(object sender, InstallEventArgs e)
        {
            if (Locations.MessageBoxShow(AMLResources.Properties.Resources.RemoveAllUserFilesQuestion,
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                FileHelper.GeneralResult = true;
                FileHelper.DeleteAllFiles(Locations.DataPath);
                if (!FileHelper.GeneralResult)
                {
                    Locations.MessageBoxShow(AMLResources.Properties.Resources.NotAllRemoved,
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void AfterCommitted_Event(object sender, InstallEventArgs e)
        {

        }
    }
}
