using ArtemisComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtemisCommSandbox
{
    public interface IPackageSelector
    {
        Packet Package { get; set; }
    }
}
