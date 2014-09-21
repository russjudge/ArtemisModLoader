using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace RussLibrary.Helpers
{

    public class DragStartedEventArgs : EventArgs
    {

        public object DragObject { get; internal set; }
    }
}