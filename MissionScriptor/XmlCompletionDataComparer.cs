using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary;

namespace MissionStudio
{
    public class XmlCompletionDataComparer :IComparer<XmlCompletionData>
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.CompareTo(System.String)")]
        public int Compare(XmlCompletionData x, XmlCompletionData y)
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
                    return x.Text.CompareTo(y.Text);
                }
            }
        }
    }
}
