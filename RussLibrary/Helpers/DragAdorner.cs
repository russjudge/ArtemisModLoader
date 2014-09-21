using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace RussLibrary.Helpers
{

    public class DragAdorner : Adorner
    {
        private UIElement _child;
        //protected VisualBrush _brush;
        //private UIElement _owner;
        private UIElement _adornElement;

        public DragAdorner(UIElement owner) : base(owner) { }

        public DragAdorner(UIElement owner, UIElement adornElement, bool useVisualBrush, double opacity)
            : base(owner)
        {
            System.Diagnostics.Debug.Assert(owner != null);
            System.Diagnostics.Debug.Assert(adornElement != null);
            //_owner = owner;
            _adornElement = adornElement;
            if (useVisualBrush)
            {
                VisualBrush _brush = new VisualBrush(adornElement);
                _brush.Opacity = opacity;
                Rectangle r = new Rectangle();
                r.RadiusX = 3;
                r.RadiusY = 3;
                if (adornElement != null)
                {
               
                    r.Width = adornElement.DesiredSize.Width;
                    r.Height = adornElement.DesiredSize.Height;
                    Point mousepos = Mouse.GetPosition(adornElement);
                    DragHelper.SetRelativeMousePoint(adornElement, mousepos);
                }
                //XCenter = adornElement.DesiredSize.Width / 2;
                //YCenter = adornElement.DesiredSize.Height / 2;

                r.Fill = _brush;
                _child = r;

            }
            else
            {
                _child = adornElement;
            }

        }


        private double _leftOffset;
        public double LeftOffset
        {
            get { return _leftOffset; }
            set
            {
                _leftOffset = value - DragHelper.GetRelativeMousePoint(_adornElement).X;
                UpdatePosition();
            }
        }

        private double _topOffset;
        public double TopOffset
        {
            get { return _topOffset; }
            set
            {
                _topOffset = value - DragHelper.GetRelativeMousePoint(_adornElement).Y;

                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            AdornerLayer adorner = (AdornerLayer)this.Parent;
            if (adorner != null)
            {
                adorner.Update(this.AdornedElement);
            }
            else
            {
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }


        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return _child.DesiredSize;
        }
        protected override Size ArrangeOverride(Size finalSize)
        {

            _child.Arrange(new Rect(_child.DesiredSize));
            return finalSize;
        }


        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();

            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
            return result;
        }
    }
}
