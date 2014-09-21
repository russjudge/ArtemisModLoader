using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DMXCommander.Engine
{
    public class Processor : IDisposable
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(Processor));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges")]
        public Processor()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (!bool.TryParse(ConfigurationManager.AppSettings["TestMode"], out InTestMode))
            {
                InTestMode = false;
            }
            ProcessTimer.AutoReset = true;
            ProcessTimer.Elapsed += ProcessTimer_Elapsed;
            if (!InTestMode)
            {
                if (!OpenDMX.IsStarted)
                {
                    throw new InvalidOperationException("DMX not started.");
                }
                //OpenDMX.Start();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        object lockObject1 = new object();
        void ProcessTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            List<int> keys = new List<int>(ChangingChannels.Keys);

            foreach (int channel in keys)
            {
                decimal newValue = ChangingChannels[channel].Value + ChangingChannels[channel].Delta;
                if (newValue > byte.MaxValue)
                {
                    newValue = byte.MaxValue;
                }

                ChangingChannels[channel].Value = newValue;
                ChangingChannels[channel].MillisecondsRemaining -= Convert.ToInt32(ProcessTimer.Interval);

                

                ApplyNewChannelValue(channel, Convert.ToByte(newValue));
                if (ChangingChannels[channel].MillisecondsRemaining <= 0)
                {
                    ChangingChannels.Remove(channel);
                    if (ChangingChannels.Count == 0)
                    {
                        ProcessTimer.Stop();

                    }

                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        bool InTestMode = false;
        public void SetChannel(int channel, byte value, decimal delta, int milliseconds)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ApplyNewChannelValue(channel, value);

            if (delta == 0)
            {
                ClearChannelDelta(channel);
            }
            else
            {
                SetChannelDelta(channel, value, delta, milliseconds);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void ApplyNewChannelValue(int channel, byte value)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (InTestMode)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.WarnFormat("Set Channel {0}, Value {1}",
                        channel.ToString(System.Globalization.CultureInfo.CurrentCulture),
                        value.ToString(System.Globalization.CultureInfo.CurrentCulture));
                }
            }
            else
            {
      
                OpenDMX.SetDmxValue(channel, value);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        
        void ClearChannelDelta(int channel)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (ChangingChannels.ContainsKey(channel))
            {
                lock (lockObject1)
                {
                    ChangingChannels.Remove(channel);
                    if (ChangingChannels.Count == 0)
                    {
                        TimerIsTicking = false;
                        ProcessTimer.Stop();
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void SetChannelDelta(int channel, byte startValue, decimal delta, int milliseconds)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (!ChangingChannels.ContainsKey(channel))
            {

                ChangingChannels.Add(channel, new ChangingChannelDefinition(startValue, delta, milliseconds));

            }
            else
            {

                ChangingChannels[channel] = new ChangingChannelDefinition(startValue, delta, milliseconds);
            }
            if (!TimerIsTicking)
            {
                ProcessTimer.Start();
                TimerIsTicking = true;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        System.Timers.Timer ProcessTimer = new System.Timers.Timer(35);
        bool TimerIsTicking = false;
        class ChangingChannelDefinition
        {
            public ChangingChannelDefinition(decimal value, decimal delta, int milliseconds)
            {
                Value = value;
                Delta = delta;
                MillisecondsRemaining = milliseconds;
            }
            public decimal Value { get; set; }
            public decimal Delta { get; private set; }
            public int MillisecondsRemaining { get; set; }
        }
        Dictionary<int, ChangingChannelDefinition> ChangingChannels = new Dictionary<int, ChangingChannelDefinition>();
        protected virtual void Dispose(bool isDisposing)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    if (ProcessTimer != null)
                    {
                        ProcessTimer.Stop();
                        ProcessTimer.Dispose();
                        ProcessTimer = null;
                    }
                    OpenDMX.Stop();
                    isDisposed = true;
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        bool isDisposed = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Dispose(true);
            GC.SuppressFinalize(this);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
    }
}
