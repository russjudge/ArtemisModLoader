using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RussLibrary.Windows
{
    public interface ISettingsPanel
    {
        void SaveSettings();
        void CancelChanges();
        void LoadSettings();
        void SetConfigurationPath(string path);
        string Header { get; }
     

    }
}
