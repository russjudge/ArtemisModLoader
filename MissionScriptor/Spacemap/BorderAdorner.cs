using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace MissionStudio.Spacemap
{

    public class BorderAdorner : Adorner
    {
        public BorderAdorner(UIElement adornedElement)
            : base(adornedElement)
        { }
        //static readonly ILog _log = LogManager.GetLogger(typeof(BorderAdorner));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        protected override void OnRender(
                System.Windows.Media.DrawingContext drawingContext)
        {
            if (drawingContext != null)
            {
                drawingContext.DrawRectangle(null, new Pen(Brushes.AliceBlue, 1),
                            new Rect(new Point(0, 0), DesiredSize));
                base.OnRender(drawingContext);
            }
        }
    }
}
