using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace MissionStudio
{
    public class ValueElement : ICompletionData
    {
        public ValueElement(string value, string description)
        {
            Text = value;
            Description = description;
        }
        public void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea,
            ICSharpCode.AvalonEdit.Document.ISegment completionSegment, 
            EventArgs insertionRequestEventArgs)
        {
            if (textArea != null && textArea.Document != null)
            {
                textArea.Document.Replace(completionSegment, this.Text);
            }
        }
        
        public object Content
        {
            get { return this.Text; }
        }

        public object Description { get; set; }

        public System.Windows.Media.ImageSource Image { get; set; }

        public double Priority { get; set; }

        public string Text { get; private set; }
    }
}
