using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Globalization;
using System.Xml;

namespace RussLibrary.Helpers
{

    public static class XmlHelper
    {
        public static bool IsAllNode(string text, int currentPosition)
        {
            if (!string.IsNullOrEmpty(text))
            {
                int start = text.LastIndexOf("<", currentPosition, StringComparison.OrdinalIgnoreCase);
                
                string wrk = text.Substring(start, currentPosition - start);
                return !wrk.Contains('>');
            }
            else
            {
                return true;
            }
        }
        static readonly ILog _log = LogManager.GetLogger(typeof(XmlHelper));
        
       
        public static string GetLastNode(string text, int currentPosition)
        {
            string retVal = null;
            //currentPosition valid for matching length of line.
            int endMark = currentPosition;
            if (!string.IsNullOrEmpty(text) && text.Length >= endMark)
            {
                int offSet = text.LastIndexOf("<", endMark, StringComparison.OrdinalIgnoreCase);
                if (offSet > -1)
                {
                    
                    retVal = text.Substring(offSet, endMark - offSet);
                }
            }
            return retVal;
        }
        public static bool IsWithinQuotes(string text, int offset)
        {
            bool retVal = false;
            
            if (string.IsNullOrEmpty(text) || offset >= text.Length)
            {
                retVal = false;
            }
            else
            {
                //have to analyze.  Go
                // until found: " or < or >. 
                for (int i = 0; i < offset; i++)
                {
                    if (text[i] == '\"')
                    {
                        retVal = !retVal;
                    }
                }
               
            }
            return retVal;
        }
        public static string GetNodeName(string node)
        {
            string retVal = node;
            if (!string.IsNullOrEmpty(node))
            {
                if (!node.StartsWith("<!--", StringComparison.OrdinalIgnoreCase))
                {
                    //string wrk = GetNode(node, node.Length - 1);  //Just to guarantee that there is nothing extra.
                    // will be either <node xx="x"> or </node> or <node xx="x" /> I want "node" part.
                    int i = 1;
                    if (node[i] == '/')
                    {
                        i++;
                    }
                    int j = i;
                    while (++i < node.Length && (char.IsLetterOrDigit(node[i]) || node[i] == '_') ) { }
                    retVal = node.Substring(j, i - j);
                }
                else
                {
                    retVal = null;
                }
            }
            return retVal;
        }
        ////public static int GetOffset(string text, int currentPosition, string match)
        ////{
        ////    //currentPosition valid for matching length of line.
        ////    int retVal = -1;
        ////    int endMark = currentPosition;
        ////    if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(match) && text.Length >= endMark)
        ////    {
        ////        int wrkPosition = endMark;
        ////        if (wrkPosition + match.Length > text.Length + 1)
        ////        {
        ////            wrkPosition = text.Length - match.Length + 1;
        ////        }
        ////        while (--wrkPosition > -1 && text.Substring(wrkPosition, match.Length) != match) { }
        ////        retVal = wrkPosition;
        ////    }
        ////    return retVal;
                
