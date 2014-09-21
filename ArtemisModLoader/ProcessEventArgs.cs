using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtemisModLoader
{
    public class ProcessEventArgs : EventArgs
    {
        public ProcessEventArgs(bool result, ModConfiguration config)
        {
            Result = result;
            Configuration = config;
        }
        public bool Result { get; private set; }
        public ModConfiguration Configuration { get; private set; }
    }
}
