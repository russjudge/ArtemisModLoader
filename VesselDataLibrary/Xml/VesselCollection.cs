using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using RussLibrary.WPF;
using ArtemisModLoader;
using System.Windows;
using RussLibrary;
using RussLibrary.Xml;
using System.Xml;
namespace VesselDataLibrary.Xml
{

    public class VesselCollection :  ChangeDependentCollection<Vessel>, IXmlStorage
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(VesselCollection));
        public VesselCollection()
        {
            Storage = new List<XmlNode>();
        }
        public Vessel AddNewVessel()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Vessel vessel = new Vessel();

            vessel.UniqueID = this.Count;
            bool duplicateID = false;
            do
            {
                foreach (Vessel r in this)
                {
                    if (r.UniqueID == vessel.UniqueID)
                    {
                        duplicateID = true;
                        vessel.UniqueID++;
                        break;
                    }
                }
            } while (duplicateID);

            this.Add(vessel);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return vessel;
        }

        protected override void ProcessValidation()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            for (int i = 0; i < this.Count; i++)
            {
                for (int j = i + 1; j < this.Count; j++)
                {
                    if (this[i].UniqueID == this[j].UniqueID)
                    {
                        this[j].ValidationCollection.AddValidation(DataStrings.UniqueID, ValidationValue.IsError,
                                AMLResources.Properties.Resources.DuplicateIDFound);
                        this[i].ValidationCollection.AddValidation(DataStrings.UniqueID, ValidationValue.IsError,
                              AMLResources.Properties.Resources.DuplicateIDFound);
                    }
                }
                
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }


        static void OnSortTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

            VesselCollection me = sender as VesselCollection;
            if (me != null)
            {
                switch (me.SortType)
                {
                    case VesselSortType.UniqueIDAscending:
                        VesselSort(me, CompareVesselsByIDAscending);
                        break;
                    case VesselSortType.UniqueIDDescending:
                        VesselSort(me, CompareVesselsByIDDescending);
                        break;
                    case VesselSortType.SideAscending:
                        VesselSort(me, CompareVesselsBySideAscending);
                        break;
                    case VesselSortType.SideDescending:
                        VesselSort(me, CompareVesselsBySideDescending);
                        break;
                }
                me.SetChanged();
            }
        }
        
        static void VesselSort(VesselCollection me, Comparison<Vessel> comparer)
        {
            if (me != null)
            {
                List<Vessel> vesselList = new List<Vessel>(me);
                vesselList.Sort(comparer);
                me.Clear();
                foreach (Vessel v in vesselList)
                {
                    me.Add(v);
                }
            }
        }
        public static readonly DependencyProperty SortTypeProperty =
            DependencyProperty.Register("SortType", typeof(VesselSortType),
            typeof(VesselCollection), new PropertyMetadata(OnSortTypeChanged));

        private static int CompareVesselsByIDAscending(Vessel x, Vessel y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    return x.UniqueID.CompareTo(y.UniqueID);
                }
            }
        }
        private static int CompareVesselsByIDDescending(Vessel x, Vessel y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (y == null)
                {
                    return -1;
                }
                else
                {
                    return y.UniqueID.CompareTo(x.UniqueID);
                }
            }
        }
        private static int CompareVesselsBySideAscending(Vessel x, Vessel y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    return x.Side.CompareTo(y.Side);
                }
            }
        }
        private static int CompareVesselsBySideDescending(Vessel x, Vessel y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (y == null)
                {
                    return -1;
                }
                else
                {
                    return y.Side.CompareTo(x.Side);
                }
            }
        }
        public VesselSortType SortType
        {
            get
            {
                return (VesselSortType)this.UIThreadGetValue(SortTypeProperty);

            }
            set
            {
                this.UIThreadSetValue(SortTypeProperty, value);

            }
        }

        public IList<System.Xml.XmlNode> Storage { get; private set; }
    }
}
