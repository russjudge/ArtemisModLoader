using log4net;
using RussLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;

namespace DMXCommand
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(App));

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            GeneralHelper.ShowApplicationError(e.Exception);
            Shutdown(1);
        }
    }
}
