using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using RussLibrary.WPF;
using RussLibrary;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using RussLibrary.Xml;
using System.Xml;

namespace VesselDataLibrary.Xml
{
    //[XmlConversionRoot("hullRace")]
    public class HullRaceCollection : ChangeDependentCollection<HullRace>, IXmlStorage
    {
        public HullRaceCollection()
        {

            Storage = new List<XmlNode>();
        }
        public void AddNewHullRace()
        {
           
            HullRace race = new HullRace();
           
            race.ID = this.Count;
            bool duplicateID = false;
            do
            {
                foreach (HullRace r in this)
                {
                    if (r.ID == race.ID)
                    {
                        duplicateID = true;
                        race.ID++;
                        break;
                    }
                }
            } while (duplicateID);

            this.Add(race);
            
        }

       
        protected override void ProcessValidation()
        {
            for (int i = 0; i < this.Count; i++)
            {
                for (int j = i + 1; j < this.Count; j++)
                {
                    if (this[i].ID == this[j].ID)
                    {
                        this[j].ValidationCollection.AddValidation("ID", ValidationValue.IsError,
                                Properties.Resources.DuplicateIDFound);
                        this[i].ValidationCollection.AddValidation("ID", ValidationValue.IsError,
                              Properties.Resources.DuplicateIDFound);
                    }
                }
            }
           
        }

        public IList<System.Xml.XmlNode> Storage { get; private set; }
    }
}
