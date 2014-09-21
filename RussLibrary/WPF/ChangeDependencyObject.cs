using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;
using log4net;
using System.Collections;
using System.ComponentModel;

namespace RussLibrary.WPF
{
    public abstract class ChangeDependencyObject : DependencyObject, INotifyPropertyChanged
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ChangeDependencyObject));

        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected ChangeDependencyObject()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            //Application.Current.Dispatcher.Invoke(new Action(SetValidationObject));

            ValidationCollection = new ValidationObjectCollection();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void SetValidationObject()
        {
            ValidationCollection = new ValidationObjectCollection();
        }
        /// <summary>
        /// Begins the initialization.  Prevents "Changed" from being set to "true", and "ProcessValidation" will not get called.
        /// </summary>
        public virtual void BeginInitialization()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            IsInitializing = true;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        /// <summary>
        /// Ends the initialization.  Restores "Changed" being set to true, and ProcessValidation will be called.
        /// </summary>
        public virtual void EndInitialization()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            IsInitializing = false;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        protected abstract void ProcessValidation();
        protected bool IsInitializing { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void DoProcessValidation()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (Original != null)
            {
                try
                {
                    ValidationCollection.Clear();
                    ProcessValidation();
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("ValidationCollection"));
                    }
                }
                catch (Exception ex)
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.Warn("Exception processing validation", ex);
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        protected static void OnItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ChangeDependencyObject me = sender as ChangeDependencyObject;
            if (me != null && !me.IsInitializing)
            {
               
                if (e.NewValue != e.OldValue)
                {
                    
                    me.Changed = true;
                    me.DoProcessValidation();
                   
                }
               
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }



        public static readonly DependencyProperty TagProperty =
          DependencyProperty.Register("Tag", typeof(object),
          typeof(ChangeDependencyObject));

        public object Tag
        {
            get
            {
                return (object)this.UIThreadGetValue(TagProperty);

            }
            set
            {
                this.UIThreadSetValue(TagProperty, value);

            }
        }

        public event EventHandler ObjectChanged;

        static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ChangeDependencyObject me = sender as ChangeDependencyObject;
            if (me != null)
            {
                if (me.Changed)
                {
                    if ((bool)e.OldValue != (bool)e.NewValue)
                    {
                        if (me.ObjectChanged != null)
                        {
                            me.ObjectChanged(me, EventArgs.Empty);
                        }
                    }
                }
            }
        }

        public static readonly DependencyProperty ChangedProperty =
          DependencyProperty.Register("Changed", typeof(bool),
          typeof(ChangeDependencyObject), new PropertyMetadata(OnChanged));

        public bool Changed
        {
            get
            {
                return (bool)this.UIThreadGetValue(ChangedProperty);

            }
            private set
            {
                this.UIThreadSetValue(ChangedProperty, value);

            }
        }
        protected void SetChanged()
        {
            if (!IsInitializing)
            {
                Changed = true;
                DoProcessValidation();
            }
        }
        /// <summary>
        /// Holding property to contain the original state of current object.
        /// </summary>
        /// <value>
        /// The original.
        /// </value>
        public ChangeDependencyObject Original { get; protected set; }

        
        public virtual void AcceptChanges()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }

            try
            {
                Type[] t = new Type[0];
                
                if (this.GetType().GetConstructor(t) != null)
                {
                    Original = this.GetType().GetInstance() as ChangeDependencyObject;
                }
            }
            catch (System.ArgumentException ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Error initializing \"Original\"", ex);
                }
            }
            if (Original != null)
            {
                Original.BeginInitialization();
                Original.CopyProperties(this);
                Original.EndInitialization();
            }

            DoProcessValidation();

            Changed = false;



            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public virtual void RejectChanges()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (Original != null)
            {
                BeginInitialization();
                
               
                try
                {
                    this.CopyProperties(Original);
                }
                catch (Exception ex)
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.Warn("Error restoring \"Original\"", ex);
                    }
                }

                Changed = false;
                DoProcessValidation();
                EndInitialization();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public static readonly DependencyProperty ValidationCollectionProperty =
            DependencyProperty.Register("ValidationCollection", typeof(ValidationObjectCollection),
             typeof(ChangeDependencyObject));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ValidationObjectCollection ValidationCollection
        {
            get
            {
                return (ValidationObjectCollection)this.UIThreadGetValue(ValidationCollectionProperty);

            }
            set
            {
                this.UIThreadSetValue(ValidationCollectionProperty, value);

            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
       
       
    }
}
