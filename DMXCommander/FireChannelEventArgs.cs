using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMXCommander
{
    public class FireChannelEventArgs : EventArgs
    {
        public FireChannelEventArgs(int priority, int channel, byte value, int milliseconds, decimal delta )
        {
            Priority = priority;
            Channel = channel;
            Value = value;
            Delta = delta;
            Milliseconds = milliseconds;
        }
        public int Channel { get; private set; }
        public int Priority { get; private set; }
        public byte Value { get; private set; }
        /// <summary>
        /// How much to alter value each millisecond.
        /// </summary>
        public decimal Delta { get; private set; }
        /// <summary>
        /// Defines the number of milliseconds to keep in effect.
        /// </summary>
        public int Milliseconds { get; private set; }
    }
}