        ////}
        public static bool IsCompletedNode(string text)
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(text))
            {
                string LastNode = GetLastNode(text, text.Length);
                retVal = (LastNode.EndsWith("/>", StringComparison.OrdinalIgnoreCase)
                    || LastNode.StartsWith("</", StringComparison.OrdinalIgnoreCase));
            }
            return retVal;
        }
        public static string InsertText(string fullText, string insertionText, int currentPosition)
        {
            string retVal = fullText;
            if (!string.IsNullOrEmpty(fullText) && fullText.Length >= currentPosition)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(fullText.Substring(0, currentPosition));
                sb.Append(insertionText);
                if (currentPosition < fullText.Length)
                {
                    sb.Append(fullText.Substring(currentPosition));
                }
                retVal = sb.ToString();
            }
            return retVal;
        }




        //new stuff:
       
        public static bool IsInTag(string text, int offset)
        {
            bool InTag = false;
            if (!string.IsNullOrEmpty(text))
            {
                int start = -1;
                if (offset <= text.Length)
                {
                    start = text.LastIndexOf("<", offset, StringComparison.OrdinalIgnoreCase);
                }
                //<create x
                if (start > -1)
                {

                    int end = text.IndexOf(">", start, StringComparison.OrdinalIgnoreCase);
                    InTag = (end < 0 || end > offset);

                }
               
            }
            return InTag;
        }
        public static bool IsInComment(string text, int offset)
        {
            bool InComment = false;
            if (!string.IsNullOrEmpty(text))
            {
                int start = -1;
                if (offset <= text.Length)
                {
                    start = text.LastIndexOf("<!--", offset, StringComparison.OrdinalIgnoreCase);
                }
                if (start > -1)
                {
                    int end = text.IndexOf("-->", StringComparison.OrdinalIgnoreCase);
                    InComment = (end > offset || end < 0);
                    
                }
                if (!InComment)
                {
                    start = -1;
                    if (offset <= text.Length)
                    {
                        start = text.LastIndexOf("<?", offset, StringComparison.OrdinalIgnoreCase);
                    }
                    if (start > -1)
                    {
                        int end = text.IndexOf("?>", StringComparison.OrdinalIgnoreCase);
                        InComment = (end > offset || end < 0);
                    }
                }
            }
            return InComment;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static public string Beautify(string text)
        {
            string retVal = text;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(text);
                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.NewLineChars = "\r\n";
                settings.NewLineHandling = NewLineHandling.Replace;
                using (XmlWriter writer = XmlWriter.Create(sb, settings))
                {
                    doc.Save(writer);
                }
                retVal = sb.ToString();
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Exception beautifying Xml", ex);
                }
            }
            return retVal;
        }
        



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "log4net.ILog.InfoFormat(System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static int GetNestingCount(string text, int offset)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            
            int nestingLevel = 0;
            
            if (!string.IsNullOrEmpty(text))
            {

                if (offset > text.Length)
                {
                    offset = text.Length;
                }
                bool InTag = false;
                bool InQuote = false;
            
                int i = 0;
                string wrk = string.Empty;
                while (i < offset)
                {
                    List<int> values = new List<int>();
                    values.Add(text.IndexOf('<', i));
                    values.Add(text.IndexOf('\"', i));
                    values.Add(text.IndexOf('/', i));
                    values.Add(text.IndexOf('>', i));
                    values.Add(text.IndexOf('\n', i));


                    int o = text.Length;
                    foreach (int j in values)
                    {
                        if (j >= 0)
                        {
                            o = Math.Min(o, j);
                        }
                    }
                    if (o + 1 < text.Length)
                    {
                        if (IsInComment(text, o))
                        {
                            int p = text.IndexOf("-->", o, StringComparison.OrdinalIgnoreCase);
                            int q = p - i + 1;
                            if (q > text.Length || i + q >= text.Length)
                            {
                                q = text.Length - i;
                            }
                            wrk = text.Substring(i, q);
                            i = p;
                            o = p;
                        }
                        else
                        {
                            int hold = i;
                            int q = o - i + 1;
                            if (q > text.Length || i+q >= text.Length)
                            {
                                q = text.Length - i;
                            }
                            wrk = text.Substring(i, q);
                            i = o;
                            if (i < text.Length)
                            {
                                if (text[i] == '<' && !InQuote)
                                {
                                    bool YesIsComment = false;
                                    if (i + 3 < text.Length)
                                    {
                                        if (text.Substring(i, 4) == "<!--")
                                        {
                                            o = text.IndexOf("-->", i, StringComparison.OrdinalIgnoreCase) + 2;

                                            wrk = text.Substring(hold, o - hold + 1);
                                            i = o;
                                            YesIsComment = true;
                                            if (_log.IsInfoEnabled)
                                            {
                                                _log.InfoFormat("setting comment up, o={0}, i={1},wrk={2}", o.ToString(), i.ToString(), wrk);
                                            }
                                        }
                                    }
                                    if (!YesIsComment)
                                    {
                                        //If this is start of comment, find end of comment and adjust.
                                        InTag = true;
                                        nestingLevel++;
                                        if (_log.IsInfoEnabled)
                                        {
                                            _log.InfoFormat("NestingLevel Increased = {0}. '<', !InQuote, !InComment, o={1}, Setting InTag true, wrk={2}", nestingLevel.ToString(), o.ToString(), wrk);
                                        }
                                    }
                                }
                                else if (text[i] == '\"')
                                {
                                    InQuote = !InQuote;
                                    if (_log.IsInfoEnabled)
                                    {
                                        _log.InfoFormat("  quote found. InQuote={0}, i={1}", InQuote.ToString(), i.ToString());
                                    }
                                }
                                else if (InTag && text[i] == '>' && i > 0
                                    && (text[i - 1] == '/'
                                    || (i - 2 >= 0 && text[i - 1] == '-' && text[i - 2] == '-')
                                    || (i - 1 >= 0 && text[i - 1] == '?'))
                                    && !InQuote)
                                {
                                    InTag = false;
                                    nestingLevel--;
                                    if (_log.IsInfoEnabled)
                                    {
                                        _log.InfoFormat("NestingLevel Decreased = {0}. '>', InTag, i={2}, !InQuote, !InComment, o={1}, ({3}) Setting InTag false,wrk={4}", nestingLevel.ToString(), o.ToString(), i.ToString(), text.Substring(i - 2, 3), wrk);
                                    }
                                }
                                else if (InTag && !InQuote && text[i] == '>')
                                {
                                    if (text[text.LastIndexOf("<", i, StringComparison.OrdinalIgnoreCase) + 1] == '/')
                                    {
                                        nestingLevel--;
                                        if (_log.IsInfoEnabled)
                                        {
                                            _log.InfoFormat("NestingLevel Decreased = {0}. '>', InTag, !InQuote, !InComment, o={1}, is in endtag?,wrk={2}", nestingLevel.ToString(), o.ToString(), wrk);
                                        }
                                    }
                                    else
                                    {
                                        nestingLevel++;
                                        if (_log.IsInfoEnabled)
                                        {
                                            _log.InfoFormat("NestingLevel Increased = {0}. '>', InTag,  !InQuote, !InComment, o={1},wrk={2}", nestingLevel.ToString(), o.ToString(), wrk);
                                        }
                                    }
                                    InTag = false;
                                    nestingLevel--;
                                    if (_log.IsInfoEnabled)
                                    {
                                        _log.InfoFormat("NestingLevel Decreased = {0}. '>', InTag,  !InQuote, !InComment, o={1}, Setting InTag to false, wrk={2}", nestingLevel.ToString(), o.ToString(), wrk);
                                    }
                                }
                            }

                        }
                        if (_log.IsInfoEnabled)
                        {
                            _log.InfoFormat("Appending wrk: =>{0}<=", wrk);
                        }
                        if (text[i] == '\n')
                        {
                            bool YeahUndo = false;
                            if (nestingLevel > 0)
                            {
                                if (o + 2 < text.Length)
                                {
                                    string w = text.Substring(o + 1, 2);
                                    if (w == "</" && !IsInComment(text, o + 1))
                                    {
                                        nestingLevel--;
                                        if (_log.IsInfoEnabled)
                                        {
                                            _log.InfoFormat("NestingLevel Decreased = {0}. '</', LF, !InComment, o={1}, Setting YeahUndo to true", nestingLevel.ToString(), o.ToString());
                                        }
                                        YeahUndo = true;
                                    }
                                }
                                

                                if (YeahUndo)
                                {
                                    nestingLevel++;
                                    if (_log.IsInfoEnabled)
                                    {
                                        _log.InfoFormat("NestingLevel Increased = {0}, YeahUndo was true", nestingLevel.ToString());
                                    }

                                }
                            }
                        }
                        i = o + 1;
                        if (_log.IsInfoEnabled)
                        {
                            _log.InfoFormat("Setting i={0}", i.ToString());
                        }
                    }
                    else
                    {
                        i = text.Length;
                    }
                    //Make i the lesser of: " \n / > <

                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return nestingLevel;
        }

      
        //public static string GetWordBeforeDot(this TextEditor textEditor)
        //{
        //      string wordBeforeDot = string.Empty;
        //      if (textEditor != null)
        //      {
              
        //        int caretPosition = textEditor.CaretOffset - 2;

        //        int lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(caretPosition));

        //        string text = textEditor.Document.GetText(lineOffset, 1);

        //        // Get text backward of the mouse position, until the first space
        //        while (!string.IsNullOrWhiteSpace(text) &&  text.CompareTo(".") > 0)
        //        {
        //            wordBeforeDot = text + wordBeforeDot;

        //            if (caretPosition == 0)
        //                break;

        //            lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(--caretPosition));

        //            text = textEditor.Document.GetText(lineOffset, 1);
        //        }
        //    }
        //    return wordBeforeDot;
        //}

    }
}
