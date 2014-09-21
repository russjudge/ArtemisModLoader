using DMXCommander.Xml;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DMXCommander.Engine
{
    public class Controller : IDisposable
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(Controller));
        Processor prc = new Processor();

        List<EventObject> ActiveEvents = new List<EventObject>();
        Dictionary<int, int> ChannelPriorities = new Dictionary<int, int>();
        Dictionary<int, List<int>> PriorityChannels = new Dictionary<int, List<int>>();
        private Controller()
        {
        }
        public static Controller Current { get; private set; }
        static Controller()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Current = new Controller();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        public void ActivateEvent(EventObject eventObject)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (!ActiveEvents.Contains(eventObject) && eventObject != null)
            {

                eventObject.FireChannel += eventObject_FireChannel;
                List<int> valueList = new List<int>();
                foreach (TimeBlock block in eventObject.TimeBlocks)
                {
                    foreach (SetValue value in block.Values)
                    {
                        if (!valueList.Contains(value.Channel))
                        {
                            valueList.Add(value.Channel);
                        }
                    }
                }

                eventObject.Activate();

                ActiveEvents.Add(eventObject);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        object EventLockObject = new object();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        void eventObject_FireChannel(object sender, FireChannelEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            //Check priorities here.  Processor is stupid--only does what told.  Controller handles priority issues.
            bool OkayToFire = false;
            lock (EventLockObject)
            {
                if (ChannelPriorities.ContainsKey(e.Channel))
                {
                    if (e.Priority > ChannelPriorities[e.Channel])
                    {
                        ChannelPriorities[e.Channel] = e.Priority;
                    }

                    if (ChannelPriorities[e.Channel] == e.Priority)
                    {
                        OkayToFire = true;
                    }
                }
                else
                {
                    OkayToFire = true;
                    ChannelPriorities.Add(e.Channel, e.Priority);

                }

                if (PriorityChannels.ContainsKey(e.Priority))
                {
                    if (!PriorityChannels[e.Priority].Contains(e.Channel))
                    {
                        PriorityChannels[e.Priority].Add(e.Channel);
                    }
                }
                else
                {
                    PriorityChannels.Add(e.Priority, new List<int>());
                    PriorityChannels[e.Priority].Add(e.Channel);
                }
            }
            if (OkayToFire)
            {
                prc.SetChannel(e.Channel, e.Value, e.Delta, e.Milliseconds);
                //if (e.Milliseconds > 2)
                //{
                //    System.Timers.Timer tmr = new System.Timers.Timer(e.Milliseconds);
                //    tmr.Elapsed += tmrDeactivateChannel_Elapsed;
                //    tmr.AutoReset = false;
                //    ChannelsToDeactivate.Add(tmr, e);
                //    tmr.Start();
                //}
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        //Dictionary<System.Timers.Timer, FireChannelEventArgs> ChannelsToDeactivate = new Dictionary<System.Timers.Timer, FireChannelEventArgs>();
        //void tmrDeactivateChannel_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    System.Timers.Timer tmr = sender as System.Timers.Timer;
        //    if (tmr != null)
        //    {
        //        tmr.Stop();
        //        FireChannelEventArgs fc = ChannelsToDeactivate[tmr];
        //        System.Windows.Application.Current.Dispatcher.Invoke(new Action<int, int>(DeactivateChannel), fc.Channel, fc.Priority);

        //        tmr.Dispose();
        //        tmr = null;

        //    }


        //}
       
        public void DeactivateChannel(int channel, int priority)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (ChannelPriorities.ContainsKey(channel))
            {
                if (ChannelPriorities[channel] == priority)
                {
                    ChannelPriorities[channel] = 0;

          

                    prc.SetChannel(channel, 0, 0, 0);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public void ResetAllChannels()
        {
            for (int i = 0; i < 512; i++)
            {
                prc.SetChannel(i, 0, 0, 0);
            }
        }
        public void DeactivateEvent(EventObject eventObject)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (eventObject != null)
            {
                eventObject.Stop();
                if (ActiveEvents.Contains(eventObject))
                {
                    eventObject.FireChannel -= eventObject_FireChannel;

                    if (PriorityChannels.ContainsKey(eventObject.Priority))
                    {
                        foreach (int channel in PriorityChannels[eventObject.Priority])
                        {
                            DeactivateChannel(channel, eventObject.Priority);
                            
                        }
                    }
                    ActiveEvents.Remove(eventObject);

                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        protected virtual void Dispose(bool isDisposing)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    if (prc != null)
                    {
                        prc.Dispose();
                    }
                   
                    
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
