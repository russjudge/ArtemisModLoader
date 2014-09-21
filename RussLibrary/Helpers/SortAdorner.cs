using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Documents;

namespace RussLibrary.Helpers
{

    public class SortAdorner : Adorner
    {
        private readonly static Geometry _AscGeometry =
              Geometry.Parse("M 0,0 L 10,0 L 5,5 Z");

        private readonly static Geometry _DescGeometry =
            Geometry.Parse("M 0,5 L 10,5 L 5,0 Z");

        public ListSortDirection Direction { get; private set; }

        public SortAdorner(UIElement element, ListSortDirection dir)
            : base(element)
        {
            Initialize(dir, Brushes.LightSteelBlue);
        }
        public SortAdorner(UIElement element, ListSortDirection dir, Brush brushColor)
            : base(element)
        {
            Initialize(dir, brushColor);
        }
        void Initialize(ListSortDirection dir, Brush brushColor)
        {
            Direction = dir;
            BrushColor = brushColor;
        }
        public Brush BrushColor { get; set; }
        protected override void OnRender(DrawingContext drawingContext)
        {
           
                base.OnRender(drawingContext);

                if (AdornedElement.RenderSize.Width < 20)
                    return;
                if (drawingContext != null)
                {
                    drawingContext.PushTransform(
                         new TranslateTransform(
                           AdornedElement.RenderSize.Width - 15,
                          (AdornedElement.RenderSize.Height - 5) / 2));

                    drawingContext.DrawGeometry(BrushColor, null,
                        Direction == ListSortDirection.Ascending ?
                          _AscGeometry : _DescGeometry);

                    drawingContext.Pop();
                }
          
        }

    }
}
