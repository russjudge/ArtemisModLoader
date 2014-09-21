using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Collections.ObjectModel;
using RussLibrary;

namespace MissionStudio
{
    public class CommandElement : XmlCompletionData
    {
        public CommandElement(string command, string description) : base(command,description)
        {


            Attributes = new ObservableCollection<AttributeElement>();
        }
        public ObservableCollection<AttributeElement> Attributes { get; private set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public List<XmlCompletionData> GetSortedAttributes()
        {

            List<XmlCompletionData> retVal = new List<XmlCompletionData>(Attributes);
            XmlCompletionDataComparer dc = new XmlCompletionDataComparer();
            retVal.Sort(dc);
            return retVal;
        }
        
        //public void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea,
        //    ICSharpCode.AvalonEdit.Document.ISegment completionSegment,
        //    EventArgs insertionRequestEventArgs)
        //{
        //    if (textArea != null && textArea.Document != null)
        //    {
        //        textArea.Document.Replace(completionSegment, this.Text);
        //    }
        //}

        //public object Content
        //{
        //    get { return this.Text; }
        //}

        //public object Description { get; private set; }

        //public System.Windows.Media.ImageSource Image { get; private set; }

        //public double Priority { get; private set; }

        //public string Text { get; private set; }
    }
}
