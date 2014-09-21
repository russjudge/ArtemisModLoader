using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using RussLibrary.WPF;
using RussLibrary.Xml;
using System.Xml;

namespace VesselDataLibrary.Xml
{

    public class TorpedoStorageCollection : ChangeDependentCollection<TorpedoStorage>, IXmlStorage
    {
        public TorpedoStorageCollection()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Storage = new List<XmlNode>();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public override void AcceptChanges()
        {
            EndInit();
            base.AcceptChanges();
        }
       
       
        private void EndInit()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }

            List<TorpedoStorage> ObjectsToRemove = new List<TorpedoStorage>();

            //Remove invalid types.
            foreach (TorpedoStorage obj in this)
            {
                if (obj.TorpedoType < 0 || obj.TorpedoType > 3)
                {
                    ObjectsToRemove.Add(obj);
                }
            }

            foreach (TorpedoStorage item in ObjectsToRemove)
            {
                this.Remove(item);
            }

            ObjectsToRemove.Clear();


            //Make sure list is at least 4 elements.
            while (this.Count < 4)
            {
                TorpedoStorage obj = new TorpedoStorage();
                obj.TorpedoType = this.Count;
                this.Add(obj);
            }



            //Now make sure they are in correct order.
            TorpedoStorage[] itemsList = new TorpedoStorage[this.Count];
            this.CopyTo(itemsList, 0);
            Array.Sort<TorpedoStorage>(itemsList, new TorpedoStorageComparer());

            this.Clear();

            //1, 1, 2, 3
            //0, 0, 1, 1, 2, 2, 3, 3

            //Remove duplicates
            Dictionary<int, TorpedoStorage> newItems = new Dictionary<int, TorpedoStorage>();
            foreach (TorpedoStorage ts in itemsList)
            {
                if (!newItems.ContainsKey(ts.TorpedoType))
                {
                    newItems.Add(ts.TorpedoType, ts);
                }
            }


            //now add back to list, adding in any missing.
            for (int i = 0; i < 4; i++)
            {
                if (newItems.ContainsKey(i))
                {
                    this.Add(newItems[i]);
                }
                else
                {
                    TorpedoStorage ts = new TorpedoStorage();
                    ts.TorpedoType = i;
                    this.Add(ts);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static readonly ILog _log = LogManager.GetLogger(typeof(TorpedoStorageCollection));


        protected override void ProcessValidation()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }




            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        public IList<System.Xml.XmlNode> Storage { get; private set; }
    }
    class TorpedoStorageComparer : IComparer<TorpedoStorage>
    {
        #region IComparer<TorpedoStorage> Members

        public int Compare(TorpedoStorage x, TorpedoStorage y)
        {
            if (x != null && y != null)
            {
                return x.TorpedoType.CompareTo(y.TorpedoType);
            }
            else if (x == null)
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
                return -1;
            }
        }

        #endregion
    }
}

