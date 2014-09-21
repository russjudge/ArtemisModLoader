using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Collections.ObjectModel;
using RussLibrary;

namespace MissionStudio
{
    public class AttributeElement : XmlCompletionData
    {
        public AttributeElement(string name)
            : base(name)
        { }
        public string ParentCommand { get; set; }
        public AttributeType AttributeType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ObservableCollection<XmlCompletionData> Values { get; set; }

    }
}
