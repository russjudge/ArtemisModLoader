using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.WPF;
using RussLibrary;
using RussLibrary.Xml;
using System.Collections.ObjectModel;
using System.Windows;

namespace ArtemisModLoader.Xml
{
    public class SubModGroup : ChangeDependencyObject
    {
        public SubModGroup()
        {
            SubMods = new SubModCollection();
            SubMods.ObjectChanged += new EventHandler(SubMods_ObjectChanged);
        }

        void SubMods_ObjectChanged(object sender, EventArgs e)
        {
            this.SetChanged();
        }
        public override void AcceptChanges()
        {
            if (SubMods != null)
            {
                SubMods.AcceptChanges();
            }
            base.AcceptChanges();
        }
        public override void BeginInitialization()
        {
            base.BeginInitialization();
            if (SubMods != null)
            {
                SubMods.BeginInitialization();
                
            }
        }
        public override void EndInitialization()
        {
            if (SubMods != null)
            {
                SubMods.EndInitialization();
            }
            base.EndInitialization();
        }
        public override void RejectChanges()
        {
            if (SubMods != null)
            {
                SubMods.RejectChanges();
            }
            base.RejectChanges();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mods")]
        public static readonly DependencyProperty SubModsProperty =
            DependencyProperty.Register("SubMods", typeof(SubModCollection),
            typeof(SubModGroup));
     
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mods")]
        [XmlConversion("SubMod")]
        public SubModCollection SubMods
        {
            get
            {
                return (SubModCollection)this.UIThreadGetValue(SubModsProperty);

            }
            private set
            {
                this.UIThreadSetValue(SubModsProperty, value);

            }
        }


        protected override void ProcessValidation()
        {
          
        }
    }
}
