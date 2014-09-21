using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RussLibrary.Xml;
using RussLibrary.WPF;
using System.Xml;
using System.Windows;
using RussLibrary;
using log4net;
using System.Reflection;

namespace DMXCommander.Xml
{
 
    [XmlConversionRoot("event")]
    public class EventObject : ChangeDependencyObject, IXmlStorage, IDisposable
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(EventObject));

        public EventObject()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Storage = new List<XmlNode>();
            TimeBlocks = new TimeBlockCollection();
            TimeBlocks.ObjectChanged += TimeBlocks_ObjectChanged;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        
        public event EventHandler<FireChannelEventArgs> FireChannel;

        public void Stop()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            foreach (System.Timers.Timer tmr in this.ActiveTimers)
            {
                tmr.Stop();
                tmr.Dispose();
            }
            ActiveTimers.Clear();
            ActiveTimeBlocks.Clear();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        List<System.Timers.Timer> ActiveTimers = new List<System.Timers.Timer>();
        Queue<TimeBlock> ActiveTimeBlocks = new Queue<TimeBlock>();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public void Activate()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            int timeFrame = 0;
            System.Timers.Timer tmr = null;

            try
            {
                foreach (TimeBlock block in this.TimeBlocks)
                {

                    if (timeFrame == 0)
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(FireTimeBlock), block);
                        timeFrame += block.Milliseconds;
                    }
                    else
                    {

                        tmr = new System.Timers.Timer(timeFrame);
                        tmr.Elapsed += tmr_Elapsed;
                        ActiveTimeBlocks.Enqueue(block);
                        tmr.Start();
                        ActiveTimers.Add(tmr);
                        timeFrame += block.Milliseconds;
         
                        tmr = null;

                    }
                }
                if (IsContinuous)
                {
                    tmr = new System.Timers.Timer(timeFrame);
                    tmr.AutoReset = false;
                    tmr.Elapsed += Restart_Elapsed;
                    ActiveTimers.Add(tmr);
                    tmr.Start();
                    tmr = null;
                }
                else
                {
                    tmr = new System.Timers.Timer(timeFrame);
                    tmr.AutoReset = false;
                    tmr.Elapsed += Done_Event;
                    ActiveTimers.Add(tmr);
                    tmr.Start();
                    tmr = null;
                }
            }
            finally
            {
                if (tmr != null)
                {
                    tmr.Stop();
                    if (ActiveTimers.Contains(tmr))
                    {
                        ActiveTimers.Remove(tmr);
                    }
                    tmr.Dispose();
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public event EventHandler DeactivateMe;
        void Done_Event(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            System.Timers.Timer tmr = sender as System.Timers.Timer;
            if (tmr != null)
            {
                tmr.Stop();
                tmr.Elapsed -= Restart_Elapsed;
                ActiveTimers.Remove(tmr);

                tmr.Dispose();

            }
            if (DeactivateMe != null)
            {
                DeactivateMe(this, EventArgs.Empty);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void Restart_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            System.Timers.Timer tmr = sender as System.Timers.Timer;
            if (tmr != null)
            {
                tmr.Stop();
                tmr.Elapsed -= Restart_Elapsed;
                ActiveTimers.Remove(tmr);

                tmr.Dispose();

            }
            Activate();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }


        void tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            System.Timers.Timer tmr = sender as System.Timers.Timer;
            if (tmr != null)
            {
                tmr.Stop();
                tmr.Elapsed -= tmr_Elapsed;
                ActiveTimers.Remove(tmr);

                tmr.Dispose();
                
            }
            FireTimeBlock(ActiveTimeBlocks.Dequeue());
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void FireTimeBlock(object state)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            FireTimeBlock(state as TimeBlock);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void FireTimeBlock(TimeBlock block)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (FireChannel != null)
            {
                foreach (SetValue value in block.Values)
                {
                    try
                    {
                        FireChannel(block, new FireChannelEventArgs(Priority, value.Channel, value.ChannelValue, block.Milliseconds, value.Delta));
                    }
                    catch (NullReferenceException)
                    {
                        break;
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        void TimeBlocks_ObjectChanged(object sender, EventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            SetChanged();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }



        public static readonly DependencyProperty PriorityProperty =
            DependencyProperty.Register("Priority", typeof(int),
            typeof(EventObject));

        public int Priority
        {
            get
            {
                return (int)this.UIThreadGetValue(PriorityProperty);

            }
            set
            {
                this.UIThreadSetValue(PriorityProperty, value);

            }
        }



        
        public static readonly DependencyProperty EventTypeProperty =
            DependencyProperty.Register("EventType", typeof(string),
            typeof(EventObject), new PropertyMetadata(OnItemChanged));
        [XmlConversion("type")]
        public string EventType
        {
            get
            {
                return (string)this.UIThreadGetValue(EventTypeProperty);

            }
            set
            {
                this.UIThreadSetValue(EventTypeProperty, value);

            }
        }

        static void OnIsContiuousChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            EventObject me = sender as EventObject;
            if (me != null)
            {
                if (!me.SettingContinuous)
                {
                    me.SettingContinuous=true;
                    me.Continuous = me.IsContinuous ? "yes" : "no";
                    me.SettingContinuous=false;
                }
                
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        
        public static readonly DependencyProperty IsContinuousProperty =
            DependencyProperty.Register("IsContinuous", typeof(bool),
            typeof(EventObject), new PropertyMetadata(OnIsContiuousChanged));
        
        public bool IsContinuous
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsContinuousProperty);

            }
            set
            {
                this.UIThreadSetValue(IsContinuousProperty, value);

            }
        }

      
        bool SettingContinuous =false ;
        static void OnContiuousChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            EventObject me = sender as EventObject;
            if (me != null)
            {
                if (!me.SettingContinuous)
                {
                    me.SettingContinuous=true;
                    me.IsContinuous = (me.Continuous.ToUpperInvariant() == "YES");

                    me.SettingContinuous=false;
                }
                OnItemChanged(sender, e);
                
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public static readonly DependencyProperty ContinuousProperty =
            DependencyProperty.Register("Continuous", typeof(string),
            typeof(EventObject), new PropertyMetadata(OnContiuousChanged));
        [XmlConversion("continuous")]
        public string Continuous
        {
            get
            {
                return (string)this.UIThreadGetValue(ContinuousProperty);

            }
            set
            {
                this.UIThreadSetValue(ContinuousProperty, value);

            }
        }

        public static readonly DependencyProperty TimeBlocksProperty =
          DependencyProperty.Register("TimeBlocks", typeof(TimeBlockCollection),
          typeof(EventObject), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("timeblock")]
        public TimeBlockCollection TimeBlocks
        {
            get
            {
                return (TimeBlockCollection)this.UIThreadGetValue(TimeBlocksProperty);

            }
            set
            {
                this.UIThreadSetValue(TimeBlocksProperty, value);

            }
        }


        protected override void ProcessValidation()
        {
          
        }

        public IList<System.Xml.XmlNode> Storage { get; private set; }
        bool isDisposed = false;
        protected virtual void Dispose(bool isDisposing)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (isDisposing)
            {
                if (!isDisposed)
                {
                    Stop();
                    isDisposed = true;
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
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
