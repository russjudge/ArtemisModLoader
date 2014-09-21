using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtemisComm
{
    public static class Utility
    {
        public static string BytesToDebugString(byte[] byteArray)
        {
            return BytesToDebugString(byteArray, 0);
        }

        public static string BytesToDebugString(byte[] byteArray, int index)
        {
            return BitConverter.ToString(byteArray, index).Replace("-", ":");
        }
    }
}
