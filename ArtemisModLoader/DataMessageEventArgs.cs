using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace ArtemisModLoader
{

    public class DataMessageEventArgs : EventArgs
    {

        public DataMessageEventArgs(string message)
        {
            
            Message = message;
            
        }
        public string Message { get; private set; }
    }
}
