using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows;
using System.Windows.Media;

namespace RussLibrary
{
    public class XmlCompletionData : ICompletionData
    {
        public XmlCompletionData()
        {
        }
        public XmlCompletionData(string text)
        {
            Initialize(text, null, text, null, double.NaN);

        }
        public XmlCompletionData(string text, string description)
        {
            Initialize(text, description, text, null, double.NaN);
        }
        public XmlCompletionData(string text, string description, object content)
        {
            Initialize(text, description, content, null, double.NaN);
        }
        public XmlCompletionData(string text, string description, object content, ImageSource image)
        {
            Initialize(text, description, content, image, double.NaN);
        }
        public XmlCompletionData(string text, string description, object content, ImageSource image, double priority)
        {
            Initialize(text, description, content, image, priority);
        }
        void Initialize(string text, string description, object content, ImageSource image, double priority)
        {
            Text = text;
            Description = description;
            Content = content;
            Image = image;
            Priority = priority;
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


        public object Content { get;  set; }

        public object Description { get;  set; }

        public System.Windows.Media.ImageSource Image { get;  set; }

        public double Priority { get;  set; }
        string _text = null;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (Content == null ||(Content != null && Content.ToString() == _text))
                {
                    Content = value;
                }
                _text = value;

            }
        }
    }
}
